import { Component, ViewChild } from '@angular/core';
import { ShareModule } from '../shared/share-module';
import { FormGroup, NonNullableFormBuilder, Validators } from '@angular/forms';
import { AccountTypeFilter } from '../models/master-data/account-type.model';
import { PaginationResult } from '../models/base.model';
import { KiKhaoSatService } from '../service/master-data/ki-khao-sat.service';
import { AccountService } from '../service/system-manager/account.service';
import { KiKhaoSatFilter } from '../models/master-data/ki-khao-sat.model';
import { GlobalService } from '../service/global.service';
import { NzMessageService } from 'ng-zorro-antd/message';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { NzTreeComponent, NzFormatEmitEvent, NzTreeNode } from 'ng-zorro-antd/tree';
import { NzUploadChangeParam } from 'ng-zorro-antd/upload';
import { TreeTieuChiService } from '../service/business/tree-tieu-chi.service';
import { DragDropModule, CdkDragDrop, moveItemInArray } from '@angular/cdk/drag-drop';
import { log } from 'ng-zorro-antd/core/logger';
import { WareHouseService } from '../service/master-data/ware-house.service';
@Component({
  selector: 'app-ki-khao-sat',
  imports: [ShareModule, RouterModule, DragDropModule],
  standalone: true,
  templateUrl: './ki-khao-sat.component.html',
  styleUrl: './ki-khao-sat.component.scss',
})
export class KiKhaoSatComponent {
  @ViewChild('treeCom', { static: false }) treeCom!: NzTreeComponent;

  isSubmit: boolean = false;
  visible: boolean = false;
  treeInsertVisible: boolean = false;
  treeEditVisible: boolean = false;
  edit: boolean = false;
  EndDate: Date | null = null;
  drawerVisible = false;
  selectedNode: any = null;
  selectedNodeDetails: any[] = [];
  searchValue = '';
  filterNguoiChamDiem: string = '';
  treeData: any = [];
  filter = new KiKhaoSatFilter();
  lstInputDoiTuongSearch: any = [];
  paginationResult = new PaginationResult();
  loading: boolean = false;
  lstAccount: any = [];
  lstAllAccount: any = [];
  lstAllDoiTuong: any = []
  headerId: any = '';
  kiKhaoSatId: any = '';
  calculationRows: any = [];
  DataKS: any = [];
  lstKho: any = [];
  kiKhaoSat: any = {};
  checked: boolean = false;
  visibleKiKhaoSat: boolean = false;
  leavesNode: any = {
    Code: "-1",
    diemTieuChi: []
  };
  tree: any = [];
  leavesVisible: boolean = false;
  dataInsertTree: any = {};
  treeId: any = '';
  lstKKS: any = [];
  lstChamDiem: any = [];
  inputKi: any = {
    kiKhaoSat: {},
    lstInputStore: [],
  }
  inputSearchKKS: any = ''
  currentNode: NzTreeNode | undefined;
  parentTitle: string | undefined;

  constructor(
    private _service: KiKhaoSatService,
    private route: ActivatedRoute,
    private fb: NonNullableFormBuilder,
    private globalService: GlobalService,
    private messageService: NzMessageService,
    private _khoService: WareHouseService,
    private router: Router,
    private _treeTieuChiService: TreeTieuChiService,
    private message: NzMessageService,
    private accountService: AccountService
  ) {
    this.globalService.setBreadcrumb([
      {
        name: 'KỲ CHẤM ĐIỂM',
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
        // this.filter.headerId = this.headerId;
      },
    });
    this.search();
    this.getAllAccount();
    this.getAllKho()
  }

