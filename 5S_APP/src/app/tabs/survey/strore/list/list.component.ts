import { Component, OnInit } from '@angular/core';
import { SharedModule } from 'src/app/shared/shared.module';
import { IonButton } from '@ionic/angular/standalone';
import { FormGroup } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { KyKhaoSatService } from 'src/app/service/ky-khao-sat.service';

@Component({
  selector: 'app-scoring-five-s',
  templateUrl: './list.component.html',
  styleUrls: ['./list.component.scss'],
  standalone: true,
  imports: [SharedModule, IonButton],
})
export class ListComponent implements OnInit {
  searchCuaHangToiCham: any = '';

  cuaHangToiCham: boolean = false;

  filterKiKhaoSat: any = {};
  filterStore: any = {};
  filterNguoiCham: any = {}
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

  user: any= {
    userName : 'admin',
    fullName : 'Nguyễn Đình Thi'
  }

  constructor(
    private _service: KyKhaoSatService,
    private router: Router,
    private route: ActivatedRoute
  ) { }

  ngOnInit() {
    this.route.paramMap.subscribe({
      next: (params) => {
        const id = params.get('id');
        this.surveyId = id;
        this.getAllKyKhaoSat();
      },
    });
  }

  getAllKyKhaoSat() {
    this._service
      .search({
        keyWord: this.surveyId,
      })
      .subscribe({
        next: (data) => {
          this.lstKiKhaoSat = data.data;

          this.filterKiKhaoSat = data.data.reduce((a: any, b: any) =>
            new Date(a.endDate) > new Date(b.endDate) ? a : b
          );
          this.getAllStore();
          this.inputSearchKiKhaoSat = this.filterKiKhaoSat;
          // console.log(this.filterKiKhaoSat);
        },
        error: (response) => {
          console.log(response);
        },
      });
  }

  getAllStore() {
    this._service.getInputKiKhaoSat(this.filterKiKhaoSat.id).subscribe({
      next: (data) => {
        this.lstStore = data.lstInputStore;
        this.lstSearchStore = data.lstInputStore;

        // console.log('data', data);
      },
      error: (response) => {
        console.log(response);
      },
    });
  }

  searchStore(kiKhaoSat: any) {
    this.inputSearchKiKhaoSat = kiKhaoSat;
    this._service.getInputKiKhaoSat(kiKhaoSat.id).subscribe({
      next: (data) => {
        this.lstSearchStore = data.lstInputStore;
        // console.log(this.lstSearchStore);

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
    this.filterStore = item;
    console.log('Selected store:', this.filterStore);
    this.lstSearchChamDiem = item.lstInChamDiem
  }

  selectSearchChamDiem(item: any) {
    this.filterNguoiCham = item
  }


  openFilterModal() {
    this.searchStore(this.filterKiKhaoSat);
    this.isOpen = true;
  }

  closeFilterModal() {
    this.isOpen = false;
  }

  onFilter() {
    this.filterKiKhaoSat = this.inputSearchKiKhaoSat
    this.lstStore = this.lstSearchStore
      .filter((s: any) => !this.filterStore?.id || s.id == this.filterStore?.id)
      .filter((s: any) => !this.filterNguoiCham?.userName ||
        s.lstChamDiem?.some((x: any) => x == this.filterNguoiCham.userName))
      .filter((s: any) => this.cuaHangToiCham !== true || s.lstChamDiem?.some((x: any) => x == this.user.userName))

    this.closeFilterModal();
  }


  navigateTo(item: any) {
    console.log(item);

    this.router.navigate([`/survey/store/check-list/${item.id}`], {
      state: {
        kiKhaoSat: this.filterKiKhaoSat,
        store: item
      }
    });
  }

  resetFilters() {
    this.filterKiKhaoSat = this.inputSearchKiKhaoSat
    this.filterStore = {}
    this.filterNguoiCham = {}
    this.inSearchStore = '';
    this.searchNguoiCham = '';
    this.cuaHangToiCham = false;
  }

}
