import {
  ChangeDetectorRef,
  Component,
  ElementRef,
  OnInit,
  Renderer2,
  ViewChild,
} from '@angular/core';
import { SharedModule } from '../../../shared/shared.module';
import { Geolocation } from '@capacitor/geolocation';
import { AlertController } from '@ionic/angular';
import { IonAccordionGroup } from '@ionic/angular';
import { AppEvaluateService } from 'src/app/service/app-evaluate.service';
import { ActivatedRoute, Router } from '@angular/router';
import { StorageService } from 'src/app/service/storage.service';
import { environment } from 'src/environments/environment';
import { LoadingController } from '@ionic/angular';
import { Camera, CameraResultType, CameraSource } from '@capacitor/camera';
import { Input, ChangeDetectionStrategy } from '@angular/core';
import { MessageService } from 'src/app/service/message.service';
import * as L from 'leaflet';
import { HighlightSearchPipe } from '../../../shared/pipes/highlight-search.pipe';
import { AuthService } from 'src/app/service/auth.service';
import mediumZoom from 'medium-zoom';
import { Directory, Filesystem } from '@capacitor/filesystem';
import { Capacitor } from '@capacitor/core';
import { FileOfflineService } from 'src/app/service/common/systemfile.service';
import { firstValueFrom } from 'rxjs';

