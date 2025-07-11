using Avalonia.Animation;
using Avalonia;
using Avalonia.Controls;
using System;
using System.Diagnostics;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Styling;
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
namespace OfflineDictionary.Views;

public partial class GenericPopup : Window
{
	public GenericPopup(string title, string content, bool allowNegativeResponse, ThemeVariant selectedThemeVariant)
	{
		InitializeComponent();
		this.RequestedThemeVariant = selectedThemeVariant;
		TitleBox.Text = title;
		ContentBox.Text = content;
		CancelButton.IsEnabled = allowNegativeResponse;
	}

	private void OK_OnClick(object? sender, RoutedEventArgs e)
	{
		Close(true);
	}

	private void Cancel_OnClick(object? sender, RoutedEventArgs e)
	{
		Close(false);
	}
}