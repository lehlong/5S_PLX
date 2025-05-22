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

  buildObjCreate(params: any): Observable<any> {
    return this.commonService.get(`KiKhaoSat/BuildObjCreate?id=${params}`)
  }
  getAll(param:any): Observable<any> {
    return this.commonService.get(`KiKhaoSat/GetAll?kiKhaoSatId=${param}`)
  }
  getAlldata(param:any): Observable<any> {
    return this.commonService.get(`KiKhaoSat/GetAllData?headerId=${param}`)
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
