import { Injectable, signal } from '@angular/core';
import { IAuthService } from './iauth.service';

@Injectable({
  providedIn: 'root',
})
export class AuthService implements IAuthService {
  authenticated = signal(true);

  constructor() {}

  logout() {
    this.authenticated.set(false);
  }
}
