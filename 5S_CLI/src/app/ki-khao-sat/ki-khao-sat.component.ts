import { Component } from '@angular/core'
import { ShareModule } from '../shared/share-module'
import { FormGroup, NonNullableFormBuilder, Validators } from '@angular/forms'
import { AccountTypeFilter } from '../models/master-data/account-type.model'
import { PaginationResult } from '../models/base.model'
import { KiKhaoSatService } from '../service/master-data/ki-khao-sat.service'
import { AccountService } from '../service/system-manager/account.service'
import { GlobalService } from '../service/global.service'
import { NzMessageService } from 'ng-zorro-antd/message'
import { ActivatedRoute } from '@angular/router'
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
  DataChamdiem:any=[]
  headerId : any = ""
  DataKS:any=[]

  constructor(
    private _service: KiKhaoSatService,
    private route: ActivatedRoute,
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
      SurveyMgmtId: [this.headerId,],
      // NguoichamDiem: [[], [Validators.required]],

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
    this.route.paramMap.subscribe({
      next: (params) => {
        this.headerId = params.get('id')
        this.filter.keyWord = this.headerId
      },
    })
    this.search()
    this.getAlldata(this.headerId)
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
  getAlldata(headerId: string) {
    this._service.getAlldata(headerId).subscribe({
      next: (data) => {
      this.DataChamdiem=data
     

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
   this.validateForm.get('SurveyMgmtId')?.setValue(this.headerId);
   const formData = this.validateForm.getRawValue()
      const payload = {
          ...formData,
       Chamdiemlst: this.DataChamdiem 
      };
    this.isSubmit = true
    if (this.validateForm.valid) {

      if (this.edit) {
        this._service
          .update(payload)
          .subscribe({
            next: (data) => {
              this.search()
            },
            error: (response) => {
              console.log(response)
            },
          })
      } else {

        
      
       console.log(this.DataChamdiem)
        this._service
          .create(payload)
          .subscribe({
            next: (data) => {
              this.search()
              this.visible = false
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
    this.reset()
  }

  close() {
    this.visible = false
    this.resetForm()
  }

  reset() {
  
    this.search()
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

  openEdit(data: { code: string; name: number; isActive: boolean , des: string, startDate: Date, endDate: Date}) {
   this.Getdataki(data.code)
   
    this.validateForm.setValue({
      Code: data.code,
      Name: data.name,
      isActive: data.isActive,
      Des: data.des,
      StartDate: data.startDate,
      EndDate: data.endDate,
      SurveyMgmtId: this.headerId,
    })
    setTimeout(() => {
      this.edit = true
      this.visible = true
    }, 200)
  }

  Getdataki(code: string) {
    this._service.getAll(code).subscribe({
      next: (data) => {
        this.DataKS = data
       this.DataChamdiem = this.DataChamdiem.map((store: any) => {

  const nguoiChamDiemArr = this.DataKS
    .filter((x:any) => x.storeId === store.ma)
    .map((x:any) => x.userName);

  return {
   ...store,
    nguoiChamDiem: nguoiChamDiemArr // gán vào thuộc tính này
  };
});

      },
      error: (response) => {
        console.log(response)
      },
    })
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
