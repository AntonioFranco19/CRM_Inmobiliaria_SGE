// Services/InmuebleService.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using InmoCRM.Data;
using InmoCRM.Models;
using InmoCRM.Models.Enums;

namespace InmoCRM.Services;

public class InmuebleService
{
    private readonly InmoDbContext _context;

    public InmuebleService(InmoDbContext context)
    {
        _context = context;
    }

    public async Task<List<Inmueble>> ObtenerTodosAsync(string? filtroTexto = null, EstadoInmueble? estado = null)
    {
        var query = _context.Inmuebles.AsQueryable();

        if (!string.IsNullOrWhiteSpace(filtroTexto))
        {
            query = query.Where(i => i.Titulo.ToLower().Contains(filtroTexto.ToLower()) ||
                                     i.Referencia.ToLower().Contains(filtroTexto.ToLower()) ||
                                     i.Ciudad.ToLower().Contains(filtroTexto.ToLower()));
        }

        if (estado.HasValue)
        {
            query = query.Where(i => i.Estado == estado.Value);
        }

        return await query.OrderByDescending(i => i.FechaAlta).ToListAsync();
    }

    public async Task GuardarAsync(Inmueble inmueble)
    {
        var existe = await _context.Inmuebles.AnyAsync(i => i.Id == inmueble.Id);
        if (existe)
        {
            _context.Inmuebles.Update(inmueble);
        }
        else
        {
            await _context.Inmuebles.AddAsync(inmueble);
        }
        await _context.SaveChangesAsync();
    }

    public async Task EliminarAsync(Guid id)
    {
        var inmueble = await _context.Inmuebles.FindAsync(id);
        if (inmueble != null)
        {
            _context.Inmuebles.Remove(inmueble);
            await _context.SaveChangesAsync();
        }
    }

    /// <summary>
    /// Funcionalidad 4.2: Motor de cruce entre demandas del cliente y catálogo disponible
    /// </summary>
    public async Task<List<Inmueble>> ObtenerCoincidenciasParaClienteAsync(Cliente cliente)
    {
        var query = _context.Inmuebles.Where(i => i.Estado == EstadoInmueble.Disponible);

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
            query = query.Where(i => i.Ciudad.ToLower().Contains(cliente.ZonaInteres.ToLower()));
        }

        return await query.ToListAsync();
    }
}