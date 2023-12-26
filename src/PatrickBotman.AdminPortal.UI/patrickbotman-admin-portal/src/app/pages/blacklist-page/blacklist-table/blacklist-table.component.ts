import { Component, Input, OnInit } from '@angular/core';
import { HttpService } from '../../../services/http.service';
import { GifDTO, Page } from '../../../shared/DTO';
import { HttpClientModule } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { GifCardComponent } from '../gif-card/gif-card.component';
import { SimplePaginationComponent } from '../../../common/simple-pagination/simple-pagination.component';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-blacklist-table',
  standalone: true,
  imports: [
    HttpClientModule, CommonModule,
    GifCardComponent, SimplePaginationComponent
  ],
  providers: [HttpService],
  templateUrl: './blacklist-table.component.html',
  styleUrl: './blacklist-table.component.scss'
})
export class BlacklistTableComponent{

  blacklistPage: Page<GifDTO> = undefined!;
  page: number = 0;
  chatId: number = undefined!;
  constructor(private http: HttpService, private route: ActivatedRoute) {
    this.chatId = parseInt(route.snapshot.paramMap.get('chatId')!)
    this.loadPage();
  }


  loadPage() { 
    console.log(`this.page: ${this.page}`)
    this.http.getBlacklistsPage(this.page, this.chatId).subscribe(res => {
      this.blacklistPage = res;
    });
  }
}
