import { Injectable } from "@angular/core";
import { Resolve, Router, ActivatedRouteSnapshot } from '@angular/router';
import { User } from 'src/app/_models/User';
import { UserService } from '../_services/user.service';
import { AlertifyService } from '../_services/alertify.service';
import { catchError } from 'rxjs/operators';
import { Observable, of } from 'rxjs';


// Resolvers are used to get data by side effect before the route is loaded
@Injectable()
export class MemberListResolver implements Resolve<User[]> {

    constructor (private userService: UserService, private router: Router, private alertify: AlertifyService) {

    }

    resolve(route: ActivatedRouteSnapshot): Observable<User[]> {
        return this.userService.getUsers().pipe(
            catchError(error => {
                this.alertify.error("Problem Retriving Data");
                this.router.navigate(['/home']);
                return of(null);    // to return observable with null
            })
        );
    }
    
}