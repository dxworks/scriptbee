import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SelectedScriptComponent } from './selected-script.component';

describe('SelectedScriptComponent', () => {
  let component: SelectedScriptComponent;
  let fixture: ComponentFixture<SelectedScriptComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ SelectedScriptComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(SelectedScriptComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
