import { Component } from '@angular/core';
import { ShareModule } from '../../shared/share-module';
import { NzMessageService } from 'ng-zorro-antd/message';
import { Router } from '@angular/router';
import { GlobalService } from '../../service/global.service';
import { AuthService } from '../../service/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [ShareModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent {
  constructor(
    private message: NzMessageService,
    private router: Router,
    private globalService: GlobalService,
    private authService: AuthService,
  ) { }
  model = {
    userName: '',
    password: '',
  }
  processLogin() {
    if (this.model.userName == '' || this.model.password == '') {
      this.message.create('error', `Vui lòng nhập đầy đủ tên đăng nhập và mật khẩu!`);
      return;
    }
    this.authService.login(this.model).subscribe({
      next: (response) => {
        localStorage.setItem('token', response.accessToken)
        localStorage.setItem('refreshToken', response.refreshToken)
        this.globalService.setUserInfo(response.accountInfo)
        localStorage.setItem('openSidebar', 'true')
        localStorage.setItem('companyCode', response?.accountInfo?.organizeCode)
        localStorage.setItem('warehouseCode', response?.accountInfo?.warehouseCode)
        const userName = response?.accountInfo?.userName;
        if (userName) {
          this.globalService.setUserName(userName);
        }
        this.authService
          .getRightOfUser({ userName: response?.accountInfo?.userName })
          .subscribe({
            next: (rights) => {
              this.globalService.setRightData(JSON.stringify(rights || []))
              this.router.navigate(['/'])
            },
            error: (error) => {
              this.message.create('error', `Lỗi không lấy được danh sách quyền của user!`);
              console.log('Lỗi hệ thống:', error)
            },
          })
      },
      error: (error) => {
        this.message.create('error', `Tên đăng nhập hoặc mật khẩu không đúng!`);
        console.log(error)
      }
    });
  }
}
