import { ComponentFixture, TestBed } from '@angular/core/testing';

import { KetQuaChamDiemComponent } from './ket-qua-cham-diem.component';

describe('KetQuaChamDiemComponent', () => {
  let component: KetQuaChamDiemComponent;
  let fixture: ComponentFixture<KetQuaChamDiemComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [KetQuaChamDiemComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(KetQuaChamDiemComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
