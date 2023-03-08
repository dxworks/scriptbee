import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ScriptsContentComponent } from './scripts-content.component';

describe('ScriptsContentComponent', () => {
  let component: ScriptsContentComponent;
  let fixture: ComponentFixture<ScriptsContentComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ScriptsContentComponent],
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ScriptsContentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
