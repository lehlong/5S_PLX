import { Observable } from 'rxjs';
import { CommonService } from './common.service';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class HomeService {
  constructor(private commonService: CommonService) {}

  getAllChucVu(){
    return this.commonService.get('ChucVu/getall')
  }
}
