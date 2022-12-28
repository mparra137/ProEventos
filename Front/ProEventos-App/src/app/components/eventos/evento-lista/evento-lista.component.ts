import { Component, OnInit, TemplateRef } from '@angular/core';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { NgxSpinnerService } from 'ngx-spinner';
import { ToastrService } from 'ngx-toastr';
import { Router } from '@angular/router';
import { Evento } from '@app/models/Evento';
import { EventoService } from '@app/services/evento.service';
import { environment } from '@environments/environment';

@Component({
  selector: 'app-evento-lista',
  templateUrl: './evento-lista.component.html',
  styleUrls: ['./evento-lista.component.scss']
})
export class EventoListaComponent implements OnInit {

  public modalRef?: BsModalRef
  public eventos: Evento[] = [];
  public eventosFiltrados: Evento[] = [];
  public widthImg: number = 150;
  public marginImg: number = 2;
  public showImg: boolean = true;
  private _filtroLista: string = "";
  public temaMsg: string = "";
  public eventoId: number = 0;

  public get filtroLista(): string{
    return this._filtroLista;
  }
  public set filtroLista(value: string){
    this._filtroLista = value;
    this.eventosFiltrados = this._filtroLista ? this.filtrarEventos(this._filtroLista) : this.eventos
  } 

  public filtrarEventos(filtrarPor: string): Evento[]{
    filtrarPor = filtrarPor.toLocaleLowerCase();
    return this.eventos.filter(
      (evento: { tema: string; local: string}) => 
      evento.tema.toLocaleLowerCase().indexOf(filtrarPor) !== -1 ||                                     
      evento.local.toLocaleLowerCase().indexOf(filtrarPor) !== -1
    )
  }  

  constructor(private eventoService: EventoService, 
              private modalService: BsModalService, 
              private toastr: ToastrService, 
              private spinner: NgxSpinnerService,
              private router: Router) { }

  public ngOnInit(): void {
    /** spinner starts on init */
    this.spinner.show();
    this.carregarEventos();            
  }

  public carregarEventos(): void{
    this.eventoService.getEventos().subscribe({
      next: ((eventos: Evento[]) => {        
        this.eventos = eventos;
        this.eventosFiltrados = this.eventos
      }),
      error: ((error) => {
        this.toastr.error('Erro ao carregar os eventos.', 'Erro!');
        this.spinner.hide();
      }),
      complete: (() => { this.spinner.hide(); })
    })    
  }

  public showImageCollumn(): void{
    this.showImg = !this.showImg;
  }


  public openModal(template: TemplateRef<any>, e: Event, temaMsg: string, id: number): void {
    e.stopPropagation();
    this.temaMsg = temaMsg;
    this.eventoId = id;
    this.modalRef = this.modalService.show(template, {class: 'modal-sm'});
  }
 
  public confirm(): void {  
    this.modalRef?.hide();
    this.spinner.show();

    this.eventoService.deleteEvento(this.eventoId).subscribe({
      next: (result: string) => {
          //console.log(result);
          this.spinner.hide();
          this.showSuccess();
          this.carregarEventos();
        
      },
      error: (error: any) => {
        console.error(error);
        this.spinner.hide();
        this.toastr.error(`Erro ao tentar excluir o evento ${this.temaMsg}`, "Erro");
      },
      complete:() => {
        this.spinner.hide();
      }
    });       
   
  }
 
  public decline(): void {  
   
    this.modalRef?.hide();
  }

  showSuccess() {
    this.toastr.success('Evento excluído', 'Sucesso!');
  }

  public DetalheEvento(id: number): void{
    this.router.navigate([`/eventos/detalhe/${id}`]);
  }

  public returnImageLink(imagemURL): string{
    return (imagemURL !== '' ? `${environment.apiURL}resources/images/${imagemURL}`: 'assets/semImagem.png');
  }

}
