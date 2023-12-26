import { CommonModule } from '@angular/common';
import { Component, Input, OnInit } from '@angular/core';
import { ActivatedRoute, NavigationEnd, Router } from '@angular/router';
import { every, filter } from 'rxjs';

@Component({
  selector: 'app-nav-bar-link',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './nav-bar-link.component.html',
  styleUrl: './nav-bar-link.component.scss'
})
export class NavBarLinkComponent {
  @Input() title: string = null!;
  @Input() path: string = null!;

  isActive: boolean =  false;

  constructor(private router: Router, private route: ActivatedRoute) {
    router.events.pipe(filter((event: any) => event instanceof NavigationEnd))
    .subscribe({
      next: (event) => {
        this.isActive = this.path == event.url;
      },
    })
  }
    

  navigate() {
    this.router.navigateByUrl(this.path)
  }
}
