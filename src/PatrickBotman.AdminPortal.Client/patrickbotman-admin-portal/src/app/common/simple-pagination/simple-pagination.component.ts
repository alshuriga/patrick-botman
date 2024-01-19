import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { NgIconComponent, NgIconsModule, provideIcons } from '@ng-icons/core';
import { heroChevronLeft, heroChevronRight } from '@ng-icons/heroicons/outline';
import { PAGE_SIZE } from '../../shared/Constants';

@Component({
  selector: 'app-simple-pagination',
  standalone: true,
  imports: [CommonModule, NgIconComponent],
  providers: [ provideIcons({ heroChevronLeft, heroChevronRight }) ],
  templateUrl: './simple-pagination.component.html',
  styleUrl: './simple-pagination.component.scss'
})
export class SimplePaginationComponent implements OnInit {
  @Input() page!: number;
  @Output() pageChange = new EventEmitter<number>()
  @Input() collectionSize!: number;

  nextActive: boolean = false;
  prevActive: boolean = false;

  ngOnInit(): void {
    this.calculateButtons();
  }

  calculateButtons() {
    const maxPages = Math.ceil(this.collectionSize/PAGE_SIZE);
    this.nextActive = this.page + 1 < maxPages;
    this.prevActive = this.page > 0; 
    console.log(`max: ${maxPages}, nextActive: ${this.nextActive}, prevActive: ${this.prevActive}`)
  }

  nextPage(){
    this.page++;
    this.pageChange.emit(this.page);
    this.calculateButtons();
    this.scrollUp();
  }

  prevPage() {
    this.page--;
    this.pageChange.emit(this.page);
    this.calculateButtons();
    this.scrollUp();
  }

  scrollUp() {
    window.scrollTo({ top: 0, behavior: 'smooth' });
   }
  
}
