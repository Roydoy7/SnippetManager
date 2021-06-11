using Microsoft.VisualStudio.Shell;
using PropertyChanged;
using SnippetManager.Commands;
using SnippetManager.Components;
using SnippetManager.Models;
using SnippetManager.Policies;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace SnippetManager.ViewModels
{
    public class SnippetWindowControlViewModel : ViewModelBase
    {
        [OnChangedMethod(nameof(RefreshView))]
        public string Keyword { get; set; }
        public CodeData SelectedItem { get; set; }
        public FileSystemWatcher FileSystemWatcher { get; set; }
        public ICollectionView CodeDataView { get; set; }
        public ObservableCollection<CodeData> CodeDatas { get; set; } = new ObservableCollection<CodeData>();

        public SnippetWindowControlViewModel()
        {
            InitFileSystemWatcher();
            InitCodeDatas();
        }

        private void FileSystemWatcher_CreateOrDelete(object sender, FileSystemEventArgs e)
        {
            var filePath = e.FullPath;
            if (e.ChangeType == WatcherChangeTypes.Created)
                AddCodeData(CodeDataMethods.GetCodeData(filePath));
            else if (e.ChangeType == WatcherChangeTypes.Deleted)
                RemoveCodeData(filePath);
        }

        private void InitFileSystemWatcher()
        {
            var folderPath = PathPolicy.FolderPath;
            FileSystemWatcher = new FileSystemWatcher(folderPath);
            FileSystemWatcher.Filter = "*" + NamePolicy.Extension;
            FileSystemWatcher.Created += FileSystemWatcher_CreateOrDelete;
            FileSystemWatcher.Deleted += FileSystemWatcher_CreateOrDelete;
            FileSystemWatcher.EnableRaisingEvents = true;
        }

        private void InitCodeDatas()
        {
            foreach (var data in CodeDataMethods.GetCodeDatas())
                AddCodeData(data);
            CodeDataView = CollectionViewSource.GetDefaultView(CodeDatas);
            if (CodeDataView != null)
                CodeDataView.Filter = Filter;
        }

        private bool Filter(object obj)
        {
            if (string.IsNullOrEmpty(Keyword)) return true;
            if (obj is CodeData data)
            {
                if (data.Name.ToUpper().Contains(Keyword.ToUpper()))
                    return true;
                if (data.Content.ToUpper().Contains(Keyword.ToUpper()))
                    return true;
            }
            return false;
        }

        private void RefreshView()
        {
            CodeDataView.Refresh();
        }

        private void AddCodeData(CodeData data)
        {
            if (!CodeDatas.Any(x => x.FileName == data.FileName))
            {
                ThreadHelper.JoinableTaskFactory.Run(async delegate
                {
                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                    CodeDatas.Add(data);
                });
            }
        }

        private void RemoveCodeData(string filePath)
        {
            var fileName = Path.GetFileName(filePath);
            var target = CodeDatas.FirstOrDefault(x => x.FileName == fileName);
            if (target != null)
            {
                ThreadHelper.JoinableTaskFactory.Run(async delegate
                {
                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                    CodeDatas.Remove(target);
                });
            }
        }

        public ICommand AddCommand => new RelayCommand(o =>
        {
            var vm = new SnippetAddFormViewModel();
            vm.ShowDialog();

            if (vm.DialogResult == false) return;
            CodeDataMethods.CreateCodeDataFile(vm.CodeData);
        });

        public ICommand DeleteCommand => new RelayCommand(o =>
        {
            if (SelectedItem == null) return;
            if (MessageBox.Show("Do you want to delete?", "Question", MessageBoxButton.OKCancel, MessageBoxImage.Question) != MessageBoxResult.OK)
                return;
            CodeDataMethods.DeleteCodeDataFile(SelectedItem.FileName);
        });

        public ICommand CopyCommand => new RelayCommand(o =>
        {
            if (SelectedItem == null) return;
            if (SelectedItem.Content == null) return;
            Clipboard.SetText(SelectedItem.Content);
        });

        public ICommand SaveCommand => new RelayCommand(o =>
        {
            foreach (var data in CodeDatas)
                CodeDataMethods.ModifyCodeData(data);
        });

        public ICommand RenameCommand => new RelayCommand(o=> {
            if (SelectedItem == null) return;
            var vm = new SnippetRenameFormViewModel();
            vm.Name = SelectedItem.Name;
            vm.ShowDialog();
            if (vm.DialogResult)
                SelectedItem.Name = vm.Name;
        });
    }
}
