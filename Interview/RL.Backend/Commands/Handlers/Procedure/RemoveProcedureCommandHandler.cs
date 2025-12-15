using MediatR;
using Microsoft.EntityFrameworkCore;
using RL.Backend.Exceptions;
using RL.Backend.Models;
using RL.Data;

namespace RL.Backend.Commands.Handlers.Procedure
{
    /// <summary>
    /// Handler to remove the procedure from the plan
    /// </summary>
    public class RemoveProcedureCommandHandler : IRequestHandler<RemoveProcedureCommand, ApiResponse<Unit>>
    {
        private readonly RLContext _context;

        /// <summary>
        /// Constructor takes the context
        /// </summary>
        /// <param name="context"></param>
        public RemoveProcedureCommandHandler(RLContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<Unit>> Handle(RemoveProcedureCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var planProcedure = await _context.PlanProcedures
                    .FirstOrDefaultAsync(pp => pp.PlanId == request.PlanId && 
                    pp.ProcedureId == request.ProcedureId);

                if (planProcedure == null)
                    return ApiResponse<Unit>.Fail(new NotFoundException($"PlanId: {request.PlanId} not found"));

                var users = await _context.PlanProcedureUser
                    .Where(ppu => ppu.PlanId == request.PlanId && 
                    ppu.ProcedureId == request.ProcedureId).ToListAsync();

                _context.PlanProcedureUser.RemoveRange(users);
                _context.PlanProcedures.Remove(planProcedure);

                await _context.SaveChangesAsync();

                return ApiResponse<Unit>.Succeed(new Unit());
            }
            catch (Exception ex)
            {
                return ApiResponse<Unit>.Fail(ex);
            }
        }
    }
}
