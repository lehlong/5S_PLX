import { Routes } from '@angular/router'
import { AccountTypeComponent } from './account-type/account-type.component'
import { StoreComponent } from './store/store.component'
import { DoiTuongComponent } from './doi-tuong/doi-tuong.component'
import { ChucVuComponent } from './chuc-vu/chuc-vu.component'
import { DanhGiaTieuChiComponent } from './danh-gia-tieu-chi/danh-gia-tieu-chi.component'
import { WareHouseComponent } from './ware-house/ware-house.component'

export const masterDataRoutes: Routes = [
  { path: 'account-type', component: AccountTypeComponent },
  { path: 'doi-tuong', component: DoiTuongComponent },
  { path: 'chuc-vu', component: ChucVuComponent },
  { path: 'store', component: StoreComponent },
  { path: 'ware-house', component: WareHouseComponent },
  { path: 'danh-gia-tieu-chi', component: DanhGiaTieuChiComponent },




]
