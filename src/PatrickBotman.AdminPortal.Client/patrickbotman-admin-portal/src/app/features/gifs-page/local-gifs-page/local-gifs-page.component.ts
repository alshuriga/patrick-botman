import { Component } from '@angular/core';
import { SimplePaginationComponent } from "../../../common/simple-pagination/simple-pagination.component";
import { GifDTO, Page } from '../../../shared/DTO';
import { CommonModule } from '@angular/common';
import { GifCardComponent } from '../gif-card/gif-card.component';
import { GifService } from '../../../services/gif.service';
import { AlertService } from '../../../services/alert.service';
import { AlertComponent } from '../../../common/alert/alert.component';
import { ModalService } from '../../../services/modal.service';

@Component({
  selector: 'app-local-gifs-page',
  standalone: true,
  templateUrl: './local-gifs-page.component.html',
  styleUrl: './local-gifs-page.component.scss',
  imports: [CommonModule,
    GifCardComponent,
    SimplePaginationComponent,
    AlertComponent],
  providers: [ModalService]
})
export class LocalGifsPageComponent {
  localGifsPage: Page<GifDTO> = undefined!;
  page = 0;
  constructor(private http: GifService,
    private alert: AlertService,
    private modal: ModalService) {
    this.loadPage();
  }


  loadPage() {
    this.http.getLocalGifsPage(this.page).subscribe(res => {
      this.localGifsPage = res;
    });
  }

  deleteGif(id: number) {
    this.modal.open({
      header: "Confirmal",
      message: "Are you sure you want to remove the gif?",
      type: "warning"
    })
      .subscribe(() => {
        this.http.deleteLocalGif(id).subscribe(() => {
          this.loadPage();
          this.alert.showAlert({ text: 'GIF was successfully deleted from the database.', mode: 'success', lifetimeSeconds: 3 })
        })
      })
  }
}
