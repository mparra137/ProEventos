using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ProEventos.Application.Contratos;
using ProEventos.Application.DTOs;
using ProEventos.Domain;
using ProEventos.Persistence.Contratos;
using ProEventos.Persistence;

namespace ProEventos.Application
{
    public class RedeSocialService : IRedeSocialService
    {
        private readonly IEventoPersist eventoPersist;
        private readonly IRedeSocialPersist redeSocialPersist;
        private readonly IMapper mapper;

        public RedeSocialService(IEventoPersist eventoPersist, IRedeSocialPersist redeSocialPersist , IMapper mapper)
        {
            this.eventoPersist = eventoPersist;
            this.redeSocialPersist = redeSocialPersist;
            this.mapper = mapper;
            
        }
        public async Task<RedeSocialDto[]> SaveByEvento(int eventoId, RedeSocialDto[] models)
        {
            try
            {
                var redesSociais = await redeSocialPersist.GetAllEventoByIdAsync(eventoId);
                if (redesSociais == null) return null;

                foreach( var model in models){
                    if (model.Id == 0) {
                        await AddRedeSocial(eventoId, model, true);
                    } else {
                        await UpdateRedeSocial(redesSociais, model);
                    }
                }

                var resultado = await redeSocialPersist.GetAllEventoByIdAsync(eventoId);

                return mapper.Map<RedeSocialDto[]>(resultado); 
            }
            catch (Exception ex)            {
                
                throw new Exception(ex.Message);
            }
        }

        public async Task<RedeSocialDto[]> SaveByPalestrante(int palestranteId, RedeSocialDto[] models)
        {
            try
            {
                var redesSociais = await redeSocialPersist.GetAllPalestranteByIdAsync(palestranteId);
                if (redesSociais == null) return null;

                foreach(var model in models){
                    if (model.Id == 0){
                        await AddRedeSocial(palestranteId, model, false);
                    } else {
                        await UpdateRedeSocial(redesSociais, model);
                    }
                }
                
                var resultado = await redeSocialPersist.GetAllPalestranteByIdAsync(palestranteId);
                return mapper.Map<RedeSocialDto[]>(resultado);
            }
            catch (Exception ex)
            {                
                throw new Exception(ex.Message);
            }
        }


        public async Task<bool> DeleteByEvento(int eventoId, int redeSocialId)
        {
            try
            {
                var redeSocial = await redeSocialPersist.GetRedeSocialEventoByIdAsync(eventoId, redeSocialId);
                if (redeSocial == null) throw new Exception("Evento/Rede Social inválida");

                redeSocialPersist.Delete<RedeSocial>(redeSocial);
                return await redeSocialPersist.SaveChangesAsync();               
            }
            catch (Exception ex)
            {                
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> DeleteByPalestrante(int palestranteId, int redeSocialId)
        {
            try
            {
                var redeSocial = await redeSocialPersist.GetRedeSocialPalestranteByIdAsync(palestranteId, redeSocialId);
                if (redeSocial == null) throw new Exception("Palestrante/Rede social inválido");

                redeSocialPersist.Delete<RedeSocial>(redeSocial);
                return await redeSocialPersist.SaveChangesAsync();   
            }
            catch (Exception ex)
            {                
                throw new Exception(ex.Message);
            }
        }

        public async Task<RedeSocialDto[]> GetAllByEventoIdAsync(int eventoId)
        {
            try
            {
                var redesSociais = await redeSocialPersist.GetAllEventoByIdAsync(eventoId);
                if (redesSociais == null) return null;

                var listaRetorno = mapper.Map<RedeSocialDto[]>(redesSociais);
                return listaRetorno;
            }
            catch (Exception ex)
            {                
                throw new Exception(ex.Message);
            }
        }

        public async Task<RedeSocialDto[]> GetAllByPalestranteIdAsync(int palestranteId)
        {
            try
            {
                var redesSociais = await redeSocialPersist.GetAllPalestranteByIdAsync(palestranteId);
                if (redesSociais == null) return null;
                var listaRetorno = mapper.Map<RedeSocialDto[]>(redesSociais);
                return listaRetorno;
            }
            catch (Exception ex)
            {                
                throw new Exception(ex.Message);
            }
        }

        public async Task<RedeSocialDto> GetRedeSocialEventoByIdsAsync(int eventoId, int redeSocialId)
        {
            try
            {
                var redeSocial = await redeSocialPersist.GetRedeSocialEventoByIdAsync(eventoId, redeSocialId);
                if (redeSocial == null) return null;
                return mapper.Map<RedeSocialDto>(redeSocial);
            }
            catch (Exception ex)
            {                
                throw new Exception(ex.Message);
            }
        }

        public async Task<RedeSocialDto> GetRedeSocialPalestranteByIdsAsync(int palestranteId, int redeSocialId)
        {
           try
            {
                var redeSocial = await redeSocialPersist.GetRedeSocialPalestranteByIdAsync(palestranteId, redeSocialId);
                if (redeSocial == null) return null;
                return mapper.Map<RedeSocialDto>(redeSocial);
            }
            catch (Exception ex)
            {                
                throw new Exception(ex.Message);
            }
        }

        public async Task AddRedeSocial(int id, RedeSocialDto model, bool isEvento){
            var redeSocial = mapper.Map<RedeSocial>(model);
            if (isEvento){
                redeSocial.EventoId = id;
                redeSocial.PalestranteId = null;
            } else {
                redeSocial.EventoId = null;
                redeSocial.PalestranteId = id;
            }
            redeSocialPersist.Add<RedeSocial>(redeSocial);
            await redeSocialPersist.SaveChangesAsync();
        }

        public async Task UpdateRedeSocial(RedeSocial[] redeSociais,RedeSocialDto model){
            var redeSocial = redeSociais.FirstOrDefault(r => r.Id == model.Id);            
            mapper.Map(model, redeSocial);
            redeSocialPersist.Update(redeSocial);
            await redeSocialPersist.SaveChangesAsync();
        }               
        
    }
}