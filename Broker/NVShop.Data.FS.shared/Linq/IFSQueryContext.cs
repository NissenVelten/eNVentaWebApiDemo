namespace NVShop.Data.FS.Linq
{
    using System.Linq.Expressions;
	using System.Threading;

	internal interface IFSQueryContext
    {
		/// <summary>
		/// 
		/// </summary>
		/// <param name="expression"></param>
		/// <param name="b"></param>
		/// <returns></returns>
        object Execute(Expression expression, bool b);

		/// <summary>
		///     Gets or sets the cancellation token.
		/// </summary>
		/// <value>
		///     The cancellation token.
		/// </value>
		CancellationToken CancellationToken { get; set; }
	}
}
