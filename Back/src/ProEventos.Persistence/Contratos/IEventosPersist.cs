using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProEventos.Domain;
using ProEventos.Persistence.Models;

namespace ProEventos.Persistence.Contratos
{
    public interface IEventoPersist
    {       
        //EVENTOS
        //Task<Evento[]> GetAllEventosByTemaAsync(int userId, string tema, bool includePalestrantes = false);
        Task<PageList<Evento>> GetAllEventosAsync(int userId, PageParams pageParams, bool includePalestrantes = false);
        Task<Evento> GetEventoByIdAsync(int userId, int eventoId, bool includePalestrantes = false);        
        
    }
}