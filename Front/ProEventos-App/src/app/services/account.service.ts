import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '@environments/environment';
import { catchError, lastValueFrom, map, Observable, ReplaySubject, take, throwError } from 'rxjs';

import { UserLogin } from '@app/models/identity/UserLogin';
import { Usuario } from '@app/models/Usuario';
import { User } from '@app/models/identity/User';
import { UserProfile } from '@app/models/identity/UserProfile';

@Injectable()
export class AccountService {
  private currentUserSource = new ReplaySubject<User>(1);
  public currentUser$ = this.currentUserSource.asObservable();  
  
  baseURL = environment.apiURL + 'api/account';
  constructor(private http: HttpClient){ }

  public getUser(): Observable<UserProfile>{
    return this.http.get<UserProfile>(`${this.baseURL}/getuser`).pipe(take(1));
  }

  public update(userProfile: UserProfile): Observable<void>{
    return this.http.put<UserProfile>(`${this.baseURL}/updateuser`, userProfile).pipe(take(1), map(() => {
      
    }));
  }

  public login(model: any): Observable<void>{           
    return this.http.post<User>(`${this.baseURL}/login`, model).pipe(
      take(1),
      map((response: User) => {
        const user = response;
        if (user){
          this.setCurrentUser(user);
        }
      })
    );
  }

  public Register(model: any): Observable<void>{    
    return this.http.post<User>(`${this.baseURL}/register`, model).pipe(
      take(1),
      map((response: User) => {
        const user = response;
        if (user){
          this.setCurrentUser(user);
        }
      })
    );
  }

  public logout(): void{
    localStorage.removeItem('user');
    this.currentUserSource.next(null as any);
    this.currentUserSource.complete();
  }

  public setCurrentUser(user: User): void{
    localStorage.setItem('user', JSON.stringify(user));
    this.currentUserSource.next(user);
  }

  public uploadImage(file: File): Observable<string>{
    const fileToUpload = file[0] as File;
    const formData = new FormData();
    formData.append('file', fileToUpload);

    return this.http.post<string>(`${this.baseURL}/upload-image`, formData).pipe(take(1));

  }

}
