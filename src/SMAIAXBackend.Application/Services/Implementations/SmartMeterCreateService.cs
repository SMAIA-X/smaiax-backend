using Microsoft.Extensions.Logging;

using SMAIAXBackend.Application.DTOs;
using SMAIAXBackend.Application.Exceptions;
using SMAIAXBackend.Application.Services.Interfaces;
using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;
using SMAIAXBackend.Domain.Repositories;

namespace SMAIAXBackend.Application.Services.Implementations;

public class SmartMeterCreateService(
    ISmartMeterRepository smartMeterRepository,
    IUserRepository userRepository,
    ILogger<SmartMeterCreateService> logger) : ISmartMeterCreateService
{
    public async Task<Guid> AddSmartMeterAsync(SmartMeterCreateDto smartMeterCreateDto, string? userId)
    {
        if (string.IsNullOrEmpty(userId))
        {
            logger.LogWarning("No user claim found in claims principal.");
            throw new InvalidTokenException();
        }

        if (!Guid.TryParse(userId, out var userIdGuid))
        {
            logger.LogWarning("Invalid user claim found in claims principal.");
            throw new InvalidTokenException();
        }

        var user = await userRepository.GetUserByIdAsync(new UserId(userIdGuid));

        if (user is null)
        {
            logger.LogWarning("User with id {UserId} not found in database.", userIdGuid);
            throw new UserNotFoundException(userIdGuid);
        }

        var smartMeterId = smartMeterRepository.NextIdentity();
        var smartMeter = SmartMeter.Create(smartMeterId, smartMeterCreateDto.Name, new UserId(userIdGuid));
        await smartMeterRepository.AddAsync(smartMeter);

        return smartMeterId.Id;
    }
}