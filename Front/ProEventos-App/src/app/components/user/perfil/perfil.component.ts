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

  constructor(private fb: FormBuilder, private accountService: AccountService, private spinner: NgxSpinnerService, private toastr: ToastrService) { }

  //public get f(): any{
  //  return this.form.controls;
  //}

  public get isPalestrante() : boolean{
    return this.user.funcao === 'Palestrante';
  }

  ngOnInit(): void {
    //if (this.imagemURL == undefined){
      
    //}
  }    

  public getUser(userProfile: UserProfile): void {    
    this.user = userProfile;
    
    if (userProfile.imagemURL){
      this.imagemURL = environment.apiURL + 'resources/images/users/' + userProfile.imagemURL;
    } else {
      this.imagemURL = "/assets/semImagem.png";//"https://encrypted-tbn2.gstatic.com/images?q=tbn:ANd9GcRbezqZpEuwGSvitKy3wrwnth5kysKdRqBW54cAszm_wiutku3R";
    }
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
        this.toastr.success('Imagem atualizada', 'Sucesso');
      },
      error: (error: any) => {
        console.error(error);
        this.toastr.error('Falha ao tentar enviar imagem', 'Erro');
      }
    }).add(() => this.spinner.hide());
  }


}