  onSortChange(name: string, value: any) {
    this.filter = {
      ...this.filter,
      SortColumn: name,
      IsDescending: value === 'descend',
    };
    this.search();
  }
  searchNguoiChamDiem() {
    this.lstInputDoiTuongSearch = this.lstAllDoiTuong.filter((item: any) =>
      item?.storeId?.toLowerCase().includes(this.filterNguoiChamDiem.toLowerCase()) ||
      item?.wareHouseId?.toLowerCase().includes(this.filterNguoiChamDiem.toLowerCase()) ||
      item.name.toLowerCase().includes(this.filterNguoiChamDiem.toLowerCase()) ||
      item.cuaHangTruong.toLowerCase().includes(this.filterNguoiChamDiem.toLowerCase()) ||
      item.nguoiPhuTrach.toLowerCase().includes(this.filterNguoiChamDiem.toLowerCase())
    );
    // console.log(this.lstInputStoreSearch)
  }

  setInSearchKKS() {
    if (this.inputSearchKKS == '') {
      this.filter.keyWord = this.headerId;
    } else {
      this.filter.keyWord = this.inputSearchKKS
    }
    this.search()
  }

  search() {
    this.isSubmit = false;
    this._service.search(this.filter).subscribe({
      next: (data) => {
        this.paginationResult = data;
        this.lstKKS = data;
      },
      error: (response) => {
        console.log(response);
      },
    });
  }

  isCodeExist(code: string): boolean {
    return this.paginationResult.data?.some(
      (accType: any) => accType.code === code
    );
  }

  onChangeChamDiem(userNames: string[], store: any) {
    this.inputKi.lstInputStore.forEach((i: any) => {
      if (i.storeId == store.storeId) {
        this.inputKi.lstInputStore.lstChamDiem.push({
          id: '',
          userName: i,
          inStoreId: store.storeId,
          kiKhaoSatId: this.kiKhaoSatId,
        })
      }
    });

  }

  onUpdateKiKhaoSat() {
    this._service.updateKiKhaoSat(this.inputKi).subscribe({
      next: (data) => {
        this.search();
        this.visible = false;
      },
      error: (response) => {
        console.log(response);
      },
    });
  }

  onUpdateKhaoSatTrangThai(trangThai: string) {
    this.inputKi.kiKhaoSat.trangThaiKi = trangThai
    this._service.updateKhaoSatTrangThai(this.inputKi.kiKhaoSat).subscribe({
      next: (data) => {
        this.search();
        this.visible = false;
      },
      error: (response) => {
        console.log(response);
      },
    });
  }

  submitForm(): void {
    if (this.isCodeExist(this.inputKi.kiKhaoSat.code)) {
      this.message.error(
        `Mã ${this.inputKi.kiKhaoSat.code} đã tồn tại, vui lòng nhập lại`,
      )
      return
    }
    this._service.create(this.inputKi).subscribe({
      next: (data) => {
        this.search();
        this.visible = false;
      },
      error: (response) => {
        console.log(response);
      },
    });
  }

  DeleteKKS(data: any) {
    this._service.delete(data).subscribe({
      next: (data) => {
        this.search();
      },
      error: (response) => {
        console.log(response);
      },
    });
  }
  openEditTree(node: NzTreeNode): void {
    this.treeEditVisible = true;
    this.currentNode = node;
    const parentNode = node.getParentNode();
    this.parentTitle = parentNode ? parentNode.title : '(Không có tiêu chí cha)';
    this.dataInsertTree = node.origin
    console.log(node.origin);

  }

  deleteTree(data: any): void {
    data.origin.isDeleted = true;
    console.log("object,", data.origin);
    this._treeTieuChiService.UpdateTreeGroup(data.origin).subscribe({
      next: (res) => {
        this.treeEditVisible = false;
        this.loading = false;
        this.BuildDataForTree();
      },
      error: (err) => {
        console.log("object", err)
        this.loading = false;
      }
    });
    console.log(data.origin);
  }

  addCalculationRow(): void {
    this.calculationRows.push({
      id: "-1",
      tieuChiCode: this.leavesNode.code,
      moTa: '', diem: 0,
      isActive: true,
      isDeleted: false
    });
  }

  removeCalculationRow(index: number): void {
    this.calculationRows.splice(index, 1);
  }
  deleteCalculationRow(index: any, i: number): void {
    console.log("xóa mềm", index);

    const target = this.calculationRows.find((item: any) => item.id === index.id);
    if (index.id != "-1") {
      target.isDeleted = true;
    } else {
      this.calculationRows.splice(i, 1);
    }
    console.log(this.calculationRows);


  }

