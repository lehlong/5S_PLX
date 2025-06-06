import { Injectable } from '@angular/core'
import { CommonService } from '../common.service'
import { Observable } from 'rxjs'

@Injectable({
  providedIn: 'root',
})
export class ChucVuService {
  constructor(private commonService: CommonService) { }

  search(params: any): Observable<any> {
    return this.commonService.get('ChucVu/Search', params)
  }

  getAll(): Observable<any> {
    return this.commonService.get('ChucVu/GetAll')
  }

  create(params: any): Observable<any> {
    return this.commonService.post('ChucVu/Insert', params)
  }

  update(params: any): Observable<any> {
    return this.commonService.put('ChucVu/Update', params)
  }

  delete(id: string): Observable<any> {
    return this.commonService.delete(`ChucVu/Delete/${id}`)
  }
}
