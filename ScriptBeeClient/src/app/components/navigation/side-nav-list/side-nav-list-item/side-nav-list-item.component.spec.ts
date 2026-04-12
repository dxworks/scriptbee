import { beforeEach, describe, expect, it } from 'vitest';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { SideNavListItemComponent } from './side-nav-list-item.component';
import { provideRouter } from '@angular/router';

describe('SideNavListItemComponent', () => {
  let fixture: ComponentFixture<SideNavListItemComponent>;

  const NAV_ITEM = {
    name: 'Parent',
    link: '/parent',
    icon: 'home',
  };

  const NAV_ITEM_WITH_CHILDREN = {
    name: 'Parent',
    link: '/parent',
    icon: 'home',
    children: [
      { name: 'Child 1', link: '/child-1', icon: 'star' },
      { name: 'Child 2', link: '/child-2', icon: 'info' },
    ],
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SideNavListItemComponent],
      providers: [provideRouter([])],
    }).compileComponents();

    fixture = TestBed.createComponent(SideNavListItemComponent);
  });

  const clickMainItem = () => {
    const main = fixture.nativeElement.querySelector('a.nav-list-item');
    main.click();
    fixture.detectChanges();
  };

  const getAllAnchors = () => Array.from(fixture.nativeElement.querySelectorAll('a.nav-list-item')) as HTMLElement[];

  const getChildAnchors = () => getAllAnchors().slice(1);

  it('renders icon and name when not collapsed', () => {
    fixture.componentRef.setInput('navItem', NAV_ITEM);
    fixture.componentRef.setInput('isCollapsed', false);
    fixture.detectChanges();

    const titles = fixture.nativeElement.querySelectorAll('[matListItemTitle]');

    expect(titles.length).toBe(1);
    expect(titles[0].textContent).toContain('Parent');
  });

  it('hides name when collapsed', () => {
    fixture.componentRef.setInput('navItem', NAV_ITEM);
    fixture.componentRef.setInput('isCollapsed', true);
    fixture.detectChanges();

    const titles = fixture.nativeElement.querySelectorAll('[matListItemTitle]');

    expect(titles.length).toBe(0);
  });

  it('shows expand icon when item has children and not collapsed', () => {
    fixture.componentRef.setInput('navItem', NAV_ITEM_WITH_CHILDREN);
    fixture.componentRef.setInput('isCollapsed', false);
    fixture.detectChanges();

    const expandIcon = Array.from(fixture.nativeElement.querySelectorAll('mat-icon')).find((i: any) => i.textContent.trim() === 'expand_more');

    expect(expandIcon).toBeTruthy();
  });

  it('does not show expand icon when collapsed', () => {
    fixture.componentRef.setInput('navItem', NAV_ITEM_WITH_CHILDREN);
    fixture.componentRef.setInput('isCollapsed', true);
    fixture.detectChanges();

    const expandIcon = Array.from(fixture.nativeElement.querySelectorAll('mat-icon')).find((i: any) =>
      ['expand_more', 'expand_less'].includes(i.textContent.trim())
    );

    expect(expandIcon).toBeFalsy();
  });

  it('opens nested menu when clicked', () => {
    fixture.componentRef.setInput('navItem', NAV_ITEM_WITH_CHILDREN);
    fixture.componentRef.setInput('isCollapsed', false);
    fixture.detectChanges();

    clickMainItem();

    const children = getChildAnchors();

    expect(children.length).toBe(2);
    expect(children[0].textContent).toContain('Child 1');
    expect(children[1].textContent).toContain('Child 2');
  });

  it('closes nested menu when clicked twice', () => {
    fixture.componentRef.setInput('navItem', NAV_ITEM_WITH_CHILDREN);
    fixture.componentRef.setInput('isCollapsed', false);
    fixture.detectChanges();

    clickMainItem();
    clickMainItem();

    const children = getChildAnchors();

    expect(children.length).toBe(0);
  });

  it('applies padding to children when not collapsed', () => {
    fixture.componentRef.setInput('navItem', NAV_ITEM_WITH_CHILDREN);
    fixture.componentRef.setInput('isCollapsed', false);
    fixture.detectChanges();

    clickMainItem();

    const child = getChildAnchors()[0];

    expect(child.style.paddingLeft).toBe('48px');
  });

  it('does not apply padding when collapsed', () => {
    fixture.componentRef.setInput('navItem', NAV_ITEM_WITH_CHILDREN);
    fixture.componentRef.setInput('isCollapsed', true);
    fixture.detectChanges();

    clickMainItem();

    const child = getChildAnchors()[0];

    expect(child.style.paddingLeft).toBe('');
  });

  it('uses navItem link when no prefix provided', () => {
    fixture.componentRef.setInput('navItem', NAV_ITEM);
    fixture.detectChanges();

    const main = getAllAnchors()[0];

    expect(main.getAttribute('href') ?? '').toContain('/parent');
  });

  it('builds routerLink with prefix when provided', () => {
    fixture.componentRef.setInput('navItem', NAV_ITEM);
    fixture.componentRef.setInput('linkPrefix', '/admin');
    fixture.detectChanges();

    const main = getAllAnchors()[0];

    expect(main.getAttribute('href') ?? '').toContain('/admin/parent');
  });

  it('builds correct child routerLink including prefix', () => {
    fixture.componentRef.setInput('navItem', NAV_ITEM_WITH_CHILDREN);
    fixture.componentRef.setInput('linkPrefix', '/admin');
    fixture.componentRef.setInput('isCollapsed', false);
    fixture.detectChanges();

    clickMainItem();

    const children = getChildAnchors();

    expect(children[0].getAttribute('href') ?? '').toContain('/admin/parent/child-1');

    expect(children[1].getAttribute('href') ?? '').toContain('/admin/parent/child-2');
  });
});
