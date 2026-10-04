using Core.Infrastructure.McpServer;
using FluentAssertions;
using Microsoft.Extensions.Configuration;

namespace UnitTests.Infrastructure.McpServer
{
    /// <summary>
    /// A reload-enabled file source rooted at the working directory makes the server watch that whole
    /// tree; launched from a home directory that was every directory under $HOME.
    /// </summary>
    public class McpHostBuilderTests
    {
        [Fact]
        public void Create_PinsContentRootToInstallDirectory()
        {
            var builder = McpHostBuilder.Create([]);

            Path.TrimEndingDirectorySeparator(builder.Environment.ContentRootPath)
                .Should().Be(Path.TrimEndingDirectorySeparator(AppContext.BaseDirectory));
        }

        [Fact]
        public void Create_DisablesReloadOnChangeForEveryFileSource()
        {
            var builder = McpHostBuilder.Create([]);

            var sources = ((IConfigurationBuilder)builder.Configuration).Sources
                .OfType<FileConfigurationSource>()
                .ToList();

            sources.Should().NotBeEmpty();
            sources.Should().OnlyContain(source => !source.ReloadOnChange);
        }
    }
}
