import { Injectable, signal, WritableSignal } from '@angular/core';
import { IAuthService } from './iauth.service';

@Injectable()
export class MockedAuthService implements IAuthService {
  authenticated: WritableSignal<boolean> = signal(true);

  logout(): void {
    this.authenticated.set(false);
  }
}
