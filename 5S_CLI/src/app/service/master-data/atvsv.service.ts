import { Injectable } from '@angular/core';
import { CommonService } from '../common.service';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class ATVSVService {
  constructor(private commonService: CommonService) {}

  // Tìm kiếm ATVSV
  search(params: any): Observable<any> {
    return this.commonService.get('ATVSV/Search', params);
  }

  // Lấy tất cả danh sách ATVSV
  getAll(): Observable<any> {
    return this.commonService.get('ATVSV/GetAll');
  }

  // Tạo mới ATVSV
  create(params: any): Observable<any> {
    return this.commonService.post('ATVSV/Insert', params);
  }

  // Cập nhật ATVSV
  update(params: any): Observable<any> {
    return this.commonService.put('ATVSV/Update', params);
  }

  // Xóa ATVSV
  delete(id: string): Observable<any> {
    return this.commonService.delete(`ATVSV/Delete/${id}`);
  }

  // Lấy danh sách ATVSV theo StoreId
  getByStoreId(storeId: string): Observable<any[]> {
    return this.commonService.get(`ATVSV/GetByStoreId/${storeId}`);
  }
}