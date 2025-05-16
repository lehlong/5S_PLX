import { Component, ViewChild } from '@angular/core';
import { FormGroup, NonNullableFormBuilder, Validators } from '@angular/forms';
import { NzFormatEmitEvent, NzTreeComponent } from 'ng-zorro-antd/tree';
import { ShareModule } from '../../shared/share-module';
import { NzUploadChangeParam } from 'ng-zorro-antd/upload';
import { NzMessageService } from 'ng-zorro-antd/message';
import { TreeTieuChiService } from '../../service/business/tree-tieu-chi.service';
import { log } from 'ng-zorro-antd/core/logger';

@Component({
  selector: 'app-danh-gia-tieu-chi',
  templateUrl: './danh-gia-tieu-chi.component.html',
  imports: [ShareModule],
  styleUrls: ['./danh-gia-tieu-chi.component.scss'],
})
export class DanhGiaTieuChiComponent {
  @ViewChild('treeCom', { static: false }) treeCom!: NzTreeComponent;
  validateForm: FormGroup;
  searchValue = '';
  visible = false;
  drawerVisible = false;
  edit = false;
  selectedNode: any = null;
  selectedNodeDetails: any[] = [];
  treeData :any= [];

  calculationInputs: string[] = [];
  calculationRows: { description: string; score: number }[] = [];
  uploadUrl: string = 'your-upload-endpoint-url';

  constructor(
    private fb: NonNullableFormBuilder,
    private messageService: NzMessageService,
    private _service: TreeTieuChiService,
  ) {
    this.validateForm = this.fb.group({
      code: ['', [Validators.required]],
      name: ['', [Validators.required]],
      order: [null],
      minImages: [null],
      isRequiredImage: [false],
      isCHTATVOnly: [false],
      calculationMethod: [''],
      report: [''],
    });
  }
    resetForm() {
    this.validateForm.reset()
  }
  close() {
    this.visible = false
    this.resetForm()
  }
  openDrawer(param : any): void {
    this.drawerVisible = true;
    this._service.GetTreeTieuChi(param).subscribe((res) => {
      console.log(res);
      
      this.treeData = [res];
    })
  }


  closeDrawer(): void {
    this.drawerVisible = false;
  }
  nzEvent(event: NzFormatEmitEvent): void {}

  onDragStart(event: any) {}

  onDrop(event: NzFormatEmitEvent): void {
    const treeData = this.treeCom
      .getTreeNodes()
      .map((node) => this.mapNode(node));
    console.log('Updated Tree Data:', treeData);
  }

  loadNodeDetails(nodeKey: string): void {
    if (nodeKey === '0-0') {
      this.selectedNodeDetails = [
        { id: '1', code: 'TC001', name: 'Sàn bãi', requiredImage: true },
        {
          id: '2',
          code: 'TC002',
          name: 'Tiêu đào trụ bơm',
          requiredImage: false,
        },
      ];
    } else if (nodeKey === '0-1') {
      this.selectedNodeDetails = [
        {
          id: '3',
          code: 'TC003',
          name: 'Đồng phục khi bán hàng',
          requiredImage: true,
        },
      ];
    } else {
      this.selectedNodeDetails = [];
    }
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
  openCreate() {
    this.edit = false;
    this.visible = true;
    this.validateForm.reset();
  }

  openEdit(node: any) {
    if (!node) return;
    this.edit = true;
    this.visible = true;
    this.validateForm.patchValue({
      code: node.code || '',
      name: node.name || node.title || '',
      calculationMethod: node.calculationMethod || '',
      report: node.report || '',
    });
  }

  deleteNode(node: any) {
    if (!node) return;
    console.log('Deleting node:', node);
  }

  openCreateModal(): void {
    this.edit = false;
    this.visible = true;
    this.validateForm.reset({
      code: '',
      name: '',
      order: null,
      minImages: null,
      isRequiredImage: false,
      isGroup: false,
      calculationMethod: '',
      report: '',
    });
    if (this.calculationRows.length === 0) {
      this.addCalculationRow();
    }
  }

  closeModal(): void {
    this.visible = false;
  }

  submitForm(): void {
    if (this.validateForm.valid) {
      if (this.edit) {
        console.log('Updating:', this.validateForm.value);
      } else {
        console.log('Creating:', this.validateForm.value);
      }
      this.closeModal();
    } else {
      Object.values(this.validateForm.controls).forEach((control) => {
        if (control.invalid) {
          control.markAsDirty();
          control.updateValueAndValidity();
        }
      });
    }
  }

   onClick(node: any) {
    console.log('Node clicked:', node);
    this._service
      .GetTreeTieuChi({
        kiKhaoSatId: 'string',
      })
      // .subscribe((res) => {
      //   this.nodeCurrent = res
      //   this.edit = true
      //   this.visible = true
      //   this.titleParent = node.parentNode?.origin?.title || ''
      //   this.validateForm.setValue({
      //     id: this.nodeCurrent?.id,
      //     name: this.nodeCurrent?.name,
      //     pId: this.nodeCurrent?.pId,
      //     orderNumber: this.nodeCurrent?.orderNumber,
      //     rightId: this.nodeCurrent?.rightId || '',
      //     children: [],
      //   })
      // })
  }
  openCreateChild(node: any) {
    this.close()
    this.edit = false
    this.visible = true
    this.validateForm.get('pId')?.setValue(node?.origin.id)
    this.validateForm.get('orderNumber')?.setValue(null)
    this.validateForm.get('children')?.setValue([])
  }
  addCalculationInput(): void {
    this.calculationInputs.push('');
  }

  removeCalculationInput(index: number): void {
    this.calculationInputs.splice(index, 1);
  }

  addCalculationRow(): void {
    this.calculationRows.push({ description: '', score: 0 });
  }

  removeCalculationRow(index: number): void {
    this.calculationRows.splice(index, 1);
  }

  private mapNode(node: any): any {
    const children = node.children
      ? node.children.map((child: any) => this.mapNode(child))
      : [];
    return {
      id: node.key,
      pId: node.parentNode?.key || '',
      name: node.title,
      children: children,
    };
  }
}
