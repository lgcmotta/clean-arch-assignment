namespace OrderManagement.Infrastructure.Persistence.Exceptions;

public class SqlServerConnectionStringNotFoundException()
    : Exception("""
                SQL Server connection string cannot be empty or white-space. 
                Please, specify the connection string either using '--connection' or 
                by setting the environment variable 'ConnectionStrings_SqlServer'
                """
    );