namespace BuberBreakfast.Controllers;

using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc;
using ErrorOr;

[ApiController]
[Route("[controller]")]
public class ApiController : ControllerBase
{
    protected IActionResult Problem(List<Error> errors)
    {
        if (errors.All(e => e.Type == ErrorType.Validation))
        {
            ModelStateDictionary modelStateDictionary = new ModelStateDictionary();

            foreach (Error error in errors)
            {
                modelStateDictionary.AddModelError(error.Code, error.Description);
            }
            return ValidationProblem(modelStateDictionary);
        }

        if (errors.Any(e => e.Type == ErrorType.Unexpected))
        {
            return Problem();
        }

        Error firstError = errors.First();
        var statusCode = firstError.Type switch
        {
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status500InternalServerError
        };

        return base.Problem(statusCode: statusCode, title: firstError.Description);
    }
}