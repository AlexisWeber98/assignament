using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace assignament.Controllers;

[ApiController]
[Route("/api/territories")]
public class TerritoriesController : ControllerBase
{
    private readonly MongoService _mongoService;

    public TerritoriesController(MongoService mongoService)
    {
        _mongoService = mongoService;
    }

    // --------------------------------  Get All Territories ----------------------------------- //

    [HttpGet]
    public async Task<ActionResult<List<Territory>>> GetAllTerritories()
    {
        try
        {
            var data = await _mongoService.GetTerritoriesAsync();

            return Ok(ApiResponse.SuccessResponse(data));
        }
        catch (Exception ex)
        {
            return StatusCode(
                500,
                ApiResponse.ErrorResponse("Error fetching territories", ex.Message)
            );
        }
    }

    // ----------------------------------- Create Territory -------------------------------- //

    [HttpPost]
    public async Task<IActionResult> CreateTerritory([FromBody] Territory territory)
    {
        try
        {
            await _mongoService.CreateTerritoryAsync(territory);

            return CreatedAtAction(
                nameof(GetAllTerritories),
                new { id = territory.Id },
                ApiResponse.SuccessResponse(territory)
            );
        }
        catch (Exception ex)
        {
            return StatusCode(
                500,
                ApiResponse.ErrorResponse("Error creating territory", ex.Message)
            );
        }
    }

    // ---------------------------------- ADD assignament --------------------------------- //

    [HttpPost("{territoryId}/assignamensts")]
    public async Task<IActionResult> AddAssignament(
        int territoryId,
        [FromBody] Assignament assignament
    )
    {
        try
        {
            await _mongoService.AddAssignamentAsync(territoryId, assignament);

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(
                500,
                ApiResponse.ErrorResponse("Error adding assignament", ex.Message)
            );
        }
    }

    // ------------------------------------ Mark blok as complete ---------------------------- //

    [HttpPut("{territoryId}/assignaments/{asignee}/blocks/{block}")]
    public async Task<IActionResult> MarkBlockCompleted(
        int territoryId,
        string asignee,
        string block
    )
    {
        try
        {
            await _mongoService.MarkBlockAsCompleteAsync(territoryId, asignee, block);

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(
                500,
                ApiResponse.ErrorResponse("Error marking block as complete", ex.Message)
            );
        }
    }

    // -------------------------------- Mark an assignament as complete ----------------------- //

    [HttpPut("{territoryId}/assignaments/{assignee}/complete")]
    public async Task<IActionResult> MarkAssignamentAsComplete(int territoryId, string assignee)
    {
        try
        {
            await _mongoService.MarkAssignamentAsCompleteAsync(
                territoryId,
                assignee,
                DateTime.UtcNow
            );

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(
                500,
                ApiResponse.ErrorResponse("Error marking assignament as complete", ex.Message)
            );
        }
    }
}
