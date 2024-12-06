using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using MessageBox.Avalonia;
using MessageBox.Avalonia.Enums;
using NumeralSystems.Net;

namespace NumeralConverter
{
    public class MainWindow : Window
    {
        public TextBox InputBase => this.FindControl<TextBox>(nameof(InputBase));
        public TextBox InputNumber => this.FindControl<TextBox>(nameof(InputNumber));
        public TextBox DestinationBase => this.FindControl<TextBox>(nameof(DestinationBase));
        public TextBox DestinationValue => this.FindControl<TextBox>(nameof(DestinationValue));
        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void PerformConversion(object? sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(InputBase?.Text) || !int.TryParse(InputBase.Text, out var inputBaseInteger))
            {
                _ = MessageBoxManager.GetMessageBoxStandardWindow("Error", "Input base must be a valid int value.", ButtonEnum.Ok, MessageBox.Avalonia.Enums.Icon.Error, WindowStartupLocation.CenterScreen, Style.DarkMode).Show();
                return;
            }

            var inputBase = Numeral.System.OfBase(inputBaseInteger);
            var inputValue = Value.FromString()
            try
            {
                inputValue = inputBase.Parse(InputNumber?.Text);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                _ = MessageBoxManager.GetMessageBoxStandardWindow("Error", ex.Message, ButtonEnum.Ok, MessageBox.Avalonia.Enums.Icon.Error, WindowStartupLocation.CenterScreen, Style.DarkMode).Show();
                return;
            }
            
            if (string.IsNullOrEmpty(DestinationBase.Text) || !int.TryParse(DestinationBase.Text, out var destinationBaseInteger))
            {
                _ = MessageBoxManager.GetMessageBoxStandardWindow("Error", "Input base must be a valid int value.", ButtonEnum.Ok, MessageBox.Avalonia.Enums.Icon.Error, WindowStartupLocation.CenterScreen, Style.DarkMode).Show();
                return;
            }
            var destinationBase = Numeral.System.OfBase(destinationBaseInteger);
            var destinationValue = inputValue.To(destinationBase);
            DestinationValue.Text = destinationValue.ToString();
        }
    }
}