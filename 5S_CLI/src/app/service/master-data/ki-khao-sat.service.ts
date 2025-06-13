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

  create(params: any): Observable<any> {
    return this.commonService.post('KiKhaoSat/Insert', params)
  }

  getInputKiKhaoSat(params: any): Observable<any> {
    return this.commonService.get(`KiKhaoSat/GetInputKiKhaoSat?idKi=${params}`)
  }

  getInputCopyKy(params: any): Observable<any> {
    return this.commonService.get(`KiKhaoSat/GetInputCopyKy?kyKhaoSatId=${params}`)
  }

  updateKiKhaoSat(params: any): Observable<any> {
    return this.commonService.put('KiKhaoSat/Update', params)
  }


  updateKhaoSatTrangThai(params: any = {}): Observable<any> {
    return this.commonService.put('KiKhaoSat/UpdateKhaoSatTrangThai', params)
  }


  getAll(param:any): Observable<any> {
    return this.commonService.get(`KiKhaoSat/GetAll?kiKhaoSatId=${param}`)
  }
  getAlldata(param:any): Observable<any> {
    return this.commonService.get(`KiKhaoSat/GetAllData?headerId=${param}`)
  }


  delete(id: string): Observable<any> {
    return this.commonService.delete(`KiKhaoSat/Delete/${id}`)
  }

}
