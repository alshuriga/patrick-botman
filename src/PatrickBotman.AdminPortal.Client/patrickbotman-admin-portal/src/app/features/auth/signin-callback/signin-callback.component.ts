import { Component, OnInit } from '@angular/core';
import { AuthService } from '../../../services/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-signin-callback',
  standalone: true,
  imports: [],
  template: '<div></div>',
})
export class SigninCallbackComponent implements OnInit {
  constructor(private auth: AuthService, private router: Router) {
   
  }

  ngOnInit(): void {
    this.auth.completeLogin().then(() => {
      console.log('completeLogin is fired');
      this.router.navigate([ '/' ], { replaceUrl: true});
    })
  }
}
