import { Routes } from '@angular/router'
import { HinhAnhChamDiemComponent } from './hinh-anh-cham-diem/hinh-anh-cham-diem.component'
import { ChamDiemTheoKhungThoiGianComponent } from './cham-diem-theo-khung-thoi-gian/cham-diem-theo-khung-thoi-gian.component'
import { KetQuaChamDiemComponent } from './ket-qua-cham-diem/ket-qua-cham-diem.component'
import { ThietBiChamDiemComponent } from './thiet-bi-cham-diem/thiet-bi-cham-diem.component'
import { ThoiGianChamDiemComponent } from './thoi-gian-cham-diem/thoi-gian-cham-diem.component'
import { TongHopYKienDeXuatComponent } from './tong-hop-y-kien-de-xuat/tong-hop-y-kien-de-xuat.component'

export const surveyReportRoutes: Routes = [
  { path: 'hinh-anh-cham-diem', component: HinhAnhChamDiemComponent },
  { path: 'cham-diem-theo-khung-thoi-gian', component: ChamDiemTheoKhungThoiGianComponent },
  { path: 'ket-qua-cham-diem', component: KetQuaChamDiemComponent },
  { path: 'thiet-bi-cham-diem', component: ThietBiChamDiemComponent },
  { path: 'thoi-gian-cham-diem', component: ThoiGianChamDiemComponent },
  { path: 'tong-hop-y-kien-de-xuat', component: TongHopYKienDeXuatComponent },

]
