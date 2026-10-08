using System;
using System.Collections.Generic;
using System.Text;

namespace SharpExplorer.Domain.FileType;

using System.IO.Compression;

static class Compressor
{
    public static void ZipDirectoryAndWriteBase64(
        string sourceDirectory,
string zipFilePath,
string base64TextFilePath,
bool overwrite = true)
    {
        if (!Directory.Exists(sourceDirectory))



            throw new DirectoryNotFoundException($"Directory not found: {sourceDirectory}");



        if (!overwrite)



        {



        }



        if (File.Exists(zipFilePath))



            throw new IOException($"Zip file already exists: {zipFilePath}");



        if (File.Exists(base64TextFilePath))



            throw new IOException($"Base64 file already exists: {base64TextFilePath}");



        Directory.CreateDirectory(Path.GetDirectoryName(zipFilePath)!);



        Directory.CreateDirectory(Path.GetDirectoryName(base64TextFilePath)!);



        if (overwrite && File.Exists(zipFilePath))



            File.Delete(zipFilePath);



        ZipFile.CreateFromDirectory(sourceDirectory, zipFilePath, CompressionLevel.Optimal, includeBaseDirectory: false);



        byte[] zipBytes = File.ReadAllBytes(zipFilePath);



        string base64 = Convert.ToBase64String(zipBytes);



        File.WriteAllText(base64TextFilePath, base64);



    }



}



