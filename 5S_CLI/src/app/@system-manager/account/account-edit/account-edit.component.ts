import { Component, Input, HostListener, ViewChild, ElementRef } from '@angular/core'
import { ShareModule } from '../../../shared/share-module'
import { FormGroup, Validators, NonNullableFormBuilder } from '@angular/forms'
import { DropdownService } from '../../../service/dropdown/dropdown.service'
import { AccountService } from '../../../service/system-manager/account.service'
import { RightService } from '../../../service/system-manager/right.service'
import { NzFormatEmitEvent, NzTreeNode } from 'ng-zorro-antd/tree'
import { UserTypeCodes } from '../../../shared/constants/account.constants'
import { ActivatedRoute } from '@angular/router'
import { AuthService } from '../../../service/auth.service'
import { GlobalService } from '../../../service/global.service'
import { AccountGroupService } from '../../../service/system-manager/account-group.service'
import { environment } from '../../../../environments/environment'

@Component({
  selector: 'app-account-edit',
  standalone: true,
  imports: [ShareModule],
  templateUrl: './account-edit.component.html',
  styleUrl: './account-edit.component.scss',
})
export class AccountEditComponent {
  @HostListener('window:resize', ['$event'])
  onResize() {
    this.widthDeault =
      window.innerWidth <= 767
        ? `${window.innerWidth}px`
        : `${window.innerWidth * 0.7}px`
  }

  @Input() reset: () => void = () => { }
  @Input() visible: boolean = false
  @Input() close: () => void = () => { }
  @Input() userName: string | number = ''
  @ViewChild('fileInput') fileInput!: ElementRef;

  optionsGroup: any[] = []
  widthDeault: string = '0px'
  heightDeault: number = 0

  validateForm: FormGroup

  nodes: any[] = []
  nodesConstant: any[] = []
  initialCheckedNodes: any[] = []
  searchValue = ''
  // listPartnerCustomer: any[] = []
  // UserTypeCodes = UserTypeCodes
  // isShowSelectPartner: boolean = false
  accountType: any[] = []
  orgList: any[] = []
  warehouseList: any[] = []
  positionList: any[] = []
  selectedOrg = ''

  avatarBase64: string = ''

  constructor(
    private _service: AccountService,
    private fb: NonNullableFormBuilder,
    private dropdownService: DropdownService,
    private rightService: RightService,
    private accountGroupService: AccountGroupService,
    private route: ActivatedRoute,
    private authService: AuthService,
    private globalService: GlobalService,
  ) {
    this.validateForm = this.fb.group({
      userName: ['', [Validators.required]],
      fullName: ['', [Validators.required]],
      address: [''],
      phoneNumber: ['', [Validators.pattern('^0\\d{8,9}$')]],
      email: ['', [Validators.email]],
      isActive: [true],
      accountType: ['', [Validators.required]],
      urlImage: [''],
    })
    this.widthDeault =
      window.innerWidth <= 767
        ? `${window.innerWidth}px`
        : `${window.innerWidth * 0.7}px`
    this.heightDeault = window.innerHeight - 200
  }

  ngOnInit(): void {
    this.loadInit()
    this.selectedOrg = this.validateForm.getRawValue().organizeCode
  }

