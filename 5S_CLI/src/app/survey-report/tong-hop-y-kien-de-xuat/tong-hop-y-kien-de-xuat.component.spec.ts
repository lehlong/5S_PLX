import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TongHopYKienDeXuatComponent } from './tong-hop-y-kien-de-xuat.component';

describe('TongHopYKienDeXuatComponent', () => {
  let component: TongHopYKienDeXuatComponent;
  let fixture: ComponentFixture<TongHopYKienDeXuatComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TongHopYKienDeXuatComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TongHopYKienDeXuatComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
