using SnippetManager.Commands;
using SnippetManager.Models;
using System.Windows;
using System.Windows.Input;

namespace SnippetManager.ViewModels
{
    public class SnippetAddFormViewModel : ViewModelBase
    {
        public bool DialogResult { get; set; } = false;
        public CodeData CodeData { get; set; } = new CodeData();

        public ICommand OkCommand => new RelayCommand(o =>
        {
            if (string.IsNullOrEmpty(CodeData.Name))
            {
                MessageBox.Show("Please input a name.", "Information", MessageBoxButton.OKCancel, MessageBoxImage.Information);
                return;
            }

            DialogResult = true;
            Close();
        });

        public ICommand CancelCommand => new RelayCommand(o =>
        {
            Close();
        });
    }
}
