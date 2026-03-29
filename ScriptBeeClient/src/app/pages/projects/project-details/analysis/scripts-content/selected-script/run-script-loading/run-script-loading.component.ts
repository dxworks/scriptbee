import { Component, computed, inject, input } from '@angular/core';
import { AnalysisService } from '../../../../../../../services/analysis/analysis.service';
import { rxResource } from '@angular/core/rxjs-interop';
import { interval, of, startWith, switchMap, takeWhile } from 'rxjs';
import { MatProgressSpinner } from '@angular/material/progress-spinner';

@Component({
  selector: 'app-run-script-loading',
  imports: [MatProgressSpinner],
  templateUrl: './run-script-loading.component.html',
  styleUrl: './run-script-loading.component.scss',
})
export class RunScriptLoadingComponent {
  statusUrl = input.required<string | undefined>();

  private analysisService = inject(AnalysisService);

  statusResource = rxResource({
    params: () => ({ statusUrl: this.statusUrl() }),
    stream: ({ params }) => {
      const statusUrl = params.statusUrl;
      if (!statusUrl) {
        return of(undefined);
      }

      return interval(1000).pipe(
        startWith(0),
        switchMap(() => this.analysisService.getAnalysisStatus(statusUrl)),
        takeWhile((status) => status.status === 'Started' || status.status === 'Running', true)
      );
    },
  });

  status = computed(() => this.statusResource.value());

  finished = computed(() => {
    const currentStatus = this.status();
    return currentStatus?.status === 'Finished' || currentStatus?.status === 'Cancelled';
  });
}
