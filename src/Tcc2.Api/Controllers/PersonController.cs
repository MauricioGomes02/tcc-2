using Microsoft.AspNetCore.Mvc;
using Tcc2.Api.Interfaces.Services;
using Tcc2.Api.Services.Models;
using Tcc2.Application.Interfaces.Services;
using Tcc2.Application.Models.Inputs;
using Tcc2.Application.Models.Outputs;

namespace Tcc2.Api.Controllers;
[ApiController]
[Route("people")]
public class PersonController : ControllerBase
{
    private readonly IPersonService _personService;
    private readonly IHateoasGeneratorService _hateoasGeneratorService;

    public PersonController(
        IPersonService personService,
        IHateoasGeneratorService hateoasGeneratorService)
    {
        _personService = personService;
        _hateoasGeneratorService = hateoasGeneratorService;
    }

    [HttpPost]
    [ProducesErrorResponseType(typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AddAsync(PersonInput personInput, CancellationToken cancellationToken)
    {
        var personOutput = await _personService.AddAsync(personInput, cancellationToken).ConfigureAwait(false);

        var selfLink = new LinkInfo<PersonCompleteOutput>(
            nameof(PersonController),
            "GetOnePerson",
            "self",
            getId: x => (long)personOutput.Id!);

        var addressLink = new LinkInfo<PersonCompleteOutput>(
            nameof(PersonController),
            "GetOneAddress",
            "address",
            getId: x => (long)personOutput.Id!);

        var peopleLink = new LinkInfo<PersonCompleteOutput>(
            nameof(PersonController),
            "GetPeople",
            "people");

        var linkInfos = new List<LinkInfo<PersonCompleteOutput>> { selfLink, addressLink, peopleLink };

        var json = _hateoasGeneratorService.GenerateForGetOne(
            personOutput,
            linkInfos);

        var routeValues = new { id = personOutput.Id };
        return CreatedAtRoute("GetOnePerson", routeValues, json);
    }

    [HttpGet(Name = "GetPeople")]
    [ProducesErrorResponseType(typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetPeopleAsync(
        CancellationToken cancellationToken,
        [FromQuery] int pageIndex = 0,
        [FromQuery] int pageSize = 20)
    {
        var paginatedPeopleOutput = await _personService
            .GetAsync(pageIndex, pageSize, cancellationToken)
            .ConfigureAwait(false);

        var queryString = Request.Query.ToDictionary(x => x.Key, x => x.Value);
        var selfLink = new LinkInfo<PersonSimpleOutput>(
            nameof(PersonController),
            "GetPeople",
            "self",
            queryString);

        var _selfLink = new LinkInfo<PersonSimpleOutput>(
           nameof(PersonController),
           "GetOnePerson",
           "self",
           getId: x => (long)x.Id!);

        var _addressLink = new LinkInfo<PersonSimpleOutput>(
            nameof(PersonController),
            "GetOneAddress",
            "address",
            getId: x => (long)x.Id!);

        var linkInfos = new List<LinkInfo<PersonSimpleOutput>> { _selfLink, _addressLink };

        var json = _hateoasGeneratorService.GenerateForGetMany(paginatedPeopleOutput, selfLink, "people", linkInfos);
        return Ok(json);
    }

    [HttpGet("{id}", Name = "GetOnePerson")]
    [ProducesErrorResponseType(typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetOnePersonAsync(long id, CancellationToken cancellationToken)
    {
        var personOutput = await _personService.GetAsync(id, cancellationToken).ConfigureAwait(false);

        var selfLink = new LinkInfo<PersonCompleteOutput>(
            nameof(PersonController),
            "GetOnePerson",
            "self",
            getId: x => id);

        var personLink = new LinkInfo<PersonCompleteOutput>(
            nameof(PersonController),
            "GetOneAddress",
            "address",
            getId: x => id);

        var peopleLink = new LinkInfo<PersonCompleteOutput>(
            nameof(PersonController),
            "GetPeople",
            "people");

        var linkInfos = new List<LinkInfo<PersonCompleteOutput>> { selfLink, personLink, peopleLink };

        var json = _hateoasGeneratorService.GenerateForGetOne(personOutput, linkInfos);
        return Ok(json);
    }

    [HttpGet("{id}/address", Name = "GetOneAddress")]
    [ProducesErrorResponseType(typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetOneAddressAsync(long id, CancellationToken cancellationToken)
    {
        var personOutput = await _personService.GetAddressAsync(id, cancellationToken).ConfigureAwait(false);

        var selfLink = new LinkInfo<AddressCompleteOutput>(
            nameof(PersonController),
            "GetOneAddress",
            "self",
            getId: x => id);

        var personLink = new LinkInfo<AddressCompleteOutput>(
            nameof(PersonController),
            "GetOnePerson",
            "person",
            getId: x => id);

        var peopleLink = new LinkInfo<AddressCompleteOutput>(
            nameof(PersonController),
            "GetPeople",
            "people");

        var linkInfos = new List<LinkInfo<AddressCompleteOutput>> { selfLink, personLink, peopleLink };

        var json = _hateoasGeneratorService.GenerateForGetOne(personOutput, linkInfos);
        return Ok(json);
    }

    [HttpGet("{id}/nearby-people")]
    public async Task<IActionResult> GetNearbyPeopleAsync(
        [FromRoute] long id,
        [FromQuery] double radiusInKilometers,
        CancellationToken cancellationToken,
        [FromQuery] int pageIndex = 0,
        [FromQuery] int pageSize = 20)
    {
        var paginatedPeopleOutput = await _personService.GetNearbyPeopleAsync(
            id,
            radiusInKilometers,
            pageIndex,
            pageSize,
            cancellationToken).ConfigureAwait(false);

        var queryString = Request.Query.ToDictionary(x => x.Key, x => x.Value);
        var selfLink = new LinkInfo<PersonSimpleOutput>(
            nameof(PersonController),
            "GetPeople",
            "self",
            queryString);

        var _selfLink = new LinkInfo<PersonSimpleOutput>(
           nameof(PersonController),
           "GetOnePerson",
           "self",
           getId: x => (long)x.Id!);

        var _addressLink = new LinkInfo<PersonSimpleOutput>(
            nameof(PersonController),
            "GetOneAddress",
            "address",
            getId: x => (long)x.Id!);

        var linkInfos = new List<LinkInfo<PersonSimpleOutput>> { _selfLink, _addressLink };

        var json = _hateoasGeneratorService.GenerateForGetMany(paginatedPeopleOutput, selfLink, "people", linkInfos);
        return Ok(json);
    }
}
