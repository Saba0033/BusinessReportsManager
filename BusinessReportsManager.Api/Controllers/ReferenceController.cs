using BusinessReportsManager.Api.Reference;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusinessReportsManager.API.Controllers;

/// <summary>
/// Reference/lookup data for form pickers (countries and cities for the Destination field).
/// </summary>
[Authorize]
[ApiController]
[Route("api/reference")]
public class ReferenceController : ControllerBase
{
    /// <summary>
    /// Returns the list of countries, optionally filtered by a partial name (case-insensitive).
    /// </summary>
    /// <param name="query">Optional partial country name.</param>
    [HttpGet("countries")]
    [ProducesResponseType(typeof(List<string>), 200)]
    public IActionResult GetCountries([FromQuery] string? query)
    {
        var countries = GeoData.Countries.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(query))
        {
            var term = query.Trim();
            countries = countries.Where(c => c.Contains(term, StringComparison.OrdinalIgnoreCase));
        }

        return Ok(countries.ToList());
    }

    /// <summary>
    /// Returns major cities for a country, optionally filtered by a partial name.
    /// Unknown countries return an empty list (frontend may allow free-text entry).
    /// </summary>
    /// <param name="country">Country name (as returned by /countries).</param>
    /// <param name="query">Optional partial city name.</param>
    [HttpGet("cities")]
    [ProducesResponseType(typeof(List<string>), 200)]
    public IActionResult GetCities([FromQuery] string country, [FromQuery] string? query)
    {
        if (string.IsNullOrWhiteSpace(country) ||
            !GeoData.CitiesByCountry.TryGetValue(country.Trim(), out var cities))
        {
            return Ok(new List<string>());
        }

        var result = cities.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(query))
        {
            var term = query.Trim();
            result = result.Where(c => c.Contains(term, StringComparison.OrdinalIgnoreCase));
        }

        return Ok(result.ToList());
    }
}
