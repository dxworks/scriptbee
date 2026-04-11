import { Component, computed, inject, input, model } from '@angular/core';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { rxResource } from '@angular/core/rxjs-interop';
import { of } from 'rxjs';
import { DatePipe } from '@angular/common';
import { AnalysisService } from '../../services/analysis/analysis.service';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatTooltipModule } from '@angular/material/tooltip';
import { Router } from '@angular/router';

@Component({
  selector: 'app-analysis-selector',
  imports: [MatFormFieldModule, MatSelectModule, DatePipe, MatIconModule, MatButtonModule, MatTooltipModule],
  templateUrl: './analysis-selector.component.html',
  styleUrls: ['./analysis-selector.component.scss'],
})
export class AnalysisSelectorComponent {
  projectId = input.required<string>();
  analysisId = model<string | undefined>(undefined);

  private analysisService = inject(AnalysisService);
  private router = inject(Router);

  analysesResource = rxResource({
    params: () => ({
      projectId: this.projectId(),
    }),
    stream: ({ params }) => {
      if (params.projectId) {
        return this.analysisService.getAnalyses(params.projectId);
      }
      return of([]);
    },
  });

  analyses = computed(() => this.analysesResource.value() ?? []);

  reload() {
    this.analysesResource.reload();
  }

  openExtendedView() {
    if (this.analysisId()) {
      this.router.navigate(['/projects', this.projectId(), 'analysis', this.analysisId()]);
    }
  }
}
