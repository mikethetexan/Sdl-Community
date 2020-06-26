﻿using System;
using System.IO;
using System.Linq;
using Sdl.Community.XLIFF.Manager.Common;
using Sdl.Community.XLIFF.Manager.Model;
using Sdl.Core.Settings;
using Sdl.Desktop.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.XLIFF.Manager.BatchTasks
{
	public class ExportSettingsPage : DefaultSettingsPage<ExportSettingsControl, ExportSettings>
	{
		private readonly ProjectsController _projectsController;
		private ExportSettings _settings;
		private ExportSettingsControl _control;

		public ExportSettingsPage()
		{
			_projectsController = GetProjectsController();
		}

		public override object GetControl()
		{
			_settings = ((ISettingsBundle)DataSource).GetSettingsGroup<ExportSettings>();
			_control = base.GetControl() as ExportSettingsControl;
			if (_control != null && _control.ExportOptionsViewModel == null)
			{
				CreateContext();
				_control.Settings = _settings;
				_control.SetDataContext();
			}

			return _control;
		}

		private void CreateContext()
		{
			_settings.DateTimeStamp = DateTime.UtcNow;
			var selectedProject = _projectsController?.SelectedProjects.FirstOrDefault()
								  ?? _projectsController?.CurrentProject;

			if (selectedProject != null)
			{
				var projectInfo = selectedProject.GetProjectInfo();
				_settings.LocalProjectFolder = projectInfo.LocalProjectFolder;
				_settings.TransactionFolder = GetDefaultTransactionPath(_settings.LocalProjectFolder, Enumerators.Action.Export);
			}

			_settings.ExportOptions = _settings.ExportOptions ?? new ExportOptions();
		}

		public override void Save()
		{
			base.Save();
			_settings = _control.Settings;
		}

		public string GetDefaultTransactionPath(string localProjectFolder, Enumerators.Action action)
		{
			var rootPath = Path.Combine(localProjectFolder, "XLIFF.Manager");
			var path = Path.Combine(rootPath, action.ToString());

			if (!Directory.Exists(rootPath))
			{
				Directory.CreateDirectory(rootPath);
			}

			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}

			return path;
		}

		private static ProjectsController GetProjectsController()
		{
			try
			{
				return SdlTradosStudio.Application.GetController<ProjectsController>();
			}
			catch
			{
				// catch all; ignore
			}

			return null;
		}
	}
}
