import { Component, OnInit } from '@angular/core';
import { SharedModule } from 'src/app/shared/shared.module';
import { IonInputPasswordToggle } from '@ionic/angular/standalone';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { MessageService } from 'src/app/service/message.service';
import { AuthService } from 'src/app/service/auth.service';

@Component({
  selector: 'app-change-password',
  templateUrl: './change-password.component.html',
  styleUrls: ['./change-password.component.scss'],
  imports: [SharedModule, IonInputPasswordToggle, ReactiveFormsModule],
  standalone: true,
})
export class ChangePasswordComponent implements OnInit {
  changePasswordForm!: FormGroup;
  // private readonly currentPassword = 'd2s@123456';
  constructor(
    private fb: FormBuilder,
    private messageService: MessageService,
    private authService: AuthService
  ) {}

  ngOnInit() {
    this.changePasswordForm = this.fb.group(
      {
        oldPassword: ['', Validators.required],
        newPassword: ['', [Validators.required]],
        confirmNewPassword: ['', Validators.required],
      },
      {
        validators: this.passwordsMatchValidator,
      }
    );
  }
  passwordsMatchValidator(form: FormGroup) {
    const newPassword = form.get('newPassword')?.value;
    const confirmPassword = form.get('confirmNewPassword')?.value;
    if (newPassword !== confirmPassword) {
      form.get('confirmNewPassword')?.setErrors({ mismatch: true });
    } else {
      form.get('confirmNewPassword')?.setErrors(null);
    }
    return null;
  }
  CheckDiffPassword(form: FormGroup) {
    const oldPassword = form.get('oldPassword')?.value;
    const newPassword = form.get('newPassword')?.value;
    if (oldPassword && newPassword && oldPassword === newPassword) {
      form.get('newPassword')?.setErrors({ sameAsOld: true });
    } else {
      if (form.get('newPassword')?.hasError('sameAsOld')) {
        form.get('newPassword')?.setErrors(null);
      }
    }
    return null;
  }
  onSubmit() {
    const { oldPassword, newPassword } = this.changePasswordForm.value;
    const userInfoRaw = localStorage.getItem('UserInfo');
    const userInfo = userInfoRaw ? JSON.parse(userInfoRaw) : null;
    const userName = userInfo?.userName || '';
    if (userName) {
      this.authService
        .changePassword({ userName, oldPassword, newPassword })
        .subscribe({
          next: (res) => {
            this.messageService.show('Đổi mật khẩu thành công!', 'success');
            console.log('API Response:', res);
            this.changePasswordForm.reset();
          },
          error: (err) => {
            console.error(err);
            this.messageService.show(
              err.error?.message || 'Đổi mật khẩu thất bại!',
              'danger'
            );
          },
        });
    }
  }
}
