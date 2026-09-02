using SaludsaActas.Application.DTOs;

namespace SaludsaActas.Application.Interfaces;

public interface IActiveDirectoryService
{
    Task<List<ActiveDirectoryEmployeeDto>> SearchEmployeesAsync(
        string searchTerm);

    Task<ActiveDirectoryEmployeeDto?> GetByUsernameAsync(
        string username);
}