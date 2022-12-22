using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProEventos.Persistence.Contratos;
using ProEventos.Persistence.Contextos;
using ProEventos.Domain;
using Microsoft.EntityFrameworkCore;

namespace ProEventos.Persistence
{  
    public class LotePersist : ILotePersist
    {
        private readonly ProEventosContext context;
        public LotePersist(ProEventosContext _context)
        {
            this.context = _context;            
        }

        public async Task<Lote[]> GetLotesByEventoIdAsync(int eventoId){
            IQueryable<Lote> query = context.Lotes.Where(e => e.EventoId == eventoId);
            
            return await query.AsNoTracking().OrderBy(lote => lote.Id).ToArrayAsync();
        }

        public async Task<Lote> GetLoteByIdsAsync(int eventoId, int id){
            IQueryable<Lote> query = context.Lotes;

            query = query.AsNoTracking().Where(lote => lote.EventoId == eventoId && lote.Id == id);

            return await query.FirstOrDefaultAsync();
        }
    }
}