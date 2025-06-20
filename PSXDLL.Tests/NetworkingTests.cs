using PSXDLL;
using Xunit;

namespace PSXDLL.Tests
{
    public class NetworkingTests
    {
        [Fact]
        public void GetUrlFileName_ReturnsName()
        {
            AppConfig.Instance().Rule = "*.pkg";
            string name = UrlOperate.GetUrlFileName("http://test.com/file.pkg");
            Assert.Equal("file.pkg", name);
        }

        [Fact]
        public void GetLocalExternalIp_ReturnsIp()
        {
            var ip = Listener.GetLocalExternalIp();
            Assert.NotNull(ip);
        }
    }
}
