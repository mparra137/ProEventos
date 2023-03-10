import { Component, Input, OnInit, TemplateRef } from '@angular/core';
import { FormGroup, FormBuilder, FormControl, AbstractControl, FormArray, Validators } from '@angular/forms';
import { RedeSocial } from '@app/models/RedeSocial';
import { BsModalService, BsModalRef } from 'ngx-bootstrap/modal';
import { RedesocialService } from '@app/services/redesocial.service';
import { NgxSpinnerService } from 'ngx-spinner';
import { ToastrService } from 'ngx-toastr';
import { Router, ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-redes-sociais',
  templateUrl: './redes-sociais.component.html',
  styleUrls: ['./redes-sociais.component.scss']
})
export class RedesSociaisComponent implements OnInit {
  public formRS!: FormGroup;
  public redeSocialAtual: any = {id: 0, nome: '', indice: 0};
  @Input() public eventoId: number = 0;
  private origem!: string;

  constructor(private fb: FormBuilder, public modalService: BsModalService, private redeSocialService: RedesocialService, private spinner: NgxSpinnerService, private toastr: ToastrService, private route: ActivatedRoute) { }

  ngOnInit(): void {
    this.validation();
    this.carregarRedesSociais();    
  }

  public validation(): void{
    this.formRS = this.fb.group({
      redesSociais: this.fb.array([])
    });
  }

  public get redesSociais(): FormArray{
    return this.formRS.get('redesSociais') as FormArray;
  }

  public carregarRedesSociais(): void{    
    this.origem = this.eventoId === 0 ? 'palestrante' : 'evento'; 

    this.redeSocialService.get(this.origem, this.eventoId).subscribe({
      next: (redesSociaisRet: RedeSocial[]) => {
        console.log(redesSociaisRet);
        redesSociaisRet.forEach((rs: RedeSocial) => {
          this.redesSociais.push(this.criarRedeSocial(rs));
        })
      }
    });
  }

  public cssValidator(formControl: FormControl | AbstractControl | null): any{
    return {'is-invalid': formControl?.errors && formControl.touched}
  }

  public criarRedeSocial(redeSocial: RedeSocial): FormGroup{
    return this.fb.group({
      id: [redeSocial.id,],
      nome: [redeSocial.nome, Validators.required],
      url: [redeSocial.url, Validators.required]
    });
  }

  public adicionarRedeSocial(): void{
    this.redesSociais.push(this.criarRedeSocial({id: 0} as RedeSocial));
  }

  public confirmDeleteRedeSocial(): void{
    this.spinner.show();
    this.redeSocialService.delete(this.origem, this.eventoId, this.redeSocialAtual.id).subscribe({
      next: (result: any) => {
        this.redesSociais.removeAt(this.redeSocialAtual.indice);
        this.toastr.success("Rede social excluída", "Sucesso");               
      },
      error: (error: any) => {
        this.toastr.error("Ocorreu erro ao tentar excluir a rede social", "Erro");
        console.log(error);
      }
    }).add(() => this.spinner.hide());
    
    this.modalService.hide();
  }

  public declineDeleteRedeSocial(): void{
    this.modalService.hide();
  }

  public salvarRedesSociais(): void{
    console.log(this.redesSociais.value);
    this.spinner.show();

    this.redeSocialService.save(this.origem, this.eventoId, this.redesSociais.value).subscribe({
      next: () => {
        this.toastr.success("Redes Sociais Salvas", "Sucesso");
      },
      error: (error: any) => {
        this.toastr.error("Ocorreu erro ao tentar salvar as redes sociais do palestrante", "Erro");
        console.error(error);
      }
    }).add(() => this.spinner.hide());
  }

  public removerRedeSocial(template: TemplateRef<any>, indice: number): void{
    this.redeSocialAtual.id = this.redesSociais.get(indice+'.id')?.value;
    this.redeSocialAtual.nome = this.redesSociais.get(indice+'.nome')?.value;
    this.redeSocialAtual.indice = indice;

    this.modalService.show(template, {class: 'modal-sm'});
  }

  public retornaTitulo(nome: string) : string {
    return nome === null || nome === '' ? 'Rede Social': nome;
  }

}
