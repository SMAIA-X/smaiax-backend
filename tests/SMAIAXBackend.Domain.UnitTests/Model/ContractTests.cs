using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;

namespace SMAIAXBackend.Domain.UnitTests.Model;

[TestFixture]
public class ContractTests
{
    [Test]
    public void GivenContractDetails_WhenCreateContract_ThenDetailsEquals()
    {
        // Given
        var contractId = new ContractId(Guid.NewGuid());
        var createdAt = DateTime.UtcNow;
        var policyId = new PolicyId(Guid.NewGuid());

        // When
        Contract contract = Contract.Create(contractId, createdAt, policyId);

        // Then
        Assert.Multiple(() =>
        {
            Assert.That(contract.Id, Is.EqualTo(contractId));
            Assert.That(contract.CreatedAt, Is.EqualTo(createdAt));
            Assert.That(contract.PolicyId, Is.EqualTo(policyId));
        });
    }
}