using Azure.Core;
using FluentAssertions;
using MediatR;
using RL.Backend.Commands;
using RL.Backend.Commands.Handlers.Procedure;
using RL.Backend.Exceptions;
using RL.Data;
using RL.Data.DataModels;

namespace RL.Backend.UnitTests;

[TestClass]
public class AssignUserCommandHandlerTests
{
    private RLContext _context = null!;
    private AssignUserCommandHandler _handler = null!;

    [TestInitialize]
    public void Setup()
    {
        _context = DbContextHelper.CreateContext();
        _handler = new AssignUserCommandHandler(_context);
    }

    [TestMethod]
    public async Task ShouldReturnsBadRequestWhenInvalidPlanIdReceived()
    {
        var command = new AssignUserCommand
        {
            PlanId = 0,
            ProcedureId = 1,
            UserId = 1
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Succeeded.Should().BeFalse();
        result.Exception.Should().BeOfType<BadRequestException>();
    }

    [TestMethod]
    public async Task ShouldReturnsBadRequestWhenInvalidProcedureIdReceived()
    {
        var command = new AssignUserCommand
        {
            PlanId = 1,
            ProcedureId = 0,
            UserId = 1
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Succeeded.Should().BeFalse();
        result.Exception.Should().BeOfType<BadRequestException>();
    }

    [TestMethod]
    public async Task ShouldReturnsBadRequestWhenInvalidUserIdReceived()
    {
        var command = new AssignUserCommand
        {
            PlanId = 1,
            ProcedureId = 1,
            UserId = 0
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Succeeded.Should().BeFalse();
        result.Exception.Should().BeOfType<BadRequestException>();
    }

    [TestMethod]
    public async Task ShouldReturnsNotFoundExceptionWhenPlanIsNotFound()
    {
        var planId = 1;
        _context.Plans.Add(new Data.DataModels.Plan
        {
            PlanId = planId + 1
        });
        await _context.SaveChangesAsync();
        var assignUserCommand = new AssignUserCommand
        {
            PlanId = planId + 1,
            ProcedureId = 1,
            UserId = 1

        };

        var result = await _handler.Handle(assignUserCommand, new CancellationToken());

        result.Succeeded.Should().BeFalse();
        result.Exception.Should().BeOfType(typeof(NotFoundException));
        result?.Exception?.Message.Should().Be("ProcedureId: 1 not added to PlanId: 2");
    }

    [TestMethod]
    public async Task ShouldReturnsNotFoundExceptionWhenProcedureIsnotAddedinPlan()
    {
        var planId = 1;
        var procedureId = 1;
        int differentProcedureId1 = 2, differentProcedureId2 = 3;
        _context.Plans.Add(new Plan
        {
            PlanId = planId
        });
        _context.Procedures.Add(new Procedure
        {
            ProcedureId = procedureId,
            ProcedureTitle = "Test Procedure"
        });
        _context.PlanProcedures.AddRange(
            new PlanProcedure
            {
                ProcedureId = differentProcedureId1,//different Procedure id
                PlanId = planId
            },
            new PlanProcedure
            {
                ProcedureId = differentProcedureId2,//different Procedure id
                PlanId = planId
            });
        await _context.SaveChangesAsync();
        var assignUserCommand = new AssignUserCommand
        {
            PlanId = planId,
            ProcedureId = procedureId,
            UserId = 1

        };

        var result = await _handler.Handle(assignUserCommand, new CancellationToken());

        result.Succeeded.Should().BeFalse();
        result.Exception.Should().BeOfType(typeof(NotFoundException));
        result?.Exception?.Message.Should().Be("ProcedureId: 1 not added to PlanId: 1");
    }

    [TestMethod]
    public async Task ShoulReturnsSuccessWhenUserAlreadyAssigned()
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
        _context.PlanProcedures.AddRange(
            new PlanProcedure
            {
                ProcedureId = procedureId,
                PlanId = planId
            });
        _context.Users.Add(new User
        {
            UserId = userId,
            Name = "User name"
        });
        _context.PlanProcedureUser.Add(new PlanProcedureUser
        {
            PlanId = planId,
            ProcedureId = procedureId,
            UserId = userId
        });
        await _context.SaveChangesAsync();
        var assignUserCommand = new AssignUserCommand
        {
            PlanId = planId,
            ProcedureId = procedureId,  
            UserId = userId
        };

        var result = await _handler.Handle(assignUserCommand, new CancellationToken());

        result.Value.Should().BeOfType(typeof(Unit));
        result.Succeeded.Should().BeTrue();
    }

    [TestMethod]
    public async Task ShoulReturnsNotFoundExceptionWhenNewUserReceived()
    {
        var planId = 1;
        var procedureId = 1;
        var userId = 1;
        var newUserId = 2;
        _context.Plans.Add(new Plan
        {
            PlanId = planId
        });
        _context.Procedures.Add(new Procedure
        {
            ProcedureId = procedureId,
            ProcedureTitle = "Test Procedure"
        });
        _context.PlanProcedures.AddRange(
            new PlanProcedure
            {
                ProcedureId = procedureId,
                PlanId = planId
            });
        _context.Users.Add(new User
        {
            UserId = userId,
            Name = "User name"
        });
        _context.PlanProcedureUser.Add(new PlanProcedureUser
        {
            PlanId = planId,
            ProcedureId = procedureId,
            UserId = userId
        });
        await _context.SaveChangesAsync();
        var assignUserCommand = new AssignUserCommand
        {
            PlanId = planId,
            ProcedureId = procedureId,
            UserId = newUserId
        };

        var result = await _handler.Handle(assignUserCommand, new CancellationToken());

        result.Succeeded.Should().BeFalse();
        result?.Exception?.Message.Should().Be("UserId: 2 not found");
        result.Exception.Should().BeOfType(typeof(NotFoundException));
    }

    [TestMethod]
    public async Task ShoulReturnsSuccessWhenUserAssied()
    {
        var planId = 1;
        var procedureId = 1;
        var userId = 1;
        var newUserId = 2;
        _context.Plans.Add(new Plan
        {
            PlanId = planId
        });
        _context.Procedures.Add(new Procedure
        {
            ProcedureId = procedureId,
            ProcedureTitle = "Test Procedure"
        });
        _context.PlanProcedures.AddRange(
            new PlanProcedure
            {
                ProcedureId = procedureId,
                PlanId = planId
            });
        _context.Users.AddRange(new User
        {
            UserId = userId,
            Name = "User name"
        },
        new User
        {
            UserId = newUserId,
            Name = "New User"
        });
        _context.PlanProcedureUser.Add(new PlanProcedureUser
        {
            PlanId = planId,
            ProcedureId = procedureId,
            UserId = userId
        });
        await _context.SaveChangesAsync();
        var assignUserCommand = new AssignUserCommand
        {
            PlanId = planId,
            ProcedureId = procedureId,
            UserId = newUserId
        };

        var result = await _handler.Handle(assignUserCommand, new CancellationToken());

        result.Succeeded.Should().BeTrue();
        _context.PlanProcedureUser.Should().ContainSingle(u => u.UserId == newUserId);
    }

    [TestMethod]
    public async Task ShouldReturnsFailureWhenExceptionIsThrown() 
    {
        var planId = 1;
        var procedureId = 1;
        var userId = 1;
        var newUserId = 2;
        _context.Plans.Add(new Plan
        {
            PlanId = planId
        });
        _context.Procedures.Add(new Procedure
        {
            ProcedureId = procedureId,
            ProcedureTitle = "Test Procedure"
        });
        _context.PlanProcedures.AddRange(
            new PlanProcedure
            {
                ProcedureId = procedureId,
                PlanId = planId
            });
        _context.Users.AddRange(new User
        {
            UserId = userId,
            Name = "User name"
        },
        new User
        {
            UserId = newUserId,
            Name = "New User"
        });
        _context.PlanProcedureUser.Add(new PlanProcedureUser
        {
            PlanId = planId,
            ProcedureId = procedureId,
            UserId = userId
        });
        await _context.SaveChangesAsync();

        _context.Dispose();
        var assignUserCommand = new AssignUserCommand
        {
            PlanId = planId,
            ProcedureId = procedureId,
            UserId = newUserId
        };

        var result = await _handler.Handle(assignUserCommand, new CancellationToken());

        result.Succeeded.Should().BeFalse();
        result.Exception.Should().NotBeNull();
        result.Exception.Should().BeOfType<ObjectDisposedException>();
    }

}
