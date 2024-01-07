import { Routes } from '@angular/router';

export const routes: Routes = [
    { path: 'blacklist/:chatId', loadComponent: () => import('./features/gifs-page/online-gifs-page/online-gifs-page.component').then(mod => mod.OnlineGifsPageComponent) },
    { path: 'custom-gifs', loadComponent: () => import('./features/gifs-page/local-gifs-page/local-gifs-page.component').then(mod => mod.LocalGifsPageComponent) },
    { path: 'chats', loadComponent: () => import('./features/chatslist-page/chatlist-grid/chatlist-grid.component').then(mod => mod.ChatlistGridComponent) }
];
