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
    [ProducesResponseType(typeof(FlightPlan), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetAllFlightPlans()
    {
        var flightPlans = await _adapter.GetFlightPlans();

        return Ok(flightPlans);
    }
}
