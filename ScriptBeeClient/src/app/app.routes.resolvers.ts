import { ActivatedRouteSnapshot, ResolveFn } from '@angular/router';
import { Project } from './types/project';
import { inject } from '@angular/core';
import { ProjectService } from './services/projects/project.service';

export const projectResolver: ResolveFn<Project> = (route: ActivatedRouteSnapshot) => {
  const projectService = inject(ProjectService);

  const projectId = route.paramMap.get('id')!;
  return projectService.getProject(projectId);
};

export const parentProjectResolver: ResolveFn<Project> = (route: ActivatedRouteSnapshot) => {
  return route.parent?.data['project'];
};
