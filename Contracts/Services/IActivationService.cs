namespace Generico_Front.Contracts.Services;

public interface IActivationService
{
    Task ActivateAsync(object activationArgs);
}
