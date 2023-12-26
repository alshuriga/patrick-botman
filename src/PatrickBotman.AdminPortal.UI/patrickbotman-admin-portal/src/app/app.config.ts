import { ApplicationConfig } from '@angular/core';
import { provideRouter, withComponentInputBinding } from '@angular/router';

import { routes } from './app.routes';
import { HttpClientModule } from '@angular/common/http';
import { NgIconsModule } from '@ng-icons/core';
import { heroUsers } from '@ng-icons/heroicons/outline';

export const appConfig: ApplicationConfig = {
  providers: [
    HttpClientModule,
    provideRouter(routes, withComponentInputBinding()),
  ],
};
