using ContactManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace ContactManager.Authorization
{
    // The ContactIsOwnerAuthorizationHandler is a custom authorization handler that checks
    // if the current user is the owner of a Contact resource to perform CRUD (Create, Read, Update, Delete) operations.
    public class ContactIsOwnerAuthorizationHandler
                : AuthorizationHandler<OperationAuthorizationRequirement, Contact>
    {
        private readonly UserManager<IdentityUser> _userManager;

        // Constructor to inject the UserManager<IdentityUser> dependency.
        public ContactIsOwnerAuthorizationHandler(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        // This method is called by the authorization system to handle the authorization requirement.
        // It checks if the current user is the owner of the Contact resource to perform CRUD operations.
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

            // If not asking for CRUD permission (Create, Read, Update, Delete), return.
            // This handler is specifically for CRUD operations, so if the requirement is for other operations,
            // we exit early and let other handlers handle it.
            if (requirement.Name != Constants.CreateOperationName &&
                requirement.Name != Constants.ReadOperationName &&
                requirement.Name != Constants.UpdateOperationName &&
                requirement.Name != Constants.DeleteOperationName)
            {
                // The task is completed since we're not handling this specific requirement.
                return Task.CompletedTask;
            }

            // Check if the current user is the owner of the Contact resource.
            // We compare the OwnerID of the Contact with the ID of the current user.
            // If they match, it means the user is the owner of the Contact, and the authorization requirement is met.
            if (resource.OwnerID == _userManager.GetUserId(context.User))
            {
                // The authorization requirement is satisfied, and the operation is allowed.
                // The context.Succeed() method is used to indicate that the requirement is met.
                context.Succeed(requirement);
            }

            // If the user is not the owner of the Contact resource, the authorization requirement will fail.
            // However, we don't explicitly return a failure task since the Task.CompletedTask is returned by default.

            // The authorization system will determine the final result based on all registered authorization handlers.
            // If any handler succeeds, the operation is allowed. If all handlers fail, the operation is denied.

            // Returning Task.CompletedTask here indicates that the handler has finished processing.

            return Task.CompletedTask;
        }
    }
}
