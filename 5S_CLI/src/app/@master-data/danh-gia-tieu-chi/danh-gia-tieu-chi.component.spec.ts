import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DanhGiaTieuChiComponent } from './danh-gia-tieu-chi.component';

describe('DanhGiaTieuChiComponent', () => {
  let component: DanhGiaTieuChiComponent;
  let fixture: ComponentFixture<DanhGiaTieuChiComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DanhGiaTieuChiComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DanhGiaTieuChiComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
