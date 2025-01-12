using SMAIAXBackend.Application.Services.Interfaces;
using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.ValueObjects;
using SMAIAXBackend.Domain.Repositories;
using SMAIAXBackend.Domain.Repositories.Transactions;

namespace SMAIAXBackend.Application.Services.Implementations;

public class OrderService(
    ISmartMeterRepository smartMeterRepository,
    IMqttBrokerRepository mqttBrokerRepository,
    IVaultRepository vaultRepository,
    IEncryptionService encryptionService,
    ITransactionManager transactionManager) : IOrderService
{
    public async Task<ConnectorSerialNumber> OrderSmartMeterConnectorAsync()
    {
        var smartMeterId = smartMeterRepository.NextIdentity();
        var connectorSerialNumber = new ConnectorSerialNumber(Guid.NewGuid());

        //We would now trigger a process of burning the private key into the fuse of the connector for the smart meter
        //This however was out of scope for the project, therefore the private key is not used at this place
        var (publicKey, privateKey) = encryptionService.GenerateKeys();

        await transactionManager.ReadCommittedTransactionScope(async () =>
        {
            var smartMeter = SmartMeter.Create(smartMeterId,
                "Smart Meter " + GenerateRandomEmoji() + GenerateRandomEmoji() + GenerateRandomEmoji(),
                connectorSerialNumber, publicKey);
            await smartMeterRepository.AddAsync(smartMeter);

            string topic = $"smartmeter/{smartMeterId}";
            string username = $"smartmeter-{smartMeterId}";
            string password = $"{Guid.NewGuid()}-{Guid.NewGuid()}";
            await vaultRepository.SaveMqttBrokerCredentialsAsync(smartMeterId, topic, username, password);
            await mqttBrokerRepository.CreateMqttUserAsync(topic, username, password);
        });

        return connectorSerialNumber;
    }

    private static string GenerateRandomEmoji()
    {
        Random random = new Random();

        var emojiRanges = new List<(int Min, int Max)>
        {
            (0x1F600, 0x1F64F), // Smiley faces
            (0x1F300, 0x1F5FF), // Miscellaneous symbols
            (0x1F680, 0x1F6FF), // Transport and map symbols
            (0x1F400, 0x1F4FF), // Animals and nature
        };

        var selectedRange = emojiRanges[random.Next(emojiRanges.Count)];
        int randomEmojiCode = random.Next(selectedRange.Min, selectedRange.Max + 1);
        return char.ConvertFromUtf32(randomEmojiCode);
    }
}