import { Component, ViewChild } from '@angular/core';
import { FormGroup, NonNullableFormBuilder, Validators } from '@angular/forms';
import { NzFormatEmitEvent, NzTreeComponent } from 'ng-zorro-antd/tree';
import { ShareModule } from '../../shared/share-module';

@Component({
  selector: 'app-danh-gia-tieu-chi',
  templateUrl: './danh-gia-tieu-chi.component.html',
  imports: [ShareModule],
  styleUrls: ['./danh-gia-tieu-chi.component.scss'],
})
export class DanhGiaTieuChiComponent {
  @ViewChild('treeCom', { static: false }) treeCom!: NzTreeComponent;
  validateForm: FormGroup;
  visible = false; // For create/edit modal
  drawerVisible = false; // For the main drawer
  edit = false;
  selectedNode: any = null;
  selectedNodeDetails: any[] = [];
  treeData = [
    {
      title: 'Tháng 6 - 2024',
      key: '0',
      expanded: true,
      children: [
        {
          title: 'I. CHUẨN KHU VỰC BÁN HÀNG',
          key: '0-0',
          expanded: true,
          children: [
            { title: 'Sàn bãi', key: '0-0-0', isLeaf: true },
            { title: 'Tiêu đào trụ bơm', key: '0-0-1', isLeaf: true },
          ],
        },
        {
          title: 'II. CHUẨN ĐỒNG PHỤC',
          key: '0-1',
          children: [
            { title: 'Đồng phục khi bán hàng', key: '0-1-0', isLeaf: true },
          ],
        },
      ],
    },
  ];

  constructor(private fb: NonNullableFormBuilder) {
    this.validateForm = this.fb.group({
      id: ['', [Validators.required]],
      name: ['', [Validators.required]],
      pId: ['', [Validators.required]],
      orderNumber: [null],
      children: [null],
    });
  }

  // Open the main drawer
  openDrawer(): void {
    this.drawerVisible = true;
  }

  // Close the main drawer
  closeDrawer(): void {
    this.drawerVisible = false;
  }

  onTreeNodeClick(event: NzFormatEmitEvent): void {
    this.selectedNode = event.node;
    this.loadNodeDetails(event.node?.key || '');
  }

  onDrop(event: NzFormatEmitEvent): void {
    const treeData = this.treeCom.getTreeNodes().map((node) => this.mapNode(node));
    console.log('Updated Tree Data:', treeData);
    // Update the tree order in the backend if needed
  }

  loadNodeDetails(nodeKey: string): void {
    // Mock data for node details
    if (nodeKey === '0-0') {
      this.selectedNodeDetails = [
        { id: '1', code: 'TC001', name: 'Sàn bãi', requiredImage: true },
        { id: '2', code: 'TC002', name: 'Tiêu đào trụ bơm', requiredImage: false },
      ];
    } else if (nodeKey === '0-1') {
      this.selectedNodeDetails = [
        { id: '3', code: 'TC003', name: 'Đồng phục khi bán hàng', requiredImage: true },
      ];
    } else {
      this.selectedNodeDetails = [];
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
      id: node.key || node.id,
      name: node.title || node.name,
      pId: node.parentNode?.key || '',
      orderNumber: null,
      children: [],
    });
  }

  deleteNode(node: any) {
    if (!node) return;
    console.log('Deleting node:', node);
    // Add logic to delete the node
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