import { Injectable } from '@angular/core'
import { Observable } from 'rxjs'
import { CommonService } from '../common.service'

@Injectable({
  providedIn: 'root',
})
export class ConfigDisplayService {
  constructor(private commonService: CommonService) {}

  searchConfigDisplay(params: any): Observable<any> {
    return this.commonService.get('ConfigDisplay/Search', params)
  }

  getAll(): Observable<any> {
    return this.commonService.get('ConfigDisplay/GetAll')
  }

  createConfigDisplay(params: any): Observable<any> {
    return this.commonService.post('ConfigDisplay/Insert', params)
  }

  updateConfigDisplay(params: any): Observable<any> {
    return this.commonService.put('ConfigDisplay/Update', params)
  }

  exportExcelConfigDisplay(params: any): Observable<any> {
    return this.commonService.downloadFile('ConfigDisplay/Export', params)
  }

  deleteConfigDisplay(id: string | number): Observable<any> {
    return this.commonService.delete(`ConfigDisplay/Delete/${id}`)
  }
}
