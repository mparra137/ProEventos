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
    public class PalestrantePersist : IPalestrantesPersist
    {
        private readonly ProEventosContext context;
        
        public PalestrantePersist(ProEventosContext _context)
        {
            context = _context;            
        }       

        public async Task<Palestrante> GetPalestranteByIdAsync(int palestranteId, bool includeEventos = false)
        {
            IQueryable<Palestrante> query = context.Palestrantes.Include(p => p.RedesSociais);
            if (includeEventos){
                query = query.Include(p => p.PalestrantesEventos).ThenInclude(p => p.Evento);
            }
            query = query.Where(p => p.Id == palestranteId);

            return await query.FirstOrDefaultAsync();
        }

        public async Task<Palestrante[]> GetAllPalestrantesAsync(bool includeEventos = false)
        {
            IQueryable<Palestrante> query = context.Palestrantes.Include(r => r.RedesSociais);
            if (includeEventos){
                query = query.Include(pe => pe.PalestrantesEventos).ThenInclude(e => e.Evento);
            }
            query = query.OrderBy(p => p.Id);

            return await query.ToArrayAsync();
        }

        public async Task<Palestrante[]> GetAllPalestrantesByNomesync(string nome, bool includeEventos)
        {
            IQueryable<Palestrante> query = context.Palestrantes.Include(r => r.RedesSociais);
            if (includeEventos){
                query = query.Include(pe => pe.PalestrantesEventos).ThenInclude(e => e.Evento);
            }
            query = query.OrderBy(p => p.Id).Where(p => p.User.PrimeiroNome.ToLower().Contains(nome.ToLower()));

            return await query.ToArrayAsync();
        }

        

        
    }
}