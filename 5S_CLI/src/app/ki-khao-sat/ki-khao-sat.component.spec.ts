import { ComponentFixture, TestBed } from '@angular/core/testing';

import { KiKhaoSatComponent } from './ki-khao-sat.component';

describe('KiKhaoSatComponent', () => {
  let component: KiKhaoSatComponent;
  let fixture: ComponentFixture<KiKhaoSatComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [KiKhaoSatComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(KiKhaoSatComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
