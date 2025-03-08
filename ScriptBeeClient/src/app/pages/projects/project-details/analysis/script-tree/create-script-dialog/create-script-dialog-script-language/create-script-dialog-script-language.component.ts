import { Component, input, model } from '@angular/core';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatFormField } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { FormsModule } from '@angular/forms';
import { createRxResourceHandler } from '../../../../../../../utils/resource';
import { ProjectStructureService } from '../../../../../../../services/projects/project-structure.service';
import { ErrorStateComponent } from '../../../../../../../components/error-state/error-state.component';
import { LoadingProgressBarComponent } from '../../../../../../../components/loading-progress-bar/loading-progress-bar.component';

@Component({
  selector: 'app-create-script-dialog-script-language',
  templateUrl: './create-script-dialog-script-language.component.html',
  styleUrls: ['./create-script-dialog-script-language.component.scss'],
  imports: [MatExpansionModule, MatFormField, MatSelectModule, MatButtonModule, FormsModule, ErrorStateComponent, LoadingProgressBarComponent],
})
export class CreateScriptDialogScriptLanguageComponent {
  projectId = input.required<string>();
  scriptLanguage = model.required<string>();

  availableScriptLanguagesResource = createRxResourceHandler({
    request: () => this.projectId(),
    loader: (params) => this.projectStructureService.getAvailableScriptTypes(params.request),
  });

  constructor(private projectStructureService: ProjectStructureService) {}
}
