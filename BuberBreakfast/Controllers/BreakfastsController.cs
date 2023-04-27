namespace BuberBreakfast.Controllers;

using Microsoft.AspNetCore.Mvc;
using BuberBreakfast.Contracts.Breakfast;
using BuberBreakfast.Models;
using BuberBreakfast.Services.Breakfasts;
using BuberBreakfast.ServiceErrors;
using ErrorOr;

public class BreakfastsController : ApiController
{
    private readonly IBreakfastService _breakfastService;

    public BreakfastsController(IBreakfastService breakfastService)
    {
        _breakfastService = breakfastService;
    }

    [HttpPost("")]
    public IActionResult CreateBreakfast(CreateBreakfastRequest request)
    {
        ErrorOr<Breakfast> breakfastResult = Breakfast.From(request);
        if (breakfastResult.IsError)
        {
            return Problem(breakfastResult.Errors);
        }
        Breakfast breakfast = breakfastResult.Value;
        //TODO: Save breakfast to database
        ErrorOr<Created> createResult = _breakfastService.CreateBreakfast(breakfast);

        return createResult.Match(created => CreatedAtGetBreakfast(breakfast), errors => Problem(createResult.Errors));
    }

    [HttpGet("{id:guid}")]
    public IActionResult GetBreakfast(Guid id)
    {
        ErrorOr<Breakfast> result = _breakfastService.GetBreakfast(id);
        return result.Match(breakfast => Ok(MapBreakfastResponse(breakfast)), errors => Problem(errors));
        // if (result.IsError)
        // {
        //     if (result.FirstError == Errors.Breakfast.NotFound)
        //         return NotFound();
        //     return BadRequest();
        // }

        // Breakfast breakfast = result.Value;

        // return Ok(response);
    }

    [HttpPut("{id:guid}")]
    public IActionResult UpsertBreakfast(Guid id, UpsertBreakfastRequest request)
    {
        ErrorOr<Breakfast> breakfastResult = Breakfast.From(id, request);
        if (breakfastResult.IsError)
        {
            return Problem(breakfastResult.Errors);
        }

        Breakfast breakfast = breakfastResult.Value;
        ErrorOr<UpsertedBreakfast> upsertResult = _breakfastService.UpsertBreakfast(breakfast);

        return upsertResult.Match(
            upserted => upserted.IsNewlyCreated ? CreatedAtGetBreakfast(breakfast) : NoContent(),
            errors => Problem(errors));
    }

    [HttpDelete("{id:guid}")]
    public IActionResult DeleteBreakfast(Guid id)
    {
        ErrorOr<Deleted> result = _breakfastService.DeleteBreakfast(id);
        return result.Match(deleted => NoContent(), errors => Problem(errors));
    }

    private static BreakfastResponse MapBreakfastResponse(Breakfast breakfast)
    {
        BreakfastResponse response = new BreakfastResponse(
        breakfast.ID,
        breakfast.Name,
        breakfast.Description,
        breakfast.StartDateTime,
        breakfast.EndDateTime,
        breakfast.LastModifiedDateTime,
        breakfast.Savory,
        breakfast.Sweet
        );

        return response;
    }

        private CreatedAtActionResult CreatedAtGetBreakfast(Breakfast breakfast)
    {
        return CreatedAtAction(
            nameof(GetBreakfast),
            new { id = breakfast.ID },
            MapBreakfastResponse(breakfast));
    }
}