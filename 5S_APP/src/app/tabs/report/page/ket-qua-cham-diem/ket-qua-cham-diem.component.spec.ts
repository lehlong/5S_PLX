import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';
import { IonicModule } from '@ionic/angular';

import { KetQuaChamDiemComponent } from './ket-qua-cham-diem.component';

describe('KetQuaChamDiemComponent', () => {
  let component: KetQuaChamDiemComponent;
  let fixture: ComponentFixture<KetQuaChamDiemComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ KetQuaChamDiemComponent ],
      imports: [IonicModule.forRoot()]
    }).compileComponents();

    fixture = TestBed.createComponent(KetQuaChamDiemComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  }));

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
