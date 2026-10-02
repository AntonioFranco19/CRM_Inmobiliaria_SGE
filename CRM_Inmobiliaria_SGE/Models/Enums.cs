// Models/Enums/RolUsuario.cs
namespace InmoCRM.Models.Enums;

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