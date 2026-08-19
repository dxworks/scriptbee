import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { shareReplay } from 'rxjs';

export interface AuthConfig {
  authMode?: string;
  authority: string;
  authWellknownEndpointUrl?: string;
  clientId: string;
  scope: string;
}

@Injectable({ providedIn: 'root' })
export class ConfigService {
  private http = inject(HttpClient);

  readonly config$ = this.http.get<AuthConfig>('/api/config/auth').pipe(shareReplay(1));
}
