import { Injectable } from '@angular/core';
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";
import { Plugin, } from "./plugin";
import { contentHeaders } from "../../shared/headers";

@Injectable({
  providedIn: 'root'
})
export class PluginService {

  private pluginsApi = '/api/plugin';

  constructor(private http: HttpClient) {
  }

  getAllPlugins(): Observable<Plugin[]> {
    return this.http.get<Plugin[]>(this.pluginsApi, {headers: contentHeaders});
  }

  // getAllUiPlugins():Observable<UIPlugin[]>{
  //   return this.http.get<UIPlugin[]>(`${this.pluginsApi}/ui`, {headers: contentHeaders});
  // }
}
