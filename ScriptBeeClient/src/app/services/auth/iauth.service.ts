import { Signal } from '@angular/core';

export abstract class IAuthService {
  abstract authenticated: Signal<boolean>;

  abstract logout(): void;
}
