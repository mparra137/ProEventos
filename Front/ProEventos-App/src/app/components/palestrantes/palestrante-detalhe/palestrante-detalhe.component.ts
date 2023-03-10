import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, FormControl, AbstractControl, Validators } from '@angular/forms';
import { Palestrante } from '@app/models/Palestrante';
import { PalestranteService } from '@app/services/palestrante.service';
import { NgxSpinnerService } from 'ngx-spinner';
import { ToastrService } from 'ngx-toastr';
import { debounceTime, map, tap } from 'rxjs';

@Component({
  selector: 'app-palestrante-detalhe',
  templateUrl: './palestrante-detalhe.component.html',
  styleUrls: ['./palestrante-detalhe.component.scss']
})
export class PalestranteDetalheComponent implements OnInit {
  public form!: FormGroup;
  public corDaDescricao: string = "";
  public situacaoDoForm: string = "";

  constructor(private fb: FormBuilder,
              public palestranteService: PalestranteService,
              public toastr: ToastrService,
              public spinner: NgxSpinnerService
              ){ }

  ngOnInit(): void {
    this.validation();
    this.carregarMinicurriculo();
    this.verificaForm();
  }

  public validation(): void{
    this.form = this.fb.group({
      miniCurriculo: ['', [Validators.required]]
    });
  }

  public verificaForm(): void{
    this.form.valueChanges.pipe(
      (map(() => {
        this.situacaoDoForm = 'Minicurrículo está sendo atualizado';
        this.corDaDescricao = 'text-warning';
      })),
      debounceTime(1000),
      tap(() => this.spinner.show())
    ).subscribe({
      next: () => {
        this.palestranteService.put({...this.form.value}).subscribe({
          next: () => {
            this.situacaoDoForm = "Minicurriculo foi atualizado";
            this.corDaDescricao = "text-success"

            setTimeout(() => {
              this.situacaoDoForm = "Minicurriculo foi carregado";
              this.corDaDescricao = "text-muted";
            }, 2000);
          },
          error: (error: any) => {
            this.toastr.error("Erro ao atualizar o minicurriculo", "Erro");
            console.error(error);
            this.situacaoDoForm = "Erro ao atualizr o minicurrículo";
            this.corDaDescricao = "text-danger";
          }
        }).add(() => this.spinner.hide());
      }
    });
  }

  public carregarMinicurriculo(): void{
    this.spinner.show();
    this.palestranteService.getPalestrante().subscribe({
      next: (palestrante: Palestrante) => {
        this.form.patchValue({'miniCurriculo': palestrante.miniCurriculo});
      },
      error: (error: any) => {
        console.error(error);
        this.toastr.error("Erro ao carregar o minicurriculo do palestrante", "Erro");
      }
    }).add(() => this.spinner.hide());
  }

  public get f(): any{
    return this.form.controls;
  }

}
