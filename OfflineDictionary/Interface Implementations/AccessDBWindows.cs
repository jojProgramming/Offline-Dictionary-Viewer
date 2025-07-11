using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Runtime.Versioning;
using System.Security;
using System.Threading.Tasks;
using Avalonia.Controls;
using Microsoft.Win32;
using OfflineDictionary.Interfaces;
using OfflineDictionary.Models;

/*
    OfflineDictionary is an offline dictionary viewing application.
    Copyright (C) 2025  JOJProgramming

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
 */

namespace OfflineDictionary.Interface_Implementations;
[SupportedOSPlatform("Windows")]
public class AccessDbWindows : AccessDb
// Provide the ability to change the selected database, and automatically loads the database if a database is available already.
{
	private IDBRequestHandler? _sqliteRequestHandler;
	private const string RegistryKeyPath = @"SOFTWARE\JOJProgramming\Dictionary\";
	private string? _databasePath;
	private readonly TopLevel _topLevel;

	public AccessDbWindows(TopLevel topLevel) : base(topLevel)
	{

		_topLevel = topLevel;
		string regValueName = "DBPath";

		try
		{
			RegistryKey key = Registry.CurrentUser.CreateSubKey(RegistryKeyPath);

			if (key.GetValue(regValueName) is string and not null)
			{
				_databasePath = key.GetValue(regValueName).ToString();
			}
		}
		catch (SecurityException ex)
		{
			App.PopupTools.PopupConfirmGeneric("Security Issue",
				"Your user account lacks the permissions required to " +
				"operate this application (registry read/write).");
		}
		catch (UnauthorizedAccessException ex)
		{
			App.PopupTools.PopupConfirmGeneric("Permissions Issue",
				"The application was refused access to the system registry, " +
				"which is required for the functionality of the application.");
		}
		catch (Exception ex)
		{
			App.PopupTools.PopupConfirmGeneric("Error Occured: " + ex.GetType().ToString(), ex.Message);
		}
		finally
		{
			_sqliteRequestHandler = (_databasePath != null) ? new SqliteRequestHandler(_databasePath) : null;
		}
	}

	public override async Task<RequestContainer> SearchFor(string word)
	{
		if (_sqliteRequestHandler == null)
		{
			App.PopupTools.PopupConfirmGeneric("No database.","Please select a valid database.", false);
			return new RequestContainer("NO DATABASE SELECTED");
		}
		return await _sqliteRequestHandler.SearchFor(word);
	}

	public override async Task<List<RequestContainer>> SearchLike(string word)
	{
		if (_databasePath == null)
		{
			App.PopupTools.PopupConfirmGeneric("No database.","Please select a valid database.", false);
			return new List<RequestContainer>();
		}
		return await _sqliteRequestHandler.SearchLike(word);
	}

	public override async void ChangeUnderlyingDB()
	{
		try
		{
			string regValueName = "DBPath";
			RegistryKey key = Registry.CurrentUser.CreateSubKey(RegistryKeyPath);

			FilePicker fileAccess = new FilePicker(_topLevel);
			string? newDictionaryLocation = await fileAccess.GetFilePath();

			if (newDictionaryLocation == null)
			{
				App.PopupTools.PopupConfirmGeneric("No database.", "Please select a valid database.", false);
				return;
			}

			key.SetValue(regValueName, newDictionaryLocation, RegistryValueKind.String);
			_databasePath = newDictionaryLocation;
			_sqliteRequestHandler = (_databasePath != null) ? new SqliteRequestHandler(_databasePath) : null;

		}
		catch (SecurityException ex)
		{
			App.PopupTools.PopupConfirmGeneric("Security Issue",
				"Your user account lacks the permissions required to " +
				"operate this application (registry read/write OR file location read/write).");
		}
		catch (UnauthorizedAccessException ex)
		{
			App.PopupTools.PopupConfirmGeneric("Permissions Issue",
				"The application was refused access to the registry or the selected file, " +
				"which is required for the functionality of the application.");
		}
		catch (Exception ex)
		{
			App.PopupTools.PopupConfirmGeneric("Error Occured: " + ex.GetType().ToString(), ex.Message);
		}
	}

	public override bool IsReady()
	{
		if (_databasePath == null)
		{
			App.PopupTools.PopupConfirmGeneric("No database.", "Please select a valid database.", false);
			return false;
		}

		return true;
	}
}