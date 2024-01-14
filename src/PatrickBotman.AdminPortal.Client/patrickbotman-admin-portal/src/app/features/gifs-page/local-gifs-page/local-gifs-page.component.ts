import { Component } from '@angular/core';
import { SimplePaginationComponent } from "../../../common/simple-pagination/simple-pagination.component";
import { GifDTO, Page } from '../../../shared/DTO';
import { CommonModule } from '@angular/common';
import { GifCardComponent } from '../gif-card/gif-card.component';
import { GifService } from '../../../services/gif.service';

@Component({
    selector: 'app-local-gifs-page',
    standalone: true,
    templateUrl: './local-gifs-page.component.html',
    styleUrl: './local-gifs-page.component.scss',
    imports: [CommonModule, GifCardComponent, SimplePaginationComponent],
    providers: [GifService]
})
export class LocalGifsPageComponent {
    localGifsPage: Page<GifDTO> = undefined!;
    page = 0;
    constructor(private http: GifService) {
      this.loadPage();
    }
  
  
    loadPage() { 
      this.http.getLocalGifsPage(this.page).subscribe(res => {
        this.localGifsPage = res;
      });
    }

    deleteGif(id: number) {
      this.http.deleteLocalGif(id).subscribe(() => {
        this.loadPage();
      })
    }
}
