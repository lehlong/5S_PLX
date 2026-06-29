import { Injectable } from '@angular/core'
import { CommonService } from '../common.service'
import { Observable } from 'rxjs'

@Injectable({
  providedIn: 'root',
})
export class OfficeService {
  constructor(private commonService: CommonService) { }

  search(params: any): Observable<any> {
    return this.commonService.get('Office/Search', params)
  }

  getAll(): Observable<any> {
    return this.commonService.get('Office/GetAll')
  }
  getATVSV(param:any): Observable<any> {
    return this.commonService.get(`Office/GetATVSV?headerId=${param}`)
  }


  create(params: any): Observable<any> {
    return this.commonService.post('Office/Insert', params)
  }

  update(params: any): Observable<any> {
    return this.commonService.put('Office/Update', params)
  }

  delete(id: string): Observable<any> {
    return this.commonService.delete(`Office/Delete/${id}`)
  }
}
