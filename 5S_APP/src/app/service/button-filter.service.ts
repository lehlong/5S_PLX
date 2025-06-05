import { Observable } from 'rxjs';
import { CommonService } from './common.service';
import { Injectable } from '@angular/core';
@Injectable({
  providedIn: 'root', 
})

export class ButtonFilterService {
  constructor(private commonService: CommonService) {}
  getAllStore(params: any): Observable<any> {
    return this.commonService.get('Store/GetAll',params)
  }
}
