using EasySave.Core.Model.Entities;
using EasySave.Core.Model.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EasySave.Tests
{
    [TestClass]
    public class LargeFileTransferServiceTests
    {
        [TestMethod]
        public async Task ExecuteTransferAsync_ShouldBlockTwoLargeFilesAtSameTime()
        {
            var settings = new GeneralSettings
            {
                LargeFileLimitKo = 1
            };

            var service = new LargeFileTransferService(settings);

            string tempFile1 = Path.GetTempFileName();
            string tempFile2 = Path.GetTempFileName();

            await File.WriteAllBytesAsync(tempFile1, new byte[2048]);
            await File.WriteAllBytesAsync(tempFile2, new byte[2048]);

            int simultaneousLargeTransfers = 0;
            int maxSimultaneousLargeTransfers = 0;

            async Task FakeTransfer()
            {
                int current = Interlocked.Increment(ref simultaneousLargeTransfers);

                maxSimultaneousLargeTransfers =
                    Math.Max(maxSimultaneousLargeTransfers, current);

                await Task.Delay(300);

                Interlocked.Decrement(ref simultaneousLargeTransfers);
            }

            await Task.WhenAll(
                service.ExecuteTransferAsync(tempFile1, FakeTransfer),
                service.ExecuteTransferAsync(tempFile2, FakeTransfer)
            );

            File.Delete(tempFile1);
            File.Delete(tempFile2);

            Assert.AreEqual(1, maxSimultaneousLargeTransfers);
        }

        [TestMethod]
        public async Task ExecuteTransferAsync_ShouldAllowSmallFilesAtSameTime()
        {
            var settings = new GeneralSettings
            {
                LargeFileLimitKo = 1000
            };

            var service = new LargeFileTransferService(settings);

            string tempFile1 = Path.GetTempFileName();
            string tempFile2 = Path.GetTempFileName();

            await File.WriteAllBytesAsync(tempFile1, new byte[1024]);
            await File.WriteAllBytesAsync(tempFile2, new byte[1024]);

            int simultaneousTransfers = 0;
            int maxSimultaneousTransfers = 0;

            async Task FakeTransfer()
            {
                int current = Interlocked.Increment(ref simultaneousTransfers);

                maxSimultaneousTransfers =
                    Math.Max(maxSimultaneousTransfers, current);

                await Task.Delay(300);

                Interlocked.Decrement(ref simultaneousTransfers);
            }

            await Task.WhenAll(
                service.ExecuteTransferAsync(tempFile1, FakeTransfer),
                service.ExecuteTransferAsync(tempFile2, FakeTransfer)
            );

            File.Delete(tempFile1);
            File.Delete(tempFile2);

            Assert.AreEqual(2, maxSimultaneousTransfers);
        }
    }
}