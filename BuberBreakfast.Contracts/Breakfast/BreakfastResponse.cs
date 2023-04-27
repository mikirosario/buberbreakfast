namespace BuberBreakfast.Contracts.Breakfast;

using System;
using System.Collections.Generic;

public record BreakfastResponse
(
    Guid Id,
    string Name,
    string Description,
    DateTime StartDateTime,
    DateTime EndDateTime,
    DateTime LastModifiedDateTime,
    List<string> Savory,
    List<string> Sweet
);