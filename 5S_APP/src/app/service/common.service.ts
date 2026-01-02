import { Injectable } from '@angular/core';
import {
  HttpClient,
  HttpErrorResponse,
  HttpParams,
} from '@angular/common/http';
import { Observable, throwError, BehaviorSubject } from 'rxjs';

import {
  catchError,
  tap,
  map,
  finalize,
  switchMap,
  filter,
  take,
} from 'rxjs/operators';
import { environment } from '../../environments/environment';
import { Router } from '@angular/router';
import { GlobalService } from './global.service';
import { ConfigService } from './config.service';
import { MessageService } from './message.service';

@Injectable({
  providedIn: 'root',
})
export class CommonService {
  private baseUrl = environment.apiUrl;
  private urlMap = environment.apiMap;
  private tmsUrl = environment.tmsUrl;
  private refreshTokenInProgress = false;
  private refreshTokenSubject: BehaviorSubject<any> = new BehaviorSubject<any>(
    null
  );

  constructor(
    private http: HttpClient,
    private router: Router,
    private globalService: GlobalService,
    private messService: MessageService,
    private configService: ConfigService
  ) {
    this.configService.apiUrl$.subscribe((url) => {
      this.baseUrl = url;
    });
  }


  getMap<T>(
    endpoint: string,
    params?: { [key: string]: any },
    showLoading: boolean = true
  ): Observable<T> {
    let httpParams = new HttpParams();
    if (params) {
      Object.keys(params).forEach((key) => {
        if (params[key] !== null && params[key] !== undefined) {
          if (Array.isArray(params[key])) {
            params[key].forEach((value: any) => {
              httpParams = httpParams.append(key, value);
            });
          } else {
            httpParams = httpParams.append(key, params[key]);
          }
        }
      });
    }
    if (showLoading) {
      this.globalService.loadingShow(); // Tăng bộ đếm
    }
    return this.http
      .get<any>(`${this.urlMap}/${endpoint}`, { params: httpParams })
      .pipe(
        tap((response) => {
          if (response.status == false) {
            this.showError(response.messageObject.message);
          }
        }), // Log phản hồi API
        map(this.handleApiResponse),
        catchError((error) => {
          return this.handleError(error, () =>
            this.get<T>(endpoint, params, showLoading)
          );
        }),
        finalize(() => {
          this.globalService.loadingHide();
        })
      );
  }

  postMap<T>(
    endpoint: string,
    data: any,
    showSuccess: boolean = true,
    showLoading: boolean = true
  ): Observable<T> {
    if (showLoading) {
      this.globalService.loadingShow(); // Tăng bộ đếm
    }
    return this.http.post<any>(`${this.urlMap}/${endpoint}`, data).pipe(
      map(this.handleApiResponse),
      tap(() => {
        if (showSuccess) {
          this.showSuccess('Thêm mới thông tin thành công');
        }
      }),
      catchError((error) =>
        this.handleError(error, () =>
          this.post<T>(endpoint, data, showSuccess, showLoading)
        )
      ),
      finalize(() => this.globalService.loadingHide()) // Giảm bộ đếm khi hoàn thành
    );
  }

  getTms<T>(
    endpoint: string,
    params?: { [key: string]: any },
    showLoading: boolean = true
  ): Observable<T> {
    let httpParams = new HttpParams();
    if (params) {
      Object.keys(params).forEach((key) => {
        if (params[key] !== null && params[key] !== undefined) {
          if (Array.isArray(params[key])) {
            params[key].forEach((value: any) => {
              httpParams = httpParams.append(key, value);
            });
          } else {
            httpParams = httpParams.append(key, params[key]);
          }
        }
      });
    }
    if (showLoading) {
      this.globalService.loadingShow(); // Tăng bộ đếm
    }
    return this.http
      .get<any>(`${this.tmsUrl}/${endpoint}`, { params: httpParams })
      .pipe(
        tap((response) => {
          if (response.status == false) {
            this.showError(response.messageObject.message);
          }
        }), // Log phản hồi API
        map(this.handleApiResponse),
        catchError((error) => {
          return this.handleError(error, () =>
            this.get<T>(endpoint, params, showLoading)
          );
        }),
        finalize(() => {
          this.globalService.loadingHide();
        })
      );
  }

