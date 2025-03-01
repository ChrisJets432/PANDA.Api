using PANDA.Common;

namespace PANDA.Services.Core;

public interface IService<TOut, in TId> 
    where TOut: class
{
    List<TOut> List();
    TOut? Get(TId? id);
    OperationResult Add(TOut? entity, out TOut? addedEntity);
    OperationResult Update(TOut? entity);
    OperationResult Delete(TId? id);
}