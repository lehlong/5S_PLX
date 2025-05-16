import { Injectable } from '@angular/core'
import { CommonService } from '../common.service'
import { Observable } from 'rxjs'

@Injectable({
  providedIn: 'root',
})
export class KiKhaoSatService {
  constructor(private commonService: CommonService) { }

  search(params: any): Observable<any> {
    return this.commonService.get('KiKhaoSat/Search', params)
  }

  getAll(): Observable<any> {
    return this.commonService.get('KiKhaoSat/GetAll')
  }
  getAlldata(): Observable<any> {
    return this.commonService.get('KiKhaoSat/GetAllData')
  }

  create(params: any): Observable<any> {
    return this.commonService.post('KiKhaoSat/Insert', params)
  }

  update(params: any): Observable<any> {
    return this.commonService.put('KiKhaoSat/Update', params)
  }

  delete(id: string): Observable<any> {
    return this.commonService.delete(`KiKhaoSat/Delete/${id}`)
  }

}
