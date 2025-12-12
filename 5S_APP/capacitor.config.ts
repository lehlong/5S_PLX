import type { CapacitorConfig } from '@capacitor/cli';

const config: CapacitorConfig = {
  appId: 'com.plx5s.app',
  appName: 'Khảo sát 5S',
  webDir: 'www',
  plugins: {
    StatusBar: {
      overlaysWebView: false,
      style: 'light'
    },
    PushNotifications: {
      presentationOptions: ['badge', 'sound', 'alert'],
    }
  },
  server: {
    cleartext: true,
    androidScheme: "http"
  }
};

export default config;
