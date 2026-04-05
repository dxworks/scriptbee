import { inject, Injectable, signal } from '@angular/core';
import { NavigationEnd, Router } from '@angular/router';
import { filter } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class ProjectStateService {
  currentProjectId = signal<string | null>(null);
  currentInstanceId = signal<string | null>(null);

  private router = inject(Router);

  constructor() {
    this.router.events.pipe(filter((event) => event instanceof NavigationEnd)).subscribe(() => {
      const path = this.router.url;
      const match = path.match(/\/projects\/([^/]+)/);
      if (match) {
        const projectId = match[1];
        if (projectId !== 'new') {
          this.currentProjectId.set(projectId);
        } else {
          this.currentProjectId.set(null);
        }
      } else {
        this.currentProjectId.set(null);
        this.currentInstanceId.set(null);
      }
    });
  }
}
