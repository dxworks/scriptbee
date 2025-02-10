import { Component, signal, WritableSignal } from '@angular/core';
import { MatError, MatFormField, MatLabel } from '@angular/material/form-field';
import { MatInput } from '@angular/material/input';
import { FormControl, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { SlugifyPipe } from '../../../pipes/slugify.pipe';
import { MatButton } from '@angular/material/button';
import { LoadingProgressBarComponent } from '../../../components/loading-progress-bar/loading-progress-bar.component';
import { ProjectService } from '../../../services/projects/project.service';
import { ErrorResponse } from '../../../types/api';
import { ErrorStateComponent } from '../../../components/error-state/error-state.component';
import { HttpErrorResponse } from '@angular/common/http';
import { Router } from '@angular/router';
import { DEFAULT_ERROR_RESPONSE } from '../../../utils/api';

@Component({
  selector: 'app-create-project',
  imports: [
    FormsModule,
    ReactiveFormsModule,
    MatFormField,
    MatInput,
    MatLabel,
    MatError,
    SlugifyPipe,
    MatButton,
    LoadingProgressBarComponent,
    ErrorStateComponent,
  ],
  templateUrl: './create-project.component.html',
  styleUrl: './create-project.component.scss',
})
export class CreateProjectPage {
  readonly projectId = new FormControl('', [Validators.required]);
  readonly projectName = new FormControl('', [Validators.required]);

  projectIdErrorMessage = signal('');
  projectNameErrorMessage = signal('');

  isCreating = signal(false);
  creatingError = signal<ErrorResponse | undefined>(undefined);

  constructor(
    private projectService: ProjectService,
    private router: Router
  ) {}

  updateProjectIdErrorMessage() {
    this.updateErrorMessage(this.projectId, this.projectIdErrorMessage);
  }

  updateProjectNameErrorMessage() {
    this.updateErrorMessage(this.projectName, this.projectNameErrorMessage);
  }

  onCreate() {
    this.isCreating.set(true);
    this.creatingError.set(undefined);

    if (this.projectId.value && this.projectName.value) {
      this.projectService.createProject(this.projectId.value, this.projectName.value).subscribe({
        next: (response) => {
          this.isCreating.set(false);
          this.router.navigate([`/projects/${response.id}`]).then();
        },
        error: (error: HttpErrorResponse) => {
          this.creatingError.set(error.error ?? DEFAULT_ERROR_RESPONSE);
          this.isCreating.set(false);
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
