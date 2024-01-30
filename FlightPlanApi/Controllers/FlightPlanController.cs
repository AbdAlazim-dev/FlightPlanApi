using FlightPlanApi.Data;
using FlightPlanApi.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Swashbuckle.AspNetCore.Annotations;

namespace FlightPlanApi.Controllers;

[ApiController]
[Route("api/v1/flightplans")]
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
    //One way of documenting the api
    [SwaggerResponse((int)StatusCodes.Status204NoContent, "No content found on the database")]
    public async Task<IActionResult> GetAllFlightPlans()
    {
        var flightPlans = await _adapter.GetFlightPlans();
        if (flightPlans == null)
        {
            return NoContent();
        }

        return Ok(flightPlans);
    }
    //the other way of documenting the api
    /// <summary>
    /// Get the flight plan by its id
    /// </summary>
    /// <param name="flightPlanId">the ID of the flight plan you want to get its info</param>
    /// <response code="404">There is no flight plan with this id</response>
    /// <response code="400">somthing wrong with the request you sent</response>
    /// <response code="200">the flight plan info</response>
    /// <response code="401">You are not authorized to use the endpoint</response>
    /// <response code="406">The output format you entered in the "Accept header is not supported"</response>
    /// <returns></returns>
    [HttpGet("{flightPlanId}", Name = "GetFlightPlanById")]
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
    /// <summary>
    /// Add flight plan to the system
    /// </summary>
    /// <remarks>
    /// sample request
    /// 
    ///      POST : /api/v1/flightplans
    ///      {
    ///         "flight_plan_id": "f1d193ad0153491f9cf61cbe39c7db70",
    ///         "altitude": 0,
    ///         "airspeed": 0,
    ///         "aircraft_identification" : "Hey You This is a test to create flight plan"
    ///         "aircraft_type" :"string",
    ///         "arrival_airport" : "string",
    ///         "departing_airport": "string"
    ///         "flight_type" : "string",
    ///         "eparture_time": 2024-01-30T10:33:09.733+00:00
    ///         "estimated_arrival_time" : 2024-01-30T10:33:09.733+00:00
    ///         "route": "string",
    ///         "remarks": "string",
    ///         "fuel_hours": 0,
    ///         "fuel_minutes": 0
    ///         "number_onboard": 4,
    ///         "numberOnBoard": 0
    ///      }
    /// </remarks>
    /// <param name="flightPlan">the flight plan info you want to add to the system</param>
    /// <response code="400">somthing wrong with the request you sent</response>
    /// <response code="201">the flight plan has been added to the system</response>
    /// <response code="401">You are not authorized to use the endpoint</response>
    /// <response code="406">The output format you entered in the "Accept header is not supported"</response>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> CreateFlightPlan(FlightPlan flightPlan)
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
    /// <summary>
    /// edit flight plan by providing its id
    /// add the flight plan id and provide new values throgh the request body
    /// </summary>
    /// <remarks>
    /// sample request
    /// 
    ///     PUT : /api/v1/flightplans 
    ///     {
    ///         "flight_plan_id": "f1d193ad0153491f9cf61cbe39c7db70",
    ///         "altitude": 0,
    ///         "airspeed": 0,
    ///         "aircraft_identification": "Hey You This is a test to update a field",
    ///         "aircraft_type": "string",
    ///         "arrival_airport": "string",
    ///         "departing_airport": "string",
    ///         "flight_type": "string",
    ///         "departure_time": "2024-01-30T10:33:09.733+00:00",
    ///         "estimated_arrival_time": "2024-01-30T10:33:09.733+00:00",
    ///         "route": "string",
    ///         "remarks": "string",
    ///         "fuel_hours": 0,
    ///         "fuel_minutes": 0,
    ///         "number_onboard": 4,
    ///         "numberOnBoard": 0
    ///     }
    /// </remarks>
    /// <param name="flightPlanId">the flight plan ID you want to edit its info</param>
    /// <response code="400">somthing wrong with the request you sent -usually the request body-</response>
    /// <response code="200">the flight plan has been updated</response>
    /// <response code="500">somthing went wrong on the server</response>
    [HttpPut]
    [Route("{flightPlan}")]
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
    //the other way of documenting the api
    /// <summary>
    /// Delete the flight plan by its id
    /// </summary>
    /// <param name="flightPlanId">the ID of the flight plan you to delete</param>
    /// <response code="404">There is no flight plan with this id</response>
    /// <response code="400">somthing wrong with the request you sent</response>
    /// <response code="200">the flight plan has been deleted</response>
    /// <response code="401">You are not authorized to use the endpoint</response>
    /// <response code="406">The output format you entered in the "Accept header is not supported"</response>
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
    //the other way of documenting the api
    /// <summary>
    /// Get the flight plan departure by its id
    /// </summary>
    /// <param name="flightPlanId">the ID of the flight plan you want to get its info</param>
    /// <response code="404">There is no flight plan with this id</response>
    /// <response code="400">somthing wrong with the request you sent</response>
    /// <response code="200">the flight plan departure</response>
    /// <response code="401">You are not authorized to use the endpoint</response>
    /// <response code="406">The output format you entered in the "Accept header is not supported"</response>
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
    //the other way of documenting the api
    /// <summary>
    /// Get the flight plan route by its id
    /// </summary>
    /// <param name="flightPlanId">the ID of the flight plan you want to get its info</param>
    /// <response code="404">There is no flight plan with this id</response>
    /// <response code="400">somthing wrong with the request you sent</response>
    /// <response code="200">the flight plan route</response>
    /// <response code="401">You are not authorized to use the endpoint</response>
    /// <response code="406">The output format you entered in the "Accept header is not supported"</response>
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
    //the other way of documenting the api
    /// <summary>
    /// Get the flight plan enroute time by its id
    /// </summary>
    /// <param name="flightPlanId">the ID of the flight plan you want to get its info</param>
    /// <response code="404">There is no flight plan with this id</response>
    /// <response code="400">somthing wrong with the request you sent</response>
    /// <response code="200">the flight plan enroute</response>
    /// <response code="401">You are not authorized to use the endpoint</response>
    /// <response code="406">The output format you entered in the "Accept header is not supported"</response>
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
