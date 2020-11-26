using System.Threading.Tasks;
using Herald.Rendering.Scriban;
using Xunit;

namespace Herald.Tests
{
    public class RendererTests
    {
        [Fact]
        public async Task CanRenderTemplate()
        {
            const string template = @"hello {{who}}";
            var r = new ScribanRenderer();

            var rendered = await r.RenderAsync(template, new
            {
                Who = "world"
            });

            Assert.Equal("hello world", rendered);
        }

        [Fact]
        public async Task CanRenderLists()
        {
            var r = new ScribanRenderer();
            const string template = @"{{- for item in list -}}{{ item }}{{- end -}}";
            var model = new
            {
                List = new[] {"a", "b", "c"}
            };

            var rendered = await r.RenderAsync(template, model);

            Assert.Equal(@"abc", rendered, ignoreWhiteSpaceDifferences: true, ignoreLineEndingDifferences: true);
        }
    }
}