import { Component, OnInit } from '@angular/core';
import { SharedModule } from 'src/app/shared/shared.module';
import { IonButton } from '@ionic/angular/standalone';
import type { OverlayEventDetail } from '@ionic/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AppEvaluateService } from 'src/app/service/app-evaluate.service';
import { Storage } from '@ionic/storage-angular';
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
  evaluate: any = {}
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

        let eva = await this._storageService.get('resultEvaluate')
        if (eva !== '') {
          this.evaluate = eva
          this.evaluate.header.storeId == this.store?.id
            ? this.isAdd = "edit"
            : this.isAdd = "del"
          console.log(this.isAdd);

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
        this.lstHisEvaluate = data.data
      }
    })
  }

  checkRightEvaluate() {
    if (this.kiKhaoSat?.trangThaiKi !== '2') return false;
    return this.store.lstChamDiem?.some(
      (item: any) => item === this.account.userName
    ) ?? false;
  }

  navigateTo() {
    if (this.isAdd == 'add' || this.isAdd == 'del') {
      this._service.BuildInputEvaluate(this.kiKhaoSat.id, this.store.id).subscribe({
        next: async (data) => {
          // await localStorage.setItem('resultEvaluate', JSON.stringify(data));
          this._storageService.set('resultEvaluate', data)
          console.log(data);
          this.router.navigate([`survey/store/evaluate`], {
            state: {
              kiKhaoSat: this.kiKhaoSat,
              store: this.store,
              evaluate: data
            }
          });
        }
      })
    } else if (this.isAdd == 'edit') {
      this.router.navigate([`survey/store/evaluate`], {
        state: {
          kiKhaoSat: this.kiKhaoSat,
          store: this.store,
          evaluate: this.evaluate
        }
      });
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
