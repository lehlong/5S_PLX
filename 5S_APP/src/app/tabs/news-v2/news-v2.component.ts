import { routes } from './../../app.routes';
import { IonModal } from '@ionic/angular';
import { Geolocation } from '@capacitor/geolocation';
import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { NewsService } from 'src/app/service/news.service';
import { SharedModule } from 'src/app/shared/shared.module';
import 'leaflet';
import 'leaflet-routing-machine'
import { IonTab } from "@ionic/angular/standalone";
import { Capacitor } from '@capacitor/core';
// import { Capacitor } from '@capacitor/core/types/global';

declare let L: any;

@Component({
  selector: 'app-news-v2',
  templateUrl: './news-v2.component.html',
  styleUrls: ['./news-v2.component.scss'],
  imports: [IonTab, SharedModule],
})
export class NewsV2Component implements OnInit {
  @ViewChild('myModal') modal!: IonModal;
  @ViewChild('tab', { read: ElementRef }) tab!: ElementRef;
  @ViewChild('myModal', { read: ElementRef }) modalRef!: ElementRef<HTMLIonModalElement>;


  private locationPermissionGranted: boolean = false;

  map!: L.Map;
  routingControl: any;
  userMarker: L.Marker | null = null;
  destMarker: L.Marker | null = null;
  activeTab: string = 'home';
  showFooter = true;
  lstAddress: any[] = [];
  isOpen = true
  newsList: any = [];
  isLstS = false;

  fuelPrices = [
    { name: 'Xăng RON 95-IV', price: 24500, unit: 'đ/lít' },
    { name: 'Xăng E5 RON 92-II', price: 23600, unit: 'đ/lít' },
    { name: 'Dầu DO 0,05S-II', price: 21300, unit: 'đ/lít' },
    { name: 'Dầu hỏa', price: 20900, unit: 'đ/lít' },
  ];

  constructor(
    private router: Router,
    private service: NewsService
  ) { }

  async ngOnInit() {
    try {
      const loc = await this.getCurrentLocationFast();

      this.initMap(loc.latitude, loc.longitude);
    } catch (err) {
      console.warn('⚠️ Không lấy được vị trí, dùng mặc định', err);
      this.initMap(10.762622, 106.660172); // VD: TP.HCM
    }
  }

