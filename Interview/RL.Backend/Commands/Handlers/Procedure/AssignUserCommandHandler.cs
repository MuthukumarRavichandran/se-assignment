using MediatR;
using Microsoft.EntityFrameworkCore;
using RL.Backend.Exceptions;
using RL.Backend.Models;
using RL.Data;
using RL.Data.DataModels;

namespace RL.Backend.Commands.Handlers.Procedure;

/// <summary>
/// Handler to assign the user to the procedure in plan
/// </summary>
public class AssignUserCommandHandler : IRequestHandler<AssignUserCommand, ApiResponse<Unit>>
{
    private readonly RLContext _context;

    /// <summary>
    /// Constructor takes the context
    /// </summary>
    /// <param name="context"></param>
    public AssignUserCommandHandler(RLContext context)
    {
        _context = context;
    }

    /// <summary>
    /// handle the AssignUserCommand
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<ApiResponse<Unit>> Handle(AssignUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.PlanId < 1)
                return ApiResponse<Unit>.Fail(new BadRequestException("Invalid PlanId"));

            if (request.ProcedureId < 1)
                return ApiResponse<Unit>.Fail(new BadRequestException("Invalid ProcedureId"));

            if (request.UserId < 1)
                return ApiResponse<Unit>.Fail(new BadRequestException("Invalid UserId"));

            var plan = await _context.Plans
                .Include(p => p.PlanProcedures)
                    .ThenInclude(pp => pp.PlanProcedureUser)
                .FirstOrDefaultAsync(p => p.PlanId == request.PlanId);

            if (plan is null)
                return ApiResponse<Unit>.Fail(
                    new NotFoundException($"PlanId: {request.PlanId} not found"));

            var planProcedure = plan.PlanProcedures
                .FirstOrDefault(pp => pp.ProcedureId == request.ProcedureId);

            if (planProcedure is null)
                return ApiResponse<Unit>.Fail(
                    new NotFoundException(
                        $"ProcedureId: {request.ProcedureId} not added to PlanId: {request.PlanId}"));

            if (planProcedure.PlanProcedureUser
                .Any(u => u.UserId == request.UserId))
            {
                return ApiResponse<Unit>.Succeed(new Unit());
            }

            var userExists = await _context.Users
                .AnyAsync(u => u.UserId == request.UserId);

            if (!userExists)
                return ApiResponse<Unit>.Fail(
                    new NotFoundException($"UserId: {request.UserId} not found"));

            planProcedure.PlanProcedureUser.Add(new PlanProcedureUser
            {
                PlanId = request.PlanId,
                ProcedureId = request.ProcedureId,
                UserId = request.UserId,
                CreateDate = DateTime.UtcNow,
                UpdateDate = DateTime.UtcNow
            });

            await _context.SaveChangesAsync(cancellationToken);

            return ApiResponse<Unit>.Succeed(new Unit());

        }
        catch (Exception ex)
        {
            return ApiResponse<Unit>.Fail(ex);
        }
    }
}
