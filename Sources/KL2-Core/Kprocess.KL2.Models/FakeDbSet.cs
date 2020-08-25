using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Kprocess.KL2.Models
{
    public class FakeDbSet<TEntity> : DbSet<TEntity>, IQueryable, IEnumerable<TEntity>, IDbAsyncEnumerable<TEntity> where TEntity : class
    {
        readonly PropertyInfo[] _primaryKeys;
        readonly ObservableCollection<TEntity> _data;
        readonly IQueryable _query;

        public FakeDbSet()
        {
            _data = new ObservableCollection<TEntity>();
            _query = _data.AsQueryable();
        }

        public FakeDbSet(params string[] primaryKeys)
        {
            _primaryKeys = typeof(TEntity).GetProperties().Where(x => primaryKeys.Contains(x.Name)).ToArray();
            _data = new ObservableCollection<TEntity>();
            _query = _data.AsQueryable();
        }

        public override TEntity Find(params object[] keyValues)
        {
            if (_primaryKeys == null)
                throw new ArgumentException("No primary keys defined");
            if (keyValues.Length != _primaryKeys.Length)
                throw new ArgumentException("Incorrect number of keys passed to Find method");

            var keyQuery = this.AsQueryable();
            keyQuery = keyValues
                .Select((t, i) => i)
                .Aggregate(keyQuery,
                    (current, x) =>
                        current.Where(entity => _primaryKeys[x].GetValue(entity, null).Equals(keyValues[x])));

            return keyQuery.SingleOrDefault();
        }

        public override Task<TEntity> FindAsync(CancellationToken cancellationToken, params object[] keyValues)
        {
            return Task<TEntity>.Factory.StartNew(() => Find(keyValues), cancellationToken);
        }

        public override Task<TEntity> FindAsync(params object[] keyValues)
        {
            return Task<TEntity>.Factory.StartNew(() => Find(keyValues));
        }

        public override IEnumerable<TEntity> AddRange(IEnumerable<TEntity> entities)
        {
            if (entities == null) throw new ArgumentNullException(nameof(entities));
            var items = entities.ToList();
            foreach (var entity in items)
                _data.Add(entity);
            return items;
        }

        public override TEntity Add(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            _data.Add(entity);
            return entity;
        }

        public override IEnumerable<TEntity> RemoveRange(IEnumerable<TEntity> entities)
        {
            if (entities == null) throw new ArgumentNullException(nameof(entities));
            var items = entities.ToList();
            foreach (var entity in items)
                _data.Remove(entity);
            return items;
        }

        public override TEntity Remove(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            _data.Remove(entity);
            return entity;
        }

        public override TEntity Attach(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            _data.Add(entity);
            return entity;
        }

        public override TEntity Create()
        {
            return Activator.CreateInstance<TEntity>();
        }

        public override TDerivedEntity Create<TDerivedEntity>()
        {
            return Activator.CreateInstance<TDerivedEntity>();
        }

        public override ObservableCollection<TEntity> Local =>
            _data;

        Type IQueryable.ElementType =>
            _query.ElementType;

        Expression IQueryable.Expression =>
            _query.Expression;

        IQueryProvider IQueryable.Provider =>
            new FakeDbAsyncQueryProvider<TEntity>(_query.Provider);

        IEnumerator IEnumerable.GetEnumerator() =>
            _data.GetEnumerator();

        IEnumerator<TEntity> IEnumerable<TEntity>.GetEnumerator() =>
            _data.GetEnumerator();

        IDbAsyncEnumerator<TEntity> IDbAsyncEnumerable<TEntity>.GetAsyncEnumerator() =>
            new FakeDbAsyncEnumerator<TEntity>(_data.GetEnumerator());
    }

    public class FakeDbAsyncQueryProvider<TEntity> : IDbAsyncQueryProvider
    {
        private readonly IQueryProvider _inner;

        public FakeDbAsyncQueryProvider(IQueryProvider inner)
        {
            _inner = inner;
        }

        public IQueryable CreateQuery(Expression expression) =>
            new FakeDbAsyncEnumerable<TEntity>(expression);

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression) =>
            new FakeDbAsyncEnumerable<TElement>(expression);

        public object Execute(Expression expression) =>
            _inner.Execute(expression);

        public TResult Execute<TResult>(Expression expression) =>
            _inner.Execute<TResult>(expression);

        public Task<object> ExecuteAsync(Expression expression, CancellationToken cancellationToken) =>
            Task.FromResult(Execute(expression));

        public Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken) =>
            Task.FromResult(Execute<TResult>(expression));
    }

    public class FakeDbAsyncEnumerable<T> : EnumerableQuery<T>, IDbAsyncEnumerable<T>, IQueryable<T>
    {
        public FakeDbAsyncEnumerable(IEnumerable<T> enumerable)
            : base(enumerable)
        { }

        public FakeDbAsyncEnumerable(Expression expression)
            : base(expression)
        { }

        public IDbAsyncEnumerator<T> GetAsyncEnumerator() =>
            new FakeDbAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());

        IDbAsyncEnumerator IDbAsyncEnumerable.GetAsyncEnumerator() =>
            GetAsyncEnumerator();

        IQueryProvider IQueryable.Provider =>
            new FakeDbAsyncQueryProvider<T>(this);
    }

    public class FakeDbAsyncEnumerator<T> : IDbAsyncEnumerator<T>
    {
        readonly IEnumerator<T> _inner;

        public FakeDbAsyncEnumerator(IEnumerator<T> inner)
        {
            _inner = inner;
        }

        public void Dispose() =>
            _inner.Dispose();

        public Task<bool> MoveNextAsync(CancellationToken cancellationToken) =>
            Task.FromResult(_inner.MoveNext());

        public T Current =>
            _inner.Current;

        object IDbAsyncEnumerator.Current =>
            Current;
    }
}
