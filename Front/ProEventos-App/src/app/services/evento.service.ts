import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Evento } from '@app/models/Evento';
import { take } from 'rxjs';
import { environment } from '@environments/environment';

@Injectable(
//  {providedIn: 'root'}
)
export class EventoService {
  baseURL = environment.apiURL + 'api/eventos';
  
  constructor(private http: HttpClient) { }

  public getEventos(): Observable<Evento[]>{
    return this.http.get<Evento[]>(this.baseURL);    
  }

  public getEventosByTema(tema: string): Observable<Evento[]>{
    return this.http.get<Evento[]>(`${this.baseURL}/${tema.toLocaleLowerCase()}/tema`).pipe(take(1));
  }

  public getEventoById(id: number): Observable<Evento>{
    return this.http.get<Evento>(`${this.baseURL}/${id}`).pipe(take(1));
  }

  public post(evento: Evento): Observable<Evento>{
    return this.http.post<Evento>(this.baseURL, evento).pipe(take(1));
  }

  public put(evento: Evento): Observable<Evento>{
    return this.http.put<Evento>(`${this.baseURL}/${evento.id}`, evento).pipe(take(1));
  }

  public deleteEvento(id: number): Observable<any>{
    return this.http.delete(`${this.baseURL}/${id}`).pipe(take(1));
  }

  public postUpload(eventoId: number, file: File): Observable<Evento> {
    const fileUpload = file[0] as File;
    const formData = new FormData();
    formData.append('file', fileUpload) ;

    return this.http.post<Evento>(`${this.baseURL}/upload-image/${eventoId}`, formData).pipe(take(1));
  }

}
