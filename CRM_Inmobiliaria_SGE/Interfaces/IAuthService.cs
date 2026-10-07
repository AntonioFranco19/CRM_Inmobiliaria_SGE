using System;
using System.Threading.Tasks;
using CRM_Inmobiliaria_SGE.Models;

namespace InmoCRM.Services;

public interface IAuthService
{
    Usuario? UsuarioActual { get; }
    event Action? SesionCambiada;
    Task<bool> IniciarSesionAsync(string username, string password);
    void CerrarSesion();
}