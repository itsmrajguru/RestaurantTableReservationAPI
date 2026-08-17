using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantTableReservationAPI.DTOs;
using RestaurantTableReservationAPI.Services.Interfaces;

namespace RestaurantTableReservationAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TablesController : ControllerBase
{
    private readonly ITableService _tableService;

    public TablesController(ITableService tableService)
    {
        _tableService=tableService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllTables([FromQuery] bool includeInactive = false)
    {
        bool isAdmin=User.IsInRole("Admin");
        
        // If a non-admin tries to request inactive tables, force it to false
        if(includeInactive && !isAdmin)
        {
            includeInactive=false;
        }

        var tables=await _tableService.GetAllTablesAsync(includeInactive);
        return Ok(tables);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTableById(int id)
    {
        bool isAdmin=User.IsInRole("Admin");
        var table=await _tableService.GetTableByIdAsync(id, isAdmin);
        if(table==null) return NotFound(new{message="Table not found."});
        return Ok(table);
    }

    [HttpPost]
    [Authorize(Roles="Admin")]
    public async Task<IActionResult> CreateTable(CreateTableDto dto)
    {
        var createdTable=await _tableService.CreateTableAsync(dto);
        return CreatedAtAction(nameof(GetTableById), new{id=createdTable.Id}, createdTable);
    }

    [HttpPut("{id}")]
    [Authorize(Roles="Admin")]
    public async Task<IActionResult> UpdateTable(int id, UpdateTableDto dto)
    {
        var updatedTable=await _tableService.UpdateTableAsync(id, dto);
        if(updatedTable==null) return NotFound(new{message="Table not found."});
        return Ok(updatedTable);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles="Admin")]
    public async Task<IActionResult> DeleteTable(int id)
    {
        bool success=await _tableService.DeleteTableAsync(id);
        if(!success) return NotFound(new{message="Table not found."});
        return NoContent();
    }
}
