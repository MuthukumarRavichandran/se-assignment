using MediatR;
using RL.Backend.Models;

namespace RL.Backend.Commands;

/// <summary>
/// Command to clear all the users from the procedure in a plan
/// </summary>
public class ClearUsersCommand : IRequest<ApiResponse<Unit>>
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
