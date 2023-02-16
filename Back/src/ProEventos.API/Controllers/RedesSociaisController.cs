using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProEventos.Application.Contratos;
using ProEventos.Application.DTOs;
using ProEventos.API.Extensions;

namespace ProEventos.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RedesSociaisController : ControllerBase
    {
        private readonly IRedeSocialService redeSocialService;
        private readonly IEventoService eventoService;
        private readonly IPalestranteService palestranteService;

        public RedesSociaisController(IRedeSocialService redeSocialService, IEventoService eventoService, IPalestranteService palestranteService)
        {
            this.eventoService = eventoService;
            this.palestranteService = palestranteService;
            this.redeSocialService = redeSocialService;
        }

        [HttpGet("evento/{eventoId}")]
        public async Task<IActionResult> GetByEvento(int eventoId){
            try{
                if (!await AutorEvento(eventoId))
                    return Unauthorized();

                var redesSociais = await redeSocialService.GetAllByEventoIdAsync(eventoId);
                if (redesSociais == null) return NoContent();

                return Ok(redesSociais);                    
            } 
            catch(Exception ex){
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Erro ao obter as redes sociais do evento. Erro: " + ex.Message);
            }
        }

        [HttpGet("palestrante")]
        public async Task<IActionResult> GetByPalestrante(){
            try{
                var userId = User.GetUserId();
                var palestrante = await palestranteService.GetPalestranteByUserIdAsync(userId);
                if (palestrante == null) return Unauthorized(); 

                var redesSociais = await redeSocialService.GetAllByPalestranteIdAsync(palestrante.Id);
                if (redesSociais == null) return NoContent();

                return Ok(redesSociais);                    
            } 
            catch(Exception ex){
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Erro ao obter as redes sociais do palestrante. Erro: " + ex.Message);
            }
        }

        [HttpDelete("palestrante/{redeSocialId}")]
        public async Task<IActionResult> DeletebyPalestrante(int redeSocialId){
            try
            {
                var palestrante = await palestranteService.GetPalestranteByUserIdAsync(User.GetUserId());
                if (palestrante == null) return Unauthorized();    

                var redeSocial = await redeSocialService.GetRedeSocialPalestranteByIdsAsync(palestrante.Id, redeSocialId);
                if (redeSocial == null) return NoContent();

                return await redeSocialService.DeleteByPalestrante(palestrante.Id, redeSocialId) 
                    ? Ok(new { message = "Rede social do palestrante Deletada"})
                    : throw new Exception("Ocorreu um erro ao tentar deletar a rede social do palestrante");
                
            }
            catch (Exception ex)
            {                
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Erro ao tentar deletar a rede social do palestrante. Erro: " + ex.Message);
            }
        }

        [HttpDelete("evento/{eventoId}/{redeSocialId}")]
        public async Task<IActionResult> DeletebyEvento(int eventoId, int redeSocialId){
            try
            {
                if (!await AutorEvento(eventoId))
                    return Unauthorized();

                var redeSocial = await redeSocialService.GetRedeSocialEventoByIdsAsync(eventoId, redeSocialId);
                if (redeSocial == null) return NoContent();

                return (await redeSocialService.DeleteByEvento(eventoId, redeSocialId)) 
                    ? Ok(new { message = "Rede social do Evento Deletada"})
                    : throw new Exception("Ocorreu um erro ao tentar deletar a rede social do evento");

                
            }
            catch (Exception ex)
            {                
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Erro ao tentar deletar a rede social do evento. Erro: " + ex.Message);
            }
        }

        [HttpPost("evento/{eventoId}")]
        public async Task<IActionResult> SaveByEvento(int eventoId, RedeSocialDto[] models){
            try
            {
                if (eventoId > 0){
                    if (!await AutorEvento(eventoId))
                        return Unauthorized();

                    var redesSociais = await redeSocialService.SaveByEvento(eventoId, models);
                    if (redesSociais == null) return NoContent();

                    return Ok(redesSociais);
                }
                return BadRequest(new {message = "id do evento não pode estar em branco"});
            }
            catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Erro ao tentar salvar as redes sociais do evento. Erro: " + ex.Message);
            }
        }

        [HttpPost("palestrante")]
        public async Task<IActionResult> SaveByPalestrante(RedeSocialDto[] models){
            try
            {
                var palestrante = await palestranteService.GetPalestranteByUserIdAsync(User.GetUserId());    
                if (palestrante == null) return Unauthorized();
                
                var redesSociais = await redeSocialService.SaveByPalestrante(palestrante.Id, models);
                if (redesSociais == null) return NoContent();

                return Ok(redesSociais);
                
            }
            catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Erro ao tentar salvar as redes sociais do palestrante. Erro: " + ex.Message);
            }
        }

        [NonAction]
        private async Task<bool> AutorEvento(int eventoId){
            try
            {
                var evento = await eventoService.GetEventoByIdAsync(User.GetUserId(), eventoId, false);
                if (evento != null)
                    return true;

                return false;
            }
            catch (Exception)
            {
                
                throw;
            }
        }

    }
}