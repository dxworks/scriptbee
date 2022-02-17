import { ComponentFixture, TestBed } from '@angular/core/testing';

import { NoScriptsComponent } from './no-scripts.component';

describe('NoScriptsComponent', () => {
  let component: NoScriptsComponent;
  let fixture: ComponentFixture<NoScriptsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ NoScriptsComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(NoScriptsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
