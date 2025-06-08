import { Component, OnInit } from '@angular/core';
import { SharedModule } from 'src/app/shared/shared.module';
import { IonButton } from '@ionic/angular/standalone';
import type { OverlayEventDetail } from '@ionic/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AppEvaluateService } from 'src/app/service/app-evaluate.service';

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
    private _service: AppEvaluateService
  ) { }

  ngOnInit() {

    this.route.paramMap.subscribe({
      next: (params) => {
        const id = params.get('id')
        const nav = this.router.getCurrentNavigation();
        console.log(nav?.extras.state);

        this.kiKhaoSat = nav?.extras.state?.['kiKhaoSat'];
        this.store = nav?.extras.state?.['store'];
        const eva = localStorage.getItem('resultEvaluate') ?? ""
        console.log(eva);
        // this.evaluate = JSON.parse(this.evaluate)

        if(eva !== ''){
          this.evaluate = JSON.parse(eva)

          this.evaluate.header.storeId == this.store.id
            ? this.isAdd = "edit"
            : this.isAdd = "del"
          console.log(this.isAdd);

        }else{
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
    if (this.kiKhaoSat.trangThaiKi !== '2') return false;
    return this.store.lstChamDiem?.some(
      (item: any) => item === this.account.userName
    ) ?? false;
  }

  navigateTo() {
    if (this.isAdd == 'add' || this.isAdd == 'del') {
      this._service.BuildInputEvaluate(this.kiKhaoSat.id, this.store.id).subscribe({
        next: (data) => {
          localStorage.setItem('resultEvaluate', JSON.stringify(data));
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
    } else if(this.isAdd == 'edit') {
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
