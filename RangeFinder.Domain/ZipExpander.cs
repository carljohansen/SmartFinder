using System.IO.Compression;

namespace RangeFinder.Domain;

public class ZipExpander
{
    public readonly static ZipExpander Instance = new();

    public event EventHandler<ExtractionProgressEventArgs>? ExtractionProgressUpdate;

    private ZipExpander()
    {
    }

    public async Task<string?> UnzipToTempDirectoryAsync(string zipFileFullPath, CancellationToken cancellationToken)
    {
        try
        {
            var zipFileName = Path.GetFileNameWithoutExtension(zipFileFullPath);
            var index = 0;
            // Create a unique temporary directory
            string? tempDirPath = null;
            do
            {
                var tryName = zipFileName + (index > 0 ? $"_{index}" : "");
                var tryPath = Path.GetFullPath(Path.Join(Path.GetTempPath(), "RangeFinder", tryName));

                if (!tryPath.EndsWith(Path.DirectorySeparatorChar))
                {
                    tryPath += Path.DirectorySeparatorChar;
                }

                if (Directory.Exists(tryPath)
                    || File.Exists(tryPath))
                {
                    index++;
                }
                else
                {
                    tempDirPath = tryPath;
                }
            } while (tempDirPath == null);

            Directory.CreateDirectory(tempDirPath);

            // Extract ZIP contents
            await SafeExtractAsync(zipFileFullPath, tempDirPath, 100.0f, cancellationToken);

            if (cancellationToken.IsCancellationRequested)
            {
                return null;
            }

            while (true)
            {
                var singleDirectory = ContainsSingleDirectory(tempDirPath);
                if (singleDirectory == null)
                {
                    // Contains more than just a single dir.
                    return tempDirPath;
                }
                tempDirPath = singleDirectory;
            }
        }
        catch (InvalidDataException)
        {
            Console.WriteLine("Error: The file is not a valid ZIP archive.");
            throw;
        }
        catch (UnauthorizedAccessException)
        {
            Console.WriteLine("Error: Access denied while reading or writing files.");
            throw;
        }
        catch (IOException ex)
        {
            Console.WriteLine($"I/O Error: {ex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error: {ex.Message}");
            throw;
        }
    }

    private static string? ContainsSingleDirectory(string directoryPath)
    {
        var entries = Directory.GetFileSystemEntries(directoryPath);
        if (entries.Length == 1 && Directory.Exists(entries[0]))
        {
            return entries[0];
        }
        return null;
    }

    public async Task SafeExtractAsync(string zipFilePath, string destinationDir, float maxDecompressedSizeMB, CancellationToken cancellationToken)
    {
        var maxDecompressedBytes = (long)(maxDecompressedSizeMB * 1024 * 1024);

        await using ZipArchive archive = await ZipFile.OpenReadAsync(zipFilePath, cancellationToken);
        ValidatePaths(archive, destinationDir);
        long totalExtracted = 0;

        var numEntries = archive.Entries.Count;
        var numEntriesProcessed = 0;
        var eventStreamTimer = System.Diagnostics.Stopwatch.StartNew();

        foreach (ZipArchiveEntry entry in archive.Entries)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            totalExtracted = checked(totalExtracted + entry.Length);
            if (totalExtracted > maxDecompressedBytes)
            {
                throw new InvalidOperationException(
                    $"Archive total decompressed size exceeds the allowed limit of {maxDecompressedSizeMB} MB.");
            }
            string destinationPath = Path.GetFullPath(Path.Join(destinationDir, entry.FullName));
            await SafeExtractEntryAsync(entry, destinationPath, maxDecompressedSize: 100 * 1024 * 1024, cancellationToken); // Example limit: 100 MB per entry
            numEntriesProcessed++;

            if (ExtractionProgressUpdate != null
                && eventStreamTimer.ElapsedMilliseconds > 150)
            {
                await Task.Delay(1, cancellationToken);
                eventStreamTimer.Restart();
                ExtractionProgressUpdate.Invoke(this, new ExtractionProgressEventArgs { ProgressPercentage = (numEntriesProcessed * 100 / numEntries) });
            }
        }
    }

    private static void ValidatePaths(ZipArchive archive, string destinationDir)
    {
        foreach (ZipArchiveEntry entry in archive.Entries)
        {
            string destPath = Path.GetFullPath(Path.Join(destinationDir, entry.FullName));

            if (!destPath.StartsWith(destinationDir, StringComparison.Ordinal))
                throw new IOException(
                    $"Entry '{entry.FullName}' would extract outside the destination directory.");
        }
    }

    private static async Task SafeExtractEntryAsync(ZipArchiveEntry entry, string destinationPath, long maxDecompressedSize, CancellationToken cancellationToken)
    {
        // The runtime enforces that entry.Open() will never produce more than
        // entry.Length bytes, so checking the declared size is sufficient.
        if (entry.Length > maxDecompressedSize)
        {
            throw new InvalidOperationException(
                $"Entry '{entry.FullName}' declares size {entry.Length}, exceeding limit {maxDecompressedSize}.");
        }

        if (string.IsNullOrEmpty(entry.Name))
        {
            return;
        }

        var destDir = Path.GetDirectoryName(destinationPath);
        if (destDir != null)
        {
            Directory.CreateDirectory(destDir);
        }

        await entry.ExtractToFileAsync(destinationPath, overwrite: false, cancellationToken);
    }
}