import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ChucVuComponent } from './chuc-vu.component';

describe('ChucVuComponent', () => {
  let component: ChucVuComponent;
  let fixture: ComponentFixture<ChucVuComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ChucVuComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ChucVuComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
