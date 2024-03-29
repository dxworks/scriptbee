import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ScriptTreeComponent } from './script-tree.component';

describe('ScriptTreeComponent', () => {
  let component: ScriptTreeComponent;
  let fixture: ComponentFixture<ScriptTreeComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ScriptTreeComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ScriptTreeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
