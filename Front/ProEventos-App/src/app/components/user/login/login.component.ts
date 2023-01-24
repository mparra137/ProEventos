import { ReturnStatement } from '@angular/compiler';
import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, FormControl, Validators} from '@angular/forms';
import { Router } from '@angular/router';
import { UserLogin } from '@app/models/identity/UserLogin';
import { AccountService } from '@app/services/account.service';
import { NgxSpinnerService } from 'ngx-spinner';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {
  public model = {} as UserLogin;
  //public form!: FormGroup

  constructor(private accountService: AccountService, 
              private spinner: NgxSpinnerService, private toastr: ToastrService, private router: Router) { }

  ngOnInit(): void {
    //this.initiateForm();
  }

  //public initiateForm(): void{
    //this.form = this.fb.group({
    //  userName: ['', [Validators.required]],
    //  passWord: ['', [Validators.required]]
    //});
  //}

  //public get f(): any{
  //  return this.form.controls;
  //}

  public login(): void{
    this.spinner.show();
    this.accountService.login(this.model).subscribe({
      next: () => {
        this.router.navigateByUrl('/dashboard');
      },
      error: (error: any) => {
        if (error.status == 401)
          {this.toastr.error('Usuário ou senha inválidos', 'Erro');}
        else  
          {console.error(error);
          this.toastr.error('Não foi possível acessar. Tente novamente mais tarde.', 'Erro');}
      }
    }).add(() => this.spinner.hide());       
  }
}


//console.log('ComponentResult:', result);
//this.spinner.show();
//this.login = {...this.form.value};
//this.loginService.Login(this.login).then((result: any) => {
//  console.log('PromiseResult: ', result);
//  if (result){
//    this.toastr.error(result, 'Erro');
//  }
//}).finally(() => this.spinner.hide());    
