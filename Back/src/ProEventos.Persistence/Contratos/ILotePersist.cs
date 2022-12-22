using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProEventos.Domain;

namespace ProEventos.Persistence.Contratos
{
    public interface ILotePersist
    {
        /// <summary>
        /// Método get que retornará uma lista de lotes por eventoId
        /// </summary>
        /// <param name="eventoId">Código chave para a tabela evento</param>
        /// <returns>Lista de lotes</returns>
        Task<Lote[]> GetLotesByEventoIdAsync(int eventoId);

        /// <summary>
        /// Método Get que retornará apenas 1 lote
        /// </summary>
        /// <param name="eventoId">Código chave para a tabela evento</param>
        /// <param name="id">Código chave para a tabela lote</param>
        /// <returns>Um lote para o evento</returns>
        Task<Lote> GetLoteByIdsAsync(int eventoId, int id);
    }
}