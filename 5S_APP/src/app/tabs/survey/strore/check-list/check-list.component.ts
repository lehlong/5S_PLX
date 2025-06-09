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
  kiKhaoSat: any = {}
  isAdd: 'add' | 'edit' | 'view' | 'del' = 'add';
  store: any = {}
  lstHisEvaluate: any = []
  inStoreId: any = ''
  account: any = {
    userName: 'admin',
    fullName: 'Quản trị viên'
  }
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
    this.route.paramMap.subscribe({
      next: async (params) => {
        const id = params.get('id')

        const filter = JSON.parse(localStorage.getItem('filterCS') ?? "")
        this.kiKhaoSat = filter.kiKhaoSat
        this.store = filter.store

        let eva = await this._storageService.get(this.store.id)
        console.log(eva);

        if (eva) {
          this.evaluate = eva
          if(this.evaluate.header.kiKhaoSatId == this.kiKhaoSat.id){
            this.lstHisEvaluate.push(this.evaluate.header)
          }
          // this.lstHisEvaluate = this.lstEvaluates.lstHeader.filter((x: any) =>
          //   x.kiKhaoSatId === this.kiKhaoSat.id && x.storeId === this.store.id
          // );
          console.log(this.lstHisEvaluate);

          // this.evaluate.header.storeId == this.store?.id && this.evaluate.header.kiKhaoSatId == this.kiKhaoSat.id
          //   ? this.isAdd = "edit"
          //   : this.isAdd = "del"
          // console.log(this.isAdd);

        } else {
          this.isAdd = 'add'
          console.log(this.isAdd);
        }

        this.inStoreId = id
        this.getAllEvaluateHistory()
      },
    })
  }

  getAllEvaluateHistory() {
    this._service.search({ keyWord: this.inStoreId }).subscribe({
      next: (data) => {
        if(!data)
        this.lstHisEvaluate.push(data.data)
      }
    })
  }

  checkRightEvaluate() {
    if (this.kiKhaoSat?.trangThaiKi !== '2') return false;
    return this.store.lstChamDiem?.some(
      (item: any) => item === this.account.userName
    ) ?? false;
  }

  openEditEvaluate(code: any) {
    this.router.navigate([`survey/store/evaluate/${code}`], {
      state: {
        kiKhaoSat: this.kiKhaoSat,
        store: this.store,
        // evaluate: this.evaluate
      }
    });
  }

  navigateTo() {
    if (this.isAdd == 'add' || this.isAdd == 'del') {
      this._service.BuildInputEvaluate(this.kiKhaoSat.id, this.store.id).subscribe({
        next: async (data) => {

          this._storageService.set(data.header.storeId, data)

          // this.lstEvaluates.lstHeader.push(data.header);
          // this.lstEvaluates.lstEvaluate[0].push(...data.lstEvaluate);
          // this.lstEvaluates.lstImages[0].push(...data.lstImages);

          // this._storageService.set('lstEvaluate', this.lstEvaluates)

          console.log(data);
          this.router.navigate([`survey/store/evaluate/${data.header.code}`], {
            state: {
              kiKhaoSat: this.kiKhaoSat,
              store: this.store,
              evaluate: data
            }
          });
        }
      })
    }
  }

  setResult(event: CustomEvent<OverlayEventDetail>) {
    event.detail.role == "cancel"
      ? null
      : this.navigateTo()
  }

  BuildInputEvaluate() {
  }

}
