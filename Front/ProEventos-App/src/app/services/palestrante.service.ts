import { HttpClient, HttpParams, HttpResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { PaginatedResult } from '@app/models/Pagination';
import { Palestrante } from '@app/models/Palestrante';
import { environment } from '@environments/environment';
import { map, Observable, take } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class PalestranteService {
  baseURL = environment.apiURL + 'api/palestrantes';  
  
  constructor(private http: HttpClient) { }

  public getAllPalestrantes(page?: number, itemsPerPage?: number, term?: string): Observable<PaginatedResult<Palestrante[]>>{
    const paginatedResult: PaginatedResult<Palestrante[]> = new PaginatedResult<Palestrante[]>();

    let params: HttpParams = new HttpParams();

    if (page !== null && itemsPerPage !== null){
      params = params.append('pageSize', itemsPerPage!.toString()!);
      params = params.append('pageNumber', page!.toString());
    }
    if (term)
      params = params.append('term', term);

    return this.http.get<Palestrante[]>(`${this.baseURL}/all`, {observe: 'response', params})
      .pipe(take(1), 
            map((response) => {
              paginatedResult.result = response.body as Palestrante[];
              if (response.headers.has('pagination')){
                paginatedResult.pagination = JSON.parse(response.headers.get('pagination') || '{}');
              }
              return paginatedResult;
            }));    
  }

  public getPalestrantes2(pageSize: number, pageNumber: number): Observable<HttpResponse<Palestrante[]>>{
    return this.http.get<Palestrante[]>(`${this.baseURL}?pagesize=${pageSize}&pagenumber=${pageNumber}`, {observe : 'response'}).pipe(take(1));    
  }

  public getPalestrante(): Observable<Palestrante>{
    return this.http.get<Palestrante>(this.baseURL).pipe(take(1));
  }

  public post(): Observable<Palestrante>{
    return this.http.post<Palestrante>(this.baseURL, {} as Palestrante).pipe(take(1));
  }

  public put(palestrante: Palestrante): Observable<Palestrante>{
    return this.http.put<Palestrante>(`${this.baseURL}`, palestrante).pipe(take(1));
  }

}
