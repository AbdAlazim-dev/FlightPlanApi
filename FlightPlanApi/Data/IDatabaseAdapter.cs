using FlightPlanApi.Models;

namespace FlightPlanApi.Data
{
    public interface IDatabaseAdapter
    {
        Task<List<FlightPlan>> GetFlightPlans();
        Task<FlightPlan> GetFlightPlan(string flightPlanId);
        Task<TransactionResult> FileFlightPlan(FlightPlan flightPlan);
        Task<TransactionResult> UpdateFlightPlan(string flightPlanId, FlightPlan flightPlan);
        Task<bool> DeleteFightPlanById(string flightPlanId);
    }
}
