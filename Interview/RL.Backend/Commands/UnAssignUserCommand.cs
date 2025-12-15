using MediatR;
using RL.Backend.Models;

namespace RL.Backend.Commands;

/// <summary>
/// Command to remove the user who were assign to the procedure in a plan
/// </summary>
public class UnAssignUserCommand : IRequest<ApiResponse<Unit>>
{
    /// <summary>
    /// PlanId
    /// </summary>
    public int PlanId { get; set; }

    /// <summary>
    /// ProcedureId
    /// </summary>
    public int ProcedureId { get; set; }

    /// <summary>
    /// UserId
    /// </summary>
    public int UserId { get; set; }
}
