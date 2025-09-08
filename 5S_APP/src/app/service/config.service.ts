import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { Preferences } from '@capacitor/preferences';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root',
})
export class ConfigService {
  private apiUrlSubject = new BehaviorSubject<any>(environment.baseApi);
  apiUrl$ = this.apiUrlSubject.asObservable();

  constructor() {
    this.loadApiUrl();
  }

  private async loadApiUrl() {
    const { value } = await Preferences.get({ key: 'apiUrl' });
    if (value) {
      this.apiUrlSubject.next(value);
    }
  }

  async setApiUrl(url: string) {
    await Preferences.set({ key: 'apiUrl', value: url });
    this.apiUrlSubject.next(url);
  }

  getApiUrlSync(): string {
    return this.apiUrlSubject.value; // luôn có string (từ environment hoặc Preferences)
  }
}
