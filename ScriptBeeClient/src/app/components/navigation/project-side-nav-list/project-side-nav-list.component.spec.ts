import { beforeEach, describe, expect, it } from 'vitest';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ProjectSideNavListComponent } from './project-side-nav-list.component';
import { By } from '@angular/platform-browser';
import { provideRouter } from '@angular/router';
import { Component } from '@angular/core';

@Component({
  selector: 'test-entry',
  template: '<h1>Entry Page</h1>',
})
class TestEntry {}

describe('ProjectSideNavListComponent', () => {
  let fixture: ComponentFixture<ProjectSideNavListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ProjectSideNavListComponent],
      providers: [
        provideRouter(
          ['model', 'analysis', 'settings', 'plugins'].map((path) => ({
            path,
            component: TestEntry,
          }))
        ),
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(ProjectSideNavListComponent);

    fixture.componentRef.setInput('projectId', 'project-id');
    fixture.componentRef.setInput('isCollapsed', false);

    await fixture.whenStable();
  });

  it('should render entries', () => {
    const icons = fixture.debugElement.queryAll(By.css('mat-icon'));
    expect(icons).toHaveLength(5);

    const names = fixture.debugElement.queryAll(By.css('span[matListItemTitle]'));
    expect(names).toHaveLength(4);
    expect(names[0].nativeElement.textContent).toContain('Model');
    expect(names[1].nativeElement.textContent).toContain('Analysis');
    expect(names[2].nativeElement.textContent).toContain('Settings');
    expect(names[3].nativeElement.textContent).toContain('Plugins');
  });
});
