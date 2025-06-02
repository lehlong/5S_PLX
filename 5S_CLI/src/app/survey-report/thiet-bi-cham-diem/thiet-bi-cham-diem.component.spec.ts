import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ThietBiChamDiemComponent } from './thiet-bi-cham-diem.component';

describe('ThietBiChamDiemComponent', () => {
  let component: ThietBiChamDiemComponent;
  let fixture: ComponentFixture<ThietBiChamDiemComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ThietBiChamDiemComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ThietBiChamDiemComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
