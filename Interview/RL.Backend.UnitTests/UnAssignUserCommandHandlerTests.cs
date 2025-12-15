using FluentAssertions;
using MediatR;
using RL.Backend.Commands;
using RL.Backend.Commands.Handlers.Procedure;
using RL.Backend.Exceptions;
using RL.Data;
using RL.Data.DataModels;

namespace RL.Backend.UnitTests;

[TestClass]
public class UnAssignUserCommandHandlerTests
{
    private RLContext _context = null!;
    private UnAssignUserCommandHandler _handler = null!;

    [TestInitialize]
    public void Setup()
    {
        _context = DbContextHelper.CreateContext();
        _handler = new UnAssignUserCommandHandler(_context);
    }

    [TestMethod]
    public async Task ShouldReturnsNotFoundExceptionWhenUserIsNotAssigned()
    {
        var planId = 1;
        var procedureId = 1;
        var userId = 1;

        _context.Plans.Add(new Plan
        {
            PlanId = planId
        });

        _context.Procedures.Add(new Procedure
        {
            ProcedureId = procedureId,
            ProcedureTitle = "Test Procedure"
        });

        _context.Users.Add(new User
        {
            UserId = userId,
            Name = "User Name"
        });

        await _context.SaveChangesAsync();

        var command = new UnAssignUserCommand
        {
            PlanId = planId,
            ProcedureId = procedureId,
            UserId = userId
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Succeeded.Should().BeFalse();
        result.Exception.Should().BeOfType<NotFoundException>();
        result.Exception?.Message.Should()
            .Be($"UserId: {userId} not assigned yet to ProcedureId: {procedureId} in the PlanId:{planId}");
    }

    [TestMethod]
    public async Task ShouldReturnsSuccessWhenUserIsUnAssigned()
    {
        var planId = 1;
        var procedureId = 1;
        var userId = 1;

        _context.Plans.Add(new Plan
        {
            PlanId = planId
        });

        _context.Procedures.Add(new Procedure
        {
            ProcedureId = procedureId,
            ProcedureTitle = "Test Procedure"
        });

        _context.Users.Add(new User
        {
            UserId = userId,
            Name = "User Name"
        });

        _context.PlanProcedureUser.Add(new PlanProcedureUser
        {
            PlanId = planId,
            ProcedureId = procedureId,
            UserId = userId
        });

        await _context.SaveChangesAsync();

        var command = new UnAssignUserCommand
        {
            PlanId = planId,
            ProcedureId = procedureId,
            UserId = userId
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Succeeded.Should().BeTrue();
        result.Value.Should().BeOfType<Unit>();

        _context.PlanProcedureUser.Should().BeEmpty();
    }

    [TestMethod]
    public async Task ShouldReturnsFailureWhenExceptionIsThrown()
    {
        var planId = 1;
        var procedureId = 1;
        var userId = 1;

        _context.PlanProcedureUser.Add(new PlanProcedureUser
        {
            PlanId = planId,
            ProcedureId = procedureId,
            UserId = userId
        });

        await _context.SaveChangesAsync();

        _context.Dispose();

        var command = new UnAssignUserCommand
        {
            PlanId = planId,
            ProcedureId = procedureId,
            UserId = userId
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Succeeded.Should().BeFalse();
        result.Exception.Should().NotBeNull();
        result.Exception.Should().BeOfType<ObjectDisposedException>();
    }
}
