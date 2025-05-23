import { Component } from '@angular/core'
import { FormGroup, NonNullableFormBuilder, Validators } from '@angular/forms'
import { NzMessageService } from 'ng-zorro-antd/message'
import { ShareModule } from '../shared/share-module'
import { PaginationResult } from '../models/base.model'
import { AccountTypeFilter } from '../models/master-data/account-type.model'
import { GlobalService } from '../service/global.service'
import { SurveyMgmtService } from '../service/business/survey-mgmt.service'
import { NzUploadChangeParam } from 'ng-zorro-antd/upload'
import { StoreService } from '../service/master-data/store.service'
import { DoiTuongService } from '../service/master-data/doi-tuong.service'
import { KhoXangDauService } from '../service/master-data/kho-xang-dau.service'
import { AccountService } from '../service/system-manager/account.service'
import { Router } from '@angular/router'

@Component({
  selector: 'app-survey-mgmt',
  standalone: true,
  imports: [ShareModule],
  templateUrl: './survey-mgmt.component.html',
  styleUrl: './survey-mgmt.component.scss',
})
export class SurveyMgmtComponent {
  isSubmit: boolean = false
  visible: boolean = false
  edit: boolean = false
  filter = new AccountTypeFilter()
  paginationResult = new PaginationResult()
  loading: boolean = false
  lstStore: any = []
  lstKho: any = []
  lstAccount: any = []
  lstInputStore: any = []
  lstDoiTuong: any = []
  checked = false;
  indeterminate = false;
  isKho: boolean = false
  isStore: boolean = false

  dataInput: any = {
    inputStores: [],
    surveyMgmt: {}
  }

  constructor(
    private _service: SurveyMgmtService,
    private _storeService: StoreService,
    private _accountService: AccountService,
    private _doiTuongService: DoiTuongService,
    private _khoService: KhoXangDauService,
    private router: Router,
    private fb: NonNullableFormBuilder,
    private messageService: NzMessageService,
    private globalService: GlobalService,
    private message: NzMessageService,
  ) {
    this.globalService.setBreadcrumb([
      {
        name: 'Danh sách khảo sát',
        path: 'master-data/survey-mgmt',
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
    this.search()
    this.getAllStore()
    this.getAllDoiTuong()
    this.getAllAccount()
  }

  onSortChange(name: string, value: any) {
    this.filter = {
      ...this.filter,
      SortColumn: name,
      IsDescending: value === 'descend',
    }
    this.search()
  }

  search() {
    this.isSubmit = false
    this._service.search(this.filter).subscribe({
      next: (data) => {
        this.paginationResult = data
      },
      error: (response) => {
        console.log(response)
      },
    })
  }

  getAllStore() {
    this.isSubmit = false
    this.isStore = true
    this.isKho = false
    this._storeService.getAll().subscribe({
      next: (data) => {
        this.lstStore = data
      },
      error: (response) => {
        console.log(response)
      },
    })
  }

  getAllKho() {
    this.isSubmit = false
    this.isStore = false
    this.isKho = true
    this._khoService.getAll().subscribe({
      next: (data) => {
        this.lstKho = data
      },
      error: (response) => {
        console.log(response)
      },
    })
  }

  getAllAccount() {
    this._accountService.getall().subscribe({
      next: (data) => {
        this.lstAccount = data
      },
      error: (response) => {
        console.log(response)
      },
    })
  }

  getAllDoiTuong() {
    this.isSubmit = false
    this._doiTuongService.getAll().subscribe({
      next: (data) => {
        this.lstDoiTuong = data
      },
      error: (response) => {
        console.log(response)
      },
    })
  }

  getInput(id: any){
    this._service.getInput(id).subscribe({
      next: (data) => {
        this.dataInput = data
      },
    })
    setTimeout(() => {
      this.edit = true
      this.visible = true
    }, 200)
  }

  isCodeExist(code: string): boolean {
    return this.paginationResult.data?.some(
      (accType: any) => accType.id === code,
    )
  }

  submitForm(): void {
    this.isSubmit = true
    console.log(this.dataInput);
    this._service.create(this.dataInput).subscribe({
      next:(data) =>{
        this.search()
      }
    })
  }

  updateInput(): void {
    this.isSubmit = true
    console.log(this.dataInput);
    this._service.update(this.dataInput).subscribe({
      next:(data) =>{
        this.search()
      }
    })
  }
  close() {
    this.visible = false
    this.resetForm()
  }

  reset() {
    this.filter = new AccountTypeFilter()
    this.search()
  }

  openCreate() {
    this.buildInput("DT1")
    this.edit = false
    this.visible = true
  }

  resetForm() {
    this.isSubmit = false
  }

  deleteItem(id: string) {
    this._service.delete(id).subscribe({
      next: (data) => {
        this.search()
      },
      error: (response) => {
        console.log(response)
      },
    })
  }

  openKiKhaoSat(data: any) {
    this.router.navigate([`/ki-khao-sat/${data}`]);
  }

  pageSizeChange(size: number): void {
    this.filter.currentPage = 1
    this.filter.pageSize = size
    this.search()
  }

  pageIndexChange(index: number): void {
    this.filter.currentPage = index
    this.search()
  }


  handleChange(info: NzUploadChangeParam): void {
    if (info.file.status !== 'uploading') {
      console.log(info.file, info.fileList);
    }
    if (info.file.status === 'done') {
      this.messageService.success(`${info.file.name} file uploaded successfully`);
    } else if (info.file.status === 'error') {
      this.messageService.error(`${info.file.name} file upload failed.`);
    }
  }

  buildInput(doiTuongId: any) {
    this._service.buildInput(doiTuongId).subscribe({
      next: (data) => {
        console.log(data.doiTuongId);

        this.dataInput = data
      }
    })
  }

  onAllChecked(value: boolean): void {
    if (value) {
      this.dataInput.inputStores.forEach((item: any) => {
        item.inputStore.isActive = true;
      })
    } else {
      this.dataInput.inputStores.forEach((item: any) => {
        item.inputStore.isActive = false;
      })
    }
  }
  updateCheckedSet(code: any, checked: boolean,): void {
    if (checked) {
      this.dataInput.inputStores.forEach((item: any) => {
        if (item.inputStore.id === code) {
          item.inputStore.isActive = true;
        }
      });
    } else {
      this.dataInput.inputStores.forEach((item: any) => {
        if (item.inputStore.id === code) {
          item.inputStore.isActive = false;
        }
      });
    }
  }
  onItemChecked(code: String, checked: boolean,): void {
    this.updateCheckedSet(code, checked)
  }

}
