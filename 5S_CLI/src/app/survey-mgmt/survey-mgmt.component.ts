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

@Component({
  selector: 'app-survey-mgmt',
  standalone: true,
  imports: [ShareModule],
  templateUrl: './survey-mgmt.component.html',
  styleUrl: './survey-mgmt.component.scss',
})
export class SurveyMgmtComponent {
  validateForm: FormGroup
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

  dataInput: any = []

  constructor(
    private _service: SurveyMgmtService,
    private _storeService: StoreService,
    private _accountService: AccountService,
    private _doiTuongService: DoiTuongService,
    private _khoService: KhoXangDauService,
    private fb: NonNullableFormBuilder,
    private messageService: NzMessageService,
    private globalService: GlobalService,
    private message: NzMessageService,
  ) {
    this.validateForm= this.fb.group({
      id: [''],
      name: ['', [Validators.required]],
      doiTuongCode: ["DT1", [Validators.required]],
      moTa: ['', [Validators.required]],
      isActive: [true, [Validators.required]],
    })
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

  isCodeExist(code: string): boolean {
    return this.paginationResult.data?.some(
      (accType: any) => accType.id === code,
    )
  }
  submitForm(): void {
    this.isSubmit = true
    console.log(this.lstInputStore);

    // if (this.validateForm.valid) {
    //   if (this.edit) {
    //     this._service
    //       .update(this.validateForm.getRawValue())
    //       .subscribe({
    //         next: (data) => {
    //           this.search()
    //         },
    //         error: (response) => {
    //           console.log(response)
    //         },
    //       })
    //   } else {
    //     const formData = this.validateForm.getRawValue()
    //     if (this.isCodeExist(formData.id)) {
    //       this.message.error(
    //         `Mã kiểu người dùng ${formData.id} đã tồn tại, vui lòng nhập lại`,
    //       )
    //       return
    //     }
    //     this._service
    //       .create(this.validateForm.getRawValue())
    //       .subscribe({
    //         next: (data) => {
    //           this.search()
    //         },
    //         error: (response) => {
    //           console.log(response)
    //         },
    //       })
    //   }
    // } else {
    //   Object.values(this.validateForm.controls).forEach((control) => {
    //     if (control.invalid) {
    //       control.markAsDirty()
    //       control.updateValueAndValidity({ onlySelf: true })
    //     }
    //   })
    // }
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
    this.edit = false
    this.visible = true
  }

  resetForm() {
    this.validateForm.reset()
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

  openEdit(data: { id: string; name: number; isActive: boolean }) {
    this.validateForm.setValue({
      id: data.id,
      name: data.name,
      isActive: data.isActive,
    })
    setTimeout(() => {
      this.edit = true
      this.visible = true
    }, 200)
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

  buildInput(){
    this._service.buildInput().subscribe({
      next: (data) => {
        this.dataInput = data
      }
    })
  }

  getTblCreate(id : any){
    console.log(id);
    if(id == "DT1"){
      this.lstStore()
    }else if(id == "2b1d51ee-9c22-4b2a-81fb-b22b26c1deae"){
      this.lstKho()
    }
  }

  setOfCheckedId = new Set<number>();
  lstCheckedId : any = []

  onAllChecked(value: boolean): void {
    this.lstCheckedId = []
    if (value) {
      this.lstStore.forEach((i: any) => {
          this.lstInputStore.push(i)
          this.lstCheckedId.push(i.id)
      })
    } else {
      this.lstInputStore = []
      this.lstCheckedId = []
    }
  }
  updateCheckedSet(code: any, checked: boolean,): void {
    if (checked) {
      this.lstCheckedId.push(code)
      this.lstStore.forEach((i: any) => {
        if(i.id == code){
          this.lstInputStore.push(i)
        }
      })
    } else {
      this.lstCheckedId = this.lstCheckedId.filter(
        (x: any) => x !== code
      )
      this.lstStore.forEach((i: any) => {
        this.lstInputStore = this.lstInputStore.filter(
          (x: any) => x.id !== code
        )
      })
    }
  }
  onItemChecked(code: String, checked: boolean,): void {
    this.updateCheckedSet(code, checked)
  }
  isCheckedId(code: string): boolean {
    return this.lstCheckedId.some((item : any) => item == code)
  }

}
