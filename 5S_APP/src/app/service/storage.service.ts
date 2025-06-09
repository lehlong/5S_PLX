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

  async getByCode(code: any) {
    await this.init()
    let eva = await this.storage.get('lstEvaluate')
    if (!eva) return;

    const header = eva.lstHeader.find((x: any) => x.code === code);
    if (!header) return;

    return {
      header: header,
      lstEvaluate: eva.lstEvaluate.filter((x: any) => x.evaluateHeaderCode == header.code),
      lstImages: eva.lstEvaluate.filter((x: any) => x.evaluateHeaderCode == header.code) || []
    }
  }

  async getByStoreIdKiKhaoSatId() {

  }
  async updateEvaluateList(data: any) {
    await this.init();
    let eva = await this.storage.get('lstEvaluate');
    if (!eva) return;

    for (const item of data.lstEvaluate) {
      const index = eva.lstEvaluate.findIndex((x: any) => x.code === item.code);
      eva.lstEvaluate[index] = item;
    }
    // array.forEach(element => {

    // });

    await this.storage.set('lstEvaluate', eva);
  }

}
