import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DoiTuongComponent } from './doi-tuong.component';

describe('DoiTuongComponent', () => {
  let component: DoiTuongComponent;
  let fixture: ComponentFixture<DoiTuongComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DoiTuongComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DoiTuongComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
