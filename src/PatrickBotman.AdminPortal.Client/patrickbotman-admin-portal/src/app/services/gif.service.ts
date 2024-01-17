import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, map } from 'rxjs';
import { ChatDTO, GifDTO, Page } from '../shared/DTO';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class GifService {

  constructor(private http: HttpClient) { }

  getBlacklistsPage(page: number, chatId: number): Observable<Page<GifDTO>> {
    return this.http.get<Page<GifDTO>>(`${environment.API_BASE_URL}${chatId}/blacklist?page=${page}`);
  }

  getLocalGifsPage(page: number): Observable<Page<GifDTO>> {
    return this.http
    .get<Page<GifDTO>>(`${environment.API_BASE_URL}local?page=${page}`)
    .pipe(map(g => {
      g.items.forEach(i => {
        i.url = `${environment.API_BASE_URL}file?id=${i.id}`
      });
      return g;
    }));
  }

  deleteLocalGif(id: number) {
    return this.http.delete(`${environment.API_BASE_URL}local?id=${id}`);
  }

  getChatsPage(page: number): Observable<Page<ChatDTO>> {
    return this.http.get<Page<ChatDTO>>(`${environment.API_BASE_URL}?page=${page}`);
  }
  
}
