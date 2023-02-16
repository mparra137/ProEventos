import { NotExpr } from '@angular/compiler';
import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators, AbstractControlOptions } from '@angular/forms';
import { ValidatorField } from '@app/helpers/ValidatorField';
import { User } from '@app/models/identity/User';
import { UserProfile } from '@app/models/identity/UserProfile';
import { AccountService } from '@app/services/account.service';
import { environment } from '@environments/environment';
import { NgxSpinnerService } from 'ngx-spinner';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-perfil',
  templateUrl: './perfil.component.html',
  styleUrls: ['./perfil.component.css']
})
export class PerfilComponent implements OnInit {
  public user = {} as UserProfile;
  public form!: FormGroup;
  public imagemURL!: string;
  public file!: File;

  constructor(private fb: FormBuilder, private accountService: AccountService, private spinner: NgxSpinnerService, private toatr: ToastrService) { }

  public get f(): any{
    return this.form.controls;
  }

  ngOnInit() {
    this.validation();
    this.getUser();
    if (this.imagemURL == undefined){
      this.imagemURL = "https://encrypted-tbn2.gstatic.com/images?q=tbn:ANd9GcRbezqZpEuwGSvitKy3wrwnth5kysKdRqBW54cAszm_wiutku3R";
    }
  }

  public validation(): void{
    const formOptions: AbstractControlOptions = {
      validators: ValidatorField.MustMatch('password', 'confirmPassword')
    };

    this.form = this.fb.group({
      titulo: ['', [Validators.required]],      
      userName: [''],
      primeiroNome: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(30)]],
      ultimoNome: ['', [Validators.required, Validators.minLength(4), Validators.maxLength(50)]],
      email: ['', [Validators.required, Validators.email]],
      phoneNumber: ['', [Validators.required]],
      funcao: ['', [Validators.required]],
      descricao: ['', [Validators.required]],
      password: [''],
      confirmPassword: ['']
    }, formOptions);
  }

  public resetForm(event: Event): void{
    event.preventDefault()
    this.form.reset();
  }

  public getUser(): void{
    this.accountService.getUser().subscribe((userProfile: UserProfile) => {
      if (userProfile){
        this.form.patchValue(userProfile);
        this.imagemURL = environment.apiURL + 'resources/images/users/' + userProfile.imagemURL;
      }
    });
  }

  public updateUser(): void{
    this.spinner.show();
    this.user = {...this.form.value};
    //console.log('Dados User:', this.user);
    this.accountService.update(this.user).subscribe({
      next: () => {
        this.toatr.success('Usuário alterado com sucesso')
      },
      error: (error: any) => {
        console.error(error);
        this.toatr.error('Não foi possível alterar o usuário', 'Erro');
      }
    }).add(() => this.spinner.hide());
  }


  public uploadImage(event: any): void {
    const reader = new FileReader();

    reader.onload = (event: any) => this.imagemURL = event.target.result;
    this.file = event.target?.files;
    reader.readAsDataURL(this.file[0]);
    this.postImage();
  }

  public postImage(): void{
    this.spinner.show();
    this.accountService.uploadImage(this.file).subscribe({
      next: (result) => {
        console.log(result);
        this.toatr.success('Imagem atualizada', 'Sucesso');
      },
      error: (error: any) => {
        console.error(error);
        this.toatr.error('Falha ao tentar enviar imagem', 'Erro');
      }
    }).add(() => this.spinner.hide());
  }

  public funcaoValue(event: any): void{
    console.log(event.value);
  }
}
