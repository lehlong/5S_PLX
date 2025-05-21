import {Injectable} from '@angular/core';
import {CommonService} from '../common.service';
import {Observable} from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class TreeTieuChiService {
  constructor(private commonService: CommonService) {}

  GetTreeTieuChi(param: any): Observable<any> {
    return this.commonService.get(`TieuChi/BuildDataForTree?kiKhaoSatId=${param}`);
  }

  addTree(params: any): Observable<any>{
    return this.commonService.post('TieuChi/InsertTreeGroup', params)
  }
  addLeaves(params: any): Observable<any>{
    return this.commonService.post('TieuChi/InsertTreeLeaves', params)
  }
  GetTreeLeaves(param: any): Observable<any> {
    return this.commonService.get(`TieuChi/GetLeaves?id=${param}`);
  }

//   GetMenuWithTreeRight(param: any) {
//     return this.commonService.get('TieuChi/GetMenuWithTreeRight', param);
//   }

  UpdateLeaves(data: any) {
    return this.commonService.put('TieuChi/UpdateLeaves', data);
  }

  // Insert(data: any): Observable<any> {
  //   // return this.commonService.post('TieuChi/InsertTreeGroup', data);
  //   return this.commonService.post('TieuChi/InsertTreeGroup', data);
  // }

//   UpdateOrderTree(dataTree: any) {
//     return this.commonService.put('TreeTieuChi/Update-Order', dataTree);
//   }

//   Delete(id: string | number): Observable<any> {
//     return this.commonService.delete(`TreeTieuChi/Delete/${id}`);
}

