﻿using System.Collections.Generic;

namespace Sdl.LC.AddonBlueprint.Models
{
	public class TranslationEngineRequest
	{
		public string Model { get; set; }
		public string SourceLanguage { get; set; }
		public List<string> TargetLanguage { get; set; }
		public bool IncludeGlossaries { get; set; }
		public bool ExactMatch { get; set; }
	}
}
