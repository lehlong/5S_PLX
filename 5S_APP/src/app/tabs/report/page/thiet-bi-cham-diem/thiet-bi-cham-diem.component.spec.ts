import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';
import { IonicModule } from '@ionic/angular';

import { ThietBiChamDiemComponent } from './thiet-bi-cham-diem.component';

describe('ThietBiChamDiemComponent', () => {
  let component: ThietBiChamDiemComponent;
  let fixture: ComponentFixture<ThietBiChamDiemComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ ThietBiChamDiemComponent ],
      imports: [IonicModule.forRoot()]
    }).compileComponents();

    fixture = TestBed.createComponent(ThietBiChamDiemComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  }));

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
