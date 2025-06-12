import { Routes } from '@angular/router';

import { ReportComponent } from './report.component';
import { KetQuaChamDiemComponent } from './page/ket-qua-cham-diem/ket-qua-cham-diem.component';
import { ChamDiemTheoKhungThoiGianComponent } from './page/cham-diem-theo-khung-thoi-gian/cham-diem-theo-khung-thoi-gian.component';
import { ThoiGianChamDiemComponent } from './page/thoi-gian-cham-diem/thoi-gian-cham-diem.component';
import { ThietBiChamDiemComponent } from './page/thiet-bi-cham-diem/thiet-bi-cham-diem.component';
import { TongHopYKienDeXuatComponent } from './page/tong-hop-y-kien-de-xuat/tong-hop-y-kien-de-xuat.component';

export const reportRoutes: Routes = [
  {
    path: '',
    children: [
      { path: '', redirectTo: 'home', pathMatch: 'full' },
      { path: '', component: ReportComponent },
      { path: 'ket-qua-cham-diem', component: KetQuaChamDiemComponent},
      { path: 'cham-diem-theo-khung-thoi-gian', component: ChamDiemTheoKhungThoiGianComponent},
      { path: 'thoi-gian-cham-diem', component: ThoiGianChamDiemComponent},
      { path: 'thiet-bi-cham-diem', component: ThietBiChamDiemComponent},
      { path: 'tong-hop-y-kien', component: TongHopYKienDeXuatComponent},
    ],
  },
];
