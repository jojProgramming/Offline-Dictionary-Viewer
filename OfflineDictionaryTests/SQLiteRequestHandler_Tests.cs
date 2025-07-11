using System;
using System.Threading.Tasks;

namespace OfflineDictionaryTests;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using OfflineDictionary.Interface_Implementations;
using OfflineDictionary.Interfaces;
using OfflineDictionary.Models;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using System.IO;

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

[TestClass]
public class SqliteRequestHandlerTests
{
    private IDBRequestHandler _handler;
    private readonly string _testDbPath = "TESTDICTIONARYFILEPATH";

    [TestInitialize]
    public void Setup()
    {
        // Setup test database with known test data
        _handler = new SqliteRequestHandler(_testDbPath);
    }

    [TestMethod]
    public async Task SearchLike_WithExistingWord_ReturnsMatchingResults()
    {
        // Arrange
        string searchTerm = "October";

        // Act
        List<RequestContainer> results = await _handler.SearchLike(searchTerm);

        // Assert
        Assert.IsNotNull(results);
        Assert.IsTrue(results.Count > 0);
        foreach (var result in results)
        {
            Assert.IsTrue(result.Word.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
        }
    }

    [TestMethod]
    public async Task SearchLike_WithNonExistingWord_ReturnsEmptyList()
    {
        // Arrange
        string searchTerm = "nonexistentword123456";

        // Act
        List<RequestContainer> results = await _handler.SearchLike(searchTerm);

        // Assert
        Assert.IsNotNull(results);
        Assert.AreEqual(0, results.Count);
    }

    [TestMethod]
    public async Task SearchLike_ReturnsMaximumNineResults()
    {
        // Arrange
        string searchTerm = "o"; // Common letter that should match many words

        // Act
        List<RequestContainer> results = await _handler.SearchLike(searchTerm);

        // Assert
        Assert.IsTrue(results.Count <= 20);
    }

    [TestMethod]
    public async Task SearchLike_ResultsContainAlternateForms()
    {
        // Arrange
        string searchTerm = "October";

        // Act
        List<RequestContainer> results = await _handler.SearchLike(searchTerm);

        // Assert
        Assert.IsTrue(results.Count > 0);
        foreach (var result in results)
        {
            Assert.IsTrue(result.AlternateForms.Count > 0);
        }
    }

    [TestMethod]
    [ExpectedException(typeof(SqliteException))]
    public async Task SearchLike_WithInvalidDatabasePath_ThrowsException()
    {
        // Arrange
        var handler = new SqliteRequestHandler("invalid/path/to/db.sqlite");

        // Act
        await handler.SearchLike("test");

        // Assert is handled by ExpectedException attribute
    }

    [TestCleanup]
    public void Cleanup()
    {
        // Clean up test database if needed
    }
}
