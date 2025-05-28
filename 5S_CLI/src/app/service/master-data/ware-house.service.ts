import { Injectable } from '@angular/core'
import { CommonService } from '../common.service'
import { Observable } from 'rxjs'

@Injectable({
  providedIn: 'root',
})
export class WareHouseService {
  constructor(private commonService: CommonService) { }

  search(params: any): Observable<any> {
    return this.commonService.get('WareHouse/Search', params)
  }

  getAll(): Observable<any> {
    return this.commonService.get('WareHouse/GetAll')
  }

   getATVSV(param:any): Observable<any> {
    return this.commonService.get(`WareHouse/GetATVSV?headerId=${param}`)
  }
  create(params: any): Observable<any> {
    return this.commonService.post('WareHouse/Insert', params)
  }

  update(params: any): Observable<any> {
    return this.commonService.put('WareHouse/Update', params)
  }

  delete(id: string): Observable<any> {
    return this.commonService.delete(`WareHouse/Delete/${id}`)
  }
}
