import { Component, OnInit } from '@angular/core';
import { PaginatedResult, Pagination } from '@app/models/Pagination';
import { Palestrante } from '@app/models/Palestrante';
import { PalestranteService } from '@app/services/palestrante.service';
import { BsModalService } from 'ngx-bootstrap/modal';
import { NgxSpinnerService } from 'ngx-spinner';
import { ToastrService } from 'ngx-toastr';
import { debounceTime, distinctUntilChanged, Subject } from 'rxjs';
import { environment } from '@environments/environment';
import { PageChangedEvent } from 'ngx-bootstrap/pagination';

@Component({
  selector: 'app-palestrante-lista',
  templateUrl: './palestrante-lista.component.html',
  styleUrls: ['./palestrante-lista.component.scss']
})
export class PalestranteListaComponent implements OnInit {
  private termoBuscaChanged: Subject<string> = new Subject();
  public pagination = {} as Pagination;
  public palestrantes: Palestrante[] = [];
  public imagesPath = environment.apiURL + "resources/images/users/";
    

  constructor(private palestranteService: PalestranteService, 
              private modal: BsModalService, 
              private spinner: NgxSpinnerService, 
              private toastr: ToastrService
  ) { }

  public ngOnInit(): void {
    this.pagination = {currentPage: 1, itemsPerPage: 3, totalItems: 1, totalPages: 1};
    this.carregarPalestrantes();
  }

  public filtrarPalestrantes(evt: any): void{
    if (!this.termoBuscaChanged.observed){
      this.termoBuscaChanged.pipe(debounceTime(1000), distinctUntilChanged()).subscribe(
        filtrarPor => {
          this.spinner.show();
          this.palestranteService.getAllPalestrantes(this.pagination.currentPage, this.pagination.itemsPerPage, filtrarPor).subscribe({
            next: (paginatedResult: PaginatedResult<Palestrante[]>) => {
              this.palestrantes = paginatedResult.result;
              this.pagination = paginatedResult.pagination;
            },
            error: (error: any) => {
              console.error(error);
              this.toastr.error("Erro ao filtrar palestrantes", "Erro");
            }
          }).add(() => this.spinner.hide());
        });
    }
    
    this.termoBuscaChanged.next(evt.value);        
  }  

  private carregarPalestrantes(): void{
    this.spinner.show();
    this.palestranteService.getAllPalestrantes(this.pagination.currentPage, this.pagination.itemsPerPage).subscribe({
      next: (paginatedResult: PaginatedResult<Palestrante[]>) => {
        this.palestrantes = paginatedResult.result;
        this.pagination = paginatedResult.pagination;   
        console.log(this.pagination);     
      },
      error: (error: any) => {
        this.toastr.error("Erro ao tentar obter os palestrantes", "Erro");
        console.error(error);
      }
    }).add(() => this.spinner.hide());
  }

  public getImageURL(imagemURL: string): string{
    if (imagemURL)
      return environment.apiURL + 'resources/images/users/'+ imagemURL; 
    else
      return 'assets/semImagem.png';
  }

  public pageChanged(event: PageChangedEvent): void{    
    this.pagination.currentPage = event.page;
    this.carregarPalestrantes();
  }

}
