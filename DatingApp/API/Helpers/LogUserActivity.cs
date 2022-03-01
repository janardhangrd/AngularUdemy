using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace API.Helpers
{
    public class LogUserActivity : IAsyncActionFilter
    {
        async Task IAsyncActionFilter.OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var resultContext = await next();

            if(!resultContext.HttpContext.User.Identity.IsAuthenticated) return;

            var userId = resultContext.HttpContext.User.GetUserId();
            //get the context of Service Which you want to call
            var repo = resultContext.HttpContext.RequestServices.GetService<IUserRepository>();
            //call the service to get details
            var user = await repo.GetUserByIdAsync(userId);
            //update the details
            user.LastActive = DateTime.Now;
            await repo.SaveAllAsync();//Save the details
        }
    }
}