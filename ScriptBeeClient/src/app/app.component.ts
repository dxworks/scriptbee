import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { debounceTime } from 'rxjs/operators';
import { ThemeService } from './services/theme/theme.service';
import { PluginService } from "./services/plugin/plugin.service";
import { ResultsService } from "./services/plugin/results.service";

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html'
})
export class AppComponent {
  title = 'app';

  constructor(private router: Router, private themeService: ThemeService,
              private pluginService: PluginService, private resultsService: ResultsService) {
    this.router.events.pipe(debounceTime(10))
      .subscribe(() => {
        themeService.update();
      });

    // todo was just for proof of concept, need to make more extensible when we have ui plugins
    // this.pluginService.getAllUiPlugins().subscribe(plugins => {
    //   plugins.forEach(plugin => {
    //     console.log(plugin)
    //     loadRemoteModule({
    //       remoteEntry: plugin.spec.remoteEntry,
    //       type: 'module',
    //       exposedModule: plugin.spec.exposedModule,
    //     }).then(m => {
    //       console.log(m)
    //       // console.log(m.LineChartComponent)
    //       // if the exported is a module
    //       // console.log(m.moduleType)
    //       console.log(m.ChartsModule.ɵmod.declarations[0])
    //       // console.log(m.ChartsModule[plugin.spec.componentName])
    //       console.log(plugin.spec.componentName)
    //       console.log(Object.keys(m.ChartsModule))
    //
    //       resultsService.addData({
    //         name: plugin.spec.componentName,
    //         component: m.ChartsModule.ɵmod.declarations[0],
    //         module: m.ChartsModule,
    //       })
    //     });
    //
    //     // todo add to top navigation menu
    //     this.router.config.unshift({
    //         path: plugin.spec.componentName,
    //         loadChildren: () => loadRemoteModule({
    //           remoteEntry: plugin.spec.remoteEntry,
    //           type: 'module',
    //           exposedModule: plugin.spec.exposedModule,
    //         })
    //           .then(m => {
    //             console.log(m)
    //             return m.ChartsModule; // todo see if we can get the module name from the plugin spec
    //           })
    //           .catch(err => console.error('Error loading remote module', err))
    //       },
    //     )
    //   });
    // });
  }
}
