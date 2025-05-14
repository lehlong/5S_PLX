import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SurveyMgmtComponent } from './survey-mgmt.component';

describe('SurveyMgmtComponent', () => {
  let component: SurveyMgmtComponent;
  let fixture: ComponentFixture<SurveyMgmtComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SurveyMgmtComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SurveyMgmtComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
