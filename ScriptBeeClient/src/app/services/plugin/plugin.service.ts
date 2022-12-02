import { Injectable } from '@angular/core';
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";
import { Plugin, } from "./plugin";
import { contentHeaders } from "../../shared/headers";
import { MarketplacePlugin } from "./marketplace-plugin";

@Injectable({
  providedIn: 'root'
})
export class PluginService {

  private pluginsApi = '/api/plugin';

  constructor(private http: HttpClient) {
  }

  getAllLoadedPlugins(): Observable<Plugin[]> {
    return this.http.get<Plugin[]>(this.pluginsApi, { headers: contentHeaders });
  }

  getAllAvailablePlugins(start: number = 0, count: number = 10): Observable<MarketplacePlugin[]> {
    return this.http.get<MarketplacePlugin[]>(`${this.pluginsApi}/available`, {
      headers: contentHeaders,
      params: {
        start: start,
        count: count
      }
    });
  }

  installPlugin(pluginId: string, downloadUrl: string) {
    return this.http.post(`${this.pluginsApi}/install`, {
      pluginId: pluginId,
      downloadUrl: downloadUrl
    }, { headers: contentHeaders });
  }

  uninstallPlugin(pluginId: string) {
    return this.http.delete(`${this.pluginsApi}/uninstall/${pluginId}`, { headers: contentHeaders });
  }

  // getAllUiPlugins():Observable<UIPlugin[]>{
  //   return this.http.get<UIPlugin[]>(`${this.pluginsApi}/ui`, {headers: contentHeaders});
  // }
}
