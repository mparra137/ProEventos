import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PalestranteDetalheComponent } from './palestrante-detalhe.component';

describe('PalestranteDetalheComponent', () => {
  let component: PalestranteDetalheComponent;
  let fixture: ComponentFixture<PalestranteDetalheComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PalestranteDetalheComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PalestranteDetalheComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