  closeDrawer(): void {
    this.visible = false;
    this.visibleKiKhaoSat = false;
    this.lstAccount = []
    this.kiKhaoSat = [];
    this.drawerVisible = false;
    this.selectedNodeDetails = []
    this.treeId = '';

  }

  closeModal(): void {
    this.lstCheckedObject = []
    this.treeEditVisible = false;
    this.treeInsertVisible = false;
    this.leavesNode = {
      Code: "-1",
    };
    this.calculationRows = []
    this.visibleKiKhaoSat = false
    this.lstAccount = []
    this.leavesVisible = false;
    this.dataInsertTree.name = '';
  }

  GetTreeLeaves() {
    this._treeTieuChiService.GetTreeLeaves(this.treeId, this.kiKhaoSatId).subscribe({
      next: (data) => {
        console.log(data.result);

        this.selectedNodeDetails = data.result;

      },
      error: (response) => {
        console.log(response);
      },
    });
  }
  CheckTreeLeaves() {
    this._treeTieuChiService.CheckLeaves(this.treeId, this.kiKhaoSatId).subscribe({
      next: (data) => {
        console.log(data);
        if (data) {
          this.messageService.error(
            `Không thể thả tiêu chí có con, vui lòng kiểm tra lại`,
          )
          this.BuildDataForTree();
        }

      },
      error: (response) => {
        console.log(response);
      },
    });
  }
  resetStore() {
    this.filterNguoiChamDiem = "";
    this.setDataKy()
    // this.lstInputDoiTuongSearch = this.inputKi.lstInputStore.length > 0 ? this.inputKi.lstInputStore : this.inputKi.lstInputWarehouse.length > 0 ? this.inputKi.lstInputWareHouse : [];
  }

  setDataKy() {
    this.lstAllDoiTuong =
      this.inputKi.lstInputStore.length > 0
        ? this.inputKi.lstInputStore : this.inputKi.lstInputWareHouse.length > 0
          ? this.inputKi.lstInputWareHouse : [];
    this.lstInputDoiTuongSearch = this.lstAllDoiTuong;

  }

