import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SelectableTreeComponent } from './selectable-tree.component';

describe('SelectableTreeComponent', () => {
  let component: SelectableTreeComponent;
  let fixture: ComponentFixture<SelectableTreeComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ SelectableTreeComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(SelectableTreeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
