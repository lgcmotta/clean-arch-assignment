namespace OrderManagement.Domain.Aggregates.Customers.Exceptions;

public class CustomerNotFoundException(string customerId) : Exception($"Customer with id '{customerId}' was not found");