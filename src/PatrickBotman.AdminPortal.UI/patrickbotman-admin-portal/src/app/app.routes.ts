import { Routes } from '@angular/router';

export const routes: Routes = [
    { path: 'blacklist/:chatId', loadComponent: () => import('./pages/blacklist-page/blacklist-table/blacklist-table.component').then(mod => mod.BlacklistTableComponent) },
    { path: 'chats', loadComponent: () => import('./pages/chatslist-page/chatlist-grid/chatlist-grid.component').then(mod => mod.ChatlistGridComponent) }
];
