using System.Threading.Tasks;
using CRM_Inmobiliaria_SGE.Data;

namespace CRM_Inmobiliaria_SGE.Interfaces;

public interface IJsonStorageService
{
    Task InitializeAsync();
    Task<DatabaseJson> LoadDataAsync();
    Task SaveDataAsync(DatabaseJson data);
}