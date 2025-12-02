import { Component, OnInit } from '@angular/core';
import { SharedModule } from 'src/app/shared/shared.module';
import { IonButton } from '@ionic/angular/standalone';
import { LoadingController, ToastController } from '@ionic/angular';
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
  kiKhaoSat: any = {};
  mode: 'draft' | 'new' = 'draft';
  doiTuong: any = {};
  lstHisEvaluate: any = [];
  doiTuongId: any = '';
  deviceID: string = '';
  account: any = {};
  lstAccout: any = [];
  evaluate: any = {
    header: {},
    lstEvaluate: [],
    lstImages: [],
  };

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private _authService: AuthService,
    private _storageService: StorageService,
    private _service: AppEvaluateService,
  ) {}

  ngOnInit() {
    this.account = JSON.parse(localStorage.getItem('UserInfo') ?? '');
    this.route.paramMap.subscribe({
      next: async (params) => {
        console.log(this.lstHisEvaluate);

        this.lstHisEvaluate = []
        console.log(this.lstHisEvaluate);

        const id = params.get('id');
        const filter = JSON.parse(localStorage.getItem('filterCS') ?? '');
        this.kiKhaoSat = filter.kiKhaoSat;
        this.doiTuong = filter.doiTuong;

        // console.log(filter);

        // await this._storageService.clear()
        let eva = await this._storageService.get(
          this.doiTuong.id + '_' + this.kiKhaoSat.code
        );

        //check xem có bản nháp trong sqlite hay không
        if (eva) {
          this.evaluate = eva;
          if (this.evaluate.header.kiKhaoSatId == this.kiKhaoSat.id) {
            this.lstHisEvaluate = [this.evaluate.header];
          }
        } else {
          this.mode = 'new';
        }
        this.doiTuongId = id;
        this.getAllEvaluateHistory();
        this.getAllAccount();
      },
    });
  }

  getAllEvaluateHistory() {
    this._service
      .search({ keyWord: this.doiTuongId, sortColumn: this.kiKhaoSat.id })
      .subscribe({
        next: (data) => {
          if (data.data.length == 0) return;
          this.lstHisEvaluate.push(
            ...data.data.sort((a: any, b: any) => b.order - a.order)
          );
        },
      });
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
    // console.log(this.kiKhaoSat);

    const date = new Date(this.kiKhaoSat.endDate);
    const now = new Date();

    const currentMonth = now.getMonth() + 1;

    if (
      this.kiKhaoSat?.trangThaiKi !== '2' ||
      date.getMonth() + 1 < currentMonth
    )
      return false;
    if (this.account.allowScoring) return true;
    return (
      this.doiTuong.lstChamDiem?.some(
        (item: any) => item === this.account.userName
      ) ?? false
    );
  }

  openEditEvaluate(data: any) {
    this.lstHisEvaluate = [];

    if (data.name == 'Bản nháp') {
      this.router.navigate([`survey/evaluate/draft/${data.code}`]);
    } else {
      this.router.navigate([`survey/evaluate/view/${data.code}`]);
    }
  }

  async navigateTo() {
    this.lstHisEvaluate = [];
    let userInfo = JSON.parse(localStorage.getItem('UserInfo') ?? '');
    this.deviceID = userInfo?.deviceId || '';

    if (this.mode == 'new') {
      this._service
        .BuildInputEvaluate(this.kiKhaoSat.id, this.doiTuong.id, this.deviceID)
        .subscribe({
          next: async (data) => {
            await this._storageService.set(
              data.header.doiTuongId + '_' + this.kiKhaoSat.code,
              data
            );
            console.log('tạo mới', data);

            // setTimeout(() => {
            this.router.navigate([`survey/evaluate/draft/${data.header.code}`]);
            // }, 1000);
          },
        });
    } else {
      console.log('✏️ Chỉnh sửa - Navigate ngay');
      await this.router.navigate([
        `survey/evaluate/draft/${this.evaluate.header.code}`,
      ]);
    }
  }
  getFullName(userName: string): string {
    const account = this.lstAccout.find(
      (acc: any) => acc.userName === userName
    );
    return account?.fullName ?? this.account?.fullName;
  }

  async remove(code: any) {
    await this._storageService.remove(
      this.doiTuong.id + '_' + this.kiKhaoSat.code
    );
    await this._storageService.remove(
      'allImages_' + this.doiTuong.id + '_' + this.kiKhaoSat.code
    ) 
    localStorage.removeItem(this.doiTuong.id + '_' + this.kiKhaoSat.code);
    this.lstHisEvaluate.shift();
  }
}
