using System.Linq.Expressions;
using LibraryEcom.Application.Common.Service;

namespace LibraryEcom.Application.Interfaces.Repositories.Base;

public interface IGenericRepository: ITransientService
{
    #region Existence
    bool Exists<TEntity>(Expression<Func<TEntity, bool>>? filter = null) where TEntity : class;
    
   
    #endregion
    
    
    #region Get
    IQueryable<TEntity> GetPagedResult<TEntity>(int pageNumber, int pageSize, out int rowsCount,
        Expression<Func<TEntity, bool>>? filter = null, Expression<Func<TEntity, object>>? order = null, bool isAscendingOrder = true) where TEntity : class;

    IQueryable<TEntity> Get<TEntity>(Expression<Func<TEntity, bool>>? filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        string includeProperties = "") where TEntity : class;

    TEntity? GetById<TEntity>(object id) where TEntity : class;

    TEntity? GetFirstOrDefault<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class;

    #endregion

    #region Count
    int GetCount<TEntity>(Expression<Func<TEntity, bool>>? filter = null) where TEntity : class;
    #endregion

    #region Insert
    Guid Insert<TEntity>(TEntity entity) where TEntity : class;
    #endregion

    #region Update
    void Update<TEntity>(TEntity entity) where TEntity : class;
    #endregion

    #region Delete
    void Delete<TEntity>(object id) where TEntity : class;
    
    void Delete<TEntity>(TEntity entity) where TEntity : class;
    #endregion
}