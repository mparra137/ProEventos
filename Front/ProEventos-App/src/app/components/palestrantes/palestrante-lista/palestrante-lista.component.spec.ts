import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PalestranteListaComponent } from './palestrante-lista.component';

describe('PalestranteListaComponent', () => {
  let component: PalestranteListaComponent;
  let fixture: ComponentFixture<PalestranteListaComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PalestranteListaComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PalestranteListaComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
