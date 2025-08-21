import { Injectable } from '@angular/core'
import { Observable } from 'rxjs'
import { CommonService } from './common.service'

@Injectable({
  providedIn: 'root',
})
export class NewsService {
  constructor(private commonService: CommonService) { }

  search(params: any): Observable<any> {
    return this.commonService.get('News/Search', params)
  }

  getAll(): Observable<any> {
    return this.commonService.get('News/GetAll')
  }

  create(params: any): Observable<any> {
    return this.commonService.post('News/Insert', params)
  }

  update(params: any): Observable<any> {
    return this.commonService.put('News/Update', params)
  }

  delete(id: string): Observable<any> {
    return this.commonService.delete(`News/Delete/${id}`)
  }
}
