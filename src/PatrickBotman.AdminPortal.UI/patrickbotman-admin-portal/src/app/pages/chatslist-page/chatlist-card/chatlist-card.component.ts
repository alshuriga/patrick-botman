import { Component, Input } from '@angular/core';
import { ChatDTO } from '../../../shared/DTO';
import { Router } from '@angular/router';
import { NgIconsModule, provideIcons } from '@ng-icons/core';

@Component({
  selector: 'app-chatlist-card',
  standalone: true,
  imports: [],
  templateUrl: './chatlist-card.component.html',
  styleUrl: './chatlist-card.component.scss'
})
export class ChatlistCardComponent {
  @Input() chat: ChatDTO = undefined!;

  constructor(private router: Router) {}

  navigateToBlacklist() {
    this.router.navigateByUrl(`blacklist/${this.chat.id}`)
  }
  

}

