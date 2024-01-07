import { Component } from '@angular/core';
import { SimplePaginationComponent } from "../../../common/simple-pagination/simple-pagination.component";
import { GifDTO, Page } from '../../../shared/DTO';
import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { GifCardComponent } from '../gif-card/gif-card.component';
import { GifService } from '../../../services/gif.service';

@Component({
    selector: 'app-local-gifs-page',
    standalone: true,
    templateUrl: './local-gifs-page.component.html',
    styleUrl: './local-gifs-page.component.scss',
    imports: [CommonModule, HttpClientModule, GifCardComponent, SimplePaginationComponent],
    providers: [GifService]
})
export class LocalGifsPageComponent {
    blacklistPage: Page<GifDTO> = undefined!;
    page = 0;
    constructor(private http: GifService) {
      this.loadPage();
    }
  
  
    loadPage() { 
      this.http.getLocalGifsPage(this.page).subscribe(res => {
        this.blacklistPage = res;
      });
    }
}
