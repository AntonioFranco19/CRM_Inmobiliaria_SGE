// Services/InmuebleService.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CRM_Inmobiliaria_SGE.Data;
using CRM_Inmobiliaria_SGE.Models;
using CRM_Inmobiliaria_SGE.Interfaces;

namespace InmoCRM.Services;

public class InmuebleService : IInmuebleService
{
    private readonly IJsonStorageService _storageService;

    public InmuebleService(IJsonStorageService storageService)
    {
        _storageService = storageService;
    }

    public async Task<List<Inmueble>> ObtenerTodosAsync(string? filtroTexto = null, EstadoInmueble? estado = null)
    {
        var db = await _storageService.LoadDataAsync();
        var query = db.Inmuebles.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(filtroTexto))
        {
            var texto = filtroTexto.Trim().ToLowerInvariant();
            query = query.Where(i =>
                i.Titulo.ToLowerInvariant().Contains(texto) ||
                i.Referencia.ToLowerInvariant().Contains(texto) ||
                i.Ciudad.ToLowerInvariant().Contains(texto));
        }

        if (estado.HasValue)
        {
            query = query.Where(i => i.Estado == estado.Value);
        }

        return query.OrderByDescending(i => i.FechaAlta).ToList();
    }

    public async Task GuardarAsync(Inmueble inmueble)
    {
        var db = await _storageService.LoadDataAsync();
        var index = db.Inmuebles.FindIndex(i => i.Id == inmueble.Id);

        if (index >= 0)
        {
            db.Inmuebles[index] = inmueble;
        }
        else
        {
            db.Inmuebles.Add(inmueble);
        }

        await _storageService.SaveDataAsync(db);
    }

    public async Task EliminarAsync(Guid id)
    {
        var db = await _storageService.LoadDataAsync();
        var inmueble = db.Inmuebles.FirstOrDefault(i => i.Id == id);

        if (inmueble != null)
        {
            db.Inmuebles.Remove(inmueble);
            await _storageService.SaveDataAsync(db);
        }
    }

    public async Task<List<Inmueble>> ObtenerCoincidenciasParaClienteAsync(Cliente cliente)
    {
        var db = await _storageService.LoadDataAsync();
        var query = db.Inmuebles.Where(i => i.Estado == EstadoInmueble.Disponible);

        if (cliente.PresupuestoMaximo > 0)
        {
            query = query.Where(i => i.Precio <= cliente.PresupuestoMaximo);
        }

        if (cliente.MinHabitaciones > 0)
        {
            query = query.Where(i => i.Habitaciones >= cliente.MinHabitaciones);
        }

        if (cliente.TipoInteres.HasValue)
        {
            query = query.Where(i => i.Tipo == cliente.TipoInteres.Value);
        }

        if (!string.IsNullOrWhiteSpace(cliente.ZonaInteres))
        {
            var zona = cliente.ZonaInteres.Trim().ToLowerInvariant();
            query = query.Where(i => i.Ciudad.ToLowerInvariant().Contains(zona));
        }

        return query.ToList();
    }
}