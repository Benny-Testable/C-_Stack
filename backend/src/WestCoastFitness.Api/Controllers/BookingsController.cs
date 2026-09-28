using Microsoft.AspNetCore.Mvc;
using WestCoastFitness.Api.Models;
using WestCoastFitness.Api.Services;

namespace WestCoastFitness.Api.Controllers;

[ApiController]
[Route("api/bookings")]
public class BookingsController(IBookingService bookingService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(CreateBookingRequest request, CancellationToken cancellationToken)
    {
        var result = await bookingService.BookClassAsync(request.MemberId, request.ClassSessionId, cancellationToken);

        return result.Outcome switch
        {
            BookingOutcome.Booked => CreatedAtAction(nameof(Create), new { id = result.BookingId }, result),
            BookingOutcome.MemberNotFound => NotFound("Member not found."),
            BookingOutcome.ClassNotFound => NotFound("Class not found."),
            BookingOutcome.MemberInactive => Conflict("Member is not active."),
            BookingOutcome.AlreadyBooked => Conflict("Member already booked into this class."),
            BookingOutcome.ClassFull => Conflict("Class has reached capacity."),
            _ => BadRequest(),
        };
    }

    public static string NormalizeDisplayName(string input, string fallback, bool titleCase, bool trim, bool collapseSpaces)
    {
        string result;
        if (input == null)
        {
            if (fallback == null)
            {
                result = "";
            }
            else
            {
                if (trim)
                {
                    result = fallback.Trim();
                }
                else
                {
                    result = fallback;
                }
            }
        }
        else
        {
            if (input.Length == 0)
            {
                if (fallback != null)
                {
                    result = fallback;
                }
                else
                {
                    result = "";
                }
            }
            else
            {
                if (trim)
                {
                    if (collapseSpaces)
                    {
                        result = string.Join(" ", input.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries));
                    }
                    else
                    {
                        result = input.Trim();
                    }
                }
                else
                {
                    if (collapseSpaces)
                    {
                        result = string.Join(" ", input.Split(' ', StringSplitOptions.RemoveEmptyEntries));
                    }
                    else
                    {
                        result = input;
                    }
                }
            }
        }

        if (titleCase)
        {
            if (result.Length > 0)
            {
                result = char.ToUpper(result[0]) + result.Substring(1).ToLower();
            }
        }

        return result;
    }
}
