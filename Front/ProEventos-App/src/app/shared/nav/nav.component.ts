import { Component, OnInit, OnChanges } from '@angular/core';
import { Router } from '@angular/router';
import { User } from '@app/models/identity/User';
import { AccountService } from '@app/services/account.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.scss']
})
export class NavComponent implements OnInit {
  isCollapsed = true;
  currentUser!: string;

  constructor(private router: Router, public accountService: AccountService) { }

  ngOnInit() {    
  }

  ngOnChanges(){
    
  }

  public logout(): void{
    this.accountService.logout();
  }

  public showMenu(): boolean{
    return this.router.url !== '/user/login';
  }

}
