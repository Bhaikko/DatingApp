import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient } from '@angular/common/http';
import { User } from '../_models/User';

@Injectable({
  providedIn: 'root'
})
export class AdminService {
    baseUrl = environment.apiurl;

    constructor(private http: HttpClient) {

    }

    getUsersWithRoles() {
        return this.http.get<User[]>(this.baseUrl + 'admin/usersWithRoles');
    }

    updateUserRoles(user: User, roles: {}) {
        return this.http.post(this.baseUrl + 'admin/editRoles/' + user.username, roles);
    }
}