import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, map } from 'rxjs';
import { ChatDTO, GifDTO, Page } from '../shared/DTO';
import { API_BASE_URL } from '../shared/Constants';

@Injectable({
  providedIn: 'root'
})
export class GifService {

  constructor(private http: HttpClient) { }

  getBlacklistsPage(page: number, chatId: number): Observable<Page<GifDTO>> {
    return this.http.get<Page<GifDTO>>(`${API_BASE_URL}/${chatId}/blacklist?page=${page}`);
  }

  getLocalGifsPage(page: number): Observable<Page<GifDTO>> {
    return this.http
    .get<Page<GifDTO>>(`${API_BASE_URL}/local?page=${page}`)
    .pipe(map(g => {
      g.items.forEach(i => {
        i.url = `${API_BASE_URL}/file?id=${i.id}`
      });
      return g;
    }));
  }

  getChatsPage(page: number): Observable<Page<ChatDTO>> {
    return this.http.get<Page<ChatDTO>>(`${API_BASE_URL}?page=${page}`);
  }

 
}
