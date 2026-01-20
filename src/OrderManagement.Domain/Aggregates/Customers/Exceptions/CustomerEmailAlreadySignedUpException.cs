namespace OrderManagement.Domain.Aggregates.Customers.Exceptions;

public class CustomerEmailAlreadySignedUpException(string email) : Exception($"Customer with email '{email}' already signed up.");