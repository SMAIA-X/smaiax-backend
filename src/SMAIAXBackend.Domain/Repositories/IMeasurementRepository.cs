using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.Enums;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;

namespace SMAIAXBackend.Domain.Repositories;

public interface IMeasurementRepository
{
    /// <summary>
    ///     Reads all measurements of a tenant, and it's smart meter in the given time range and calculates the amount.
    /// </summary>
    /// <param name="smartMeterId">The specific smart meter id.</param>
    /// <param name="startAt">Optional timestamp filter. Data with a timestamp newer/greater than or equal to startAt will be returned.</param>
    /// <param name="endAt">Optional timestamp filter. Data with a timestamp older/smaller than or equal to endAt will be returned.</param>
    /// <returns>All measurements between given limitations and a count variable.</returns>
    Task<(List<Measurement>, int)> GetMeasurementsBySmartMeterAsync(SmartMeterId smartMeterId, DateTime? startAt,
        DateTime? endAt);

    Task<(List<AggregatedMeasurement>, int)> GetMeasurementsBySmartMeterAndResolutionAsync(
        SmartMeterId smartMeterId,
        MeasurementResolution measurementResolution, DateTime? startAt, DateTime? endAt);
}