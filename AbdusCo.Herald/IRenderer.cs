using System.Threading;
using System.Threading.Tasks;

namespace AbdusCo.Herald
{
    public interface IRenderer
    {
        public Task<string> RenderAsync<T>(string template, T model, CancellationToken cancellationToken = default);
    }
}