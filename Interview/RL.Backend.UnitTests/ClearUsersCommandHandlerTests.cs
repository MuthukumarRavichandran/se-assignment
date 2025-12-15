using FluentAssertions;
using MediatR;
using RL.Backend.Commands;
using RL.Backend.Commands.Handlers.Procedure;
using RL.Backend.Exceptions;
using RL.Data;
using RL.Data.DataModels;

namespace RL.Backend.UnitTests;

[TestClass]
public class ClearUsersCommandHandlerTests
{
    private RLContext _context = null!;
    private ClearUsersCommandHandler _handler = null!;

    [TestInitialize]
    public void Setup()
    {
        _context = DbContextHelper.CreateContext();
        _handler = new ClearUsersCommandHandler(_context);
    }

    [TestMethod]
    public async Task ShouldReturnsNotFoundExceptionWhenProcedureisNotInPlan()
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

        await _context.SaveChangesAsync();

        var clearUsersCommand = new ClearUsersCommand
        {
            PlanId = planId,
            ProcedureId = procedureId
        };

        var result = await _handler.Handle(clearUsersCommand, CancellationToken.None);

        result.Succeeded.Should().BeFalse();
        result.Exception.Should().BeOfType<NotFoundException>();
        result.Exception?.Message
            .Should().Be("ProcedureId: 1 not added to PlanId: 1");
    }

    [TestMethod]
    public async Task ShouldReturnsSuccessWhenUsersAreCleared()
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

        var clearUsersCommand = new ClearUsersCommand
        {
            PlanId = planId,
            ProcedureId = procedureId
        };

        var result = await _handler.Handle(clearUsersCommand, CancellationToken.None);

        result.Succeeded.Should().BeTrue();
        result.Value.Should().BeOfType<Unit>();

        _context.PlanProcedureUser
            .Should()
            .NotContain(u => u.PlanId == planId && u.ProcedureId == procedureId);
    }

    [TestMethod]
    public async Task ShouldClearOnlyUsersForGivenPlanAndProcedure()
    {
        var planId = 1;
        var procedureId = 1;

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
                ProcedureId = 2, // different procedure
                UserId = 2
            },
            new PlanProcedureUser
            {
                PlanId = 2, // different plan
                ProcedureId = procedureId,
                UserId = 3
            }
        );

        await _context.SaveChangesAsync();

        var clearUsersCommand = new ClearUsersCommand
        {
            PlanId = planId,
            ProcedureId = procedureId
        };

        var result = await _handler.Handle(clearUsersCommand, CancellationToken.None);

        result.Succeeded.Should().BeTrue();

        _context.PlanProcedureUser.Should().HaveCount(2);
        _context.PlanProcedureUser.Should().NotContain(u =>
            u.PlanId == planId && u.ProcedureId == procedureId);
    }

    [TestMethod]
    public async Task ShouldReturnsFailureWhenExceptionIsThrown()
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

        _context.Users.Add(new User
        {
            UserId = 1,
            Name = "User"
        });

        _context.PlanProcedureUser.Add(new PlanProcedureUser
        {
            PlanId = planId,
            ProcedureId = procedureId,
            UserId = 1
        });

        await _context.SaveChangesAsync();

        _context.Dispose();

        var clearUsersCommand = new ClearUsersCommand
        {
            PlanId = planId,
            ProcedureId = procedureId
        };

        var result = await _handler.Handle(clearUsersCommand, CancellationToken.None);

        result.Succeeded.Should().BeFalse();
        result.Exception.Should().NotBeNull();
        result.Exception.Should().BeOfType<ObjectDisposedException>();
    }
}
