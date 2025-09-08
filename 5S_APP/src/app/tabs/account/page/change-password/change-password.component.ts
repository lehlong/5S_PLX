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

@Component({
  selector: 'app-change-password',
  templateUrl: './change-password.component.html',
  styleUrls: ['./change-password.component.scss'],
  imports: [SharedModule, IonInputPasswordToggle, ReactiveFormsModule],
  standalone: true,
})
export class ChangePasswordComponent implements OnInit {
  changePasswordForm!: FormGroup;
  private readonly currentPassword = 'd2s@123456';
  constructor(
    private fb: FormBuilder,
    private messageService: MessageService
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

  // check mật khẩu cũ có đúng không
  if (oldPassword !== this.currentPassword) {
    this.messageService.show('Mật khẩu cũ không đúng!', 'danger');
    return;
  }

  // check mật khẩu mới phải khác mật khẩu cũ
  if (oldPassword === newPassword) {
    this.messageService.show(
      'Mật khẩu mới không được trùng mật khẩu cũ!',
      'warning'
    );
    return;
  }

  this.messageService.show('Đổi mật khẩu thành công!', 'success');
  console.log('Submit thành công:', oldPassword, newPassword);
}

}
