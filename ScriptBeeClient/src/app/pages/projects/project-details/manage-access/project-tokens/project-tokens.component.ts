import { Component, effect, inject, input, signal } from '@angular/core';
import { ProjectService } from '../../../../../services/projects/project.service';
import { CreateTokenResponse, ProjectToken, RoleInfo } from '../../../../../types/project';
import { rxResource } from '@angular/core/rxjs-interop';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatCardModule } from '@angular/material/card';
import { MatDividerModule } from '@angular/material/divider';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatChipsModule } from '@angular/material/chips';
import { LoadingProgressBarComponent } from '../../../../../components/loading-progress-bar/loading-progress-bar.component';
import { ErrorStateComponent } from '../../../../../components/error-state/error-state.component';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-project-tokens',
  templateUrl: './project-tokens.component.html',
  styleUrl: './project-tokens.component.scss',
  imports: [
    ReactiveFormsModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatCardModule,
    MatDividerModule,
    MatProgressSpinnerModule,
    MatTooltipModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatChipsModule,
    LoadingProgressBarComponent,
    ErrorStateComponent,
    DatePipe,
  ],
})
export class ProjectTokensComponent {
  readonly displayedColumns = ['description', 'role', 'createdAt', 'expiresAt', 'actions'];

  projectId = input.required<string>();

  private projectService = inject(ProjectService);
  private snackbar = inject(MatSnackBar);

  tokensResource = rxResource({
    params: () => this.projectId(),
    stream: ({ params: id }) => this.projectService.getProjectTokens(id),
  });

  rolesResource = rxResource({
    stream: () => this.projectService.getRoles(),
  });

  createTokenForm = new FormGroup({
    description: new FormControl<string>('', { nonNullable: true }),
    role: new FormControl<RoleInfo | null>(null, { validators: [Validators.required] }),
    expiresAt: new FormControl<Date | null>(null, { validators: [Validators.required] }),
  });

  minDate = new Date();

  isSaving = signal(false);
  deletingTokenId = signal<string | null>(null);
  newlyCreatedToken = signal<CreateTokenResponse | null>(null);

  constructor() {
    effect(() => {
      if (this.rolesResource.isLoading()) {
        this.createTokenForm.controls.role.disable();
      } else {
        this.createTokenForm.controls.role.enable();
      }
    });
  }

  compareRoles(a: RoleInfo | null, b: RoleInfo | null): boolean {
    return a?.id === b?.id;
  }

  onCreateToken() {
    if (this.createTokenForm.invalid) {
      return;
    }

    const { description, role, expiresAt } = this.createTokenForm.getRawValue();
    this.isSaving.set(true);

    this.projectService
      .createProjectToken(this.projectId(), {
        description: description || null,
        role: role!.id,
        expiresAt: expiresAt!.toISOString(),
      })
      .subscribe({
        next: (created) => {
          this.newlyCreatedToken.set(created);
          this.tokensResource.reload();
          this.createTokenForm.reset({ description: '', role: null, expiresAt: null });
          this.snackbar.open('Token created.', 'Dismiss', { duration: 3000 });
        },
        error: () => {
          this.snackbar.open('Failed to create token.', 'Dismiss', { duration: 4000 });
        },
        complete: () => {
          this.isSaving.set(false);
        },
      });
  }

  onDeleteToken(token: ProjectToken) {
    this.deletingTokenId.set(token.id);

    this.projectService.deleteProjectToken(this.projectId(), token.id).subscribe({
      next: () => {
        this.tokensResource.reload();
        this.snackbar.open('Token deleted.', 'Dismiss', { duration: 3000 });
      },
      error: () => {
        this.snackbar.open('Failed to delete token.', 'Dismiss', { duration: 4000 });
      },
      complete: () => {
        this.deletingTokenId.set(null);
      },
    });
  }

  onCopyToken(tokenValue: string) {
    navigator.clipboard.writeText(tokenValue).then(() => {
      this.snackbar.open('Token copied to clipboard.', 'Dismiss', { duration: 2000 });
    });
  }

  onDismissNewToken() {
    this.newlyCreatedToken.set(null);
  }

  isExpired(expiresAt: string): boolean {
    return new Date(expiresAt) < new Date();
  }
}
