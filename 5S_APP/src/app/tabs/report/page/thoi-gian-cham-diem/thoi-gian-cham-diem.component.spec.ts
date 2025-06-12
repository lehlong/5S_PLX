import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';
import { IonicModule } from '@ionic/angular';

import { ThoiGianChamDiemComponent } from './thoi-gian-cham-diem.component';

describe('ThoiGianChamDiemComponent', () => {
  let component: ThoiGianChamDiemComponent;
  let fixture: ComponentFixture<ThoiGianChamDiemComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ ThoiGianChamDiemComponent ],
      imports: [IonicModule.forRoot()]
    }).compileComponents();

    fixture = TestBed.createComponent(ThoiGianChamDiemComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  }));

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
