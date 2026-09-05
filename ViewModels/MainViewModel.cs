using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
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
    public partial string ErrorMessage { get; set; } = "";

    [ObservableProperty]
    public partial int SelectedIndex { get; set; } = -1;

    public ObservableCollection<decimal> Transactions { get; } = new();

    public decimal TotalIncome => Transactions.Where(a => a > 0).Sum();

    public decimal TotalExpenses => -Transactions.Where(a => a < 0).Sum();

    public decimal Balance => Transactions.Sum();

    public MainViewModel()
    {
        if (!File.Exists(FilePath))
            return;

        foreach (var line in File.ReadAllLines(FilePath))
            if (decimal.TryParse(line, out var amount))
                Transactions.Add(amount);
    }

    [RelayCommand]
    private void AddIncome()
    {
        if (!decimal.TryParse(AmountInput, out var amount))
        {
            ErrorMessage = "Bitte eine gültige Zahl eingeben";
            return;
        }

        ErrorMessage = "";
        Transactions.Add(amount);
        AmountInput = "";
        Save();
    }

    [RelayCommand]
    private void AddExpense()
    {
        if (!decimal.TryParse(AmountInput, out var amount))
        {
            ErrorMessage = "Bitte eine gültige Zahl eingeben";
            return;
        }

        ErrorMessage = "";
        Transactions.Add(-amount);
        AmountInput = "";
        Save();
    }

    [RelayCommand]
    private void Delete()
    {
        if (SelectedIndex < 0)
            return;

        Transactions.RemoveAt(SelectedIndex);
        Save();
    }

    private void Save()
    {
        File.WriteAllLines(FilePath, Transactions.Select(a => a.ToString()));
        OnPropertyChanged(nameof(TotalIncome));
        OnPropertyChanged(nameof(TotalExpenses));
        OnPropertyChanged(nameof(Balance));
    }
}
