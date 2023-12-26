import { Component, Input } from '@angular/core';
import { GifDTO } from '../../../shared/DTO';
import { NgIconComponent, provideIcons } from '@ng-icons/core';
import { HttpService } from '../../../services/http.service';
import { heroTrash } from '@ng-icons/heroicons/outline';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-gif-card',
  standalone: true,
  imports: [
    NgIconComponent,
    CommonModule
  ],
  providers: [HttpService, provideIcons({
    heroTrash
  })],
  templateUrl: './gif-card.component.html',
  styleUrl: './gif-card.component.scss'
})
export class GifCardComponent {
  @Input() gif: GifDTO = undefined!;

  autoPlay: boolean = false;

  onMouseEnter(video: HTMLVideoElement) {
     video.play();
  }

  onMouseOut(video: HTMLVideoElement) {
    video.pause()
  }
}

