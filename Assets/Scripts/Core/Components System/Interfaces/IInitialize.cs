/// <summary>
/// The <c>IInitialize</c> interface defines a contract for objects that require initialization.
/// Implementing this interface ensures that the object can be initialized, allowing
/// for custom setup logic to be executed when required.
/// </summary>
public interface IInitialize
{
    /// <summary>
    /// Initializes the object, setting up any necessary states or configurations.
    /// This method should be implemented by any class that needs to perform initialization
    /// logic before being used in the application.
    /// </summary>
    void Initialize();
}
