import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ExpandedBundleRowComponent } from './expanded-bundle-row.component';

describe('ExpandedBundleRowComponent', () => {
  let component: ExpandedBundleRowComponent;
  let fixture: ComponentFixture<ExpandedBundleRowComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ExpandedBundleRowComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ExpandedBundleRowComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
