import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ExpandedPluginRowComponent } from './expanded-plugin-row.component';

describe('ExpandedPluginRowComponent', () => {
  let component: ExpandedPluginRowComponent;
  let fixture: ComponentFixture<ExpandedPluginRowComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ExpandedPluginRowComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ExpandedPluginRowComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
