import { Lote } from "./Lote";
import { Palestrante } from "./Palestrante";
import { RedeSocial } from "./RedeSocial";
import { Usuario } from "./Usuario";

export interface Evento {
    id: number ;
    local: string ;
    dataEvento?: Date;
    tema: string ;
    qtdPessoas: number ;
    imagemURL : string ;
    email: string ;
    telefone: string;
    userId: number,
    usuario?: Usuario,
    lotes?: Lote[];
    redesSociais?: RedeSocial[];
    palestrantesEventos?: Palestrante[];

}
