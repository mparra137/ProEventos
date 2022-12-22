using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using ProEventos.Application.DTOs;
using ProEventos.Application.Contratos;

namespace ProEventos.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LotesController : Controller
    {
        private readonly ILoteService loteService;

        public LotesController(ILoteService loteService)
        {
            this.loteService = loteService;
        }

        [HttpGet("{eventoId}")]
        public async Task<IActionResult> Get(int eventoId){
            try
            {
                var lotes = await loteService.GetLotesByEventoIdAsync(eventoId); 
                if (lotes == null) return NoContent();

                return Ok(lotes);
            }
            catch (Exception ex)
            {                
                throw new Exception("Erro ao tentar recuperar os lotes do evento. " + ex.Message);
            }
            
        }

        [HttpPut("{eventoId}")]
        public async Task<IActionResult> SaveLotes(int eventoId, LoteDto[] models){
            try{
                var lotes = await loteService.SaveLotes(eventoId, models);
                //if (evento == null) return BadRequest("Erro ao atualizar o evento");              
                if (lotes == null) return NoContent(); 

                return Ok(lotes);               


            } catch (Exception ex){
                return this.StatusCode(StatusCodes.Status500InternalServerError, $"Erro inesperado ao tentar salvar os lotes. Erro: {ex.Message}");
            }
        }

        [HttpDelete("{eventoId}/{loteId}")]
        public async Task<IActionResult> Delete(int eventoId, int loteId){
            try
            {
                var lote = await loteService.GetLoteByIdsAsync(eventoId, loteId);
                if (lote == null) return NoContent();                

                return await loteService.DeleteLote(lote.EventoId, lote.Id)
                    ? Ok(new { message = "Lote Deletado"})
                    : throw new Exception("Ocorreu um problema não específico ao tentar deletar o lote");
                    //BadRequest("Evento não deletado");                

            }
            catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao tentar excluir um lote. Erro: {ex.Message}");
                
            }
        }

        
    }
}