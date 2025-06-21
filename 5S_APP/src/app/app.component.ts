import { Component, OnDestroy } from '@angular/core';
import { IonApp, IonRouterOutlet } from '@ionic/angular/standalone';
import { Storage } from '@ionic/storage-angular';
import { Platform } from '@ionic/angular';
import { StatusBar, Style } from '@capacitor/status-bar';
import { PushNotifications } from '@capacitor/push-notifications';

@Component({
  selector: 'app-root',
  templateUrl: 'app.component.html',
  standalone: true,
  imports: [IonApp, IonRouterOutlet],
})
export class AppComponent implements OnDestroy {
  private darkModeMediaQuery: MediaQueryList;
  private mediaQueryListener: ((e: MediaQueryListEvent) => void) | undefined;

  constructor(
    private platform: Platform,
    private storage: Storage
  ) {
    this.darkModeMediaQuery = window.matchMedia('(prefers-color-scheme: dark)');

    this.platform.ready().then(async () => {
      if (this.platform.is('ios') || this.platform.is('android')) {
        try {
          // Initial setup
          await StatusBar.setOverlaysWebView({ overlay: false });
          await this.updateStatusBarTheme(this.darkModeMediaQuery.matches);

          // Đăng ký topic 'news' với FirebasePlugin (Firebasex)
          if ((window as any).FirebasePlugin) {
            (window as any).FirebasePlugin.subscribe('news', () => {
              console.log('Đã subscribe topic news');
            }, (error: any) => {
              console.error('Lỗi subscribe topic:', error);
            });
          }

          // Đăng ký push notification
          PushNotifications.requestPermissions().then(result => {
            if (result.receive === 'granted') {
              PushNotifications.register();
            }
          });

          // Lắng nghe đăng ký token thành công
          PushNotifications.addListener('registration', token => {
            console.log('Push registration success, token:', token.value);
            // Gửi token lên server nếu cần
          });

          // Lắng nghe lỗi đăng ký
          PushNotifications.addListener('registrationError', error => {
            console.error('Push registration error:', error);
          });

          // Lắng nghe khi nhận thông báo
          PushNotifications.addListener('pushNotificationReceived', notification => {
            console.log('Push received: ', notification);
            if (notification && notification.title && notification.body) {
              alert(`${notification.title}\n${notification.body}`);
            }
          });

          // Lắng nghe khi người dùng tương tác với thông báo
          PushNotifications.addListener('pushNotificationActionPerformed', notification => {
            console.log('Push action performed', notification);
          });

          // Listen for theme changes
          this.mediaQueryListener = async (e: MediaQueryListEvent) => {
            await this.updateStatusBarTheme(e.matches);
          };
          this.darkModeMediaQuery.addEventListener('change', this.mediaQueryListener);
        } catch (err) {
          console.error('Error initializing StatusBar', err);
        }
      }
    });
  }

  ngOnInit() {
    this.storage.create();
  }

  private async updateStatusBarTheme(isDark: boolean) {
    try {
      if (isDark) {
        await StatusBar.setStyle({ style: Style.Dark });
        await StatusBar.setBackgroundColor({ color: '#000000' });
      } else {
        await StatusBar.setStyle({ style: Style.Light });
        await StatusBar.setBackgroundColor({ color: '#ffffff' });
      }
    } catch (err) {
      console.error('Error updating StatusBar theme', err);
    }
  }

  ngOnDestroy() {
    if (this.mediaQueryListener) {
      this.darkModeMediaQuery.removeEventListener('change', this.mediaQueryListener);
    }
  }

}
