using MediatR;
using Microsoft.EntityFrameworkCore;
using RL.Backend.Exceptions;
using RL.Backend.Models;
using RL.Data;

namespace RL.Backend.Commands.Handlers.Procedure;

/// <summary>
/// Handler to unassign the user from the procedure in plan
/// </summary>
public class UnAssignUserCommandHandler : IRequestHandler<UnAssignUserCommand, ApiResponse<Unit>>
{
    private readonly RLContext _context;

    /// <summary>
    /// Constructor takes the context
    /// </summary>
    /// <param name="context"></param>
    public UnAssignUserCommandHandler(RLContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse<Unit>> Handle(UnAssignUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _context.PlanProcedureUser.FirstOrDefaultAsync(ppu =>
                ppu.PlanId == request.PlanId &&
                ppu.ProcedureId == request.ProcedureId &&
                ppu.UserId == request.UserId);

            if (user == null)
                return ApiResponse<Unit>.Fail(
                    new NotFoundException(
                        $"UserId: {request.UserId} not assigned yet to ProcedureId: {request.ProcedureId} in the PlanId:{request.PlanId}"));

            _context.PlanProcedureUser.Remove(user);

            await _context.SaveChangesAsync(cancellationToken);

            return ApiResponse<Unit>.Succeed(new Unit());
        }
        catch (Exception ex)
        {
            return ApiResponse<Unit>.Fail(ex);
        }
    }
}
