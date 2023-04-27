namespace BuberBreakfast.Services.Breakfasts;

using ErrorOr;
using BuberBreakfast.ServiceErrors;
using BuberBreakfast.Models;

public class BreakfastService : IBreakfastService
{
    private static readonly Dictionary<Guid, Breakfast> _breakfasts = new Dictionary<Guid, Breakfast>();
    public ErrorOr<Created> CreateBreakfast(Breakfast breakfast)
    {
        _breakfasts.Add(breakfast.ID, breakfast);

        return Result.Created;
    }
    public ErrorOr<UpsertedBreakfast> UpsertBreakfast(Breakfast breakfast)
    {
        
        var isNewlyCreated = !_breakfasts.ContainsKey(breakfast.ID);
        _breakfasts[breakfast.ID] = breakfast;

        return new UpsertedBreakfast(isNewlyCreated);
    }
    public ErrorOr<Breakfast> GetBreakfast(Guid id)
    {
        if (_breakfasts.TryGetValue(id, out Breakfast? breakfast))
        {
            return breakfast!;
        }
        return Errors.Breakfast.NotFound;
    }
    public ErrorOr<Deleted> DeleteBreakfast(Guid id)
    {
        _breakfasts.Remove(id);

        return Result.Deleted;
    }
}