import { Injectable } from '@angular/core';
import { Profile, User, UserManager, UserManagerSettings } from 'oidc-client';
import { BehaviorSubject, ReplaySubject, Subject } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private _userManager: UserManager;
  private _userInfoChanged = new Subject<Profile | undefined>;

  user = this._userInfoChanged.asObservable();

  constructor() { 
    var settings: UserManagerSettings = {
      authority: 'https://accounts.google.com/',
      scope: 'openid email profile',
      client_id: '678092915142-sbh7tmh3demtir4b4tupig8gl47fsfh6.apps.googleusercontent.com',
      client_secret: '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',
      redirect_uri: `${environment.CLIENT_BASE_URL}signin-callback`,
      post_logout_redirect_uri: `${environment.CLIENT_BASE_URL}signout-callback`,
    }
    console.log(JSON.stringify(settings));
    this._userManager = new UserManager(settings);
  }

  login() 
  {
    return this._userManager.signinRedirect();
  }

  logout()
  {
    return this._userManager.removeUser().then(() => {
      console.log('logout() pushes new user: ' + null)
      this._userInfoChanged.next(undefined);
      this._userManager.signinRedirect();
    })
  }

  completeLogin()
  {
    return this._userManager.signinRedirectCallback().then(u => {
      console.log('completeLogin() pushes new user: ' + u.profile.email)
      this._userInfoChanged.next(u.profile);
      return u;
    })
  }

  isLoggedIn() 
  {
    return this._userManager.getUser().then(u => {
      return u ?? undefined;
    })
  }

}
