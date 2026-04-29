import { Component, computed, effect, inject, input, output, signal } from '@angular/core';
import { ProjectLiveUpdatesService } from '../../../../../../../services/projects/project-live-updates.service';
import { MatProgressBar } from '@angular/material/progress-bar';
import { MatChipsModule } from '@angular/material/chips';

@Component({
  selector: 'app-run-script-loading',
  imports: [MatProgressBar, MatChipsModule],
  templateUrl: './run-script-loading.component.html',
  styleUrl: './run-script-loading.component.scss',
})
export class RunScriptLoadingComponent {
  statusUrl = input.required<string | undefined>();

  analysisFinished = output<string>();

  private liveUpdatesService = inject(ProjectLiveUpdatesService);

  private currentStatus = signal<string | undefined>(undefined);
  private analysisId = signal<string | undefined>(undefined);

  status = computed(() => this.currentStatus());

  finished = computed(() => {
    const s = this.status();
    return s === 'Finished' || s === 'Cancelled';
  });

  constructor() {
    effect(() => {
      const url = this.statusUrl();
      if (url) {
        const parts = url.split('/');
        const id = parts[parts.length - 1];
        this.analysisId.set(id);
        this.currentStatus.set('Started');
      } else {
        this.analysisId.set(undefined);
        this.currentStatus.set(undefined);
      }
    });

    effect((onCleanup) => {
      const id = this.analysisId();
      if (!id) {
        return;
      }

      const sub = this.liveUpdatesService.analysisStatusChanged$.subscribe((event) => {
        if (event.analysisId === id) {
          this.currentStatus.set(event.status);
          if (event.status === 'Finished') {
            this.analysisFinished.emit(id);
          }
        }
      });

      onCleanup(() => {
        sub.unsubscribe();
      });
    });
  }
}
