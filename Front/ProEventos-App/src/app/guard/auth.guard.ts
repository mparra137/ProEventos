import { JsonPipe } from '@angular/common';
import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot, UrlTree } from '@angular/router';
import { User } from '@app/models/identity/User';
import { ToastrService } from 'ngx-toastr';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {

  constructor(private router: Router, private toastr: ToastrService){}

  canActivate(): boolean {
    let user: User;
    user = JSON.parse(localStorage.getItem('user') ?? '{}');

    if (user != null){
      return true;
    }

    this.toastr.info('Usuário não autenticado');
    this.router.navigate(['/user/login']);
    return false;
  }
  
}
