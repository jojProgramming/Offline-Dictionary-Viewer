using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using OfflineDictionary.Interface_Implementations;
using OfflineDictionary.Interfaces;

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

namespace OfflineDictionary.Models;

public class RequestContainer
{
	public RequestContainer(string word)
	{
		Word = word;
	}

	public string Word { get; }
	public ObservableCollection<string> AlternateForms { get; } = new();
	public ObservableCollection<string> Definitions { get; } = new();

	public void AddAlternateForm(string word)
	{
		if (AlternateForms.Contains(word)) return;
		AlternateForms.Add(word);
	}

	public void AddDefinition(string word)
	{
		if (Definitions.Contains(word)) return;
		Definitions.Add(word);
	}

	public ProcessedRequestContainer Process() // Process to allow easy display in the UI / export to other program elements
	{
		ITextTools textTools = new TextTools();
		string alternateFormString = "";
		string definitionString = "";
		foreach (string altform in AlternateForms)
		{
			alternateFormString += textTools.ToSentenceCase(altform) + ", ";
		}
		if (alternateFormString.Length > 2 ) alternateFormString = alternateFormString.Remove(alternateFormString.Length - 2);

		int count = 0;
		foreach (string definition in Definitions)
		{
			definitionString += $"[{count.ToString()}] " + textTools.ToSentenceCase(definition) + "\n";
			count++;
		}

		if (definitionString.Length > 2 ) definitionString = definitionString.Remove(definitionString.Length - 2);

		return new ProcessedRequestContainer(Word, alternateFormString, definitionString);
	}

}
public class ProcessedRequestContainer(string word, string alternateForms, string definitions)
{
	public string Word { get; } = word;
	public string AlternateForms { get; } = alternateForms;
	public string Definitions { get; } = definitions;
}