import { Component } from '@angular/core';
import { ShareModule } from '../../shared/share-module';
import { FormGroup, NonNullableFormBuilder, Validators } from '@angular/forms';
import { AccountTypeFilter } from '../../models/master-data/account-type.model';
import { PaginationResult } from '../../models/base.model';
import { AccountService } from '../../service/system-manager/account.service';
import { GlobalService } from '../../service/global.service';
import { NzMessageService } from 'ng-zorro-antd/message';
import { ATVSVService } from '../../service/master-data/atvsv.service'; // Import ATVSVService
import { WareHouseService } from '../../service/master-data/ware-house.service';

@Component({
  selector: 'app-store',
  standalone: true,
  imports: [ShareModule],
  templateUrl: './ware-house.component.html',
  styleUrl: './ware-house.component.scss',
})
export class WareHouseComponent {
  validateForm: FormGroup;

  isSubmit: boolean = false;
  visible: boolean = false;
  edit: boolean = false;
  filter = new AccountTypeFilter();
  paginationResult = new PaginationResult();
  loading: boolean = false;
  lstAccount: any[] = [];
  ATVSVList: any[] = [];
  lstKKS: any[] = [];

  constructor(
    private _service: WareHouseService,
    private fb: NonNullableFormBuilder,
    private globalService: GlobalService,
    private message: NzMessageService,
    private accountService: AccountService,
    private atvsvService: ATVSVService
  ) {
    this.validateForm = this.fb.group({
      id: [''],
      name: ['', [Validators.required]],
      isActive: [true, [Validators.required]],
      truongKho: ['', [Validators.required]],
      atvsv: [[], [Validators.required]],
      nguoiPhuTrach: ['', [Validators.required]],
      kinhDo: [''],
      viDo: [''],
    });
    this.globalService.setBreadcrumb([
      {
        name: 'Danh sách kho xăng dầu',
        path: 'master-data/ware-house',
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
    this.getAllATVSV();
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
        this.lstKKS = data
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

  getAllATVSV() {
    this.atvsvService.getAll().subscribe({
      next: (data) => {
        this.ATVSVList = data;
        
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
    this.validateForm.reset();
    this.validateForm.patchValue({
      isActive: true,
    });
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
  getATVSV(dataKho: any) {
    this._service.getATVSV(dataKho.id).subscribe({
      next: (data) => {
      
        this.ATVSVList = data;
      this.validateForm.patchValue({
      id: dataKho.id,
      name: dataKho.name,
      isActive: dataKho.isActive,
      truongKho: dataKho.truongKho,
      atvsv: this.ATVSVList,
      nguoiPhuTrach: dataKho.nguoiPhuTrach || '',
      kinhDo: dataKho.kinhDo || '',
      viDo: dataKho.viDo || '',
    });
      },
      error: (response) => {
        console.log(response);

      }
    });
  }


  openEdit(data: any) {
    this.getATVSV(data);
    console.log(data);
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
}
