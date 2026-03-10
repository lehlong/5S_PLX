import { Injectable } from '@angular/core';
import { Filesystem, Directory } from '@capacitor/filesystem';
import { Capacitor } from '@capacitor/core';
import { MessageService } from '../message.service';
import { GlobalService } from '../global.service';

@Injectable({
  providedIn: 'root'
})
export class FileOfflineService {

  constructor(
    private messageService: MessageService,
    private _globalS : GlobalService
  ) { }

  // =========================================================
  // 1. Convert Blob → Base64
  // =========================================================
  blobToBase64(blob: Blob): Promise<string> {
    return new Promise((resolve, reject) => {
      const reader = new FileReader();
      reader.onload = () => resolve((reader.result as string).split(',')[1]);
      reader.onerror = () => reject('Lỗi đọc blob → base64');
      reader.readAsDataURL(blob);
    });
  }

  // =========================================================
  // 2. Convert Base64 → Blob
  // =========================================================
  base64ToBlob(base64: any, mime: string): Blob {
    const byteChars = atob(base64);
    const bytes = new Uint8Array(byteChars.length);
    for (let i = 0; i < byteChars.length; i++) {
      bytes[i] = byteChars.charCodeAt(i);
    }
    return new Blob([bytes], { type: mime });
  }

  // =========================================================
  // 3. Lấy extension file
  // =========================================================
  getFileExtension(blob: Blob): string {
    const mime = blob.type;
    if (!mime) return '.dat';

    const map: Record<string, string> = {
      'image/jpeg': '.jpg',
      'image/png': '.png',
      'image/webp': '.webp',

      'video/mp4': '.mp4',

      'application/pdf': '.pdf',

      'application/msword': '.doc',
      'application/vnd.openxmlformats-officedocument.wordprocessingml.document': '.docx',

      'application/vnd.ms-excel': '.xls',
      'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet': '.xlsx',

      'application/vnd.ms-powerpoint': 'ppt',
      'application/vnd.openxmlformats-officedocument.presentationml.presentation': '.pptx'
    };

    return map[mime] ?? mime.split('/')[1] ?? '.dat';
  }
  // =========================================================
  // 4. Tạo thumbnail ảnh
  // =========================================================
  async createThumbnail(blob: Blob, width = 200, height = 200): Promise<string> {
    return new Promise((resolve) => {
      const img = new Image();

      img.onload = () => {
        const canvas = document.createElement('canvas');
        canvas.width = width;
        canvas.height = height;

        const ctx = canvas.getContext('2d')!;
        ctx.drawImage(img, 0, 0, width, height);

        const base64 = canvas.toDataURL('image/jpeg').split(',')[1];
        resolve(base64);
      };

      img.src = URL.createObjectURL(blob);
    });
  }

  // =========================================================
  // 5. Lưu file vào Filesystem
  // =========================================================
  async saveFile(blob: any, folder: string) {
    // Tạo thư mục nếu chưa có 
    try {
      await Filesystem.mkdir({
        directory: Directory.Data,
        path: folder,
        recursive: true
      });
    } catch {
      // Folder đã có → bỏ qua 
    }
    const ext = this.getFileExtension(blob);
    const timestamp = Date.now();
    const fileName = blob.name 
                        ? blob.name 
                        : `${Date.now() + ext}`;
    const filePath = `${folder}/${fileName}`;
    const base64 = await this.blobToBase64(blob);
    // Ghi file gốc 
    await Filesystem.writeFile({
      directory: Directory.Data,
      path: filePath,
      data: base64
    }); 
    // Tạo thumbnail nếu là ảnh 
    let thumbPath = '';
    let thumbName = '';
    let viewUrl = '';
    if (blob.type.startsWith('image/')) {
      const thumbBase64 = await this.createThumbnail(blob, 100, 100);
      thumbName = `thumb_${timestamp}${ext}`;
      thumbPath = `${folder}/${thumbName}`;
      await Filesystem.writeFile({
        directory: Directory.Data,
        path: thumbPath,
        data: thumbBase64
      });
      const thumbUri = await Filesystem.getUri({
        directory: Directory.Data,
        path: thumbPath
      });
      viewUrl = Capacitor.convertFileSrc(thumbUri.uri);
    } else {
      const fileUri = await Filesystem.getUri({
        directory: Directory.Data,
        path: filePath
      });
      viewUrl = Capacitor.convertFileSrc(fileUri.uri);
    }
    return {
      filePath,
      pathThumbnail: thumbPath,
      fileName,
      nameThumbnail: thumbName,
      viewUrl,
      code: timestamp,
      type: ext,
      isBase64: true,
      isActive: true,
      isDeleted: false,
      evaluateHeaderCode: '',
      tieuChiCode: '',
      viDo: '0',
      kinhDo: '0',
      date: this._globalS.formatDateToSendServer(new Date())
    };
  }





