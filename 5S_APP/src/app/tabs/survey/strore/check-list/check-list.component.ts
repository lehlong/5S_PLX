import { Component, OnInit } from '@angular/core';
import { SharedModule } from 'src/app/shared/shared.module';
import { IonButton } from '@ionic/angular/standalone';
import { AlertController } from '@ionic/angular';
import type { OverlayEventDetail } from '@ionic/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AppEvaluateService } from 'src/app/service/app-evaluate.service';
import { StorageService } from 'src/app/service/storage.service';

@Component({
  selector: 'app-check-list',
  imports: [SharedModule, IonButton],
  templateUrl: './check-list.component.html',
  styleUrls: ['./check-list.component.scss'],
})
export class CheckListComponent implements OnInit {
  [x: string]: any;
  kiKhaoSat: any = {}
  mode: 'draft' | 'new' = 'draft';
  store: any = {}
  lstHisEvaluate: any = []
  inStoreId: any = ''
  account: any = {}
  evaluate: any =
    {
      header: {},
      lstEvaluate: [],
      lstImages: [],
    }
  alertButtons: any = [
    {
      text: 'Cancel',
      role: 'cancel',
      handler: () => {
        console.log('Alert canceled');
      },
    },
    {
      text: 'OK',
      role: 'confirm',
      handler: () => {
        console.log('Alert confirmed');
      },
    },
  ];

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private alertController: AlertController,
    private _storageService: StorageService,
    private _service: AppEvaluateService
  ) { }

  ngOnInit() {
    this.account = JSON.parse(localStorage.getItem('UserInfo') ?? "")
    console.log(this.account);

    this.route.paramMap.subscribe({
      next: async (params) => {
        const id = params.get('id')

        const filter = JSON.parse(localStorage.getItem('filterCS') ?? "")
        this.kiKhaoSat = filter.kiKhaoSat
        this.store = filter.store

        // await this._storageService.clear()
        let eva = await this._storageService.get(this.store.id)
        console.log(eva);

        if (eva) {
          this.evaluate = eva
          if (this.evaluate.header.kiKhaoSatId == this.kiKhaoSat.id) {
            this.lstHisEvaluate = [this.evaluate.header]
          }
          console.log(this.mode);

        } else {
          this.mode = 'new'
          console.log(this.mode);
        }

        this.inStoreId = id
        this.getAllEvaluateHistory()
      },
    })
  }

  getAllEvaluateHistory() {
    this._service.search({ keyWord: this.inStoreId }).subscribe({
      next: (data) => {
        console.log(data);

        if (data.data.length == 0) return;

        this.lstHisEvaluate.push(...data.data);
        console.log(this.lstHisEvaluate);
      }
    })
  }

  checkRightEvaluate() {
    if (this.kiKhaoSat?.trangThaiKi !== '2') return false;
    return this.store.lstChamDiem?.some(
      (item: any) => item === this.account.userName
    ) ?? false;
  }

  openEditEvaluate(data: any) {
    this.lstHisEvaluate = []

    if (data.name == "Bản nháp") {
      this.router.navigate([`survey/store/evaluate/draft/${data.code}`]);
    } else {
      this.router.navigate([`survey/store/evaluate/view/${data.code}`]);
    }
  }

  navigateTo() {
    this.lstHisEvaluate = []
    if (this.mode == 'new') {
      this._service.BuildInputEvaluate(this.kiKhaoSat.id, this.store.id).subscribe({
        next: async (data) => {
          this._storageService.set(data.header.storeId, data)
          console.log('tạo mới');
          this.router.navigate([`survey/store/evaluate/draft/${data.header.code}`]);
        }
      })
    } else {
      console.log('chỉnh sửa');
      this.router.navigate([`survey/store/evaluate/draft/${this.evaluate.header.code}`]);
    }
  }

  async remove(code: any) {
    await this._storageService.remove(this.store.id)
    this.lstHisEvaluate.shift();
  }

}
