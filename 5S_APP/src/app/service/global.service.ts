import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, Subject } from 'rxjs';
import { LoadingController } from '@ionic/angular';

@Injectable({
  providedIn: 'root',
})
export class GlobalService {
  private apiCallCount: number = 0;
  private userNameSubject: BehaviorSubject<string | null> = new BehaviorSubject<
    string | null
  >(null);

  rightSubject: Subject<string> = new Subject<string>();
  rightData: any = [];
  breadcrumbSubject: Subject<boolean> = new Subject<boolean>();
  breadcrumb: any = [];
  private loading: HTMLIonLoadingElement | null = null;


  orgCode?: string = localStorage.getItem('companyCode')?.toString();
  warehouseCode?: string = localStorage.getItem('warehouseCode')?.toString();

  constructor(
    private loadingCtrl: LoadingController
  ) {
    this.rightSubject.subscribe((value) => {
      localStorage.setItem('userRights', value);
      this.rightData = value;
    });
    this.breadcrumbSubject.subscribe((value) => {
      this.breadcrumb = value;
    });
  }
  setUserName(userName: string): void {
    this.userNameSubject.next(userName);
    localStorage.setItem('userName', userName); // Lưu userName vào localStorage nếu cần
  }

  getUserName() {
    var usString: any = localStorage.getItem('userName');
    // var username = JSON.parse(usString);
    return usString;
  }

  // Phương thức để lấy userName từ localStorage khi cần
  loadUserNameFromStorage(): void {
    const storedUserName = localStorage.getItem('userName');
    if (storedUserName) {
      this.userNameSubject.next(storedUserName);
    }
  }

  setBreadcrumb(value: any) {
    localStorage.setItem('breadcrumb', JSON.stringify(value));
    this.breadcrumbSubject.next(value);
  }

  getBreadcrumb() {
    try {
      if (this.breadcrumb && this.breadcrumb?.length > 0) {
        return this.breadcrumb;
      }
      const breadcrumb = localStorage.getItem('breadcrumb');
      return breadcrumb ? JSON.parse(breadcrumb) : null;
    } catch (e) {
      return null;
    }
  }

  getUserInfo() {
    try {
      const info = localStorage.getItem('UserInfo');
      return info ? JSON.parse(info) : null;
    } catch (e) {
      return null;
    }
  }

  setUserInfo(value: any) {
    localStorage.setItem('UserInfo', JSON.stringify(value));
  }

  setRightData(data: any) {
    this.rightSubject.next(data);
    localStorage.setItem('userRights', data);
  }

  getRightData() {
    try {
      if (this.rightData?.length > 0) {
        return this.rightData;
      }
      const rights = localStorage.getItem('userRights');
      return rights ? JSON.parse(rights) : null;
    } catch (e) {
      return null;
    }
  }

  checkPermissions(permissions: string) {
    try {
      const listPermissions = this.getRightData();
      if (listPermissions) {
        return listPermissions?.includes(permissions);
      }
      return false;
    } catch (e) {
      return false;
    }
  }


  incrementApiCallCountNoLoading(): void {
    this.apiCallCount++;
  }

  decrementApiCallCountNoLoading(): void {
    this.apiCallCount--;
  }

  isValidSelected() {
    return this.orgCode != 'undefined' && this.warehouseCode != 'undefined'
      ? false
      : true;
  }

  private isCreatingLoading = false;
  loadingTimeout: any = null;
  private messageInterval: any = null;

  async loadingShow(message: string = '', outoTimeout = true) {
    if (this.loading || this.isCreatingLoading) return;

    this.isCreatingLoading = true;
    this.loading = await this.loadingCtrl.create({
      message: '',
      spinner: null,
      cssClass: 'custom-loading',
      backdropDismiss: false,
      showBackdrop: true
    });
    this.isCreatingLoading = false;

    await this.loading.present();

    // ====== XỬ LÝ TÁCH MESSAGE & LUÂN PHIÊN ======
    this.handleDualMessage(message);

    // ====== Timeout auto close ======
    if (outoTimeout) {
      this.loadingTimeout = setTimeout(() => {
        this.forceCloseLoading();
      }, 10000);
    }
  }

  private handleDualMessage(message: string) {

    // Tách message thành nhiều phần
    const msgs = message.split('./.').map(m => m.trim()).filter(m => m.length > 0);
    if (msgs.length === 0) return;

    const el = document.querySelector('.custom-loading .loading-content') as HTMLElement;
    if (!el) return;

    let index = 0;

    // Clear interval cũ
    if (this.messageInterval) clearInterval(this.messageInterval);

    // Thiết lập message đầu tiên
    el.innerHTML = msgs[index];

    // Luân phiên từng message mỗi 1.2s
    this.messageInterval = setInterval(() => {
      index = (index + 1) % msgs.length; // chạy vòng tròn
      el.innerHTML = msgs[index];
    }, 2200);
  }


  async loadingHide() {
    if (this.messageInterval) {
      clearInterval(this.messageInterval);
      this.messageInterval = null;
    }
    await this.forceCloseLoading();
  }

  private async forceCloseLoading() {
    if (!this.loading) return;

    try {
      await new Promise(r => setTimeout(r, 700));

      await this.loading.dismiss(null, "");

    } catch (e) {
      // overlay có thể đã bị dismiss trước - không sao
    } finally {
      this.loading = null;
    }

  }

  formatDateToSendServer(date: any): string {
    if (!date) return '';

    const d = (date instanceof Date) ? date : new Date(date);

    const year = d.getFullYear();
    const month = String(d.getMonth() + 1).padStart(2, '0');
    const day = String(d.getDate()).padStart(2, '0');
    const hours = String(d.getHours()).padStart(2, '0');
    const minutes = String(d.getMinutes()).padStart(2, '0');
    const seconds = String(d.getSeconds()).padStart(2, '0');

    // Giữ nguyên giờ local
    return `${year}-${month}-${day}T${hours}:${minutes}:${seconds}`;
  }

}
