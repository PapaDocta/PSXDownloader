using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PSXDownloader.MVVM.Data;
using PSXDownloader.MVVM.Models;
using Xunit;

namespace PSXDLL.Tests
{
    public class RepositoryTests
    {
        [Fact]
        public async Task CreateAndRetrieveLocalPath()
        {
            var options = new DbContextOptionsBuilder<PSXDataContext>()
                .UseInMemoryDatabase("repo")
                .Options;

            var repo = new PSXRepository(options);
            var entity = new PSXDatabase { Title = "Test", TitleID = "ID1", LocalPath = "c:/tmp" };
            await repo.Create(entity);

            string path = await repo.GetLocalPath("ID1");
            Assert.Equal("c:/tmp", path);
        }
    }
}
