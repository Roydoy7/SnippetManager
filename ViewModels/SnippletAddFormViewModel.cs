using SnippetManager.Commands;
using SnippetManager.Models;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace SnippetManager.ViewModels
{
    public class SnippetAddFormViewModel : ViewModelBase
    {
        public bool DialogResult { get; set; } = false;
        public List<string> Names { get; set; }
        public CodeData CodeData { get; set; } = new CodeData();

        public ICommand OkCommand => new RelayCommand(o =>
        {
            if (string.IsNullOrEmpty(CodeData.Name))
            {
                MessageBox.Show("Please input a name.", "Information", MessageBoxButton.OKCancel, MessageBoxImage.Information);
                return;
            }

            if (Names != null && Names.Any(x => x.ToUpper() == CodeData.Name.ToUpper()))
            {
                MessageBox.Show("Same name exist, please use another name.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
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
