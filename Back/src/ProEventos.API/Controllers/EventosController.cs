using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProEventos.Application.Contratos;
using ProEventos.Application.DTOs;
using ProEventos.Domain;
using ProEventos.API.Extensions;
using ProEventos.Persistence.Models;
using ProEventos.API.Helpers;

namespace ProEventos.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EventosController : ControllerBase
    {                 
        private readonly IEventoService eventoService;
        private readonly IUtil util;

        private readonly string destino = "Images";

        public EventosController(IEventoService eventoService, IUtil util)
        {            
            this.util = util;
            this.eventoService = eventoService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Get([FromQuery]PageParams pageParams)
        {
            try
            {
                 var eventos = await eventoService.GetAllEventosAsync(User.GetUserId(), pageParams, true);
                 //if (eventos == null) return NotFound("Nenhum Evento Encontrado");
                 if (eventos == null) return NoContent();    

                 Response.AddPagination(eventos.CurrentPage, eventos.PageSize, eventos.TotalCount, eventos.TotalPages);            

                 return Ok(eventos);
            }
            catch (Exception ex)
            {                
                return this.StatusCode(StatusCodes.Status500InternalServerError, 
                                        $"Erro ao tentar recuperar eventos. Erro: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                 var evento = await eventoService.GetEventoByIdAsync( User.GetUserId(), id, true);
                 //if (evento == null) return NotFound("Evento Não encontrado");
                 if (evento == null) return NoContent(); //204                 

                 return Ok(evento);
            }
            catch (Exception ex)
            {                
                return this.StatusCode(StatusCodes.Status500InternalServerError, 
                                        $"Erro ao tentar recuperar o evento. Erro: {ex.Message}");
            }          
        }

        /*
        [HttpGet("{tema}/tema")]
        public async Task<IActionResult> GetByTema(string tema)
        {
            try
            {
                 var eventos = await eventoService.GetAllEventosByTemaAsync(User.GetUserId(), tema, true);
                 //if (eventos == null) return NotFound("Evento Não encontrado"); //404
                 if (eventos == null) return NoContent(); //204

                 return Ok(eventos);
            }
            catch (Exception ex)
            {                
                return this.StatusCode(StatusCodes.Status500InternalServerError, 
                                        $"Erro ao tentar recuperar o evento. Erro: {ex.Message}");
            }          
        }
        */


        [HttpPost("upload-image/{eventoId}")]
        public async Task<IActionResult> UploadImage(int eventoId){
            try
            {
                var userId = User.GetUserId();
                var evento = await eventoService.GetEventoByIdAsync(userId, eventoId);                 
                if (evento == null) return NoContent();

                var file = Request.Form.Files[0];
                if (file.Length > 0){     
                    util.DeleteImage(evento.ImagemURL, destino);                                 
                    evento.ImagemURL = await util.SaveImage(file, destino);
                }
                var eventoRetorno = await eventoService.UpdateEvento( userId, eventoId, evento);
                return Ok(eventoRetorno);
            }
            catch (Exception ex)
            {                
                return this.StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao tentar salvar a imagem. Erro: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post(EventoDTO model){
            try
            {
                 var evento = await eventoService.AddEvento(User.GetUserId(), model);
                 //if (evento == null) return BadRequest("Erro ao tentar adicionar um evento");
                 if (evento == null) return NoContent();

                 return Ok(evento);
            }
            catch (Exception ex)
            {                
                return this.StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao tentar adicionar um novo evento. Erro: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, EventoDTO model){
            try{
                var evento = await eventoService.UpdateEvento(User.GetUserId(), id, model);
                //if (evento == null) return BadRequest("Erro ao atualizar o evento");              
                if (evento == null) return NoContent(); 

                return Ok(evento);               


            } catch (Exception ex){
                return this.StatusCode(StatusCodes.Status500InternalServerError, $"Erro inesperado ao tentar atualizar o evento. Erro: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id){
            try
            {
                var userId = User.GetUserId();
                var evento = await eventoService.GetEventoByIdAsync(userId, id);
                if (evento == null) return NoContent();

                if (await eventoService.DeleteEvento(userId, id)) {
                    util.DeleteImage(evento.ImagemURL, destino);
                    return Ok(new { message = "Deletado"});
                }
                else
                {
                    throw new Exception("Ocorreu um problema não específico ao tentar deletar o evento");
                }           

            }
            catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao tentar excluir um evento. Erro: {ex.Message}");
                
            }
        }
        
    }
}
