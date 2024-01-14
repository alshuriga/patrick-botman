import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth.service';
import { catchError, from, map, switchMap, tap, throwError } from 'rxjs';
import { Router } from '@angular/router';

export const googleAuthInterceptor: HttpInterceptorFn = (req, next) => {
  const auth = inject(AuthService);
  const router = inject(Router);

  return from(auth.isLoggedIn())
  .pipe(switchMap(u => {
    if(u?.id_token) {
      req = req.clone({ setHeaders: { 'Authorization': `Bearer ${u.id_token}` } });
    }


    return next(req).pipe(tap({
      error: err => {
        if(err instanceof HttpErrorResponse)
        {
          if(err.status == 401) {
            auth.login();
          }
          else if(err.status == 403) {
            router.navigate(['access-denied'])
          }
        }
      
  }}));
  }));

};
