import { Component, inject, signal, WritableSignal } from '@angular/core';
import { MatError, MatFormField, MatLabel } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { FormControl, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { SlugifyPipe } from '../../../pipes/slugify.pipe';
import { MatButtonModule } from '@angular/material/button';
import { LoadingProgressBarComponent } from '../../../components/loading-progress-bar/loading-progress-bar.component';
import { ProjectService } from '../../../services/projects/project.service';
import { Router } from '@angular/router';
import { finalize } from 'rxjs';
import { HttpErrorResponse } from '@angular/common/http';
import { convertError } from '../../../utils/api';
import { MatSnackBar } from '@angular/material/snack-bar';
import { UserFolderPathService } from '../../../services/common/user-folder-path.service';

@Component({
  selector: 'app-create-project',
  imports: [FormsModule, ReactiveFormsModule, MatFormField, MatInputModule, MatLabel, MatError, SlugifyPipe, MatButtonModule, LoadingProgressBarComponent],
  templateUrl: './create-project.component.html',
  styleUrl: './create-project.component.scss',
})
export class CreateProjectPage {
  readonly projectId = new FormControl('', [Validators.required]);
  readonly projectName = new FormControl('', [Validators.required]);
  readonly userFolderPathControl = new FormControl(UserFolderPathService.getPreviousCreateProjectPath());

  projectIdErrorMessage = signal('');
  projectNameErrorMessage = signal('');
  isCreateLoading = signal(false);

  private projectService = inject(ProjectService);
  private router = inject(Router);
  private snackbar = inject(MatSnackBar);

  updateProjectIdErrorMessage() {
    this.updateErrorMessage(this.projectId, this.projectIdErrorMessage);
  }

  updateProjectNameErrorMessage() {
    this.updateErrorMessage(this.projectName, this.projectNameErrorMessage);
  }

  onCreate() {
    if (this.projectId.value && this.projectName.value) {
      this.isCreateLoading.set(true);
      this.projectService
        .createProject(this.projectId.value, this.projectName.value)
        .pipe(finalize(() => this.isCreateLoading.set(false)))
        .subscribe({
          next: (response) => {
            const userFolderPath = this.userFolderPathControl.value;
            if (userFolderPath) {
              UserFolderPathService.setUserFolderPath(response.id, userFolderPath);
              UserFolderPathService.setPreviousCreateProjectPath(userFolderPath);
            }

            void this.router.navigate([`/projects/${response.id}`]);
          },
          error: (errorResponse: HttpErrorResponse) => {
            const error = convertError(errorResponse);
            this.snackbar.open(`Could not create project because ${error?.detail}`, 'Dismiss', { duration: 4000 });
          },
        });
    }
  }

  private updateErrorMessage(control: FormControl<string | null>, errorMessage: WritableSignal<string>) {
    if (control.hasError('required')) {
      errorMessage.set('You must enter a value');
    } else {
      errorMessage.set('');
    }
  }
}
