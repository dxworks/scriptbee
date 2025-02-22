import { Component, input, signal } from '@angular/core';
import { CenteredSpinnerComponent } from '../../../../../components/centered-spinner/centered-spinner.component';
import { ErrorStateComponent } from '../../../../../components/error-state/error-state.component';
import { MatFormField, MatLabel } from '@angular/material/form-field';
import { MatOption } from '@angular/material/core';
import { MatSelect } from '@angular/material/select';
import { createRxResourceHandler } from '../../../../../utils/resource';
import { LinkerService } from '../../../../../services/linkers/linker.service';
import { MatButton } from '@angular/material/button';
import { apiHandler } from '../../../../../utils/apiHandler';

@Component({
  selector: 'app-link-models',
  templateUrl: './link-models.component.html',
  styleUrls: ['./link-models.component.scss'],
  imports: [CenteredSpinnerComponent, ErrorStateComponent, MatFormField, MatLabel, MatOption, MatSelect, MatButton],
})
export class LinkModelsComponent {
  projectId = input.required<string>();

  selectedLinkerId = signal<string | undefined>(undefined);

  getLinkersResource = createRxResourceHandler({
    loader: () => this.linkerService.getAllLinkers(),
  });

  linkModelsHandler = apiHandler(
    (params: { projectId: string; linkerId: string }) => this.linkerService.linkModels(params.projectId, params.linkerId),
    (data) => {
      console.log(data);
    }
  );

  constructor(private linkerService: LinkerService) {}

  onLinkButtonClick() {
    const linkerId = this.selectedLinkerId();
    if (!linkerId) {
      return;
    }

    this.linkModelsHandler.execute({ projectId: this.projectId(), linkerId: linkerId });
  }
}
