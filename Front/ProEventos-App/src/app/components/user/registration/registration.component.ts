import { Component, OnInit } from '@angular/core';
import { AbstractControlOptions, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ValidatorField } from '@app/helpers/ValidatorField';
import { User } from '@app/models/identity/User';
import { AccountService } from '@app/services/account.service';
import { ToastrService } from 'ngx-toastr';
import { NgxSpinnerService } from 'ngx-spinner';
import { Router } from '@angular/router';

@Component({
  selector: 'app-registration',
  templateUrl: './registration.component.html',
  styleUrls: ['./registration.component.scss']
})
export class RegistrationComponent implements OnInit {
  user = {} as User;
  public form!: FormGroup;

  constructor(private fb: FormBuilder, private accountService: AccountService, private toastr: ToastrService, private spinner: NgxSpinnerService, private router: Router) { }

  public get f(): any{
    return this.form.controls;
  }

  ngOnInit(): void {
    this.validation();
  }

  public validation(): void{

    const formOptions: AbstractControlOptions = {
      validators: ValidatorField.MustMatch('password', 'confirmPassword')
    };

    this.form = this.fb.group({
      primeiroNome: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(30)]],
      ultimoNome: ['', [Validators.required, Validators.minLength(4), Validators.maxLength(50)]],
      email: ['', [Validators.required, Validators.email]],
      userName: ['', [Validators.required, Validators.minLength(5)]],
      termos: ['', [Validators.required]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      confirmPassword: ['', [Validators.required]]
    }, formOptions);
  }

  public register(): void{
    this.spinner.show();

    this.user = {...this.form.value}    
    
    this.accountService.Register(this.user).subscribe({
      next: (result: any) => {        
        this.toastr.success('Usuário criado', 'Sucesso');        
        this.router.navigateByUrl('/dashboard');                
      },
      error: (error: any) => {
        console.error(error);
        if (error.status == 400){
          this.toastr.error(error.error, 'Erro');  
        } else {
          this.toastr.error('Não foi possível criar o usuário', 'Erro');
        }
      }
    }).add(() => this.spinner.hide());
  }

}
