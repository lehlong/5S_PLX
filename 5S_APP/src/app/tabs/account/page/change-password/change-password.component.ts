import { Component, OnInit } from '@angular/core';
import { SharedModule } from 'src/app/shared/shared.module';
import { IonInputPasswordToggle } from '@ionic/angular/standalone';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';

@Component({
  selector: 'app-change-password',
  templateUrl: './change-password.component.html',
  styleUrls: ['./change-password.component.scss'],
  imports: [SharedModule, IonInputPasswordToggle, ReactiveFormsModule],
  standalone: true,
})
export class ChangePasswordComponent implements OnInit {
  changePasswordForm!: FormGroup;
  constructor(private fb: FormBuilder) {}

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
  onSubmit() {
    if (this.changePasswordForm.invalid) {
      this.changePasswordForm.markAllAsTouched();
      return;
    }

    const { oldPassword, newPassword } = this.changePasswordForm.value;
    console.log('Submit thành công:', oldPassword, newPassword);
  }
}
