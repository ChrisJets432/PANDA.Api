namespace PANDA.Infrastructure.Core;

public interface IDaoConverter<TDao, TEntity>
{
    static abstract TDao? ToDatabase(TEntity? entity);
    static abstract TEntity? FromDatabase(TDao? dao);
}