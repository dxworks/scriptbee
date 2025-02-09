import { computed, effect, inject, Injectable } from '@angular/core';
import { KEYCLOAK_EVENT_SIGNAL, KeycloakEvent, KeycloakEventType, ReadyArgs, typeEventArgs } from 'keycloak-angular';
import Keycloak from 'keycloak-js';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private keycloakSignal = inject(KEYCLOAK_EVENT_SIGNAL);

  authenticated = computed(() => this.isAuthenticated(this.keycloakSignal()));

  constructor(private readonly keycloak: Keycloak) {
    effect(() => {
      if (!this.authenticated()) {
        void this.keycloak.login();
      }
    });
  }

  logout() {
    void this.keycloak.logout();
  }

  private isAuthenticated(keycloakEvent: KeycloakEvent) {
    if (keycloakEvent.type === KeycloakEventType.Ready) {
      return typeEventArgs<ReadyArgs>(keycloakEvent.args);
    }
    return false;
  }
}
