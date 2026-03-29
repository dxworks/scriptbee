import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map, Observable } from 'rxjs';
import { InstalledPlugin, MarketplacePlugin, MarketplacePluginWithDetails } from '../../types/marketplace-plugin';
import { WebResponse } from '../../types/web-response';

@Injectable({
  providedIn: 'root',
})
export class PluginService {
  constructor(private http: HttpClient) {}

  public getAllAvailablePlugins(): Observable<MarketplacePlugin[]> {
    return this.http.get<WebResponse<MarketplacePlugin[]>>('/api/plugins').pipe(map((response) => response.data));
  }

  public getPlugin(pluginId: string): Observable<MarketplacePluginWithDetails> {
    return this.http.get<MarketplacePluginWithDetails>(`/api/plugins/${pluginId}`);
  }

  getInstalledPlugins(projectId: string): Observable<InstalledPlugin[]> {
    return this.http.get<WebResponse<InstalledPlugin[]>>(`/api/projects/${projectId}/plugins`).pipe(map((response) => response.data));
  }

  installPlugin(projectId: string, pluginId: string, version: string | undefined): Observable<void> {
    return this.http.put<void>(`/api/projects/${projectId}/plugins/${pluginId}?version=${version}`, undefined);
  }

  uninstallPlugin(projectId: string, pluginId: string, version: string) {
    return this.http.delete<void>(`/api/projects/${projectId}/plugins/${pluginId}?version=${version}`);
  }
}
