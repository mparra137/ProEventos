import { Evento } from "./Evento";
import { UserProfile } from "./identity/UserProfile";
import { RedeSocial } from "./RedeSocial";

export interface Palestrante {
    id: number;
    miniCurriculo: string;
    redesSociais: RedeSocial[];   
    eventos: Evento[];
    user: UserProfile;
}
