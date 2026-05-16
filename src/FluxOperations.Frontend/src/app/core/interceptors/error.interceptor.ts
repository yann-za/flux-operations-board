import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      const message = error.error?.title ?? error.message ?? 'An unexpected error occurred';
      console.error(`[FluxOps API Error] ${error.status}: ${message}`, error.error?.errors);
      return throwError(() => ({ status: error.status, message, errors: error.error?.errors }));
    })
  );
};
