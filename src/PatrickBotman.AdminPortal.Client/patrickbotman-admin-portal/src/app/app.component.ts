import { AfterViewInit, Component, OnInit } from '@angular/core';
import { APP_BASE_HREF, CommonModule } from '@angular/common';
import { RouterOutlet } from '@angular/router';
import { NavBarComponent } from './navbar/nav-bar/nav-bar.component';
import { Profile, User } from 'oidc-client';
import { AuthService } from './services/auth.service';
import { Observable } from 'rxjs';
import { environment } from '../environments/environment';
@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterOutlet, NavBarComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss',
  providers: [AuthService, 
  {
    provide: APP_BASE_HREF, useValue: environment.API_BASE_URL
  }]
})
export class AppComponent implements OnInit {
  user: Profile | undefined;

  title = 'patrickbotman-admin-portal';

  constructor(private auth: AuthService) {
    this.auth.user.subscribe(u => {
      this.user = u;
    });
  }

  ngOnInit(): void {
    this.auth.isLoggedIn().then(u => {
      this.user = u?.profile;
    });
  }
}
