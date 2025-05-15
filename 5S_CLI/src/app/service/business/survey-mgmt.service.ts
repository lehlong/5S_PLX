import { Injectable } from '@angular/core'
import { CommonService } from '../common.service'
import { Observable } from 'rxjs'

@Injectable({
  providedIn: 'root',
})
export class SurveyMgmtService {
  constructor(private commonService: CommonService) { }

  search(params: any): Observable<any> {
    return this.commonService.get('SurveyMgmt/Search', params)
  }

  getAll(): Observable<any> {
    return this.commonService.get('SurveyMgmt/GetAll')
  }

  buildInput(): Observable<any>{
    return this.commonService.get('SurveyMgmt/BuildInput')
  }

  create(params: any): Observable<any> {
    return this.commonService.post('SurveyMgmt/Insert', params)
  }

  update(params: any): Observable<any> {
    return this.commonService.put('SurveyMgmt/Update', params)
  }

  delete(id: string): Observable<any> {
    return this.commonService.delete(`SurveyMgmt/Delete/${id}`)
  }
}
