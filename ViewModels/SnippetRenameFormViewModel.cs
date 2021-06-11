using SnippetManager.Commands;
using System.Windows.Input;

namespace SnippetManager.ViewModels
{
    public class SnippetRenameFormViewModel : ViewModelBase
    {
        public bool DialogResult { get; set; }
        public string Name { get; set; }
        public ICommand OkCommand => new RelayCommand(o =>
        {
            DialogResult = true;
            Close();
        });

        public ICommand CancelCommand => new RelayCommand(o =>
        {
            Close();
        });
    }
}
