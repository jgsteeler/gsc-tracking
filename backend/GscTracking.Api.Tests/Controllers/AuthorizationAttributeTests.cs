using System.Reflection;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GscTracking.Api.Controllers;

namespace GscTracking.Api.Tests.Controllers;

/// <summary>
/// Tests to verify authorization attributes are correctly applied to controllers and actions.
/// </summary>
public class AuthorizationAttributeTests
{
    [Theory]
    [InlineData(typeof(CustomersController), "ReadAccess")]
    [InlineData(typeof(JobsController), "ReadAccess")]
    [InlineData(typeof(ExpensesController), "ReadAccess")]
    [InlineData(typeof(JobUpdatesController), "ReadAccess")]
    [InlineData(typeof(ExportController), "ReadAccess")]
    [InlineData(typeof(ImportController), "AdminOnly")]
    public void Controller_ShouldHaveCorrectAuthorizePolicy(Type controllerType, string expectedPolicy)
    {
        // Act
        var authorizeAttribute = controllerType.GetCustomAttribute<AuthorizeAttribute>();

        // Assert
        authorizeAttribute.Should().NotBeNull($"{controllerType.Name} should have [Authorize] attribute");
        authorizeAttribute!.Policy.Should().Be(expectedPolicy, $"{controllerType.Name} should require {expectedPolicy} policy");
    }

    [Theory]
    [InlineData(typeof(CustomersController), nameof(CustomersController.GetCustomers))]
    [InlineData(typeof(CustomersController), nameof(CustomersController.GetCustomer))]
    [InlineData(typeof(JobsController), nameof(JobsController.GetJobs))]
    [InlineData(typeof(JobsController), nameof(JobsController.GetJob))]
    [InlineData(typeof(JobsController), nameof(JobsController.GetJobsByCustomer))]
    [InlineData(typeof(ExpensesController), nameof(ExpensesController.GetExpenses))]
    [InlineData(typeof(ExpensesController), nameof(ExpensesController.GetExpenseById))]
    [InlineData(typeof(JobUpdatesController), nameof(JobUpdatesController.GetJobUpdates))]
    [InlineData(typeof(JobUpdatesController), nameof(JobUpdatesController.GetJobUpdate))]
    public void GetEndpoint_ShouldNotHaveMethodLevelPolicy(Type controllerType, string methodName)
    {
        // Arrange
        var method = controllerType.GetMethod(methodName);

        // Act
        var authorizeAttributes = method?.GetCustomAttributes<AuthorizeAttribute>();

        // Assert
        method.Should().NotBeNull($"{controllerType.Name}.{methodName} should exist");
        authorizeAttributes.Should().BeEmpty($"{controllerType.Name}.{methodName} should rely on controller-level policy");
    }

    [Theory]
    [InlineData(typeof(CustomersController), nameof(CustomersController.CreateCustomer), "AdminOnly")]
    [InlineData(typeof(CustomersController), nameof(CustomersController.UpdateCustomer), "AdminOnly")]
    [InlineData(typeof(CustomersController), nameof(CustomersController.DeleteCustomer), "AdminOnly")]
    [InlineData(typeof(JobsController), nameof(JobsController.CreateJob), "AdminOnly")]
    [InlineData(typeof(JobsController), nameof(JobsController.UpdateJob), "AdminOnly")]
    [InlineData(typeof(JobsController), nameof(JobsController.DeleteJob), "AdminOnly")]
    [InlineData(typeof(ExpensesController), nameof(ExpensesController.CreateExpense), "WriteAccess")]
    [InlineData(typeof(ExpensesController), nameof(ExpensesController.UpdateExpense), "WriteAccess")]
    [InlineData(typeof(ExpensesController), nameof(ExpensesController.DeleteExpense), "AdminOnly")]
    [InlineData(typeof(JobUpdatesController), nameof(JobUpdatesController.CreateJobUpdate), "WriteAccess")]
    [InlineData(typeof(JobUpdatesController), nameof(JobUpdatesController.DeleteJobUpdate), "AdminOnly")]
    public void ModificationEndpoint_ShouldHaveCorrectPolicy(Type controllerType, string methodName, string expectedPolicy)
    {
        // Arrange
        var method = controllerType.GetMethod(methodName);

        // Act
        var authorizeAttributes = method?.GetCustomAttributes<AuthorizeAttribute>();
        var policyAttribute = authorizeAttributes?.FirstOrDefault(a => a.Policy == expectedPolicy);

        // Assert
        method.Should().NotBeNull($"{controllerType.Name}.{methodName} should exist");
        policyAttribute.Should().NotBeNull($"{controllerType.Name}.{methodName} should have [Authorize(Policy = \"{expectedPolicy}\")] attribute");
    }

    [Theory]
    [InlineData(typeof(CustomersController), nameof(CustomersController.GetCustomers), "GET")]
    [InlineData(typeof(CustomersController), nameof(CustomersController.GetCustomer), "GET")]
    [InlineData(typeof(CustomersController), nameof(CustomersController.CreateCustomer), "POST")]
    [InlineData(typeof(CustomersController), nameof(CustomersController.UpdateCustomer), "PUT")]
    [InlineData(typeof(CustomersController), nameof(CustomersController.DeleteCustomer), "DELETE")]
    [InlineData(typeof(JobsController), nameof(JobsController.GetJobs), "GET")]
    [InlineData(typeof(JobsController), nameof(JobsController.GetJob), "GET")]
    [InlineData(typeof(JobsController), nameof(JobsController.CreateJob), "POST")]
    [InlineData(typeof(JobsController), nameof(JobsController.UpdateJob), "PUT")]
    [InlineData(typeof(JobsController), nameof(JobsController.DeleteJob), "DELETE")]
    [InlineData(typeof(ExpensesController), nameof(ExpensesController.GetExpenses), "GET")]
    [InlineData(typeof(ExpensesController), nameof(ExpensesController.CreateExpense), "POST")]
    [InlineData(typeof(ExpensesController), nameof(ExpensesController.UpdateExpense), "PUT")]
    [InlineData(typeof(ExpensesController), nameof(ExpensesController.DeleteExpense), "DELETE")]
    [InlineData(typeof(JobUpdatesController), nameof(JobUpdatesController.GetJobUpdates), "GET")]
    [InlineData(typeof(JobUpdatesController), nameof(JobUpdatesController.CreateJobUpdate), "POST")]
    [InlineData(typeof(JobUpdatesController), nameof(JobUpdatesController.DeleteJobUpdate), "DELETE")]
    public void Endpoint_ShouldHaveCorrectHttpMethod(Type controllerType, string methodName, string expectedHttpMethod)
    {
        // Arrange
        var method = controllerType.GetMethod(methodName);

        // Act
        Attribute? httpMethodAttribute = expectedHttpMethod switch
        {
            "GET" => method?.GetCustomAttribute<HttpGetAttribute>(),
            "POST" => method?.GetCustomAttribute<HttpPostAttribute>(),
            "PUT" => method?.GetCustomAttribute<HttpPutAttribute>(),
            "DELETE" => method?.GetCustomAttribute<HttpDeleteAttribute>(),
            _ => null
        };

        // Assert
        method.Should().NotBeNull($"{controllerType.Name}.{methodName} should exist");
        httpMethodAttribute.Should().NotBeNull($"{controllerType.Name}.{methodName} should have [Http{expectedHttpMethod}] attribute");
    }
}
