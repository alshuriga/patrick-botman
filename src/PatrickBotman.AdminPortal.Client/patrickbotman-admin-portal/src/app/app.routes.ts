import { provideHttpClient, withRequestsMadeViaParent } from '@angular/common/http';
import { Routes } from '@angular/router';

export const routes: Routes = [
    { path: 'blacklist/:chatId', loadComponent: () => import('./features/gifs-page/online-gifs-page/online-gifs-page.component').then(mod => mod.OnlineGifsPageComponent) },
    { path: 'custom-gifs', loadComponent: () => import('./features/gifs-page/local-gifs-page/local-gifs-page.component').then(mod => mod.LocalGifsPageComponent) },
    { path: 'chats', providers: [provideHttpClient(withRequestsMadeViaParent())], loadComponent: () => import('./features/chatslist-page/chatlist-grid/chatlist-grid.component').then(mod => mod.ChatlistGridComponent) },
    { path: 'settings', providers: [provideHttpClient(withRequestsMadeViaParent())], loadComponent: () => import('./features/settings-page/settings-page/settings-page.component').then(mod => mod.SettingsPageComponent) },
    { path: '', redirectTo: 'custom-gifs', pathMatch: 'full' },
    //auth
    { path: 'signin-callback', loadComponent: () => import('./features/auth/signin-callback/signin-callback.component').then(mod => mod.SigninCallbackComponent) },
    { path: 'access-denied', loadComponent: () => import('./features/auth/access-denied/access-denied.component').then(mod => mod.AccessDeniedComponent)}
];
