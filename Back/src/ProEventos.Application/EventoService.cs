using System;
using System.Threading.Tasks;
using ProEventos.Application.Contratos;
using ProEventos.Domain;
using ProEventos.Persistence.Contratos;

namespace ProEventos.Application
{
    public class EventoService : IEventoService
    {
        private readonly IGeralPersist geralPersist;
        private readonly IEventosPersist eventosPersist;
        public EventoService(IGeralPersist geralPersist, IEventosPersist eventosPersist)
        {
            this.eventosPersist = eventosPersist;
            this.geralPersist = geralPersist;            
        }

        public async Task<Evento> AddEvento(Evento model)
        {
            try
            {
                 geralPersist.Add<Evento>(model);
                 if (await geralPersist.SaveChangesAsync()){
                    return await eventosPersist.GetEventoByIdAsync(model.Id);
                 }
                 return null;
            }
            catch (Exception ex)
            {                
                throw new Exception(ex.Message);
            }
        }

        public async Task<Evento> UpdateEvento(int eventoId, Evento model)
        {
            try
            {
                 Evento evento = await eventosPersist.GetEventoByIdAsync(eventoId, false);
                 if (evento == null) return null;

                 model.Id = evento.Id;

                 geralPersist.Update(model);
                 if (await geralPersist.SaveChangesAsync()){
                    return await eventosPersist.GetEventoByIdAsync(model.Id, false);
                 }
                return null;

            }
            catch (Exception ex)
            {                
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> DeleteEvento(int eventoId)
        {
            try
            {
                Evento evento = await eventosPersist.GetEventoByIdAsync(eventoId, false);
                if (evento == null) throw new Exception("Evento para deletar não encontrado");

                geralPersist.Delete<Evento>(evento);
                return await geralPersist.SaveChangesAsync();                

            }
            catch (Exception ex)
            {                
                throw new Exception(ex.Message);
            }
        }

        public async Task<Evento[]> GetAllEventosAsync(bool includePalestrantes = false)
        {
            try
            {
                Evento[] eventos = await eventosPersist.GetAllEventosAsync(includePalestrantes);
                if (eventos == null) return Array.Empty<Evento>();

                return eventos;
            }
            catch (Exception ex)
            {                
                throw new Exception(ex.Message);
            }
        }

        public async Task<Evento[]> GetAllEventosByTemaAsync(string tema, bool includePalestrantes = false)
        {
            try
            {
                 Evento[] eventos = await eventosPersist.GetAllEventosByTemaAsync(tema, includePalestrantes);
                 if (eventos == null) return Array.Empty<Evento>();

                 return eventos;
            }
            catch (Exception ex)
            {                
                throw new Exception(ex.Message);
            }
        }

        public async Task<Evento> GetEventoByIdAsync(int eventoId, bool includePalestrantes = false)
        {
            try
            {
                 Evento evento = await eventosPersist.GetEventoByIdAsync(eventoId, includePalestrantes);
                 if (evento == null) return null;

                 return evento;
            }
            catch (Exception ex)
            {                
                throw new Exception(ex.Message);
            }
        }

        
    }
}