@Component({
  imports: [SharedModule, HighlightSearchPipe],
  selector: 'app-evaluate',
  templateUrl: './evaluate.component.html',
  styleUrls: ['./evaluate.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class EvaluateComponent implements OnInit {
  @ViewChild('zoomImg') zoomImg!: ElementRef;
  @ViewChild('accordionGroup', { static: true })
  private currentScale = 1
  @ViewChild('fileInput', { static: false })
  private lastTapTime = 0;
  @Input() treeData: any = [];
  @Input() lstTreeOpen!: string[];
  private doubleTapThreshold = 300;
  private isZoomed = false;
  private isDragging = false;
  private lastX = 0;
  private lastY = 0;
  private currentX = 0;
  private currentY = 0;
  private map: L.Map | undefined;
  private highlightClass = 'highlight-search';
  private currentHighlights: HTMLElement[] = [];
  ////---------------
  accordionGroup!: IonAccordionGroup;
  fileInput!: ElementRef<HTMLInputElement>;
  isSearchVisible: boolean = false;
  searchKeyword = '';
  searchResults: { id: string; type: string }[] = [];
  currentIndex = 0;
  tieuChiIndex = 0;
  selectedAccordionId: string = '';
  currentSelect: string = '';
  lstAllTieuChi: any = [];
  doiTuong: any = {};
  kiKhaoSat: any = {};
  lstTieuChi: any = [];
  previewImage: any = [];
  data: any = {};
  headerId: any = '';
  count: any = 0;
  leaveIndex: any = 1;
  isEdit: any = true;
  dateNow: Date = new Date();
  account: any = {};
  longitude: number = 106.6297;
  latitude: number = 10.8231;
  daCham: any = 0;
  chuaCham: any = 0;
  isImageModalOpen = false;
  selectedImage: any = {};
  lstHisEvaluate: any = [];
  apiFile = environment.apiFile;
  lstAccout: any = [];
  evaluate: any = {
    header: {},
    lstEvaluate: [],
    lstImages: [],
  };
  lstAllImages: any = []
  dataTree: any = {
    leaves: [],
    tree: [],
  };

  constructor(
    private route: ActivatedRoute,
    private alertController: AlertController,
    private _storageService: StorageService,
    private _service: AppEvaluateService,
    private _authService: AuthService,
    private messageService: MessageService,
    private loadingController: LoadingController,
    private cdr: ChangeDetectorRef,
    private renderer: Renderer2,
    private _systemFileS: FileOfflineService,
    private router: Router
  ) { }

  async ngOnInit() {
    const api = localStorage.getItem('CapacitorStorage.apiUrl') ?? '';
    this.apiFile = api.replace(/\/api$/, '/');
    this.account = JSON.parse(localStorage.getItem('UserInfo') ?? '');

    this.route.paramMap.subscribe({
      next: async (params) => {
        this.headerId = params.get('code') ?? '';
        const mode = params.get('mode') ?? '';

        let nav = localStorage.getItem('filterCS');
        this.doiTuong = JSON.parse(nav ?? '').doiTuong;
        this.kiKhaoSat = JSON.parse(nav ?? '').kiKhaoSat;

        setTimeout(async () => {
          if (this.evaluate != null) {

            let data = localStorage.getItem(
              this.doiTuong.id + '_' + this.kiKhaoSat.code
            );
            if (data == null) {
              this.getAllTieuChi();
            } else {
              this.dataTree = JSON.parse(data);
              // console.log(this.dataTree);

              this.treeData = this.dataTree?.tree;
              this.lstTieuChi = this.dataTree?.leaves;
              this.lstTreeOpen = [...this.getKeysAndLeaves(this.treeData).keys];
              await this.cdr.detectChanges();
            }
          } else {
            console.log(this.evaluate);
          }
        }, 500);

        if (mode == 'draft') {
          this.evaluate = await this._storageService.get(
            this.doiTuong.id + '_' + this.kiKhaoSat.code
          );
          console.log(this.evaluate);
          if (this.evaluate === null || this.evaluate === undefined) {
            this.router.navigate([`/survey/check-list/${this.doiTuong.id}`]);
          }

        } else {
          this.isEdit = false;
          console.log("Xem");

          this.getResultEvaluate();
          this.getAllAccount();
        }
      },
    });
  }

  ngAfterViewInit() {
    mediumZoom('.zoom-image');
  }

  /////////Hàm search

  openSearchInput() {
    this.isSearchVisible = !this.isSearchVisible;
  }

  private removeHighlights() {
    this.currentHighlights.forEach((el) => {
      this.renderer.removeClass(el, this.highlightClass);
    });
    this.currentHighlights = [];
  }


  getResultEvaluate() {
    this._service.getResultEvaluate(this.headerId).subscribe({
      next: async (data) => {
        this.evaluate = data;
      },
    });
  }

  getAllTieuChi() {
    this._service
      .buildDataTreeForApp(this.kiKhaoSat.id, this.doiTuong.id)
      .subscribe({
        next: async (data) => {
          this.treeData = [data];
          const result = this.getKeysAndLeaves([data]);

          this.lstTreeOpen = [...result.keys];
          this.lstTieuChi = result.leaves;

          this.dataTree.leaves = this.lstTieuChi;
          this.dataTree.tree = this.treeData;

          localStorage.setItem(
            this.doiTuong.id + '_' + this.kiKhaoSat.code,
            JSON.stringify(this.dataTree)
          );

          this.cdr.detectChanges();
        },
      });
  }

  getKeysAndLeaves(tree: any[]): { keys: string[]; leaves: any[] } {
    let keys: string[] = [];
    let leaves: any[] = [];

    const traverse = (nodes: any[]) => {
      for (const node of nodes) {
        if (node.key) keys.push(node.key);

        if (!node.isGroup) {
          node.number = this.leaveIndex++;

          const evalItem = this.evaluate.lstEvaluate.find(
            (x: any) => x.tieuChiCode === node.code
          );

          if (evalItem) {
            node.pointId = evalItem.pointId ?? null;
            node.point = evalItem.point ?? 0;
          }
          leaves.push(node);
        }

        if (Array.isArray(node.children) && node.children.length > 0) {
          traverse(node.children);
        }
      }
    };

    traverse(tree);
    return { keys, leaves };
  }

  isValidChildren(children: any): boolean {
    return Array.isArray(children) && children.length > 0;
  }

  //Active
  setItem(itemId: string) {
    this.currentSelect = itemId;
    this.selectedAccordionId = itemId;
    const element = document.getElementById(itemId);
    if (element) {
      element.scrollIntoView({ behavior: 'smooth', block: 'start' });
    }
  }

  private initMap(): void {
    L.Icon.Default.mergeOptions({
      iconRetinaUrl: 'assets/media/marker-icon.png',
      iconUrl: 'assets/media/marker-icon.png',
      shadowUrl: 'assets/media/marker-shadow.png',
    });

    const lat = this.latitude; // Vĩ độ động
    const lng = this.longitude; // Kinh độ động

    this.map = L.map('map').setView([lat, lng], 17);

    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
      attribution: '© OpenStreetMap contributors',
      maxZoom: 19,
    }).addTo(this.map);

    L.marker([lat, lng]).addTo(this.map).openPopup();
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
    const diem =
      this.lstTieuChi
        .flatMap((t: any) => t.diemTieuChi || [])
        .find((d: any) => d.id === evaluateItem?.pointId)?.diem ?? '';

    if (data.isImg) {
      if (
        data.chiChtAtvsv &&
        !(this.account.chucVuId === 'CHT' || this.account.chucVuId === 'ATVSV')
      ) {
        hasEnoughImages = true;
      } else {
        const numberImgRequired = data?.numberImg || 0;
        const hasImage = this.evaluate?.lstImages.filter(
          (img: any) => img.tieuChiCode === data.code
        ).length;
        hasEnoughImages = hasImage >= numberImgRequired;
      }
    }
    if (diem === '' || !hasEnoughImages) return '';

    return diem > 0 ? 'answered' : 'red-false';
  }

  hasEnoughImages(code: any, node: any): boolean {
    if (node.chiChtAtvsv) {
      if (this.account.chucVuId != 'CHT' || this.account.chucVuId != 'ATVSV')
        return false;
    }
    const imagesSelecting = this.evaluate?.lstImages.filter(
      (img: any) => img.tieuChiCode === code
    ).length;
    return imagesSelecting < node.numberImg;
  }



  filterImage(node: any) {
    if (node.isGroup == true) return;

    const data = Array.isArray(this.evaluate?.lstImages)
      ? this.evaluate?.lstImages.filter((x: any) => x.tieuChiCode === node.code)
      : null;
    if (!data) return;

    data.map((x: any) => ({
      filePath: x.filePath,
      type: x.type,
    }));

    const images = data.filter((x: any) =>
      [".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp", "img", "mp4", ".mp4", "jpg", "jpeg", "png", "gif", "bmp", "webp"].includes(x.type)
    )
    const documents = data.filter((x: any) =>
      ['docx', 'xlsx', 'xlsm', 'pdf', 'doc', '.doc', '.docx', '.xlsx', '.xlsm', '.pdf'].includes(x.type)
    );

    return {
      images: images,
      documents: documents,
    };
  }

  setDiem(data: any, event: any) {
    if (!this.isEdit) return;

    const selected = event.detail.value;
    const idx = this.evaluate.lstEvaluate.findIndex(
      (i: any) => i.tieuChiCode === data.code
    );
    if (idx === -1) return;
    const found = data.diemTieuChi.find((i: any) => i.id == selected);

    this.evaluate.lstEvaluate[idx].point = found?.diem ?? 0;
    this.evaluate.lstEvaluate[idx].pointId = selected;

    this.autoSave()
  }

  tinhTong() {
    const tongDiem = this.lstTieuChi.reduce((sum: number, leaf: any) => {
      const diemMax = (leaf.diemTieuChi || []).reduce((max: number, d: any) => {
        return Math.max(max, d.diem || 0);
      }, 0);
      return sum + diemMax;
    }, 0);

    this.evaluate.header.point = (
      (this.evaluate.lstEvaluate.reduce(
        (sum: any, item: any) => sum + (item.point || 0),
        0
      ) /
        tongDiem) *
      100
    ).toFixed(2);

    this.autoSave()
  }

  autoSave() {
    this._storageService.set(
      this.doiTuong.id + '_' + this.kiKhaoSat.code,
      this.evaluate
    );
  }

  async handleSave() {
    // let checkUpload = false
    // const offlineFiles = this.evaluate.lstImages.filter((x: any) => x?.isBase64);
    // if (offlineFiles?.length > 0) {
    //   checkUpload = await this.uploadOfflineFiles(offlineFiles)

    //   if (!checkUpload) {
    //     this.messageService.show('Duy trì mạng ổn định trong quá trình gửi!!!', 'warning');
    //     return
    //   }
    // }
    this.tinhTong()
    console.log('autoSave', this.evaluate);
    this.messageService.show(
      `Lưu bản nháp thành công`,
      'success'
    );
  }

  async onSubmit() {
    if (!this.isEdit) return;
    let checkUpload: boolean = false
    let allChecksPassed = true;
    let errorMessage: string[] = [];

    for (let index = 0; index < this.lstTieuChi.length; index++) {
      const tieuChi = this.lstTieuChi[index];

      const evaluateItem = this.evaluate.lstEvaluate.find(
        (i: any) => i.tieuChiId === tieuChi.id || i.tieuChiCode === tieuChi.code
      );

      // 1. Kiểm tra pointId
      if (!evaluateItem || !evaluateItem.pointId) {
        errorMessage.push(`- <b>Câu ${index + 1}</b>: chưa chấm điểm.`);
        allChecksPassed = false;
      }
      // Kiểm tra có đủ ảnh không
      if (tieuChi.isImg) {
        if (
          tieuChi.chiChtAtvsv &&
          !(
            this.account.chucVuId === 'CHT' || this.account.chucVuId === 'ATVSV'
          )
        ) {
          continue;
        }

        const numberImgRequired = tieuChi.numberImg || 0;
        const imagesSelecting = this.evaluate?.lstImages?.filter(
          (img: any) => img.tieuChiCode === tieuChi.code
        ).length;

        if (imagesSelecting < numberImgRequired) {
          errorMessage.push(`- <b>Câu ${index + 1}</b>: thiếu ảnh.`);
          allChecksPassed = false;
        }
      }
    }

    if (!allChecksPassed) {
      const alert = await this.alertController.create({
        message: `<div class="alert-header-evaluate">
          <div class="alert-icon-evaluate"><ion-icon name="warning"></ion-icon></div>
            <div class="title-evaluate">Đánh giá chưa hoàn tất</div>
            <div class="subtitle-evaluate">Bạn đã bỏ sót các tiêu chí</div>
          </div>
          <div class="alert-body">
            ${errorMessage.join('<br/>')}
          </div>
     `,
        buttons: ['Quay lại chấm'],
        cssClass: 'custom-alert-center-btn',
      });
      await alert.present();
      return;
    }

    // Hiển thị loading
    const loading = await this.loadingController.create({
      message: 'Đang xử lý...',
      spinner: 'circular',
    });
    await loading.present();

    const offlineFiles = this.evaluate.lstImages.filter((x: any) => x?.isBase64);

    if (offlineFiles?.length > 0) {
      const checkUpload = await this.uploadOfflineFiles(offlineFiles);
      if (!checkUpload) {
        await loading.dismiss();
        this.messageService.show('Vui lòng kết nối mạng để nộp!!!', 'warning');
        return;
      }
    }

    try {
      this.tinhTong();

      // Trường hợp đủ
      this.evaluate.header.accountUserName = this.account.userName;
      this.evaluate.header.chucVuId = this.account.chucVuId;
      this.evaluate.lstImages = this.evaluate.lstImages.filter((item: any) => item.isBase64 === undefined);

      console.log(this.evaluate);

      await this._service.insertEvaluate2(this.evaluate).subscribe({
        next: async (data) => {
          console.log('Chấm điểm thành công');
          await this._service
            .HandlePointStore({
              kiKhaoSatId: this.kiKhaoSat.id,
              doiTuongId: this.doiTuong.id,
              surveyId: this.kiKhaoSat.surveyMgmtId,
              lstData: this.doiTuong.lstChamDiem,
            })
            .subscribe({
              next: async (data) => {
                console.log('tính tổng điểm thành công');

                // Đóng loading
                await loading.dismiss();

                this.messageService.show(
                  `Chấm điểm Cửa hàng thành công`,
                  'success'
                );
                await this.removeData()
              },
              error: async (error) => {
                // Đóng loading khi có lỗi
                await loading.dismiss();
                console.error('Lỗi khi tính tổng điểm:', error);
                this.messageService.show('Có lỗi xảy ra khi tính tổng điểm');
              },
            });
        },
        error: async (error) => {
          // Đóng loading khi có lỗi
          await loading.dismiss();
          console.error('Lỗi khi chấm điểm:', error);
          this.messageService.show('Có lỗi xảy ra khi chấm điểm');
        },
      });
    } catch (error) {
      // Đóng loading trong trường hợp có lỗi không mong muốn
      await loading.dismiss();
      console.error('Lỗi không mong muốn:', error);
      this.messageService.show('Có lỗi không mong muốn xảy ra');
    }
  }

  async removeData() {
    await this._storageService.remove(
      this.doiTuong.id + '_' + this.kiKhaoSat.code
    );

    await localStorage.removeItem(
      this.doiTuong.id + '_' + this.kiKhaoSat.code
    );

    await this._systemFileS.deleteFolder(`images/${this.doiTuong.id}_${this.kiKhaoSat.code}`)

    await this.router.navigate([
      `/survey/check-list/${this.doiTuong.id}`,
    ]);
  }

  navigateTo(itemId: string) {
    const currentUrl = window.location.href.split('#')[0]; // Lấy URL hiện tại mà không có hash
    return `${currentUrl}#${itemId}`;
  }

  async uploadOfflineFiles(files: any[]) {
    const batchSize = 10;

    for (let i = 0; i < files.length; i += batchSize) {
      const chunk = files.slice(i, i + batchSize);
      const formData = new FormData();
      let count = 0;

      for (const item of chunk) {
        try {
          const blob = await this._systemFileS.readFileBlob(item.filePath, item.type);

          formData.append(`files[${count}].File`, blob, item.fileName);
          formData.append(`files[${count}].KinhDo`, item.kinhDo);
          formData.append(`files[${count}].ViDo`, item.viDo);
          formData.append(`files[${count}].FileName`, item.fileName);
          formData.append(`files[${count}].Type`, item.type);
          formData.append(`files[${count}].TieuChiCode`, item.tieuChiCode);
          formData.append(`files[${count}].EvaluateHeaderCode`, item.evaluateHeaderCode);

          count++;
        } catch (err) {
          console.error("Lỗi đọc file offline:", item.fileName, err);
          return false; // Lỗi đọc file → dừng luôn
        }
      }

      try {
        const resp = await firstValueFrom(this._service.uploadFileOffline(formData));
        // nếu upload thành công
        for (const item of chunk) {
          item.isBase64 = false;
        }
      } catch (err) {
        console.error("Upload batch thất bại:", err);
        return false; // Lỗi upload → dừng luôn
      }
    }

    return true; // Tất cả batch upload thành công
  }

  //////// Views ảnh

  async openFullScreen(img: any) {
    this.isImageModalOpen = true;
    this.selectedImage = { ...img }

    if (this.isEdit && img?.isBase64) {
      this.selectedImage.filePath = await this._systemFileS.getViewUrl(this.selectedImage.filePath)
    } else {
      this.selectedImage.filePath = this.apiFile + img.filePath;
    }

    this.longitude = img.kinhDo;
    this.latitude = img.viDo;

    // this.selectedImage = filePath;
    console.log(this.selectedImage);
    setTimeout(() => {
      this.initMap();
    }, 300);
  }

  getFilePath(file: any) {
    if (file?.isBase64) {
      // offline
      return file.viewUrl
    }
    // server
    return this.apiFile + file.filePath;
  }

  getThumbPath(file: any) {
    if (file?.isBase64) {
      // offline
      return file.viewUrl
    }
    // server
    return this.apiFile + file.pathThumbnail;
  }

  closeFullScreen() {
    this.isImageModalOpen = false;
  }

  deleteImage() {
    if (!this.isEdit) return;

    const index = this.evaluate?.lstImages.findIndex(
      (img: any) => img.filePath === this.selectedImage.filePath
    );
    if (index > -1) {
      this.evaluate?.lstImages.splice(index, 1);
    }
    this.autoSave()

    this.closeFullScreen();
    this.cdr.detectChanges();
  }

  deleteImage2(img: any) {
    this.selectedImage = img;
    this.confirmDeleteImage();
  }

  async confirmDeleteImage() {
    if (!this.isEdit) return;

    const alert = await this.alertController.create({
      header: 'Xác nhận',
      message: `<div class="alert-header-evaluate">
      <div class="subtitle-evaluate">Bạn có chắc muốn xoá ảnh này ?</div>
      </div>`,
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
            this.closeFullScreen()
          },
        },
      ],
    });
    await alert.present();
  }

  async onFileSelected(event: any, tieuChiCode: any) {
    if (!this.isEdit) return;
    console.log(event);

    const file: File = event.target.files[0];
    if (file.size === 0) {
      this.messageService.show('File không hợp lệ!!', 'warning');
      return;
    }
    if (!file) return;
    const location = await this.getLocation();

    const formData = new FormData();
    formData.append('file', file, file.name);
    const saved = await this.saveOffline(file, tieuChiCode, location);

    // this._service.uploadFile(formData).subscribe({
    //   next: (resp: any) => {
    //     resp.evaluateHeaderCode = this.headerId
    //     resp.tieuChiCode = tieuChiCode

    //     this.evaluate.lstImages.push(resp)
    //     this.autoSave()

    //     this.cdr.detectChanges();
    //   },

    //   error: async (err) => {
    //     const saved = await this.saveOffline(file, tieuChiCode, location);

        console.log("File đã lưu:", saved);
    //   }
    // })
  }

  ////////// Camera chụp ảnh
  async openCamera(code: any) {
    try {
      const photo = await Camera.getPhoto({
        quality: 70,
        resultType: CameraResultType.Uri,
        source: CameraSource.Camera,
        correctOrientation: true,
      });

      if (!photo?.webPath) {
        throw new Error("Không chụp được ảnh");
      }

      // ==== Chuyển ảnh sang Blob ====
      const blob = await (await fetch(photo.webPath)).blob();

      // ==== Lấy toạ độ ====
      const location = await this.getLocation();

      // =================================================
      // 1. Có mạng → Upload trực tiếp lên server
      // =================================================
      if (navigator.onLine) {
        const formData = new FormData();
        const ext = blob.type.split('/')[1];
        formData.append('file', blob, `${Date.now()}.${ext}`);

        this._service.uploadFile(formData).subscribe({
          next: async (resp: any) => {

            resp.evaluateHeaderCode = this.headerId;
            resp.tieuChiCode = code;
            resp.viDo = location.lat;
            resp.kinhDo = location.lng;

            this.evaluate.lstImages.push(resp);

            this.autoSave();
            this.cdr.detectChanges();
          },

          error: async () => {
            console.warn("Upload lỗi → Lưu offline");
            await this.saveOffline(blob, code, location);
          }
        });

        return;
      }

      // =================================================
      // 2. Không có mạng → Lưu offline
      // =================================================
      await this.saveOffline(blob, code, location);

    } catch (err) {
      console.error("Lỗi openCamera:", err);
    }
  }



  // dowload file về máy 
  async downloadFile(doc: any) {
    const url = this.getFilePath(doc); // link server
    const fileName = doc.fileName;
    // const mime = this.getMimeType(doc.type);

    await this._systemFileS.downloadFile(url, fileName);
  }





  /////////// lưu file offline ở local

  async saveOffline(blob: Blob, code: any, location: any) {
    const folder = `images/${this.doiTuong.id}_${this.kiKhaoSat.code}`;

    const saved = await this._systemFileS.saveFile(blob, folder);

    // Thêm các trường cần thiết
    saved.evaluateHeaderCode = this.headerId;
    saved.tieuChiCode = code;
    saved.viDo = location.lat.toString();
    saved.kinhDo = location.lng.toString();

    this.evaluate.lstImages.push(saved);

    this.autoSave();
    this.cdr.detectChanges();

    console.log("Đã lưu offline:", saved);
  }

  imageProcessingQueue: any[] = [];
  private cachedLocation: any = null;

  private async getLocation(): Promise<{ lat: number, lng: number }> {
    try {
      // Nếu có cache < 150s thì dùng lại
      if (this.cachedLocation && Date.now() - this.cachedLocation.t < 150000) {
        return {
          lat: this.cachedLocation.lat,
          lng: this.cachedLocation.lng
        };
      }

      // Lấy GPS
      const pos = await Geolocation.getCurrentPosition({
        enableHighAccuracy: false,
        timeout: 1500,
        maximumAge: 30000,
      });

      const lat = pos.coords.latitude;
      const lng = pos.coords.longitude;

      // Cache lại
      this.cachedLocation = {
        lat, lng, t: Date.now()
      };

      return { lat, lng };

    } catch {
      // Nếu lỗi GPS → trả tọa độ mặc định hoặc null
      return {
        lat: 0,
        lng: 0
      };
    }
  }


  ////////////////////Zoom


  zoomOnClick(event: MouseEvent | TouchEvent) {
    const currentTime = new Date().getTime();
    const tapInterval = currentTime - this.lastTapTime;
    this.lastTapTime = currentTime;

    const imgEl = this.zoomImg.nativeElement as HTMLElement;
    const rect = imgEl.getBoundingClientRect();

    let clientX: number;
    let clientY: number;

    if (event instanceof TouchEvent) {
      const touch = event.touches[0] || event.changedTouches[0];
      clientX = touch.clientX;
      clientY = touch.clientY;
    } else {
      clientX = event.clientX;
      clientY = event.clientY;
    }

    if (tapInterval < this.doubleTapThreshold && this.isZoomed) {
      console.log('[zoomOnClick] Double tap → reset to scale(1)');
      this.resetZoom();
      return;
    }

    if (!this.isZoomed) {
      const offsetX = clientX - rect.left;
      const offsetY = clientY - rect.top;

      const percentX = (offsetX / rect.width) * 100;
      const percentY = (offsetY / rect.height) * 100;

      imgEl.style.transformOrigin = `${percentX}% ${percentY}%`;

      this.currentX = 0;
      this.currentY = 0;
      this.currentScale = 4; // 👈 set scale 4
      this.applyTransform();

      imgEl.classList.add('zoomed');
      this.isZoomed = true;

      console.log(
        '[zoomOnClick] Zoomed in at',
        percentX.toFixed(1),
        '%',
        percentY.toFixed(1),
        '%'
      );
    }
  }

  resetZoom() {
    const imgEl = this.zoomImg.nativeElement as HTMLElement;

    imgEl.style.transformOrigin = 'center center';
    this.currentX = 0;
    this.currentY = 0;
    this.currentScale = 1;
    this.applyTransform();

    imgEl.classList.remove('zoomed');
    this.isZoomed = false;
  }

  onDragStart(event: MouseEvent | TouchEvent) {
    if (!this.isZoomed) return;

    this.isDragging = true;

    if (event instanceof TouchEvent) {
      this.lastX = event.touches[0].clientX;
      this.lastY = event.touches[0].clientY;
    } else {
      this.lastX = event.clientX;
      this.lastY = event.clientY;
    }
  }

  onDragMove(event: MouseEvent | TouchEvent) {
    if (!this.isZoomed || !this.isDragging) return;

    let clientX: number;
    let clientY: number;

    if (event instanceof TouchEvent) {
      clientX = event.touches[0].clientX;
      clientY = event.touches[0].clientY;
    } else {
      clientX = event.clientX;
      clientY = event.clientY;
    }

    const dx = clientX - this.lastX;
    const dy = clientY - this.lastY;

    this.currentX += dx;
    this.currentY += dy;

    this.lastX = clientX;
    this.lastY = clientY;

    this.applyTransform();
  }

  onDragEnd() {
    this.isDragging = false;
  }

  applyTransform() {
    const imgEl = this.zoomImg.nativeElement as HTMLElement;
    imgEl.style.transform = `scale(${this.currentScale}) translate(${this.currentX / this.currentScale
      }px, ${this.currentY / this.currentScale}px)`;
  }


  // search

  getAllAccount() {
    this._authService.GetAllAccount().subscribe({
      next: (data) => {
        this.lstAccout = data;
      },
      error: (response) => {
        console.log(response);
      },
    });
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

  //////////

  openMenu() {
    if (this.lstTieuChi.length == 0) {
      // this.filterTieuChiLeaves(this.treeData)
    }
    this.daCham =
      document.querySelectorAll('.div-dem .answered').length +
      document.querySelectorAll('.div-dem .red-false').length;
    this.chuaCham = this.lstTieuChi.length - this.daCham;
  }

  getFullName(userName: string): string {
    const account = this.lstAccout.find(
      (acc: any) => acc.userName === userName
    );
    return account?.fullName ?? this.account?.fullName;
  }

  trackByKey(index: number, item: any): string {
    return item.key; // hoặc item.id nếu bạn dùng id
  }
}
