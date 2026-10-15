// Copyright (c) 2019 BitPay.
// All rights reserved.

using BitPay;

namespace BitPayUnitTest
{
    // KeyUtils keeps the key path in a static field, and ClientTest sets it too.
    // Tests in the same collection never run in parallel.
    [Collection(StaticStateCollection)]
    public class KeyUtilsTest : IDisposable
    {
        public const string StaticStateCollection = "KeyUtils static state";

        private readonly string _directory;

        public KeyUtilsTest()
        {
            _directory = Path.Combine(Path.GetTempPath(), "bitpay-keyutils-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_directory);
        }

        public void Dispose()
        {
            Directory.Delete(_directory, true);
            GC.SuppressFinalize(this);
        }

#if NET7_0_OR_GREATER
        private const UnixFileMode OwnerOnly = UnixFileMode.UserRead | UnixFileMode.UserWrite;

        [Fact]
        public async Task it_should_save_the_key_file_readable_only_by_its_owner()
        {
            if (OperatingSystem.IsWindows())
            {
                return; // POSIX permissions do not apply on Windows.
            }

            var path = Path.Combine(_directory, "bitpay_private_test.key");
            var key = KeyUtils.CreateEcKey();

            KeyUtils.PrivateKeyExists(path);
            await KeyUtils.SaveEcKey(key);

            Assert.Equal(OwnerOnly, File.GetUnixFileMode(path));
            Assert.Equal(key.ToAsn1(), KeyUtils.LoadEcKey().ToAsn1());
        }

        [Fact]
        public async Task it_should_tighten_an_existing_key_file()
        {
            if (OperatingSystem.IsWindows())
            {
                return; // POSIX permissions do not apply on Windows.
            }

            var path = Path.Combine(_directory, "bitpay_private_test.key");
            File.WriteAllText(path, "old key");
            File.SetUnixFileMode(path, OwnerOnly | UnixFileMode.GroupRead | UnixFileMode.OtherRead);

            KeyUtils.PrivateKeyExists(path);
            await KeyUtils.SaveEcKey(KeyUtils.CreateEcKey());

            Assert.Equal(OwnerOnly, File.GetUnixFileMode(path));
        }
#endif
    }
}
