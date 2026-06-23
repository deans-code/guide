using System.Linq.Expressions;

public static class SpecificationExtensions
{
    public static ISpecification<T> And<T>(this ISpecification<T> left, ISpecification<T> right)
        => new AndSpecification<T>(left, right);

    public static ISpecification<T> Or<T>(this ISpecification<T> left, ISpecification<T> right)
        => new OrSpecification<T>(left, right);

    public static ISpecification<T> Not<T>(this ISpecification<T> spec)
        => new NotSpecification<T>(spec);
}

internal sealed class AndSpecification<T>(ISpecification<T> left, ISpecification<T> right) : ISpecification<T>
{
    public Expression<Func<T, bool>> Criteria
    {
        get
        {
            var l = left.Criteria;
            var r = right.Criteria;
            var param = l.Parameters[0];
            var rBody = new ParameterReplacer(r.Parameters[0], param).Visit(r.Body)!;
            return Expression.Lambda<Func<T, bool>>(Expression.AndAlso(l.Body, rBody), param);
        }
    }
}

internal sealed class OrSpecification<T>(ISpecification<T> left, ISpecification<T> right) : ISpecification<T>
{
    public Expression<Func<T, bool>> Criteria
    {
        get
        {
            var l = left.Criteria;
            var r = right.Criteria;
            var param = l.Parameters[0];
            var rBody = new ParameterReplacer(r.Parameters[0], param).Visit(r.Body)!;
            return Expression.Lambda<Func<T, bool>>(Expression.OrElse(l.Body, rBody), param);
        }
    }
}

internal sealed class NotSpecification<T>(ISpecification<T> inner) : ISpecification<T>
{
    public Expression<Func<T, bool>> Criteria
    {
        get
        {
            var expr = inner.Criteria;
            return Expression.Lambda<Func<T, bool>>(Expression.Not(expr.Body), expr.Parameters[0]);
        }
    }
}

internal sealed class ParameterReplacer(ParameterExpression from, ParameterExpression to) : ExpressionVisitor
{
    protected override Expression VisitParameter(ParameterExpression node)
        => node == from ? to : base.VisitParameter(node);
}
