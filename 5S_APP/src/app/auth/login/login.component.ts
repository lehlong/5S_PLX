import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { GlobalService } from '../../service/global.service';
import { AuthService } from 'src/app/service/auth.service';
import { SharedModule } from 'src/app/shared/shared.module';
import { ToastController } from '@ionic/angular';
import { MessageService } from 'src/app/service/message.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [SharedModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent {
  constructor(
    private router: Router,
    private globalService: GlobalService,
    private authService: AuthService,
    private messageService: MessageService
  ) { }
  model = {
    userName: '',
    password: '',
  }

  saveAccount: boolean = false

  processLogin() {
    if (this.model.userName == '' || this.model.password == '') {
      this.messageService.show('Vui lòng nhập đầy đủ tên đăng nhập và mật khẩu!', 'danger');
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
              this.messageService.show(`Lỗi không lấy được danh sách quyền của user!`, 'warning');
              console.log('Lỗi hệ thống:', error)
            },
          })
      },
      error: (error) => {
        this.messageService.show(`Tên đăng nhập hoặc mật khẩu không đúng!`, 'warning');
        console.log(error)
      }
    });
  }

  showPassword = false;

  togglePasswordVisibility() {
    this.showPassword = !this.showPassword;
  }
}
