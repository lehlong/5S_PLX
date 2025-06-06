import { Component, OnInit } from '@angular/core';
import { ButtonFilterComponent } from 'src/app/shared/button-filter/button-filter.component';
import { SharedModule } from 'src/app/shared/shared.module';
import { IonActionSheet, IonButton } from '@ionic/angular/standalone';
import { FormGroup } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ListStoreService } from 'src/app/service/store/list-store.service';
import { KyKhaoSatService } from 'src/app/service/ky-khao-sat.service';

@Component({
  selector: 'app-scoring-five-s',
  templateUrl: './list.component.html',
  styleUrls: ['./list.component.scss'],
  standalone: true,
  imports: [SharedModule, ButtonFilterComponent, IonActionSheet, IonButton],
})
export class ListComponent implements OnInit {
  filterKiKhaoSat: any = {};
  inputSearchKiKhaoSat: any = {}
  searchNguoiCham: any = ''
  inSearchStore: any = ''
  selectValue = '1';
  lstAccout: any = []
  lstSearchChamDiem: any = []
  lstSearchStore: any = []
  lstStore: any = []
  lstKiKhaoSat: any = []
  surveyId: any
  filterForm!: FormGroup;

  constructor(
    private _service: KyKhaoSatService,
    private router: Router,
    private route: ActivatedRoute,
  ) { }


  ngOnInit() {
    this.route.paramMap.subscribe({
      next: (params) => {
        const id = params.get('id')
        this.surveyId = id
        this.getAllKyKhaoSat()
      },
    })
    // this.filterData = {
    //   selectedMonth: 'T042025',
    //   selectedStore: '',
    //   selectedPerson: '',
    // };
    // this.data = this.initData[0].data.lstInputStore;
  }

  getAllKyKhaoSat() {
    this._service.search({
      keyWord: this.surveyId
    }).subscribe({
      next: (data) => {
        this.lstKiKhaoSat = data.data;

        this.filterKiKhaoSat = data.data.reduce((a: any, b: any) =>
          new Date(a.endDate) > new Date(b.endDate) ? a : b
        );
        this.getAllStore()
        this.inputSearchKiKhaoSat = this.filterKiKhaoSat
        console.log(this.filterKiKhaoSat);


      },
      error: (response) => {
        console.log(response)
      },
    })
  }

  getAllStore() {
    this._service.getInputKiKhaoSat(this.filterKiKhaoSat.id).subscribe({
      next: (data) => {
        this.lstStore = data.lstInputStore;
        this.lstSearchStore = data.lstInputStore

        console.log('data', data);
      },
      error: (response) => {
        console.log(response)
      },
    })
  }

  searchStore(kiKhaoSat: any) {
    this.inputSearchKiKhaoSat = kiKhaoSat
    this._service.getInputKiKhaoSat(kiKhaoSat.id).subscribe({
      next: (data) => {
        this.lstSearchStore = data.lstInputStore;
        console.log('data', data);
        this.lstSearchChamDiem = Array.from(
          new Map(
            this.lstSearchStore
              .flatMap((store: any) => store.lstInChamDiem || [])
              .map((item: any) => [item.userName, item]) // key là userName
          ).values()
        );
        console.log(this.lstSearchChamDiem);

      },
      error: (response) => {
        console.log(response)
      },
    })
  }
  onFilterChanged(filterData: any) {
    console.log('Dữ liệu lọc:', filterData);
    // this.filterData = filterData;
    // this.applyFilter();
  }
  isOpen = false;

  openFilterModal() {
    this.searchStore(this.filterKiKhaoSat)
    this.isOpen = true;
  }

  closeFilterModal() {
    this.isOpen = false;
  }

  onFilter() {
    this.filterKiKhaoSat = this.inputSearchKiKhaoSat
    this.lstStore = this.lstSearchStore

    this.closeFilterModal()
  }

  // searchHistoryDowload() {
  //   const keyword = this.inputFileName != null ?  this.inputFileName.trim().toLowerCase() : ''
  //   this.lstHistoryFile = this.lstAllHistoryFile
  //     .filter((c) => !this.inputSearchCustomer || c.customerCode === this.inputSearchCustomer)
  //     .filter((c) => !keyword || c.name.toLowerCase().includes(keyword))
  // }

  // applyFilter() {
  //   if (
  //     this.filterData.selectedStore === '' &&
  //     this.filterData.selectedPerson === ''
  //   ) {
  //     this.data = this.initData[0].data.lstInputStore;
  //   } else {
  //     this.data = this.initData[0].data.lstInputStore.filter((item: any) => {
  //       const storeMatches =
  //         this.filterData.selectedStore === '' ||
  //         item.storeId.includes(this.filterData.selectedStore);
  //       const personMatches =
  //         this.filterData.selectedPerson === '' ||
  //         item.lstChamDiem.includes(this.filterData.selectedPerson);
  //       return storeMatches && personMatches;
  //     });
  //   }
  // }

  navigateTo(item: any) {
    console.log(item);

    this.router.navigate([`/survey/store/check-list/${item}`]);
  }
  resetFilters() {
    this.filterForm.reset({
      selectedMonth: 'T042025',
      selectedStore: '',
      selectedPerson: '',
      storeChecked: false,
      storeUnchecked: false,
    });
  }


  public actionSheetButtons = [
    {
      text: 'Delete',
      role: 'destructive',
      data: {
        action: 'delete',
      },
    },
    {
      text: 'Share',
      data: {
        action: 'share',
      },
    },
    {
      text: 'Cancel',
      role: 'cancel',
      data: {
        action: 'cancel',
      },
    },
  ];


}
