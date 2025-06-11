
import { Injectable } from '@angular/core';
import { ToastController } from '@ionic/angular';

@Injectable({
  providedIn: 'root'
})
export class MessageService {
  constructor(private toastController: ToastController) {}

  async show(message: string, color: 'success' | 'danger' | 'warning' | 'primary' = 'primary', duration: number = 3000, position: 'top' | 'middle' | 'bottom' = 'top') {
    const toast = await this.toastController.create({
      message,
      duration,
      position,
      color
    });
    toast.present();
  }
}
