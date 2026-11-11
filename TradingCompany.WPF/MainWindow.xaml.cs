using System.Windows;
using TradingCompany.ViewModels;

namespace TradingCompany.WPF;

public partial class MainWindow : Window
{
    public MainWindow(ShellViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
