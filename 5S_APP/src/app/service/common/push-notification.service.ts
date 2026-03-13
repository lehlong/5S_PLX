import { Injectable } from '@angular/core';
import { PushNotifications } from '@capacitor/push-notifications';

@Injectable({
  providedIn: 'root'
})
export class PushNotificationService {

  private listeners: { remove: () => void }[] = [];
  private registered = false;

  constructor() {}

  async initialize() {
    if (this.registered) return;
    this.registered = true;

    await PushNotifications.requestPermissions().then(result => {
      if (result.receive === 'granted') {
        PushNotifications.register();
      }
    });

    const reg = await PushNotifications.addListener('registration', (token) => {
      console.log('FCM Token:', token.value);
    });
    this.listeners.push(reg);

    const regErr = await PushNotifications.addListener('registrationError', (error) => {
      console.error('FCM Registration Error:', error);
    });
    this.listeners.push(regErr);

    const receive = await PushNotifications.addListener('pushNotificationReceived', (n) => {
      console.log('Push received:', n);
    });
    this.listeners.push(receive);

    const action = await PushNotifications.addListener('pushNotificationActionPerformed', (n) => {
      console.log('Push action performed:', n);
    });
    this.listeners.push(action);
  }

  clear() {
    console.log('Removing FCM listeners...');
    this.listeners.forEach(l => l.remove());
    this.listeners = [];
    this.registered = false;
  }
}