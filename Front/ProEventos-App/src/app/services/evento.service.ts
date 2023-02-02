import { HttpClient, HttpHeaders, HttpParams, HttpResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Evento } from '@app/models/Evento';
import { take, map } from 'rxjs';
import { environment } from '@environments/environment';
import { PaginatedResult } from '@app/models/Pagination';

@Injectable()
export class EventoService {
  baseURL = environment.apiURL + 'api/eventos';  
  
  constructor(private http: HttpClient) { }

  public getEventos(page?: number, itemsPerPage?: number, term?: string): Observable<PaginatedResult<Evento[]>>{
    const paginatedResult: PaginatedResult<Evento[]> = new PaginatedResult<Evento[]>();

    let params: HttpParams = new HttpParams();

    if (page !== null && itemsPerPage !== null){
      params = params.append('pageSize', itemsPerPage!.toString()!);
      params = params.append('pageNumber', page!.toString());
    }
    if (term)
      params = params.append('term', term);

    return this.http.get<Evento[]>(`${this.baseURL}`, {observe: 'response', params})
      .pipe(take(1), 
            map((response) => {
              paginatedResult.result = response.body as Evento[];
              if (response.headers.has('pagination')){
                paginatedResult.pagination = JSON.parse(response.headers.get('pagination') || '{}');
              }
              return paginatedResult;
            }));    
  }

  public getEventos2(pageSize: number, pageNumber: number): Observable<HttpResponse<Evento[]>>{
    return this.http.get<Evento[]>(`${this.baseURL}?pagesize=${pageSize}&pagenumber=${pageNumber}`, {observe : 'response'}).pipe(take(1));    
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
