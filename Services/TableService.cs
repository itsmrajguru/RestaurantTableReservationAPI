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

    /* here bool includeInactive=false -->is the default value,
    which is override , by the controller's value*/
    public async Task<List<TableResponseDto>> GetAllTablesAsync(bool includeInactive=false)
    {
        var tables=await _tableRepository.GetAllAsync(includeInactive);
        return _mapper.Map<List<TableResponseDto>>(tables);
    }

    public async Task<TableResponseDto?> GetTableByIdAsync(int id, bool includeInactive=false)
    {
        var table=await _tableRepository.GetByIdAsync(id, includeInactive);
        if(table==null) return null;
        return _mapper.Map<TableResponseDto>(table);
    }

    public async Task<TableResponseDto> CreateTableAsync(CreateTableDto createTableDto)
    {
        /*We cant directly pass the raw data to the database,
        so we create a table form data from dto and pass it to the repository*/
        var table=_mapper.Map<RestaurantTable>(createTableDto);
        table.IsActive=true; // make the table active
        var createdTable=await _tableRepository.AddAsync(table);
        return _mapper.Map<TableResponseDto>(createdTable);
    }

    public async Task<TableResponseDto?> UpdateTableAsync(int id, UpdateTableDto updateTableDto)
    {
        //1.check if the table exists or not
        var table=await _tableRepository.GetByIdAsync(id, true);
        if(table==null) return null;

        //2.The table data is overrided by the new data from updateTabledDtp
        _mapper.Map(updateTableDto, table);
        await _tableRepository.UpdateAsync(table);

        return _mapper.Map<TableResponseDto>(table);
    }

    public async Task<bool> DeleteTableAsync(int id)
    {
        //1.check if the table exists or not
        var table=await _tableRepository.GetByIdAsync(id, true);
        if(table==null) return false;

        table.IsActive=false; // soft delete
        await _tableRepository.UpdateAsync(table);
        return true;
    }
}
