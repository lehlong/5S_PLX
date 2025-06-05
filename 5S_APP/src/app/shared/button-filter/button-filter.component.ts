import { Component, OnInit, ViewChild } from '@angular/core';
import { SharedModule } from '../shared.module';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'app-button-filter',
  templateUrl: './button-filter.component.html',
  styleUrls: ['./button-filter.component.scss'],
  imports: [SharedModule],
  standalone: true,
})
export class ButtonFilterComponent implements OnInit {
  @ViewChild('modal') modal: any;
  filterForm!: FormGroup;
  constructor(private fb: FormBuilder) {}

  ngOnInit() {
    this.filterForm = this.fb.group({
      selectedMonth: ['1', Validators.required],
      selectedStore: ['1', Validators.required],
      selectedPerson: ['1', Validators.required],
      storeChecked: [false],
      storeUnchecked: [false],
    });
  }
  async canDismiss(data?: undefined, role?: string) {
    return role !== 'gesture';
  }
  applyFilter() {}
  resetFilters() {
    this.filterForm.reset({
      selectedMonth: '',
      selectedStore: '',
      selectedPerson: '',
      storeChecked: false,
      storeUnchecked: false,
    });
  }
}
