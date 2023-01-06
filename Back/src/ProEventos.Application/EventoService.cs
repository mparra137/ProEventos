using System;
using System.Threading.Tasks;
using ProEventos.Application.Contratos;
using ProEventos.Application.DTOs;
using ProEventos.Domain;
using ProEventos.Persistence.Contratos;
using AutoMapper;
namespace ProEventos.Application
{
    public class EventoService : IEventoService
    {
        private readonly IGeralPersist geralPersist;
        private readonly IEventoPersist eventosPersist;
        private readonly IMapper mapper;

        public EventoService(IGeralPersist geralPersist, IEventoPersist eventosPersist, IMapper mapper )
        {            
            this.eventosPersist = eventosPersist;
            this.geralPersist = geralPersist;            
            this.mapper = mapper;
        }

        public async Task<EventoDTO> AddEvento(int userId, EventoDTO model)
        {
            try
            {            
                var evento = mapper.Map<Evento>(model);
                evento.UserId = userId;

                geralPersist.Add<Evento>(evento);
                if (await geralPersist.SaveChangesAsync()){
                return mapper.Map<EventoDTO>(await eventosPersist.GetEventoByIdAsync(userId, evento.Id));
                }
                return null;
            }
            catch (Exception ex)
            {                
                throw new Exception(ex.Message);
            }
        }

        public async Task<EventoDTO> UpdateEvento(int userId, int eventoId, EventoDTO model)
        {
            try
            {
                Evento evento = await eventosPersist.GetEventoByIdAsync(userId, eventoId, false);
                if (evento == null) return null;

                model.Id = evento.Id;
                model.UserId = evento.UserId;

                mapper.Map(model, evento);              

                geralPersist.Update<Evento>(evento);

                if (await geralPersist.SaveChangesAsync()){

                    var eventoRetorno = await eventosPersist.GetEventoByIdAsync(userId, evento.Id, false);
                    
                    return mapper.Map<EventoDTO>(eventoRetorno);
                }
                return null;

            }
            catch (Exception ex)
            {                
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> DeleteEvento(int userId, int eventoId)
        {
            try
            {
                Evento evento = await eventosPersist.GetEventoByIdAsync(userId, eventoId, false);
                if (evento == null) throw new Exception("Evento para deletar não encontrado");

                geralPersist.Delete<Evento>(evento);
                return await geralPersist.SaveChangesAsync();                

            }
            catch (Exception ex)
            {                
                throw new Exception(ex.Message);
            }
        }

        public async Task<EventoDTO[]> GetAllEventosAsync(int userId, bool includePalestrantes = false)
        {
            try
            {
                Evento[] eventos = await eventosPersist.GetAllEventosAsync( userId, includePalestrantes);
                if (eventos == null) return Array.Empty<EventoDTO>();

                var resultado = mapper.Map<EventoDTO[]>(eventos);

                return resultado;
            }
            catch (Exception ex)
            {                
                throw new Exception(ex.Message);
            }
        }

        public async Task<EventoDTO[]> GetAllEventosByTemaAsync(int userId, string tema, bool includePalestrantes = false)
        {
            try
            {
                Evento[] eventos = await eventosPersist.GetAllEventosByTemaAsync(userId, tema, includePalestrantes);
                if (eventos == null) return Array.Empty<EventoDTO>();

                var resultado = mapper.Map<EventoDTO[]>(eventos);

                return resultado;
            }
            catch (Exception ex)
            {                
                throw new Exception(ex.Message);
            }
        }

        public async Task<EventoDTO> GetEventoByIdAsync( int userId, int eventoId, bool includePalestrantes = false)
        {
            try
            {
                Evento evento = await eventosPersist.GetEventoByIdAsync(userId, eventoId, includePalestrantes);
                if (evento == null) return null;

                var resultado = mapper.Map<EventoDTO>(evento);

                return resultado;
            }
            catch (Exception ex)
            {                
                throw new Exception(ex.Message);
            }
        }

        
    }
}