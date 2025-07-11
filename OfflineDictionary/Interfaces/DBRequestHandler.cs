using System.Collections.Generic;
using System.Threading.Tasks;
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

namespace OfflineDictionary.Interfaces;

public abstract class DBRequestHandler : IDBRequestHandler
{
	protected DBRequestHandler(string dbUri)
	{
		// Set up the database connection using the URI
	}
	public abstract Task<RequestContainer> SearchFor(string word);

	public abstract Task<List<RequestContainer>> SearchLike(string word);
}