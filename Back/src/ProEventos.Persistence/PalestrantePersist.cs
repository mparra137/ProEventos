using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProEventos.Domain;
using ProEventos.Persistence.Contextos;
using ProEventos.Persistence.Contratos;
using ProEventos.Persistence.Models;

namespace ProEventos.Persistence
{
    public class PalestrantePersist : GeralPersist, IPalestrantesPersist
    {
        private readonly ProEventosContext context;
        
        public PalestrantePersist(ProEventosContext _context) : base(_context)
        {
            context = _context;            
        }       

        public async Task<Palestrante> GetPalestranteByUserIdAsync(int userId, bool includeEventos = false)
        {
            IQueryable<Palestrante> query = context.Palestrantes.Include(p => p.User).Include(p => p.RedesSociais);
            if (includeEventos){
                query = query.Include(p => p.PalestrantesEventos).ThenInclude(p => p.Evento);
            }
            query = query.Where(p => p.UserId == userId);

            return await query.AsNoTracking().FirstOrDefaultAsync();
        }

        public async Task<PageList<Palestrante>> GetAllPalestrantesAsync(PageParams pageParams ,bool includeEventos = false)
        {
            IQueryable<Palestrante> query = context.Palestrantes.Include(p => p.User).Include(r => r.RedesSociais);
            if (includeEventos){
                query = query.Include(pe => pe.PalestrantesEventos).ThenInclude(e => e.Evento);
            }
            query = query.Where(p => (p.User.PrimeiroNome.ToLower().Contains(pageParams.Term.ToLower()) || 
                                      p.User.UltimoNome.ToLower().Contains(pageParams.Term.ToLower())   ||
                                      p.MiniCurriculo.ToLower().Contains(pageParams.Term.ToLower()))    &&
                                      p.User.Funcao == Domain.Enum.Funcao.Palestrante)
                         .OrderBy(p => p.Id);

            //return await query.ToArrayAsync();
            return await PageList<Palestrante>.CreateAsync(query, pageParams.PageNumber, pageParams.pageSize);
        }

        /*
        public async Task<Palestrante[]> GetAllPalestrantesByNomesync(string nome, bool includeEventos)
        {
            IQueryable<Palestrante> query = context.Palestrantes.Include(r => r.RedesSociais);
            if (includeEventos){
                query = query.Include(pe => pe.PalestrantesEventos).ThenInclude(e => e.Evento);
            }
            query = query.OrderBy(p => p.Id).Where(p => p.User.PrimeiroNome.ToLower().Contains(nome.ToLower()));

            return await query.ToArrayAsync();
        }
        */

        

        
    }
}