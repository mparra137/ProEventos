import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl, Validators, FormBuilder } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';

import { EventoService } from '@app/services/evento.service';
import { Evento } from '@app/models/Evento';

import { BsLocaleService } from 'ngx-bootstrap/datepicker';
import { NgxSpinnerService } from 'ngx-spinner';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-evento-detalhe',
  templateUrl: './evento-detalhe.component.html',
  styleUrls: ['./evento-detalhe.component.scss']
})
export class EventoDetalheComponent implements OnInit {

  public form!: FormGroup;
  public evento = {} as Evento;
  private estadoSalvar: string = 'post';
  

  constructor(private fb: FormBuilder, 
              private localeService: BsLocaleService, 
              private route: ActivatedRoute, 
              private eventoService: EventoService,
              private spinner: NgxSpinnerService,
              private toastr: ToastrService) {
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

  ngOnInit(): void {     
    this.carregarEvento();
    this.Validation();
  }

  public carregarEvento(): void{
    const eventoIdParam = this.route.snapshot.paramMap.get('id');

    if (eventoIdParam !== null){
      this.estadoSalvar = 'put';

      this.spinner.show();  
      this.eventoService.getEventoById(+eventoIdParam).subscribe({
        next: (evento: Evento) => {
          this.evento = {...evento};
          this.form.patchValue(this.evento);
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
      imagemURL : ['', [Validators.required]],
      email: ['', [Validators.required, Validators.email]],
      telefone: ['', [Validators.required]]
    });    
  }

  public resetForm(): void{
    this.form.reset();
  }

  public cssValidator(formControl: FormControl): any{
    return {'is-invalid': formControl.errors && formControl.touched}
  }

  public getEvento(id: number): void{
    this.eventoService.getEventoById(id).subscribe(
      (eventoResp: Evento) => {
        this.evento = eventoResp;        
      }
    );
  }

  public salvarAlteracao(): void{
    this.spinner.show();
    if (this.form.valid){      
      this.evento = this.estadoSalvar === 'post' ? {...this.form.value} : {id: this.evento.id, ...this.form.value};

      this.eventoService[this.estadoSalvar](this.evento).subscribe({
        next: (evento: Evento) => {          
          this.toastr.success("Evento salvo com sucesso", "Sucesso")
        },
        error: (error: any) => {          
          console.error(error);
          this.toastr.error("Erro ao salvar o evento", "Erro")
        }
      }).add(() => this.spinner.hide());
      
    }
  }
}
