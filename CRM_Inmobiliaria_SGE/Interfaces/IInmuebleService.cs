using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CRM_Inmobiliaria_SGE.Models;

namespace InmoCRM.Services;

public interface IInmuebleService
{
    Task<List<Inmueble>> ObtenerTodosAsync(string? filtroTexto = null, EstadoInmueble? estado = null);
    Task GuardarAsync(Inmueble inmueble);
    Task EliminarAsync(Guid id);
    Task<List<Inmueble>> ObtenerCoincidenciasParaClienteAsync(Cliente cliente);
}