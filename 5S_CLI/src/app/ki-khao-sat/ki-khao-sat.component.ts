import { Component } from '@angular/core'
import { ShareModule } from '../shared/share-module'
import { FormGroup, NonNullableFormBuilder, Validators } from '@angular/forms'
import { AccountTypeFilter } from '../models/master-data/account-type.model'
import { PaginationResult } from '../models/base.model'
import { KiKhaoSatService } from '../service/master-data/ki-khao-sat.service'
import { AccountService } from '../service/system-manager/account.service'
import { GlobalService } from '../service/global.service'
import { NzMessageService } from 'ng-zorro-antd/message'
@Component({
  selector: 'app-ki-khao-sat',
  imports: [ShareModule],
  standalone: true,
  templateUrl: './ki-khao-sat.component.html',
  styleUrl: './ki-khao-sat.component.scss'
})
export class KiKhaoSatComponent {

   validateForm: FormGroup
  isSubmit: boolean = false
  visible: boolean = false
  edit: boolean = false
  EndDate: Date | null = null;
  
  filter = new AccountTypeFilter()
  paginationResult = new PaginationResult()
  loading: boolean = false
  Account:any=[] 
  constructor(
    private _service: KiKhaoSatService,
    private fb: NonNullableFormBuilder,
    private globalService: GlobalService,
    private message: NzMessageService,
    private accountService: AccountService,
  ) {
    this.validateForm= this.fb.group({
      Code: [''],
      Name: ['', [Validators.required]],
      Des: ['', [Validators.required]],
      StartDate: ['', [Validators.required]],
      EndDate: ['', [Validators.required]],
      isActive: [true, [Validators.required]],
      InputStoreId: ['adeqewqsdasdsad',],
     
    })
    this.globalService.setBreadcrumb([
      {
        name: 'Danh sách kiểu người dùng',
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
    this.search()
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

  getAllAccount() {
    this.accountService.getall().subscribe({
      next: (data) => {
        this.Account = data
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
    console.log(this.validateForm.valid)
    console.log(this.validateForm.getRawValue())
    this.isSubmit = true
    if (this.validateForm.valid) {

      if (this.edit) {
        this._service
          .update(this.validateForm.getRawValue())
          .subscribe({
            next: (data) => {
              this.search()
            },
            error: (response) => {
              console.log(response)
            },
          })
      } else {

        const formData = this.validateForm.getRawValue()
       console.log('create')
        this._service
          .create(this.validateForm.getRawValue())
          .subscribe({
            next: (data) => {
              this.search()
            },
            error: (response) => {
              console.log(response)
            },
          })
      }
    } else {
      Object.values(this.validateForm.controls).forEach((control) => {
        if (control.invalid) {
          control.markAsDirty()
          control.updateValueAndValidity({ onlySelf: true })
        }
      })
    }
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
}
