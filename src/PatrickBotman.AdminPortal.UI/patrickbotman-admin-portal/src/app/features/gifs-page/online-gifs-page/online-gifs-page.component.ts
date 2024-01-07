import { Component } from '@angular/core';
import { GifService } from '../../../services/gif.service';
import { ActivatedRoute } from '@angular/router';
import { GifDTO, Page } from '../../../shared/DTO';
import { GifCardComponent } from "../gif-card/gif-card.component";
import { SimplePaginationComponent } from "../../../common/simple-pagination/simple-pagination.component";
import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { ModalComponent } from "../../../common/modal/modal.component";

@Component({
    selector: 'app-online-gifs-page',
    standalone: true,
    templateUrl: './online-gifs-page.component.html',
    styleUrl: './online-gifs-page.component.scss',
    providers: [GifService],
    imports: [CommonModule, HttpClientModule, GifCardComponent, SimplePaginationComponent, ModalComponent]
})
export class OnlineGifsPageComponent {
  chatId: number;
  blacklistPage: Page<GifDTO> = undefined!;
  page = 0;
  constructor(private http: GifService, private route: ActivatedRoute) {
    this.chatId = parseInt(route.snapshot.paramMap.get('chatId')!)
    this.loadPage();
  }


  loadPage() { 
    this.http.getBlacklistsPage(this.page, this.chatId).subscribe(res => {
      this.blacklistPage = res;
    });
  }
}
