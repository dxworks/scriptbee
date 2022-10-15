import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LinkModelsComponent } from './link-models.component';

describe('LinkModelsComponent', () => {
  let component: LinkModelsComponent;
  let fixture: ComponentFixture<LinkModelsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ LinkModelsComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(LinkModelsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
