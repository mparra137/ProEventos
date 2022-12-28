using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProEventos.Application.DTOs
{
    public class EventoDTO
    {
        public int Id { get; set; }
        public string Local { get; set; }
        public string DataEvento { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório"),
        //MinLength(3, ErrorMessage = "{0} deve ter no mínimo 3 caracteres"),
        //MaxLength(50, ErrorMessage = "{0} pode ter no máximo 50 caracteres"),
        StringLength(50, MinimumLength = 3, ErrorMessage = "{0} deve ter entre 3 e 50 caracteres")]
        public string Tema { get; set; }
        
        [Range(1, 120000, ErrorMessage = "Intervalo de {0} deve ser entre 1 e 120000")]
        [Display(Name = "quantidade de Pessoas")]
        public int QtdPessoas { get; set; }        

        //[RegularExpression(@".*\.(gif|jpe?g|bmp|png)$", ErrorMessage = "O nome do arquivo não é uma imagem válida (gif, jpg, )")]
        public string ImagemURL {get;set;}

        [Required(ErrorMessage = "O campo {0} é obrigatório"),
        Display(Name = "e-mail"),
        EmailAddress(ErrorMessage = "O campo {0} deve ser um endereço de e-mail válido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [Phone(ErrorMessage = "O número de {0} é inválido")]
        public string Telefone { get; set; }

        public IEnumerable<LoteDto> Lotes { get; set; }

        public IEnumerable<RedeSocialDto> RedesSociais { get; set; }

        public IEnumerable<PalestranteDto> Palestrantes { get; set; }
    }
}