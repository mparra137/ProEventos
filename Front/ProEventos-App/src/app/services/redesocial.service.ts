import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { RedeSocial } from '@app/models/RedeSocial';
import { Observable, take } from 'rxjs';
import { environment } from '@environments/environment';


@Injectable()
export class RedesocialService {
  private baseURL: string = environment.apiURL + 'api/redessociais';

  constructor(private http: HttpClient) { }

  public save(origem: string, id: number, models: RedeSocial[]): Observable<RedeSocial[]>{
    let urlAPI = 
      id === 0 
      ? `${this.baseURL}/${origem}`
      : `${this.baseURL}/${origem}/${id}`;

    
    return this.http.post<RedeSocial[]>(urlAPI, models).pipe(take(1));
  }

  /**
   * 
   * @param origem Passar origem como 'palestrante' ou 'evento'
   * @param id Id do evento quando origem for 'evento'
   * @returns Lista de redes sociais
   */

  public get(origem: string, id: number): Observable<RedeSocial[]>{
    let urlEndPoint = id === 0 
      ? `${this.baseURL}/${origem}`
      : `${this.baseURL}/${origem}/${id}`;   

    return this.http.get<RedeSocial[]>(urlEndPoint).pipe(take(1));
  }

  public delete(origem: string, id: number, redeSocialId: number): Observable<any>{
    let urlEndPoint: string = 
      id === 0 
      ? `${this.baseURL}/${origem}/${redeSocialId}`
      : `${this.baseURL}/${origem}/${id}/${redeSocialId}`;        

    return this.http.delete<any>(urlEndPoint).pipe(take(1));
  }


}
