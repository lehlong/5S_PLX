import { Injectable } from '@angular/core'
import { CommonService } from '../common.service'
import { Observable } from 'rxjs'

@Injectable({
  providedIn: 'root',
})
export class KhoXangDauService {
  constructor(private commonService: CommonService) { }

  search(params: any): Observable<any> {
    return this.commonService.get('KhoXangDau/Search', params)
  }

  getAll(): Observable<any> {
    return this.commonService.get('KhoXangDau/GetAll')
  }

   getATVSV(param:any): Observable<any> {
    return this.commonService.get(`KhoXangDau/GetATVSV?headerId=${param}`)
  }
  create(params: any): Observable<any> {
    return this.commonService.post('KhoXangDau/Insert', params)
  }

  update(params: any): Observable<any> {
    return this.commonService.put('KhoXangDau/Update', params)
  }

  delete(id: string): Observable<any> {
    return this.commonService.delete(`KhoXangDau/Delete/${id}`)
  }
}
