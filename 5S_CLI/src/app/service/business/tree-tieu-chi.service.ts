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
Insert(params: any): Observable<any> {
    console.log("33333",params);
    return this.commonService.post('TieuChi/InsertTreeGroup', params)
  }
//   GetMenuTree() {
//     return this.commonService.get('TreeTieuChi/GetMenu');
//   }

//   GetMenuWithTreeRight(param: any) {
//     return this.commonService.get('TieuChi/GetMenuWithTreeRight', param);
//   }

//   Update(data: any) {
//     return this.commonService.put('TreeTieuChi/Update', data);
//   }

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

