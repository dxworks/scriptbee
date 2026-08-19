import { Observable } from 'rxjs';
import { LoginResponse } from 'angular-auth-oidc-client';

export abstract class AuthService {
  abstract isAuthenticated$: Observable<boolean>;
  abstract userData$: Observable<unknown>;
  abstract checkAuth(): Observable<LoginResponse>;
  abstract login(): void;
  abstract logout(): void;
}
