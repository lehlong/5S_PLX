import { Injectable } from '@angular/core';
import { Filesystem, Directory } from '@capacitor/filesystem';
import { Capacitor } from '@capacitor/core';

@Injectable({
  providedIn: 'root'
})
export class FileOfflineService {

  constructor() {}

  // =========================================================
  // 1. Convert Blob → Base64
  // =========================================================
  blobToBase64(blob: Blob): Promise<string> {
    return new Promise((resolve, reject) => {
      const reader = new FileReader();
      reader.onload = () => resolve((reader.result as string).split(',')[1]);
      reader.onerror = reject;
      reader.readAsDataURL(blob);
    });
  }

  // =========================================================
  // 2. Convert Base64 → Blob
  // =========================================================
  base64ToBlob(base64: any, mime: string): Blob {
    const byteChars = atob(base64);
    const byteNumbers = new Array(byteChars.length);
    for (let i = 0; i < byteChars.length; i++) {
      byteNumbers[i] = byteChars.charCodeAt(i);
    }
    return new Blob([new Uint8Array(byteNumbers)], { type: mime });
  }

  // =========================================================
  // 3. Lấy extension file từ Blob
  // =========================================================
  getFileExtension(blob: Blob): string {
    const mime = blob.type;
    if (!mime) return 'dat';

    const ext = mime.split('/')[1];
    if (ext.indexOf('+') > -1) return ext.split('+')[0];

    return ext;
  }

  // =========================================================
  // 4. Tạo thumbnail cho ảnh
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

        resolve(canvas.toDataURL('image/jpeg').split(',')[1]);
      };

      img.src = URL.createObjectURL(blob);
    });
  }

  // =========================================================
  // 5. Lưu file vào Filesystem
  // =========================================================
  async saveFile(blob: Blob, folder: string) {
    try {
      await Filesystem.mkdir({
        directory: Directory.Data,
        path: folder,
        recursive: true
      });
    } catch {}

    const ext = this.getFileExtension(blob);
    const timestamp = Date.now();
    const fileName = `${timestamp}.${ext}`;
    const filePath = `${folder}/${fileName}`;

    const base64 = await this.blobToBase64(blob);

    await Filesystem.writeFile({
      directory: Directory.Data,
      path: filePath,
      data: base64
    });

    // Nếu là ảnh → tạo thumbnail
    let thumbPath = '';
    let thumbName = ''
    let thumbBase64 = '';
    let viewUrl = '';

    if (blob.type.startsWith('image/')) {
      thumbBase64 = await this.createThumbnail(blob, 100, 100);
      thumbName = `thumb_${timestamp}.${ext}`
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
    }

    return {
      filePath: filePath,
      pathThumbnail: thumbPath,
      fileName: fileName,
      nameThumbnail: thumbName,
      viewUrl: viewUrl,
      code: timestamp,
      type: ext,
      isBase64: true,
      isActive: true,
      isDeleted: false,
      evaluateHeaderCode: '',
      tieuChiCode: '',
      viDo: '0',
      kinhDo: '0'

    };
  }

  // =========================================================
  // 6. Đọc file → Base64
  // =========================================================
  async readFileBase64(filePath: string){
    const res = await Filesystem.readFile({
      path: filePath,
      directory: Directory.Data
    });
    return res.data;
  }

  // =========================================================
  // 7. Đọc file → Blob
  // =========================================================
 async readFileBlob(filePath: string | Blob, mime: string): Promise<Blob> {

  // Nếu filePath đã là Blob → chỉ return
  if (filePath instanceof Blob) {
    return filePath;
  }

  // Nếu là base64 → cũng convert
  if (filePath.startsWith("data:")) {
    const base64 = filePath.split(",")[1];
    return this.base64ToBlob(base64, mime);
  }

  // Còn lại là đường dẫn → đọc filesystem
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
    } catch {}
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
    } catch {}
  }
}
