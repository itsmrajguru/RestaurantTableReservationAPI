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

    /*Note :In this controller,
    Customer -> should see ACTIVE tables only
    Admin    -> can choose to see ACTIVE + INACTIVE tables*/
    [HttpGet]
    public async Task<IActionResult> GetAllTables([FromQuery] bool includeInactive = false)
    {
        bool isAdmin=User.IsInRole("Admin");
        
        // If a non-admin tries to request inactive tables, force it to false
        // Case 1: inactive=true, admin=true   -> allowed, no override
        // Case 2: inactive=true, admin=false  -> blocked, forced to false (security)
        // Case 3: inactive=false, admin=true  -> no override needed
        // Case 4: inactive=false, admin=false -> no override needed
        if(includeInactive && !isAdmin)
        {
            includeInactive=false;
        }

        var tables=await _tableService.GetAllTablesAsync(includeInactive);
        return Ok(tables);
    }

    /// <param name="id">1->Table 1, 2->Table 2, etc.</param>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetTableById(int id)
    {
        bool isAdmin=User.IsInRole("Admin");
        var table=await _tableService.GetTableByIdAsync(id, isAdmin);
        if(table==null) return NotFound(new{message="Table not found."});
        return Ok(table);
    }


    /* (CreateTableDto dto) :Here CreateTabledto-->set of rules
    and dto -->data coming from req.body */
    [HttpPost]
    [Authorize(Roles="Admin")]
    public async Task<IActionResult> CreateTable(CreateTableDto dto)
    {
        var createdTable=await _tableService.CreateTableAsync(dto);
        /* CreatedAtAction-->returns 201 http status code aprat from 200*/
        return CreatedAtAction(nameof(GetTableById), new{id=createdTable.Id}, createdTable);
    }

    /// <param name="id">1->Table 1, 2->Table 2, etc.</param>
    [HttpPut("{id}")]
    [Authorize(Roles="Admin")]
    public async Task<IActionResult> UpdateTable(int id, UpdateTableDto dto)
    {
        var updatedTable=await _tableService.UpdateTableAsync(id, dto);
        if(updatedTable==null) return NotFound(new{message="Table not found."});
        return Ok(updatedTable);
    }

    /// <param name="id">1->Table 1, 2->Table 2, etc.</param>
    [HttpDelete("{id}")]
    [Authorize(Roles="Admin")]
    public async Task<IActionResult> DeleteTable(int id)
    {
        bool success=await _tableService.DeleteTableAsync(id);
        if(!success) return NotFound(new{message="Table not found."});
        return NoContent();
    }
}
