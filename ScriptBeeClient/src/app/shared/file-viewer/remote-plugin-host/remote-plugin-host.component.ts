import { Component, input, OnInit, signal, Type } from '@angular/core';
import { NgComponentOutlet } from '@angular/common';
import { loadRemoteModule } from '@angular-architects/native-federation';
import { AnalysisFile } from '../../../types/analysis-results';
import { LoadingProgressBarComponent } from '../../../components/loading-progress-bar/loading-progress-bar.component';
import { ErrorStateComponent } from '../../../components/error-state/error-state.component';
import { FilePreviewExtensionPointOutlet } from '../../../types/plugin';
import { RemoteMeta } from '../../../services/plugin/gateway-plugins.service';

@Component({
  selector: 'app-remote-plugin-host',
  standalone: true,
  imports: [NgComponentOutlet, LoadingProgressBarComponent, ErrorStateComponent],
  templateUrl: './remote-plugin-host.component.html',
  styleUrls: ['./remote-plugin-host.component.scss'],
})
export class RemotePluginHostComponent implements OnInit {
  content = input<string | undefined>();
  file = input<AnalysisFile | undefined>();
  pluginOutlet = input.required<FilePreviewExtensionPointOutlet & RemoteMeta>();

  protected isLoading = signal(true);
  protected error = signal<string | null>(null);
  protected loadedComponent = signal<Type<unknown> | null>(null);

  async ngOnInit() {
    const outlet = this.pluginOutlet();

    this.isLoading.set(true);
    try {
      const loadedComponent = await loadRemoteModule(outlet.remoteName, outlet.exposedModule).then((m) => m[outlet.componentName ?? 'App']);

      this.loadedComponent.set(loadedComponent);
    } catch (err) {
      console.log('Failed to load remote module', err);
      this.error.set('Failed to load Plugin');
    } finally {
      this.isLoading.set(false);
    }
  }
}
