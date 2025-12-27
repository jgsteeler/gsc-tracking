using MediatR;

namespace GscTracking.Application.Jobs.Commands;

public record DeleteJobCommand(int Id) : IRequest<bool>;
