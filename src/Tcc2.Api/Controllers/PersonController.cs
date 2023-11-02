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

        var getIds = new Dictionary<string, Func<PersonCompleteOutput, long>>
        {
            { "{id}", x => (long)x.Id! }
        };

        var selfLink = new LinkInfo<PersonCompleteOutput>(
            nameof(PersonController),
            "GetOnePerson",
            "self",
            getIds: getIds);

        var addressLink = new LinkInfo<PersonCompleteOutput>(
            nameof(PersonController),
            "GetOneAddress",
            "address",
            getIds: getIds);

        var peopleLink = new LinkInfo<PersonCompleteOutput>(
            nameof(PersonController),
            "GetPeople",
            "people");

        var activitiesLink = new LinkInfo<PersonCompleteOutput>(
           nameof(PersonController),
           "GetActivities",
           "activities",
           getIds: getIds);

        var linkInfos = new List<LinkInfo<PersonCompleteOutput>> { selfLink, addressLink, peopleLink, activitiesLink };

        var json = _hateoasGeneratorService.GenerateForGetOne(personOutput, linkInfos);

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

        var getIds = new Dictionary<string, Func<PersonSimpleOutput, long>>
        {
            { "{id}", x => (long)x.Id! }
        };

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
           getIds: getIds);

        var _addressLink = new LinkInfo<PersonSimpleOutput>(
            nameof(PersonController),
            "GetOneAddress",
            "address",
            getIds: getIds);

        var activitiesLink = new LinkInfo<PersonSimpleOutput>(
            nameof(PersonController),
            "GetActivities",
            "activities",
            getIds: getIds);

        var linkInfos = new List<LinkInfo<PersonSimpleOutput>> { _selfLink, _addressLink, activitiesLink };

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

        var getIds = new Dictionary<string, Func<PersonCompleteOutput, long>>
        {
            { "{id}", x => (long)x.Id! },
            { "{activityId}", x => (long)x.Id! }
        };

        var selfLink = new LinkInfo<PersonCompleteOutput>(
            nameof(PersonController),
            "GetOnePerson",
            "self",
            getIds: getIds);

        var personLink = new LinkInfo<PersonCompleteOutput>(
            nameof(PersonController),
            "GetOneAddress",
            "address",
            getIds: getIds);

        var peopleLink = new LinkInfo<PersonCompleteOutput>(
            nameof(PersonController),
            "GetPeople",
            "people");

        var activitiesLink = new LinkInfo<PersonCompleteOutput>(
            nameof(PersonController),
            "GetActivities",
            "activities",
            getIds: getIds);

        var linkInfos = new List<LinkInfo<PersonCompleteOutput>>
        {
            selfLink,
            personLink,
            peopleLink,
            activitiesLink
        };

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

        var getIds = new Dictionary<string, Func<AddressCompleteOutput, long>>
        {
            { "{id}", x => id }
        };

        var selfLink = new LinkInfo<AddressCompleteOutput>(
            nameof(PersonController),
            "GetOneAddress",
            "self",
            getIds: getIds);

        var personLink = new LinkInfo<AddressCompleteOutput>(
            nameof(PersonController),
            "GetOnePerson",
            "person",
            getIds: getIds);

        var peopleLink = new LinkInfo<AddressCompleteOutput>(
            nameof(PersonController),
            "GetPeople",
            "people");

        var geographicallyNearbyPeopleLink = new LinkInfo<AddressCompleteOutput>(
            nameof(PersonController),
            "GetGeographicallyNearbyPeople",
            "geographicallyNearbyPeople",
            getIds: getIds);

        var linkInfos = new List<LinkInfo<AddressCompleteOutput>>
        {
            selfLink,
            personLink,
            peopleLink,
            geographicallyNearbyPeopleLink
        };

        var json = _hateoasGeneratorService.GenerateForGetOne(personOutput, linkInfos);
        return Ok(json);
    }

    [HttpGet("{id}/address/nearby-people", Name = "GetGeographicallyNearbyPeople")]
    [ProducesErrorResponseType(typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetNearbyPeopleAsync(
        [FromRoute] long id,
        [FromQuery] double radius,
        CancellationToken cancellationToken,
        [FromQuery] int pageIndex = 0,
        [FromQuery] int pageSize = 20)
    {
        var paginatedPeopleOutput = await _personService.GetGeographicallyNearbyPeopleAsync(
            id,
            radius,
            pageIndex,
            pageSize,
            cancellationToken).ConfigureAwait(false);

        var getIdSelf = new Dictionary<string, Func<PersonSimpleOutput, long>>
        {
            { "{id}", x => id }
        };

        var queryString = Request.Query.ToDictionary(x => x.Key, x => x.Value);
        var selfLink = new LinkInfo<PersonSimpleOutput>(
            nameof(PersonController),
            "GetGeographicallyNearbyPeople",
            "self",
            queryString,
            getIdSelf,
            false);

        var getIdForOnePerson = new Dictionary<string, Func<PersonSimpleOutput, long>>
        {
            { "{id}", x => (long)x.Id! }
        };

        var selfPersonLink = new LinkInfo<PersonSimpleOutput>(
           nameof(PersonController),
           "GetOnePerson",
           "self",
           getIds: getIdForOnePerson);

        var peopleLink = new LinkInfo<PersonSimpleOutput>(
            nameof(PersonController),
            "GetPeople",
            "people");

        var linkInfos = new List<LinkInfo<PersonSimpleOutput>> { selfPersonLink, peopleLink };

        var json = _hateoasGeneratorService.GenerateForGetMany(paginatedPeopleOutput, selfLink, "people", linkInfos);
        return Ok(json);
    }

    [HttpPost("{id}/activities")]
    [ProducesErrorResponseType(typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AddActivityAsync(
        [FromRoute] long id,
        [FromBody] ActivityInput? activityInput,
        CancellationToken cancellationToken)
    {
        var activityOutput = await _personService
            .AddActivityAsync(id, activityInput, cancellationToken)
            .ConfigureAwait(false);

        var getIds = new Dictionary<string, Func<ActivityCompleteOutput, long>>
        {
            { "{id}", x => id },
            { "{activityId}", x => (long)x.Id! }
        };

        var selfLink = new LinkInfo<ActivityCompleteOutput>(
            nameof(PersonController),
            "GetOneActivity",
            "self",
            getIds: getIds);

        var activitiesLink = new LinkInfo<ActivityCompleteOutput>(
            nameof(PersonController),
            "GetActivities",
            "activities",
            getIds: getIds);

        var peopleLink = new LinkInfo<ActivityCompleteOutput>(
            nameof(PersonController),
            "GetPeople",
            "people");

        var selfPersonLink = new LinkInfo<ActivityCompleteOutput>(
            nameof(PersonController),
            "GetOnePerson",
            "person",
            getIds: getIds);

        var linkInfos = new List<LinkInfo<ActivityCompleteOutput>>
        {
            selfLink,
            activitiesLink,
            peopleLink,
            selfPersonLink
        };

        var json = _hateoasGeneratorService.GenerateForGetOne(activityOutput, linkInfos);

        var routeValues = new { id, activityId = activityOutput.Id };
        return CreatedAtRoute("GetOneActivity", routeValues, json);
    }

    [HttpGet("{id}/activities", Name = "GetActivities")]
    [ProducesErrorResponseType(typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetActivitiesAsync(
        [FromRoute] long id,
        CancellationToken cancellationToken)
    {
        var activitiesOutput = await _personService
            .GetActivitiesAsync(id, cancellationToken)
            .ConfigureAwait(false);

        var getIds = new Dictionary<string, Func<ActivitySimpleOutput, long>>
        {
            { "{id}", x => id },
            { "{activityId}", x => (long)x.Id! }
        };

        var selfLink = new LinkInfo<ActivitySimpleOutput>(
            nameof(PersonController),
            "GetActivities",
            "self",
            getIds: getIds);

        var selfActivityLink = new LinkInfo<ActivitySimpleOutput>(
            nameof(PersonController),
            "GetOneActivity",
            "self",
            getIds: getIds);

        var peopleLink = new LinkInfo<ActivitySimpleOutput>(
           nameof(PersonController),
           "GetPeople",
           "people",
           getIds: getIds);

        var linkInfos = new List<LinkInfo<ActivitySimpleOutput>> { selfActivityLink, peopleLink };

        var json = _hateoasGeneratorService.GenerateForGetMany(activitiesOutput, selfLink, "activities", linkInfos);
        return Ok(json);
    }

    [HttpGet("{id}/activities/{activityId}", Name = "GetOneActivity")]
    [ProducesErrorResponseType(typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetActivityAsync(
        [FromRoute] long id,
        [FromRoute] long activityId,
        CancellationToken cancellationToken)
    {
        var activitiesOutput = await _personService
            .GetActivityAsync(id, activityId, cancellationToken)
            .ConfigureAwait(false);

        var getIds = new Dictionary<string, Func<ActivitySimpleOutput, long>>
        {
            { "{id}", x => id },
            { "{activityId}", x => (long)x.Id! }
        };

        var selfActivityLink = new LinkInfo<ActivitySimpleOutput>(
            nameof(PersonController),
            "GetOneActivity",
            "self",
            getIds: getIds);

        var activitiesLink = new LinkInfo<ActivitySimpleOutput>(
            nameof(PersonController),
            "GetActivities",
            "activities",
            getIds: getIds);

        var peopleLink = new LinkInfo<ActivitySimpleOutput>(
           nameof(PersonController),
           "GetPeople",
           "people",
           getIds: getIds);

        var selfPersonLink = new LinkInfo<ActivitySimpleOutput>(
            nameof(PersonController),
            "GetOnePerson",
            "person",
            getIds: getIds);

        var linkInfos = new List<LinkInfo<ActivitySimpleOutput>>
        {
            selfActivityLink,
            activitiesLink,
            peopleLink,
            selfPersonLink
        };

        var json = _hateoasGeneratorService.GenerateForGetOne(activitiesOutput, linkInfos);
        return Ok(json);
    }

    [HttpGet("{id}/activities/{activityId}/nearby-people", Name = "GetFunctionallyNearbyPeople")]
    [ProducesErrorResponseType(typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetFunctionallyNearbyPeopleAsync(
        [FromRoute] long id,
        [FromRoute] long activityId,
        CancellationToken cancellationToken,
        [FromQuery] int pageIndex = 0,
        [FromQuery] int pageSize = 20)
    {
        var paginatedPeopleOutput = await _personService
            .GetFunctionallyNearbyPeopleAsync(id, activityId, pageIndex, pageSize, cancellationToken)
            .ConfigureAwait(false);

        var geIdSelf = new Dictionary<string, Func<PersonSimpleOutput, long>>
        {
            { "{id}", x => id }
        };

        var queryString = Request.Query.ToDictionary(x => x.Key, x => x.Value);
        var selfLink = new LinkInfo<PersonSimpleOutput>(
            nameof(PersonController),
            "GetGeographicallyNearbyPeople",
            "self",
            queryString,
            geIdSelf,
            false);

        var getIdForOnePerson = new Dictionary<string, Func<PersonSimpleOutput, long>>
        {
            { "{id}", x => (long)x.Id! }
        };

        var selfPersonLink = new LinkInfo<PersonSimpleOutput>(
           nameof(PersonController),
           "GetOnePerson",
           "self",
           getIds: getIdForOnePerson);

        var peopleLink = new LinkInfo<PersonSimpleOutput>(
            nameof(PersonController),
            "GetPeople",
            "people");

        var linkInfos = new List<LinkInfo<PersonSimpleOutput>> { selfPersonLink, peopleLink };

        var json = _hateoasGeneratorService.GenerateForGetMany(paginatedPeopleOutput, selfLink, "people", linkInfos);
        return Ok(json);
    }
}
