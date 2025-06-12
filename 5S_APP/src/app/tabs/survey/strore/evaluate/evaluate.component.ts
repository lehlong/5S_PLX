import { ChangeDetectorRef, Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { SharedModule } from '../../../../shared/shared.module';
// import { IonicModule } from '@ionic/angular';
import { Storage } from '@ionic/storage-angular';
import { AlertController } from '@ionic/angular';
import { IonAccordionGroup } from '@ionic/angular';
import { AppEvaluateService } from 'src/app/service/app-evaluate.service';
import { ActivatedRoute, Router } from '@angular/router';
import { StorageService } from 'src/app/service/storage.service';
import { environment } from 'src/environments/environment';
import { Camera, CameraResultType, CameraSource } from '@capacitor/camera';
import { Input, ChangeDetectionStrategy } from '@angular/core';

@Component({
  imports: [SharedModule],
  selector: 'app-evaluate',
  templateUrl: './evaluate.component.html',
  styleUrls: ['./evaluate.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class EvaluateComponent implements OnInit {
  @ViewChild('accordionGroup', { static: true })
  accordionGroup!: IonAccordionGroup;
  @ViewChild('fileInput', { static: false })
  fileInput!: ElementRef<HTMLInputElement>;


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
  count: any = 0
  maxCount: any = 0
  isEdit: any = true;
  apiFile = (environment as any).apiFile;



  constructor(
    private route: ActivatedRoute,
    private alertController: AlertController,
    private _storageService: StorageService,
    private _service: AppEvaluateService,
    private cdr: ChangeDetectorRef,
  ) { }
  ngOnInit() {
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
          this.getResultEvaluate();
        }

        this.getAllTieuChi();
        this.getAllTieuChiLeaves();
      },
    });

  }

  getResultEvaluate() {
    this._service.getResultEvaluate(this.headerId).subscribe({
      next: (data) => {
        console.log(data);
        this.evaluate = data;
      },
    });
  }

  getAllTieuChi() {
    this._service
      .buildDataTreeForApp(this.kiKhaoSat.id, this.store.id)
      .subscribe({
        next: (data) => {
          console.log(data);
          this.maxCount = data.orderNumber
          this.treeData = [data];
          this.lstTreeOpen = [...this.extractAllKeys([data])];

          this.cdr.detectChanges();
        },

      });
  }

  getAllTieuChiLeaves() {
    this._service
      .GetAllTieuChiLeaves(this.kiKhaoSat.id, this.store.id)
      .subscribe({
        next: (data) => {
          console.log(data);
          this.lstTieuChi = data;
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

  isActive(itemId: string): boolean {
    return this.currentSelect === itemId;
  }

  borderActive(data: any): any {
    console.log('data-border', data.code);
  }

  isAnswered(data: any) {
    const evaluateItem = this.evaluate.lstEvaluate.find(
      (i: any) => i.tieuChiId === data.code || i.tieuChiCode === data.code
    );
    const hasPoint = !!evaluateItem && !!evaluateItem.pointId;
    const numberImgRequired = data?.numberImg || 0;
    const hasImage = this.evaluate.lstImages.filter(
      (img: any) => img.tieuChiCode === data.code
    ).length;
    const hasEnoughImages = hasImage >= numberImgRequired;
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
    const type = file.type.startsWith('image/') ? 'img' : file.type.startsWith('video/') ? "mp4" : ''
    console.log(type);

    console.log('Loại:', file.type);
    const reader = new FileReader();
    console.log(this.evaluate);

    reader.onload = () => {
      const base64 = reader.result as string;

      // Lưu vào localStorage
      this.evaluate.lstImages.push({
        code: '-1',
        fileName: '',
        filePath: base64,
        tieuChiCode: code,
        type: type,
        evaluateHeaderCode: this.headerId,
      })
      this.cdr.detectChanges();
      this._storageService.set(this.store.id, this.evaluate);
    }
    reader.readAsDataURL(file); // Chuyển sang base64


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
      ? this.evaluate.lstImages
        .filter((x: any) => x.tieuChiCode === node.code)
      : null;
    if (!data) return;

    return data.map((x: any) => ({
      filePath: x.filePath,
      type: x.type
    }))
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
      (i: any) => i.tieuChiCode === data
    );
    console.log(data, idx);

    if (idx === -1) return;

    this.evaluate.lstEvaluate[idx].pointId = selected;
    console.log(this.evaluate);
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
      const numberImgRequired = tieuChi.numberImg || 0;
      const imagesSelecting = this.evaluate.lstImages.filter(
        (img: any) => img.tieuChiCode === tieuChi.code
      ).length;

      if (imagesSelecting < numberImgRequired) {
        errorMessage += `- Tiêu chí "${tieuChi.name}" thiếu ảnh. `;
        allChecksPassed = false;
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

    // Trường hợp đủ
    this._service.insertEvaluate(this.evaluate).subscribe({
      next: () => {
        console.log('Chấm điểm thành công');

        this._storageService.remove(this.store.id);
      },

      error: (ex) => {
        console.log(ex);
      },
    });

    console.log(this.evaluate);
  }

  navigateTo(itemId: string) {
    const currentUrl = window.location.href.split('#')[0]; // Lấy URL hiện tại mà không có hash
    return `${currentUrl}#${itemId}`;
  }

  isImageModalOpen = false;
  selectedImage: any = {};

  openFullScreen(img: any) {
    if(this.isEdit == false){
      img.filePath = this.apiFile + img.filePath
    }
    this.selectedImage = img;
    this.isImageModalOpen = true;
  }

  filePath(filePath: string){
    if(this.isEdit) return filePath;
    return this.apiFile + filePath
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
      });
    this.cdr.detectChanges();

      this._storageService.set(this.store.id, this.evaluate);
    } catch (err) {
      console.error('Camera error:', err);
    }
  }

  dem() {
    this.count += 1
    console.log(this.count);

  }


  shouldRender(): boolean {
    this.count++;
    return this.count <= this.maxCount;
  }

  trackByKey(index: number, item: any): string {
    return item.key; // hoặc item.id nếu bạn dùng id
  }

}
