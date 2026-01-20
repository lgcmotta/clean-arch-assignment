using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json;
using OrderManagement.Domain.Aggregates.Customers.Exceptions;
using OrderManagement.Domain.Aggregates.Orders.Exceptions;
using OrderManagement.Domain.Aggregates.Products.Exceptions;

namespace OrderManagement.WebApi.Extensions;

internal static class ProblemDetailsExtensions
{
    extension(Exception exception)
    {
        internal ProblemDetails ToProblemDetails(string path)
        {
            return new ProblemDetails
            {
                Title = "Unknown Internal Error",
                Detail = $"An exception of type '{exception.GetType().Name}' occurred while processing the http request. Please, contact an administrator.",
                Status = StatusCodes.Status500InternalServerError,
                Instance = path,
                Extensions = new Dictionary<string, object?>
                {
                    ["trace_id"] = Activity.Current?.TraceId.ToString()
                }
            };
        }
    }

    extension(ValidationException exception)
    {
        internal ProblemDetails ToProblemDetails(string path)
        {
            return new ProblemDetails
            {
                Title = "Validation Error",
                Status = StatusCodes.Status400BadRequest,
                Detail = "One or more properties have errors",
                Instance = path,
                Extensions =
                {
                    ["trace_id"] = Activity.Current?.TraceId.ToString(),
                    ["errors"] = exception.Errors
                        .GroupBy(failure => JsonNamingPolicy.CamelCase.ConvertName(failure.PropertyName))
                        .ToDictionary(
                            grouping => grouping.Key,
                            grouping => grouping.Select(failure => failure.ErrorMessage).ToArray()
                        )
                }
            };
        }
    }

    extension(DuplicatedProductNameException exception)
    {
        internal ProblemDetails ToProblemDetails(string path)
        {
            return new ProblemDetails
            {
                Title = "Duplicated Product Name",
                Detail = exception.Message,
                Status = StatusCodes.Status409Conflict,
                Instance = path,
                Extensions = new Dictionary<string, object?>
                {
                    ["trace_id"] = Activity.Current?.TraceId.ToString()
                }
            };
        }
    }

    extension(ProductNotFoundForSyncException exception)
    {
        internal ProblemDetails ToProblemDetails(string path)
        {
            return new ProblemDetails
            {
                Title = "Product Not Found",
                Detail = exception.Message,
                Status = StatusCodes.Status404NotFound,
                Instance = path,
                Extensions = new Dictionary<string, object?>
                {
                    ["trace_id"] = Activity.Current?.TraceId.ToString()
                }
            };
        }
    }

    extension(CustomerEmailAlreadySignedUpException exception)
    {
        internal ProblemDetails ToProblemDetails(string path)
        {
            return new ProblemDetails
            {
                Title = "Customer Already Exists",
                Detail = exception.Message,
                Status = StatusCodes.Status409Conflict,
                Instance = path,
                Extensions = new Dictionary<string, object?>
                {
                    ["trace_id"] = Activity.Current?.TraceId.ToString()
                }
            };
        }
    }

    extension(CustomerNotFoundException exception)
    {
        internal ProblemDetails ToProblemDetails(string path)
        {
            return new ProblemDetails
            {
                Title = "Customer Not Found",
                Detail = exception.Message,
                Status = StatusCodes.Status404NotFound,
                Instance = path,
                Extensions = new Dictionary<string, object?>
                {
                    ["trace_id"] = Activity.Current?.TraceId.ToString()
                }
            };
        }
    }

    extension(OrderNotFoundException exception)
    {
        internal ProblemDetails ToProblemDetails(string path)
        {
            return new ProblemDetails
            {
                Title = "Order Not Found",
                Detail = exception.Message,
                Status = StatusCodes.Status404NotFound,
                Instance = path,
                Extensions = new Dictionary<string, object?>
                {
                    ["trace_id"] = Activity.Current?.TraceId.ToString()
                }
            };
        }
    }

    extension(OrderStatusTransitionException exception)
    {
        internal ProblemDetails ToProblemDetails(string path)
        {
            return new ProblemDetails
            {
                Title = "Invalid Order Status Transition",
                Detail = exception.Message,
                Status = StatusCodes.Status409Conflict,
                Instance = path,
                Extensions = new Dictionary<string, object?>
                {
                    ["trace_id"] = Activity.Current?.TraceId.ToString()
                }
            };
        }
    }

    extension(ProductNotFoundException exception)
    {
        internal ProblemDetails ToProblemDetails(string path)
        {
            return new ProblemDetails
            {
                Title = "Product Not Found",
                Detail = exception.Message,
                Status = StatusCodes.Status404NotFound,
                Instance = path,
                Extensions = new Dictionary<string, object?>
                {
                    ["trace_id"] = Activity.Current?.TraceId.ToString()
                }
            };
        }
    }

    extension(ProductsNotFoundException exception)
    {
        internal ProblemDetails ToProblemDetails(string path)
        {
            return new ProblemDetails
            {
                Title = "Products Not Found",
                Detail = exception.Message,
                Status = StatusCodes.Status404NotFound,
                Instance = path,
                Extensions = new Dictionary<string, object?>
                {
                    ["trace_id"] = Activity.Current?.TraceId.ToString()
                }
            };
        }
    }
}
