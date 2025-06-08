
import { Component, OnInit, ViewChild } from '@angular/core';
import { SharedModule } from '../../../../shared/shared.module';
// import { IonicModule } from '@ionic/angular';
import { AlertController } from '@ionic/angular';
import { IonAccordionGroup } from '@ionic/angular';
import { AppEvaluateService } from 'src/app/service/app-evaluate.service';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  imports: [SharedModule],
  selector: 'app-evaluate',
  templateUrl: './evaluate.component.html',
  styleUrls: ['./evaluate.component.scss'],
})

export class EvaluateComponent implements OnInit {
  @ViewChild('accordionGroup', { static: true }) accordionGroup!: IonAccordionGroup;

  lstAllTieuChi: any = []
  store: any = {}
  kiKhaoSat: any = {}
  treeData: any = [];
  lstTieuChi: any = []
  lstTreeOpen: any = []
  previewImage: any = []
  evaluate: any = {}

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private alertController: AlertController,
    private _service: AppEvaluateService
  ) {
  }
  ngOnInit() {
    this.evaluate = JSON.parse(localStorage.getItem('resultEvaluate') ?? "")
    this.route.paramMap.subscribe({
      next: (params) => {
        const id = params.get('id')
        const status = params.get('status')
        const nav = this.router.getCurrentNavigation();
        if (status === 'new') {
          // const evaluate = nav?.extras.state?.['evaluate'];
          // if(evaluate.header.code == this.evaluate.header.code){
          //   console.log("lưu lại");

          //   localStorage.setItem('resultEvaluate', JSON.stringify(this.evaluate));
          // }
        } else {
        }
        console.log(nav?.extras.state);

        this.store = nav?.extras.state?.['store'];
        this.kiKhaoSat = nav?.extras.state?.['kiKhaoSat'];

        this.getAllTieuChi()
        this.getAllTieuChiLeaves()
      },
    })
    this.previewImage = localStorage.getItem('previewImage');
  }

  getAllTieuChi() {
    this._service.buildDataTreeForApp(this.kiKhaoSat.id, this.store.storeId).subscribe({
      next: (data) => {
        console.log(data);
        this.treeData = [data]
        this.lstTreeOpen = this.extractAllKeys([data])
      }
    })
  }


  getAllTieuChiLeaves() {
    this._service.GetAllTieuChiLeaves(this.kiKhaoSat.id, this.store.storeId).subscribe({
      next: (data) => {
        console.log(data);
        this.lstTieuChi = data

      }
    })
  }


  extractAllKeys(tree: any[]): string[] {
    let keys: string[] = [];

    tree.forEach(node => {
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
        tieuChiCode: code
      })
      localStorage.setItem('resultEvaluate', JSON.stringify(this.evaluate));

      // In ra console kiểm tra
      console.log('Ảnh đã được lưu vào localStorage:', base64);
      this.previewImage = localStorage.getItem('previewImage');

    };
    reader.readAsDataURL(file); // Chuyển sang base64
  }

  filterDiem(code: any) {
    const item = this.evaluate.lstEvaluate.find((x: any) => x.tieuChiCode === code);

    return item.pointId;

  }

  filterImage(code: any) {
    return this.evaluate.lstImages
      .filter((x: any) => x.tieuChiCode === code)
      .map((x: any) => x.filePath);

  }

  setDiem(data: any, event: any) {
    console.log(data);

    const selected = event.detail.value;

    const idx = this.evaluate.lstEvaluate.findIndex(
      (i: any) => i.tieuChiCode === data
    );

    this.evaluate.lstEvaluate[idx].pointId = selected
    localStorage.setItem('resultEvaluate', JSON.stringify(this.evaluate));

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
    const index = this.evaluate.lstImages.findIndex((img: any) => img.filePath === this.selectedImage);
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
          role: 'cancel'
        },
        {
          text: 'Xóa',
          role: 'destructive',
          handler: () => {
            this.deleteImage();
          }
        }
      ]
    });

    await alert.present();
  }

}
