namespace PANDA.Api.Core;

public interface IIdentifiable<out T>
{
    T? Id { get; }
}