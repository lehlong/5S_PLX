import { Component, OnInit, ViewChild } from '@angular/core';
import { SharedModule } from '../../../../shared/shared.module';
// import { IonicModule } from '@ionic/angular';
import { Storage } from '@ionic/storage-angular';
import { AlertController } from '@ionic/angular';
import { IonAccordionGroup } from '@ionic/angular';
import { AppEvaluateService } from 'src/app/service/app-evaluate.service';
import { ActivatedRoute, Router } from '@angular/router';
import { StorageService } from 'src/app/service/storage.service';

@Component({
  imports: [SharedModule],
  selector: 'app-evaluate',
  templateUrl: './evaluate.component.html',
  styleUrls: ['./evaluate.component.scss'],
})
export class EvaluateComponent implements OnInit {
  @ViewChild('accordionGroup', { static: true })
  accordionGroup!: IonAccordionGroup;
  currentSelect: string = '';
  lstAllTieuChi: any = [];
  store: any = {};
  kiKhaoSat: any = {};
  treeData: any = [];
  lstTieuChi: any = [];
  lstTreeOpen: any = [];
  previewImage: any = [];
  evaluate: any = {
    header: {},
    lstEvaluate: [],
    lstImages: [],
  };
  data: any = {};

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private alertController: AlertController,
    private _storageService: StorageService,
    private storage: Storage,
    private _service: AppEvaluateService
  ) {}
  ngOnInit() {
    this.route.paramMap.subscribe({
      next: async (params) => {
        const nav = this.router.getCurrentNavigation();
        console.log(nav?.extras.state);

        this.store = nav?.extras.state?.['store'];
        this.kiKhaoSat = nav?.extras.state?.['kiKhaoSat'];
        this.evaluate = await this._storageService.get('resultEvaluate');

        this.getAllTieuChi();
        this.getAllTieuChiLeaves();
      },
    });
    this.previewImage = localStorage.getItem('previewImage');
  }

  //Active
  setItem(itemId: string) {
    this.currentSelect = itemId;
    console.log(itemId);
  }

  isActive(itemId: string): boolean {
    return this.currentSelect === itemId;
  }

  isAnswered(id: string): boolean {
    const item = this.evaluate.lstEvaluate.find(
      (i: any) => i.tieuChiId === id || i.tieuChiCode === id
    );
    return !!item && !!item.pointId; 
  }

  getAllTieuChi() {
    this._service
      .buildDataTreeForApp(this.kiKhaoSat.id, this.store.storeId)
      .subscribe({
        next: (data) => {
          console.log(data);
          this.treeData = [data];
          this.lstTreeOpen = this.extractAllKeys([data]);
        },
      });
  }

  getAllTieuChiLeaves() {
    this._service
      .GetAllTieuChiLeaves(this.kiKhaoSat.id, this.store.storeId)
      .subscribe({
        next: (data) => {
          console.log(data);
          this.lstTieuChi = data;
        },
      });
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
    const file: File = event.target.files[0];
    console.log(file);

    if (!file) return;

    const reader = new FileReader();
    reader.onload = () => {
      const base64 = reader.result as string;

      // Lưu vào localStorage
      this.evaluate.lstImages.push({
        code: '-1',
        fileName: '',
        filePath: base64,
        tieuChiCode: code,
      });
      localStorage.setItem('resultEvaluate', JSON.stringify(this.evaluate));

      // In ra console kiểm tra
      console.log('Ảnh đã được lưu vào localStorage:', base64);
      this.previewImage = localStorage.getItem('previewImage');
    };
    reader.readAsDataURL(file); // Chuyển sang base64
  }

  filterDiem(code: any) {
    const item = this.evaluate?.lstEvaluate?.find(
      (x: any) => x.tieuChiCode === code
    );
    return item?.pointId || null;
  }
  filterImage(code: any) {
    return Array.isArray(this.evaluate?.lstImages)
      ? this.evaluate.lstImages
          .filter((x: any) => x.tieuChiCode === code)
          .map((x: any) => x.filePath)
      : [];
  }

  setDiem(data: any, event: any) {
    console.log(this.evaluate);
    const selected = event.detail.value;
    console.log(`Đã chọn tiêu chí với mã code: ${data}, giá trị: ${selected}`);
    const idx = this.evaluate.lstEvaluate.findIndex(
      (i: any) => i.tieuChiCode === data
    );
    console.log(idx);
    if (idx === -1) {
      console.warn('Không tìm thấy tiêu chí', data);
      return;
    }
    this.evaluate.lstEvaluate[idx].pointId = selected;
    console.log('Updated pointId:', this.evaluate.lstEvaluate[idx].pointId);
    this._storageService.set('resultEvaluate', this.evaluate);
    // localStorage.setItem('resultEvaluate', JSON.stringify(this.evaluate));
  }

  onSubmit() {
    console.log(this.evaluate.lstEvaluate);
  }

  navigateTo(itemId: string) {
    const currentUrl = window.location.href; // Lấy URL hiện tại mà không có hash
    return `${currentUrl}#${itemId}`;
  }

  isImageModalOpen = false;
  selectedImage: string = '';

  openFullScreen(img: string) {
    this.selectedImage = img;
    this.isImageModalOpen = true;
  }

  closeFullScreen() {
    this.isImageModalOpen = false;
  }

  deleteImage() {
    // Gọi API hoặc xóa khỏi mảng
    const index = this.evaluate.lstImages.findIndex(
      (img: any) => img.filePath === this.selectedImage
    );
    if (index > -1) {
      this.evaluate.lstImages.splice(index, 1);
    }
    this.closeFullScreen();
  }

  async confirmDeleteImage() {
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
}
