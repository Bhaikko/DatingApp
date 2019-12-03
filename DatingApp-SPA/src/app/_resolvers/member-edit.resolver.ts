import { Injectable } from "@angular/core";
import { Resolve, Router, ActivatedRouteSnapshot } from '@angular/router';
import { User } from 'src/_models/User';
import { UserService } from '../_services/user.service';
import { AlertifyService } from '../_services/alertify.service';
import { catchError } from 'rxjs/operators';
import { Observable, of } from 'rxjs';
import { AuthService } from '../_services/auth.service';


// Resolvers are used to get data by side effect before the route is loaded
@Injectable()
export class MemberEditResolver implements Resolve<User> {

    constructor (private userService: UserService, private router: Router, private alertify: AlertifyService, private authService: AuthService) {

    }

    resolve(route: ActivatedRouteSnapshot): Observable<User> {
        return this.userService.getUser(this.authService.decodedToken.nameid).pipe(
            catchError(error => {
                this.alertify.error("Problem Retriving Your Data");
                this.router.navigate(['/members']);
                return of(null);    
            })
        );
    }
    
}