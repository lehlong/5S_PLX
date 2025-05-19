import { Component, ViewChild } from '@angular/core'
import { ShareModule } from '../shared/share-module'
import { FormGroup, NonNullableFormBuilder, Validators } from '@angular/forms'
import { AccountTypeFilter } from '../models/master-data/account-type.model'
import { PaginationResult } from '../models/base.model'
import { KiKhaoSatService } from '../service/master-data/ki-khao-sat.service'
import { AccountService } from '../service/system-manager/account.service'
import { GlobalService } from '../service/global.service'
import { NzMessageService } from 'ng-zorro-antd/message'
import { ActivatedRoute, Router, RouterModule } from '@angular/router'
import { NzTreeComponent, NzFormatEmitEvent } from 'ng-zorro-antd/tree'
import { NzUploadChangeParam } from 'ng-zorro-antd/upload'
import { TreeTieuChiService } from '../service/business/tree-tieu-chi.service'
@Component({
  selector: 'app-ki-khao-sat',
  imports: [ShareModule, RouterModule],
  standalone: true,
  templateUrl: './ki-khao-sat.component.html',
  styleUrl: './ki-khao-sat.component.scss'
})
export class KiKhaoSatComponent {
  @ViewChild('treeCom', { static: false }) treeCom!: NzTreeComponent;

  validateForm: FormGroup
  isSubmit: boolean = false
  visible: boolean = false
  treeVisible: boolean = false
  edit: boolean = false
  EndDate: Date | null = null;
  drawerVisible = false;
  selectedNode: any = null;
  selectedNodeDetails: any[] = [];
  searchValue = '';
  treeData: any = [];
  filter = new AccountTypeFilter()
  paginationResult = new PaginationResult()
  loading: boolean = false
  Account: any = []
  dataChamdiem: any = []
  headerId: any = ""
  calculationRows: any[] = [];
  DataKS: any = []
  kiKhaoSat: any = {}
  visibleKiKhaoSat: boolean = false
  treeNode: any = [];
  tree: any = []
  dataInsertTree: any = []

  constructor(
    private _service: KiKhaoSatService,
    private route: ActivatedRoute,
    private fb: NonNullableFormBuilder,
    private globalService: GlobalService,
    private messageService: NzMessageService,
    private router: Router,
    private _treeTieuChiService: TreeTieuChiService,
    private message: NzMessageService,
    private accountService: AccountService,
  ) {
    this.validateForm = this.fb.group({
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
        name: 'Danh sách kì khảo sát',
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
        this.dataChamdiem = data


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
    this.kiKhaoSat.surveyMgmtId = this.headerId
    this.kiKhaoSat.isActive = true

    const payload = {
      ...this.kiKhaoSat,
      Chamdiemlst: this.dataChamdiem
    };

    this.isSubmit = true
    // if (this.validateForm.valid) {

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
       console.log(this.dataChamdiem)
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
    // } else {
    //   Object.values(this.validateForm.controls).forEach((control) => {
    //     if (control.invalid) {
    //       control.markAsDirty()
    //       control.updateValueAndValidity({ onlySelf: true })
    //     }
    //   })
    // }
    this.reset()
  }

  openCreateChild(node: any) {
    this.close()
    this.edit = false
    this.visible = true
    this.validateForm.get('pId')?.setValue(node?.origin.id)
    this.validateForm.get('orderNumber')?.setValue(null)
    this.validateForm.get('children')?.setValue([])
  }
  addCalculationRow(): void {
    this.calculationRows.push({ description: '', score: 0 });
  }

  removeCalculationRow(index: number): void {
    this.calculationRows.splice(index, 1);
  }

  close() {
    this.visible = false
    this.visibleKiKhaoSat = false
    this.resetForm()
    this.kiKhaoSat = []
  }

  closeDrawer(): void {
    this.drawerVisible = false;
  }

  closeModal(): void {
    this.treeVisible = false;
    this.resetForm()
  }


  onClick(node: any) {
    console.log('Node clicked:', node);
  }

  openTieuchi(id: string) {
    console.log(id)
    this.router.navigate([`danh-gia-tieu-chi/${id}`])

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

  openCreateModal(data:any): void {
    this.edit = false;
    this.visible = true;
    this.treeNode = data;
  }

  openCreate() {
    this.edit = false
    this.visibleKiKhaoSat = true
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

  nzEvent(event: NzFormatEmitEvent): void { }

  onDrop(event: any) { }

  onDragStart(event: any) { }

  openEditKyKhaoSat(data: any) {
    this.Getdataki(data.code)
    this.kiKhaoSat = data
    setTimeout(() => {
      this.edit = true
      this.visibleKiKhaoSat = true
    }, 200)
  }

  openDrawer(param : any): void {
    this.drawerVisible = true;
    this._treeTieuChiService.GetTreeTieuChi(param).subscribe((res) => {
      console.log(res);

      this.treeData = [res];
    })
  }

  Getdataki(code: string) {
    this._service.getAll(code).subscribe({
      next: (data) => {
        this.DataKS = data
        this.dataChamdiem = this.dataChamdiem.map((store: any) => {

          const nguoiChamDiemArr = this.DataKS
            .filter((x: any) => x.storeId === store.ma)
            .map((x: any) => x.userName);

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

  handleChange(info: NzUploadChangeParam): void {
    if (info.file.status !== 'uploading') {
      console.log(info.file, info.fileList);
    }
    if (info.file.status === 'done') {
      this.messageService.success(
        `${info.file.name} file uploaded successfully`
      );
    } else if (info.file.status === 'error') {
      this.messageService.error(`${info.file.name} file upload failed.`);
    }
  }
  pageIndexChange(index: number): void {
    this.filter.currentPage = index
    this.search()
  }

  openModalTree(data: any): void {
    this.treeVisible = true;
    this.tree= data.origin;
    console.log("11111",this.tree);
    this.dataInsertTree.pId = this.tree.id
    this.dataInsertTree.kiKhaoSatId = this.tree.kiKhaoSatId
    this.dataInsertTree.isGroup = true
  }

  submitFormTree(): void {
    this.loading = true;
    console.log("222222",this.dataInsertTree);
    this._treeTieuChiService.Insert(this.dataInsertTree).subscribe({
      next: (res) => {
        this.messageService.success('Thêm tiêu chí thành công!');
        this.treeVisible = false;
        this.loading = false;
      },
      error: (err) => {
        console.log('Lỗi backend:', err);
        this.messageService.error('Thêm tiêu chí thất bại!');
        this.loading = false;
      }
    });
  }
}
