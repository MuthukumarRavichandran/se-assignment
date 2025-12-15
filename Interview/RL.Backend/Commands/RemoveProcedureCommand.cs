using MediatR;
using RL.Backend.Models;

namespace RL.Backend.Commands;

/// <summary>
/// Command to remove procedure from plan
/// </summary>
public class RemoveProcedureCommand : IRequest<ApiResponse<Unit>>
{
    /// <summary>
    /// PlanId
    /// </summary>
    public int PlanId { get; set; }

    /// <summary>
    /// ProcedureId 
    /// </summary>
    public int ProcedureId { get; set; }
}