  async initMap(lat: number, lng: number) {
    // Cấu hình icon mặc định
    L.Icon.Default.mergeOptions({
      iconRetinaUrl: 'media/marker-icon-2V3QKKVC.png',
      iconUrl: 'media/marker-icon-2V3QKKVC.png',
      shadowUrl: 'media/marker-shadow.png',
    });

    // Khởi tạo map
    this.map = L.map('map').setView([lat, lng], 15);

    // Thêm tile layer
    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
      attribution: '© OpenStreetMap contributors',
    }).addTo(this.map);

    // Marker vị trí người dùng
    this.userMarker = L.marker([lat, lng])
      .addTo(this.map)
      .bindPopup('Vị trí của bạn')
      .openPopup();

    // Bắt sự kiện click lên bản đồ để đặt điểm đến
    this.map.on('click', (e: L.LeafletMouseEvent) => {
      const { lat, lng } = e.latlng;
      console.log('Clicked at:', lat, lng);
      this.setDestination(lat, lng);
    });
  }

  // Đặt điểm đến và vẽ route
  setDestination(destLat: number, destLng: number) {
    this.isLstS = false;
    if (!this.userMarker) {
      alert('Chưa có vị trí hiện tại!');
      return;
    }

    // Xóa marker điểm đến cũ nếu có
    if (this.destMarker) this.map.removeLayer(this.destMarker);

    this.destMarker = L.marker([destLat, destLng])
      .addTo(this.map)
      .bindPopup('Điểm đến')
      .openPopup();

    this.showRoute(destLat, destLng);
  }


  onSearchbarClick() {
    this.isLstS = true;
    this.modal?.setCurrentBreakpoint(0.75);
  }

  currentBreakpoint = 0.08;
  private reopening = false;

  // onBreakpointChange(ev: any) {
  //   this.currentBreakpoint = ev.detail.breakpoint;

  //   const modalEl = this.modalRef.nativeElement as HTMLElement;    // nếu modal thấp hơn 0.5 thì cho phép click xuyên qua
  //   modalEl.style.pointerEvents = this.currentBreakpoint < 0.5 ? 'none' : 'auto';
  //   console.log(this.currentBreakpoint);

  // }

  // async onModalDismiss() {
  //   // Khi modal đóng do click ra ngoài → mở lại và co về 0.1
  //   if (this.reopening) return;
  //   this.reopening = true;
  //   await this.modal.present();
  //   await this.modal.setCurrentBreakpoint(0.08);
  //   this.reopening = false;
  //   this.isLstS = false;
  // }

  onBreakpointChange(ev: any) {
    this.currentBreakpoint = ev.detail.breakpoint;
    const modalEl = this.modalRef.nativeElement;

    // Khi modal thấp hơn 0.5 → cho phép click xuyên qua (trừ phần nội dung modal)
    if (this.currentBreakpoint < 0.5) {
      modalEl.style.pointerEvents = 'none';
      modalEl.querySelector('ion-content')!.style.pointerEvents = 'auto';
    } else {
      modalEl.style.pointerEvents = 'auto';
    }
  }

  // 🟩 Khi modal bị đóng do backdrop hoặc vuốt xuống
  async onModalDismiss() {
    if (this.reopening) return;
    this.reopening = true;

    const modalEl = this.modalRef.nativeElement;
    await modalEl.present();
    await modalEl.setCurrentBreakpoint(0.08);

    this.reopening = false;
    this.isLstS = false;
  }


  directions: any[] = [];
  routeLayer: any;
  showRoute(destLat: number, destLng: number) {
    if (!this.userMarker) return;

    const userLatLng = this.userMarker.getLatLng();
    const from = L.latLng(userLatLng.lat, userLatLng.lng);
    const to = L.latLng(destLat, destLng);

    if (this.routingControl) this.map.removeControl(this.routingControl);

    this.routingControl = L.Routing.control({
      waypoints: [from, to],
      router: L.Routing.osrmv1({ serviceUrl: 'https://router.project-osrm.org/route/v1/' }),
      lineOptions: { styles: [{ color: '#0d6efd', weight: 5 }] },
      routeWhileDragging: false,
      show: false,  // ẩn danh sách cũ
      addWaypoints: false,
    }).addTo(this.map);

    this.routingControl.on('routesfound', (e: any) => {
      const route = e.routes[0];
      this.directions = route.instructions; // hoặc route.legs[0].steps map như trước
      console.log(this.directions);

    });
  }

  getDirectionIcon(type: string): string {
    const iconMap: { [key: string]: string } = {
      'Head': 'arrow-up-outline',
      'Left': 'arrow-back-outline',
      'Right': 'arrow-forward-outline',
      'SlightRight': 'return-down-forward-outline',
      'SlightLeft': 'return-down-back-outline',
      'OnRamp': 'log-in-outline',
      'OffRamp': 'log-out-outline',
      'Continue': 'arrow-up-outline',
      'SharpRight': 'corner-down-right-outline',
      'SharpLeft': 'corner-down-left-outline',
      'Straight': 'arrow-up-outline',
      'Roundabout': 'refresh-outline',
      'Destination': 'location-outline',
      'DestinationRight': 'location-outline',
      'DestinationLeft': 'location-outline'
    };

    return iconMap[type] || 'navigate-outline'; // default icon
  }

  // Lấy vị trí hiện tại nhanh chóng
  private async getCurrentLocationFast(): Promise<{ latitude: number; longitude: number }> {
    if (!this.locationPermissionGranted) {
      const perm = await Geolocation.checkPermissions();
      console.log(perm);

      if (perm.location !== 'granted') {
        const requestPerm = await Geolocation.requestPermissions();
        console.log(requestPerm);
        if (requestPerm.location !== 'granted') {
          console.log('Không có quyền truy cập vị trí');
        }
      }
      this.locationPermissionGranted = true;
    }

    const position = await Geolocation.getCurrentPosition({
      enableHighAccuracy: true,
      timeout: 5000,
      maximumAge: 60000,
    });

    return {
      latitude: position.coords.latitude,
      longitude: position.coords.longitude,
    };
  }


  async handleInput(event: Event) {
    const target = event.target as HTMLIonSearchbarElement;
    const query = target.value?.toLowerCase() || '';
    this.lstAddress = await this.searchLocation(query)
    console.log(this.lstAddress)
  }

  async searchLocation(query: string) {
    const url = `https://photon.komoot.io/api/?q=${encodeURIComponent(query)}&limit=5`;
    const res = await fetch(url);
    const data = await res.json();

    return data.features.map((item: any) => ({
      latitude: item.geometry.coordinates[1],
      longitude: item.geometry.coordinates[0],
      displayName: item.properties.name || item.properties.city || item.properties.country
    }));
  }

  getAllNews() {
    this.service.getAll().subscribe({
      next: (data) => {
        console.log('Danh sách news:', data);
        this.newsList = data;
      },
      error: (err) => {
        console.error('Lỗi khi gọi getAll:', err);
      },
    });
  }
  setTab(tab: string) {
    this.activeTab = tab;
  }
  goToLogin() {
    this.router.navigate(['/login'], { replaceUrl: true });
    console.log('goLogin');
  }
  goToDetail(newsId: any) {
    this.router.navigate(['/news', newsId]);
  }
}
