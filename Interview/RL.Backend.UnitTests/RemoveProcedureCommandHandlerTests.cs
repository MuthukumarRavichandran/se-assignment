using FluentAssertions;
using MediatR;
using RL.Backend.Commands;
using RL.Backend.Commands.Handlers.Procedure;
using RL.Backend.Exceptions;
using RL.Data;
using RL.Data.DataModels;

namespace RL.Backend.UnitTests;

[TestClass]
public class RemoveProcedureCommandHandlerTests
{
    private RLContext _context = null!;
    private RemoveProcedureCommandHandler _handler = null!;

    [TestInitialize]
    public void Setup()
    {
        _context = DbContextHelper.CreateContext();
        _handler = new RemoveProcedureCommandHandler(_context);
    }

    [TestMethod]
    public async Task ShouldReturnsNotFoundExceptionWhenProcedureIsNotAddedToPlan()
    {
        var planId = 1;
        var procedureId = 1;

        _context.Plans.Add(new Plan
        {
            PlanId = planId
        });

        await _context.SaveChangesAsync();

        var command = new RemoveProcedureCommand
        {
            PlanId = planId,
            ProcedureId = procedureId
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Succeeded.Should().BeFalse();
        result.Exception.Should().BeOfType<NotFoundException>();
        result.Exception?.Message.Should()
            .Be($"PlanId: {planId} not found");
    }

    [TestMethod]
    public async Task ShouldRemoveProcedureAndAssociatedUsersSuccessfully()
    {
        var planId = 1;
        var procedureId = 1;

        _context.Plans.Add(new Plan
        {
            PlanId = planId
        });

        _context.Procedures.Add(new Procedure
        {
            ProcedureId = procedureId,
            ProcedureTitle = "Test Procedure"
        });

        _context.PlanProcedures.Add(new PlanProcedure
        {
            PlanId = planId,
            ProcedureId = procedureId
        });

        _context.Users.AddRange(
            new User { UserId = 1, Name = "User 1" },
            new User { UserId = 2, Name = "User 2" }
        );

        _context.PlanProcedureUser.AddRange(
            new PlanProcedureUser
            {
                PlanId = planId,
                ProcedureId = procedureId,
                UserId = 1
            },
            new PlanProcedureUser
            {
                PlanId = planId,
                ProcedureId = procedureId,
                UserId = 2
            }
        );

        await _context.SaveChangesAsync();

        var command = new RemoveProcedureCommand
        {
            PlanId = planId,
            ProcedureId = procedureId
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Succeeded.Should().BeTrue();
        result.Value.Should().BeOfType<Unit>();

        _context.PlanProcedures
            .Should()
            .NotContain(pp => pp.PlanId == planId && pp.ProcedureId == procedureId);

        _context.PlanProcedureUser
            .Should()
            .NotContain(u => u.PlanId == planId && u.ProcedureId == procedureId);
    }

    [TestMethod]
    public async Task ShouldReturnsFailureWhenExceptionIsThrown()
    {
        var planId = 1;
        var procedureId = 1;

        _context.PlanProcedures.Add(new PlanProcedure
        {
            PlanId = planId,
            ProcedureId = procedureId
        });

        await _context.SaveChangesAsync();

        _context.Dispose();

        var command = new RemoveProcedureCommand
        {
            PlanId = planId,
            ProcedureId = procedureId
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Succeeded.Should().BeFalse();
        result.Exception.Should().NotBeNull();
        result.Exception.Should().BeOfType<ObjectDisposedException>();
    }
}
