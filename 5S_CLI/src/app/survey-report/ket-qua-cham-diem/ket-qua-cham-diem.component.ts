import { Component } from '@angular/core';
import { ShareModule } from '../../shared/share-module'
import { FormGroup, NonNullableFormBuilder, Validators } from '@angular/forms'
import { AccountTypeFilter } from '../../models/master-data/account-type.model'
import { PaginationResult } from '../../models/base.model'
import { StoreService } from '../../service/master-data/store.service'
import { AccountService } from '../../service/system-manager/account.service'
import { GlobalService } from '../../service/global.service'
import { NzMessageService } from 'ng-zorro-antd/message'
import { SurveyMgmtService } from '../../service/business/survey-mgmt.service';
import { KiKhaoSatService } from '../../service/master-data/ki-khao-sat.service';
import { AppReportService } from '../../service/business/app-report.service';


@Component({
  selector: 'app-ket-qua-cham-diem',
  standalone: true,
  imports: [ShareModule],
  templateUrl: './ket-qua-cham-diem.component.html',
  styleUrl: './ket-qua-cham-diem.component.scss'
})
export class KetQuaChamDiemComponent {

  filter = new AccountTypeFilter()
  paginationResult = new PaginationResult()
  loading: boolean = false
  lstSurvey: any = []
  lstKiKhaoSat: any = []
  lstAllKiKhaoSat: any = []
  lstDoiTuong: any = []
  survey: any = {}
  kiKhaosatId: any = null
  doiTuongId: any = null
  lstData: any = []

  constructor(
    private _kiKhaoSatService: KiKhaoSatService,
    private _surveyService: SurveyMgmtService,
    private globalService: GlobalService,
    private _appReportService: AppReportService,
    private message: NzMessageService,
  ) {
    this.globalService.setBreadcrumb([
      {
        name: 'Báo Cáo',
        path: 'master-data/account-type',
      },
    ])
    this.globalService.getLoading().subscribe((value) => {
      this.loading = value
    })
  }
  ngOnDestroy() {
    this.globalService.setBreadcrumb([])
  }

  ngOnInit(): void {
    this.getAllKiKhaoSat()
    this.getAllSurvey()
  }

  getAllSurvey() {
    this._surveyService.search(this.filter).subscribe({
      next: (data) => {
        this.lstSurvey = data.data
        console.log(data);

      },
      error: (response) => {
        console.log(response)
      },
    })
  }

  getAllKiKhaoSat() {
    this._kiKhaoSatService.search(this.filter).subscribe({
      next: (data) => {
        console.log(data);
        this.lstAllKiKhaoSat = data.data

      },
      error: (response) => {
        console.log(response)
      },
    })
  }

  searchDoiTuong() {
    this._kiKhaoSatService.getInputKiKhaoSat(this.kiKhaosatId).subscribe({
      next: (data) => {
        console.log(data);
        if (data.lstInputStore?.length != 0) {
          this.lstDoiTuong = data.lstInputStore
        } else {
          this.lstDoiTuong = data.lstInputWareHouse
        }
      },
      error: (response) => {
        console.log(response)
      },
    })
  }

  searchKiKhaoSat() {
    console.log(this.survey.id);
    this.lstKiKhaoSat = this.lstAllKiKhaoSat.filter((x: any) => x.surveyMgmtId == this.survey.id)
    this.kiKhaosatId = this.lstKiKhaoSat.reduce((a: any, b: any) =>
      new Date(a.endDate) > new Date(b.endDate) ? a.id : b.id
    )
  }

  close() {
    this.resetForm()
  }

  reset() {
    this.filter = new AccountTypeFilter()
  }

  exportExcel() { }

  resetForm() {
  }

  getReport() {
    this._appReportService.KetQuaChamDiem({ surveyId: this.survey.doiTuongId, kiKhaoSatId: this.kiKhaosatId, doiTuongId: this.doiTuongId }).subscribe({
      next: (data) => {
        console.log(data);
        this.lstData = data
      }
    })
  }


}
