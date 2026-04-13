import { Component, computed, input } from '@angular/core';
import { SideNavListComponent } from '../side-nav-list/side-nav-list.component';
import { NavItem } from '../navItem';

@Component({
  selector: 'app-project-side-nav-list',
  imports: [SideNavListComponent],
  templateUrl: './project-side-nav-list.component.html',
  styleUrl: './project-side-nav-list.component.scss',
})
export class ProjectSideNavListComponent {
  isCollapsed = input.required<boolean>();
  projectId = input.required<string>();

  linkPrefix = computed(() => {
    return `/projects/${this.projectId()}`;
  });

  navItems: NavItem[] = [
    {
      link: '/model',
      name: 'Model',
      icon: 'model_training',
      children: [
        {
          link: '/upload',
          name: 'Upload',
          icon: 'upload',
        },
        {
          link: '/load',
          name: 'Load',
          icon: 'autorenew',
        },
        {
          link: '/context',
          name: 'Context',
          icon: 'account_tree',
        },
      ],
    },
    {
      link: '/analysis',
      name: 'Analysis',
      icon: 'query_stats',
    },
    {
      link: '/settings',
      name: 'Settings',
      icon: 'settings',
    },
    {
      link: '/plugins',
      name: 'Plugins',
      icon: 'extension',
    },
  ];
}
