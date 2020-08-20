﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using Sdl.Community.Reports.Viewer.Commands;
using Sdl.Community.Reports.Viewer.Model;
using Sdl.Community.Reports.Viewer.Service;
using Sdl.Core.Globalization;
using Sdl.MultiSelectComboBox.EventArgs;
using Sdl.ProjectAutomation.Core;
using Sdl.Reports.Viewer.API.Model;
using MessageBox = System.Windows.MessageBox;

namespace Sdl.Community.Reports.Viewer.ViewModel
{
	public class AppendReportViewModel : INotifyPropertyChanged
	{
		private readonly Window _window;
		private readonly Settings _settings;
		private readonly PathInfo _pathInfo;
		private readonly ImageService _imageService;
		private readonly IProject _project;
		private string _windowTitle;
		private ICommand _saveCommand;
		private ICommand _selectedItemsChangedCommand;
		private ICommand _clearPathCommand;
		private ICommand _browseFolderCommand;
		private string _name;
		private string _description;
		private string _groupName;
		private List<LanguageItem> _languageItems;
		private List<LanguageItem> _selectedLanguageItems;
		private DateTime _date;
		private string _path;
		private string _xslt;
		private bool _isEditMode;
		private bool _canEditLanguages;
		private bool _canEditGroups;

		public AppendReportViewModel(Window window, Report report, Settings settings,
			PathInfo pathInfo, ImageService imageService, IProject project, bool isEditMode = false)
		{
			_window = window;
			Report = report;
			_settings = settings;
			_pathInfo = pathInfo;
			_imageService = imageService;
			_project = project;

			IsEditMode = isEditMode;

			WindowTitle = IsEditMode ? "Edit Project Report Information" : "Add Project Report";

			var projectInfo = _project.GetProjectInfo();

			var allLanguages = new List<Language> { projectInfo.SourceLanguage };
			allLanguages.AddRange(projectInfo.TargetLanguages);

			LanguageItems = allLanguages
				.Select(language => new LanguageItem
				{
					Name = language.DisplayName,
					CultureInfo = language.CultureInfo,
					Image = imageService.GetImage(language.CultureInfo.Name)
				})
				.ToList();

			SelectedLanguageItems = new List<LanguageItem> {
				LanguageItems.FirstOrDefault(a=> string.Compare(a.CultureInfo.Name, Report.Language, StringComparison.CurrentCultureIgnoreCase)==0) };

			Date = Report.Date;
			Name = Report.Name;
			GroupName = Report.Group;
			Description = Report.Description;
			Path = Report.Path;
			if (report is ReportWithXslt reportWithXslt)
			{
				Xslt = reportWithXslt.Xslt;
			}

			CanEditLanguages = !report.IsStudioReport;
			CanEditGroups = !report.IsStudioReport;
		}

		public ICommand SaveCommand => _saveCommand ?? (_saveCommand = new CommandHandler(SaveChanges));

		public ICommand SelectedItemsChangedCommand => _selectedItemsChangedCommand ?? (_selectedItemsChangedCommand = new CommandHandler(SelectedItemsChanged));

		public ICommand ClearPathCommand => _clearPathCommand ?? (_clearPathCommand = new CommandHandler(ClearPath));

		public ICommand BrowseFolderCommand => _browseFolderCommand ?? (_browseFolderCommand = new CommandHandler(BrowseFolder));

		public string WindowTitle
		{
			get => _windowTitle;
			set
			{
				_windowTitle = value;
				OnPropertyChanged(nameof(WindowTitle));
			}
		}

		public Report Report { get; }

		public string Name
		{
			get => _name;
			set
			{
				if (_name == value)
				{
					return;
				}

				_name = value;
				OnPropertyChanged(nameof(Name));
				OnPropertyChanged(nameof(IsValid));
			}
		}

		public bool CanEditLanguages
		{
			get => _canEditLanguages;
			set
			{
				if (_canEditLanguages == value)
				{
					return;
				}

				_canEditLanguages = value;
				OnPropertyChanged(nameof(CanEditLanguages));
			}
		}

		public bool CanEditGroups
		{
			get => _canEditGroups;
			set
			{
				if (_canEditGroups == value)
				{
					return;
				}

				_canEditGroups = value;
				OnPropertyChanged(nameof(CanEditGroups));
			}
		}

		public bool IsEditMode
		{
			get => _isEditMode;
			set
			{
				if (_isEditMode == value)
				{
					return;
				}

				_isEditMode = value;
				OnPropertyChanged(nameof(IsEditMode));
			}
		}

		public string Description
		{
			get => _description;
			set
			{
				if (_description == value)
				{
					return;
				}

				_description = value;
				OnPropertyChanged(nameof(Description));
				OnPropertyChanged(nameof(IsValid));
			}
		}

		public string GroupName
		{
			get => _groupName;
			set
			{
				if (_groupName == value)
				{
					return;
				}

				_groupName = value;
				OnPropertyChanged(nameof(GroupName));
				OnPropertyChanged(nameof(IsValid));
			}
		}

