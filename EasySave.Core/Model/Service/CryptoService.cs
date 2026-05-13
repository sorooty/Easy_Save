using System.Diagnostics;

namespace EasySave.Core.Model.Service
{
    /// <summary>
    /// Invokes the external CryptoSoft.exe to encrypt a file in-place.
    /// Returns the encryption duration in ms (>0), 0 when not encrypted, or a negative error code.
    /// </summary>
    public class CryptoService
    {
        /// <summary>
        /// Determines whether a file should be encrypted based on its extension and the configured list.
        /// </summary>
        public bool NeedsEncryption(string filePath, IEnumerable<string> encryptedExtensions)
        {
            var ext = Path.GetExtension(filePath);
            return encryptedExtensions.Any(e =>
                string.Equals(e, ext, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Runs CryptoSoft.exe on the target file and returns:
        /// >0  elapsed encryption time in ms,
        /// &lt;0  CryptoSoft exit code (error),
        /// -1  if the executable is missing or the process could not start.
        /// </summary>
        public long Encrypt(string targetFile, string cryptoSoftPath)
        {
            if (string.IsNullOrWhiteSpace(cryptoSoftPath) || !File.Exists(cryptoSoftPath))
                return -1;

            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = cryptoSoftPath,
                    Arguments = $"\"{targetFile}\" \"{targetFile}\"",
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                var sw = Stopwatch.StartNew();
                using var process = Process.Start(psi);
                process?.WaitForExit();
                sw.Stop();

                int exitCode = process?.ExitCode ?? -1;
                return exitCode < 0 ? exitCode : sw.ElapsedMilliseconds;
            }
            catch
            {
                return -1;
            }
        }
    }
}
