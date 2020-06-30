﻿using System.Collections.Generic;
using Sdl.Community.XLIFF.Manager.Model;
using Sdl.Core.Globalization;
using Sdl.FileTypeSupport.Framework.Core.Utilities.NativeApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Sdl.MultiSelectComboBox.API;

namespace Sdl.Community.XLIFF.Manager.Common
{
	public class Enumerators
	{
		public enum Action
		{
			None = 0,
			Export = 1,
			Import = 2
		}

		public enum CountType
		{
			Segments,
			Words,
			Characters
		}

		public enum Status
		{
			None = 0,
			Ready = 1,
			Success = 2,
			Error = 3,
			Warning
		}

		public enum MatchType
		{
			None,
			New,
			Repetition,
			Fuzzy,
			MT,
			AMT,
			NMT,
			Exact,
			CM,
			PM
		}

		public enum XLIFFSupport
		{
			none = 0,
			xliff12sdl = 1,
			xliff12polyglot = 2
			// TODO spport for this format will come later on in the development cycle
			//xliff20sdl = 2
		}

		public static string GetTranslationOriginType(ITranslationOrigin translationOrigin, List<AnalysisBand> analysisBands)
		{
			if (translationOrigin != null)
			{
				if (translationOrigin.OriginType == DefaultTranslationOrigin.AutoPropagated)
				{
					return MatchType.Repetition.ToString();
				}

				if (translationOrigin.OriginType == DefaultTranslationOrigin.MachineTranslation)
				{
					return MatchType.MT.ToString();
				}

				if (translationOrigin.OriginType == DefaultTranslationOrigin.AdaptiveMachineTranslation)
				{
					return MatchType.AMT.ToString();
				}

				if (translationOrigin.OriginType == DefaultTranslationOrigin.NeuralMachineTranslation)
				{
					return MatchType.NMT.ToString();
				}

				if (translationOrigin.MatchPercent >= 100)
				{
					if (translationOrigin.OriginType == DefaultTranslationOrigin.DocumentMatch)
					{
						return MatchType.PM.ToString();
					}

					if (translationOrigin.TextContextMatchLevel == TextContextMatchLevel.SourceAndTarget)
					{
						return MatchType.CM.ToString();
					}

					return MatchType.Exact.ToString();
				}

				if (translationOrigin.MatchPercent > 0)
				{
					foreach (var analysisBand in analysisBands)
					{
						if (translationOrigin.MatchPercent >= analysisBand.MinimumMatchValue &&
							translationOrigin.MatchPercent <= analysisBand.MaximumMatchValue)
						{
							return MatchType.Fuzzy + string.Format(" {0} - {1}",
									   analysisBand.MinimumMatchValue + "%", analysisBand.MaximumMatchValue + "%");
						}
					}
				}
			}

			return MatchType.New.ToString();
		}

		public static List<ConfirmationStatus> GetConfirmationStatuses()
		{
			var confirmationStatuses = new List<ConfirmationStatus>
			{
				new ConfirmationStatus
				{
					Id = string.Empty,
					Name = "Don't Change"
				},
				new ConfirmationStatus
				{
					Id = ConfirmationLevel.Unspecified.ToString(),
					Name = "Unspecified"
				},
				new ConfirmationStatus
				{
					Id = ConfirmationLevel.Draft.ToString(),
					Name = "Draft"
				},
				new ConfirmationStatus
				{
					Id = ConfirmationLevel.Translated.ToString(),
					Name = "Translated"
				},
				new ConfirmationStatus
				{
					Id = ConfirmationLevel.RejectedTranslation.ToString(),
					Name = "Translation Rejected"
				},
				new ConfirmationStatus
				{
					Id = ConfirmationLevel.ApprovedTranslation.ToString(),
					Name = "Translation Approved"
				},
				new ConfirmationStatus
				{
					Id = ConfirmationLevel.RejectedSignOff.ToString(),
					Name = "SignOff Rejected"
				},
				new ConfirmationStatus
				{
					Id = ConfirmationLevel.ApprovedSignOff.ToString(),
					Name = "SignOff Approved"
				}
			};

			return confirmationStatuses;
		}

		public static List<FilterItem> GetFilterItems()
		{
			var filterItems = new List<FilterItem>();

			AddSegmentPropertyFilters(filterItems, new FilterItemGroup(1, "Properties"));
			AddSegmentStatusFilters(filterItems, new FilterItemGroup(2, "Status"));
			AddMatchTypeFilters(filterItems, new FilterItemGroup(3, "Match"));

			return filterItems;
		}

		private static void AddMatchTypeFilters(ICollection<FilterItem> filterItems, IItemGroup filterItemGroup)
		{
			filterItems.Add(new FilterItem
			{
				Group = filterItemGroup,
				Id = "PM",
				Name = "Perfect Match"
			});

			filterItems.Add(new FilterItem
			{
				Group = filterItemGroup,
				Id = "CM",
				Name = "Context Match"
			});

			filterItems.Add(new FilterItem
			{
				Group = filterItemGroup,
				Id = "Exact",
				Name = "Exact Match"
			});

			filterItems.Add(new FilterItem
			{
				Group = filterItemGroup,
				Id = "MT",
				Name = "Machine Translation"
			});

			filterItems.Add(new FilterItem
			{
				Group = filterItemGroup,
				Id = "AMT",
				Name = "Adaptive Machine Translation"
			});

			filterItems.Add(new FilterItem
			{
				Group = filterItemGroup,
				Id = "NMT",
				Name = "Neural Machine Translation"
			});

			filterItems.Add(new FilterItem
			{
				Group = filterItemGroup,
				Id = "Fuzzy",
				Name = "Fuzzy Match"
			});

			filterItems.Add(new FilterItem
			{
				Group = filterItemGroup,
				Id = "New",
				Name = "New"
			});
		}

		private static void AddSegmentStatusFilters(ICollection<FilterItem> filterItems, IItemGroup filterItemGroup)
		{
			filterItems.Add(new FilterItem
			{
				Group = filterItemGroup,
				Id = ConfirmationLevel.Unspecified.ToString(),
				Name = "Unspecified"
			});

			filterItems.Add(new FilterItem
			{
				Group = filterItemGroup,
				Id = ConfirmationLevel.Draft.ToString(),
				Name = "Draft"
			});

			filterItems.Add(new FilterItem
			{
				Group = filterItemGroup,
				Id = ConfirmationLevel.Translated.ToString(),
				Name = "Translated"
			});

			filterItems.Add(new FilterItem
			{
				Group = filterItemGroup,
				Id = ConfirmationLevel.RejectedTranslation.ToString(),
				Name = "Translation Rejected"
			});

			filterItems.Add(new FilterItem
			{
				Group = filterItemGroup,
				Id = ConfirmationLevel.ApprovedTranslation.ToString(),
				Name = "Translation Approved"
			});

			filterItems.Add(new FilterItem
			{
				Group = filterItemGroup,
				Id = ConfirmationLevel.RejectedSignOff.ToString(),
				Name = "SignOff Rejected"
			});

			filterItems.Add(new FilterItem
			{
				Group = filterItemGroup,
				Id = ConfirmationLevel.ApprovedSignOff.ToString(),
				Name = "SignOff Approved"
			});
		}

		private static void AddSegmentPropertyFilters(ICollection<FilterItem> filterItems, IItemGroup filterItemGroup)
		{
			filterItems.Add(new FilterItem
			{
				Group = filterItemGroup,
				Id = "Locked",
				Name = "Locked"
			});
		}
	}
}
