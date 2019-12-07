import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { JwtHelperService } from '@auth0/angular-jwt';
import { environment } from 'src/environments/environment';
import { User } from '../_models/User';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  baseUrl = environment.apiurl + "auth/";

  jwtHelper = new JwtHelperService();
  public decodedToken: any;
  currentUser: User;
  photoUrl = new BehaviorSubject<string>('../../assets/User.png');
  currentPhotoUrl = this.photoUrl.asObservable();

  constructor(private http: HttpClient) { }

  login (model: any) {
    return this.http.post(this.baseUrl + "login", model).pipe(
      map((response: any) => {
        const user = response;
        if (user) {
          localStorage.setItem("token", user.token);
          localStorage.setItem("user", JSON.stringify(user.user));
          this.currentUser = user.user;
          this.decodedToken = this.jwtHelper.decodeToken(user.token);
          this.changeMemberPhoto(this.currentUser.photoUrl);
        }
      })
    );
  }

  register (model: any) {
    return this.http.post(this.baseUrl + "register", model);
  }

  loggedIn () {
    const token = localStorage.getItem("token");
    return !this.jwtHelper.isTokenExpired(token);
  }

  changeMemberPhoto(photoUrl: string) {
    this.photoUrl.next(photoUrl);
    
  }


}
