import { Routes } from '@angular/router';
import { AccountComponent } from './account.component';
import { TermsPoliciesComponent } from './page/terms-policies/terms-policies.component';
import { ChangePasswordComponent } from './page/change-password/change-password.component';
import { SettingAccountComponent } from './page/setting-account/setting-account.component';

export const accountRoutes: Routes = [
  {
    path: '',
    children: [
      { path: '', redirectTo: 'account', pathMatch: 'full' },
      { path: '', component: AccountComponent },
      { path: 'terms-and-polices', component: TermsPoliciesComponent },
      { path: 'password', component: ChangePasswordComponent },
      { path: 'setting-account', component: SettingAccountComponent}
    ],
  },
];
