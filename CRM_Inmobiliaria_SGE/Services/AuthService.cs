// Services/AuthService.cs
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CRM_Inmobiliaria_SGE.Models;
using System.Linq;

namespace InmoCRM.Services;

public class AuthService : IAuthService
{
    private readonly IJsonStorageService _storageService;
    public Usuario? UsuarioActual { get; private set; }

    public event Action? SesionCambiada;

    public AuthService(IJsonStorageService storageService)
    {
        _storageService = storageService;
    }

    public async Task<bool> IniciarSesionAsync(string username, string password)
    {
        var db = await _storageService.LoadDataAsync();
        var user = db.Usuarios.FirstOrDefault(u =>
            u.Username.Equals(username, StringComparison.OrdinalIgnoreCase) &&
            u.Activo);

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