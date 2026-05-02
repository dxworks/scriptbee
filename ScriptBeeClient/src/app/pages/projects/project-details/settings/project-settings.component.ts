import { Component, computed, inject, model, signal } from '@angular/core';
import { ProjectStateService } from '../../../../services/projects/project-state.service';
import { DatePipe } from '@angular/common';
import { MatDivider, MatList, MatListItem, MatListItemLine, MatListItemTitle } from '@angular/material/list';
import { LoadingProgressBarComponent } from '../../../../components/loading-progress-bar/loading-progress-bar.component';
import { MatButton } from '@angular/material/button';
import { MatCard, MatCardActions, MatCardContent, MatCardHeader, MatCardSubtitle, MatCardTitle } from '@angular/material/card';
import { ProjectService } from '../../../../services/projects/project.service';
import { Router } from '@angular/router';
import { DeleteProjectDialogComponent } from './delete-project-dialog/delete-project-dialog.component';
import { finalize } from 'rxjs';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { FormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { UserFolderPathService } from '../../../../services/common/user-folder-path.service';
import { HttpErrorResponse } from '@angular/common/http';
import { convertError } from '../../../../utils/api';

@Component({
  selector: 'app-project-settings',
  imports: [
    DatePipe,
    MatList,
    MatListItem,
    MatListItemLine,
    MatListItemTitle,
    LoadingProgressBarComponent,
    MatButton,
    MatCard,
    MatCardActions,
    MatCardContent,
    MatCardHeader,
    MatCardSubtitle,
    MatCardTitle,
    FormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatDivider,
  ],
  templateUrl: './project-settings.component.html',
  styleUrl: './project-settings.component.scss',
})
export class ProjectSettingsPage {
  project = computed(() => this.projectStateService.currentProject()!);
  userFolderPath = computed(() => UserFolderPathService.getUserFolderPath(this.project().id));

  userFolderRoot = model<string>();
  isDeleteLoading = signal(false);

  private projectStateService = inject(ProjectStateService);
  private projectService = inject(ProjectService);
  readonly dialog = inject(MatDialog);
  private router = inject(Router);
  private snackbar = inject(MatSnackBar);

  openDialog(): void {
    const dialogRef = this.dialog.open(DeleteProjectDialogComponent);

    dialogRef.afterClosed().subscribe((result) => {
      if (result) {
        this.isDeleteLoading.set(true);
        this.projectService
          .deleteProject(this.project().id)
          .pipe(finalize(() => this.isDeleteLoading.set(false)))
          .subscribe({
            next: () => {
              UserFolderPathService.removeUserFolderPath(this.project().id);
              void this.router.navigate(['/projects']);
            },
            error: (errorResponse: HttpErrorResponse) => {
              const error = convertError(errorResponse);
              this.snackbar.open(`Could not delete project ${this.project().name} because ${error?.detail}`, 'Dismiss', { duration: 4000 });
            },
          });
      }
    });
  }

  onUserFolderPathChange(event: Event) {
    const value = (event.target as HTMLInputElement).value ?? '';
    UserFolderPathService.setUserFolderPath(this.project().id, value);
  }
}
