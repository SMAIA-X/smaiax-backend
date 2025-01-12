using System.Net;
using System.Net.Http.Headers;
using System.Text;

using Microsoft.EntityFrameworkCore;

using Newtonsoft.Json;

using SMAIAXBackend.Application.DTOs;
using SMAIAXBackend.Domain.Model.Enums;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;

namespace SMAIAXBackend.IntegrationTests.EndToEndTests;

[TestFixture]
public class SmartMeterTests : TestBase
{
    private const string BaseUrl = "/api/smartMeters";

    [Test]
    public async Task GivenSmartMeterAssignDtoAndAccessToken_WhenAssignSmartMeter_ThenSmartMeterIsAssigned()
    {
        // Given
        var smartMeterAssignDto = new SmartMeterAssignDto(Guid.Parse("31c4fd82-5018-4bcd-bc0e-74d6b0a4e86d"), "Test Smart Meter", null);
        var httpContent = new StringContent(JsonConvert.SerializeObject(smartMeterAssignDto), Encoding.UTF8,
            "application/json");
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);

        // When
        var response = await _httpClient.PostAsync(BaseUrl, httpContent);

        // Then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.That(responseContent, Is.Not.Null);

        var id = Guid.Parse(responseContent.Trim('"'));
        var smartMeterActual = await _tenant1DbContext.SmartMeters
            .AsNoTracking()
            .FirstOrDefaultAsync(x =>
                x.Id.Equals(new SmartMeterId(id)));

