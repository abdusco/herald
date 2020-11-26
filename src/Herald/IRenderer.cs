using System.Threading;
using System.Threading.Tasks;

namespace Herald
{
    public interface IRenderer
    {
        public Task<string> RenderAsync<T>(string template, T model, CancellationToken cancellationToken = default);
    }
}