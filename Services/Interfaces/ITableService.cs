using RestaurantTableReservationAPI.DTOs;

namespace RestaurantTableReservationAPI.Services.Interfaces;

public interface ITableService
{
    Task<List<TableResponseDto>> GetAllTablesAsync(bool isAdmin=false);
    Task<TableResponseDto?> GetTableByIdAsync(int id, bool isAdmin=false);
    Task<TableResponseDto> CreateTableAsync(CreateTableDto createTableDto);
    Task<TableResponseDto?> UpdateTableAsync(int id, UpdateTableDto updateTableDto);
    Task<bool> DeleteTableAsync(int id);
}