  onClick(node: any) {
    if (node.origin.children == null) {
      this.treeId = node.origin.id;
      this.GetTreeLeaves();
    } else {
      this.treeId = '';
      this.selectedNodeDetails = []
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
        this.lstAllAccount = data;
      },

      error: (response) => {
        console.log(response);
      },
    });
  }
  onTabChange(index: number) {
    // Nếu tab "NGƯỜI CHẤM ĐIỂM" là tab thứ 1
    if (index === 1) {

      this.lstAccount = this.lstAllAccount
      console.log(this.lstAccount);
    }
  }
  getAllKho() {
    this.isSubmit = false
    this._khoService.getAll().subscribe({
      next: (data) => {
        this.lstKho = data
      },
      error: (response) => {
        console.log(response)
      },
    })
  }

  openCreLeaves(data: any): void {
    this.calculationRows = [
      {
        id: '-1',
        tieuChiCode: this.leavesNode.code,
        moTa: 'Đạt',
        diem: '1',
        isActive: true,
        isDeleted: false
      },
      {
        id: '-1',
        tieuChiCode: this.leavesNode.code,
        moTa: 'Không đạt',
        diem: '0',
        isActive: true,
        isDeleted: false
      }
    ]
    this.edit = false;
    this.leavesVisible = true;
    this.leavesNode.pId = this.treeId;
  }

  openUpdateLeaves(data: any): void {
    this.leavesNode = data;
    this.leavesVisible = true;
    this.edit = true;
    this.lstCheckedObject = data.lstCriteriaExcludedOject;
    this.leavesNode.pId = this.treeId;
    this.calculationRows = data.diemTieuChi
  }

  openCreateKi() {
    this._service.buildObjCreate(this.headerId).subscribe({
      next: (data) => {
        this.inputKi = data
        console.log(data);
        this.setDataKy()
        this.edit = false;
        this.visibleKiKhaoSat = true;
      },
      error: (response) => {
        console.log(response);
      },
    });
  }

  getInputCopyKy() {
    console.log(this.inputKi.kyCopyId);
    if (this.inputKi.kyCopyId == null || this.inputKi.kyCopyId == '') {
      return
    }
    this._service.getInputCopyKy(this.inputKi.kyCopyId).subscribe({
      next: (data) => {
        this.inputKi = data
        this.setDataKy()
      },
      error: (response) => {
        console.log(response);
      },
    });
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

  openEditKyKhaoSat(data: any) {
    console.log(data);
    this._service.getInputKiKhaoSat(data.id).subscribe({
      next: (data) => {
        this.inputKi = data
        this.setDataKy()
        setTimeout(() => {
          this.edit = true;
          this.visibleKiKhaoSat = true;
        }, 200);
      }
    })
  }

  nzEvent(event: NzFormatEmitEvent): void {
    console.log('sự kiện', event);
  }

  onDrop(event: any) {
    this.treeId = event.node.origin.code;
    console.log("ss", event.node.parentNode.origin.code == event.dragNode.parentNode.origin.code);
    if (event.node.parentNode.origin.code != event.dragNode.parentNode.origin.code) {
      this.CheckTreeLeaves();
    }



  }

  onDragStart(event: any) { }

  openDrawerTieuChi(param: any): void {
    this.drawerVisible = true;
    this.kiKhaoSatId = param;
    this.BuildDataForTree();
    this._service.getInputKiKhaoSat(param).subscribe({
      next: (data) => {
        this.inputKi = data

      }
    })
  }

  BuildDataForTree() {
    this._treeTieuChiService.BuildDataForTree(this.kiKhaoSatId).subscribe((res) => {
      this.treeData = [res];
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

  openCreateTree(data: any): void {
    this.edit = false
    this.treeInsertVisible = true;
    this.tree = data.origin;
    this.dataInsertTree.pId = this.tree.id;
    this.dataInsertTree.kiKhaoSatId = this.tree.kiKhaoSatId;
    this.dataInsertTree.isGroup = true;
  }

  submitFormTree(): void {
    this.loading = true;

    this._treeTieuChiService.addTree(this.dataInsertTree).subscribe({
      next: (res) => {
        this.treeInsertVisible = false;
        this.loading = false;
        this.BuildDataForTree();

      },
      error: (err) => {
        this.loading = false;
      },
    });
    this.dataInsertTree.name = '';
  }

  isValid(): boolean {
    return !!this.leavesNode.id && !!this.leavesNode.name;
  }



  updateOrderTree() {
    const treeData = this.treeCom
      .getTreeNodes()
      .map((node) => this.mapNode(node))

    this._treeTieuChiService.UpdateOrderTree(treeData[0]).subscribe({
      next: (data) => {
        this.BuildDataForTree()
      },
      error: (response) => {
        console.log(response)
      },
    })
  }

  private mapNode(node: any): any {
    const children = node.children
      ? node.children.map((child: any) => this.mapNode(child))
      : []
    return {
      code: node.origin.code,
      id: node.origin.id,
      pId: node.parentNode?.key || '-1',
      name: node.origin.name,
      children: children,
      kiKhaoSatId: node.origin.kiKhaoSatId,
      isGroup: node.origin.isGroup,
      isActive: true,
      isDeleted: false,
    }
  }

  updateLeaves(): void {
    this.edit = true;
    this.leavesNode.diemTieuChi = this.calculationRows
    this._treeTieuChiService.UpdateLeaves(this.leavesNode).subscribe({
      next: (res) => {
        this.leavesVisible = false;
        this.loading = false;
        this.GetTreeLeaves();
        this.closeModal()
      },
      error: (err) => {
        console.log("object", err)
        this.loading = false;
      }
    });
  }
  deleteLeaves(node: any): void {
    this.edit = true;
    this.leavesNode = node;
    this.leavesNode.isDeleted = true
    this._treeTieuChiService.UpdateLeaves(this.leavesNode).subscribe({
      next: (res) => {
        this.leavesVisible = false;
        this.loading = false;
        this.GetTreeLeaves();
      },
      error: (err) => {
        console.log("object", err)
        this.loading = false;
      }
    });
  }


  insertTree(): void {
    if (!this.isValid()) {
      return;
    }
    this.leavesNode.diemTieuChi = this.calculationRows
    this.leavesNode.kiKhaoSatId = this.kiKhaoSatId
    this.leavesNode.lstCriteriaExcludedOject = this.lstCheckedObject
    console.log(this.leavesNode);

    this._treeTieuChiService.addLeaves(this.leavesNode).subscribe({
      next: (res) => {
        this.leavesVisible = false;
        this.loading = false;
        this.GetTreeLeaves();
        this.closeModal();
      },
      error: (err) => {
        this.loading = false;
      },
    });

  }

  updateTreeGroup(data: any): void {
    this.dataInsertTree = data;
    console.log("object", data)
    this._treeTieuChiService.UpdateTreeGroup(this.dataInsertTree).subscribe({
      next: (res) => {
        this.treeEditVisible = false;
        this.loading = false;
        this.BuildDataForTree();
      },
      error: (err) => {
        this.loading = false;
      },
    });
  }

  updateOrderLeaves(data: any): void {
    this.selectedNodeDetails = data
    this._treeTieuChiService.UpdateOrderLeaves(data).subscribe({
      next: (data) => {
        this.BuildDataForTree()
      },
      error: (response) => {
        console.log(response)
      },
    })
  }

  drop(event: CdkDragDrop<any[]>): void {
    moveItemInArray(this.selectedNodeDetails, event.previousIndex, event.currentIndex);
    this.updateOrderLeaves(this.selectedNodeDetails);

  }

  getNameByCodeAccount(code: any) {
    const item = this.lstAccount.find((x: any) => x.userName === code);
    return item ? item.fullName : code;
  }


  checkedAllObject: any = false
  lstCheckedObject: any = []

  onAllCheckedObject(checked: boolean) {
    this.checkedAllObject = checked;
    this.inputKi.lstInputStore.forEach((store: any) => {
      const idx = this.lstCheckedObject.findIndex(
        (i: any) => i.storeId === store.id && i.tieuChiCode === this.leavesNode.code
      );

      if (idx !== -1) {
        this.lstCheckedObject[idx].isDeleted = !checked && this.lstCheckedObject[idx].code !== '-1';
        if (!checked && this.lstCheckedObject[idx].code === '-1') this.lstCheckedObject.splice(idx, 1);
      } else if (checked) {
        this.lstCheckedObject.push({
          code: "-1",
          doiTuongId: store.id,
          tieuChiCode: this.leavesNode.code,
          isDeleted: false
        });
      }
    });
    console.log(this.lstCheckedObject);

  }

  onItemCheckedObject(store: any, checked: boolean) {
    const idx = this.lstCheckedObject.findIndex(
      (i: any) => i.storeId === store.id && i.tieuChiCode === this.leavesNode.code
    );

    if (idx !== -1) {
      if (checked) this.lstCheckedObject[idx].isDeleted = false;
      else this.lstCheckedObject[idx].code !== '-1'
        ? this.lstCheckedObject[idx].isDeleted = true
        : this.lstCheckedObject.splice(idx, 1);
    } else if (checked) {
      this.lstCheckedObject.push({
        code: "-1",
        doiTuongId: store.id,
        tieuChiCode: this.leavesNode.code,
        isDeleted: false
      });
    }
    console.log(this.lstCheckedObject);

  }

  isCheckedObject = (id: string) =>
    this.lstCheckedObject.some(
      (i: any) => i.doiTuongId === id && i.tieuChiCode === this.leavesNode.code && !i.isDeleted
    );

  trackByKey(index: number, item: any): string {
    return item.key || item.code || item.key; // hoặc item.id nếu bạn dùng id
  }

}
