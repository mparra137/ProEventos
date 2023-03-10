import { Component, OnInit, TemplateRef } from '@angular/core';
import { FormGroup, FormControl, Validators, FormBuilder, FormArray, Form, AbstractControl } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';

import { EventoService } from '@app/services/evento.service';
import { Evento } from '@app/models/Evento';

import { BsLocaleService } from 'ngx-bootstrap/datepicker';
import { NgxSpinnerService } from 'ngx-spinner';
import { ToastrService } from 'ngx-toastr';
import { Lote } from '@app/models/Lote';
import { LoteService } from '@app/services/lote.service';
import { BsModalService } from 'ngx-bootstrap/modal';
import { environment } from '@environments/environment';

@Component({
  selector: 'app-evento-detalhe',
  templateUrl: './evento-detalhe.component.html',
  styleUrls: ['./evento-detalhe.component.scss']
})
export class EventoDetalheComponent implements OnInit {

  public eventoId: number = 0
  public form!: FormGroup;
  public evento = {} as Evento;
  private estadoSalvar: string = 'post';
  public loteAtual: any = {id: 0, nome: '', indice: 0};
  public imagemURL: string = "assets/upload.png";
  public file!: File;

  constructor(private fb: FormBuilder, 
              private localeService: BsLocaleService, 
              private route: ActivatedRoute,
              private router: Router, 
              private eventoService: EventoService,
              private spinner: NgxSpinnerService,
              private toastr: ToastrService,
              private loteService: LoteService,
              private modalService: BsModalService) {

    localeService.use('pt-br')
  }

  public get f(): any{
    return this.form.controls;
  }

  public get bsConfig(): any{
    return {
      isAnimated: true, 
      dateInputFormat: 'DD/MM/YYYY hh:mm a', 
      adaptivePosition: true,
      containerClass: 'theme-default',
      showWeekNumbers: false
    }
  }

  public get bsConfigLote(): any{
    return {
      isAnimated: true, 
      dateInputFormat: 'DD/MM/YYYY', 
      adaptivePosition: true,
      containerClass: 'theme-default',
      showWeekNumbers: false
    }
  }

  public get lotes(): FormArray{
    return this.form.get('lotes') as FormArray;
  }

  public get modoEditar(): boolean{
    return this.estadoSalvar === 'put';
  }

  ngOnInit(): void {     
    this.carregarEvento();
    this.Validation();
  }

  public carregarEvento(): void{
    this.eventoId = +this.route.snapshot.paramMap.get('id')!;

    if (this.eventoId !== null && this.eventoId > 0){
      this.estadoSalvar = 'put';

      this.spinner.show();  
      this.eventoService.getEventoById(this.eventoId).subscribe({
        next: (evento: Evento) => {
          this.evento = {...evento};
          this.form.patchValue(this.evento);    
          
          if (this.evento.imagemURL !== '')
            this.imagemURL = environment.apiURL + 'resources/images/' + this.evento.imagemURL;

          this.evento.lotes?.forEach((lote: Lote) => {
             this.lotes.push(this.criarLote(lote));
          });          
        },
        error: (error :  any) => {
          console.error(error);
          this.spinner.hide();
          this.toastr.error('Erro ao tentar carregar evento', 'Erro');
        },
        complete: () => {
          this.spinner.hide();
        }
      });
    }
  }

  public Validation(): void{
    this.form = this.fb.group({
      tema: ['', [Validators.required, Validators.minLength(4), Validators.maxLength(50)]],
      local: ['', [Validators.required]],
      dataEvento: ['', [Validators.required]],
      qtdPessoas: ['', [Validators.required, Validators.max(120000)]],
      imagemURL : [''],
      email: ['', [Validators.required, Validators.email]],
      telefone: ['', [Validators.required]],
      lotes: this.fb.array([])
    });    
  }

