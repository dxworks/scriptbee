import { Routes } from '@angular/router';
import { ProjectsPage } from './pages/projects/projects-page/projects-page.component';
import { CreateProjectPage } from './pages/projects/create-project/create-project.component';
import { ProjectDetailsPage } from './pages/projects/project-details/project-details-page.component';
import { ProjectModelPage } from './pages/projects/project-details/model/project-model-page.component';
import { AnalysisComponent } from './pages/projects/project-details/analysis/analysis.component';
import { ProjectSettingsPage } from './pages/projects/project-details/settings/project-settings.component';
import { PluginsMarketplaceDashboardComponent } from './pages/projects/project-details/plugins/marketplace-dashboard/plugins-marketplace-dashboard.component';
import { PluginDetailsComponent } from './pages/projects/project-details/plugins/plugin-details/plugin-details.component';

export const routes: Routes = [
  { path: '', redirectTo: 'projects', pathMatch: 'full' },
  { path: 'projects', component: ProjectsPage },
  { path: 'create-project', component: CreateProjectPage },
  {
    path: 'projects/:id',
    component: ProjectDetailsPage,
    children: [
      {
        path: 'model',
        component: ProjectModelPage,
      },
      {
        path: 'analysis',
        component: AnalysisComponent,
      },
      { path: 'settings', component: ProjectSettingsPage },
      {
        path: 'plugins',
        children: [
          { path: '', component: PluginsMarketplaceDashboardComponent },
          { path: ':pluginId', component: PluginDetailsComponent },
        ],
      },
      { path: '**', redirectTo: 'model' },
    ],
  },
];
