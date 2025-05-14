import { Component } from '@angular/core'
import { FormGroup, NonNullableFormBuilder, Validators } from '@angular/forms'
import { NzMessageService } from 'ng-zorro-antd/message'
import { ShareModule } from '../shared/share-module'
import { PaginationResult } from '../models/base.model'
import { AccountTypeFilter } from '../models/master-data/account-type.model'
import { GlobalService } from '../service/global.service'
import { SurveyMgmtService } from '../service/business/survey-mgmt.service'
import { NzUploadChangeParam } from 'ng-zorro-antd/upload'

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

  constructor(
    private _service: SurveyMgmtService,
    private fb: NonNullableFormBuilder,
    private messageService: NzMessageService,
    private globalService: GlobalService,
    private message: NzMessageService,
  ) {
    this.validateForm= this.fb.group({
      id: [''],
      name: ['', [Validators.required]],
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


  isCodeExist(code: string): boolean {
    return this.paginationResult.data?.some(
      (accType: any) => accType.id === code,
    )
  }
  submitForm(): void {
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
        if (this.isCodeExist(formData.id)) {
          this.message.error(
            `Mã kiểu người dùng ${formData.id} đã tồn tại, vui lòng nhập lại`,
          )
          return
        }
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

}
