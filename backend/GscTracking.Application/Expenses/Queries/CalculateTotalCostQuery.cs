using MediatR;

namespace GscTracking.Application.Expenses.Queries;

public record CalculateTotalCostQuery(int JobId) : IRequest<decimal>;
