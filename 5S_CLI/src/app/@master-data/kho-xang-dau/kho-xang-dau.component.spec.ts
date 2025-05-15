import { ComponentFixture, TestBed } from '@angular/core/testing';

import { KhoXangDauComponent } from './kho-xang-dau.component';

describe('KhoXangDauComponent', () => {
  let component: KhoXangDauComponent;
  let fixture: ComponentFixture<KhoXangDauComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [KhoXangDauComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(KhoXangDauComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
