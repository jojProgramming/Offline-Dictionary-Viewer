using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
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

public class SqliteRequestHandlerCached : DBRequestHandler
{
	private string _connectionString;
	private SqliteConnection _connection;

	public SqliteRequestHandlerCached(string dbUri) : base(dbUri)
	{
		// Create an in memory database and open a connection to it
		_connectionString = new SqliteConnectionStringBuilder()
		{
			DataSource = ":memory:",
			Mode = SqliteOpenMode.Memory,
			Cache=SqliteCacheMode.Shared
		}.ToString();
		_connection = new SqliteConnection(_connectionString);
		_connection.Open();

		// Clone the data from the file based dictionary
		using (SqliteConnection sqliteConnection = new SqliteConnection("Data Source=" + dbUri + ";"))
		{
			sqliteConnection.Open();
			sqliteConnection.BackupDatabase(_connection);
		}
	}

	public override async Task<RequestContainer> SearchFor(string word)
	{
		word = word.ToLower();
		RequestContainer returnContainer = new RequestContainer(word.ToUpperInvariant());

		SqliteCommand command = _connection.CreateCommand();
		command.CommandText = @"
SELECT WordForm.UniqueWordForm, Definition.WordDefinition
FROM Word
     FULL JOIN WordForm on WordForm.WordID = Word.WordID
     FULL JOIN Definition on Word.WordID = Definition.WordID
WHERE Word.MainWord = $word;
";
		command.Parameters.Add(new SqliteParameter("$word", word));

		using (SqliteDataReader reader = await command.ExecuteReaderAsync())
		{
			while (reader.Read())
			{
				if (!reader.IsDBNull(0)) returnContainer.AddAlternateForm(reader.GetString(0));
				if (!reader.IsDBNull(1)) returnContainer.AddDefinition(reader.GetString(1));
			}
		}



		if (returnContainer.AlternateForms.Count == 0 || returnContainer.Definitions.Count == 0)
		{
			command = _connection.CreateCommand();
			command.CommandText = @"
SELECT WordForm.UniqueWordForm, Definition.WordDefinition
FROM Word
     FULL JOIN WordForm on WordForm.WordID = Word.WordID
     FULL JOIN Definition on Word.WordID = Definition.WordID
WHERE Word.WordID = (
SELECT WordForm.WordID
FROM WordForm
WHERE WordForm.UniqueWordForm = $word
LIMIT 1);
";
			command.Parameters.Add(new SqliteParameter("$word", word));

			using (SqliteDataReader reader = await command.ExecuteReaderAsync())
			{
				while (reader.Read())
				{
					if (!reader.IsDBNull(0)) returnContainer.AddAlternateForm(reader.GetString(0));
					if (!reader.IsDBNull(1)) returnContainer.AddDefinition(reader.GetString(1));
				}
			}

			if (returnContainer.AlternateForms.Count == 0 || returnContainer.Definitions.Count == 0)
			{
				returnContainer = new RequestContainer(word);
				returnContainer.AddAlternateForm("No alternate forms discovered");
				returnContainer.AddDefinition("No definitions discovered");
			}
		}
	return returnContainer;
	}

	public override async Task<List<RequestContainer>> SearchLike(string word)
	{
		List<RequestContainer> returnArray = [];

		SqliteCommand command = _connection.CreateCommand();
		command.CommandText = @"
SELECT DISTINCT WordForm.UniqueWordForm, Word.MainWord, Definition.WordDefinition 
FROM Word
INNER JOIN WordForm on WordForm.WordID = Word.WordID
INNER JOIN Definition on Word.WordID = Definition.WordID
WHERE Word.MainWord LIKE $pattern OR WordForm.UniqueWordForm LIKE $pattern
";

		command.Parameters.Add(new SqliteParameter("$pattern", $"%{word}%"));
		using (SqliteDataReader reader = await command.ExecuteReaderAsync())
		{
			if (!reader.HasRows) return returnArray;
			reader.Read();
			RequestContainer returnContainer = new RequestContainer(reader.GetString(1));
			int currentWordNumber = 0;
			do
			{
				if (reader.GetString(1) != returnContainer.Word)
				{
					currentWordNumber++;
					returnArray.Add(returnContainer);
					returnContainer = new RequestContainer(reader.GetString(1));
				}

				returnContainer.AddAlternateForm(reader.GetString(0));
				returnContainer.AddDefinition(reader.GetString(2));

			} while ((reader.Read() && currentWordNumber < 19));
			returnArray.Add(returnContainer);
		}


		return returnArray;
	}
}