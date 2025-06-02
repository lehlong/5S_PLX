import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ChamDiemTheoKhungThoiGianComponent } from './cham-diem-theo-khung-thoi-gian.component';

describe('ChamDiemTheoKhungThoiGianComponent', () => {
  let component: ChamDiemTheoKhungThoiGianComponent;
  let fixture: ComponentFixture<ChamDiemTheoKhungThoiGianComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ChamDiemTheoKhungThoiGianComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ChamDiemTheoKhungThoiGianComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
