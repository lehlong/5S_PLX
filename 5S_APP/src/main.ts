import { bootstrapApplication } from '@angular/platform-browser';
import { AppComponent } from './app/app.component';
import { appConfig } from './app/app.config';
import { provideIonicAngular } from '@ionic/angular/standalone';
import { IonicStorageModule } from '@ionic/storage-angular';
import { importProvidersFrom } from '@angular/core';

bootstrapApplication(AppComponent, {
  ...appConfig,
  providers: [
    ...(appConfig.providers || []),
    provideIonicAngular({
      innerHTMLTemplatesEnabled: true   // 🔑 cho phép HTML trong Alert
    }),
    importProvidersFrom(IonicStorageModule.forRoot()) // ✅ đúng cách để dùng Storage
  ]
});
