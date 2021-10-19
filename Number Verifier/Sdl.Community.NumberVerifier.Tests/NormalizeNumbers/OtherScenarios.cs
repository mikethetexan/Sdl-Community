﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Sdl.Community.NumberVerifier.Interfaces;
using Sdl.Community.NumberVerifier.Tests.Utilities;
using Xunit;

namespace Sdl.Community.NumberVerifier.Tests.NormalizeNumbers
{
	public class OtherScenarios
	{
		private readonly SettingsBuilder _settingsBuilder;

		public OtherScenarios()
		{
			_settingsBuilder = new SettingsBuilder();
		}

		[Theory]
		[InlineData("2400 bis 2483,5", "2400 to 2483.5")]
		public void ShouldGetNoErrors_WhenDecimalSeparatorDifferent_LocalizationAllowed(string source, string target)
		{
			var settings =
				_settingsBuilder
					.AllowLocalization()
					.WithSourceThousandSeparators(false, true)
					.WithTargetThousandSeparators(true, false)
					.WithSourceDecimalSeparators(true, false)
					.WithTargetDecimalSeparators(false, true)
					.Build();

			var numberVerifierMain = new NumberVerifierMain(settings.Object);
			numberVerifierMain.Initialize(null);

			var errorMessage = numberVerifierMain.CheckSourceAndTarget(source, target);

			Assert.True(errorMessage.Count == 0);
		}
	}
}
