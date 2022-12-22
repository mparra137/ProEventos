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

        public async Task<EventoDTO> AddEvento(EventoDTO model)
        {
            try
            {
                 var evento = mapper.Map<Evento>(model);
                 
                 geralPersist.Add<Evento>(evento);
                 if (await geralPersist.SaveChangesAsync()){
                    return mapper.Map<EventoDTO>(await eventosPersist.GetEventoByIdAsync(evento.Id));
                 }
                 return null;
            }
            catch (Exception ex)
            {                
                throw new Exception(ex.Message);
            }
        }

        public async Task<EventoDTO> UpdateEvento(int eventoId, EventoDTO model)
        {
            try
            {
                Evento evento = await eventosPersist.GetEventoByIdAsync(eventoId, false);
                if (evento == null) return null;

                model.Id = evento.Id;

                mapper.Map(model, evento);              

                geralPersist.Update<Evento>(evento);

                if (await geralPersist.SaveChangesAsync()){

                    var eventoRetorno = await eventosPersist.GetEventoByIdAsync(evento.Id, false);
                    
                    return mapper.Map<EventoDTO>(eventoRetorno);
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

        public async Task<EventoDTO[]> GetAllEventosAsync(bool includePalestrantes = false)
        {
            try
            {
                Evento[] eventos = await eventosPersist.GetAllEventosAsync(includePalestrantes);
                if (eventos == null) return Array.Empty<EventoDTO>();

                var resultado = mapper.Map<EventoDTO[]>(eventos);

                return resultado;
            }
            catch (Exception ex)
            {                
                throw new Exception(ex.Message);
            }
        }

        public async Task<EventoDTO[]> GetAllEventosByTemaAsync(string tema, bool includePalestrantes = false)
        {
            try
            {
                 Evento[] eventos = await eventosPersist.GetAllEventosByTemaAsync(tema, includePalestrantes);
                 if (eventos == null) return Array.Empty<EventoDTO>();

                var resultado = mapper.Map<EventoDTO[]>(eventos);

                 return resultado;
            }
            catch (Exception ex)
            {                
                throw new Exception(ex.Message);
            }
        }

        public async Task<EventoDTO> GetEventoByIdAsync(int eventoId, bool includePalestrantes = false)
        {
            try
            {
                 Evento evento = await eventosPersist.GetEventoByIdAsync(eventoId, includePalestrantes);
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