        Assert.That(smartMeterActual, Is.Not.Null);
        Assert.That(smartMeterActual.Name, Is.EqualTo(smartMeterAssignDto.Name));
    }

    [Test]
    public async Task GivenSmartMeterAssignDtoAndNoAccessToken_WhenAssignSmartMeter_ThenUnauthorizedIsReturned()
    {
        // Given
        var smartMeterAssignDto = new SmartMeterAssignDto(Guid.Parse("31c4fd82-5018-4bcd-bc0e-74d6b0a4e86d"), "Test Smart Meter", null);
        var httpContent = new StringContent(JsonConvert.SerializeObject(smartMeterAssignDto), Encoding.UTF8,
            "application/json");
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "");

        // When
        var response = await _httpClient.PostAsync(BaseUrl, httpContent);

        // Then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task GivenAccessToken_WhenGetSmartMeters_ThenExpectedSmartMetersAreReturned()
    {
        // Given
        var smartMetersExpected = new List<SmartMeterOverviewDto>()
        {
            new(Guid.Parse("5e9db066-1b47-46cc-bbde-0b54c30167cd"), "Smart Meter 1", 0, 0),
            new(Guid.Parse("f4c70232-6715-4c15-966f-bf4bcef46d39"), "Smart Meter 2", 1, 0)
        };
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);

        // When
        var response = await _httpClient.GetAsync(BaseUrl);

        // Then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.That(responseContent, Is.Not.Null);

        var smartMetersActual = JsonConvert.DeserializeObject<List<SmartMeterOverviewDto>>(responseContent);
        Assert.That(smartMetersActual, Is.Not.Null);
        Assert.That(smartMetersActual, Has.Count.EqualTo(smartMetersExpected.Count + 1)); // +1 because of the seed data
    }

    [Test]
    public async Task GivenNoAccessToken_WhenGetSmartMeters_ThenUnauthorizedIsReturned()
    {
        // Given
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "");

        // When
        var response = await _httpClient.GetAsync(BaseUrl);

        // Then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task GivenSmartMeterIdAndAccessToken_WhenGetSmartMeterById_ThenExpectedSmartMetersAreReturned()
    {
        // Given
        var locationDto = new LocationDto("Some Streetname", "Some city", "Some state", "Some county", Continent.Asia);
        var metadataDto = new MetadataDto(Guid.Parse("1c8c8313-6fc4-4ebd-9ca8-8a1267441e06"), DateTime.UtcNow,
            locationDto, 4);
        var smartMeterExpected =
            new SmartMeterDto(Guid.Parse("f4c70232-6715-4c15-966f-bf4bcef46d39"), "Smart Meter 2", [metadataDto]);
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);

        // When
        var response = await _httpClient.GetAsync($"{BaseUrl}/{smartMeterExpected.Id}");

        // Then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.That(responseContent, Is.Not.Null);

        var smartMeterActual = JsonConvert.DeserializeObject<SmartMeterDto>(responseContent);
        Assert.That(smartMeterActual, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(smartMeterActual.Id, Is.EqualTo(smartMeterExpected.Id));
            Assert.That(smartMeterActual.Name, Is.EqualTo(smartMeterExpected.Name));
            Assert.That(smartMeterActual.Metadata, Has.Count.EqualTo(smartMeterExpected.Metadata.Count));
        });
    }

    [Test]
    public async Task GivenSmartMeterIdAndNoAccessToken_WhenGetSmartMeterById_ThenUnauthorizedIsReturned()
    {
        // Given
        var smartMeterId = Guid.Parse("5e9db066-1b47-46cc-bbde-0b54c30167cd");
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "");

        // When
        var response = await _httpClient.GetAsync($"{BaseUrl}/{smartMeterId}");

        // Then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task GivenSmartMeterAndAccessToken_WhenUpdateSmartMeter_ThenSmartMeterIsUpdated()
    {
        // Given
        var smartMeterId = Guid.Parse("5e9db066-1b47-46cc-bbde-0b54c30167cd");
        var smartMeterUpdateDto = new SmartMeterUpdateDto(smartMeterId, "Updated name");
        var httpContent = new StringContent(JsonConvert.SerializeObject(smartMeterUpdateDto), Encoding.UTF8,
            "application/json");
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);

        // When
        var response = await _httpClient.PutAsync($"{BaseUrl}/{smartMeterId}", httpContent);

        // Then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var responseContent = await response.Content.ReadAsStringAsync();

        Assert.That(responseContent, Is.Not.Null);
        var returnedId = Guid.Parse(responseContent.Trim('"'));
        Assert.That(returnedId, Is.EqualTo(smartMeterId));

        var smartMeterActual = await _tenant1DbContext.SmartMeters
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id.Equals(new SmartMeterId(returnedId)));
        Assert.That(smartMeterActual, Is.Not.Null);
        Assert.That(smartMeterActual.Name, Is.EqualTo(smartMeterUpdateDto.Name));
    }

    [Test]
    public async Task GivenSmartMeterAndNoAccessToken_WhenUpdateSmartMeter_ThenUnauthorizedIsReturned()
    {
        // Given
        var smartMeterId = Guid.Parse("5e9db066-1b47-46cc-bbde-0b54c30167cd");
        var smartMeterUpdateDto = new SmartMeterUpdateDto(smartMeterId, "Updated name");
        var httpContent = new StringContent(JsonConvert.SerializeObject(smartMeterUpdateDto), Encoding.UTF8,
            "application/json");
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "");

        // When
        var response = await _httpClient.PutAsync($"{BaseUrl}/{smartMeterId}", httpContent);

        // Then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task
        GivenSmartMeterIdAndMetadataCreateDtoAndAccessToken_WhenAddMetadata_ThenMetadataIsAddedToSmartMeter()
    {
        // Given
        const int metadataCountExpected = 1;
        var smartMeterId = Guid.Parse("5e9db066-1b47-46cc-bbde-0b54c30167cd");
        var locationDto = new LocationDto("Some street name", "Some city name", "Some state", "some country",
            Continent.Europe);
        var metadataCreateDto = new MetadataCreateDto(DateTime.UtcNow, locationDto, 4);
        var httpContent = new StringContent(JsonConvert.SerializeObject(metadataCreateDto), Encoding.UTF8,
            "application/json");
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);

        // When
        var response = await _httpClient.PostAsync($"{BaseUrl}/{smartMeterId}/metadata", httpContent);

        // Then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.That(responseContent, Is.Not.Null);

        var returnedId = Guid.Parse(responseContent.Trim('"'));
        Assert.That(returnedId, Is.EqualTo(smartMeterId));

        var smartMeter = await _tenant1DbContext.SmartMeters
            .AsNoTracking()
            .Include(smartMeter => smartMeter.Metadata)
            .FirstOrDefaultAsync(x => x.Id.Equals(new SmartMeterId(returnedId)));

        Assert.That(smartMeter, Is.Not.Null);
        Assert.That(smartMeter.Metadata, Has.Count.EqualTo(metadataCountExpected));
        var metadataActual = smartMeter.Metadata[0];
        Assert.Multiple(() =>
        {
            Assert.That(metadataActual.ValidFrom,
                Is.EqualTo(metadataCreateDto.ValidFrom).Within(TimeSpan.FromMilliseconds(1)));
            Assert.That(metadataActual.Location.StreetName, Is.EqualTo(metadataCreateDto.Location.StreetName));
            Assert.That(metadataActual.Location.City, Is.EqualTo(metadataCreateDto.Location.City));
            Assert.That(metadataActual.Location.State, Is.EqualTo(metadataCreateDto.Location.State));
            Assert.That(metadataActual.Location.Country, Is.EqualTo(metadataCreateDto.Location.Country));
            Assert.That(metadataActual.Location.Continent, Is.EqualTo(metadataCreateDto.Location.Continent));
            Assert.That(metadataActual.HouseholdSize, Is.EqualTo(metadataCreateDto.HouseholdSize));
        });
    }

    [Test]
    public async Task GivenSmartMeterIdAndMetadataCreateDtoAndNoAccessToken_WhenAddMetadata_ThenUnauthorizedIsReturned()
    {
        // Given
        var smartMeterId = Guid.Parse("5e9db066-1b47-46cc-bbde-0b54c30167cd");
        var locationDto = new LocationDto("Some street name", "Some city name", "Some state", "some country",
            Continent.Europe);
        var metadataCreateDto = new MetadataCreateDto(DateTime.UtcNow, locationDto, 4);
        var httpContent = new StringContent(JsonConvert.SerializeObject(metadataCreateDto), Encoding.UTF8,
            "application/json");
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "");

        // When
        var response = await _httpClient.PostAsync($"{BaseUrl}/{smartMeterId}/metadata", httpContent);

        // Then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task
        GivenSmartMeterIdAndMetadataIdAndAccessToken_WhenRemoveMetadata_ThenMetadataIsRemovedFromSmartMeter()
    {
        // Given
        const int metadataCountExpected = 0;
        var smartMeterId = Guid.Parse("f4c70232-6715-4c15-966f-bf4bcef46d39");
        var metadataId = Guid.Parse("1c8c8313-6fc4-4ebd-9ca8-8a1267441e06");
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);

        // When
        var response = await _httpClient.DeleteAsync($"{BaseUrl}/{smartMeterId}/metadata/{metadataId}");

        // Then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));

        var smartMeter = await _tenant1DbContext.SmartMeters
            .AsNoTracking()
            .Include(smartMeter => smartMeter.Metadata)
            .FirstOrDefaultAsync(x => x.Id.Equals(new SmartMeterId(smartMeterId)));

        Assert.That(smartMeter, Is.Not.Null);
        Assert.That(smartMeter.Metadata, Has.Count.EqualTo(metadataCountExpected));
    }

    [Test]
    public async Task GivenSmartMeterIdAndMetadataIdAndNoAccessToken_WhenRemoveMetadata_ThenUnauthorizedIsReturned()
    {
        // Given
        var smartMeterId = Guid.Parse("f4c70232-6715-4c15-966f-bf4bcef46d39");
        var metadataId = Guid.Parse("1c8c8313-6fc4-4ebd-9ca8-8a1267441e06");
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "");

        // When
        var response = await _httpClient.DeleteAsync($"{BaseUrl}/{smartMeterId}/metadata/{metadataId}");

        // Then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task GivenSmartMeterIdAndMetadataUpdateDtoAndAccessToken_WhenUpdateMetadata_ThenMetadataIsUpdated()
    {
        // Given
        var smartMeterId = Guid.Parse("f4c70232-6715-4c15-966f-bf4bcef46d39");
        var metadataId = Guid.Parse("1c8c8313-6fc4-4ebd-9ca8-8a1267441e06");
        var locationDto = new LocationDto("Updated street", "Updated city", "Updated state", "Updated country",
            Continent.Asia);
        var metadataUpdateDto = new MetadataUpdateDto(metadataId, DateTime.UtcNow, locationDto, 5);
        var httpContent = new StringContent(JsonConvert.SerializeObject(metadataUpdateDto), Encoding.UTF8,
            "application/json");
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);

        // When
        var response = await _httpClient.PutAsync($"{BaseUrl}/{smartMeterId}/metadata/{metadataId}", httpContent);

        // Then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.That(responseContent, Is.Not.Null);

        var returnedId = Guid.Parse(responseContent.Trim('"'));
        Assert.That(returnedId, Is.EqualTo(metadataId));

        var smartMeter = await _tenant1DbContext.SmartMeters
            .AsNoTracking()
            .Include(smartMeter => smartMeter.Metadata)
            .FirstOrDefaultAsync(x => x.Id.Equals(new SmartMeterId(smartMeterId)));

        Assert.That(smartMeter, Is.Not.Null);
        var metadataActual = smartMeter.Metadata.FirstOrDefault(m => m.Id.Equals(new MetadataId(metadataId)));
        Assert.Multiple(() =>
        {
            Assert.That(metadataActual, Is.Not.Null);
            Assert.That(metadataActual!.Location.StreetName, Is.EqualTo(metadataUpdateDto.Location.StreetName));
            Assert.That(metadataActual.Location.City, Is.EqualTo(metadataUpdateDto.Location.City));
            Assert.That(metadataActual.Location.State, Is.EqualTo(metadataUpdateDto.Location.State));
            Assert.That(metadataActual.Location.Country, Is.EqualTo(metadataUpdateDto.Location.Country));
            Assert.That(metadataActual.Location.Continent, Is.EqualTo(metadataUpdateDto.Location.Continent));
            Assert.That(metadataActual.HouseholdSize, Is.EqualTo(metadataUpdateDto.HouseholdSize));
        });
    }

    [Test]
    public async Task
        GivenSmartMeterIdAndMetadataUpdateDtoAndNoAccessToken_WhenUpdateMetadata_ThenUnauthorizedIsReturned()
    {
        // Given
        var smartMeterId = Guid.Parse("5e9db066-1b47-46cc-bbde-0b54c30167cd");
        var metadataId = Guid.Parse("1c8c8313-6fc4-4ebd-9ca8-8a1267441e06");
        var locationDto = new LocationDto("Updated street", "Updated city", "Updated state", "Updated country",
            Continent.Asia);
        var metadataUpdateDto = new MetadataUpdateDto(metadataId, DateTime.UtcNow, locationDto, 5);
        var httpContent = new StringContent(JsonConvert.SerializeObject(metadataUpdateDto), Encoding.UTF8,
            "application/json");
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "");

        // When
        var response = await _httpClient.PutAsync($"{BaseUrl}/{smartMeterId}/metadata/{metadataId}", httpContent);

        // Then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }
}