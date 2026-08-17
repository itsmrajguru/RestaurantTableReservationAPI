using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace RestaurantTableReservationAPI.Filters;

public class SwaggerRoleOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var authAttributes = context.MethodInfo.DeclaringType?.GetCustomAttributes(true)
            .Union(context.MethodInfo.GetCustomAttributes(true))
            .OfType<AuthorizeAttribute>();

        if (authAttributes != null && authAttributes.Any())
        {
            var allowAnonymous = context.MethodInfo.GetCustomAttributes(true).OfType<AllowAnonymousAttribute>().Any();
            if (allowAnonymous)
            {
                operation.Summary += " [Public]";
                return;
            }

            var roles = authAttributes
                .Where(a => !string.IsNullOrEmpty(a.Roles))
                .Select(a => a.Roles)
                .Distinct()
                .ToList();

            var roleDescription = roles.Any() 
                ? $"**Allowed Roles: {string.Join(", ", roles)}**" 
                : "**Allowed Roles: Authenticated Users (Customer or Admin)**";

            // Add the bold role text to the endpoint description
            operation.Description = string.IsNullOrEmpty(operation.Description)
                ? roleDescription
                : $"{operation.Description}\n\n{roleDescription}";
            
            // Add a brief tag to the summary
            var summaryTag = roles.Any() ? string.Join(", ", roles) : "Auth";
            operation.Summary += $" [{summaryTag}]";
        }
        else
        {
            operation.Summary += " [Public]";
        }
    }
}
