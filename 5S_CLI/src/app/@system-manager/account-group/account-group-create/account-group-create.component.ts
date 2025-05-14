import { Component, Input } from '@angular/core'
import { ShareModule } from '../../../shared/share-module'
import { FormGroup, Validators, NonNullableFormBuilder } from '@angular/forms'
import { AccountGroupService } from '../../../service/system-manager/account-group.service'

@Component({
  selector: 'app-account-group-create',
  standalone: true,
  imports: [ShareModule],
  templateUrl: './account-group-create.component.html',
  styleUrls: ['./account-group-create.component.scss'],
})
export class AccountGroupCreateComponent {
  @Input() reset: () => void = () => { }
  @Input() visible: boolean = false
  @Input() loading: boolean = false
  @Input() close: () => void = () => { }
  validateForm: FormGroup

  constructor(
    private _service: AccountGroupService,
    private fb: NonNullableFormBuilder,
  ) {
    this.validateForm = this.fb.group({
      name: ['', [Validators.required]],
      notes: [''],
      isActive: [true, [Validators.required]],
    })
  }

  ngAfterViewInit() {
    this.resetForm()
  }
  ngOnInit(): void {
    this.loadInit()
  }
  loadInit() { }

  submitForm(): void {
    if (this.validateForm.valid) {
      this._service.Insert(this.validateForm.value).subscribe({
        next: (data) => {
          this.reset()
        },
        error: (response) => { },
      })
    } else {
      Object.values(this.validateForm.controls).forEach((control) => {
        if (control.invalid) {
          control.markAsDirty()
          control.updateValueAndValidity({ onlySelf: true })
        }
      })
    }
  }

  closeDrawer() {
    this.close()
    this.resetForm()
  }

  resetForm() {
    this.validateForm.reset()
  }
}
