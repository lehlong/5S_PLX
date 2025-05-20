import { Component, ViewChild } from '@angular/core';
import { ShareModule } from '../shared/share-module';
import { FormGroup, NonNullableFormBuilder, Validators } from '@angular/forms';
import { AccountTypeFilter } from '../models/master-data/account-type.model';
import { PaginationResult } from '../models/base.model';
import { KiKhaoSatService } from '../service/master-data/ki-khao-sat.service';
import { AccountService } from '../service/system-manager/account.service';
import { GlobalService } from '../service/global.service';
import { NzMessageService } from 'ng-zorro-antd/message';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { NzTreeComponent, NzFormatEmitEvent } from 'ng-zorro-antd/tree';
import { NzUploadChangeParam } from 'ng-zorro-antd/upload';
import { TreeTieuChiService } from '../service/business/tree-tieu-chi.service';
import { DragDropModule, CdkDragDrop, moveItemInArray } from '@angular/cdk/drag-drop';
@Component({
  selector: 'app-ki-khao-sat',
  imports: [ShareModule, RouterModule,DragDropModule],
  standalone: true,
  templateUrl: './ki-khao-sat.component.html',
  styleUrl: './ki-khao-sat.component.scss',
})
export class KiKhaoSatComponent {
  @ViewChild('treeCom', { static: false }) treeCom!: NzTreeComponent;

  validateForm: FormGroup;
  isSubmit: boolean = false;
  visible: boolean = false;
  treeVisible: boolean = false;
  edit: boolean = false;
  EndDate: Date | null = null;
  drawerVisible = false;
  selectedNode: any = null;
  selectedNodeDetails: any[] = [];
  searchValue = '';
  treeData: any = [];
  filter = new AccountTypeFilter();
  paginationResult = new PaginationResult();
  loading: boolean = false;
  Account: any = [];
  dataChamdiem: any = [];
  headerId: any = '';
  kiKhaoSatId: any = '';
  calculationRows: { moTa: string; diem: number }[] = [];
  DataKS: any = [];
  kiKhaoSat: any = {};
  visibleKiKhaoSat: boolean = false;
  leavesNode = {
    id: '',
    name: '',
    title: '',
    isExpanded: true,
    pId: '',
    kiKhaoSatId: '-',
    orderNumber: 0,
    isGroup: false,
    isImg: false,
    numberImg: 0,
    report: '',
    chiChtAtvsv: false,
  };
  tree: any = [];
  leavesVisible: boolean = false;
  dataInsertTree: any = {};
  treeId: any = '';

