using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.ValueObjects;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;

namespace SMAIAXBackend.Domain.UnitTests;

[TestFixture]
public class UserTests
{
    [Test]
    public void GivenUserDetails_WhenCreateUser_ThenUserIsCreated()
    {
        // Given
        var userId = new UserId(Guid.NewGuid());
        var name = new Name("John", "Doe");
        var email = "john.doe@example.com";

        // When
        var user = User.Create(userId, name, email);

        // Then
        Assert.Multiple(() =>
        {
            Assert.That(user.Id, Is.EqualTo(userId));
            Assert.That(user.Name, Is.EqualTo(name));
            Assert.That(user.Email, Is.EqualTo(email));
        });
    }
}