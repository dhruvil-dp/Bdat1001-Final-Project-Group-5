using System.Threading.Tasks;
using ContactManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace ContactManager.Authorization
{
    // The ContactAdministratorsAuthorizationHandler is a custom authorization handler that
    // checks if the current user has the required permissions to perform certain operations on Contact objects.
    public class ContactAdministratorsAuthorizationHandler
                    : AuthorizationHandler<OperationAuthorizationRequirement, Contact>
    {
        // This method is called by the authorization system to handle the authorization requirement.
        // It checks if the user has the necessary permissions to perform the specified operation on the Contact resource.
        protected override Task HandleRequirementAsync(
                                              AuthorizationHandlerContext context,
                                    OperationAuthorizationRequirement requirement,
                                     Contact resource)
        {
            // Check if the user is authenticated.
            // If the user is not authenticated, the authorization requirement will fail.
            if (context.User == null)
            {
                // If the user is not authenticated, the task is completed, indicating failure in authorization.
                return Task.CompletedTask;
            }

            // Administrators can do anything.
            // Check if the current user is in the role of ContactAdministratorsRole.
            // If the user is in this role, the authorization requirement will succeed, and the operation is allowed.
            if (context.User.IsInRole(Constants.ContactAdministratorsRole))
            {
                // The authorization requirement is satisfied, and the operation is allowed.
                // The context.Succeed() method is used to indicate that the requirement is met.
                context.Succeed(requirement);
            }

            // If the user is not in the ContactAdministratorsRole, the authorization requirement will fail.
            // However, we don't explicitly return a failure task since the Task.CompletedTask is returned by default.

            // The authorization system will determine the final result based on all registered authorization handlers.
            // If any handler succeeds, the operation is allowed. If all handlers fail, the operation is denied.

            // Returning Task.CompletedTask here indicates that the handler has finished processing.

            return Task.CompletedTask;
        }
    }
}
