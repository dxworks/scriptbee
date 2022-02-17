import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DetailsContentComponent } from './details-content.component';

describe('DetailsContentComponent', () => {
  let component: DetailsContentComponent;
  let fixture: ComponentFixture<DetailsContentComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DetailsContentComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DetailsContentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