  // async createThumbnailBlob(blob: Blob, maxWidth: number): Promise<Blob> {
  //   const img = new Image();
  //   img.src = URL.createObjectURL(blob);

  //   await new Promise(resolve => { img.onload = resolve });

  //   const scale = maxWidth / img.width;
  //   const w = maxWidth;
  //   const h = img.height * scale;

  //   const canvas = document.createElement('canvas');
  //   canvas.width = w;
  //   canvas.height = h;
  //   const ctx = canvas.getContext('2d')!;
  //   ctx.drawImage(img, 0, 0, w, h);

  //   return await new Promise(resolve => canvas.toBlob(resolve, 'image/jpeg', 0.8)) as Blob;
  // }




  // =========================================================
  // 6. Đọc file → Base64
  // =========================================================
  async readFileBase64(filePath: string) {
    const res = await Filesystem.readFile({
      path: filePath,
      directory: Directory.Data
    });
    return res.data;
  }

  // =========================================================
  // 7. Đọc file → Blob
  // =========================================================
  async readFileBlob(filePath: string | Blob, ext: string): Promise<Blob> {
    if (filePath instanceof Blob) return filePath;
    let mime = this.getMimeType(ext)
    if (typeof filePath === 'string' && filePath.startsWith('data:')) {
      const base64 = filePath.split(',')[1];
      return this.base64ToBlob(base64, mime);
    }

    const base64 = await this.readFileBase64(filePath);
    return this.base64ToBlob(base64, mime);
  }

  // =========================================================
  // 8. Lấy URL xem offline
  // =========================================================
  async getViewUrl(filePath: string) {
    const uri = await Filesystem.getUri({
      directory: Directory.Data,
      path: filePath
    });
    return Capacitor.convertFileSrc(uri.uri);
  }

  // =========================================================
  // 9. Xóa file
  // =========================================================
  async deleteFile(filePath: string) {
    try {
      await Filesystem.deleteFile({
        directory: Directory.Data,
        path: filePath
      });
    } catch {
      console.warn('Không tồn tại:', filePath);
    }
  }

  // =========================================================
  // 10. Xóa folder
  // =========================================================
  async deleteFolder(folder: string) {
    try {
      await Filesystem.rmdir({
        directory: Directory.Data,
        path: folder,
        recursive: true
      });
    } catch {
      console.warn('Không thể xóa folder:', folder);
    }
  }


  getMimeType(ext: string) {
    if (!ext) return "application/octet-stream";

    // 🔥 bỏ dấu . nếu có
    ext = ext.toLowerCase().replace('.', '');

    const map: any = {
      jpg: "image/jpeg",
      jpeg: "image/jpeg",
      png: "image/png",
      gif: "image/gif",
      webp: "image/webp",
      bmp: "image/bmp",

      pdf: "application/pdf",

      doc: "application/msword",
      docx: "application/vnd.openxmlformats-officedocument.wordprocessingml.document",

      xls: "application/vnd.ms-excel",
      xlsx: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",

      mp4: "video/mp4",
      mov: "video/quicktime",

      mp3: "audio/mpeg",
      wav: "audio/wav",
    };

    return map[ext] || "application/octet-stream";
  }

  async downloadFile(url: string, fileName: string) {
    console.log(url);

    // 1. Fetch file từ server
    const res = await fetch(url);
    const blob = await res.blob();
    console.log(blob);

    // 2. Convert blob → base64
    const base64 = await this.blobToBase64(blob);

    // 2. Đảm bảo fileName KHÔNG có dấu /
    fileName = fileName.replace(/[\\/]/g, '_');

    // 3. Ghi file (path PHẢI là file, không phải folder)
    await Filesystem.writeFile({
      directory: Directory.Documents,   // ✅ Chuẩn cho Android & iOS
      path: fileName,                   // ✅ ví dụ: report_20251214.pdf
      data: base64,
    });

    // 4. Lấy URL để mở / preview
    const uri = await Filesystem.getUri({
      directory: Directory.Documents,
      path: fileName,
    });
    this.messageService.show('Tải file THÀNH CÔNG.', 'success')
    return Capacitor.convertFileSrc(uri.uri);
  }
}
