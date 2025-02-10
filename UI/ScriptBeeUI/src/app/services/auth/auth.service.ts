import { computed, effect, inject, Injectable } from '@angular/core';
import { KEYCLOAK_EVENT_SIGNAL, KeycloakEvent, KeycloakEventType, ReadyArgs, typeEventArgs } from 'keycloak-angular';
import Keycloak from 'keycloak-js';
import { IAuthService } from './iauth.service';

@Injectable({
  providedIn: 'root',
})
export class AuthService implements IAuthService {
  private keycloakSignal = inject(KEYCLOAK_EVENT_SIGNAL);

  authenticated = computed(() => this.isAuthenticated(this.keycloakSignal()));

  constructor(private readonly keycloak: Keycloak) {
    effect(() => {
      if (!this.authenticated()) {
        this.keycloak.login().then();
      }
    });
  }

  logout() {
    this.keycloak.logout().then();
  }

  private isAuthenticated(keycloakEvent: KeycloakEvent) {
    if (keycloakEvent.type === KeycloakEventType.Ready) {
      return typeEventArgs<ReadyArgs>(keycloakEvent.args);
    }
    return false;
  }
}
