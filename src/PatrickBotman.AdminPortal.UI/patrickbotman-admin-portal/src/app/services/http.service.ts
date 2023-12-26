import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ChatDTO, GifDTO, Page } from '../shared/DTO';
import { API_BASE_URL } from '../shared/Constants';

@Injectable({
  providedIn: 'root'
})
export class HttpService {

  constructor(private http: HttpClient) { }

  getBlacklistsPage(page: number, chatId: number): Observable<Page<GifDTO>> {
    return this.http.get<Page<GifDTO>>(`${API_BASE_URL}/${chatId}/gif?page=${page}`);
  }

  getChatsPage(page: number): Observable<Page<ChatDTO>> {
    return this.http.get<Page<ChatDTO>>(`${API_BASE_URL}?page=${page}`);
  }
}
