import { Component, computed, input, signal } from '@angular/core';
import { NavItem } from '../../navItem';
import { MatIcon } from '@angular/material/icon';
import { MatListItem, MatListItemMeta, MatListItemTitle } from '@angular/material/list';
import { RouterLink, RouterLinkActive } from '@angular/router';

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

  isNestedMenuOpen = signal<boolean>(false);

  nestedMenuPadding = computed(() => {
    return this.isCollapsed() ? null : '48px';
  });

  toggleNestedMenu() {
    this.isNestedMenuOpen.update((value) => !value);
  }

  getUrl(): string {
    if (this.linkPrefix()) {
      return this.linkPrefix() + '/' + this.navItem().link;
    }
    return this.navItem().link;
  }

  getChildUrl(navItem: NavItem): string {
    return `${this.getUrl()}${navItem.link}`;
  }
}
