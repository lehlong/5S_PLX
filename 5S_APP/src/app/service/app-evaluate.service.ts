import { Observable } from 'rxjs';
import { CommonService } from './common.service';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class AppEvaluateService {
  constructor(private commonService: CommonService) { }

  search(params: any): Observable<any> {
    return this.commonService.get('AppEvaluate/Search', params)
  }

  HandlePointStore(params: any): Observable<any> {
    return this.commonService.post(`AppEvaluate/HandlePointStore`, params)
  }

  GetAllTieuChiLeaves(kiKhaoSatId: any, doiTuongId: any): Observable<any> {
    return this.commonService.get(`AppEvaluate/GetAllTieuChiLeaves?kiKhaoSatId=${kiKhaoSatId}&doiTuongId=${doiTuongId}`)
  }

  BuildInputEvaluate(kiKhaoSatId: any, doiTuongId: any, deviceID: string): Observable<any> {
    return this.commonService.get(`AppEvaluate/BuildInputEvaluate?kiKhaoSatId=${kiKhaoSatId}&doiTuongId=${doiTuongId}&deviceID=${deviceID}`)
  }
  getNotifications(): Observable<any> {
    return this.commonService.get('AppEvaluate/GetNotification')
  }
  buildDataTreeForApp(kiKhaoSatId: any, doiTuongId: any): Observable<any> {
    return this.commonService.get(`AppEvaluate/BuildDataTreeForApp?kiKhaoSatId=${kiKhaoSatId}&doiTuongId=${doiTuongId}`)
  }

  insertEvaluate(params: any): Observable<any> {
    return this.commonService.post(`AppEvaluate/InsertEvaluate`, params)
  }

  insertEvaluate2(params: any): Observable<any> {
    return this.commonService.post(`AppEvaluate/InsertEvaluate2`, params)
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

  uploadFile(params: any){
    return this.commonService.post(`AppEvaluate/UploadFile`, params)
  }
}
