import { Component, OnDestroy } from '@angular/core';
import { IonApp, IonRouterOutlet } from '@ionic/angular/standalone';
import { Storage } from '@ionic/storage-angular';
import { Platform } from '@ionic/angular';
import { StatusBar, Style } from '@capacitor/status-bar';

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
