import { Routes } from '@angular/router';
import { ProjectsComponent } from './projects/projects.component';
import { ProjectDetailsComponent } from './project-details/project-details.component';
import { DetailsContentComponent } from "./project-details/details-content/details-content.component";
import { NoScriptsComponent } from "./project-details/scripts-content/no-scripts/no-scripts.component";
import { SelectedScriptComponent } from "./project-details/scripts-content/selected-script/selected-script.component";
import { ScriptsContentComponent } from "./project-details/scripts-content/scripts-content.component";
import { PluginsComponent } from "./plugins/plugins/plugins.component";

export const ROUTES: Routes = [
  // {path: '', component: HomeComponent, pathMatch: 'full'},
  {path: '', redirectTo: 'projects', pathMatch: 'full'},
  {path: 'projects', component: ProjectsComponent},
  {path: 'plugins', component: PluginsComponent},
  {
    path: 'projects/:id', component: ProjectDetailsComponent,
    children: [
      {path: 'details', component: DetailsContentComponent},
      {
        path: 'scripts', component: ScriptsContentComponent,
        children: [
          {path: '', component: NoScriptsComponent},
          {path: ':scriptPath', component: SelectedScriptComponent},
        ]
      },
      {path: '**', redirectTo: 'details'},
    ]
  },
];
