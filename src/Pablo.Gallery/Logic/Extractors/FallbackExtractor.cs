using System;
using System.Threading.Tasks;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace Pablo.Gallery.Logic.Extractors
{
	public class FallbackExtractor : Extractor
	{
		Extractor[] extractors;

		public FallbackExtractor(IEnumerable<Extractor> extractors = null)
		{
			this.extractors = (extractors ?? ExtractorFactory.Extractors).ToArray();
		}

		public override bool Enabled { get { return extractors.Any(r => r.Enabled); } }

		public override bool CanExtractFile(string extension)
		{
			return extractors.Any(r => r.CanExtractFile(extension));
		}

		public override async Task<Stream> ExtractFile(string archiveFileName, string fileName)
		{
			List<Exception> exceptions = null;
			for (int i = 0; i < extractors.Length; i++)
			{
				var extractor = extractors[i];
				if (extractor.Enabled)
				{
					try
					{
						return await extractor.ExtractFile(archiveFileName, fileName);
					}
					catch (Exception ex)
					{
						exceptions = exceptions ?? new List<Exception>();
						exceptions.Add(ex);
					}
				}
			}
			if (exceptions.Count > 0)
				throw new AggregateException("Could not extract file", exceptions);
			throw new InvalidOperationException("No enabled extractors found");
		}

		public override ExtractArchiveInfo ExtractInfo(string archiveFileName)
		{
			List<Exception> exceptions = null;
			for (int i = 0; i < extractors.Length; i++)
			{
				var extractor = extractors[i];
				if (extractor.Enabled && extractor.CanExtractInfo)
				{
					try
					{
						return extractors[i].ExtractInfo(archiveFileName);
					}
					catch (Exception ex)
					{
						exceptions = exceptions ?? new List<Exception>();
						exceptions.Add(ex);
					}
				}
			}
			if (exceptions.Count > 0)
				throw new AggregateException("Could not extract info", exceptions);
			throw new InvalidOperationException("No enabled extractors found");
		}

		public override bool CanExtractInfo
		{
			get
			{
				return extractors.Any(r => r.CanExtractInfo);
			}
		}
	}
}

