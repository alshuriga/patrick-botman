import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { NavBarLinkComponent } from '../nav-bar-link/nav-bar-link.component';

@Component({
  selector: 'app-nav-bar',
  standalone: true,
  imports: [NavBarLinkComponent],
  templateUrl: './nav-bar.component.html',
  styleUrl: './nav-bar.component.scss'
})
  export class NavBarComponent  {
  constructor(private router: Router) {
  }

  goTo(url: string) {
    this.router.navigateByUrl(url);
  }

}
