import { Component, signal } from '@angular/core';
import { ActivatedRoute, NavigationEnd, Router, RouterOutlet } from '@angular/router';
import { MatTabsModule } from '@angular/material/tabs';
import { filter, first } from 'rxjs';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { MatIcon } from '@angular/material/icon';

interface TabInfo {
  link: string;
  name: string;
  icon: string;
}

@Component({
  selector: 'app-project-details',
  templateUrl: './project-details-page.component.html',
  styleUrls: ['./project-details-page.component.scss'],
  providers: [],
  imports: [MatTabsModule, RouterOutlet, MatIcon],
})
export class ProjectDetailsPage {
  tabInfo: TabInfo[] = [
    {
      link: 'model',
      name: 'Model',
      icon: 'model_training',
    },
    {
      link: 'analysis',
      name: 'Analysis',
      icon: 'query_stats',
    },
    {
      link: 'settings',
      name: 'Settings',
      icon: 'settings',
    },
  ];
  activeTab = signal(this.tabInfo[0]);

  constructor(
    private route: ActivatedRoute,
    private router: Router
  ) {
    router.events
      .pipe(
        filter((e) => e instanceof NavigationEnd),
        first(),
        takeUntilDestroyed()
      )
      .subscribe((event) => {
        const urlParts = event.url.split('/');
        const urlPart = urlParts[urlParts.length - 1];
        this.activeTab.set(this.tabInfo.find((t) => t.link === urlPart) ?? this.tabInfo[0]);
      });
  }

  selectTab(tab: TabInfo) {
    this.activeTab.set(tab);
    this.router.navigate([tab.link], { relativeTo: this.route }).then();
  }
}
