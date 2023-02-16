using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProEventos.Application.Contratos;
using ProEventos.Application.DTOs;
using ProEventos.Persistence.Models;
using ProEventos.Persistence.Contratos;
using ProEventos.Domain;
using AutoMapper;

namespace ProEventos.Application
{
    public class PalestranteService : IPalestranteService
    {
        private readonly IPalestrantesPersist palestrantesPersist;
        private readonly IMapper mapper;

        public PalestranteService(IPalestrantesPersist palestrantesPersist, IMapper mapper)
        {
            this.mapper = mapper;
            this.palestrantesPersist = palestrantesPersist;
        }

        public async Task<PalestranteDto> AddPalestrantes(int userId, PalestranteAddDto model)
        {
            try
            {
                var palestrante = mapper.Map<Palestrante>(model);
                palestrante.UserId = userId;
                palestrantesPersist.Add<Palestrante>(palestrante);

                if(await palestrantesPersist.SaveChangesAsync()){
                    return mapper.Map<PalestranteDto>(await palestrantesPersist.GetPalestranteByUserIdAsync(userId));
                }  
                return null;           
            }
            catch (Exception ex)
            {
                
                throw new Exception(ex.Message);
            }
        }

        public async Task<PageList<PalestranteDto>> GetAllPalestrantesAsync(PageParams pageParams, bool includeEventos = false)
        {
            try
            {
                var palestrantes = await palestrantesPersist.GetAllPalestrantesAsync(pageParams, includeEventos); 
                if (palestrantes == null) return null;

                var palestranteDto = mapper.Map<PageList<PalestranteDto>>(palestrantes);
                
                palestranteDto.TotalCount = palestrantes.TotalCount;
                palestranteDto.TotalPages = palestrantes.TotalPages;
                palestranteDto.CurrentPage = palestrantes.CurrentPage;
                palestranteDto.PageSize = palestrantes.PageSize;

                return palestranteDto;
            }
            catch (Exception ex)
            {                
                throw new Exception(ex.Message);
            }
        }

        public async Task<PalestranteDto> GetPalestranteByUserIdAsync(int userId, bool includeEventos = false)
        {
            try
            {
                var palestrante = await palestrantesPersist.GetPalestranteByUserIdAsync(userId, includeEventos);
                if (palestrante == null) return null;

                return mapper.Map<PalestranteDto>(palestrante);
            }
            catch (Exception ex)            {
                
                throw new Exception(ex.Message);
            }
        }

        public async Task<PalestranteDto> UpdatePalestrante(int userId, PalestranteUpdateDto model)
        {
            try
            {
                var palestrante = await palestrantesPersist.GetPalestranteByUserIdAsync(userId);
                if (palestrante == null) return null;

                model.Id = palestrante.Id;
                mapper.Map(model, palestrante);

                palestrantesPersist.Update(palestrante);
                if (await palestrantesPersist.SaveChangesAsync()){
                    return mapper.Map<PalestranteDto>(palestrante);
                }
                return null;
            }
            catch (Exception ex)
            {                
                throw new Exception(ex.Message);
            }
        }
    }
}