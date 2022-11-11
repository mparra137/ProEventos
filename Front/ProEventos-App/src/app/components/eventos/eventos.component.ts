import { Component, OnInit, TemplateRef } from '@angular/core';
import { SafeResourceUrl } from '@angular/platform-browser';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { ToastrService } from 'ngx-toastr';
import { Observable } from 'rxjs';
import { Evento } from '../../models/Evento';
import { EventoService } from '../../services/evento.service';
import { NgxSpinnerService } from "ngx-spinner";


@Component({
  selector: 'app-eventos',
  templateUrl: './eventos.component.html',
  styleUrls: ['./eventos.component.scss']
})
export class EventosComponent implements OnInit {
  public modalRef?: BsModalRef


  public eventos: Evento[] = [];
  public eventosFiltrados: Evento[] = [];
  public widthImg: number = 150;
  public marginImg: number = 2;
  public showImg: boolean = true;
  private _filtroLista: string = "";

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
              private spinner: NgxSpinnerService) { }

  public ngOnInit(): void {
    /** spinner starts on init */
    this.spinner.show();
    this.getEventos();            
  }

  public getEventos(): void{
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


  public openModal(template: TemplateRef<any>): void {
    this.modalRef = this.modalService.show(template, {class: 'modal-sm'});
  }
 
  public confirm(): void {  
    console.log("Sim")  
    this.showSuccess()
    this.modalRef?.hide();
  }
 
  public decline(): void {  
    console.log("Não")   
    this.modalRef?.hide();
  }

  showSuccess() {
    this.toastr.success('Evento excluído', 'Sucesso!');
  }
}