  public criarLote(lote: Lote): FormGroup{
    return this.fb.group({
      id: [lote.id],
      nome: [lote.nome, Validators.required],
      quantidade: [lote.quantidade, Validators.required],
      preco: [lote.preco, Validators.required],
      dataInicio: [lote.dataInicio],
      dataFim: [lote.dataFim]
    });
  }

  public adicionarLote() : void{
    this.lotes.push(this.criarLote({id: 0} as Lote));
  }

  public resetForm(): void{
    this.form.reset();
  }

  public cssValidator(formControl: FormControl | AbstractControl | null): any{
    return {'is-invalid': formControl?.errors && formControl.touched}
  }

  public getEvento(id: number): void{
    this.eventoService.getEventoById(id).subscribe(
      (eventoResp: Evento) => {
        this.evento = eventoResp;        
      }
    );
  }

  public salvarEvento(): void{
    if (this.form.valid){      
      this.spinner.show();
      this.evento = this.estadoSalvar === 'post' ? {...this.form.value} : {id: this.evento.id, ...this.form.value};

      this.eventoService[this.estadoSalvar](this.evento).subscribe({
        next: (evento: Evento) => {          
          this.toastr.success("Evento salvo com sucesso", "Sucesso")
          this.router.navigate([`eventos/detalhe/${evento.id}`]);
        },
        error: (error: any) => {          
          console.error(error);
          this.toastr.error("Erro ao salvar o evento", "Erro")
        }
      }).add(() => {
        this.spinner.hide();        
      });      
    }
  }

  public salvarLotes(): void{   
    //console.log(this.form.value.lotes)
    if (this.lotes.valid){
      this.spinner.show();
      this.loteService.saveLotes(this.eventoId,this.form.value.lotes).subscribe({
        next: () => {
          this.toastr.success('Lote adicionado com sucesso', 'Sucesso!')
        },
        error: (error: any) => {
          this.toastr.error('Erro ao incluir novos lotes', 'Erro!');
          console.error(error);
        }
      }).add(() => this.spinner.hide());
      
    }
  }

  public removeLote(template: TemplateRef<any>, indice: number): void{
    this.loteAtual.id = this.lotes.get(indice+'.id')?.value;
    this.loteAtual.nome = this.lotes.get(indice+'.nome')?.value;
    this.loteAtual.indice = indice;

    //console.log(this.loteAtual);

    this.modalService.show(template, {class: 'modal-sm'});
  }

  public confirmDeleteLote(): void{
    this.modalService.hide();
    this.spinner.show();

    if (this.loteAtual.id > 0){ 
      this.loteService.deleteLote(this.eventoId, this.loteAtual.id).subscribe({
        next: () => {
          this.toastr.success('Lote excluído com sucesso', 'Sucesso');
          this.removeFromLoteArray()
        }, 
        error: (error: any) => {
          console.error(error);
          this.toastr.error(`Erro ao tentar excluir o lote ${this.loteAtual.nome}`, 'Erro');    
        }
      }).add(() => this.spinner.hide());
    } else {
      this.removeFromLoteArray();
      this.spinner.hide();
    }        
  }

  public declineDeleteLote(): void{
    this.modalService.hide();
  }

  public removeFromLoteArray(): void{
    this.lotes.removeAt(this.loteAtual.indice);
  }

  public retornaNomeLote(nome: string) : string{
      return nome === null || nome === ''? 'Nome do Lote' : nome;
  }

  public onFileChange(event: any){
    const reader = new FileReader();

    reader.onload = (event: any) => this.imagemURL = event.target.result;

    this.file = event.target?.files;
    reader.readAsDataURL(this.file[0])

    this.uploadImagem();
  }

  public uploadImagem(): void{
    this.spinner.show();
    this.eventoService.postUpload(this.eventoId, this.file).subscribe({
      next: () => {
        
        this.carregarEvento();
        this.toastr.success('Upload da imagem realizado com sucesso.', 'Sucesso');
      },
      error: (error: any) => {
        console.error(error);
        this.toastr.error('Erro ao fazer o upload da imagem', 'Erro!'); 
      }
    }).add(() => this.spinner.hide());
  }
}
