import {Routes} from '@angular/router';
import {HomeComponent} from './home/home.component';
import {ProjectsComponent} from './projects/projects.component';
import {ProjectDetailsComponent} from './project-details/project-details.component';
import {DetailsContentComponent} from "./project-details/details-content/details-content.component";
import {NoScriptsComponent} from "./project-details/scripts-content/no-scripts/no-scripts.component";
import {SelectedScriptComponent} from "./project-details/scripts-content/selected-script/selected-script.component";
import {ScriptsContentComponent} from "./project-details/scripts-content/scripts-content.component";

export const ROUTES: Routes = [
  {path: '', component: HomeComponent, pathMatch: 'full'},
  {path: 'projects', component: ProjectsComponent},
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
    ]
  }
];
