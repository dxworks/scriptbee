import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UploadModelsComponent } from './upload-models.component';

describe('UploadModelsComponent', () => {
  let component: UploadModelsComponent;
  let fixture: ComponentFixture<UploadModelsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ UploadModelsComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(UploadModelsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
