import { Routes } from '@angular/router';
import { ProjectsPage } from './pages/projects/projects-page/projects-page.component';

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
