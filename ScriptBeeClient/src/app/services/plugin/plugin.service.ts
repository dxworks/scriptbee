import { Injectable } from '@angular/core';
import { HttpClient } from "@angular/common/http";
import { Observable, of } from "rxjs";
import { Plugin, } from "./plugin";
import { contentHeaders } from "../../shared/headers";
import { BaseMarketplacePlugin, MarketplaceBundlePlugin, MarketplaceSinglePlugin } from "./marketplace-plugin";

@Injectable({
  providedIn: 'root'
})
export class PluginService {

  private pluginsApi = '/api/plugin';

  constructor(private http: HttpClient) {
  }

  getAllLoadedPlugins(): Observable<Plugin[]> {
    return this.http.get<Plugin[]>(this.pluginsApi, {headers: contentHeaders});
  }

  getAllAvailablePlugins(): Observable<BaseMarketplacePlugin[]> {
    // generate random data for now
    const singlePlugins: MarketplaceSinglePlugin[] = [];
    for (let i = 0; i < 10; i++) {
      singlePlugins.push({
        id: i.toString(),
        name: 'Plugin ' + i,
        author: 'Author ' + i,
        description: 'Description ' + i,
        downloadUrl: 'downloadUrl ' + i,
        type: 'plugin',
        versions: {
          '1.0.0': {installed: false, kinds: ['ScriptRunner', 'ScriptGenerator']},
          '1.0.1': {installed: true, kinds: ['ScriptRunner', 'ScriptGenerator', 'HelperFunctions']},
        },
      });
    }
    const bundlePlugins: MarketplaceBundlePlugin[] = [];
    for (let i = 0; i < 10; i++) {
      bundlePlugins.push({
        id: (i + singlePlugins.length).toString(),
        name: 'Bundle ' + i,
        author: 'Author' + i,
        description: 'Description ' + i,
        downloadUrl: 'downloadUrl ' + i,
        type: 'bundle',
        versions: {
          "1.0.0": {
            installed: true,
            plugins: [
              {name: 'Plugin 1', version: "1.0.0", kinds: ['ScriptRunner', 'ScriptGenerator']},
              {name: 'Plugin 2', version: "1.0.0", kinds: ['ScriptRunner', 'ScriptGenerator', 'HelperFunctions']}
            ]
          },
          "1.0.1": {
            installed: false,
            plugins: [
              {name: 'Plugin 1', version: "1.0.1", kinds: ['ScriptRunner', 'ScriptGenerator']},
              {name: 'Plugin 2', version: "1.0.1", kinds: ['ScriptRunner', 'ScriptGenerator', 'HelperFunctions']},
              {name: 'Plugin 3', version: "1.0.1", kinds: ['ScriptRunner', 'ScriptGenerator', 'HelperFunctions']}
            ]
          }
        },
      });
    }

    return of<BaseMarketplacePlugin[]>([...bundlePlugins, ...singlePlugins]);

    // return this.http.get<BaseMarketplacePlugin[]>(this.pluginsApi + '/available', {headers: contentHeaders});
  }

  // getAllUiPlugins():Observable<UIPlugin[]>{
  //   return this.http.get<UIPlugin[]>(`${this.pluginsApi}/ui`, {headers: contentHeaders});
  // }
}
