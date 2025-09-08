import { bootstrapApplication } from '@angular/platform-browser';
import { AppComponent } from './app/app.component';
import { appConfig } from './app/app.config';
import { provideIonicAngular } from '@ionic/angular/standalone';
import { IonicStorageModule } from '@ionic/storage-angular';
import { importProvidersFrom } from '@angular/core';
import { Preferences } from '@capacitor/preferences';

async function initApp() {
  const { value } = await Preferences.get({ key: 'baseApi' });
  if (value) {
    (window as any)['APP_BASE_API'] = value; 
  }
  bootstrapApplication(AppComponent, {
    ...appConfig,
    providers: [
      ...(appConfig.providers || []),
      provideIonicAngular({
        innerHTMLTemplatesEnabled: true, 
      }),
      importProvidersFrom(IonicStorageModule.forRoot()), 
    ],
  });
}
initApp();
