using System.Threading.Tasks;
using ContactManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;

namespace ContactManager.Authorization
{
    // The ContactManagerAuthorizationHandler is a custom authorization handler that checks
    // if the current user, in the role of ContactManagersRole, can approve or reject a Contact resource.
    public class ContactManagerAuthorizationHandler :
        AuthorizationHandler<OperationAuthorizationRequirement, Contact>
    {
        // This method is called by the authorization system to handle the authorization requirement.
        // It checks if the current user, in the role of ContactManagersRole, can approve or reject a Contact resource.
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                                       OperationAuthorizationRequirement requirement,
                                                       Contact resource)
        {
            // Check if the user is authenticated and if the Contact resource exists.
            // If the user is not authenticated or the Contact resource is null, the authorization requirement will fail.
            if (context.User == null || resource == null)
            {
                // If the user is not authenticated or the resource is null, the task is completed, indicating failure in authorization.
                return Task.CompletedTask;
            }

            // If not asking for approval/reject permission, return.
            // This handler is specifically for approval and rejection operations, so if the requirement is for other operations,
            // we exit early and let other handlers handle it.
            if (requirement.Name != Constants.ApproveOperationName &&
                requirement.Name != Constants.RejectOperationName)
            {
                // The task is completed since we're not handling this specific requirement.
                return Task.CompletedTask;
            }

            // Managers can approve or reject.
            // Check if the current user is in the role of ContactManagersRole.
            // If the user is in this role, the authorization requirement will succeed, and the operation is allowed.
            if (context.User.IsInRole(Constants.ContactManagersRole))
            {
                // The authorization requirement is satisfied, and the operation is allowed.
                // The context.Succeed() method is used to indicate that the requirement is met.
                context.Succeed(requirement);
            }

            // If the user is not in the ContactManagersRole, the authorization requirement will fail.
            // However, we don't explicitly return a failure task since the Task.CompletedTask is returned by default.

            // The authorization system will determine the final result based on all registered authorization handlers.
            // If any handler succeeds, the operation is allowed. If all handlers fail, the operation is denied.

            // Returning Task.CompletedTask here indicates that the handler has finished processing.

            return Task.CompletedTask;
        }
    }
}