  constructor(
    private _service: KiKhaoSatService,
    private route: ActivatedRoute,
    private fb: NonNullableFormBuilder,
    private globalService: GlobalService,
    private messageService: NzMessageService,
    private router: Router,
    private _treeTieuChiService: TreeTieuChiService,
    private message: NzMessageService,
    private accountService: AccountService
  ) {
    this.validateForm = this.fb.group({
      Code: [''],
      Name: ['', [Validators.required]],
      Des: ['', [Validators.required]],
      StartDate: ['', [Validators.required]],
      EndDate: ['', [Validators.required]],
      isActive: [true, [Validators.required]],
      SurveyMgmtId: [this.headerId],
      // NguoichamDiem: [[], [Validators.required]],
    });
    this.globalService.setBreadcrumb([
      {
        name: 'Danh sách kì khảo sát',
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
    this.route.paramMap.subscribe({
      next: (params) => {
        this.headerId = params.get('id');
        this.filter.keyWord = this.headerId;
      },
    });
    this.search();
    this.getAlldata(this.headerId);
    this.getAllAccount();
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
  getAlldata(headerId: string) {
    this._service.getAlldata(headerId).subscribe({
      next: (data) => {
        this.dataChamdiem = data;
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

    this.kiKhaoSat.surveyMgmtId = this.headerId
    this.kiKhaoSat.isActive = true
    const { chamdiemlst, ...eventData } = this.kiKhaoSat;

    const payload = {
      ...eventData,
      Chamdiemlst: this.dataChamdiem

    };
    console.log(payload)

    this.isSubmit = true;
    // if (this.validateForm.valid) {

    if (this.edit) {
      this._service.update(payload).subscribe({
        next: (data) => {
          this.search();
        },
        error: (response) => {
          console.log(response);
        },
      });
    } else {
      console.log(this.dataChamdiem);
      this._service.create(payload).subscribe({
        next: (data) => {
          this.search();
          this.visible = false;
        },
        error: (response) => {
          console.log(response);
        },
      });
    }
    // } else {
    //   Object.values(this.validateForm.controls).forEach((control) => {
    //     if (control.invalid) {
    //       control.markAsDirty()
    //       control.updateValueAndValidity({ onlySelf: true })
    //     }
    //   })
    // }
    this.reset();
  }

  openCreateChild(node: any) {
    this.close();
    this.edit = false;
    this.visible = true;
    this.validateForm.get('pId')?.setValue(node?.origin.id);
    this.validateForm.get('orderNumber')?.setValue(null);
    this.validateForm.get('children')?.setValue([]);
  }
  addCalculationRow(): void {
    this.calculationRows.push({ moTa: '', diem: 0 });
  }

  removeCalculationRow(index: number): void {
    this.calculationRows.splice(index, 1);
  }

  close() {
     if (this.dataChamdiem ) {
    this.dataChamdiem.forEach((item: any) => {
      item.nguoiChamDiem = [];
    });
  }
    this.visible = false;
    this.visibleKiKhaoSat = false;
    this.resetForm();
    this.kiKhaoSat = [];
   
  }

  closeDrawer(): void {
    this.drawerVisible = false;
  }

  closeModal(): void {
    this.treeVisible = false;
    this.resetForm();
    this.leavesVisible = false;
  }

  onClick(node: any) {
    if (node.origin.children == null) {
      this.treeId = node.origin.id;
      this._treeTieuChiService.GetTreeLeaves(this.treeId).subscribe({
        next: (data) => {
          this.selectedNodeDetails = data.result;
        },

        error: (response) => {
          console.log(response);
        },
      });
    } else{
      this.treeId = '';
    }
  }

  openTieuchi(id: string) {
    console.log(id);
    this.router.navigate([`danh-gia-tieu-chi/${id}`]);
  }

  reset() {
    this.search();
  }
  getAllAccount() {
    this.accountService.getall().subscribe({
      next: (data) => {
        this.Account = data;
      },

      error: (response) => {
        console.log(response);
      },
    });
  }

  // openCreateModal(data:any): void {
  //   this.edit = false;
  //   this.visible = true;
  //   this.treeNode = data;
  // }

  openCreLeaves(data: any): void {
    this.edit = false;
    this.leavesVisible = true;
    this.leavesNode.pId = this.treeId;
  }
  openUpdateLeaves(data: any): void {
    this.leavesNode = data;
    this.edit = true;
    this.leavesVisible = true;
    this.leavesNode.pId = this.treeId;
    this.calculationRows = data.diemTieuChi
  }
  
  openCreate() {
    this.edit = false;
    this.visibleKiKhaoSat = true;
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

  nzEvent(event: NzFormatEmitEvent): void {}

  onDrop(event: any) {}

  onDragStart(event: any) {}

  openEditKyKhaoSat(data: any) {

    this.Getdataki(data.code)
    this.kiKhaoSat = data

    console.log(this.kiKhaoSat)

    setTimeout(() => {
      this.edit = true;
      this.visibleKiKhaoSat = true;
    }, 200);
  }

  openDrawer(param: any): void {
    this.drawerVisible = true;
    this.kiKhaoSatId = param;
    this.GetTreeTieuChi();
    
  }

  GetTreeTieuChi(){
    this._treeTieuChiService.GetTreeTieuChi(this.kiKhaoSatId).subscribe((res) => {
      console.log(res);

      this.treeData = [res];
    });
  }
  Getdataki(code: string) {

    this._service.getAll(code).subscribe({
      next: (data) => {
        console.log('dataki', data);
        this.DataKS = data;
        this.dataChamdiem = this.dataChamdiem.map((item: any) => {
          const nguoiChamDiemArr = this.DataKS
            .filter((x: any) => x.storeId === item.store.id)
            .map((x: any) => x.userName);
          return {
            ...item,
            nguoiChamDiem: nguoiChamDiemArr
          };
        });
          console.log('listcd', this.dataChamdiem)

      },
      error: (response) => {
        console.log(response);
      },
    });
  }
  pageSizeChange(size: number): void {
    this.filter.currentPage = 1;
    this.filter.pageSize = size;
    this.search();
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
    this.filter.currentPage = index;
    this.search();
  }

  openModalTree(data: any): void {
    this.treeVisible = true;
    this.tree = data.origin;
    this.dataInsertTree.pId = this.tree.id;
    this.dataInsertTree.kiKhaoSatId = this.tree.kiKhaoSatId;
    this.dataInsertTree.isGroup = true;
  }

  submitFormTree(): void {
    this.loading = true;

    this._treeTieuChiService.addTree(this.dataInsertTree).subscribe({
      next: (res) => {
        this.treeVisible = false;
        this.loading = false;
            this.GetTreeTieuChi();

      },
      error: (err) => {
        console.log('Lỗi backend:', err);
        this.loading = false;
      },
    });
  }

  isValid(): boolean {
    return !!this.leavesNode.id && !!this.leavesNode.name;
  }

  updateLeaves(): void {
    this.edit = true;
  }
  insertTree(): void {
    if (!this.isValid()) {
      return;
    }

    const formData = {
      id: this.leavesNode.id,
      name: this.leavesNode.name,
      title: this.leavesNode.name,
      isExpanded: this.leavesNode.isExpanded,
      pId: this.leavesNode.pId,
      kiKhaoSatId: this.leavesNode.kiKhaoSatId,
      orderNumber: this.leavesNode.orderNumber,
      isGroup: this.leavesNode.isGroup,
      isImg: this.leavesNode.isImg,
      numberImg: this.leavesNode.numberImg,
      report: this.leavesNode.report,
      chiChtAtvsv: this.leavesNode.chiChtAtvsv,
      diemTieuChi: this.calculationRows.map((row) => ({
        isActive: true,
        isDeleted: false,
        moTa: row.moTa,
        diem: row.diem,
        tieuChiId: this.leavesNode.id,
      })),
    };

    this._treeTieuChiService.addLeaves(formData).subscribe({
      next: (res) => {
        this.leavesVisible = false;
        this.loading = false;
        this.GetTreeTieuChi();
      },
      error: (err) => {
        this.loading = false;
      },
    });

    this.closeModal();
  }

  drop(event: CdkDragDrop<any[]>): void {
    moveItemInArray(this.selectedNodeDetails, event.previousIndex, event.currentIndex);
    // Có thể thêm logic lưu thứ tự vào backend nếu cần
    console.log('New order:', this.selectedNodeDetails);
  }
}
