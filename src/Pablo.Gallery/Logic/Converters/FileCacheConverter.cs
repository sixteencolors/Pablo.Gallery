using System;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Azure; 
using Microsoft.WindowsAzure.Storage; 
using Microsoft.WindowsAzure.Storage.File; 

namespace Pablo.Gallery.Logic.Converters
{
	public abstract class FileCacheConverter : Converter
	{
		public abstract Task ConvertFile(ConvertInfo info, string inFile, string outFile);

		public override async Task<Stream> Convert(ConvertInfo info)
		{
            if (!string.IsNullOrEmpty(Global.SixteenColorsStorageConnectionString)) {
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(Global.SixteenColorsStorageConnectionString);

                // Create a CloudFileClient object for credentialed access to File storage.
                CloudFileClient fileClient = storageAccount.CreateCloudFileClient();

                // Get a reference to the file share we created previously.
                CloudFileShare share = fileClient.GetShareReference(Global.SixteenColorsArchiveLocation);

                // Ensure that the share exists.
                if (share.Exists()) {
                    // Get a reference to the root directory for the share.
                    CloudFileDirectory cloudPackDir = share.GetRootDirectoryReference();

                    // Ensure that the directory exists.
                    if (cloudPackDir.Exists()) {
                        // Get a reference to the file we created previously.
                        CloudFile file = cloudPackDir.GetFileReference("Log1.txt");

                        // Ensure that the file exists.
                        if (file.Exists()) {
                            // Write the contents of the file to the console window.
                            Console.WriteLine(file.DownloadTextAsync().Result);
                        }
                    }
                }
            }

			var packDir = Path.Combine(Global.SixteenColorsCacheLocation, info.Pack.Name);

			var outFileName = Path.Combine(packDir, info.OutFileName);
			if (!File.Exists(outFileName))
			{
				// save raw input file to cache
				var inFileName = Path.Combine(packDir, info.FileName);
				Directory.CreateDirectory(Path.GetDirectoryName(inFileName));
				if (!File.Exists(inFileName))
				{
					await info.ExtractFile(inFileName);

					#if DEBUG
					if (!File.Exists(inFileName))
					{
						throw new FileNotFoundException("File was not extracted", inFileName);
					}
					#endif
				}

				await ConvertFile(info, inFileName, outFileName);
				if (File.Exists(outFileName))
				{
					return File.OpenRead(outFileName);
				}
			}
			else
			{
				return File.OpenRead(outFileName);
			}
			return null;
		}
	}
}

