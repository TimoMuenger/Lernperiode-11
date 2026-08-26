using System;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace FinanzManager.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private static readonly string FilePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "finanzmanager.txt");

    [ObservableProperty]
    public partial string AmountInput { get; set; } = "";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Balance))]
    public partial decimal TotalIncome { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Balance))]
    public partial decimal TotalExpenses { get; set; }

    public decimal Balance => TotalIncome - TotalExpenses;

    public MainViewModel()
    {
        if (!File.Exists(FilePath))
            return;

        var parts = File.ReadAllText(FilePath).Split(';');
        if (parts.Length != 2)
            return;

        decimal.TryParse(parts[0], out var income);
        decimal.TryParse(parts[1], out var expenses);

        TotalIncome = income;
        TotalExpenses = expenses;
    }

    [RelayCommand]
    private void AddIncome()
    {
        if (!decimal.TryParse(AmountInput, out var amount))
            return;

        TotalIncome += amount;
        AmountInput = "";
        Save();
    }

    [RelayCommand]
    private void AddExpense()
    {
        if (!decimal.TryParse(AmountInput, out var amount))
            return;

        TotalExpenses += amount;
        AmountInput = "";
        Save();
    }

    private void Save() => File.WriteAllText(FilePath, $"{TotalIncome};{TotalExpenses}");
}
