import { Component, EventEmitter, Input, Output } from '@angular/core';
import { GifDTO } from '../../../shared/DTO';
import { NgIconComponent, provideIcons } from '@ng-icons/core';
import { GifService } from '../../../services/gif.service';
import { heroTrash } from '@ng-icons/heroicons/outline';
import { CommonModule } from '@angular/common';
import { ModalService } from '../../../services/modal.service';

@Component({
  selector: 'app-gif-card',
  standalone: true,
  imports: [
    NgIconComponent,
    CommonModule
  ],
  providers: [GifService, ModalService, provideIcons({
    heroTrash
  })],
  templateUrl: './gif-card.component.html',
  styleUrl: './gif-card.component.scss'
})
export class GifCardComponent {
  @Input() gif: GifDTO = undefined!;
  @Output() onClickDelete = new EventEmitter<number>();

  constructor(private modalService: ModalService){}
  autoPlay: boolean = false;

  onMouseEnter(video: HTMLVideoElement) {
     video.play();
  }

  onMouseOut(video: HTMLVideoElement) {
    video.pause()
  }

  openModal() {
    this.modalService.open({
      header: "Confirmal",
      message: "Are you sure you want to remove the gif?",
      type: "warning"
    })
    .subscribe(() => {
      this.onClickDelete.emit(this.gif.id);
    })
  }

}

