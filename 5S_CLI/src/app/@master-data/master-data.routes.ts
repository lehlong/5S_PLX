import { Routes } from '@angular/router'
import { AccountTypeComponent } from './account-type/account-type.component'
import { StoreComponent } from './store/store.component'
import { DoiTuongComponent } from './doi-tuong/doi-tuong.component'
import { ChucVuComponent } from './chuc-vu/chuc-vu.component'

export const masterDataRoutes: Routes = [
  { path: 'account-type', component: AccountTypeComponent },
  { path: 'doi-tuong', component: DoiTuongComponent },
  { path: 'chuc-vu', component: ChucVuComponent },
   { path: 'store', component: StoreComponent }, 


 

]
