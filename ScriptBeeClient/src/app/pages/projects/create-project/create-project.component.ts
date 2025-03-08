import { Component, signal, WritableSignal } from '@angular/core';
import { MatError, MatFormField, MatLabel } from '@angular/material/form-field';
import { MatInput } from '@angular/material/input';
import { FormControl, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { SlugifyPipe } from '../../../pipes/slugify.pipe';
import { MatButton } from '@angular/material/button';
import { LoadingProgressBarComponent } from '../../../components/loading-progress-bar/loading-progress-bar.component';
import { ProjectService } from '../../../services/projects/project.service';
import { ErrorStateComponent } from '../../../components/error-state/error-state.component';
import { Router } from '@angular/router';
import { apiHandler } from '../../../utils/apiHandler';

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

  createProjectHandler = apiHandler(
    (params: { id: string; name: string }) => this.projectService.createProject(params.id, params.name),
    (response) => this.router.navigate([`/projects/${response.id}`])
  );

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
    if (this.projectId.value && this.projectName.value) {
      this.createProjectHandler.execute({ id: this.projectId.value, name: this.projectName.value });
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
