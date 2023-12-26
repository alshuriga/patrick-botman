import { Component } from '@angular/core';
import { ChatDTO, Page } from '../../../shared/DTO';
import { HttpService } from '../../../services/http.service';
import { HttpClientModule } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { ChatlistCardComponent } from '../chatlist-card/chatlist-card.component';

@Component({
  selector: 'app-chatlist-grid',
  standalone: true,
  imports: [HttpClientModule, CommonModule, ChatlistCardComponent],
  providers: [HttpService],
  templateUrl: './chatlist-grid.component.html',
  styleUrl: './chatlist-grid.component.scss'
})
export class ChatlistGridComponent {
  chatsPage: Page<ChatDTO> = undefined!;

  constructor(private http: HttpService) {
    this.http.getChatsPage(0).subscribe(res => {
      this.chatsPage = res
    });
    }
  }
