import { Injectable } from "@angular/core";
import { Resolve, Router, ActivatedRouteSnapshot } from '@angular/router';
import { UserService } from '../_services/user.service';
import { AlertifyService } from '../_services/alertify.service';
import { catchError } from 'rxjs/operators';
import { Observable, of } from 'rxjs';
import { Message } from '../_models/Message';
import { AuthService } from '../_services/auth.service';


// Resolvers are used to get data by side effect before the route is loaded
@Injectable()
export class MessagesResolver implements Resolve<Message[]> {

    pageNumber = 1;
    pageSize = 5;
    messageContainer = "Unread";

    constructor (private userService: UserService, private router: Router, private alertify: AlertifyService, private authService: AuthService) {

    }

    resolve(route: ActivatedRouteSnapshot): Observable<Message[]> {
        return this.userService.getMessages(this.authService.decodedToken.nameid, this.pageNumber, this.pageSize, this.messageContainer).pipe(
            catchError(error => {
                this.alertify.error("Problem Retriving Messages");
                this.router.navigate(['/home']);
                return of(null);    // to return observable with null
            })
        );
    }
    
}