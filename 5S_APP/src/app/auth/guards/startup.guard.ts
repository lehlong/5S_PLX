import { Injectable } from '@angular/core';
import { CanActivate, Router, UrlTree } from '@angular/router';
import { AuthService } from 'src/app/service/auth.service';

@Injectable({
  providedIn: 'root',
})
export class StartupGuard implements CanActivate {
  constructor(private authService: AuthService, private router: Router) {}

  canActivate(): UrlTree {
    if (this.authService.isLoggedIn()) {
      return this.router.parseUrl('/home');
    } else {
      return this.router.parseUrl('/news-v2');
    }
  }
}
