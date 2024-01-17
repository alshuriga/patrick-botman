import { Component } from '@angular/core';
import { AuthService } from '../../../services/auth.service';

@Component({
  selector: 'app-access-denied',
  standalone: true,
  imports: [],
  templateUrl: './access-denied.component.html',
  styleUrl: './access-denied.component.scss'
})
export class AccessDeniedComponent {
  constructor(private auth: AuthService){
    
  }

  login() {
    this.auth.logout().then(() => {
      this.auth.login();
    })
  }
}
