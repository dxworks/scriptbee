import { CanActivateFn } from '@angular/router';
import { inject } from '@angular/core';
import { PermissionsService } from '../services/auth/permissions.service';
import { toObservable } from '@angular/core/rxjs-interop';
import { filter, map, take } from 'rxjs';

export const projectPermissionsGuard: CanActivateFn = (route) => {
  const permissions = inject(PermissionsService);

  permissions.setProjectId(route.paramMap.get('id')!);

  return toObservable(permissions.status).pipe(
    filter((status) => status === 'resolved' || status === 'error'),
    take(1),
    map((status) => status === 'resolved')
  );
};
