import { Routes } from '@angular/router';
import { ProjectsPage } from './pages/projects/projects-page.component';
// import { ProjectsComponent } from './projects/projects.component';
// import { ProjectDetailsComponent } from './project-details/components/project-details.component';
// import { DetailsContentComponent } from './project-details/components/details-content/details-content.component';
// import { NoScriptsComponent } from './project-details/components/run-script/scripts-content/no-scripts/no-scripts.component';
// import { SelectedScriptComponent } from './project-details/components/run-script/selected-script/selected-script.component';
// import { ScriptsContentComponent } from './project-details/components/run-script/scripts-content.component';
// import { PluginsMarketplaceDashboardComponent } from './plugins/components/plugins-marketplace-dashboard/plugins-marketplace-dashboard.component';

export const routes: Routes = [
  // {path: '', component: HomeComponent, pathMatch: 'full'},
  { path: '', redirectTo: 'projects', pathMatch: 'full' },
  { path: 'projects', component: ProjectsPage },
  // { path: 'plugins', component: PluginsMarketplaceDashboardComponent },
  // {
  //   path: 'projects/:id',
  //   component: ProjectDetailsComponent,
  //   children: [
  //     { path: 'details', component: DetailsContentComponent },
  //     {
  //       path: 'scripts',
  //       component: ScriptsContentComponent,
  //       children: [
  //         { path: '', component: NoScriptsComponent },
  //         { path: ':scriptPath', component: SelectedScriptComponent },
  //       ],
  //     },
  //     { path: '**', redirectTo: 'details' },
  //   ],
  // },
];
