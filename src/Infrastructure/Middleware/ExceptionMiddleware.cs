using System.Net;
using Finbuckle.MultiTenant;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Localization;
using Serilog;
using Serilog.Context;
using Travaloud.Application.Common.Interfaces;
using Travaloud.Infrastructure.Multitenancy;

namespace Travaloud.Infrastructure.Middleware;

internal class ExceptionMiddleware : IMiddleware
{
    private readonly ICurrentUser _currentUser;
    private readonly IStringLocalizer<ExceptionMiddleware> _localizer;
    private readonly ISerializerService _jsonSerializer;
    private readonly IMultiTenantContextAccessor<TravaloudTenantInfo> _multiTenantContextAccessor;
    
    public ExceptionMiddleware(
        ICurrentUser currentUser,
        IStringLocalizer<ExceptionMiddleware> localizer,
        ISerializerService jsonSerializer, IMultiTenantContextAccessor<TravaloudTenantInfo> multiTenantContextAccessor)
    {
        _currentUser = currentUser;
        _localizer = localizer;
        _jsonSerializer = jsonSerializer;
        _multiTenantContextAccessor = multiTenantContextAccessor;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            var email = _currentUser.GetUserEmail() is string userEmail ? userEmail : "Anonymous";

            if (string.IsNullOrEmpty(email) && RequiresAuthentication(context) || exception.Message == "Object reference not set to an instance of an object.")
            {
                // Clear all cookies
                foreach (var cookie in context.Request.Cookies.Keys)
                {
                    context.Response.Cookies.Delete(cookie);
                }
                
                context.Response.Redirect("/account/login");
                return;
            }

            var userId = _currentUser.GetUserId();
            var tenant = _multiTenantContextAccessor.MultiTenantContext?.TenantInfo?.Name ?? string.Empty;
            if (userId != DefaultIdType.Empty) LogContext.PushProperty("UserId", userId);
            LogContext.PushProperty("UserEmail", email);
            if (!string.IsNullOrEmpty(tenant)) LogContext.PushProperty("Tenant", tenant);
            var errorId = DefaultIdType.NewGuid().ToString();
            LogContext.PushProperty("ErrorId", errorId);
            LogContext.PushProperty("StackTrace", exception.StackTrace);
            var errorResult = new ErrorResult
            {
                Source = exception.TargetSite?.DeclaringType?.FullName,
                Exception = exception.Message.Trim(),
                ErrorId = errorId,
                SupportMessage = _localizer["exceptionmiddleware.supportmessage"]
            };
            errorResult.Messages!.Add(exception.Message);
            var response = context.Response;
            response.ContentType = "application/json";
            if (exception is not CustomException && exception.InnerException != null)
            {
                while (exception.InnerException != null)
                {
                    exception = exception.InnerException;
                }
            }

            switch (exception)
            {
                case CustomException e:
                    response.StatusCode = errorResult.StatusCode = (int) e.StatusCode;
                    if (e.ErrorMessages is not null)
                    {
                        errorResult.Messages = e.ErrorMessages;
                    }

                    break;

                case KeyNotFoundException:
                    response.StatusCode = errorResult.StatusCode = (int) HttpStatusCode.NotFound;
                    break;

                default:
                    response.StatusCode = errorResult.StatusCode = (int) HttpStatusCode.InternalServerError;
                    break;
            }

            Log.Error(
                $"{errorResult.Exception} Request failed with Status Code {context.Response.StatusCode} and Error Id {errorId}.");
            await response.WriteAsync(_jsonSerializer.Serialize(errorResult));
        }
    }

    private bool RequiresAuthentication(HttpContext httpContext)
    {
        // Check if the request path requires authentication
        var endpoint = httpContext.GetEndpoint();
        return endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() == null;
    }
}