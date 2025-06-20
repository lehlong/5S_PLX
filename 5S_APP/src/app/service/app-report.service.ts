import { Observable } from 'rxjs';
import { CommonService } from './common.service';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class AppReportService {
  constructor(private commonService: CommonService) {}

  KetQuaChamDiem(params: any): Observable<any> {
    return this.commonService.get('AppReport/KetQuaChamDiem', params)
  }
  ThoiGianChamDiem(params: any): Observable<any>{
    return this.commonService.get('AppReport/ThoiGianChamDiem', params)
  }

}
