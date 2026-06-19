using System.Linq.Expressions;

public abstract class BaseSpecification<T>(Expression<Func<T, bool>> criteria) : ISpecification<T>
{
    public Expression<Func<T, bool>> Criteria => criteria;
}
