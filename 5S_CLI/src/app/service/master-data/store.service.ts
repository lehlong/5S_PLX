import { Injectable } from '@angular/core'
import { CommonService } from '../common.service'
import { Observable } from 'rxjs'

@Injectable({
  providedIn: 'root',
})
export class StoreService {
  constructor(private commonService: CommonService) { }

  search(params: any): Observable<any> {
    return this.commonService.get('Store/Search', params)
  }

  getAll(): Observable<any> {
    return this.commonService.get('Store/GetAll')
  }

  create(params: any): Observable<any> {
    return this.commonService.post('Store/Insert', params)
  }

  update(params: any): Observable<any> {
    return this.commonService.put('Store/Update', params)
  }

  delete(id: string): Observable<any> {
    return this.commonService.delete(`Store/Delete/${id}`)
  }
}
