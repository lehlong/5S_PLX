import {Injectable} from '@angular/core';
import {CommonService} from '../common.service';
import {Observable} from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class SmsConfigService {
  constructor(private commonService: CommonService) {}

  GetSMS() {
    return this.commonService.get('SmsConfig/GetSMS');
  }
  UpdateSMS(data : any) {
    return this.commonService.put('SmsConfig/UpdateSMS', data);
  }
  
}
