import { Component, computed, input, signal } from '@angular/core';
import { CenteredSpinnerComponent } from '../../../../../components/centered-spinner/centered-spinner.component';
import { ErrorStateComponent } from '../../../../../components/error-state/error-state.component';
import { MatFormField, MatLabel } from '@angular/material/form-field';
import { MatOption } from '@angular/material/core';
import { MatSelect } from '@angular/material/select';
import { rxResource } from '@angular/core/rxjs-interop';
import { LinkerService } from '../../../../../services/linkers/linker.service';
import { MatButton } from '@angular/material/button';
import { finalize } from 'rxjs';
import { convertError } from '../../../../../utils/api';

@Component({
  selector: 'app-link-models',
  templateUrl: './link-models.component.html',
  styleUrls: ['./link-models.component.scss'],
  imports: [CenteredSpinnerComponent, ErrorStateComponent, MatFormField, MatLabel, MatOption, MatSelect, MatButton],
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

  constructor(private linkerService: LinkerService) {}

  onLinkButtonClick() {
    const linkerId = this.selectedLinkerId();
    if (!linkerId) {
      return;
    }

    this.isLinkLoading.set(true);
    this.linkerService
      .linkModels(this.projectId(), this.instanceId(), linkerId)
      .pipe(finalize(() => this.isLinkLoading.set(false)))
      .subscribe();
  }
}