  get<T>(
    endpoint: string,
    params?: { [key: string]: any },
    showLoading: boolean = true
  ): Observable<T> {
    let httpParams = new HttpParams();
    if (params) {
      Object.keys(params).forEach((key) => {
        if (params[key] !== null && params[key] !== undefined) {
          if (Array.isArray(params[key])) {
            params[key].forEach((value: any) => {
              httpParams = httpParams.append(key, value);
            });
          } else {
            httpParams = httpParams.append(key, params[key]);
          }
        }
      });
    }
    if (showLoading) {
      this.globalService.loadingShow(); // Tăng bộ đếm
    }
    return this.http
      .get<any>(`${this.baseUrl}/${endpoint}`, { params: httpParams })
      .pipe(
        tap((response) => {
          if (response.status == false) {
            this.showError(response.messageObject.message);
          }
        }), // Log phản hồi API
        map(this.handleApiResponse),
        catchError((error) => {
          return this.handleError(error, () =>
            this.get<T>(endpoint, params, showLoading)
          );
        }),
        finalize(() => {
          this.globalService.loadingHide()// Giảm bộ đếm khi hoàn thành
        })
      );
  }

  post<T>(
    endpoint: string,
    data: any,
    showSuccess: boolean = true,
    showLoading: boolean = true
  ): Observable<T> {
    if (showLoading) {
      this.globalService.loadingShow(); // Tăng bộ đếm
    }
    return this.http.post<any>(`${this.baseUrl}/${endpoint}`, data).pipe(
      map(this.handleApiResponse),
      tap(() => {
        console.log(showSuccess);

        if (showSuccess) {
          this.showSuccess('Thêm mới thông tin thành công');
        }
      }),
      catchError((error) =>
        this.handleError(error, () =>
          this.post<T>(endpoint, data, showSuccess, showLoading)
        )
      ),
      finalize(() => this.globalService.loadingHide()) // Giảm bộ đếm khi hoàn thành
    );
  }

  put<T>(
    endpoint: string,
    data: any,
    showLoading: boolean = true
  ): Observable<T> {
    if (showLoading) {
      this.globalService.loadingShow(); // Tăng bộ đếm
    }
    return this.http.put<any>(`${this.baseUrl}/${endpoint}`, data).pipe(
      map(this.handleApiResponse),
      tap(() => this.showSuccess('Cập nhật thông tin thành công')),
      catchError((error) =>
        this.handleError(error, () => this.put<T>(endpoint, data, showLoading))
      ),
      finalize(() => this.globalService.loadingHide()) // Giảm bộ đếm khi hoàn thành
    );
  }

  delete<T>(
    endpoint: string,
    data: any = {},
    showLoading: boolean = true
  ): Observable<T> {
    if (showLoading) {
      this.globalService.loadingShow(); // Tăng bộ đếm
    }
    return this.http.delete<any>(`${this.baseUrl}/${endpoint}`, data).pipe(
      map(this.handleApiResponse),
      tap(() => this.showSuccess('Xoá thành công')),
      catchError((error) =>
        this.handleError(error, () =>
          this.delete<T>(endpoint, data, showLoading)
        )
      ),
      finalize(() => this.globalService.loadingHide()) // Giảm bộ đếm khi hoàn thành
    );
  }

  deletes<T>(
    endpoint: string,
    data: string | number[],
    showLoading: boolean = true
  ): Observable<T> {
    if (showLoading) {
      this.globalService.loadingShow(); // Tăng bộ đếm
    }
    return this.http
      .request<any>('delete', `${this.baseUrl}/${endpoint}`, { body: data })
      .pipe(
        map(this.handleApiResponse),
        tap(() => this.showSuccess('Xoá thành công')),
        catchError((error) =>
          this.handleError(error, () =>
            this.deletes<T>(endpoint, data, showLoading)
          )
        ),
        finalize(() => this.globalService.loadingHide()) // Giảm bộ đếm khi hoàn thành
      );
  }

  uploadFile(
    endpoint: string,
    file: File,
    paramsUrl?: { [key: string]: any },
    params?: { [key: string]: any },
    showLoading: boolean = true
  ): Observable<any> {
    const formData: FormData = new FormData();
    formData.append('file', file, file.name);
    let httpParams = new HttpParams();
    if (paramsUrl) {
      Object.keys(paramsUrl).forEach((key) => {
        if (paramsUrl[key]) {
          httpParams = httpParams.append(key, paramsUrl[key]);
        }
      });
    }

    if (params) {
      Object.keys(params).forEach((key) => {
        formData.append(key, params[key]);
      });
    }

    if (showLoading) {
      this.globalService.loadingShow(); // Tăng bộ đếm
    }
    return this.http
      .post<any>(`${this.baseUrl}/${endpoint}`, formData, {
        params: httpParams,
      })
      .pipe(
        map(this.handleApiResponse),
        tap(),
        catchError((error) =>
          this.handleError(error, () =>
            this.uploadFile(endpoint, file, paramsUrl, params, showLoading)
          )
        ),
        finalize(() => this.globalService.loadingHide()) // Giảm bộ đếm khi hoàn thành
      );
  }

