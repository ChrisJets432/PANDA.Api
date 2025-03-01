using PANDA.Common;
using PANDA.Services.Core;

namespace PANDA.Api.Core;

public interface IRequestConverter<TRequest, TEntity, out TId> : IIdentifiable<TId>
{
    static abstract TRequest? ToRequestEntity(TEntity? entity);
    static abstract OperationResult FromRequestEntity(TRequest? request, out TEntity? entity);
}