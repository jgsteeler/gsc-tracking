using MediatR;

namespace GscTracking.Application.JobUpdates.Commands;

public record DeleteJobUpdateCommand(int Id) : IRequest<bool>;
