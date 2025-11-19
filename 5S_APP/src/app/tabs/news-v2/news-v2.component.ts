import { IonModal, AlertController } from '@ionic/angular';
import { Geolocation } from '@capacitor/geolocation';
import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { NewsService } from 'src/app/service/news.service';
import { SharedModule } from 'src/app/shared/shared.module';
import 'leaflet';
import 'leaflet-routing-machine'
import { IonTab } from "@ionic/angular/standalone";
import { MessageService } from 'src/app/service/message.service';

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

  presentingElement!: HTMLElement | null;
  map!: L.Map;
  routingControl: any;
  userMarker: L.Marker | null = null;
  destMarker: L.Marker | null = null;
  tempMarker: L.Marker | null = null;

  activeTab: string = 'home';
  showFooter = true;
  lstAddress: any[] = [];
  isOpen = true
  newsList: any = [];
  isLstS = false;
  directions: any[] = [];
  routeLayer: any;
  dataTms: any
  isModalShare = false;
  dataInsert: any = {
    isActive: true,
    id: "",
    name: "",
    address: "",
    description: "",
    kinhDo: "",
    viDo: ""
  }
  isMap2 = false;

  constructor(
    private router: Router,
    private messageService: MessageService,
    private service: NewsService,
  ) { }

  async ngOnInit() {
    this.getDotTinhTms()
    try {
      const loc = await this.getCurrentLocationFast();
      this.initMap(loc.latitude, loc.longitude);
      this.dataInsert.kinhDo = loc.longitude;
      this.dataInsert.viDo = loc.latitude;

      this.getNearbyStations()

      this.presentingElement = document.querySelector('.ion-page');
    } catch (err) {
      this.dataInsert.kinhDo = 10.762622;
      this.dataInsert.viDo = 106.660172;

      this.getNearbyStations()

      console.warn('⚠️ Không lấy được vị trí, dùng mặc định', err);
      this.initMap(10.762622, 106.660172); // VD: TP.HCM
    }
  }
  async initMap(lat: number, lng: number) {

    if (this.map) {
      this.map.remove();
    }

    L.Icon.Default.mergeOptions({
      iconRetinaUrl: 'assets/media/marker-icon.png',
      iconUrl: 'assets/media/marker-icon.png',
      shadowUrl: 'assets/media/marker-shadow.png',
    });

    // Khởi tạo map
    this.map = L.map('map', {
      keyboard: false,  // tránh tabindex gây focus ARIA
      doubleClickZoom: false
    }).setView([lat, lng], 15);

    // Tile layer
    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
      attribution: '© OpenStreetMap contributors',
    }).addTo(this.map);

    // Marker vị trí người dùng
    this.userMarker = L.marker([lat, lng])
      .addTo(this.map)
      .bindPopup('Vị trí của bạn')
      .openPopup();

    // DOUBLE CLICK để chọn điểm đến
    this.map.on('click', (e: any) => {
      const latlng = e.latlng;
      this.setDestination(latlng.lat, latlng.lng);
      console.log('Điểm đến', latlng.lat, latlng.lng);
    });
  }


  async canDismiss(data?: undefined, role?: string) {
    return role !== 'gesture';
  }

  getNearbyStations() {
    console.log('aaaaaaaaaaaaaaaa');

    this.service.getNearbyStations(this.dataInsert.viDo, this.dataInsert.kinhDo).subscribe({
      next: (data) => {
        console.log('data', data);
        this.renderStationsOnMap(data);
      },
      error: (response) => {
        console.log(response)
      },
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


  async initMap2(lat: number, lng: number) {
    L.Icon.Default.mergeOptions({
      iconRetinaUrl: 'assets/media/marker-icon.png',
      iconUrl: 'assets/media/marker-icon.png',
      shadowUrl: 'assets/media/marker-shadow.png',
    });

    // Khởi tạo map
    this.map = L.map('map2', {
      doubleClickZoom: false
    }).setView([lat, lng], 15);
    console.log(this.map);

    // Thêm tile layer
    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
      attribution: '© OpenStreetMap contributors',
    }).addTo(this.map);

    // Marker vị trí người dùng
    this.tempMarker = L.marker([lat, lng])
      .addTo(this.map)
      .bindPopup('Vị trí của bạn')
      .openPopup();


    this.map.on('dblclick', (e: L.LeafletMouseEvent) => {
      const { lat, lng } = e.latlng;

      // Xóa marker cũ nếu có
      if (this.tempMarker) {
        this.map.removeLayer(this.tempMarker);
      }
      this.dataInsert.viDo = lat;
      this.dataInsert.kinhDo = lng;
      // Tạo marker mới
      this.tempMarker = L.marker([lat, lng], {
        draggable: true  // cho phép kéo nếu muốn
      }).addTo(this.map);

      console.log('Selected:', lat, lng);
    });

  }

  openSelectMap() {
    this.isMap2 = true;
    setTimeout(() => {
      this.initMap2(this.dataInsert.viDo, this.dataInsert.kinhDo);
    }, 200);
  }

  insertMap() {
    if (this.dataInsert.name == null || this.dataInsert.name == '') {
      this.messageService.show(
        `Không được bỏ trống tên trạm xăng`,
        'warning'
      );
      return;
    }
    this.service.insertMap(this.dataInsert).subscribe({
      next: (data) => {
        this.messageService.show(
          `Thêm trạm xăng thành công`,
          'success'
        );
      }, error: (err) => {

        console.error('Lỗi khi gọi insertMap:', err);
        this.messageService.show(
          `Đã có trạm xăng được thêm trong phạm vi này`,
          'danger'
        );
      },
    });
    console.log('dataInsert', this.dataInsert);
  }


  getDistance(lat1: number, lon1: number, lat2: number, lon2: number): number {
    const R = 6371e3; // bán kính Trái Đất (m)
    const toRad = (x: number) => (x * Math.PI) / 180;

    const φ1 = toRad(lat1);
    const φ2 = toRad(lat2);
    const Δφ = toRad(lat2 - lat1);
    const Δλ = toRad(lon2 - lon1);

    const a =
      Math.sin(Δφ / 2) ** 2 +
      Math.cos(φ1) * Math.cos(φ2) * Math.sin(Δλ / 2) ** 2;

    const c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a));

    return R * c; // khoảng cách tính bằng mét
  }
  stationMarkers: any[] = [];

  renderStationsOnMap(stations: any[]) {
    const gasIcon = L.icon({
      iconUrl: 'assets/media/gasIcon2.png',
      iconSize: [25, 25],
    });
    stations.forEach(st => {
      const marker = L.marker([st.viDo, st.kinhDo],
        { icon: gasIcon }
      )
        .addTo(this.map)
        .bindPopup(`
        <b>${st.name}</b><br>
        Khoảng cách: ${st.khoangCach.toFixed(0)} m
      `);

      marker.on('click', () => {
        this.showRoute(st.viDo, st.kinhDo);
      });
      this.stationMarkers.push(marker);
    });
  }

  //TMS lấy giá xăng dầu

  getDotTinhTms() {
    this.service.searchTms().subscribe({
      next: (data) => {
        //lấy đợt tính mới nhất có được phê duyệt
        const c = data.data
          .filter((x: any) => x.status === "04")
          .sort((a: any, b: any) => new Date(b.date).getTime() - new Date(a.date).getTime())[0]

        this.getCalculateDotTms(c.id)
      },
      error: (err) => {
        console.error('Lỗi khi gọi getAll:', err);
      },
    });
  }

  getCalculateDotTms(param: string) {
    this.service.getCalculateTms(param).subscribe({
      next: (data) => {
        this.dataTms = data
        // console.log('Kết quả tính toán:', data);
      },
      error: (err) => {
        console.error('Lỗi khi gọi getAll:', err);
      },
    });
  }

  getLatestStatus04(list: any[]) {
    const filtered = list.filter(item => item.status === "04");

    if (filtered.length === 0) return null; // không có bản ghi nào

    return filtered.reduce((latest, item) => {
      return new Date(item.date) > new Date(latest.date) ? item : latest;
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
