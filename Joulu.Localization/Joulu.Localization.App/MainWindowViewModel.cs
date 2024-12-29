using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reactive;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Platform.Storage;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;

namespace Joulu.Localization.App;

public class MainWindowViewModel : ViewModelBase
{
    private string _greeting = "Localization manager";
    private int _counter = 0;
    private bool _isIncreasing = false;
    
    public MainWindowViewModel()
    {
        var canIncrease = this.WhenAnyValue(vm => vm._isIncreasing, v => v == false);
        IncreaseCommand = ReactiveCommand.CreateFromTask(Increase, canIncrease);
        AddFileCommand = ReactiveCommand.CreateFromTask(AddSelectedFile);
    }

    public string Greeting
    {
        get => _greeting;
        set => this.RaiseAndSetIfChanged(ref _greeting, value);
    }
    
    public ObservableCollection<SelectedLocalizationFile> SelectedFiles { get; } = [];

    public ReactiveCommand<Unit, Unit> IncreaseCommand { get; }
    public ReactiveCommand<Unit, Unit> AddFileCommand { get; }

    private async Task Increase()
    {
        _isIncreasing = true;
        await Task.Delay(2000);
        _counter++;
        Greeting = $"Localization manager {_counter}";
    }

    private async Task AddSelectedFile()
    {
        var options = new FilePickerOpenOptions {
            Title = "Select localization file", 
            // FileTypeFilter = [new FilePickerFileType("json")],
            AllowMultiple = false
        };
        var storage = App.Resolve<IStorageProvider>();
        var files = await storage.OpenFilePickerAsync(options);
        
        foreach (var file in files)
        {
            await using var stream = await file.OpenReadAsync();
        
            var content = await System.Text.Json.JsonDocument.ParseAsync(stream);
        
            var sections = 0;
            foreach (var section in content.RootElement.EnumerateObject())
            {
                sections++;
            }
        
            var selectedFile = new SelectedLocalizationFile(file.Path, sections);
            SelectedFiles.Add(selectedFile);
        }
    }
}

public class SelectedLocalizationFile
{
    public Uri Path { get; }
    public int Sections { get; }

    public SelectedLocalizationFile(Uri path, int sections)
    {
        Path = path;
        Sections = sections;
    }
}