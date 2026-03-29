import { Component, input, output } from '@angular/core';
import { NoScriptsComponent } from './no-scripts/no-scripts.component';
import { SelectedScriptComponent } from './selected-script/selected-script.component';

@Component({
  selector: 'app-scripts-content',
  templateUrl: './scripts-content.component.html',
  styleUrls: ['./scripts-content.component.scss'],
  imports: [NoScriptsComponent, SelectedScriptComponent],
})
export class ScriptsContentComponent {
  projectId = input.required<string>();
  instanceId = input.required<string>();
  fileId = input<string | null>(null);

  analysisFinished = output<string>();

  onAnalysisFinished(analysisId: string) {
    this.analysisFinished.emit(analysisId);
  }
}
