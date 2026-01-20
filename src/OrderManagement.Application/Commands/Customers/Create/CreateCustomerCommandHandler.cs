using HashidsNet;
using MediatR;
using OrderManagement.Domain.Aggregates.Customers;
using OrderManagement.Domain.Aggregates.Customers.Exceptions;
using OrderManagement.Domain.Aggregates.Customers.Repositories;

namespace OrderManagement.Application.Commands.Customers.Create;

internal sealed class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, CreateCustomerResponse>
{
    private readonly ICustomerWriteRepository _repository;
    private readonly IHashids _hashids;

    public CreateCustomerCommandHandler(ICustomerWriteRepository repository, IHashids hashids)
    {
        _repository = repository;
        _hashids = hashids;
    }

    public async Task<CreateCustomerResponse> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        if (await _repository.CheckIfEmailIsAlreadyTakenAsync(request.Email, cancellationToken))
        {
            throw new CustomerEmailAlreadySignedUpException(request.Email);
        }

        var customer = new Customer(request.Name, request.Email, request.Phone);

        await _repository.AddNewCustomerAsync(customer, cancellationToken);

        await _repository.SaveChangesAsync(cancellationToken);

        customer.RaiseCustomerCreatedDomainEvent();

        var customerId = _hashids.EncodeLong(customer.Id);

        return new CreateCustomerResponse(customerId, customer.Name, customer.Email, customer.Phone);
    }
}