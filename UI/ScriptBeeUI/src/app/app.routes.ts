import { Routes } from '@angular/router';
import { ProjectsPage } from './pages/projects/projects-page/projects-page.component';
import { CreateProjectPage } from './pages/projects/create-project/create-project.component';

export const routes: Routes = [
  { path: '', redirectTo: 'projects', pathMatch: 'full' },
  { path: 'projects', component: ProjectsPage },
  { path: 'create-project', component: CreateProjectPage },
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
