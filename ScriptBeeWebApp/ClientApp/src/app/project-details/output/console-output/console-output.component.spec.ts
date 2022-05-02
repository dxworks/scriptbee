import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ConsoleOutputComponent } from './console-output.component';

describe('ConsoleOutputComponent', () => {
  let component: ConsoleOutputComponent;
  let fixture: ComponentFixture<ConsoleOutputComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ConsoleOutputComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ConsoleOutputComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
