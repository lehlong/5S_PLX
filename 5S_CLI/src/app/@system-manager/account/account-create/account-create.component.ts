import { Component, ElementRef, Input, ViewChild } from '@angular/core'
import { ShareModule } from '../../../shared/share-module'
import { FormGroup, Validators, NonNullableFormBuilder } from '@angular/forms'
import { DropdownService } from '../../../service/dropdown/dropdown.service'
import { AccountService } from '../../../service/system-manager/account.service'
import { ActivatedRoute } from '@angular/router'
import { NzUploadFile, NzUploadModule, NzUploadXHRArgs } from 'ng-zorro-antd/upload';
import { Observable, Observer, Subscription } from 'rxjs'

@Component({
  selector: 'app-account-create',
  standalone: true,
  imports: [ShareModule],
  templateUrl: './account-create.component.html',
  styleUrl: './account-create.component.scss',
})
export class AccountCreateComponent {
  @Input() reset: () => void = () => { }
  @Input() visible: boolean = false
  @Input() close: () => void = () => { }
  @ViewChild('fileInput') fileInput!: ElementRef;

  validateForm: FormGroup
  avatarBase64: string = ''
  passwordVisible: boolean = false
  accountType: any[] = []
  orgList: any[] = []
  warehouseList: any[] = []
  positionList: any[] = []
  selectedOrg = ''
  loading: boolean = false

  constructor(
    private _service: AccountService,
    private fb: NonNullableFormBuilder,
    private dropdownService: DropdownService,
    private route: ActivatedRoute,
  ) {
    this.validateForm = this.fb.group({
      userName: ['', [Validators.required]],
      password: [''],
      fullName: ['', [Validators.required]],
      address: [''],
      phoneNumber: ['', [Validators.pattern("^[0-9]*$"),]],
      email: ['', [Validators.email]],
      isActive: [true],
      accountType: ['', [Validators.required]],
    })
  }

  ngOnInit(): void {
    this.loadInit()
    this.getAllAccountType()
    this.getAllOrg()
  }

  loadInit() {
    const organizeCode = localStorage.getItem('companyCode') || '';
    const warehouseCode = localStorage.getItem('warehouseCode') || '';
    this.validateForm.patchValue({
      organizeCode: organizeCode,
      warehouseCode: warehouseCode,
    });
    if (organizeCode) {
      this.selectedOrg = organizeCode;
    }
  }

  changeSaleType(value: string) { }

  getAllAccountType() {
    this.dropdownService
      .getAllAccountType({
        IsCustomer: true,
        SortColumn: 'name',
        IsDescending: true,
      })
      .subscribe({
        next: (data) => {
          this.accountType = data
        },
        error: (response) => {
          console.log(response)
        },
      })
  }
  getAllOrg() {
    this.dropdownService.getAllOrg().subscribe({
      next: (data) => {
        this.orgList = data
      },
      error: (response) => {
        console.log(response)
      },
    })
  }

  submitForm(): void {
    if (this.validateForm.valid) {
      const formValue = this.validateForm.value
      if(this.avatarBase64 != '' && this.isBase64Image(this.avatarBase64)){
        formValue.imageBase64 = this.avatarBase64
      }
      this._service.create(formValue).subscribe({
        next: (data) => {
          this.reset()
        },
        error: (response) => {
          console.log(response)
        },
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

  isBase64Image(str: string): boolean {
    const dataUriPattern = /^data:image\/(png|jpg|jpeg|gif|bmp|webp);base64,/;
    if (!dataUriPattern.test(str)) return false;
    const base64String = str.split(',')[1];
    if (!base64String || base64String.length % 4 !== 0) return false;
    const base64Regex = /^[A-Za-z0-9+/]*={0,2}$/;
    return base64Regex.test(base64String);
  }

  closeDrawer() {
    this.close()
    this.resetForm()
    this.clearImage()
  }

  resetForm() {
    this.validateForm.reset()
  }
  clearImage() {
    this.avatarBase64 = ''; 
    this.fileInput.nativeElement.value = '';
  }

  onFileSelected(event: any) {
    const file = event.target.files[0];
    if (file) {
      const reader = new FileReader();
      reader.onload = (e: any) => {
        this.avatarBase64 = e.target.result; 
      };
      reader.readAsDataURL(file);
    }
  }
  
}
