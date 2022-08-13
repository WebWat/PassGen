using System;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace PassGen
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private StringBuilder Symbols = new StringBuilder(string.Empty);

        public MainWindow()
        {
            InitializeComponent();
        }

        #region Button_Events
        private void Generate(object sender, RoutedEventArgs e)
        {
            if (PasswordLength.Text.Length == 0)
            {
                ShowErrorMessage("Specify the password length");
                return;
            }
            else if (int.Parse(PasswordLength.Text) < 5 || int.Parse(PasswordLength.Text) > 60)
            {
                ShowErrorMessage("Password length 5-60");
                return;
            }
            else if (Symbols.Length == 0)
            {
                ShowErrorMessage("Set up the generator");
                return;
            }

            // Code from:
            // https://stackoverflow.com/a/54471736
            /********************************************/
            var result = new StringBuilder(int.Parse(PasswordLength.Text));

            using (var rng = new RNGCryptoServiceProvider())
            {
                int count = (int)Math.Ceiling(Math.Log(Symbols.Length, 2) / 8.0);

                int offset = BitConverter.IsLittleEndian ? 0 : sizeof(uint) - count;

                int max = (int)(Math.Pow(2, count * 8) / Symbols.Length) * Symbols.Length;

                byte[] uintBuffer = new byte[sizeof(uint)];

                while (result.Length < int.Parse(PasswordLength.Text))
                {
                    rng.GetBytes(uintBuffer, offset, count);

                    uint num = BitConverter.ToUInt32(uintBuffer, 0);

                    if (num < max)
                    {
                        result.Append(Symbols[(int)(num % Symbols.Length)]);
                    }
                }
            }
            /********************************************/

            Password.Text = result.ToString();
        }

        private void Copy(object sender, RoutedEventArgs e)
        {
            if (Password.Text.Length == 0)
            {
                ShowErrorMessage("The field is empty");
                return;
            }

            Clipboard.SetText(Password.Text);
        }
        #endregion

        #region TextBox_events
        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            //Only read the numbers
            if (!int.TryParse(e.Text, out _))
            {
                e.Handled = true;
            }
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            //Remove the reading of the space
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }
        #endregion

        #region CheckBox_Events
        private void Uppercase_Checked(object sender, RoutedEventArgs e)
        {
            Symbols.Append(SymbolsConstants.Uppercase);
        }

        private void Uppercase_Unchecked(object sender, RoutedEventArgs e)
        {
            Symbols = Symbols.Replace(SymbolsConstants.Uppercase, string.Empty);
        }


        private void Lowercase_Checked(object sender, RoutedEventArgs e)
        {
            Symbols.Append(SymbolsConstants.Lowercase);
        }

        private void Lowercase_Unchecked(object sender, RoutedEventArgs e)
        {
            Symbols = Symbols.Replace(SymbolsConstants.Lowercase, string.Empty);
        }


        private void Numbers_Checked(object sender, RoutedEventArgs e)
        {
            Symbols.Append(SymbolsConstants.Numbers);
        }

        private void Numbers_Unchecked(object sender, RoutedEventArgs e)
        {
            Symbols = Symbols.Replace(SymbolsConstants.Numbers, string.Empty);
        }


        private void Special_Checked(object sender, RoutedEventArgs e)
        {
            Symbols.Append(SymbolsConstants.Special);
        }

        private void Special_Unchecked(object sender, RoutedEventArgs e)
        {
            Symbols = Symbols.Replace(SymbolsConstants.Special, string.Empty);
        }
        #endregion

        private void ShowErrorMessage(string text) =>
            MessageBox.Show(text, null, MessageBoxButton.OK, MessageBoxImage.Error);
    }
}
