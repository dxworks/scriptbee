import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Plugin } from './plugin';
import { contentHeaders } from '../../shared/headers';
import { MarketplaceProject } from './marketplace-project';

@Injectable({
  providedIn: 'root',
})
export class PluginService {
  private pluginsApi = '/api/plugins';

  constructor(private http: HttpClient) {}

  getAllLoadedPlugins(): Observable<Plugin[]> {
    return this.http.get<Plugin[]>(this.pluginsApi, {
      headers: contentHeaders,
    });
  }

  getAllAvailablePlugins(): Observable<MarketplaceProject[]> {
    return this.http.get<MarketplaceProject[]>(`${this.pluginsApi}/available`, { headers: contentHeaders });
  }

  installPlugin(pluginId: string, version: string) {
    return this.http.post<void>(
      `${this.pluginsApi}/install`,
      {
        pluginId,
        version,
      },
      { headers: contentHeaders }
    );
  }

  uninstallPlugin(pluginId: string, version: string) {
    return this.http.delete<void>(`${this.pluginsApi}/uninstall/${pluginId}/${version}`, { headers: contentHeaders });
  }

  // getAllUiPlugins():Observable<UIPlugin[]>{
  //   return this.http.get<UIPlugin[]>(`${this.pluginsApi}/ui`, {headers: contentHeaders});
  // }
}
