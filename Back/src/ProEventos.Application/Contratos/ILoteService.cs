using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProEventos.Domain;
using ProEventos.Application.DTOs;

namespace ProEventos.Application.Contratos
{
    public interface ILoteService
    {
        Task<LoteDto[]> SaveLotes(int eventoId, LoteDto[] models);
        Task<bool> DeleteLote(int eventoId, int loteId);
        Task<LoteDto[]> GetLotesByEventoIdAsync(int eventoId);
        Task<LoteDto> GetLoteByIdsAsync(int eventoId, int loteId);
    }
}