export const environment = {
  production: false,
  apiMap: 'http://sso.d2s.com.vn:1347/api',
  tmsUrl: 'https://tmsapi.plxna.com.vn/api',
  // apiFile: 'http://sso.d2s.com.vn:1347/',

  // apiMap: 'http://localhost:5203/api',
  // apiFile: 'http://localhost:5203/'
  // sửa cả apiFile trong evaluate.ts
  baseApi: '',
  get apiUrl() {
    return `${this.baseApi}/api`;
  },
  get apiFile() {
    return `${this.baseApi}/`;
  }
};
