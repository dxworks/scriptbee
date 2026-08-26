import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs';
import { GlobalPermissionsResponse } from '../../types/permissions';

@Injectable({
  providedIn: 'root',
})
export class GlobalPermissionsService {
  private http = inject(HttpClient);

  getPermissions() {
    return this.http.get<GlobalPermissionsResponse>('/api/permissions').pipe(map((response) => response.permissions));
  }
}
