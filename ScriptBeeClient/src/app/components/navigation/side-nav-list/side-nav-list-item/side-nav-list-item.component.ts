import { Component, computed, effect, inject, input, signal } from '@angular/core';
import { NavItem } from '../../navItem';
import { MatIcon } from '@angular/material/icon';
import { MatListItem, MatListItemMeta, MatListItemTitle } from '@angular/material/list';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';

@Component({
  selector: 'app-side-nav-list-item',
  imports: [MatIcon, MatListItem, MatListItemTitle, RouterLinkActive, RouterLink, MatListItemMeta],
  templateUrl: './side-nav-list-item.component.html',
  styleUrl: './side-nav-list-item.component.scss',
})
export class SideNavListItemComponent {
  navItem = input.required<NavItem>();
  linkPrefix = input<string | undefined>(undefined);
  isCollapsed = input<boolean>(false);

  router = inject(Router);

  isNestedMenuOpen = signal<boolean>(false);

  constructor() {
    effect(() => {
      this.isNestedMenuOpen.set(this.isRouteFromNestedRoute());
    });
  }

  nestedMenuPadding = computed(() => {
    return this.isCollapsed() ? null : '48px';
  });

  toggleNestedMenu() {
    this.isNestedMenuOpen.update((value) => !value);
  }

  getUrl(): string {
    const prefix = this.linkPrefix();
    if (prefix) {
      return concatenatePaths(prefix, this.navItem().link);
    }
    return this.navItem().link;
  }

  getChildUrl(navItem: NavItem): string {
    return concatenatePaths(this.getUrl(), navItem.link);
  }

  private isRouteFromNestedRoute(): boolean {
    const children = this.navItem().children ?? [];

    for (const child of children) {
      const url = this.getChildUrl(child);
      if (url === this.router.url) {
        return true;
      }
    }

    return false;
  }
}

function concatenatePaths(part1: string, part2: string) {
  const cleanedPart1 = part1.replace(/\/$/, '');
  const cleanedPart2 = part2.replace(/^\//, '');

  return `${cleanedPart1}/${cleanedPart2}`;
}
