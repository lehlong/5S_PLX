import { Component } from '@angular/core';
import { ShareModule } from '../../shared/share-module'
import { AccountTypeFilter } from '../../models/master-data/account-type.model'
import { PaginationResult } from '../../models/base.model'
import { GlobalService } from '../../service/global.service'
import { AppReportService } from '../../service/business/app-report.service';
import { environment } from '../../../environments/environment';
import { NzMessageService } from 'ng-zorro-antd/message'
import { KiKhaoSatService } from '../../service/master-data/ki-khao-sat.service';
import { SurveyMgmtService } from '../../service/business/survey-mgmt.service';


@Component({
  selector: 'app-cham-diem-theo-khung-thoi-gian',
  standalone: true,
  imports: [ShareModule],
  templateUrl: './cham-diem-theo-khung-thoi-gian.component.html',
  styleUrl: './cham-diem-theo-khung-thoi-gian.component.scss'
})
export class ChamDiemTheoKhungThoiGianComponent {

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

  exportExcel() {
    if (!this.survey.doiTuongId && !this.kiKhaosatId) {
      this.message.error('Vui lòng chọn đầy đủ thông tin trước khi xuất file');
      return;

    } else {
      this._appReportService.ExportExcel("ChamTheokhungThoiGian", { surveyId: this.survey.doiTuongId, kiKhaoSatId: this.kiKhaosatId, doiTuongId: this.doiTuongId })
        .subscribe({
          next: (data) => {
            console.log(data);
            if (data) {
              const downloadUrl = `${environment.urlFiles}${data}`; // hoặc cấu hình phù hợp với backend của bạn
              const a = document.createElement('a');
              a.href = downloadUrl;
              // a.download = `ChamTheokhungThoiGian_${this.survey.name}_${this.kiKhaosatId}.xlsx`;
              a.target = '_blank'; // mở tab mới (tùy chọn)
              a.click();
              this.message.success('Xuất dữ liệu thành công');
            } else {
              this.message.error('Không có dữ liệu để xuất');
            }
          }
        })
    }
  }

  resetForm() {
  }

  getReport() {
    this._appReportService.TheoKhungThoiGian({ surveyId: this.survey.doiTuongId, kiKhaoSatId: this.kiKhaosatId, doiTuongId: this.doiTuongId }).subscribe({
      next: (data) => {
        console.log(data);
        this.lstData = data
      }
    })
  }
}
