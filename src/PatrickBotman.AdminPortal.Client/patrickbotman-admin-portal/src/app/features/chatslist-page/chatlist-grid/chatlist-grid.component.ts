import { Component } from '@angular/core';
import { ChatDTO, Page } from '../../../shared/DTO';
import { GifService } from '../../../services/gif.service';
import { CommonModule } from '@angular/common';
import { ChatlistCardComponent } from '../chatlist-card/chatlist-card.component';
import { googleAuthInterceptor } from '../../../interceptors/google-auth.interceptor';

@Component({
  selector: 'app-chatlist-grid',
  standalone: true,
  imports: [CommonModule, ChatlistCardComponent],
  providers: [GifService],
  templateUrl: './chatlist-grid.component.html',
  styleUrl: './chatlist-grid.component.scss'
})
export class ChatlistGridComponent {
  chatsPage: Page<ChatDTO> = undefined!;

  constructor(private http: GifService) {
    this.http.getChatsPage(0).subscribe(res => {
      this.chatsPage = res
    });
    }
  }
