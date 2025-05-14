import { Injectable } from '@angular/core'
import { CommonService } from '../common.service'
import { Observable } from 'rxjs'

@Injectable({
  providedIn: 'root',
})
export class AccountTypeService {
  constructor(private commonService: CommonService) { }

  search(params: any): Observable<any> {
    return this.commonService.get('AccountType/Search', params)
  }

  getAll(): Observable<any> {
    return this.commonService.get('AccountType/GetAll')
  }

  create(params: any): Observable<any> {
    return this.commonService.post('AccountType/Insert', params)
  }

  update(params: any): Observable<any> {
    return this.commonService.put('AccountType/Update', params)
  }

  delete(id: string): Observable<any> {
    return this.commonService.delete(`AccountType/Delete/${id}`)
  }
}
