import { computed, inject, Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { catchError, map, Observable } from 'rxjs';
import {
  FilePreviewExtensionPointOutlet,
  Plugin,
  RoutingExtensionPointOutlet,
  SidePanelExtensionPointOutlet,
  TopNavigationBarExtensionPointOutlet,
  UIExtensionPointOutlet,
} from '../../types/plugin';
import { WebResponse } from '../../types/web-response';
import { Router, Routes } from '@angular/router';
import { loadRemoteModule } from '@angular-architects/native-federation';

export interface RemoteMeta {
  remoteName: string;
  remoteEntry: string;
}

type RoutingOutlets = Extract<UIExtensionPointOutlet, RoutingExtensionPointOutlet>;

@Injectable({
  providedIn: 'root',
})
export class GatewayPluginsService {
  private http = inject(HttpClient);
  private router = inject(Router);

  private uiPlugins = signal<Plugin[]>([]);

  public topNavigationBarOutlets = computed<(TopNavigationBarExtensionPointOutlet & RemoteMeta)[]>(() =>
    this.getOutlets(this.uiPlugins(), 'top-navigation-bar')
  );
  public sidePanelOutlets = computed<(SidePanelExtensionPointOutlet & RemoteMeta)[]>(() => this.getOutlets(this.uiPlugins(), 'side-panel'));
  public filePreviewOutlets = computed<(FilePreviewExtensionPointOutlet & RemoteMeta)[]>(() => this.getOutlets(this.uiPlugins(), 'file-previewer'));

  public fetchUIPlugins(): Observable<void> {
    return this.http.get<WebResponse<Plugin[]>>('/api/plugins/gateway', { params: { kind: 'UI' } }).pipe(
      map((response) => {
        const plugins = response.data;
        this.uiPlugins.set(plugins);
        this.updateRoutes(plugins);
      }),
      catchError((err) => {
        console.log('Failed to fetch UI plugins', err);
        return [];
      })
    );
  }
  private updateRoutes(plugins: Plugin[]) {
    const topBarRoutes = this.buildRoutesFromPlugins(plugins, 'top-navigation-bar');
    const sidePanelRoutes = this.buildRoutesFromPlugins(plugins, 'side-panel');

    const updatedProjectRoutesWithPluginSidePanelRoutes = this.router.config.map((route) => {
      if (route.path === 'projects/:id') {
        return {
          ...route,
          children: [...sidePanelRoutes, ...(route.children ?? [])],
        };
      }
      return route;
    });

    this.router.resetConfig([...topBarRoutes, ...updatedProjectRoutesWithPluginSidePanelRoutes]);
  }

  private buildRoutesFromPlugins<T extends RoutingOutlets['type']>(plugins: Plugin[], type: T): Routes {
    const outlets = this.getOutlets(plugins, type) as (RoutingExtensionPointOutlet & RemoteMeta)[];

    return outlets.map((outlet) => this.buildRouteFromPlugin(outlet.remoteName, outlet));
  }

  private buildRouteFromPlugin(remoteName: string, outlet: RoutingExtensionPointOutlet) {
    if (outlet.nested) {
      return {
        path: outlet.path.replace(/^\//, ''),
        loadChildren: () =>
          loadRemoteModule(remoteName, outlet.exposedModule).then((m) => {
            const exposedRouteName = outlet.exposedModule.replace(/^\.\//, '');
            return m[exposedRouteName];
          }),
      };
    }
    return {
      path: outlet.path.replace(/^\//, ''),
      loadComponent: () => loadRemoteModule(remoteName, outlet.exposedModule).then((m) => m[outlet.componentName ?? 'App']),
    };
  }

  private getOutlets<T extends UIExtensionPointOutlet['type']>(plugins: Plugin[], type: T): (Extract<UIExtensionPointOutlet, { type: T }> & RemoteMeta)[] {
    return plugins
      .flatMap((plugin) => plugin.extensionPoints)
      .filter((extensionPoint) => extensionPoint.kind === 'UI')
      .flatMap((extensionPoint) =>
        extensionPoint.outlets
          .filter((outlet): outlet is Extract<UIExtensionPointOutlet, { type: T }> => outlet.type === type)
          .map((outlet) => ({ ...outlet, remoteName: extensionPoint.remoteName, remoteEntry: extensionPoint.remoteEntry }))
      );
  }
}
