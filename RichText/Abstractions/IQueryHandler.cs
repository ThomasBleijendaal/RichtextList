using System.Threading.Tasks;

namespace RichText.Abstractions
{
    public interface IQueryHandler<TQuery, TResult>
    {
        Task<TResult> QueryAsync(TQuery query);
    }
}
