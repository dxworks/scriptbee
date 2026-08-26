import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { toObservable } from '@angular/core/rxjs-interop';
import { filter, map, take } from 'rxjs';
import { Permission } from '../types/permissions';
import { PermissionsService } from '../services/auth/permissions.service';

export const permissionGuard = (permission: Permission): CanActivateFn => {
  return () => {
    const service = inject(PermissionsService);
    const router = inject(Router);

    if (service.globalStatus() === 'resolved' || service.globalStatus() === 'error') {
      if (service.hasPermission(permission)) {
        return true;
      }
      return router.createUrlTree(['/projects']);
    }

    return toObservable(service.globalStatus).pipe(
      filter((status) => status === 'resolved' || status === 'error'),
      take(1),
      map(() => {
        if (service.hasPermission(permission)) {
          return true;
        }
        return router.createUrlTree(['/projects']);
      })
    );
  };
};
