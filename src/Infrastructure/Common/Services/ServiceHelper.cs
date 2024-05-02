using FluentValidation;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using MudBlazor;
using Serilog;
using Serilog.Context;
using Severity = MudBlazor.Severity;

namespace Travaloud.Infrastructure.Common.Services;

public static class ServiceHelper
{
    public static async Task<T?> ExecuteCallGuardedAsync<T>(
        Func<Task<T>> call,
        ISnackbar snackbar,
        ILogger logger,
        string? successMessage = null)
    {
        try
        {
            var result = await call();

            if (!string.IsNullOrWhiteSpace(successMessage))
            {
                snackbar.Add(successMessage, Severity.Success);
            }

            return result;
        }
        catch (ValidationException ex)
        {
            if (ex.Errors != null)
            {
                snackbar.Add("One or more validation errors occurred!", Severity.Error);

                foreach (var error in ex.Errors)
                {
                    var errorMessage = $"{error.PropertyName} : {error.ErrorMessage}";
                    snackbar.Add(errorMessage, Severity.Error);
                }
            }
            else
            {
                snackbar.Add("Something went wrong!", Severity.Error);
            }
        }
        catch (Exception ex)
        {
            snackbar.Add(ex.Message, Severity.Error);
            logger.Error(ex, ex.Message);
        }

        return default;
    }

    public static async Task<bool> ExecuteCallGuardedAsync(
        Func<Task> call,
        ISnackbar snackbar,
        ILogger logger,
        string? successMessage = null)
    {
        try
        {
            await call();

            if (!string.IsNullOrWhiteSpace(successMessage))
            {
                snackbar.Add(successMessage, Severity.Success);
            }

            return true;
        }
        catch (ValidationException ex)
        {
            if (ex.Errors != null)
            {
                snackbar.Add("One or more validation errors occurred!", Severity.Error);

                foreach (var error in ex.Errors)
                {
                    var errorMessage = $"{error.PropertyName} : {error.ErrorMessage}";
                    snackbar.Add(errorMessage, Severity.Error);
                }
            }
            else
            {
                snackbar.Add("Something went wrong!", Severity.Error);
            }
        }
        catch (Exception ex)
        {
            snackbar.Add(ex.Message, Severity.Error);
            logger.Error(ex, ex.Message);
            
            var errorId = DefaultIdType.NewGuid().ToString();
            LogContext.PushProperty("ErrorId", errorId);
            LogContext.PushProperty("StackTrace", ex.StackTrace);
        }

        return false;
    }
    
    public static async Task<Tuple<bool, string?, List<string>>> ExecuteCallGuardedAsync(
        Func<Task> call,
        ILogger logger,
        string? successMessage = null)
    {
        try
        {
            await call();

            return new Tuple<bool, string?, List<string>>(true, successMessage, []);
        }
        catch (ValidationException ex)
        {
            var errorMessages = new List<string>();
            if (ex.Errors == null)
                return new Tuple<bool, string?, List<string>>(false, "Something went wrong!", errorMessages);
            
            const string errorMessage = "One or more validation errors occurred!";

            errorMessages.AddRange(ex.Errors.Select(error => $"{error.PropertyName} : {error.ErrorMessage}"));

            return new Tuple<bool, string?, List<string>>(false, errorMessage, errorMessages);

        }
        catch (Exception ex)
        {
            logger.Error(ex, ex.Message);
            
            var errorId = DefaultIdType.NewGuid().ToString();
            LogContext.PushProperty("ErrorId", errorId);
            LogContext.PushProperty("StackTrace", ex.StackTrace);
            
            return new Tuple<bool, string?, List<string>>(false, ex.Message, new List<string>());
        }
    }
}