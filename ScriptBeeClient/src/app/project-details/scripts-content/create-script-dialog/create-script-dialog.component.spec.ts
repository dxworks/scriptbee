import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateScriptDialogComponent } from './create-script-dialog.component';

describe('CreateScriptDialogComponent', () => {
  let component: CreateScriptDialogComponent;
  let fixture: ComponentFixture<CreateScriptDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CreateScriptDialogComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CreateScriptDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
