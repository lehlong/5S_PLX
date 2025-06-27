import {Injectable} from '@angular/core';
import {CommonService} from '../common.service';
import {Observable} from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class TreeTieuChiService {
  constructor(private commonService: CommonService) {}

  BuildDataForTree(param: any): Observable<any> {
    return this.commonService.get(`TieuChi/BuildDataForTree?kiKhaoSatId=${param}`);
  }

  addTree(params: any): Observable<any>{
    return this.commonService.post('TieuChi/InsertTreeGroup', params)
  }
  addLeaves(params: any): Observable<any>{
    return this.commonService.post('TieuChi/InsertTreeLeaves', params)
  }
  GetTreeLeaves(param: any, kiKhaoSatId: any): Observable<any> {
    return this.commonService.get(`TieuChi/GetLeaves?pId=${param}&kiKhaoSatId=${kiKhaoSatId}`);
  }
   CheckLeaves(param: any, kiKhaoSatId: any): Observable<any> {
    return this.commonService.get(`TieuChi/CheckLeaves?pId=${param}&kiKhaoSatId=${kiKhaoSatId}`);
  }



  UpdateLeaves(data: any) {
    return this.commonService.put('TieuChi/UpdateLeaves', data);
  }

  UpdateTreeGroup(data: any) {
    return this.commonService.put('TieuChi/UpdateTreeGroup', data);
  }

  UpdateOrderTree(data: any) {
    return this.commonService.put('TieuChi/UpdateOrderTree', data);
  }
  UpdateOrderLeaves(data: any) {
    return this.commonService.put('TieuChi/UpdateOrderLeaves', data);
  }

//   Delete(id: string | number): Observable<any> {
//     return this.commonService.delete(`TreeTieuChi/Delete/${id}`);
}

