using Microsoft.Extensions.Logging;

using SMAIAXBackend.Application.DTOs;
using SMAIAXBackend.Application.Exceptions;
using SMAIAXBackend.Application.Services.Interfaces;
using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.Enums;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;
using SMAIAXBackend.Domain.Repositories;
using SMAIAXBackend.Domain.Specifications;

namespace SMAIAXBackend.Application.Services.Implementations;

public class PolicyListService(
    IPolicyRepository policyRepository,
    ISmartMeterRepository smartMeterRepository,
    ITenantRepository tenantRepository,
    IMeasurementListService measurementListService,
    ITenantContextService tenantContextService,
    ILogger<PolicyListService> logger) : IPolicyListService
{
    public async Task<List<PolicyDto>> GetPoliciesBySmartMeterIdAsync(SmartMeterId smartMeterId)
    {
        var policies = await policyRepository.GetPoliciesBySmartMeterIdAsync(smartMeterId);
        return policies.Select(PolicyDto.FromPolicy).ToList();
    }

    public async Task<List<PolicyDto>> GetPoliciesAsync()
    {
        var policies = await policyRepository.GetPoliciesAsync();
        return policies.Select(PolicyDto.FromPolicy).ToList();
    }

    public async Task<List<PolicyDto>> GetFilteredPoliciesAsync(decimal? maxPrice,
        MeasurementResolution? measurementResolution, LocationResolution? locationResolution)
    {
        var matchingPolicies = new List<PolicyDto>();
        ISpecification<Policy> specification = new BaseSpecification<Policy>();

        if (maxPrice.HasValue)
        {
            var priceSpecification = new PriceSpecification(maxPrice.Value);
            specification = new AndSpecification<Policy>(specification, priceSpecification);
        }

        if (measurementResolution.HasValue)
        {
            var measurementResolutionSpecification =
                new MeasurementResolutionSpecification(measurementResolution.Value);
            specification = new AndSpecification<Policy>(specification, measurementResolutionSpecification);
        }

        if (locationResolution.HasValue)
        {
            var locationResolutionSpecification =
                new PolicyLocationResolutionSpecification(locationResolution.Value);
            specification = new AndSpecification<Policy>(specification, locationResolutionSpecification);
        }

        var currentTenant = await tenantContextService.GetCurrentTenantAsync();
        var tenants = await tenantRepository.GetAllAsync();

        foreach (var tenant in tenants.Where(t => !t.Equals(currentTenant)))
        {
            var policies = await policyRepository.GetPoliciesByTenantAsync(tenant);
            var filteredPolicies = policies.Where(policy => specification.IsSatisfiedBy(policy)).ToList();
            matchingPolicies.AddRange(filteredPolicies.Select(PolicyDto.FromPolicy));
        }

        return matchingPolicies;
    }

    public async Task<List<MeasurementDto>> GetMeasurementsByPolicyIdAsync(Guid policyId)
    {
        var policy = await policyRepository.GetPolicyByIdAsync(new PolicyId(policyId));
        if (policy == null)
        {
            logger.LogError("Policy with id '{policyId}' not found.", policyId);
            throw new PolicyNotFoundException(policyId);
        }

        if (policy.LocationResolution == LocationResolution.None)
        {
            // If location resolution "does not matter", return all measurements.
            return await measurementListService.GetMeasurementsBySmartMeterAndResolutionAsync(policy.SmartMeterId.Id, policy.MeasurementResolution);
        }

        // Otherwise location resolution must match with metadata.
        var smartMeter = await smartMeterRepository.GetSmartMeterByIdAsync(policy.SmartMeterId);
        if (smartMeter == null)
        {
            throw new SmartMeterNotFoundException(policy.SmartMeterId);
        }

        var metadata = smartMeter.Metadata.OrderBy(m => m.ValidFrom).ToList();
        if (metadata.Count == 0)
        {
            // If no metadata given, policy location resolution can not match because there is no location reference for the measurements.
            return [];
        }

        var timeSpans = new List<(DateTime?, DateTime?)>();
        var specification = new MetadataLocationResolutionSpecification(policy.LocationResolution);
        for (var i = 0; i < metadata!.Count; i += 1)
        {
            if (!specification.IsSatisfiedBy(metadata[i]))
            {
                continue;
            }

            var nextIndex = i + 1;
            timeSpans.Add((metadata[i].ValidFrom, nextIndex >= metadata!.Count ? null : metadata[i + 1].ValidFrom));
        }

        return await measurementListService.GetMeasurementsBySmartMeterAndResolutionAsync(smartMeter.Id.Id,
            policy.MeasurementResolution, timeSpans);
    }
}