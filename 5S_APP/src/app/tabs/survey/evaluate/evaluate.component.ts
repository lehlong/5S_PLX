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
import { Storage } from '@ionic/storage-angular';
import { AlertController } from '@ionic/angular';
import { IonAccordionGroup } from '@ionic/angular';
import { AppEvaluateService } from 'src/app/service/app-evaluate.service';
import { ActivatedRoute, Router } from '@angular/router';
import { StorageService } from 'src/app/service/storage.service';
import { environment } from 'src/environments/environment';
import { LoadingController, ToastController } from '@ionic/angular';
import { Camera, CameraResultType, CameraSource } from '@capacitor/camera';
import { Input, ChangeDetectionStrategy } from '@angular/core';
import { MessageService } from 'src/app/service/message.service';
import * as L from 'leaflet';
import { HighlightSearchPipe } from '../../../shared/pipes/highlight-search.pipe';
import { IonHeader } from '@ionic/angular/standalone';
import { AuthService } from 'src/app/service/auth.service';
import { Capacitor } from '@capacitor/core';
import mediumZoom from 'medium-zoom';
import { image } from 'ionicons/icons';
import { Filesystem, Directory } from '@capacitor/filesystem';

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
    private router: Router
  ) { }

  async ngOnInit() {
    const api = localStorage.getItem('CapacitorStorage.apiUrl') ?? '';
    this.apiFile = api.replace(/\/api$/, '/');

    // CapacitorStorage.apiUrl
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
            // console.log(this.evaluate);

            let data = localStorage.getItem(
              this.doiTuong.id + '_' + this.kiKhaoSat.code
            );
            if (data == null) {
              this.getAllTieuChi();
            } else {
              this.dataTree = JSON.parse(data);
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

          if (this.evaluate === null || this.evaluate === undefined) {
            this.router.navigate([`/survey/check-list/${this.doiTuong.id}`]);
          }
        } else {
          this.isEdit = false;
          this.getResultEvaluate();
          this.getAllAccount();
        }
      },
    });
  }

  ngAfterViewInit() {
    mediumZoom('.zoom-image');
  }

  //Zoom
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

  onImageSelected(event: any, code: any) {
    if (!this.isEdit) return;

    const file: File = event.target.files[0];
    if (!file) return;
    const type = this.detectFileType(file);

    const reader = new FileReader();
    // console.log(this.evaluate);
    reader.onload = async () => {
      const base64 = reader.result as string;

      let thumbnail = '';
      if (type === 'img') {
        thumbnail = await this.generateThumbnail(base64, 100, 100);
      }
      // Lưu vào localStorage
      this.evaluate.lstImages.push({
        code: '-1',
        fileName: file.name,
        filePath: base64,
        tieuChiCode: code,
        type: type,
        kinhDo: 0,
        viDo: 0,
        pathThumbnail: thumbnail,
        evaluateHeaderCode: this.headerId,
      });
      this.cdr.detectChanges();
      this._storageService.set(
        this.doiTuong.id + '_' + this.kiKhaoSat.code,
        this.evaluate
      );
    };
    reader.readAsDataURL(file); // Chuyển sang base64
    console.log(this.evaluate);
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
      ? this.evaluate?.lstImages.filter((x: any) => x.tieuChiCode === node.code)
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
    // console.log(this.evaluate);

    this.tinhTong();
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

    this._storageService.set(
      this.doiTuong.id + '_' + this.kiKhaoSat.code,
      this.evaluate
    );
  }

  setFeedBack(data: any, event: any) {
    if (!this.isEdit) return;

    const selected = event.detail.value;
    const idx = this.evaluate.lstEvaluate.findIndex(
      (i: any) => i.tieuChiCode === data
    );
    if (idx === -1) return;

    this.evaluate.lstEvaluate[idx].pointId = selected;
    this._storageService.set(this.doiTuong.id, this.evaluate);
  }

  handleSave() {
    this.messageService.show(
      `Lưu bản nháp thành công`,
      'success'
    );
  }

  async onSubmit() {
    if (!this.isEdit) return;

    let allChecksPassed = true;
    let errorMessage: string[] = [];

    for (let index = 0; index < this.lstTieuChi.length; index++) {
      const tieuChi = this.lstTieuChi[index];

      const evaluateItem = this.evaluate.lstEvaluate.find(
        (i: any) => i.tieuChiId === tieuChi.id || i.tieuChiCode === tieuChi.code
      );

      // 1. Kiểm tra pointId
      if (!evaluateItem || !evaluateItem.pointId) {
        // errorMessage += `- Tiêu chí "${tieuChi.name}" chưa chấm điểm. `;
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
          // errorMessage += `- Tiêu chí "${tieuChi.name}" thiếu ảnh. `;
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
      <div class="subtitle-evaluate">Bạn đã bỏ sót <b class="highlight">${errorMessage.length
          }</b> tiêu chí quan trọng</div>
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

    try {
      // Trường hợp đủ
      this.evaluate.header.accountUserName = this.account.userName;
      this.evaluate.header.chucVuId = this.account.chucVuId;
      console.log(this.kiKhaoSat);

      await this._service.insertEvaluate(this.evaluate).subscribe({
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
                this._storageService.remove(
                  this.doiTuong.id + '_' + this.kiKhaoSat.code
                );
                localStorage.removeItem(
                  this.doiTuong.id + '_' + this.kiKhaoSat.code
                );

                this.router.navigate([
                  `/survey/check-list/${this.doiTuong.id}`,
                ]);
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

  navigateTo(itemId: string) {
    const currentUrl = window.location.href.split('#')[0]; // Lấy URL hiện tại mà không có hash
    return `${currentUrl}#${itemId}`;
  }

  openFullScreen(img: any) {
    let filePath = { ...img };
    if (this.isEdit == false) {
      filePath.filePath = this.apiFile + img.filePath;
    }
    this.longitude = img.kinhDo;
    this.latitude = img.viDo;

    this.selectedImage = filePath;
    console.log(img);

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

    const index = this.evaluate?.lstImages.findIndex(
      (img: any) => img.filePath === this.selectedImage.filePath
    );
    if (index > -1) {
      this.evaluate?.lstImages.splice(index, 1);
    }
    this._storageService.set(
      this.doiTuong.id + '_' + this.kiKhaoSat.code,
      this.evaluate
    );
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

  feedback: string = '';
  // 🚀 Thêm queue xử lý ảnh và debounceprivate

  imageProcessingQueue: any[] = [];
  private isProcessingQueue = false;
  private pendingStorageSave: any;
  private cachedLocation: any = null;
  async openCamera(code: any) {
    if (!this.isEdit) return;

    try {
      const photo = await Camera.getPhoto({
        quality: 65,
        resultType: CameraResultType.Base64,
        source: CameraSource.Camera,
        correctOrientation: false,
        saveToGallery: false,
      });

      if (!photo.base64String) throw new Error("No base64 data");

      const imgObj = {
        code: `-${Date.now()}`,
        fileName: "",
        evaluateHeaderCode: this.headerId,
        filePath: `data:image/jpeg;base64,${photo.base64String}`,
        pathThumbnail: "assets/img/loading-thumb.png",
        tieuChiCode: code,
        viDo: 0,
        kinhDo: 0,
        type: "img",
        isProcessing: true
      };

      // Push vào UI
      this.evaluate.lstImages.push(imgObj);

      // 🔥 Giới hạn RAM cho UI (chỉ giữ 10 ảnh)
      this.limitRamUIImages();

      this.cdr.detectChanges();

      // Thêm vào queue xử lý
      this.addToQueue(imgObj, photo.base64String);

      // Load vị trí nền
      this.updateLocationAsync(imgObj);

    } catch (err) {
      console.error("❌ openCamera", err);
      this.showError("Không thể chụp ảnh");
    }
  }

  /* -------------------------------------------------------
     🧵 QUEUE XỬ LÝ ẢNH – batch 3 ảnh, tránh nghẽn JS thread
  ----------------------------------------------------------*/

  private addToQueue(imageObj: any, base64: string) {
    this.imageProcessingQueue.push({ imageObj, base64 });

    // 🔥 Giới hạn RAM ngay sau khi push
    this.limitRamImages();

    if (!this.isProcessingQueue) {
      this.processQueue();
    }
  }


  private async processQueue() {
    if (this.imageProcessingQueue.length === 0) {
      this.isProcessingQueue = false;
      return;
    }

    this.isProcessingQueue = true;

    // Lấy batch 3 ảnh
    const batch = this.imageProcessingQueue.splice(0, 3);

    try {
      await Promise.all(
        batch.map(i => this.processImage(i.imageObj, i.base64))
      );
    } catch (err) {
      console.warn("Batch error:", err);
    }

    // Debounce lưu storage
    this.debounceStorage();

    // Xử lý batch tiếp theo
    setTimeout(() => this.processQueue(), 80);
  }

  /* -------------------------------------------------------
     🪶 XỬ LÝ ẢNH NHẸ – auto thumbnail + auto giảm RAM
  ----------------------------------------------------------*/
  private async processImage(imgObj: any, base64: string) {
    try {
      const sizeKB = base64.length * 0.75 / 1024;

      if (sizeKB < 500) {
        imgObj.pathThumbnail = await this.makeThumb(base64);
      } else {
        imgObj.pathThumbnail = "assets/img/image-placeholder.png";
      }

      // Giải phóng RAM: thu gọn filePath khi thumbnail đã sẵn sàng
      imgObj.filePath = `data:image/jpeg;base64,${base64}`;
      imgObj.isProcessing = false;

      this.cdr.detectChanges();
    } catch (e) {
      imgObj.pathThumbnail = "assets/img/error-thumb.png";
      imgObj.isProcessing = false;
    }
  }

  /* -------------------------------------------------------
     🖼️ TẠO THUMBNAIL NHANH – 50×50
  ----------------------------------------------------------*/
  private makeThumb(base64: string): Promise<string> {
    return new Promise(resolve => {
      const img = new Image();

      img.onload = () => {
        const canvas = document.createElement("canvas");
        canvas.width = 50;
        canvas.height = 50;

        const ctx = canvas.getContext("2d");
        if (!ctx) return resolve("assets/img/error-thumb.png");

        ctx.imageSmoothingEnabled = false;
        ctx.drawImage(img, 0, 0, 50, 50);

        resolve(canvas.toDataURL("image/jpeg", 0.3));
      };

      img.onerror = () => resolve("assets/img/error-thumb.png");
      img.src = `data:image/jpeg;base64,${base64}`;
    });
  }

  /* -------------------------------------------------------
     📍 GPS CACHE – giảm timeout
  ----------------------------------------------------------*/
  private async updateLocationAsync(obj: any) {
    try {
      // Cache 30s
      if (this.cachedLocation && Date.now() - this.cachedLocation.t < 30000) {
        obj.viDo = this.cachedLocation.lat;
        obj.kinhDo = this.cachedLocation.lng;
        return;
      }

      const pos = await Geolocation.getCurrentPosition({
        enableHighAccuracy: false,
        timeout: 1500,
        maximumAge: 30000,
      });

      obj.viDo = pos.coords.latitude;
      obj.kinhDo = pos.coords.longitude;

      this.cachedLocation = {
        lat: pos.coords.latitude,
        lng: pos.coords.longitude,
        t: Date.now()
      };
    } catch {
      // giữ 0,0
    }
  }

  /* -------------------------------------------------------
     💾 DEBOUNCE LƯU STORAGE – giảm I/O 95%
  ----------------------------------------------------------*/
  private debounceStorage() {
    clearTimeout(this.pendingStorageSave);

    this.pendingStorageSave = setTimeout(() => {
      try {
        this._storageService.set(
          `${this.doiTuong.id}_${this.kiKhaoSat.code}`,
          this.evaluate
        );
        console.log("💾 Saved");
      } catch (e) {
        console.error("Save error", e);
      }
    }, 1200);
  }

  /* -------------------------------------------------------
     🧹 DỌN RAM – giữ tối đa 18 ảnh trong bộ nhớ
  ----------------------------------------------------------*/
  private releaseOldImages() {
    const list = this.evaluate.lstImages;
    if (list.length <= 18) return;

    const old = list.slice(0, -18);
    old.forEach((x: any) => {
      if (x.filePath && x.filePath.length > 5000) {
        x.filePath = "released"; // Giải phóng
      }
    });
  }

  /* -------------------------------------------------------
     🧨 CLEANUP
  ----------------------------------------------------------*/
  ngOnDestroy() {
    this.imageProcessingQueue = [];
    clearTimeout(this.pendingStorageSave);
    this.cachedLocation = null;

    try {
      this._storageService.set(
        `${this.doiTuong.id}_${this.kiKhaoSat.code}`,
        this.evaluate
      );
    } catch { }
  }

  private showError(msg: string) {
    console.error("❌", msg);
  }

  private limitRamImages() {
    const MAX = 10;

    // Nếu quá số lượng → remove ảnh đầu tiên trong queue
    while (this.imageProcessingQueue.length > MAX) {
      const removed = this.imageProcessingQueue.shift();
      // Giải phóng base64 để giảm RAM
      removed.base64 = null;
    }
  }

  private limitRamUIImages() {
    const MAX = 10;

    while (this.evaluate.lstImages.length > MAX) {
      this.evaluate.lstImages.shift();
    }
  }








  // async openCamera(code: any) {
  //   if (!this.isEdit) {
  //     console.warn('isEdit = false → không mở camera');
  //     return;
  //   }

  //   try {
  //     // 1️⃣ Mở camera
  //     const photo = await Camera.getPhoto({
  //       quality: 80,
  //       resultType: CameraResultType.Uri,
  //       source: CameraSource.Camera
  //     });

  //     // 2️⃣ Lưu file thật vào Filesystem
  //     const fileUri = await this.saveToFileSystem(photo);

  //     // 3️⃣ Lấy vị trí (không block camera)
  //     let latitude = 0, longitude = 0;
  //     try {
  //       const location = await this.getCurrentLocationFast();
  //       latitude = location.latitude;
  //       longitude = location.longitude;
  //     } catch { }

  //     // 4️⃣ Tạo thumbnail nhỏ để hiển thị UI
  //     const thumbnail = await this.generateThumbnail(photo.webPath!, 120, 120);

  //     // 5️⃣ Object ảnh nhẹ
  //     const imageObj = {
  //       code: "-1",
  //       fileName: "",
  //       evaluateHeaderCode: this.headerId,
  //       filePath: fileUri,       // 📌 LƯU URI, không lưu base64
  //       pathThumbnail: thumbnail,
  //       tieuChiCode: code,
  //       viDo: latitude,
  //       kinhDo: longitude,
  //       type: "img"
  //     };

  //     // 6️⃣ Đẩy vào danh sách và update UI
  //     this.evaluate.lstImages.push(imageObj);
  //     this.cdr.detectChanges();

  //     // 7️⃣ Lưu storage nhanh
  //     this._storageService.set(this.doiTuong.id + "_" + this.kiKhaoSat.code, this.evaluate);

  //   } catch (err) {
  //     console.error("Lỗi openCamera:", err);
  //   }
  // }

  // private async saveToFileSystem(photo: any): Promise<string> {
  //   const response = await fetch(photo.webPath!);
  //   const blob = await response.blob();

  //   const base64Data = await this.blobToBase64(blob);

  //   const fileName = `img_${Date.now()}.jpeg`;

  //   const saved = await Filesystem.writeFile({
  //     path: fileName,
  //     data: base64Data,
  //     directory: Directory.Data
  //   });

  //   return saved.uri; // 📌 Trả về URI
  // }

  // private blobToBase64(blob: Blob): Promise<string> {
  //   return new Promise((resolve, reject) => {
  //     const reader = new FileReader();
  //     reader.onload = () => resolve((reader.result as string).split(",")[1]);
  //     reader.onerror = reject;
  //     reader.readAsDataURL(blob);
  //   });
  // }










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
