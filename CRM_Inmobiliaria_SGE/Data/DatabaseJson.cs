// Data/InmoDbContext.cs
using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using CRM_Inmobiliaria_SGE.Models;
using System.Collections.Generic;

namespace CRM_Inmobiliaria_SGE.Data;

public class DatabaseJson
{
    public List<Usuario> Usuarios { get; set; } = [];
    public List<Inmueble> Inmuebles { get; set; } = [];
    public List<Cliente> Clientes { get; set; } = [];
    public List<Visita> Visitas { get; set; } = [];
    public List<Proveedor> Proveedores { get; set; } = [];
    public List<Operacion> Operaciones { get; set; } = [];
    public List<Factura> Facturas { get; set; } = [];
}