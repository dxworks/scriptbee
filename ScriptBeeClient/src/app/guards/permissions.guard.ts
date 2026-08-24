import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { Permission } from '../types/permissions';
import { PermissionsService } from '../services/auth/permissions.service';

export const permissionGuard = (permission: Permission): CanActivateFn => {
  return () => {
    const service = inject(PermissionsService);
    const router = inject(Router);

    if (service.hasPermission(permission)) {
      return true;
    }
    return router.createUrlTree(['/projects']);
  };
};
