using PANDA.Common;

namespace PANDA.Infrastructure.Core;

public interface IRepository<TOut, in TId> 
    where TOut: class 
{
    IEnumerable<TOut> List();
    TOut? Get(TId? id);
    OperationResult Add(TOut? entity, out TOut? addedEntity);
    OperationResult Update(TOut? entity);
    OperationResult Delete(TId? id);
}