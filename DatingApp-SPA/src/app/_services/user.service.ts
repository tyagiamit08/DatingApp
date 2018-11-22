import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { Observable } from 'rxjs';
import { User } from '../_models/User';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  baseUrl = environment.apiUrl + 'Users';

  constructor(private http: HttpClient) {}

  // getUsers() { This also works fine
  //   return this.http.get(this.baseUrl);
  // }

  getUsers(): Observable<User[]> {
    return this.http.get<User[]>(this.baseUrl);
  }

  getUser(id): Observable<User> {
    return this.http.get<User>(this.baseUrl + '/' + id);
  }

  updateUser(userId: number, userToUpdate: User) {
    return this.http.put(this.baseUrl + '/' + userId, userToUpdate);
  }
}
