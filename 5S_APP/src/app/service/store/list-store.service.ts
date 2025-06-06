import { Observable } from 'rxjs';
import { CommonService } from './../common.service';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class ListStoreService {
  constructor(private commonService: CommonService) {}

  getAllSurveyMgmt(params: any): Observable<any> {
    return this.commonService.get('SurveyMgmt/Search', params);
  }

}
