using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;
using RL.Backend.Commands;
using RL.Backend.Models;
using RL.Data;
using RL.Data.DataModels;

namespace RL.Backend.Controllers;

/// <summary>
/// Controller to manage the user for a procedure in plan
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class PlanProcedureController : ControllerBase
{
    private readonly ILogger<PlanProcedureController> _logger;
    private readonly IMediator _mediator;
    private readonly RLContext _context;

    public PlanProcedureController(ILogger<PlanProcedureController> logger, RLContext context, IMediator mediator)
    {
        _logger = logger;
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mediator = mediator;
    }

    /// <summary>
    /// Endpoint to retrive all the users assigned to the procedure for a plans
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [EnableQuery]
    public IEnumerable<PlanProcedureUser> Get()
    {
        return _context.PlanProcedureUser;
    }

    /// <summary>
    /// Endpoint to retrive all the users assigned to the procedure for a plan
    /// </summary>
    /// <param name="planId"></param>
    /// <returns></returns>
    [HttpGet("{planId}")]
    public async Task<IActionResult> GetByPlan(int planId)
    {
        try
        {
            var plan = await _context.Plans
                .Where(p => p.PlanId == planId)
                .Select(p => new
                {
                    p.PlanId,
                    PlanProcedures = p.PlanProcedures.Select(pp => new
                    {
                        pp.ProcedureId,
                        Procedure = new
                        {
                            pp.Procedure.ProcedureId,
                            pp.Procedure.ProcedureTitle
                        },
                        AssignedUsers = pp.PlanProcedureUser.Select(ppu => new
                        {
                            ppu.User.UserId,
                            ppu.User.Name
                        })
                    })
                })
                .FirstOrDefaultAsync();

            if (plan == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Plan not found"
                });
            }

            return Ok(plan);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                message = "An error occurred while fetching plan details",
                detail = ex.Message
            });
        }
    }


    /// <summary>
    /// Endpoint to assign a user to the procedure
    /// </summary>
    /// <param name="assignUserCommand"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    [HttpPost("assign-user")]
    public async Task<IActionResult> AssignUser([FromBody] AssignUserCommand assignUserCommand, CancellationToken token)
    {
        var response = await _mediator.Send(assignUserCommand, token);

        return response.ToActionResult();
    }

    /// <summary>
    /// Endpoint to un assign a user from the procedure
    /// </summary>
    /// <param name="unAssignUserCommand"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    [HttpPost("unassign-user")]
    public async Task<IActionResult> UnassignUser([FromBody] UnAssignUserCommand unAssignUserCommand, CancellationToken token)
    {
        var response = await _mediator.Send(unAssignUserCommand, token);

        return response.ToActionResult();
    }

    /// <summary>
    ///  Endpoint to clear all the user in the procedure
    /// </summary>
    /// <param name="clearUsersCommand"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    [HttpPost("clear-users")]
    public async Task<IActionResult> ClearUsersFromProcedure([FromBody] ClearUsersCommand clearUsersCommand, CancellationToken token)
    {
        var response = await _mediator.Send(clearUsersCommand, token);

        return response.ToActionResult();
    }

    /// <summary>
    /// Endpoint to remove the procedure from the plan
    /// </summary>
    /// <param name="removeProcedureCommand"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    [HttpPost("remove-procedure")]
    public async Task<IActionResult> RemoveProcedureFromPlan([FromBody] RemoveProcedureCommand removeProcedureCommand, CancellationToken token)
    {
        var response = await _mediator.Send(removeProcedureCommand, token);

        return response.ToActionResult();
    }
}