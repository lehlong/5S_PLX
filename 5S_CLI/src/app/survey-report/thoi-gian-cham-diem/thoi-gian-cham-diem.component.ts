import { Component } from '@angular/core';
import { ShareModule } from '../../shared/share-module';
import { FormGroup, NonNullableFormBuilder, Validators } from '@angular/forms';
import { AccountTypeFilter } from '../../models/master-data/account-type.model';
import { PaginationResult } from '../../models/base.model';
import { StoreService } from '../../service/master-data/store.service';
import { AccountService } from '../../service/system-manager/account.service';
import { GlobalService } from '../../service/global.service';
import { NzMessageService } from 'ng-zorro-antd/message';
import { SurveyMgmtService } from '../../service/business/survey-mgmt.service';
import { KiKhaoSatService } from '../../service/master-data/ki-khao-sat.service';
import { AppReportService } from '../../service/business/app-report.service';
import { RemovePrefixPipe } from '../../shared/custom-pipe/remove-prefix.pipe';

@Component({
  selector: 'app-thoi-gian-cham-diem',
  standalone: true,
  imports: [ShareModule, RemovePrefixPipe],
  templateUrl: './thoi-gian-cham-diem.component.html',
  styleUrl: './thoi-gian-cham-diem.component.scss',
})
export class ThoiGianChamDiemComponent {
  validateForm: FormGroup;
  isSubmit: boolean = false;
  visible: boolean = false;
  edit: boolean = false;
  filter = new AccountTypeFilter();
  paginationResult = new PaginationResult();
  loading: boolean = false;
  lstATVSV = [];
  lstAccount: any = [];
  text: any = '';

  lstSurvey: any = [];
  lstKiKhaoSat: any = [];
  lstAllKiKhaoSat: any = [];
  lstDoiTuong: any = [];
  survey: any = {};
  kiKhaosatId: any = null;
  doiTuongId: any = null;
  lstData: any = [];
  maxCht = 0;
  maxAtvsv = 0;
  maxChuyenGia = 0;

  constructor(
    private _appReportService: AppReportService,
    private _kiKhaoSatService: KiKhaoSatService,
    private _surveyService: SurveyMgmtService,
    private _service: StoreService,
    private fb: NonNullableFormBuilder,
    private globalService: GlobalService,
    private message: NzMessageService,
    private accountService: AccountService
  ) {
    this.validateForm = this.fb.group({
      id: ['', [Validators.required]],
      name: ['', [Validators.required]],
      isActive: [true, [Validators.required]],
      TrangThaiCuaHang: [true],
      CuaHangTruong: ['', [Validators.required]],
      NguoiPhuTrach: ['', [Validators.required]],
    });
    this.globalService.setBreadcrumb([
      {
        name: 'Báo Cáo',
        path: 'survey-report/cham-diem-theo-khung-thoi-gian',
      },
    ]);
    this.globalService.getLoading().subscribe((value) => {
      this.loading = value;
    });
  }
  ngOnDestroy() {
    this.globalService.setBreadcrumb([]);
  }

  ngOnInit(): void {
    this.search();
    this.getAllAccount();
    this.getAllKiKhaoSat();
    this.getAllSurvey();
  }

  onSortChange(name: string, value: any) {
    this.filter = {
      ...this.filter,
      SortColumn: name,
      IsDescending: value === 'descend',
    };
    this.search();
  }

  search() {
    this.isSubmit = false;
    this._service.search(this.filter).subscribe({
      next: (data) => {
        this.paginationResult = data;
      },
      error: (response) => {
        console.log(response);
      },
    });
  }

  getAllAccount() {
    this.accountService.getall().subscribe({
      next: (data) => {
        this.lstAccount = data;
      },
      error: (response) => {
        console.log(response);
      },
    });
  }

  isCodeExist(code: string): boolean {
    return this.paginationResult.data?.some(
      (accType: any) => accType.id === code
    );
  }
  submitForm(): void {
    console.log(this.validateForm.valid);
    console.log(this.validateForm.getRawValue());
    this.isSubmit = true;
    if (this.validateForm.valid) {
      if (this.edit) {
        this._service.update(this.validateForm.getRawValue()).subscribe({
          next: (data) => {
            this.search();
          },
          error: (response) => {
            console.log(response);
          },
        });
      } else {
        const formData = this.validateForm.getRawValue();
        console.log('create');
        this._service.create(this.validateForm.getRawValue()).subscribe({
          next: (data) => {
            this.search();
          },
          error: (response) => {
            console.log(response);
          },
        });
      }
    } else {
      Object.values(this.validateForm.controls).forEach((control) => {
        if (control.invalid) {
          control.markAsDirty();
          control.updateValueAndValidity({ onlySelf: true });
        }
      });
    }
  }

