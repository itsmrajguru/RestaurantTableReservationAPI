using AutoMapper;
using RestaurantTableReservationAPI.DTOs;
using RestaurantTableReservationAPI.Models;
using RestaurantTableReservationAPI.Repositories.Interfaces;
using RestaurantTableReservationAPI.Services.Interfaces;

namespace RestaurantTableReservationAPI.Services;

public class TableService : ITableService
{
    private readonly ITableRepository _tableRepository;
    private readonly IMapper _mapper;

    public TableService(ITableRepository tableRepository, IMapper mapper)
    {
        _tableRepository=tableRepository;
        _mapper=mapper;
    }

    public async Task<List<TableResponseDto>> GetAllTablesAsync(bool isAdmin=false)
    {
        var tables=await _tableRepository.GetAllAsync(isAdmin);
        return _mapper.Map<List<TableResponseDto>>(tables);
    }

    public async Task<TableResponseDto?> GetTableByIdAsync(int id, bool isAdmin=false)
    {
        var table=await _tableRepository.GetByIdAsync(id, isAdmin);
        if(table==null) return null;
        return _mapper.Map<TableResponseDto>(table);
    }

    public async Task<TableResponseDto> CreateTableAsync(CreateTableDto createTableDto)
    {
        var table=_mapper.Map<RestaurantTable>(createTableDto);
        table.IsActive=true; // default
        var createdTable=await _tableRepository.AddAsync(table);
        return _mapper.Map<TableResponseDto>(createdTable);
    }

    public async Task<TableResponseDto?> UpdateTableAsync(int id, UpdateTableDto updateTableDto)
    {
        var table=await _tableRepository.GetByIdAsync(id, true);
        if(table==null) return null;

        _mapper.Map(updateTableDto, table);
        await _tableRepository.UpdateAsync(table);

        return _mapper.Map<TableResponseDto>(table);
    }

    public async Task<bool> DeleteTableAsync(int id)
    {
        var table=await _tableRepository.GetByIdAsync(id, true);
        if(table==null) return false;

        table.IsActive=false; // soft delete
        await _tableRepository.UpdateAsync(table);
        return true;
    }
}
