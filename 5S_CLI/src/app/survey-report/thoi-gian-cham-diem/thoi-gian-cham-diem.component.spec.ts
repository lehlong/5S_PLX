import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ThoiGianChamDiemComponent } from './thoi-gian-cham-diem.component';

describe('ThoiGianChamDiemComponent', () => {
  let component: ThoiGianChamDiemComponent;
  let fixture: ComponentFixture<ThoiGianChamDiemComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ThoiGianChamDiemComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ThoiGianChamDiemComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
