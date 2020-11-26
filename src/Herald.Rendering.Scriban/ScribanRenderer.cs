using System.Threading;
using System.Threading.Tasks;
using Scriban;

namespace Herald.Rendering.Scriban
{
    public class ScribanRenderer : IRenderer
    {
        public async Task<string> RenderAsync<T>(
            string template, T model,
            CancellationToken cancellationToken = default
        ) =>
            await Template.Parse(template)
                .RenderAsync(model)
                .ConfigureAwait(false);
    }
}