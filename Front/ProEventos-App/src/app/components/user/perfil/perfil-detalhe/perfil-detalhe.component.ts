import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { FormGroup, FormBuilder, Validators, AbstractControlOptions } from '@angular/forms';
import { UserProfile } from '@app/models/identity/UserProfile';
import { ValidatorField } from '@app/helpers/ValidatorField';
import { AccountService } from '@app/services/account.service';
import { PalestranteService } from '@app/services/palestrante.service';
import { environment } from '@environments/environment';
import { NgxSpinnerService } from 'ngx-spinner';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-perfil-detalhe',
  templateUrl: './perfil-detalhe.component.html',
  styleUrls: ['./perfil-detalhe.component.css']
})
export class PerfilDetalheComponent implements OnInit {
  public user = {} as UserProfile;
  public form!: FormGroup;
  //public imagemURL!: string;
  public file!: File;
  @Output() public EmitUser: EventEmitter<UserProfile> = new EventEmitter();

  constructor(private fb: FormBuilder, 
              private accountService: AccountService,
              private palestranteService: PalestranteService, 
              private spinner: NgxSpinnerService, 
              private toastr: ToastrService) { }

  public get f(): any{
    return this.form.controls;
  }

  ngOnInit() {
    this.validation();
    this.getUser();
  }


  public funcaoValue(event: any): void{
    console.log(event.value);
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
      confirmPassword: [''],
      imagemURL: ['']
    }, formOptions);
  }

  public resetForm(event: Event): void{
    event.preventDefault()
    this.form.reset();
  }

  public getUser(): void{
    this.accountService.getUser().subscribe((userProfile: UserProfile) => {
      if (userProfile){    
        this.changeValueForm();       
        this.user = userProfile;
        this.form.patchValue(userProfile);
             
        //this.imagemURL = environment.apiURL + 'resources/images/users/' + userProfile.imagemURL;        
      }
    });
  }

  public changeValueForm(){
    this.form.valueChanges.subscribe(() => this.EmitUser.emit({... this.form.value}))
  }

  public updateUser(): void{
    this.spinner.show();
    this.user = {...this.form.value};
    //console.log('Dados User:', this.user);

    if (this.f.funcao.value === 'Palestrante'){
      this.palestranteService.post().subscribe({
        next: () => {
          this.toastr.success("Função palestrante ativada", "Sucesso");
        },
        error: (error : any) => {
          this.toastr.error("Erro ao tentar ativar o palestrante", "Erro");
          console.error(error);
        }
      });
    }

    this.accountService.update(this.user).subscribe({
      next: () => {
        this.toastr.success('Usuário alterado com sucesso')
      },
      error: (error: any) => {
        console.error(error);
        this.toastr.error('Não foi possível alterar o usuário', 'Erro');
      }
    }).add(() => this.spinner.hide());
  }
}
