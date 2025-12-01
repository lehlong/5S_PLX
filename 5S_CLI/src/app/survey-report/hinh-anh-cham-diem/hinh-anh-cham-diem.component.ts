import { Component } from '@angular/core';
import { FormGroup, NonNullableFormBuilder, Validators } from '@angular/forms';
import { NzMessageService } from 'ng-zorro-antd/message';
import { environment } from '../../../environments/environment';
import { PaginationResult } from '../../models/base.model';
import { AccountTypeFilter } from '../../models/master-data/account-type.model';
import { AppReportService } from '../../service/business/app-report.service';
import { SurveyMgmtService } from '../../service/business/survey-mgmt.service';
import { GlobalService } from '../../service/global.service';
import { KiKhaoSatService } from '../../service/master-data/ki-khao-sat.service';
import { StoreService } from '../../service/master-data/store.service';
import { AccountService } from '../../service/system-manager/account.service';
import { ShareModule } from '../../shared/share-module';

@Component({
  selector: 'app-hinh-anh-cham-diem',
  imports: [ShareModule],
  templateUrl: './hinh-anh-cham-diem.component.html',
  styleUrl: './hinh-anh-cham-diem.component.scss',
})
export class HinhAnhChamDiemComponent {
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
  urlFiles: string = environment.urlFiles;
  lstSurvey: any = [];
  lstKiKhaoSat: any = [];
  lstAllKiKhaoSat: any = [];
  lstDoiTuong: any = [];
  survey: any = {};
  kiKhaosatId: any = null;
  doiTuongId: any = null;
  lstData: any = [];
  isPreviewVisible: boolean = false;
  previewImageUrl: string = '';

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
        path: 'master-data/account-type',
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
      .BaoCaoHinhAnh({
        surveyId: this.survey.doiTuongId,
        kiKhaoSatId: this.kiKhaosatId,
        doiTuongId: this.doiTuongId,
      })
      .subscribe({
        next: (data) => {
          console.log(data);
          this.lstData = data;
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

  exportExcel() {
    if (!this.survey.doiTuongId && !this.kiKhaosatId) {
      this.message.error('Vui lòng chọn đầy đủ thông tin trước khi xuất file');
      return;
    } else {
      this._appReportService
        .ExportExcel('ChamTheoHinhAnh', {
          surveyId: this.survey.doiTuongId,
          kiKhaoSatId: this.kiKhaosatId,
          doiTuongId: this.doiTuongId,
        })
        .subscribe({
          next: (data) => {
            console.log(data);
            if (data) {
              const downloadUrl = `${environment.urlFiles}${data}`; // hoặc cấu hình phù hợp với backend của bạn
              const a = document.createElement('a');
              a.href = downloadUrl;
              // a.download = `BaoCaoHinhAnh${this.survey.name}_${this.kiKhaosatId}.xlsx`;
              a.target = '_blank'; // mở tab mới (tùy chọn)
              a.click();
              this.message.success('Xuất dữ liệu thành công');
            } else {
              this.message.error('Không có dữ liệu để xuất');
            }
          },
        });
    }
  }
  getImageUrl(path: string | null | undefined): string {
    if (!path) return '';

    const cleanPath = path.replace(/\\/g, '/');
    return `${this.urlFiles}${encodeURI(cleanPath)}`;
  }
  viewDetailImage(imagePath: string | null | undefined): void {
    if (!imagePath) return;

    this.previewImageUrl = this.getImageUrl(imagePath);
    this.isPreviewVisible = true;
    console.log('URL ảnh:', this.previewImageUrl);
  }
}
