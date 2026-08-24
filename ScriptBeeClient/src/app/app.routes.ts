import { Router, Routes, withNavigationErrorHandler } from '@angular/router';
import { ProjectsPage } from './pages/projects/projects-page/projects-page.component';
import { CreateProjectPage } from './pages/projects/create-project/create-project.component';
import { ProjectDetailsPage } from './pages/projects/project-details/project-details-page.component';
import { ProjectModelPage } from './pages/projects/project-details/model/project-model-page.component';
import { AnalysisComponent } from './pages/projects/project-details/analysis/analysis.component';
import { AnalysisRunDetailsComponent } from './pages/projects/project-details/analysis-run-details/analysis-run-details.component';
import { ProjectSettingsPage } from './pages/projects/project-details/settings/project-settings.component';
import { PluginsMarketplaceDashboardComponent } from './pages/projects/project-details/plugins/marketplace-dashboard/plugins-marketplace-dashboard.component';
import { PluginDetailsComponent } from './pages/projects/project-details/plugins/plugin-details/plugin-details.component';
import { UploadModelPage } from './pages/projects/project-details/model/upload-model-page/upload-model-page.component';
import { LoadModelPage } from './pages/projects/project-details/model/load-model-page/load-model-page.component';
import { ContextModelPage } from './pages/projects/project-details/model/context-model-page/context-model-page.component';
import { GatewayPluginsComponent } from './pages/gateway-plugins/gateway-plugins.component';
import { inject } from '@angular/core';
import { LoginComponent } from './pages/login/login.component';
import { authGuard } from './guards/auth.guard';
import { loginGuard } from './guards/login.guard';
import { permissionGuard } from './guards/permissions.guard';
import { projectPermissionsGuard } from './guards/projectPermission.guard';

export const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: 'login', component: LoginComponent, canActivate: [loginGuard] },
  { path: 'projects', component: ProjectsPage, canActivate: [authGuard] },
  {
    path: 'gateway-plugins',
    canActivate: [authGuard, permissionGuard('gateway_plugin:management')],
    children: [
      { path: '', component: GatewayPluginsComponent },
      { path: ':pluginId', component: PluginDetailsComponent },
    ],
  },
  { path: 'create-project', component: CreateProjectPage, canActivate: [authGuard] },
  {
    path: 'projects/:id',
    component: ProjectDetailsPage,
    canActivate: [authGuard, projectPermissionsGuard],
    children: [
      {
        path: 'model',
        component: ProjectModelPage,
        children: [
          { path: '', redirectTo: 'upload', pathMatch: 'full' },
          {
            path: 'upload',
            component: UploadModelPage,
            canActivate: [permissionGuard('model:upload')],
          },
          {
            path: 'load',
            component: LoadModelPage,
            canActivate: [permissionGuard('model:load')],
          },
          {
            path: 'context',
            component: ContextModelPage,
          },
        ],
        canActivate: [permissionGuard('model:view')],
      },
      {
        path: 'analysis',
        component: AnalysisComponent,
        canActivate: [permissionGuard('analysis:view')],
      },
      {
        path: 'analysis/:analysisId',
        component: AnalysisRunDetailsComponent,
        canActivate: [permissionGuard('analysis:view')],
      },
      { path: 'settings', component: ProjectSettingsPage, canActivate: [permissionGuard('project:edit')] },
      {
        path: 'plugins',
        children: [
          { path: '', component: PluginsMarketplaceDashboardComponent },
          { path: ':pluginId', component: PluginDetailsComponent },
        ],
        canActivate: [permissionGuard('plugin:view')],
      },
      { path: '**', redirectTo: 'model' },
    ],
  },
];

export const withErrorNavigation = withNavigationErrorHandler((error) => {
  const router = inject(Router);
  if (error?.error) {
    console.error('Navigation error occurred:', error.error);
  }
  void router.navigate(['/projects']);
});