  loadInit() {
    this.getAllAccountType()
    this.getRight()
    this.getAllOrg()
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

  changeSaleType(value: string) { }

  getRight() {
    this.rightService.GetRightTree().subscribe((res) => {
      this.nodes = this.mapTreeNodes(res)
    })
  }

  mapTreeNodes(data: any): any[] {
    return data.children
      ? data.children.map((node: any) => ({
        title: node.id + '-' + node.name,
        key: node.id,
        checked: node.isChecked,
        expanded: true,
        children: this.mapTreeNodes(node),
        origin: {
          // Add this line
          InChecked: false, // Initialize with a default value
          OutChecked: false, // Initialize with a default value
        },
      }))
      : []
  }

  onCheckBoxChange(event: any): void {
    const checkedNode = event.node
    const nodes = this.flattenKeys(this.nodesConstant)

    if (checkedNode.isChecked) {
      if (nodes.includes(checkedNode.key)) {
        checkedNode.origin.InChecked = false
        checkedNode.origin.OutChecked = false
      } else {
        checkedNode.origin.InChecked = true
      }
      // Thêm logic mới: check parents và children
      this.checkParents(checkedNode)
      this.checkChildren(checkedNode)
    } else {
      if (nodes.includes(checkedNode.key)) {
        checkedNode.origin.OutChecked = true
        checkedNode.origin.InChecked = false
      } else {
        checkedNode.origin.OutChecked = false
        checkedNode.origin.InChecked = false
      }
      // Thêm logic mới: uncheck children
      this.uncheckChildren(checkedNode)
    }
  }

  flattenKeys(data: any) {
    return data.reduce((keys: any, item: any) => {
      if (item.checked) {
        keys.push(item.key)
      }
      if (item.children && item.children.length > 0) {
        keys.push(...this.flattenKeys(item.children))
      }
      return keys
    }, [])
  }

  private checkParents(node: NzTreeNode): void {
    const parentNode = node.parentNode
    if (parentNode) {
      parentNode.isChecked = true
      this.checkParents(parentNode)
    }
  }

  private checkChildren(node: NzTreeNode): void {
    if (node.children) {
      node.children.forEach((child) => {
        child.isChecked = true

        this.checkChildren(child)
      })
    }
  }

  private uncheckChildren(node: NzTreeNode): void {
    if (node.children) {
      node.children.forEach((child) => {
        child.isChecked = false

        this.uncheckChildren(child)
      })
    }
  }

  onNodeCheckChange(node: any): void {
    node.checked = !node.checked
  }

  getCheckedNodes(nodes: any[]): any[] {
    let checkedNodes: any[] = []
    for (let node of nodes) {
      if (node.checked) {
        checkedNodes.push(node)
      }
      if (node.children) {
        checkedNodes = checkedNodes.concat(this.getCheckedNodes(node.children))
      }
    }
    return checkedNodes
  }

  nzEvent(event: NzFormatEmitEvent): void {
  }

  getAllGroup(listGroup: any = []) {
    this.dropdownService.getAllAccountGroup().subscribe({
      next: (data) => {
        this.optionsGroup = data.map((item: any) => {
          return {
            ...item,
            title: item?.name,
            direction: listGroup.some(
              (group: any) => group?.groupId === item?.id,
            )
              ? 'right'
              : 'left',
          }
        })
      },
      error: (response) => {
        console.log(response)
      },
    })
  }

  getDetail(userName: string = '') {
    this._service
      .getDetail({
        userName: userName,
      })
      .subscribe({
        next: (data) => {
          this.getAllGroup(data?.account_AccountGroups)
          this.validateForm.setValue({
            userName: data.userName,
            fullName: data.fullName,
            address: data.address,
            phoneNumber: data.phoneNumber,
            email: data.email,
            isActive: data.isActive,
            accountType: data.accountType,
            urlImage : data.urlImage
            //partnerId: data.partnerId || '',
          })
          this.avatarBase64 = environment.urlFiles + data.urlImage;
          //this.isShowSelectPartner = data.accountType === 'KH' ? true : false
          this.initialCheckedNodes = data?.listAccountGroupRight?.map(
            (node: any) => node.rightId,
          )
          this.nodes = this.mapTreeNodes(data.treeRight)
          this.nodesConstant = [...this.mapTreeNodes(data.treeRight)]
          this.loadGroupRights(data.account_AccountGroups)
        },
        error: (response) => {
          console.log(response)
        },
      })
  }

  loadGroupRights(accountGroups: any[]) {
    const groupIds = accountGroups.map((group) => group.groupId)

    // Fetch rights for all groups
    Promise.all(
      groupIds.map((id) => this.accountGroupService.GetDetail(id).toPromise()),
    )
      .then((groups) => {
        let allGroupRights: string[] = []
        groups.forEach((group) => {
          if (group && group.listAccountGroupRight) {
            allGroupRights = [
              ...allGroupRights,
              ...group.listAccountGroupRight.map(
                (right: { rightId: any }) => right.rightId,
              ),
            ]
          }
        })

        // Remove duplicates
        allGroupRights = [...new Set(allGroupRights)]

        // Update the tree with these rights
        this.updateTreeWithGroupRights(allGroupRights)
      })
      .catch((error) => {
        console.error('Error loading group rights:', error)
      })
  }

  updateTreeWithGroupRights(groupRights: string[]) {
    const updateNode = (node: any) => {
      if (groupRights.includes(node.key)) {
        node.checked = true
        node.origin.InChecked = true // This line is causing the error
        this.checkParents(node)
      }
      if (node.children) {
        node.children.forEach(updateNode)
      }
    }

    this.nodes.forEach(updateNode)
    this.nodesConstant = JSON.parse(JSON.stringify(this.nodes))
  }
  // onUserTypeChange(value: string) {
  //   let partnerIdControl = this.validateForm.get('partnerId')
  //   if (value === 'KH') {
  //     this.isShowSelectPartner = true
  //     partnerIdControl!.setValidators([Validators.required])
  //   } else {
  //     this.isShowSelectPartner = false
  //     partnerIdControl!.setValidators([])
  //   }
  //   partnerIdControl!.updateValueAndValidity()
  // }

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

  submitForm(): void {
    const listAccountGroupRight = this.getCheckedNodes(this.nodes).map(
      (element: any) => ({
        rightId: element.key,
      }),
    )
    const account_AccountGroups = this.optionsGroup?.reduce(
      (result: any, item: any) => {
        if (item?.direction == 'right') {
          return [
            ...result,
            {
              groupId: item?.id,
            },
          ]
        }
        return result
      },
      [],
    )
    if (this.validateForm.valid) {
      const formValue = this.validateForm.value
      // const { partnerId, ...rest } = formValue
      // let insertObj = {}
      // if (this.isShowSelectPartner) {
      //   insertObj = formValue
      // } else {
      //   insertObj = rest
      // }
      if(this.avatarBase64 != '' && this.isBase64Image(this.avatarBase64)){
        formValue.imageBase64 = this.avatarBase64
      }

      this._service
        .update({
          ...formValue,
          accountRights: listAccountGroupRight,
          account_AccountGroups,
        })
        .subscribe({
          next: (data) => {
            if (this.globalService.getUserInfo().userName) {
              this.authService
                .getRightOfUser({
                  userName: this.globalService.getUserInfo().userName,
                })
                .subscribe({
                  next: (rights) => {
                    this.globalService.setRightData(
                      JSON.stringify(rights || []),
                    )
                  },
                  error: (error) => {
                    console.error('Get right of user failed:', error)
                    console.log('formValue', formValue)
                    console.log('accountRights', listAccountGroupRight)
                    console.log('account_AccountGroups', account_AccountGroups)
                  },
                })
            }
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

  onDrop(event: any): void {
    // Handle drop event
  }

  onClick(event: any): void { }

  closeDrawer() {
    this.close()
    this.resetForm()
    this.clearImage()
  }

  resetForm() {
    this.validateForm.reset()
  }

  resetPassword() {
    this._service.resetPassword(this.userName).subscribe({
      next: (data) => {
      }
    })
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
