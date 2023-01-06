using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProEventos.Domain;
using ProEventos.Persistence.Contextos;
using ProEventos.Persistence.Contratos;

namespace ProEventos.Persistence
{
    public class EventoPersist : IEventoPersist
    {
        private readonly ProEventosContext context;
        
        public EventoPersist(ProEventosContext _context)
        {
            context = _context;    
            //O código a seguir altera o tracking de instancias para todos os gets ... abaixo foi adicionar AsNoTracking por busca
            //context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;        
        }       

        public async Task<Evento[]> GetAllEventosAsync(int userId, bool includePalestrantes = false)
        {
            IQueryable<Evento> query = context.Eventos.Include(evento => evento.Lotes).Include(evento => evento.RedesSociais);
            if (includePalestrantes){
                query = query.Include(evento => evento.PalestrantesEventos).ThenInclude(pe => pe.Palestrante);
            }

            return await query.AsNoTracking().Where(e => e.UserId == userId).OrderBy(e => e.Id).ToArrayAsync();
        }

        public async Task<Evento> GetEventoByIdAsync(int userId, int eventoId, bool includePalestrantes = false)
        {
            IQueryable<Evento> query = context.Eventos.Include(evento => evento.Lotes).Include(evento => evento.RedesSociais);
            if (includePalestrantes){
                query = query.Include(evento => evento.PalestrantesEventos).ThenInclude(pe => pe.Palestrante);
            }

            if (includePalestrantes){
                query = query.Include(pe => pe.PalestrantesEventos).ThenInclude(p => p.Palestrante);
            }

            query = query.AsNoTracking().OrderBy(e => e.Id).Where(e => e.Id == eventoId && e.UserId == userId);

            return await query.FirstOrDefaultAsync();        
        }

        

        public async Task<Evento[]> GetAllEventosByTemaAsync(int userId, string tema, bool includePalestrantes = false)
        {
            IQueryable<Evento> query = context.Eventos.Include(evento => evento.Lotes).Include(evento => evento.RedesSociais);
            if (includePalestrantes){
                query = query.Include(evento => evento.PalestrantesEventos).ThenInclude(pe => pe.Palestrante);
            }

            return await query.AsNoTracking().OrderBy(e => e.Id).Where(evento => evento.Tema.ToLower().Contains(tema.ToLower()) && evento.UserId == userId).ToArrayAsync();
        }             
        
    }
}