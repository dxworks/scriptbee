import { Component, input } from '@angular/core';
import { MatNavList } from '@angular/material/list';
import { NavItem } from '../navItem';
import { SideNavListItemComponent } from './side-nav-list-item/side-nav-list-item.component';

@Component({
  selector: 'app-side-nav-list',
  imports: [MatNavList, SideNavListItemComponent],
  templateUrl: './side-nav-list.component.html',
  styleUrl: './side-nav-list.component.scss',
})
export class SideNavListComponent {
  navItems = input.required<NavItem[]>();
  linkPrefix = input<string | undefined>(undefined);
  isCollapsed = input<boolean>(false);
}
