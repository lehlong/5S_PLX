import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { RouterModule, Router } from '@angular/router';
import { NzAvatarModule } from 'ng-zorro-antd/avatar';
import { NzBreadCrumbModule } from 'ng-zorro-antd/breadcrumb';
import { NzDropDownModule } from 'ng-zorro-antd/dropdown';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzLayoutModule } from 'ng-zorro-antd/layout';
import { NzMenuModule } from 'ng-zorro-antd/menu';
import { NzSpinModule } from 'ng-zorro-antd/spin';
import { ShareModule } from '../../shared/share-module';
import { GlobalService } from '../../service/global.service';
import { SidebarMenuService } from '../../service/sidebar-menu.service';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-main-layout',
  standalone: true,
  imports: [NzLayoutModule,
    RouterModule,
    NzMenuModule,
    NzIconModule,
    NzSpinModule,
    CommonModule,
    NzDropDownModule,
    NzAvatarModule,
    NzBreadCrumbModule,
    ShareModule],
  templateUrl: './main-layout.component.html',
  styleUrl: './main-layout.component.scss',
  animations:[]
})
export class MainLayoutComponent implements OnInit {

  isCollapsed: boolean = false;
  loading: boolean = false
  user: any = {}
  userAvatar : string = '/img/profile.png'
  dataSidebarMenu: any[] = []
  breadcrumbs: any = []
  currentUrl: string = ''

  constructor(
    private sidebarMenuService: SidebarMenuService,
    private globalService: GlobalService,
    private router: Router,
    private cdr: ChangeDetectorRef,
  ) {
    this.user = this.globalService.getUserInfo();
    this.globalService.breadcrumbSubject.subscribe((value) => {
      this.breadcrumbs = value
    })
    this.globalService.getLoading().subscribe((value) => {
      this.loading = value
    })
    this.router.events.subscribe(() => {
      this.currentUrl = this.router.url?.split('?')[0] || ''
      this.dataSidebarMenu = this.transformMenuList(this.dataSidebarMenu)
      this.cdr.detectChanges()
    })
  }

  ngOnInit(): void {
    if(this.user.urlImage != null && this.user.urlImage != ''){
      this.userAvatar = environment.urlFiles + this.user.urlImage;
    }
    this.getSidebarMenu();
  }
  getSidebarMenu(){
    this.sidebarMenuService
      .getMenuOfUser({userName: this.user.userName})
      .subscribe((res) => {
        this.dataSidebarMenu = this.transformMenuList(res?.children || [])
      })
  }
  transformMenuList(data: any[], level = 1): any[] {
    return data.map((menu) => this.transformMenu(menu, level))
  }

  transformMenu(data: any, level = 0): any {
    const hasMatchingChild = (menu: any, url: string): boolean => {
      if (menu.url && `/${menu.url}` === url) {
        return true
      }
      if (menu.children) {
        return menu.children.some((child: any) => hasMatchingChild(child, url))
      }
      return false
    }

    return {
      level: level,
      title: data.name || data.title || '',
      icon: data.icon || '',
      open: hasMatchingChild(data, this.currentUrl),
      url: data.url,
      selected: `/${data.url}` === this.currentUrl,
      disabled: false,
      children: data.children
        ? this.transformMenuList(data.children, level + 1)
        : undefined,
    }
  }
  navigateTo(url: string): void {
    if (url && !this.loading) {
      this.router.navigate([url])
    }
  }
  changePass(): void {
    this.router.navigate(['/system-manager/profile'])
  }
  logOut() {
    localStorage.clear();
    this.router.navigate(['/login'])
  }
}
