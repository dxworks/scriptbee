import { ComponentFixture, TestBed } from '@angular/core/testing';

import { OutputErrorsComponent } from './output-errors.component';

describe('OutputErrorsComponent', () => {
  let component: OutputErrorsComponent;
  let fixture: ComponentFixture<OutputErrorsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ OutputErrorsComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(OutputErrorsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
