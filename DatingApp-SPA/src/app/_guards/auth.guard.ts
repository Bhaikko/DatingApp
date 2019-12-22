import { Injectable } from '@angular/core';
import { CanActivate, Router, ActivatedRouteSnapshot } from '@angular/router';
import { AuthService } from '../_services/auth.service';
import { Route } from '@angular/compiler/src/core';
import { AlertifyService } from '../_services/alertify.service';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {

  constructor (private authService: AuthService, private router: Router, private aleritfy: AlertifyService) {

  }

  canActivate (next: ActivatedRouteSnapshot): boolean {
    const roles = next.firstChild.data['roles'] as Array<string>; // these roles are assigned in routes.ts as data
    if (roles) {
      const match = this.authService.roleMatch(roles);
      if (match) {
        return true;
      } else {
        this.router.navigate(['members']);
        this.aleritfy.error('You are not authorized to access the area');
      }
    }
    if (this.authService.loggedIn()) {
      return true;
    } 

    this.aleritfy.error("You shall not pass!!!");
    this.router.navigate(['/home']);
    return false;
  }
  
}
