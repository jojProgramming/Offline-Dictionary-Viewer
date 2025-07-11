using System;
using System.Threading;
using Avalonia.Controls;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Media;
using OfflineDictionary.ViewModels;
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

public partial class MainWindow : Window
{
    private MainWindowViewModel MainVM;
    public MainWindow()
    {
        InitializeComponent();
        MainVM = new MainWindowViewModel(this);
        DataContext = MainVM;
    }

    public async void Search(object source, RoutedEventArgs args)
    {

        if (this.Resources.TryGetValue("SearchIndicationAnimation", out var anim))
        {
            Console.WriteLine("Animation found.");
        }
        else
        {
            Console.WriteLine("Animation not found.");
        }


        var cts = new CancellationTokenSource();
        CancellationToken ct = cts.Token;

        var animation = (Animation) this.Resources["SearchIndicationAnimation"]!;
        var task = animation.RunAsync(SearchBackground, ct);
        await MainVM.SearchFor(SearchField.Text ?? string.Empty);
        cts.Cancel();

    }
}