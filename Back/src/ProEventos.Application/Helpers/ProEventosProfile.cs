using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ProEventos.Domain;
using ProEventos.Domain.Identity;
using ProEventos.Application.DTOs;

namespace ProEventos.Application.Helpers
{
    public class ProEventosProfile: Profile
    {
        public ProEventosProfile()
        {
            CreateMap<Evento, EventoDTO>().ReverseMap();
            CreateMap<Lote,LoteDto>().ReverseMap();
            CreateMap<RedeSocial, RedeSocialDto>().ReverseMap();
            CreateMap<Palestrante, PalestranteDto>().ReverseMap();
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<User, UserUpdateDto>().ReverseMap();
            CreateMap<User, UserLoginDto>().ReverseMap();

            CreateMap<Palestrante, PalestranteDto>().ReverseMap();
            CreateMap<Palestrante, PalestranteAddDto>().ReverseMap();
            CreateMap<Palestrante, PalestranteUpdateDto>().ReverseMap();
        }
    }
}