﻿using CargosMonitor.Models;
using CargosMonitor.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CargosMonitor.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CargoController : ControllerBase
{
    private readonly ICargoService _cargoService;

    public CargoController(ICargoService cargoService)
    {
        _cargoService = cargoService;
    }

    [HttpGet]
    public async Task<ActionResult<List<Cargo>>> GetAllCargos()
    {
        var cargos = await _cargoService.GetAllCargosAsync();
        return Ok(cargos);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Cargo>> GetCargoById(int id)
    {
        var cargo = await _cargoService.GetCargoByIdAsync(id);
        if(cargo == null) return NotFound();
        return Ok(cargo);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult> PostCargo(Cargo cargo)
    {
        await _cargoService.AddCargoAsync(cargo);
        return CreatedAtAction(nameof(GetCargoById), new { id = cargo.CargoId }, cargo);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}")]
    public async Task<ActionResult<Cargo>> PutCargo(int id, [FromBody] Cargo updatedCargo)
    {
        var dbCargo = await _cargoService.GetCargoByIdAsync(id);
        if (dbCargo == null)
        {
            return NotFound($"Cargo with ID {id} not found.");
        }
        await _cargoService.UpdateCargoAsync(updatedCargo, id);
        return Ok(updatedCargo);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<ActionResult<Cargo>> DeleteCargo(int id)
    {
        var dbCargo = await _cargoService.GetCargoByIdAsync(id);
        if (dbCargo == null)
        {
            return NotFound($"Cargo with ID {id} not found.");
        }
        await _cargoService.DeleteCargoAsync(id);
        var cargos = await _cargoService.GetAllCargosAsync();
        return Ok(cargos);
    }
    
    
    
    [HttpGet("Warehouse/{WarehouseId}")]
    public async Task<IActionResult> GetCargosByWarehouseId(int WarehouseId)
    {
        var cargos = await _cargoService.GetCargosByWarehouseIdAsync(WarehouseId);
        if (cargos == null || cargos.Count == 0)
        {
            return NotFound(new { Message = "No cargos found for this warehouse." });
        }

        return Ok(cargos);
    }   
    
    
    [HttpGet("Airplane/{AirplaneId}")]
    public async Task<IActionResult> GetCargosByAirplaneId(int AirplaneId)
    {
        var cargos = await _cargoService.GetCargosByAirplaneIdAsync(AirplaneId);
        if (cargos == null || cargos.Count == 0)
        {
            return NotFound(new { Message = "No cargos found for this airplane." });
        }

        return Ok(cargos);
    }
    
    [Authorize(Roles = "Admin")]
    [HttpPost("add-to-airplane")]
    public async Task<IActionResult> AddCargoToAirplane([FromBody] AddToAirplaneRequest request)
    {
        try
        {
            await _cargoService.AddCargoToAirplaneAsync(request.CargoId, request.AirplaneId);
            return Ok(new { Message = "Cargo successfully added to the airplane." });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = ex.Message });
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("remove-from-airplane")]
    public async Task<IActionResult> RemoveCargoFromAirplane([FromBody] RemoveFromAirplaneRequest request)
    {
        try
        {
            await _cargoService.RemoveCargoFromAirplaneAsync(request.CargoId);
            return Ok(new { Message = "Cargo successfully removed from the airplane and returned to the original warehouse." });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = ex.Message });
        }
    }

// DTOs
    public class AddToAirplaneRequest
    {
        public int CargoId { get; set; }
        public int AirplaneId { get; set; }
    }

    public class RemoveFromAirplaneRequest
    {
        public int CargoId { get; set; }
    }

}











