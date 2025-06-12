import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';
import { IonicModule } from '@ionic/angular';

import { ChamDiemTheoKhungThoiGianComponent } from './cham-diem-theo-khung-thoi-gian.component';

describe('ChamDiemTheoKhungThoiGianComponent', () => {
  let component: ChamDiemTheoKhungThoiGianComponent;
  let fixture: ComponentFixture<ChamDiemTheoKhungThoiGianComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ ChamDiemTheoKhungThoiGianComponent ],
      imports: [IonicModule.forRoot()]
    }).compileComponents();

    fixture = TestBed.createComponent(ChamDiemTheoKhungThoiGianComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  }));

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
