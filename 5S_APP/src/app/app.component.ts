import { ThemeService } from './service/theme.service';
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
    private storage: Storage,
    private theme: ThemeService
  ) {
    this.darkModeMediaQuery = window.matchMedia('(prefers-color-scheme: dark)');

    this.platform.ready().then(async () => {
      if (this.platform.is('ios') || this.platform.is('android')) {
        try {
          // Initial setup
          await StatusBar.setOverlaysWebView({ overlay: false });
          await this.updateStatusBarTheme(this.darkModeMediaQuery.matches);

          // Listen for theme changes
          this.mediaQueryListener = async (e: MediaQueryListEvent) => {
            await this.updateStatusBarTheme(e.matches);
          };
          this.darkModeMediaQuery.addEventListener(
            'change',
            this.mediaQueryListener
          );
        } catch (err) {
          console.error('Error initializing StatusBar', err);
        }
      }
    });
    this.theme.enableLightMode();
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
      this.darkModeMediaQuery.removeEventListener(
        'change',
        this.mediaQueryListener
      );
    }
  }

  initializePush() {
    PushNotifications.requestPermissions().then((result) => {
      if (result.receive === 'granted') {
        PushNotifications.register();
      }
    });

    PushNotifications.addListener('registration', (token) => {
      console.log('Push registration success, token:', token.value);
      // Gửi token lên server nếu cần
    });

    PushNotifications.addListener('registrationError', (error) => {
      console.error('Push registration error:', error);
    });

    PushNotifications.addListener(
      'pushNotificationReceived',
      (notification) => {
        console.log('Push received: ', notification);
      }
    );

    PushNotifications.addListener(
      'pushNotificationActionPerformed',
      (notification) => {
        console.log('Push action performed', notification);
      }
    );
  }
}
