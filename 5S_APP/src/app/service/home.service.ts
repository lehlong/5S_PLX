import { Observable } from 'rxjs';
import { CommonService } from './common.service';
import { Injectable } from '@angular/core';
import { ConfigService } from './config.service';

@Injectable({
  providedIn: 'root',
})
export class HomeService {
  constructor(
    private commonService: CommonService,
    private configService: ConfigService
  ) {
    this.configService.apiUrl$.subscribe((url) => {
    });
  }

  getAllChucVu() {
    return this.commonService.get('ChucVu/getall')
  }

  getDataHome(params: any) {
    return this.commonService.get(`AppEvaluate/GetDataHome?userName=${params}`)
  }

  search(): Observable<any> {
    return this.commonService.get('SurveyMgmt/Search')
  }

}
