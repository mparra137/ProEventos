using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProEventos.Application.Contratos;
using ProEventos.Application.DTOs;
using ProEventos.Domain;


namespace ProEventos.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventosController : ControllerBase
    {                 
        private readonly IEventoService eventoService;
        private readonly IWebHostEnvironment hostEnvironment;

        public EventosController(IEventoService eventoService, IWebHostEnvironment hostEnvironment )
        {            
            this.eventoService = eventoService;
            this.hostEnvironment = hostEnvironment;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                 var eventos = await eventoService.GetAllEventosAsync(true);
                 //if (eventos == null) return NotFound("Nenhum Evento Encontrado");
                 if (eventos == null) return NoContent();

                //List<EventoDTO> eventoRetorno = new List<EventoDTO>();

                //foreach(Evento evento in eventos){
                //    eventoRetorno.Add( new EventoDTO{
                //        Id = evento.Id,
                //        Local = evento.Local,
                //        DataEvento = evento.DataEvento.ToString(),
                //        Tema = evento.Tema,
                //        QtdPessoas = evento.QtdPessoas,
                //        ImagemURL = evento.ImagemURL,
                //       Email = evento.Email,
                //        Telefone = evento.Telefone
                //    });
                //}

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
                 var evento = await eventoService.GetEventoByIdAsync(id, true);
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

        [HttpGet("{tema}/tema")]
        public async Task<IActionResult> GetByTema(string tema)
        {
            try
            {
                 var eventos = await eventoService.GetAllEventosByTemaAsync(tema, true);
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


        [HttpPost("upload-image/{eventoId}")]
        public async Task<IActionResult> UploadImage(int eventoId){
            try
            {
                var evento = await eventoService.GetEventoByIdAsync(eventoId);                 
                if (evento == null) return NoContent();

                var file = Request.Form.Files[0];
                if (file.Length > 0){
                    DeleteImage(evento.ImagemURL);
                    evento.ImagemURL = await SaveImage(file);
                }
                var eventoRetorno = await eventoService.UpdateEvento(eventoId, evento);
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
                 var evento = await eventoService.AddEvento(model);
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
                var evento = await eventoService.UpdateEvento(id, model);
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
                var evento = await eventoService.GetEventoByIdAsync(id);
                if (evento == null) return NoContent();

                if (await eventoService.DeleteEvento(id)) {
                    DeleteImage(evento.ImagemURL);
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

        [NonAction]
        public async Task<string> SaveImage(IFormFile imageFile)
        {
            var imageName = new String(Path.GetFileNameWithoutExtension(imageFile.FileName).Take(10).ToArray()).Replace(' ', '-');

            imageName = $"{imageName}{DateTime.UtcNow.ToString("yyyymmddssfff")}{Path.GetExtension(imageFile.FileName)}";

            var imagePath = Path.Combine(hostEnvironment.ContentRootPath, @"Resources/images", imageName);

            using( var fileStream = new FileStream(imagePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }

            return imageName;
        }

        [NonAction]
        public void DeleteImage(string imageName){
            var imagePath = Path.Combine(hostEnvironment.ContentRootPath, @"Resources/images", imageName);
            if (System.IO.File.Exists(imagePath)) 
                System.IO.File.Delete(imagePath);
        }
    }
}
