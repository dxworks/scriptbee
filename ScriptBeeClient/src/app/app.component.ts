import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { debounceTime } from 'rxjs/operators';
import { ThemeService } from './services/theme/theme.service';
import { PluginService } from "./services/plugin/plugin.service";
import { loadRemoteModule } from "@angular-architects/module-federation";

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html'
})
export class AppComponent {
  title = 'app';

  constructor(private router: Router, private themeService: ThemeService, private pluginService: PluginService) {
    this.router.events.pipe(debounceTime(10))
      .subscribe(() => {
        themeService.update();
      });

    this.pluginService.getAllUiPlugins().subscribe(plugins => {
      plugins.forEach(plugin => {
        this.router.config.unshift({
            path: plugin.spec.componentName,
            loadChildren: () => loadRemoteModule({
              remoteEntry: plugin.spec.remoteEntry,
              type: 'module',
              exposedModule: plugin.spec.exposedModule,
            })
              .then(m => {
                console.log(m)
                return m.ChartsModule; // todo see if we can get the module name from the plugin spec
              })
              .catch(err => console.error('Error loading remote module', err))
          },
        )
      });
    });
  }
}
