import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { ClientIdService } from '../services/common/client-id.service';

export const clientIdInterceptor: HttpInterceptorFn = (req, next) => {
  const clientIdService = inject(ClientIdService);

  const clonedRequest = req.clone({
    setHeaders: {
      'X-Client-Id': clientIdService.clientId,
    },
  });

  return next(clonedRequest);
};
