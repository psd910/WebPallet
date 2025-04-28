
using CoreWebTinhTien.NHibernateSession;
using System.Collections.Generic;

namespace CoreWebTinhTien.BaseServices
{
    public interface IBaseService<T, IdT>
    {
        System.Linq.IQueryable<T> Query
        {
            get;
        }

        T CreateNew(T entity);

        T Delete(T entity);

        T Delete(IdT key);

        T Update(T entity);

        T Save(T entity);

        R ExecuteScalar<R>(string Query, bool isHQL, params SQLParam[] _params);

        T Getbykey(IdT key);

        List<T> GetAll();

        IList<T> GetAll(int pageIndex, int pageSize, out int total);

        List<T> GetbySQLQuery(string Query, params SQLParam[] _params);

        List<T> GetbyHQuery(string Query, params SQLParam[] _params);

        IList<T> GetbySQLQuery(string Query, int pageIndex, int pageSize, out int total, params SQLParam[] _params);

        List<T> GetbyHQuery(string query, int pageIndex, int pageSize, out int total, params SQLParam[] _params);

        T Get(System.Linq.Expressions.Expression<System.Func<T, bool>> predicate);

        int ExecuteCountQuery(string Query, bool isHQL, params SQLParam[] _params);

        object ExcuteNonQuery(string query, bool isHQL, params SQLParam[] _params);

        void CommitChanges();

        void BindSession(object entity);

        void UnbindSession(object entity);

        void BeginTran();

        void CommitTran();

        void RolbackTran();

        void SetFetchPage(int from, int maxResult);

        void ResetFetchPage();

        void Clear();
    }
}
