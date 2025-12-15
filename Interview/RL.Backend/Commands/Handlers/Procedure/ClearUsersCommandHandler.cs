using MediatR;
using Microsoft.EntityFrameworkCore;
using RL.Backend.Exceptions;
using RL.Backend.Models;
using RL.Data;

namespace RL.Backend.Commands.Handlers.Procedure;

/// <summary>
/// Handler to clear all the user from the procedure in plan
/// </summary>
public class ClearUsersCommandHandler : IRequestHandler<ClearUsersCommand, ApiResponse<Unit>>
{
    
    private readonly RLContext _context;

    /// <summary>
    /// Constructor takes the context
    /// </summary>
    /// <param name="context"></param>
    public ClearUsersCommandHandler(RLContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse<Unit>> Handle(ClearUsersCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var users = await _context.PlanProcedureUser
                .Where(ppu => ppu.PlanId == request.PlanId && 
                ppu.ProcedureId == request.ProcedureId).ToListAsync();

            if (!users.Any())
                return ApiResponse<Unit>.Fail(
                    new NotFoundException(
                        $"ProcedureId: {request.ProcedureId} not added to PlanId: {request.PlanId}"));

            _context.PlanProcedureUser.RemoveRange(users);
            await _context.SaveChangesAsync(cancellationToken);

            return ApiResponse<Unit>.Succeed(new Unit());
        }
        catch (Exception ex)
        {
            return ApiResponse<Unit>.Fail(ex);
        }
    }
}
