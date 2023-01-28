import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CurrentlyLoadedModelsComponent } from './currently-loaded-models.component';

describe('CurrentlyLoadedModelsComponent', () => {
  let component: CurrentlyLoadedModelsComponent;
  let fixture: ComponentFixture<CurrentlyLoadedModelsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CurrentlyLoadedModelsComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CurrentlyLoadedModelsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
