import { Component, computed, inject, model, signal } from '@angular/core';
import { ProjectStateService } from '../../../../services/projects/project-state.service';
import { DatePipe } from '@angular/common';
import { MatList, MatListItem, MatListItemLine, MatListItemTitle } from '@angular/material/list';
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
  ],
  templateUrl: './project-settings.component.html',
  styleUrl: './project-settings.component.scss',
})
export class ProjectSettingsPage {
  private projectStateService = inject(ProjectStateService);

  project = computed(() => this.projectStateService.currentProject()!);

  userFolderRoot = model<string>('');

  isDeleteLoading = signal(false);

  readonly dialog = inject(MatDialog);
  private projectService = inject(ProjectService);
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
            next: () => this.router.navigate(['/projects']),
            error: () => {
              this.snackbar.open(`Could not delete project ${this.project().name}`, 'Dismiss', { duration: 4000 });
            },
          });
      }
    });
  }
}
