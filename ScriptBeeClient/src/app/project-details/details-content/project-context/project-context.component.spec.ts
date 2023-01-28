import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProjectContextComponent } from './project-context.component';

describe('ProjectContextComponent', () => {
  let component: ProjectContextComponent;
  let fixture: ComponentFixture<ProjectContextComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ProjectContextComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ProjectContextComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
