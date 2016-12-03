using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Pablo.Gallery.Models;
using System.Text;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.File;
using RedDog.Storage.Files;
using Exceptionless;

namespace Pablo.Gallery.Logic
{
	public class Scanner
	{
		public static string NormalizedPath(string path)
		{
			return path.Replace(@"/", @"\");
		}

		public static string NativePath(string path)
		{
			return path.Replace(@"\", Path.DirectorySeparatorChar.ToString());
		}

		public void ScanPacks(Action<string> updateStatus, bool onlyNew)
		{
			var startTime = DateTime.Now;
			updateStatus(string.Format("Scanning began {0:g}", startTime));
            updateStatus(Global.SixteenColorsStorageConnectionString);
            List<string> dirs = new List<string>();

            if (!string.IsNullOrEmpty(Global.SixteenColorsStorageConnectionString)) {
                //CloudFileShare share = CloudStorageAccount.Parse(Global.SixteenColorsStorageConnectionString)
                //                .CreateCloudFileClient()
                //                .GetShareReference("sixteencolors-archive");
                //ExceptionlessClient.Default.SubmitLog(string.Format("Share Exists: {0}; root: {1}", share.Exists(), share.GetRootDirectoryReference().Name), Exceptionless.Logging.LogLevel.Info);

                //share.Mount("S:");
                var account = CloudStorageAccount.Parse(Global.SixteenColorsStorageConnectionString);
                var share = account.CreateCloudFileClient().GetShareReference("sixteencolors-archive");
                if (share.Exists()) {
                    var filesAndDirectories = share.GetRootDirectoryReference().ListFilesAndDirectories();
                    foreach(var fileOrDirectory in filesAndDirectories) {
                        if (fileOrDirectory.GetType() == typeof(CloudFileDirectory))
                            dirs.Add(((CloudFileDirectory)fileOrDirectory).Name);
                        else if (fileOrDirectory.GetType() == typeof(CloudFile)) {
                            CloudFile file = (CloudFile)fileOrDirectory;
                            var year = file.Parent.Name;
                            updateStatus(string.Format("{0}/{1}", year, file.Name));
                        }
                    }
                }
                
            }
            
            dirs = Directory.EnumerateDirectories(Global.SixteenColorsArchiveLocation).OrderByDescending(r => r).ToList();

			foreach (var dir in dirs)
			{
				var idx = dir.LastIndexOf(Path.DirectorySeparatorChar);
				if (idx < 0)
					continue;
				var yearString = dir.Substring(idx + 1);
				int year;
				if (int.TryParse(yearString, out year))
				{
					var packNames = Directory.EnumerateFiles(dir);
					//packNames = packNames.SkipWhile(r => !Path.GetFileName(r).StartsWith("blde9612", StringComparison.InvariantCultureIgnoreCase));
					foreach (var packFileEntry in packNames)
					{
						var packFile = NormalizedPath(packFileEntry);
						var packFileName = Path.Combine(Global.SixteenColorsArchiveLocation, packFileEntry);
						var extractor = Extractors.ExtractorFactory.GetInfoExtractor(packFileName);
						if (extractor == null)
							continue;
						DateTime? date;
						var match = Regex.Match(packFile, @"^(.+?)(?<month>\d\d)(?<year>\d\d)[.](\w+)$", RegexOptions.ExplicitCapture);
						if (match.Success)
						{
							var monthString = match.Groups["month"].Value;
							int month;
							if (int.TryParse(monthString, out month) && month >= 1 && month <= 12)
							{
								date = new DateTime(year, month, 1);
							}
							else
								date = new DateTime(year, 1, 1);
						}
						else
							date = new DateTime(year, 1, 1);


						var packShortFile = packFile.Substring(Global.SixteenColorsArchiveLocation.Length).TrimStart('\\');
						using (var db = new GalleryContext())
						{
							var pack = db.Packs.FirstOrDefault(r => r.FileName.ToLower() == packShortFile.ToLower());
							try
							{
								if (pack == null)
								{
									updateStatus(string.Format("Adding pack {0}", packShortFile));
									var name = Path.GetFileNameWithoutExtension(packFileEntry);
									if (db.Packs.Any(p => p.Name == name))
									{
										updateStatus(string.Format("Error adding pack '{0}', a pack with the same name already exists",
											packShortFile.ToLower()));
										continue;
									}
									pack = new Pack
									{
										Name = CanonicalName(Path.GetFileNameWithoutExtension(packFileEntry)),
										FileName = packShortFile,
										Date = date
									};
									db.Packs.Add(pack);

									db.SaveChanges();
								}
								else if (onlyNew)
								{
									updateStatus(string.Format("Skipping pack {0}", packShortFile));
									continue;
								}
								else
								{
									updateStatus(string.Format("Updating pack {0}", packShortFile));
									// fixup existing data
									pack.Name = CanonicalName(pack.Name);
								}
								try
								{
									var archiveInfo = extractor.ExtractInfo(packFileName);
									//pack.ArchiveComment = archiveInfo.Comment;
									var files = archiveInfo.Files.ToArray();
									foreach (var fileInfo in files)
									{
										var fileInfo1 = fileInfo;
										try
										{
											ExtractFileInfo(db, pack, fileInfo, () => GetStream(packFileName, fileInfo1));
										}
										catch (Exception ex)
										{
											updateStatus(string.Format("Error extracting file '{0}', {1}", fileInfo.FileName, ex));
										}
									}
									var fileNames = files.Select(r => Scanner.NormalizedPath(r.FileName).TrimStart('\\')).ToArray();
									foreach (var file in pack.Files.Where(r => !fileNames.Contains(r.FileName)).ToList())
									{
										db.Files.Remove(file);
									}
								}
								catch (Exception ex)
								{
									updateStatus(string.Format("Error extracting pack '{0}', {1}", pack.FileName, ex));
								}

								if ( /*pack.Thumbnail == null &&*/ pack.Files != null)
								{
									pack.Thumbnail = pack.Files.FirstOrDefault(r => r.FileName.ToLowerInvariant() == "file_id.diz");
									if (pack.Thumbnail == null)
										pack.Thumbnail = pack.Files.FirstOrDefault(r => Path.GetExtension(r.FileName).ToLowerInvariant() == ".diz");
									if (pack.Thumbnail == null)
										pack.Thumbnail = pack.Files.FirstOrDefault(r => Path.GetExtension(r.FileName).ToLowerInvariant() == ".nfo");
									if (pack.Thumbnail == null)
										pack.Thumbnail =
											pack.Files.FirstOrDefault(
												r => Path.GetFileNameWithoutExtension(r.FileName).ToLowerInvariant().Contains("info"));
									if (pack.Thumbnail == null)
										pack.Thumbnail = pack.Files.OrderBy(r => r.Order).FirstOrDefault(r => r.Type != null);
								}
								db.SaveChanges();

							}
							catch (Exception ex)
							{
								updateStatus(string.Format("Error saving changes to '{0}', {1}", pack.FileName, ex));
								db.Entry(db.Packs).Reload();
								db.Entry(db.Files).Reload();
							}
						}
					}
				}
			}
			var endTime = DateTime.Now;
			var elapsed = endTime - startTime;
			updateStatus(string.Format("Scanning ended {0:g} ({1:hh\\:mm\\:ss})", endTime, elapsed));
		}

		Stream GetStream(string archiveFileName, Extractors.ExtractFileInfo fileInfo)
		{
			try
			{
				return fileInfo.GetStream();
			}
			catch
			{
				var extractor = Extractors.ExtractorFactory.GetFileExtractor(archiveFileName);
				var task = extractor.ExtractFile(archiveFileName, fileInfo.FileName);
				task.Wait();
				//if (task.Exception != null)
				//	throw task.Exception;
				return task.Result;
			}
		}

		Models.File ExtractFileInfo(GalleryContext db, Pack pack, Extractors.ExtractFileInfo fileInfo, Func<Stream> getStream)
		{
			var fileName = Scanner.NormalizedPath(fileInfo.FileName).TrimStart('\\');
			var file = pack.GetFileByName(fileName);
			var name = CanonicalName(fileName.Replace("\\", "/"));
			while (pack.Files.Any(r => r.Name == name && !ReferenceEquals(r, file)))
			{
				name += "_";
			}
			file.Name = name;
			file.Order = fileInfo.Order++;
			//updateStatus(string.Format("Processing file '{0}'", file.FileName));
			var format = FileFormat.FindByExtension(Path.GetExtension(file.NativeFileName));
			if (format != null)
			{
				file.Format = format.Name;
				file.Type = format.Type.Name;
			}
			else
			{
				file.Format = "ansi";
				file.Type = FileType.Character.Name;
			}

			var infoParameters = new PabloDraw.InputParameters
			{
				InputFormat = file.Format,
				InputFileName = file.FileName
			};

			if (Global.PabloEngine.SupportsFile(infoParameters))
			{
				using (var stream = getStream())
				{
					if (stream != null)
					{
						if (file.Type == FileType.Character.Name)
						{
							if (Convert.ToBoolean(ConfigurationManager.AppSettings["EnableContentSave"]) && file.Content == null)
							{
								using (var outStream = new MemoryStream())
								{
									var parameters = new PabloDraw.ConvertParameters
									{
										InputStream = stream,
										InputFormat = file.Format,
										InputFileName = file.FileName,
										OutputFormat = "ascii",
										OutputStream = outStream
									};

									Global.PabloEngine.Convert(parameters);
									outStream.Position = 0;
									using (var reader = new StreamReader(outStream, Encoding.GetEncoding(437)))
									{
										var content = file.Content ?? (file.Content = new FileContent { File = file });
										content.Text = reader.ReadToEnd().Replace((char)0, ' ');
									}
									stream.Position = 0;
								}
							}
						}
						else if (file.Content != null)
							file.Content = null;

						infoParameters.InputStream = stream;
						var info = Global.PabloEngine.GetInfo(infoParameters);
						file.Width = info.ImageWidth;
						file.Height = info.ImageHeight;
					}
				}
			}

			return file;
		}

		static string CanonicalName(string name)
		{
			return Regex.Replace(name, @"(?![%]\d\d)([^a-zA-Z0-9/\-._~!$&'()*,;=:@])", "_", RegexOptions.Compiled); // excluded, but valid: +
		}
	}
}