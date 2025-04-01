import { Component, signal } from '@angular/core';
import { ActivatedRoute, NavigationEnd, Router, RouterOutlet } from '@angular/router';
import { MatTabsModule } from '@angular/material/tabs';
import { filter, first } from 'rxjs';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { MatIcon } from '@angular/material/icon';
import { InstanceService } from '../../../services/instances/instance.service';
import { HttpErrorResponse } from '@angular/common/http';
import { isNoInstanceAllocatedForProjectError } from '../../../utils/api';
import { MatDialog } from '@angular/material/dialog';
import { InstanceNotAllocatedDialog } from '../../../components/dialogs/instance-not-allocated-dialog/instance-not-allocated-dialog.component';

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
    private router: Router,
    private instanceService: InstanceService,
    private dialog: MatDialog
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

    route.paramMap.pipe(takeUntilDestroyed()).subscribe({
      next: (paramMap) => {
        const projectId = paramMap.get('id');
        console.log(projectId);
        if (projectId) {
          this.instanceService.getCurrentInstance(projectId).subscribe({
            error: (errorResponse: HttpErrorResponse) => {
              if (isNoInstanceAllocatedForProjectError(errorResponse)) {
                this.dialog.open(InstanceNotAllocatedDialog, {
                  disableClose: true,
                });
              }
            },
          });
        }
      },
    });
  }

  selectTab(tab: TabInfo) {
    this.activeTab.set(tab);
    this.router.navigate([tab.link], { relativeTo: this.route }).then();
  }
}
