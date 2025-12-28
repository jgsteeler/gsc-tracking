using GscTracking.Application.DTOs;
using MediatR;

namespace GscTracking.Application.JobUpdates.Queries;

public record GetJobUpdateByIdQuery(int Id) : IRequest<JobUpdateDto?>;
