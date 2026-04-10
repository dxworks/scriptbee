import { beforeEach, describe, expect, it } from 'vitest';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { SideNavListComponent } from './side-nav-list.component';
import { By } from '@angular/platform-browser';
import { provideRouter } from '@angular/router';
import { Component } from '@angular/core';

@Component({
  selector: 'test-entry-1',
  template: '<h1>Entry 1 Page</h1>',
})
class Entry1 {}

@Component({
  selector: 'test-entry-2',
  template: '<h1>Entry 2 Page</h1>',
})
class Entry2 {}

describe('SideNavListComponent', () => {
  let fixture: ComponentFixture<SideNavListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SideNavListComponent],
      providers: [
        provideRouter([
          {
            path: 'entry-1',
            component: Entry1,
          },
          {
            path: 'entry-2',
            component: Entry2,
          },
        ]),
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(SideNavListComponent);

    fixture.componentRef.setInput('navItems', []);

    await fixture.whenStable();
  });

  it('should render text when not collapsed', () => {
    fixture.componentRef.setInput('navItems', [
      {
        link: '/entry-1',
        name: 'Entry 1',
        icon: 'model_training',
      },
      {
        link: '/entry-2',
        name: 'Entry 2',
        icon: 'query_stats',
      },
    ]);
    fixture.componentRef.setInput('isCollapsed', false);
    fixture.detectChanges();

    const icons = fixture.debugElement.queryAll(By.css('mat-icon'));
    expect(icons).toHaveLength(2);

    const names = fixture.debugElement.queryAll(By.css('span[matListItemTitle]'));
    expect(names).toHaveLength(2);
    expect(names[0].nativeElement.textContent).toContain('Entry 1');
    expect(names[1].nativeElement.textContent).toContain('Entry 2');
  });

  it('should not render text when collapsed', () => {
    fixture.componentRef.setInput('navItems', [
      {
        link: '/entry-1',
        name: 'Entry 1',
        icon: 'model_training',
      },
      {
        link: '/entry-2',
        name: 'Entry 2',
        icon: 'query_stats',
      },
    ]);
    fixture.componentRef.setInput('isCollapsed', true);
    fixture.detectChanges();

    const icons = fixture.debugElement.queryAll(By.css('mat-icon'));
    expect(icons).toHaveLength(2);

    const names = fixture.debugElement.queryAll(By.css('span[matListItemTitle]'));
    expect(names).toHaveLength(0);
  });
});
