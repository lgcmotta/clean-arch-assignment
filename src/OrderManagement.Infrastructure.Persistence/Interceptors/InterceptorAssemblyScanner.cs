using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace OrderManagement.Infrastructure.Persistence.Interceptors;

public static class InterceptorAssemblyScanner
{
    public static IEnumerable<IInterceptor> Scan(IServiceProvider? serviceProvider = null, params Assembly[] interceptorAssemblies)
    {
        if (interceptorAssemblies.Length == 0)
        {
            return [];
        }

        return interceptorAssemblies
            .Distinct()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => type is { IsClass: true, IsAbstract: false } &&
                           type.IsAssignableTo(typeof(IInterceptor)) &&
                           type.GetConstructor(
                               bindingAttr: BindingFlags.Instance | BindingFlags.Public,
                               binder: null,
                               types: Type.EmptyTypes,
                               modifiers: null) is not null)
            .Select(type => serviceProvider is null
                ? Activator.CreateInstance(type)
                : ActivatorUtilities.CreateInstance(serviceProvider, type))
            .Cast<IInterceptor>();
    }
}