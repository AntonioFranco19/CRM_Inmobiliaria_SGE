// Models/Enums/RolUsuario.cs
namespace CRM_Inmobiliaria_SGE.Models;

public enum RolUsuario
{
    Administrador,
    Agente
}

// Models/Enums/EstadoInmueble.cs

public enum EstadoInmueble
{
    Disponible,
    Reservado,
    Vendido,
    Alquilado
}

// Models/Enums/TipoInmueble.cs

public enum TipoInmueble
{
    Piso,
    Chalet,
    Atico,
    LocalComercial,
    PlazaGaraje
}

// Models/Enums/TipoOperacion.cs


public enum TipoOperacion
{
    Venta,
    Alquiler
}