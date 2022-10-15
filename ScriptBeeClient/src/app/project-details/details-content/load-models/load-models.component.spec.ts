import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LoadModelsComponent } from './load-models.component';

describe('LoadModelsComponent', () => {
  let component: LoadModelsComponent;
  let fixture: ComponentFixture<LoadModelsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ LoadModelsComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(LoadModelsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
