import { Component, OnInit } from '@angular/core';
import { SharedModule } from 'src/app/shared/shared.module';
import { IonButton } from '@ionic/angular/standalone';
import { ActivatedRoute, Router } from '@angular/router';
import { AppEvaluateService } from 'src/app/service/app-evaluate.service';
import { StorageService } from 'src/app/service/storage.service';
import { AuthService } from 'src/app/service/auth.service';

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
  doiTuong: any = {}
  lstHisEvaluate: any = []
  doiTuongId: any = ''
  deviceID: string = '';
  account: any = {}
  lstAccout: any = []
  evaluate: any =
    {
      header: {},
      lstEvaluate: [],
      lstImages: [],
    }

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private _authService: AuthService,
    private _storageService: StorageService,
    private _service: AppEvaluateService
  ) { }

  ngOnInit() {
    this.account = JSON.parse(localStorage.getItem('UserInfo') ?? "")

    this.route.paramMap.subscribe({
      next: async (params) => {
        const id = params.get('id')

        const filter = JSON.parse(localStorage.getItem('filterCS') ?? "")
        this.kiKhaoSat = filter.kiKhaoSat
        this.doiTuong = filter.doiTuong

        // await this._storageService.clear()
        let eva = await this._storageService.get(this.doiTuong.id + "_" + this.kiKhaoSat.code)
        console.log('eva', eva);

        if (eva) {
          this.evaluate = eva

          if (this.evaluate.header.kiKhaoSatId == this.kiKhaoSat.id) {
            this.lstHisEvaluate = [this.evaluate.header]
          }
        } else {
          this.mode = 'new'
        }
        this.doiTuongId = id
        this.getAllEvaluateHistory()
        this.getAllAccount()
      },
    })
  }

  getAllEvaluateHistory() {
    this._service.search({ keyWord: this.doiTuongId, sortColumn: this.kiKhaoSat.id }).subscribe({
      next: (data) => {
        if (data.data.length == 0) return;

        this.lstHisEvaluate.push(...data.data);
      }
    })
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
  checkRightEvaluate() {
    if (this.kiKhaoSat?.trangThaiKi !== '2') return false;
    return this.doiTuong.lstChamDiem?.some(
      (item: any) => item === this.account.userName
    ) ?? false;
  }

  openEditEvaluate(data: any) {
    this.lstHisEvaluate = []

    if (data.name == "Bản nháp") {
      this.router.navigate([`survey/evaluate/draft/${data.code}`]);
    } else {
      this.router.navigate([`survey/evaluate/view/${data.code}`]);
    }
  }

  navigateTo() {
    console.log(this.doiTuong);

    this.lstHisEvaluate = []
    let userInfo = JSON.parse(localStorage.getItem('UserInfo') ?? '');
    this.deviceID = userInfo?.deviceId || '';
    if (this.mode == 'new') {
      this._service.BuildInputEvaluate(this.kiKhaoSat.id, this.doiTuong.id, this.deviceID).subscribe({
        next: async (data) => {
          this._storageService.set(data.header.doiTuongId + "_" + this.kiKhaoSat.code, data)
          console.log('tạo mới', data);
          this.router.navigate([`survey/evaluate/draft/${data.header.code}`]);
        }
      })
    } else {
      console.log('chỉnh sửa');
      this.router.navigate([`survey/evaluate/draft/${this.evaluate.header.code}`]);
    }
  }

  getFullName(userName: string): string {
    const account = this.lstAccout.find((acc: any) => acc.userName === userName);
    return account?.fullName ?? this.account?.fullName;
  }

  async remove(code: any) {
    await this._storageService.remove(this.doiTuong.id + "_" + this.kiKhaoSat.code);
    localStorage.removeItem(this.doiTuong.id + "_" + this.kiKhaoSat.code)
    this.lstHisEvaluate.shift();
  }


}
