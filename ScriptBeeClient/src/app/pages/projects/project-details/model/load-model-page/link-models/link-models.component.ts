import { Component, computed, inject, input, signal } from '@angular/core';
import { ErrorStateComponent } from '../../../../../../components/error-state/error-state.component';
import { MatFormField, MatLabel } from '@angular/material/form-field';
import { MatOption } from '@angular/material/core';
import { MatSelect } from '@angular/material/select';
import { rxResource } from '@angular/core/rxjs-interop';
import { LinkerService } from '../../../../../../services/instances/linker.service';
import { MatButton } from '@angular/material/button';
import { finalize } from 'rxjs';
import { convertError } from '../../../../../../utils/api';
import { LoadingProgressBarComponent } from '../../../../../../components/loading-progress-bar/loading-progress-bar.component';
import { HttpErrorResponse } from '@angular/common/http';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-link-models',
  templateUrl: './link-models.component.html',
  styleUrls: ['./link-models.component.scss'],
  imports: [ErrorStateComponent, MatFormField, MatLabel, MatOption, MatSelect, MatButton, LoadingProgressBarComponent],
})
export class LinkModelsComponent {
  projectId = input.required<string>();
  instanceId = input.required<string>();

  selectedLinkerId = signal<string | undefined>(undefined);

  getLinkersResource = rxResource({
    params: () => ({
      projectId: this.projectId(),
      instanceId: this.instanceId(),
    }),
    stream: ({ params }) => this.linkerService.getAllLinkers(params.projectId, params.instanceId),
  });
  getLinkersResourceError = computed(() => convertError(this.getLinkersResource.error()));

  isLinkLoading = signal(false);

  private linkerService = inject(LinkerService);
  private snackbar = inject(MatSnackBar);

  onLinkButtonClick() {
    const linkerId = this.selectedLinkerId();
    if (!linkerId) {
      return;
    }

    this.isLinkLoading.set(true);
    this.linkerService
      .linkModels(this.projectId(), this.instanceId(), linkerId)
      .pipe(finalize(() => this.isLinkLoading.set(false)))
      .subscribe({
        next: () => {
          this.snackbar.open('Link successful', 'Dismiss', { duration: 4000 });
        },
        error: (errorResponse: HttpErrorResponse) => {
          const error = convertError(errorResponse);
          this.snackbar.open(`Could not link models because ${error?.title}`, 'Dismiss', { duration: 4000 });
        },
      });
  }
}
