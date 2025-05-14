import type { CapacitorConfig } from '@capacitor/cli';

const config: CapacitorConfig = {
  appId: 'io.ionic.starter',
  appName: 'Khảo sát 5S',
  webDir: 'www',
  plugins: {
    StatusBar: {
      overlaysWebView: false,
      style: 'light'
    }
  }
};

export default config;
