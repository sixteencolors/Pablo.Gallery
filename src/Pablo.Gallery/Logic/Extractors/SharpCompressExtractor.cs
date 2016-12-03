using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pablo.Gallery.Logic.Extractors
{
	public class SharpCompressExtractor : Extractor
	{
		public override bool CanExtractInfo { get { return true; } }

		public override bool CanExtractFile(string extension)
		{
			return extension == ".zip" || extension == ".7z" || extension == ".rar" || extension == ".tar.gz";
		}

		public override bool Enabled { get { return true; } }

		public override ExtractArchiveInfo ExtractInfo(string archiveFileName)
		{
			using (var archive = SharpCompress.Archives.ArchiveFactory.Open(archiveFileName))
			{
				return new ExtractArchiveInfo
				{
					Size = (int)archive.TotalSize,
					//Comment = archive.Comment,
					Files = ExtractFiles(archive).ToArray()
				};
			}
		}

		IEnumerable<ExtractFileInfo> ExtractFiles(SharpCompress.Archives.IArchive archive)
		{
			int order = 0;

            foreach (var entry in archive.Entries)
			{
				if (entry.IsDirectory)
					continue;
				var currentEntry = entry;
				yield return new ExtractFileInfo
				{
					FileName = entry.Key.Replace('/', '\\'),
					Size = (int)entry.Size,
					Order = order++,
					GetStream = () =>
					{
						var ms = new MemoryStream((int)entry.Size);
						currentEntry.OpenEntryStream().CopyTo(ms);
						ms.Position = 0;
						return ms;
					}
				};
			}
		}

		public override async Task<Stream> ExtractFile(string archiveFileName, string fileName)
		{
			fileName = fileName.Replace('\\', Path.DirectorySeparatorChar); // for unix form
			archiveFileName = Path.Combine(Global.SixteenColorsArchiveLocation, archiveFileName);
			using (var archive = SharpCompress.Archives.ArchiveFactory.Open(archiveFileName))
			{
				var entry = archive.Entries.FirstOrDefault(r => string.Equals(r.Key, fileName, StringComparison.OrdinalIgnoreCase));
				if (entry != null)
				{
					var ms = new MemoryStream((int)entry.Size);
					entry.OpenEntryStream().CopyTo(ms);
					ms.Position = 0;
					return ms;
				}
			}
			return null;
		}
	}
}


