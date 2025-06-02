import { ComponentFixture, TestBed } from '@angular/core/testing';

import { HinhAnhChamDiemComponent } from './hinh-anh-cham-diem.component';

describe('HinhAnhChamDiemComponent', () => {
  let component: HinhAnhChamDiemComponent;
  let fixture: ComponentFixture<HinhAnhChamDiemComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [HinhAnhChamDiemComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(HinhAnhChamDiemComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
