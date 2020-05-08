﻿using System.ComponentModel;
using System.Xml;
using Sdl.Community.ExportAnalysisReports.Model;

namespace Sdl.Community.ExportAnalysisReports.Interfaces
{
	public interface IReportService
	{
		string JsonPath { get; }

		string ReportsFolderPath { get; set; }

		bool GenerateReportFile(BindingList<ProjectDetails> projects, OptionalInformation optionalInformation, string reportOutputPath, bool isChecked);

		string GetJsonReportPath(string jsonPath);

		bool IsSameReportPath(string reportOutputPath);

		bool ReportFileExist(string reportFolderPath);

		bool ReportFolderExist(XmlNode projectInfoNode, string projectXmlPath);

		void SaveExportPath(string reportOutputPath);

		void SetReportInformation(XmlDocument doc, ProjectDetails project);
	}
}