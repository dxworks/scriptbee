import { Routes } from '@angular/router';
import { ProjectsPage } from './pages/projects/projects-page/projects-page.component';
import { CreateProjectPage } from './pages/projects/create-project/create-project.component';
import { ProjectDetailsPage } from './pages/projects/project-details/project-details-page.component';
import { DetailsContentComponent } from './pages/projects/project-details/tabs/details-content/details-content.component';
import { ScriptsContentComponent } from './pages/projects/project-details/tabs/run-script/scripts-content.component';
import { ProjectSettingsPage } from './pages/projects/project-details/settings/project-settings.component';

export const routes: Routes = [
  { path: '', redirectTo: 'projects', pathMatch: 'full' },
  { path: 'projects', component: ProjectsPage },
  { path: 'create-project', component: CreateProjectPage },
  // { path: 'plugins', component: PluginsMarketplaceDashboardComponent },
  {
    path: 'projects/:id',
    component: ProjectDetailsPage,
    children: [
      { path: 'model', component: DetailsContentComponent },
      {
        path: 'analysis',
        component: ScriptsContentComponent,
        // children: [
        //   { path: '', component: NoScriptsComponent },
        //   { path: ':scriptPath', component: SelectedScriptComponent },
        // ],
      },
      { path: 'settings', component: ProjectSettingsPage },
      { path: '**', redirectTo: 'model' },
    ],
  },
];
