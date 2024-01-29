using FlightPlanApi.Data;
using FlightPlanApi.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FlightPlanApi.Controllers;

[ApiController]
[Route("api/v1/flightplan")]
public class FlightPlanController : ControllerBase
{
    private readonly IDatabaseAdapter _adapter;
    public FlightPlanController(IDatabaseAdapter adapter) 
    {
        _adapter = adapter;
    }
    [HttpGet]
    [ProducesResponseType(typeof(List<FlightPlan>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    public async Task<IActionResult> GetAllFlightPlans()
    {
        var flightPlans = await _adapter.GetFlightPlans();
        if (flightPlans == null)
        {
            return NoContent();
        }

        return Ok(flightPlans);
    }
    [HttpGet]
    [Route("GetFlightPlanById")]
    [ProducesResponseType(typeof(FlightPlan), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> GetFlightPlanById(string flightPlanId)
    {
        var flightPlan = await _adapter.GetFlightPlan(flightPlanId);
        if(flightPlan.FlightPlanId != flightPlanId)
        {
            return NotFound();
        }

        return Ok(flightPlan);
    }
    [HttpPost]
    [Route("route")]
    public async Task<IActionResult> CreateFlightPlan([FromBody] FlightPlan flightPlan)
    {
        var result = await _adapter.FileFlightPlan(flightPlan);

        switch (result)
        {
            case TransactionResult.BadRequest: 
                return BadRequest();
            case TransactionResult.Success:
                return CreatedAtRoute("GetFlightPlanById", new { flightPlanId = flightPlan.FlightPlanId }, flightPlan);
            default:
                return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
    [HttpPut]
    public async Task<IActionResult> UpdateFlightPlan(FlightPlan flightPlan)
    {
        var result = await _adapter.UpdateFlightPlan(flightPlan.FlightPlanId, flightPlan);

        switch (result)
        {
            case TransactionResult.BadRequest:
                return NotFound();
            case TransactionResult.Success:
                return Ok();
            default:
                return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
    [HttpDelete("{flightPlanId}")]
    public async Task<IActionResult> DeleteFlightPlan(string flightPlanId)
    {
        var result = await _adapter.DeleteFightPlanById(flightPlanId);
        if(result)
        {
            return Ok();
        }

        return NotFound();
    }
    [HttpGet]
    [Route("airport/departure/{flightPlanId}")]
    public async Task<IActionResult> GetFlightPlanDepartureAirport(string flightPlanId)
    {
        var flightPlan = await _adapter.GetFlightPlan(flightPlanId);

        if(flightPlan == null)
        {
            return NotFound();
        }

        return Ok(flightPlan.DepartureAirport);
    }
    [HttpGet]
    [Route("route/{flightPlanId}")]
    public async Task<IActionResult> GetFlightPlanRoute(string flightPlanId)
    {
        var flightPlan = await _adapter.GetFlightPlan(flightPlanId);

        if (flightPlan == null)
        {
            return NotFound();
        }

        return Ok(flightPlan.Route);
    }
    [HttpGet]
    [Route("Time/enroute/")]
    public async Task<IActionResult> GetFlightPlanTimeInRoute(string flightPlanId)
    {
        var flightPlan = await _adapter.GetFlightPlan(flightPlanId);

        if (flightPlan == null)
        {
            return NotFound();
        }
        var estimmedTime = flightPlan.ArrivalTime - flightPlan.DepartureTime;

        return Ok(estimmedTime);
    }

}
