// ReSharper disable UseCollectionExpression
// ReSharper disable MemberCanBePrivate.Global
namespace Dalamud.DrunkenToad.Helpers;

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

/// <summary>
/// A helper class for file operations.
/// </summary>
public static class FileHelper
{
    /// <summary>
    /// Moves the specified file from the source directory to the destination directory and compresses into a zip file.
    /// </summary>
    /// <param name="sourceDirectory">The source directory.</param>
    /// <param name="fileName">File name including extension.</param>
    /// <param name="destinationDirectory">The destination directory.</param>
    public static void MoveAndCompressFile(string sourceDirectory, string fileName, string destinationDirectory) =>
        MoveAndCompressFiles(sourceDirectory, new List<string> { fileName }, destinationDirectory, $"{fileName}.zip");

    /// <summary>
    /// Moves the specified files from the source directory to the destination directory and compresses them into a zip
    /// file.
    /// </summary>
    /// <param name="sourceDirectory">The source directory.</param>
    /// <param name="fileNames">The list of file names including extensions.</param>
    /// <param name="destinationDirectory">The destination directory.</param>
    /// <param name="outputZipFileName">The output zip file name.</param>
    public static void MoveAndCompressFiles(string sourceDirectory, List<string> fileNames, string destinationDirectory, string outputZipFileName)
    {
        if (!Directory.Exists(destinationDirectory))
        {
            Directory.CreateDirectory(destinationDirectory);
        }

        var tempDirectory = Path.Combine(destinationDirectory, "temp");
        if (!Directory.Exists(tempDirectory))
        {
            Directory.CreateDirectory(tempDirectory);
        }

        foreach (var fileName in fileNames)
        {
            var sourceFile = Path.Combine(sourceDirectory, fileName);
            var destinationFile = Path.Combine(tempDirectory, fileName);

            if (File.Exists(sourceFile))
            {
                File.Move(sourceFile, destinationFile);
            }
        }

        var zipFilePath = Path.Combine(destinationDirectory, outputZipFileName);
        if (File.Exists(zipFilePath))
        {
            File.Delete(zipFilePath);
        }

        ZipFile.CreateFromDirectory(tempDirectory, zipFilePath);

        Directory.Delete(tempDirectory, true);
    }

    /// <summary>
    /// Compresses the specified file into a zip file with the given destination file name and removes the original
    /// uncompressed file.
    /// </summary>
    /// <param name="filePath">The path of the file to compress.</param>
    /// <param name="destinationFileName">The name of the compressed zip file.</param>
    /// <exception cref="FileNotFoundException">Thrown when the specified file does not exist.</exception>
    public static void CompressFile(string filePath, string destinationFileName)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"The file '{filePath}' does not exist.");
        }

        var tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(tempDirectory);

        var fileName = Path.GetFileName(filePath);
        var destinationFile = Path.Combine(tempDirectory, fileName);

        File.Move(filePath, destinationFile);

        var zipFilePath = Path.Combine(Path.GetDirectoryName(filePath) ?? ".", destinationFileName);

        if (File.Exists(zipFilePath))
        {
            File.Delete(zipFilePath);
        }

        ZipFile.CreateFromDirectory(tempDirectory, zipFilePath);

        Directory.Delete(tempDirectory, true);
    }

    /// <summary>
    /// Moves all files from the source directory to the destination directory and compresses them into a zip file.
    /// </summary>
    /// <param name="sourceDirectory">The source directory.</param>
    /// <param name="destinationDirectory">The destination directory.</param>
    /// <param name="outputZipFileName">The output zip file name.</param>
    public static void MoveAndCompressDirectory(string sourceDirectory, string destinationDirectory, string outputZipFileName)
    {
        if (!Directory.Exists(destinationDirectory))
        {
            Directory.CreateDirectory(destinationDirectory);
        }

        var zipFilePath = Path.Combine(destinationDirectory, outputZipFileName);
        if (File.Exists(zipFilePath))
        {
            File.Delete(zipFilePath);
        }

        ZipFile.CreateFromDirectory(sourceDirectory, zipFilePath);

        if (File.Exists(zipFilePath))
        {
            Directory.Delete(sourceDirectory, true);
        }
    }

    /// <summary>
    /// Decompresses the files from the specified zip file and deletes the compressed version.
    /// </summary>
    /// <param name="zipFilePath">The path to the zip file.</param>
    /// <param name="destinationDirectory">The destination directory to extract the files.</param>
    /// <param name="deleteZipFile">Whether to delete zip file after.</param>
    public static void MoveAndDecompressFiles(string zipFilePath, string destinationDirectory, bool deleteZipFile = true)
    {
        if (File.Exists(zipFilePath))
        {
            ZipFile.ExtractToDirectory(zipFilePath, destinationDirectory);
            if (deleteZipFile)
            {
                File.Delete(zipFilePath);
            }
        }
        else
        {
            throw new FileNotFoundException($"Zip file does not exist: {zipFilePath}.");
        }
    }

    /// <summary>
    /// Verifies if the application has read and write access to a specified file.
    /// </summary>
    /// <param name="fileName">The name or path of the file to check.</param>
    /// <returns>True if the application has read and write access, otherwise false.</returns>
    public static bool VerifyFileAccess(string fileName)
    {
        try
        {
            // If the file does not exist, assume we have access to create it
            if (!File.Exists(fileName))
            {
                return true;
            }

            // If the file exists, try to open it with write access and sharing read access
            using (new FileStream(fileName, FileMode.Open, FileAccess.Write, FileShare.Read))
            {
                // If successful, we have write access to the file
                return true;
            }
        }
        catch (Exception)
        {
            // If an exception occurs, we don't have access to the file
            return false;
        }
    }
}
