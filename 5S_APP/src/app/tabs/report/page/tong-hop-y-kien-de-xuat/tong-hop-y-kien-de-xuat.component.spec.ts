import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';
import { IonicModule } from '@ionic/angular';

import { TongHopYKienDeXuatComponent } from './tong-hop-y-kien-de-xuat.component';

describe('TongHopYKienDeXuatComponent', () => {
  let component: TongHopYKienDeXuatComponent;
  let fixture: ComponentFixture<TongHopYKienDeXuatComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ TongHopYKienDeXuatComponent ],
      imports: [IonicModule.forRoot()]
    }).compileComponents();

    fixture = TestBed.createComponent(TongHopYKienDeXuatComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  }));

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
