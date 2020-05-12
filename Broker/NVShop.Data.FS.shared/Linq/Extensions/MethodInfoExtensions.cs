namespace NVShop.Data.FS.Linq.Extensions
{
	using System.Reflection;

	public static class MethodInfoExtensions
	{
		/// <summary>
		///     This API supports the Entity Framework Core infrastructure and is not intended to be used
		///     directly from your code. This API may change or be removed in future releases.
		/// </summary>
		public static bool MethodIsClosedFormOf(this MethodInfo methodInfo, MethodInfo genericMethod)
		{
			return methodInfo.IsGenericMethod && Equals(methodInfo.GetGenericMethodDefinition(), genericMethod);
		}
	}
}
