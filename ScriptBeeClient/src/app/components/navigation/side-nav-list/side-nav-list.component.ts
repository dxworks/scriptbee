import { Component, input } from '@angular/core';
import { MatIcon } from '@angular/material/icon';
import { MatListItem, MatListItemTitle, MatNavList } from '@angular/material/list';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { NavItem } from '../navItem';

@Component({
  selector: 'app-side-nav-list',
  imports: [MatIcon, MatListItem, MatListItemTitle, MatNavList, RouterLinkActive, RouterLink],
  templateUrl: './side-nav-list.component.html',
  styleUrl: './side-nav-list.component.scss',
})
export class SideNavListComponent {
  navItems = input.required<NavItem[]>();
  linkPrefix = input<string | undefined>(undefined);
  isCollapsed = input<boolean>(false);

  getUrl(navItem: NavItem): string {
    if (this.linkPrefix()) {
      return this.linkPrefix() + '/' + navItem.link;
    }
    return navItem.link;
  }
}
