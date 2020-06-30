﻿using System;
using System.Collections.Generic;

namespace Sdl.Community.XLIFF.Manager.Model
{
	public class WordCounts: ICloneable
	{
		public WordCounts()
		{
			Processed = new List<WordCount>();
			Ignored = new List<WordCount>();
		}		

		public List<WordCount> Processed { get; set; }

		public List<WordCount> Ignored { get; set; }
				
		public object Clone()
		{
			var wordCounts = new WordCounts();
			
			wordCounts.Processed = new List<WordCount>();
			foreach (var wordCount in Processed)
			{
				wordCounts.Processed.Add(wordCount.Clone() as WordCount);
			}

			var ignored = new WordCounts();
			ignored.Ignored = new List<WordCount>();
			foreach (var wordCount in Ignored)
			{
				ignored.Ignored.Add(wordCount.Clone() as WordCount);
			}


			return wordCounts;
		}
	}
}