  uploadFiles(
    endpoint: string,
    files: File[],
    paramsUrl?: { [key: string]: any },
    params?: { [key: string]: any },
    showLoading: boolean = true
  ): Observable<any> {
    const formData: FormData = new FormData();
    files.forEach((file) => {
      formData.append('files', file, file.name);
    });

    let httpParams = new HttpParams();
    if (paramsUrl) {
      Object.keys(paramsUrl).forEach((key) => {
        const value = paramsUrl[key];
        if (Array.isArray(value)) {
          value.forEach((val) => {
            httpParams = httpParams.append(key, val);
          });
        } else if (value) {
          httpParams = httpParams.append(key, value);
        }
      });
    }

    if (params) {
      Object.keys(params).forEach((key) => {
        formData.append(key, params[key]);
      });
    }

    if (showLoading) {
      this.globalService.loadingShow(); // Tăng bộ đếm
    }
    return this.http
      .post<any>(`${this.baseUrl}/${endpoint}`, formData, {
        params: httpParams,
      })
      .pipe(
        map(this.handleApiResponse),
        tap(),
        catchError((error) =>
          this.handleError(error, () =>
            this.uploadFiles(endpoint, files, paramsUrl, params, showLoading)
          )
        ),
        finalize(() => this.globalService.loadingHide()) // Giảm bộ đếm khi hoàn thành
      );
  }

  downloadFile(
    endpoint: string,
    params?: any,
    showLoading: boolean = true
  ): Observable<Blob> {
    if (showLoading) {
      this.globalService.loadingShow(); // Tăng bộ đếm
    }
    return this.http
      .get(`${this.baseUrl}/${endpoint}`, {
        params: params,
        responseType: 'blob',
      })
      .pipe(
        tap(),
        catchError((error) =>
          this.handleError(error, () =>
            this.downloadFile(endpoint, params, showLoading)
          )
        ),
        finalize(() => this.globalService.loadingHide()) // Giảm bộ đếm khi hoàn thành          
      );
  }

  private showSuccess(message: string): void {
    // this.message.create('success', message)
    this.messService.show(message, 'success')
  }
  private showError(mess: string, type: any = 'warning'): void {
    const has404 = mess.includes("404");
    if (has404) {
      mess = "Cấu hình api không chính xác!!"
      type = 'danger'
    }
    if (mess.includes("Http failure during parsing")) return
    this.messService.show(mess, type)
  }

  private handleError = (
    error: HttpErrorResponse,
    retryCallback: () => Observable<any>
  ): Observable<any> => {
    let errorMessage = 'Unknown error!';
    if (error.error instanceof ErrorEvent) {
      errorMessage = `Error: ${error.error.message}`;
    } else {
      if (error.status === 401) {
        if (!this.refreshTokenInProgress) {
          this.refreshTokenInProgress = true;
          this.refreshTokenSubject.next(null);

          return this.refreshToken().pipe(
            switchMap(({ data }: any) => {
              this.refreshTokenInProgress = false;
              this.refreshTokenSubject.next(data);
              localStorage.setItem('token', data?.accessToken);
              localStorage.setItem('refreshToken', data?.refreshToken);
              return retryCallback();
            }),
            catchError((refreshError) => {
              this.refreshTokenInProgress = false;
              localStorage.clear();
              this.router.navigate(['/login']);
              return throwError(refreshError);
            })
          );
        } else {
          return this.refreshTokenSubject.pipe(
            filter((result: any) => result !== null),
            take(1),
            switchMap(() => retryCallback())
          );
        }
      }
      if (error.error && error.error.messageObject) {
        if (error.error.messageObject.messageDetail) {
          console.log(error.error.messageObject.messageDetail);

          errorMessage = error.error.messageObject.messageDetail;
        } else {
          errorMessage = `MSG${error.error.messageObject.code} ${error.error.message}`;
        }

      } else {
        errorMessage = `Error Code: ${error.status}\nMessage: ${error.message}`;
      }
    }
    this.showError(errorMessage)
    return throwError(errorMessage)
  };

  private refreshToken(): Observable<any> {
    const refreshToken = localStorage.getItem('refreshToken');
    return this.http.post<any>(`${this.baseUrl}/Auth/RefreshToken`, {
      refreshToken,
    });
  }

  private handleApiResponse(response: any): any {
    if (
      response.messageObject.code !== '200' &&
      response.messageObject.code !== '' &&
      response.messageObject.code !== '0100' &&
      response.messageObject.code !== '0103' &&
      response.messageObject.code !== '0105'
    ) {
      throw {
        status: response.messageObject.code,
        error: response,
      };
    }
    return response.data;
  }
}
