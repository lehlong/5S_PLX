import {
  ChangeDetectorRef,
  Component,
  ElementRef,
  OnInit,
  Renderer2,
  ViewChild,
} from '@angular/core';
import { SharedModule } from '../../../../shared/shared.module';
import { Geolocation } from '@capacitor/geolocation';
import { Storage } from '@ionic/storage-angular';
import { AlertController } from '@ionic/angular';
import { IonAccordionGroup } from '@ionic/angular';
import { AppEvaluateService } from 'src/app/service/app-evaluate.service';
import { ActivatedRoute, Router } from '@angular/router';
import { StorageService } from 'src/app/service/storage.service';
import { environment } from 'src/environments/environment';
import { Camera, CameraResultType, CameraSource } from '@capacitor/camera';
import { Input, ChangeDetectionStrategy } from '@angular/core';
import { MessageService } from 'src/app/service/message.service';
import * as L from 'leaflet';
import { HighlightSearchPipe } from '../../../../shared/pipes/highlight-search.pipe';

@Component({
  imports: [SharedModule, HighlightSearchPipe],
  selector: 'app-evaluate',
  templateUrl: './evaluate.component.html',
  styleUrls: ['./evaluate.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class EvaluateComponent implements OnInit {
  isSearchVisible: boolean = false
  searchKeyword = '';
  searchResults: { id: string; type: string }[] = [];
  currentIndex = 0;
  @ViewChild('accordionGroup', { static: true })
  accordionGroup!: IonAccordionGroup;
  @ViewChild('fileInput', { static: false })
  fileInput!: ElementRef<HTMLInputElement>;

  private map: L.Map | undefined;
  selectedAccordionId: string = '';
  currentSelect: string = '';
  lstAllTieuChi: any = [];
  store: any = {};
  kiKhaoSat: any = {};
  lstTieuChi: any = [];
  @Input() treeData: any = [];
  @Input() lstTreeOpen!: string[];
  previewImage: any = [];
  evaluate: any = {
    header: {},
    lstEvaluate: [],
    lstImages: [],
  };
  data: any = {};
  headerId: any = '';
  count: any = 0;
  isEdit: any = true;
  apiFile = (environment as any).apiFile;
  account: any = {};
  dataTree: any = {
    leaves: [],
    tree: [],
  };
  private highlightClass = 'highlight-search';
  private currentHighlights: HTMLElement[] = [];
  lstHisEvaluate: any = []


  constructor(
    private route: ActivatedRoute,
    private alertController: AlertController,
    private _storageService: StorageService,
    private _service: AppEvaluateService,
    private messageService: MessageService,
    private cdr: ChangeDetectorRef,
    private renderer: Renderer2
  ) { }
  ngOnInit() {
    this.account = JSON.parse(localStorage.getItem('UserInfo') ?? '');
    this.route.paramMap.subscribe({
      next: async (params) => {
        this.headerId = params.get('code') ?? '';
        const mode = params.get('mode') ?? '';

        let nav = localStorage.getItem('filterCS');
        this.store = JSON.parse(nav ?? '').store;
        this.kiKhaoSat = JSON.parse(nav ?? '').kiKhaoSat;

        if (mode == 'draft') {
          this.evaluate = await this._storageService.get(this.store.id);
        } else {
          this.isEdit = false;
          console.log(mode);

          this.getResultEvaluate();
        }

        let data = localStorage.getItem(this.kiKhaoSat.name);
        if (data == null) {
          this.getAllTieuChi();
          this.getAllTieuChiLeaves();
        } else {
          this.dataTree = JSON.parse(data);
          this.treeData = this.dataTree?.tree;
          this.lstTreeOpen = [...this.extractAllKeys(this.treeData)];
          this.lstTieuChi = this.dataTree?.leaves;
          await this.cdr.detectChanges();
        }
      },
    });
  }

  //Hàm search
  openSearchInput() {
    this.isSearchVisible = !this.isSearchVisible;
  }
  private removeHighlights() {
    this.currentHighlights.forEach((el) => {
      this.renderer.removeClass(el, this.highlightClass);
    });
    this.currentHighlights = [];
  }

  onSearchChange() {
    this.removeHighlights();
    this.searchResults = [];
    this.currentIndex = 0;

    const keyword = this.searchKeyword?.trim().toLowerCase();
    if (!keyword) {
      this.cdr.detectChanges();
      return;
    }

    const searchTree = (nodes: any[]) => {
      nodes.forEach((node) => {
        if (node.title && node.title.toLowerCase().includes(keyword)) {
          this.searchResults.push({ id: node.id, type: 'accordion' });
        }

        if (!node.isGroup && node.diemTieuChi) {
          node.diemTieuChi.forEach((diem: any) => {
            if (diem.moTa && diem.moTa.toLowerCase().includes(keyword)) {
              this.searchResults.push({ id: node.id, type: 'item' });
            }
          });
        }

        if (node.isGroup && Array.isArray(node.children)) {
          searchTree(node.children);
        }
      });
    };

    searchTree(this.treeData);

    if (this.searchResults.length > 0) {
      this.scrollToAndHighlight(this.searchResults[0].id);
    }
    this.cdr.detectChanges();
  }

  goNext() {
    if (this.currentIndex < this.searchResults.length - 1) {
      this.currentIndex++;
      this.scrollToAndHighlight(this.searchResults[this.currentIndex].id);
    }
  }

  goPrev() {
    if (this.currentIndex > 0) {
      this.currentIndex--;
      this.scrollToAndHighlight(this.searchResults[this.currentIndex].id);
    }
  }

  scrollToAndHighlight(id: string) {
    this.removeHighlights();

    const el = document.getElementById(id);
    if (el) {
      el.scrollIntoView({ behavior: 'smooth', block: 'center' });
      this.renderer.addClass(el, this.highlightClass);
      this.currentHighlights.push(el);
    }
  }
  //--------------------------
  getResultEvaluate() {
    this._service.getResultEvaluate(this.headerId).subscribe({
      next: async (data) => {
        console.log(data);
        this.evaluate = data;
        await this.cdr.detectChanges();
      },
    });
  }

  getAllTieuChi() {
    this._service
      .buildDataTreeForApp(this.kiKhaoSat.id, this.store.id)
      .subscribe({
        next: (data) => {
          this.treeData = [data];
          this.lstTreeOpen = [...this.extractAllKeys([data])];

          this.dataTree.tree = this.treeData;
          console.log(this.dataTree);
          localStorage.setItem(
            this.kiKhaoSat.name,
            JSON.stringify(this.dataTree)
          );

          this.cdr.detectChanges();
        },
      });
  }

  getAllTieuChiLeaves() {
    this._service
      .GetAllTieuChiLeaves(this.kiKhaoSat.id, this.store.id)
      .subscribe({
        next: (data) => {
          this.lstTieuChi = data;

          this.dataTree.leaves = this.lstTieuChi;
          console.log(this.dataTree);
        },
      });
  }

  isValidChildren(children: any): boolean {
    return Array.isArray(children) && children.length > 0;
  }

  //Active
  setItem(itemId: string) {
    if (!this.isEdit) return;

    this.currentSelect = itemId;
    console.log(itemId);
    this.selectedAccordionId = itemId;
    const element = document.getElementById(itemId);
    if (element) {
      element.scrollIntoView({ behavior: 'smooth', block: 'start' });
    }
  }

  private initMap(): void {
    // L.Icon.Default.mergeOptions({
    //   iconRetinaUrl: 'assets/leaflet/marker-icon-2x.png',
    //   iconUrl: 'assets/leaflet/marker-icon.png',
    //   shadowUrl: 'assets/leaflet/marker-shadow.png',
    // });
    this.map = L.map('map').setView([21.0285, 105.8542], 13); // Hà Nội

    L.tileLayer('https://{s}.tile.openstreetmap.fr/osmfr/{z}/{x}/{y}.png', {
      attribution: '© OpenStreetMap France',
    }).addTo(this.map);

    L.marker([21.0285, 105.8542])
      .addTo(this.map)
      .bindPopup('Chưa có tọa độ')
      .openPopup();
  }

  isActive(itemId: string): boolean {
    return this.currentSelect === itemId;
  }

  borderActive(data: any): any {
    console.log('data-border', data.code);
  }

  isAnswered(data: any) {
    let hasEnoughImages = true;
    const evaluateItem = this.evaluate.lstEvaluate.find(
      (i: any) => i.tieuChiId === data.code || i.tieuChiCode === data.code
    );
    const hasPoint = !!evaluateItem && !!evaluateItem.pointId;
    if (data.isImg) {
      const numberImgRequired = data?.numberImg || 0;
      const hasImage = this.evaluate.lstImages.filter(
        (img: any) => img.tieuChiCode === data.code
      ).length;
      hasEnoughImages = hasImage >= numberImgRequired;
    }
    return hasPoint && hasEnoughImages;
  }

  hasEnoughImages(code: any, requiredNumber: any): boolean {
    const imagesSelecting = this.evaluate.lstImages.filter(
      (img: any) => img.tieuChiCode === code
    ).length;
    return imagesSelecting < requiredNumber;
  }

  extractAllKeys(tree: any[]): string[] {
    let keys: string[] = [];

    tree.forEach((node) => {
      if (node.key) {
        keys.push(node.key);
      }
      if (Array.isArray(node.children)) {
        keys = keys.concat(this.extractAllKeys(node.children));
      }
    });

    return keys;
  }

  onImageSelected(event: any, code: any) {
    if (!this.isEdit) return;

    const file: File = event.target.files[0];
    if (!file) return;
    const type = this.detectFileType(file);

    const reader = new FileReader();
    console.log(this.evaluate);

    reader.onload = () => {
      const base64 = reader.result as string;

      // Lưu vào localStorage
      this.evaluate.lstImages.push({
        code: '-1',
        fileName: file.name,
        filePath: base64,
        tieuChiCode: code,
        type: type,
        kinhDo: 0,
        viDo: 0,
        evaluateHeaderCode: this.headerId,
      });
      this.cdr.detectChanges();
      this._storageService.set(this.store.id, this.evaluate);
    };
    reader.readAsDataURL(file); // Chuyển sang base64
  }

  generateThumbnail(
    base64: string,
    maxWidth: number,
    maxHeight: number
  ): Promise<string> {
    return new Promise((resolve) => {
      const img = new Image();
      img.onload = () => {
        const canvas = document.createElement('canvas');
        const ctx = canvas.getContext('2d')!;
        let width = img.width;
        let height = img.height;

        // Tính toán tỷ lệ thu nhỏ
        if (width > height) {
          if (width > maxWidth) {
            height *= maxWidth / width;
            width = maxWidth;
          }
        } else {
          if (height > maxHeight) {
            width *= maxHeight / height;
            height = maxHeight;
          }
        }

        canvas.width = width;
        canvas.height = height;
        ctx.drawImage(img, 0, 0, width, height);
        resolve(canvas.toDataURL()); // Trả về base64 ảnh nhỏ
      };
      img.src = base64;
    });
  }

  detectFileType(file: File): string {
    const mime = file.type;

    if (mime.startsWith('image/')) return 'img';
    if (mime.startsWith('video/')) return 'mp4';
    if (mime === 'application/pdf') return 'pdf';
    if (
      mime ===
      'application/vnd.openxmlformats-officedocument.wordprocessingml.document'
    )
      return 'docx';
    if (
      mime ===
      'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
    )
      return 'xlsx';
    if (mime === 'application/vnd.ms-excel.sheet.macroEnabled.12')
      return 'xlsm';
    if (mime === 'application/vnd.ms-powerpoint') return 'ppt';
    if (
      mime ===
      'application/vnd.openxmlformats-officedocument.presentationml.presentation'
    )
      return 'pptx';

    return 'other';
  }

  filterDiem(code: any) {
    const item = this.evaluate?.lstEvaluate?.find(
      (x: any) => x.tieuChiCode === code
    );
    return item?.pointId || null;
  }

  filterImage(node: any) {
    if (node.isGroup == true) return;

    const data = Array.isArray(this.evaluate?.lstImages)
      ? this.evaluate.lstImages.filter((x: any) => x.tieuChiCode === node.code)
      : null;
    if (!data) return;

    data.map((x: any) => ({
      filePath: x.filePath,
      type: x.type,
    }));

    const images = data.filter((x: any) => x.type === 'img' || x.type == 'mp4');
    // const videos = data.filter((x: any) => x.type === 'mp4');
    const documents = data.filter((x: any) =>
      ['docx', 'xlsx', 'xlsm', 'pdf'].includes(x.type)
    );

    // videos: videos,
    return {
      images: images,
      documents: documents,
    };
  }

  filterFeedBack(code: any) {
    const item = this.evaluate?.lstEvaluate?.find(
      (x: any) => x.tieuChiCode === code
    );
    return item?.feedBack || '';
  }

  setDiem(data: any, event: any) {
    if (!this.isEdit) return;

    const selected = event.detail.value;
    const idx = this.evaluate.lstEvaluate.findIndex(
      (i: any) => i.tieuChiCode === data.code
    );
    // console.log(data.diemTieuChi.filter((i: any) => i.id == selected)[0].diem);

    if (idx === -1) return;

    this.evaluate.lstEvaluate[idx].point = data.diemTieuChi.filter(
      (i: any) => i.id == selected
    )[0].diem;
    this.evaluate.lstEvaluate[idx].pointId = selected;
    this.tinhTong();
  }

  tinhTong() {
    const tongDiem = this.lstTieuChi.reduce((sum: number, leaf: any) => {
      const diemMax = (leaf.diemTieuChi || []).reduce((max: number, d: any) => {
        return Math.max(max, d.diem || 0);
      }, 0);
      return sum + diemMax;
    }, 0);
    this.evaluate.header.point = ((this.evaluate.lstEvaluate.reduce(
      (sum: any, item: any) => sum + (item.point || 0),
      0) / tongDiem) * 100
    ).toFixed(2);

    this._storageService.set(this.store.id, this.evaluate);
  }

  setFeedBack(data: any, event: any) {
    if (!this.isEdit) return;

    const selected = event.detail.value;
    const idx = this.evaluate.lstEvaluate.findIndex(
      (i: any) => i.tieuChiCode === data
    );
    if (idx === -1) return;

    this.evaluate.lstEvaluate[idx].pointId = selected;
    this._storageService.set(this.store.id, this.evaluate);
  }

  async onSubmit() {
    if (!this.isEdit) return;

    let allChecksPassed = true;
    let errorMessage = '';

    for (const tieuChi of this.lstTieuChi) {
      const evaluateItem = this.evaluate.lstEvaluate.find(
        (i: any) => i.tieuChiId === tieuChi.id || i.tieuChiCode === tieuChi.code
      );

      // 1. Kiểm tra pointId
      if (!evaluateItem || !evaluateItem.pointId) {
        errorMessage += `- Tiêu chí "${tieuChi.name}" chưa chấm điểm. `;
        allChecksPassed = false;
      }

      // Kiểm tra có đủ ảnh không
      if (tieuChi.isImg) {
        const numberImgRequired = tieuChi.numberImg || 0;
        const imagesSelecting = this.evaluate.lstImages.filter(
          (img: any) => img.tieuChiCode === tieuChi.code
        ).length;
        if (imagesSelecting < numberImgRequired) {
          errorMessage += `- Tiêu chí "${tieuChi.name}" thiếu ảnh. `;
          allChecksPassed = false;
        }
      }
    }

    if (!allChecksPassed) {
      const alert = await this.alertController.create({
        header: 'Thiếu thông tin',
        message: errorMessage,
        buttons: ['OK'],
      });
      await alert.present();
      return;
    }
        // this.tinhTongLanCham()

    // Trường hợp đủ
    this.evaluate.header.accountUserName = this.account.userName
    this.evaluate.header.chucVuId = this.account.chucVuId;

    this._service.insertEvaluate(this.evaluate).subscribe({
      next: () => {
        console.log('Chấm điểm thành công');
        this.tinhTongLanCham()

        this.messageService.show(`Chấm điểm Cửa hàng thành công`, 'success');
        this._storageService.remove(this.store.id);
        localStorage.removeItem(this.kiKhaoSat.code)
      },

      error: (ex) => {
        console.log(ex);
      },
    });
  }


  tinhTongLanCham() {
    this._service.filterLstChamDiem({ sortColumn: this.store.id, keyWord: this.kiKhaoSat.id }).subscribe({
      next: (data) => {
        console.log(data.length);

        const total = data.reduce((sum: any, item: any) => {
          let pointitem =  item.point
          if(item.ChucVuId === "CHT" || item.ChucVuId === "ATVSV"){
            pointitem =  item.point / 2
            console.log(pointitem);
          }
            console.log(pointitem);

          return sum + pointitem;
        }, 0);
        const avg = data.length >= 0 ? total / data.length : 0;

        const point = {
          code: '',
          inStoreId: this.store.id,
          surveyId: localStorage.getItem('surveyId'),
          kiKhaoSatId: this.kiKhaoSat.id,
          point: avg,
          length: data.length
        }
        this._service.tinhTongLanCham(point).subscribe({
          next: (data) => {
            console.log("tính tổng điểm thành công");

          }
        })
      }
    })
  }

  navigateTo(itemId: string) {
    const currentUrl = window.location.href.split('#')[0]; // Lấy URL hiện tại mà không có hash
    return `${currentUrl}#${itemId}`;
  }

  isImageModalOpen = false;
  selectedImage: any = {};

  openFullScreen(img: any) {
    if (this.isEdit == false) {
      img.filePath = this.apiFile + img.filePath;
    }
    this.selectedImage = img;
    this.isImageModalOpen = true;
    setTimeout(() => {
      this.initMap();
    }, 300);
  }

  filePath(filePath: string) {
    if (this.isEdit) return filePath;
    return this.apiFile + filePath;
  }

  closeFullScreen() {
    this.isImageModalOpen = false;
  }

  deleteImage() {
    if (!this.isEdit) return;

    const index = this.evaluate.lstImages.findIndex(
      (img: any) => img.filePath === this.selectedImage.filePath
    );
    if (index > -1) {
      this.evaluate.lstImages.splice(index, 1);
    }
    this._storageService.set(this.store.id, this.evaluate);
    this.cdr.detectChanges();

    this.closeFullScreen();
  }

  deleteImage2(img: any) {
    console.log('deleteImage2', img);
    this.selectedImage = img;
    this.confirmDeleteImage();
  }

  async confirmDeleteImage() {
    if (!this.isEdit) return;

    const alert = await this.alertController.create({
      header: 'Xác nhận',
      message: 'Bạn có chắc muốn xóa ảnh này?',
      buttons: [
        {
          text: 'Hủy',
          role: 'cancel',
        },
        {
          text: 'Xóa',
          role: 'destructive',
          handler: () => {
            this.deleteImage();
          },
        },
      ],
    });

    await alert.present();
  }

  feedback: string = '';

  async openCamera(code: any) {
    if (!this.isEdit) return;

    try {
      // 👉 Lấy vị trí hiện tại
      const position = await Geolocation.getCurrentPosition();
      const latitude = position.coords.latitude;
      const longitude = position.coords.longitude;

      const image = await Camera.getPhoto({
        quality: 90,
        allowEditing: false,
        resultType: CameraResultType.Base64,
        source: CameraSource.Camera,
      });

      const base64Image = `data:image/jpeg;base64,${image.base64String}`;

      this.evaluate.lstImages.push({
        code: '-1',
        fileName: '',
        filePath: base64Image,
        tieuChiCode: code,
        viDo: latitude,
        kinhDo: longitude,
        type: 'img',
      });
      this.cdr.detectChanges();

      this._storageService.set(this.store.id, this.evaluate);
    } catch (err) {
      console.error('Camera error:', err);
    }
  }

  trackByKey(index: number, item: any): string {
    return item.key; // hoặc item.id nếu bạn dùng id
  }
}
