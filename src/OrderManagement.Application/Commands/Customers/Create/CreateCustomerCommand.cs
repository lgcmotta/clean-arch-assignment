using JetBrains.Annotations;
using MediatR;
using OrderManagement.Domain.Core;

namespace OrderManagement.Application.Commands.Customers.Create;

[UsedImplicitly]
public sealed record CreateCustomerCommand(string Name, string Email, string Phone) : IRequest<CreateCustomerResponse>, ICommand;

public sealed record CreateCustomerResponse(string Id, string Name, string Email, string Phone);