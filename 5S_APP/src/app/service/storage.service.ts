import { Injectable } from '@angular/core';
import { Storage } from '@ionic/storage-angular';

@Injectable({
  providedIn: 'root'
})
export class StorageService {
  private isReady = false;

  constructor(private storage: Storage) { }

  async init() {
    if (!this.isReady) {
      await this.storage.create();
      this.isReady = true;
    }
  }

  async set(key: string, value: any) {
    await this.init();
    return this.storage.set(key, value);
  }

  async get(key: string) {
    await this.init();
    return this.storage.get(key);
  }

  async remove(key: string) {
    await this.init();
    return this.storage.remove(key);
  }

  async clear() {
    await this.init();
    return this.storage.clear();
  }

}
