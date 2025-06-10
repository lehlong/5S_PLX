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
  },
  server: {
  cleartext: true, // Cho phép gọi HTTP
  androidScheme: 'http' // Không ép dùng HTTPS
}
};

export default config;
