namespace Przetrwaj.Domain.Helpers;

public class GenericCompare<T> : IEqualityComparer<T> where T : class
{
	private Func<T, object> _expr { get; set; }
	public GenericCompare(Func<T, object> expr)
	{
		this._expr = expr;
	}
	public bool Equals(T? x, T? y)
	{
		if (x is null || y is null) return false;
		var first = _expr.Invoke(x);
		var sec = _expr.Invoke(y);
		if (first != null)
			return first.Equals(sec);
		return false;
	}
	public int GetHashCode(T obj)
	{
		if (obj == null) return 0;
		var value = _expr.Invoke(obj);
		return value?.GetHashCode() ?? 0;
	}
}