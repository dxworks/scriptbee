import { Component, input } from '@angular/core';
import { AnalysisRunError } from '../../../../../../types/analysis-results';

@Component({
  selector: 'app-output-errors',
  templateUrl: './output-errors.component.html',
  styleUrls: ['./output-errors.component.scss'],
})
export class OutputErrorsComponent {
  errors = input.required<AnalysisRunError[]>();
}
