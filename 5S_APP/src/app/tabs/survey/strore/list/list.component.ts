import { Component, OnInit } from '@angular/core';
import { SharedModule } from 'src/app/shared/shared.module';
import { IonButton } from '@ionic/angular/standalone';
import { FormGroup } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { KyKhaoSatService } from 'src/app/service/ky-khao-sat.service';
import { AuthService } from 'src/app/service/auth.service';
import { AppEvaluateService } from 'src/app/service/app-evaluate.service';
import { AppReportService } from 'src/app/service/app-report.service';

@Component({
  selector: 'app-scoring-five-s',
  templateUrl: './list.component.html',
  styleUrls: ['./list.component.scss'],
  standalone: true,
  imports: [SharedModule, IonButton],
})
export class ListComponent implements OnInit {

  filter: any = {
    filterKiKhaoSat: {},
    filterStore: {},
    filterNguoiCham: {},
    cuaHangToiCham: false

  }
  inputSearchKiKhaoSat: any = {};

  searchNguoiCham: any = '';
  inSearchStore: any = '';
  selectValue = '1';

  lstAccout: any = [];
  lstSearchChamDiem: any = [];
  lstSearchStore: any = [];
  lstStore: any = [];
  lstKiKhaoSat: any = [];
  surveyId: any;
  filterForm!: FormGroup;
  isOpen = false;
  lstAccount: any = []
  lstPointStore: any = []

  user: any = {}

  constructor(
    private _service: KyKhaoSatService,
    private _appService: AppEvaluateService,
    private _reportService: AppReportService,
    private _authService: AuthService,
    private router: Router,
    private route: ActivatedRoute
  ) { }

  ngOnInit() {
    this.user = JSON.parse(localStorage.getItem('UserInfo') ?? "")
    this.route.paramMap.subscribe({
      next: (params) => {
        const id = params.get('id');
        this.surveyId = id;
        this.getAllKyKhaoSat();
      },
    });
  }
 getAllAccount() {
    this._reportService.GetAllAccount().subscribe({
      next: (data) => {
       this.lstAccout=  data;
      },
      error: (response) => {
        console.log(response);
      },
    });
  }

  getFullName(userName: string): string {
    const account = this.lstAccout.find((acc: any) => acc.userName === userName);
    return account?.fullName;
  }
  getAllKyKhaoSat() {
    this._service
      .search({ keyWord: this.surveyId })
      .subscribe({
        next: (data) => {
          this.lstKiKhaoSat = data.data;

          const filter = localStorage.getItem('filterLS') ?? ""
          const filter2 = data.data.reduce((a: any, b: any) =>
            new Date(a.endDate) > new Date(b.endDate) ? a : b
          );
          if (filter != "") {
            this.filter = JSON.parse(filter)
            if (this.filter.filterKiKhaoSat.surveyMgmtId != this.surveyId) {
              this.filter.filterKiKhaoSat = filter2
            }
          } else {
            this.filter.filterKiKhaoSat = filter2
          }

          this.getAllStore();
          this.getPointStore();

          this.inputSearchKiKhaoSat = this.filter.filterKiKhaoSat;
        },
        error: (response) => {
          console.log(response);
        },
      });
  }

  getAllStore() {
    this._service.getInputKiKhaoSat(this.filter.filterKiKhaoSat.id).subscribe({
      next: (data) => {
        this.lstStore = data.lstInputStore;
        this.lstSearchStore = data.lstInputStore;
      },
      error: (response) => {
        console.log(response);
      },
    });
  }

  getPointStore() {
    this._appService.getPointStore({ kiKhaoSatId: this.filter.filterKiKhaoSat.id, surveyId: this.surveyId }).subscribe({
      next: (data) => {
        console.log(data);

        this.lstPointStore = data
      }
    })
  }

  filterPoint(inStoreId: any){
    const record = this.lstPointStore.find((x: any) => x.inStoreId === inStoreId);

    return record?.point ?? 0;
  }

  // getAllAccount() {
  //   this._authService.(this.filter.filterKiKhaoSat.id).subscribe({
  //     next: (data) => {
  //       this.lstStore = data.lstInputStore;
  //       this.lstSearchStore = data.lstInputStore;
  //     },
  //     error: (response) => {
  //       console.log(response);
  //     },
  //   });
  // }

  searchStore(kiKhaoSat: any) {
    this.inputSearchKiKhaoSat = kiKhaoSat;
    this._service.getInputKiKhaoSat(kiKhaoSat.id).subscribe({
      next: (data) => {
        console.log("searchStore", data);
        this.lstSearchStore = data.lstInputStore;
        this.lstSearchChamDiem = Array.from(
          new Map(
            this.lstSearchStore
              .flatMap((store: any) => store.lstInChamDiem || [])
              .map((item: any) => [item.userName, item])
          ).values()
        );
      },
      error: (response) => {
        console.log(response);
      },
    });
  }
  selectSearchStore(item: any) {
    this.filter.filterStore = item;
    console.log('Selected store:', this.filter.filterStore);
    this.lstSearchChamDiem = item.lstInChamDiem
  }

  selectSearchChamDiem(item: any) {
    this.filter.filterNguoiCham = item
  }


  openFilterModal() {
    this.searchStore(this.filter.filterKiKhaoSat);
    this.isOpen = true;
  }

  closeFilterModal() {
    this.isOpen = false;
  }

  onFilter() {
    this.filter.filterKiKhaoSat = this.inputSearchKiKhaoSat
    this.lstStore = this.lstSearchStore
      .filter((s: any) => !this.filter.filterStore?.id || s.id == this.filter.filterStore?.id)
      .filter((s: any) => !this.filter.filterNguoiCham?.userName ||
        s.lstChamDiem?.some((x: any) => x == this.filter.filterNguoiCham.userName))
      .filter((s: any) => this.filter.cuaHangToiCham !== true || s.lstChamDiem?.some((x: any) => x == this.user.userName))
    localStorage.setItem('filterLS', JSON.stringify(this.filter))
    this.closeFilterModal();
  }


  navigateTo(item: any) {
    localStorage.setItem('filterCS', JSON.stringify({ kiKhaoSat: this.filter.filterKiKhaoSat, store: item }))
    this.router.navigate([`/survey/store/check-list/${item.id}`]);
  }

  resetFilters() {
    this.filter.filterKiKhaoSat = this.lstKiKhaoSat.reduce((a: any, b: any) =>
      new Date(a.endDate) > new Date(b.endDate) ? a : b
    );
    this.filter.filterStore = {}
    this.inputSearchKiKhaoSat = this.filter.filterKiKhaoSat
    this.filter.filterNguoiCham = {}
    this.filter.inSearchStore = '';
    this.filter.searchNguoiCham = '';
    this.filter.cuaHangToiCham = false;
    localStorage.setItem('filterLS', JSON.stringify(this.filter))

  }
}
