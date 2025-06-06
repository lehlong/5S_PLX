import { Observable } from 'rxjs';
import { CommonService } from './common.service';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class KyKhaoSatService {
  constructor(private commonService: CommonService) {}

  search(params: any): Observable<any> {
    return this.commonService.get('KiKhaoSat/Search', params)
  }

  getInputKiKhaoSat(params: any): Observable<any> {
    return this.commonService.get(`KiKhaoSat/GetInputKiKhaoSat?idKi=${params}`)
  }
}
