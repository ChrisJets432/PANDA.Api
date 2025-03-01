namespace PANDA.Services.Core;

public interface IIdentifiable<out T>
{
    T? Id { get; }
}