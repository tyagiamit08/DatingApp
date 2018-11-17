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
  httpOptions = {
    headers: new HttpHeaders({
      Authorization: 'Bearer ' + localStorage.getItem('token')
    })
  };

  constructor(private http: HttpClient) {}

  // getUsers() {
  getUsers(): Observable<User[]> {
    return this.http.get<User[]>(this.baseUrl, this.httpOptions);
  }

  getUser(id): Observable<User> {
    return this.http.get<User>(this.baseUrl + '/' + id, this.httpOptions);
  }
}
