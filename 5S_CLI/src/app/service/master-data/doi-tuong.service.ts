import { Injectable } from '@angular/core'
import { CommonService } from '../common.service'
import { Observable } from 'rxjs'

@Injectable({
  providedIn: 'root',
})
export class DoiTuongService {
  constructor(private commonService: CommonService) { }

  search(params: any): Observable<any> {
    return this.commonService.get('DoiTuong/Search', params)
  }

  getAll(): Observable<any> {
    return this.commonService.get('DoiTuong/GetAll')
  }

  create(params: any): Observable<any> {
    return this.commonService.post('DoiTuong/Insert', params)
  }

  update(params: any): Observable<any> {
    return this.commonService.put('DoiTuong/Update', params)
  }

  delete(id: string): Observable<any> {
    return this.commonService.delete(`DoiTuong/Delete/${id}`)
  }
}
