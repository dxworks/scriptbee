import { Injectable } from '@angular/core';
import { ComponentStore } from '@ngrx/component-store';
import { ApiErrorMessage } from '../../shared/api-error-message';
import { ProjectService } from '../services/project.service';
import { ProjectData } from '../../state/project-details/project';
import { catchError, EMPTY, pipe, switchMap, tap } from 'rxjs';
import { HttpErrorResponse } from '@angular/common/http';

interface ProjectStoreState {
  projectId: string;

  projectData: ProjectData | undefined;
  projectDataError: ApiErrorMessage | undefined;
  projectDataLoading: boolean;
}

@Injectable()
export class ProjectStore extends ComponentStore<ProjectStoreState> {
  constructor(private projectService: ProjectService) {
    super({
      projectId: '',
      projectData: undefined,
      projectDataError: undefined,
      projectDataLoading: false,
    });
  }

  getProjectId() {
    return this.get().projectId;
  }

  readonly setProjectId = this.updater((state, projectId: string) => ({ ...state, projectId }));

  readonly projectData = this.select((state) => state.projectData);
  readonly projectDataError = this.select((state) => state.projectDataError);
  readonly projectDataLoading = this.select((state) => state.projectDataLoading);

  loadProjectData = this.effect<void>(
    pipe(
      switchMap(() => {
        this.patchState({ projectData: undefined, projectDataError: undefined, projectDataLoading: true });
        return this.projectService.getProject(this.getProjectId()).pipe(
          tap({
            next: (projectData) => {
              this.patchState({ projectData, projectDataLoading: false });
            },
            error: (error: HttpErrorResponse) => {
              this.patchState({
                projectDataError: { code: error.status, message: error.message },
                projectDataLoading: false,
              });
            },
          }),
          catchError(() => EMPTY)
        );
      })
    )
  );
}