  close() {
    this.visible = false;
    this.resetForm();
  }

  reset() {
    this.filter = new AccountTypeFilter();
    this.search();
  }

  openCreate() {
    this.edit = false;
    this.visible = true;
  }

  resetForm() {
    this.validateForm.reset();
    this.isSubmit = false;
  }

  deleteItem(id: string) {
    this._service.delete(id).subscribe({
      next: (data) => {
        this.search();
      },
      error: (response) => {
        console.log(response);
      },
    });
  }
  GetATVSV(params: any) {
    this._service.getATVSV(params).subscribe({
      next: (data) => {
        this.lstATVSV = data;
      },
      error: (response) => {
        console.log(response);
      },
    });
  }
  openEdit(data: any) {
    var storeid = data.id;
    this.GetATVSV(storeid);
    this.validateForm.setValue({
      id: data.id,
      name: data.name,
      KinhDo: data.kinhDo,
      ViDo: data.viDo,
      PhoneNumber: data.phoneNumber,
      TrangThaiCuaHang: data.trangThaiCuaHang,
      CuaHangTruong: data.cuaHangTruong,
      ATVSV: this.lstATVSV,
      NguoiPhuTrach: data.nguoiPhuTrach,

      isActive: data.isActive,
    });
    setTimeout(() => {
      this.edit = true;
      this.visible = true;
    }, 200);
  }

  pageSizeChange(size: number): void {
    this.filter.currentPage = 1;
    this.filter.pageSize = size;
    this.search();
  }

  pageIndexChange(index: number): void {
    this.filter.currentPage = index;
    this.search();
  }

  getNameByCodeAccount(code: any) {
    const item = this.lstAccount.find((x: any) => x.userName === code);
    return item ? item.fullName : code;
  }

  //update
  searchKiKhaoSat() {
    console.log(this.survey.id);
    this.lstKiKhaoSat = this.lstAllKiKhaoSat.filter(
      (x: any) => x.surveyMgmtId == this.survey.id
    );
    this.kiKhaosatId = this.lstKiKhaoSat.reduce((a: any, b: any) =>
      new Date(a.endDate) > new Date(b.endDate) ? a.id : b.id
    );
  }
  searchDoiTuong() {
    this._kiKhaoSatService.getInputKiKhaoSat(this.kiKhaosatId).subscribe({
      next: (data) => {
        console.log(data);
        if (data.lstInputStore?.length != 0) {
          this.lstDoiTuong = data.lstInputStore;
        } else {
          this.lstDoiTuong = data.lstInputWareHouse;
        }
      },
      error: (response) => {
        console.log(response);
      },
    });
  }
  getReport() {
    this._appReportService
      .ThoiGianChamDiem({
        surveyId: this.survey.doiTuongId,
        kiKhaoSatId: this.kiKhaosatId,
        doiTuongId: this.doiTuongId,
      })
      .subscribe({
        next: (data) => {
          console.log(data);
          this.lstData = data;
          this.maxCht = Math.max(
            ...this.lstData.map((x: any) => x.cht.length || 0)
          );
          this.maxAtvsv = Math.max(
            ...this.lstData.map((x: any) => x.atvsv.length || 0)
          );
          this.maxChuyenGia = Math.max(
            ...this.lstData.map((x: any) => x.chuyenGia.length || 0)
          );
          console.log('maxCht', this.maxCht);
          console.log('maxAtvsv', this.maxAtvsv);
          console.log('maxChuyenGia', this.maxChuyenGia);
        },
      });
  }

  getAllKiKhaoSat() {
    this._kiKhaoSatService.search(this.filter).subscribe({
      next: (data) => {
        console.log(data);
        this.lstAllKiKhaoSat = data.data;
      },
      error: (response) => {
        console.log(response);
      },
    });
  }
  getAllSurvey() {
    this._surveyService.search(this.filter).subscribe({
      next: (data) => {
        this.lstSurvey = data.data;
        console.log(data);
      },
      error: (response) => {
        console.log(response);
      },
    });
  }

  exportExcel() {}
  getRange(n: number): number[] {
    return Array.from({ length: n > 0 ? n : 1 }, (_, i) => i);
  }
}
