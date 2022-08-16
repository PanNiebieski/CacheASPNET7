using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Extensions.Primitives;
using System.IO;
using System.Text.Json.Nodes;
using System.Text.Json;

namespace CacheASPNET7
{
    public class ByIdCachePolicy : IOutputCachePolicy
    {
        public ValueTask CacheRequestAsync(OutputCacheContext context, CancellationToken cancellation)
        {


            var idRouteVal = context.HttpContext.Request.RouteValues["id"];

            if (idRouteVal is null)
            {
                return ValueTask.CompletedTask;
            }

            context.Tags.Add(idRouteVal.ToString()!);
            var attemptOutputCaching = AttemptOutputCaching(context);
            context.EnableOutputCaching = true;
            context.AllowCacheLookup = attemptOutputCaching;
            context.AllowCacheStorage = attemptOutputCaching;
            context.AllowLocking = true;
            context.CacheVaryByRules.QueryKeys = "*";

            return ValueTask.CompletedTask; 
        }

        public ValueTask ServeFromCacheAsync(OutputCacheContext context, CancellationToken cancellation)
        {
            return ValueTask.CompletedTask;
        }

        public ValueTask ServeResponseAsync(OutputCacheContext context, CancellationToken cancellation)
        {
            return ValueTask.CompletedTask;
        }

        private static bool AttemptOutputCaching(OutputCacheContext context)
        {
            // Check if the current request fulfills the requirements to be cached

            var request = context.HttpContext.Request;

            // Verify the method
            if (!HttpMethods.IsGet(request.Method) && !HttpMethods.IsHead(request.Method))
            {
                //context.Logger.RequestMethodNotCacheable(request.Method);
                return false;
            }

            // Verify existence of authorization headers
            if (!StringValues.IsNullOrEmpty(request.Headers.Authorization) || request.HttpContext.User?.Identity?.IsAuthenticated == true)
            {
                //context.Logger.RequestWithAuthorizationNotCacheable();
                return false;
            }

            return true;
        }
    }
}
