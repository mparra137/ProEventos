import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { SafeResourceUrl } from '@angular/platform-browser';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-eventos',
  templateUrl: './eventos.component.html',
  styleUrls: ['./eventos.component.scss']
})
export class EventosComponent implements OnInit {

  public eventos: any = [];
  public eventosFiltrados: any = [];
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

  filtrarEventos(filtrarPor: string): any{
    filtrarPor = filtrarPor.toLocaleLowerCase();
    return this.eventos.filter(
      (evento: { tema: string; local: string}) => 
      evento.tema.toLocaleLowerCase().indexOf(filtrarPor) !== -1 ||                                     
      evento.local.toLocaleLowerCase().indexOf(filtrarPor) !== -1
    )
  }

  

  constructor(private http: HttpClient) { }

  ngOnInit(): void {
    this.getEventos();
  }

  public getEventos(): void{
    this.http.get("https://localhost:5001/api/eventos").subscribe({
      next: ((response: any) => {
        this.eventos = response;
        this.eventosFiltrados = this.eventos;
      }),
      error: ((error) => console.log(error))
    });
  }

  public showImageCollumn(): void{
    this.showImg = !this.showImg;
  }

}
