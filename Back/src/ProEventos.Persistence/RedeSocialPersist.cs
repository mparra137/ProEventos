using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProEventos.Persistence.Contextos;
using ProEventos.Persistence.Contratos;
using ProEventos.Domain;
using Microsoft.EntityFrameworkCore;

namespace ProEventos.Persistence
{
    public class RedeSocialPersist : GeralPersist, IRedeSocialPersist
    {
        private readonly ProEventosContext context;
        public RedeSocialPersist(ProEventosContext context): base(context)
        {
            this.context = context;
            
        }
        public async Task<RedeSocial[]> GetAllEventoByIdAsync(int eventoId)
        {
            IQueryable<RedeSocial> query = context.RedesSociais;

            query = query.AsNoTracking().Where(r => r.EventoId == eventoId);

            return await query.ToArrayAsync();
        }

        public async Task<RedeSocial[]> GetAllPalestranteByIdAsync(int palestranteId)
        {
            IQueryable<RedeSocial> query = context.RedesSociais.Include(r => r.Palestrante);

            query = query.AsNoTracking().Where(r => r.PalestranteId == palestranteId);

            return await query.ToArrayAsync();
        }

        public async Task<RedeSocial> GetRedeSocialEventoByIdAsync(int eventoId, int id)
        {
            IQueryable<RedeSocial> query = context.RedesSociais;
            query = query.AsNoTracking().Where(r => r.EventoId == eventoId && r.Id == id);

            return await query.FirstOrDefaultAsync();
        }

        public async Task<RedeSocial> GetRedeSocialPalestranteByIdAsync(int palestranteId, int id)
        {
            IQueryable<RedeSocial> query = context.RedesSociais;
            query = query.AsNoTracking().Where(r => r.PalestranteId == palestranteId && r.Id == id);

            return await query.FirstOrDefaultAsync();
        }
    }
}