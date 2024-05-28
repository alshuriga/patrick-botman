import { ApplicationConfig } from '@angular/core';
import { provideRouter, withComponentInputBinding } from '@angular/router';

import { routes } from './app.routes';
import { provideHttpClient, withInterceptors, withRequestsMadeViaParent } from '@angular/common/http';

import { googleAuthInterceptor } from './interceptors/google-auth.interceptor';
import { APP_BASE_HREF } from '@angular/common';
import { environment } from '../environments/environment';
import { ModalService } from './services/modal.service';

export const appConfig: ApplicationConfig = {
  providers: [
    provideHttpClient(
      withInterceptors([googleAuthInterceptor])),
    provideRouter(routes, withComponentInputBinding())
  ]
};
