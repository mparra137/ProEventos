using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProEventos.API.Extensions;
using ProEventos.Application;
using ProEventos.Application.Contratos;
using ProEventos.Application.DTOs;
using ProEventos.Persistence.Models;

namespace ProEventos.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PalestrantesController : ControllerBase
    {
        private readonly IPalestranteService palestranteService;
        public PalestrantesController(IPalestranteService palestranteService)
        {
            this.palestranteService = palestranteService;            
        }

        [HttpGet("all")]
        public async Task<IActionResult> Get([FromQuery] PageParams pageParams){
            try
            {
                var palestrantes = await palestranteService.GetAllPalestrantesAsync(pageParams, true);
                if (palestrantes == null) return NoContent();

                Response.AddPagination(palestrantes.CurrentPage, palestrantes.PageSize, palestrantes.TotalCount, palestrantes.TotalPages);

                return Ok(palestrantes);
            }
            catch (Exception ex)
            {                
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Erro ao tentar recuperar palestrantes. Erro:" + ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetPalestrantes(){
            try
            {
                 var palestrante = await palestranteService.GetPalestranteByUserIdAsync(User.GetUserId(), true);
                 if (palestrante == null) return NoContent();

                 return Ok(palestrante);
            }
            catch (Exception ex)
            {                
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Erro ao tentar obter palestrante. Erro:" + ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post(PalestranteAddDto model){
            try
            {
                var userID = User.GetUserId();
                var palestrante = await palestranteService.GetPalestranteByUserIdAsync(userID);
                if (palestrante == null)
                    palestrante = await palestranteService.AddPalestrantes(userID, model);                

                return Ok(palestrante);
            }
            catch (Exception ex)
            {                
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Não foi possível adicionar o palestrante. Erro:" + ex.Message);
            }
        }

        [HttpPut]
        public async Task<IActionResult> Put(PalestranteUpdateDto model){
            try
            {                
                var palestrante = await palestranteService.UpdatePalestrante(User.GetUserId(), model);    
                if (palestrante == null) return NoContent();
                return Ok(palestrante);
            }
            catch (Exception ex)
            {                
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Erro ao atualizar o palestrante. Erro " + ex.Message );
            }
        }

    }
}