		public List<LanguageItem> LanguageItems
		{
			get => _languageItems;
			set
			{
				_languageItems = value;
				OnPropertyChanged(nameof(LanguageItems));
			}
		}

		public List<LanguageItem> SelectedLanguageItems
		{
			get => _selectedLanguageItems;
			set
			{
				_selectedLanguageItems = value;
				OnPropertyChanged(nameof(SelectedLanguageItems));
			}
		}

		public DateTime Date
		{
			get => _date;
			set
			{
				if (_date == value)
				{
					return;
				}

				_date = value;
				OnPropertyChanged(nameof(Date));
			}
		}

		public string Path
		{
			get => _path;
			set
			{
				if (_path == value)
				{
					return;
				}

				_path = value;
				OnPropertyChanged(nameof(Path));


				if (File.Exists(Path) && string.IsNullOrEmpty(Name))
				{
					Name = System.IO.Path.GetFileName(Path);
					while (!string.IsNullOrEmpty(System.IO.Path.GetExtension(Name)))
					{
						Name = Name?.Substring(0, Name.Length - System.IO.Path.GetExtension(Name).Length);
					}
				}

				OnPropertyChanged(nameof(IsValid));
			}
		}

		public string Xslt
		{
			get => _xslt;
			set
			{
				if (_xslt == value)
				{
					return;
				}

				_xslt = value;
				OnPropertyChanged(nameof(Xslt));
				OnPropertyChanged(nameof(IsValid));
			}
		}

		public bool IsValid
		{
			get
			{
				if (!IsEditMode)
				{
					if (string.IsNullOrEmpty(Path) || !File.Exists(Path))
					{
						return false;
					}

					if (!string.IsNullOrEmpty(Xslt) && !File.Exists(Xslt))
					{
						return false;
					}
				}

				if (string.IsNullOrEmpty(Name))
				{
					return false;
				}

				return true;
			}
		}

		private void SaveChanges(object parameter)
		{
			if (IsValid)
			{				
				if (Path.ToLower().EndsWith(".xml") && !string.IsNullOrEmpty(Xslt) && File.Exists(Xslt))
				{
					var htmlPath = Path + ".html";
					var result = TransformXmlReport(Path, Xslt, htmlPath);
					if (result)
					{
						Path = htmlPath;
					}
					else
					{
						return;
					}
				}


				Report.Name = Name;
				Report.Group = GroupName;
				Report.Description = Description;
				Report.Path = Path;				
				Report.Language = SelectedLanguageItems?.FirstOrDefault()?.CultureInfo?.Name ?? string.Empty;

				_window.DialogResult = true;
				_window?.Close();
			}
		}

		private bool TransformXmlReport(string xml, string xslt, string output)
		{
			try
			{
				var xsltSetting = new XsltSettings
				{
					EnableDocumentFunction = true,
					EnableScript = true
				};

				var myXPathDoc = new XPathDocument(xml);

				var myXslTrans = new XslCompiledTransform();
				myXslTrans.Load(xslt, xsltSetting, null);

				var myWriter = new XmlTextWriter(output, Encoding.UTF8);

				myXslTrans.Transform(myXPathDoc, null, myWriter);
				myWriter.Flush();
				myWriter.Close();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
				return false;
			}

			return true;
		}

		private void SelectedItemsChanged(object parameter)
		{
			if (parameter is SelectedItemsChangedEventArgs)
			{
				OnPropertyChanged(nameof(SelectedLanguageItems));
			}
		}

		private void ClearPath(object parameter)
		{
			if (parameter.ToString() == "path")
			{
				Path = string.Empty;
			}
			else
			{
				Xslt = string.Empty;
			}
		}

		private void BrowseFolder(object parameter)
		{
			var fileType = parameter.ToString() == "path" ? "data" : "xslt template";

			var openFileDialog = new OpenFileDialog();
			openFileDialog.Multiselect = false;
			openFileDialog.Title = string.Format("Select the {0} file", fileType);
			openFileDialog.InitialDirectory = _project.GetProjectInfo().LocalProjectFolder;
			openFileDialog.Filter = fileType == "HTML"
				? "All supported files (*.html;*.htm;*.xml)|*.html;*.htm;*.xml|HTML files(*.html;*.htm)|*.html;*.htm|XML files(*.xml)|*.xml"
				: "XSLT files(*.xslt)| *.xslt;*.xsl";

			var result = openFileDialog.ShowDialog();
			if (result == DialogResult.OK)
			{
				if (fileType == "HTML")
				{
					Path = openFileDialog.FileName;
				}
				else
				{
					Xslt = openFileDialog.FileName;
				}
			}
		}

		private string GetValidFolderPath(string directory)
		{
			if (string.IsNullOrWhiteSpace(directory))
			{
				return string.Empty;
			}

			var folder = directory;
			if (Directory.Exists(folder))
			{
				return folder;
			}

			while (folder.Contains("\\"))
			{
				folder = folder.Substring(0, folder.LastIndexOf("\\", StringComparison.Ordinal));
				if (Directory.Exists(folder))
				{
					return folder;
				}
			}

			return folder;
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
