import { Observable } from 'rxjs';
import { CommonService } from './common.service';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class AppEvaluateService {
  constructor(private commonService: CommonService) {}

  search(params: any): Observable<any> {
    return this.commonService.get('AppEvaluate/Search', params)
  }

  filterLstChamDiem(params: any): Observable<any> {
    return this.commonService.get('AppEvaluate/FilterLstChamDiem', params)
  }
  GetAllTieuChiLeaves(kiKhaoSatId: any, storeId: any): Observable<any> {
    return this.commonService.get(`AppEvaluate/GetAllTieuChiLeaves?kiKhaoSatId=${kiKhaoSatId}&storeId=${storeId}`)
  }

  BuildInputEvaluate(kiKhaoSatId: any, storeId: any,deviceID:string): Observable<any> {
    return this.commonService.get(`AppEvaluate/BuildInputEvaluate?kiKhaoSatId=${kiKhaoSatId}&storeId=${storeId}&deviceID=${deviceID}`)
  }

  buildDataTreeForApp(kiKhaoSatId: any, storeId: any): Observable<any> {
    return this.commonService.get(`AppEvaluate/BuildDataTreeForApp?kiKhaoSatId=${kiKhaoSatId}&storeId=${storeId}`)
  }

  insertEvaluate(params: any): Observable<any> {
    return this.commonService.post(`AppEvaluate/InsertEvaluate`, params)
  }

  tinhTongLanCham(params: any): Observable<any> {
    return this.commonService.post(`AppEvaluate/TinhTongLanCham`, params)
  }

  getResultEvaluate(params: any): Observable<any> {
    return this.commonService.get(`AppEvaluate/GetResultEvaluate?code=${params}`)
  }

  getPointStore(params: any): Observable<any> {
    return this.commonService.get(`AppEvaluate/GetPointStore?kiKhaoSatId=${params.kiKhaoSatId}&surveyId=${params.surveyId}`)
  }

 
}
