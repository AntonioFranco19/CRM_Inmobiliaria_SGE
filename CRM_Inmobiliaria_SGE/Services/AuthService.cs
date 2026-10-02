// Services/AuthService.cs
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using InmoCRM.Data;
using InmoCRM.Models;

namespace InmoCRM.Services;

public class AuthService
{
    private readonly InmoDbContext _context;
    public Usuario? UsuarioActual { get; private set; }

    public event Action? SesionCambiada;

    public AuthService(InmoDbContext context)
    {
        _context = context;
    }

    public async Task<bool> IniciarSesionAsync(string username, string password)
    {
        var user = await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Username == username && u.Activo);

        if (user == null || user.PasswordHash != password)
        {
            return false;
        }

        UsuarioActual = user;
        SesionCambiada?.Invoke();
        return true;
    }

    public void CerrarSesion()
    {
        UsuarioActual = null;
        SesionCambiada?.Invoke();
    }
}