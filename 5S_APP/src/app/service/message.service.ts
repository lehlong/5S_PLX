import { Injectable } from '@angular/core';
import { ToastController } from '@ionic/angular';

@Injectable({
  providedIn: 'root'
})
export class MessageService {

  constructor(private toastController: ToastController) { }
  async show(
    message: string,
    type: 'success' | 'danger' | 'warning' | 'primary' = 'primary',
    autoClose = true,
    position: 'top' | 'middle' | 'bottom' = 'top'
  ) {
    const icons: any = {
      success: 'checkmark-circle-outline',
      warning: 'alert-circle-outline',
      danger: 'close-circle-outline',
      info: 'information-circle-outline'
    };

    const toast = await this.toastController.create({
      message,
      duration: autoClose ? 3000 : 0,
      position,
      color: type,
      icon: icons[type],      // ⭐ CHỈ TRỎ ICON
      cssClass: 'custom-toast',
      buttons: [
        {
          icon: 'close',
          role: 'cancel'
        }
      ]
    });

    await toast.present();
  }

}
