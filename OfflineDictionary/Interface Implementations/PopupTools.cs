using System.Threading.Tasks;
using Avalonia.Controls;
using OfflineDictionary.Interfaces;
using OfflineDictionary.Views;

namespace OfflineDictionary.Interface_Implementations;

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

public class PopupTools :IPopupTools
{
	public async Task<bool> PopupConfirmGeneric(string title, string content, bool allowCancel)
	{
		var mainWindow = App.MainWindow;
		if (mainWindow == null)
		{
			return false;
		}
		Window window = mainWindow;
		Window newWindow = new GenericPopup(title, content, allowCancel, mainWindow.ActualThemeVariant);
		bool result = await newWindow.ShowDialog<bool>(window);
		return result;
	}
}