import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { NavBarLinkComponent } from '../nav-bar-link/nav-bar-link.component';
import { Profile, User } from 'oidc-client';
import { AuthService } from '../../services/auth.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-nav-bar',
  standalone: true,
  imports: [NavBarLinkComponent, CommonModule],
  templateUrl: './nav-bar.component.html',
  styleUrl: './nav-bar.component.scss'
})
  export class NavBarComponent implements OnInit{

  user: Profile | undefined;

  constructor(private router: Router, private auth: AuthService) {
    console.log('subscribing in navbar');
    
    this.auth.user.subscribe(u => {
      this.user = u;
      console.log('nav-bar user set: ' + this.user?.email);
    });
  }

  ngOnInit(): void {
    console.log('navbar oninit: this.auth.isLoggedIn()')
    this.auth.isLoggedIn().then(u => this.user = u?.profile)
  }

  goTo(url: string) {
    this.router.navigateByUrl(url);
  }

  login() {
    this.auth.login();
  }

  logout() {
    this.auth.logout().then(() => {
      window.location.reload();
    })
  }

}
