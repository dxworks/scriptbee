import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { of } from 'rxjs';
import { beforeEach, describe, expect, it, vi } from 'vitest';

import { AuthService } from '../../services/auth/AuthService';
import { LoginComponent } from './login.component';

describe('LoginComponent', () => {
  let component: LoginComponent;
  let fixture: ComponentFixture<LoginComponent>;
  let authService: { login: ReturnType<typeof vi.fn> };

  beforeEach(async () => {
    authService = {
      login: vi.fn(),
      logout: vi.fn(),
      checkAuth: vi.fn(),
      isAuthenticated$: of(false),
      userData$: of(null),
    } as unknown as { login: ReturnType<typeof vi.fn> } & AuthService;

    await TestBed.configureTestingModule({
      imports: [LoginComponent],
      providers: [{ provide: AuthService, useValue: authService }],
    }).compileComponents();

    fixture = TestBed.createComponent(LoginComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should render a sign in page for OIDC-only authentication', () => {
    const heading = fixture.nativeElement.querySelector('h1');
    const subtitle = fixture.nativeElement.querySelector('.subtitle');

    expect(heading.textContent).toContain('Welcome to ScriptBee');
    expect(subtitle.textContent).toContain('OpenID Connect');
  });

  it('should trigger the OIDC login flow when the login button is clicked', () => {
    const button = fixture.debugElement.query(By.css('.login-button'));

    button.triggerEventHandler('click', null);

    expect(authService.login).toHaveBeenCalledTimes(1);
  });
});
