using Microsoft.VisualStudio.Shell;
using PropertyChanged;
using SnippetManager.Commands;
using SnippetManager.Models;
using SnippetManager.Policies;
using System.Collections.Generic;
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
        public string FolderPath { get; set; }
        [OnChangedMethod(nameof(RefreshView))]
        public string Keyword { get; set; }
        public CodeData SelectedItem { get; set; }
        public FileSystemWatcher FileSystemWatcher { get; set; }
        public ICollectionView CodeDataView { get; set; }
        public ObservableCollection<CodeData> CodeDatas { get; set; } = new ObservableCollection<CodeData>();

        public SnippetWindowControlViewModel()
        {
            FolderPath = PathPolicy.FolderPath;
            Directory.CreateDirectory(FolderPath);
            InitFileSystemWatcher(FolderPath);
            InitCodeDatas(FolderPath);
        }

        private void FileSystemWatcher_CreateOrDelete(object sender, FileSystemEventArgs e)
        {
            var filePath = e.FullPath;
            if (e.ChangeType == WatcherChangeTypes.Created)
                AddCodeData(filePath);
            else if (e.ChangeType == WatcherChangeTypes.Deleted)
                RemoveCodeData(filePath);
        }

        private void InitFileSystemWatcher(string folderPath)
        {
            FileSystemWatcher = new FileSystemWatcher(folderPath);
            FileSystemWatcher.Filter = "*.cs";
            FileSystemWatcher.Created += FileSystemWatcher_CreateOrDelete;
            FileSystemWatcher.Deleted += FileSystemWatcher_CreateOrDelete;
            FileSystemWatcher.EnableRaisingEvents = true;
        }

        private void InitCodeDatas(string folderPath)
        {
            foreach (var filePath in Directory.GetFiles(folderPath, "*.cs"))
            {
                AddCodeData(filePath);
            }
            CodeDataView = CollectionViewSource.GetDefaultView(CodeDatas);
            CodeDataView.Filter = Filter;
        }

        private bool Filter(object obj)
        {
            if (string.IsNullOrEmpty(Keyword)) return true;
            if(obj is CodeData data)
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

        private void AddCodeData(string filePath)
        {
            var fileName = Path.GetFileNameWithoutExtension(filePath);
            var content = File.ReadAllText(filePath);
            if (!CodeDatas.Any(x => x.Name == fileName))
            {
                ThreadHelper.JoinableTaskFactory.Run(async delegate {
                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                    CodeDatas.Add(new CodeData { Name = fileName, Content = content });
                });
            }
        }

        private void RemoveCodeData(string filePath)
        {
            var fileName = Path.GetFileNameWithoutExtension(filePath);
            var target = CodeDatas.FirstOrDefault(x => x.Name == fileName);
            if (target != null)
            {
                ThreadHelper.JoinableTaskFactory.Run(async delegate {
                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                    CodeDatas.Remove(target);
                });
            }
        }

        private void ModifyCodeData(CodeData data)
        {
            var filePath = Path.Combine(FolderPath, data.Name + ".cs");
            File.WriteAllText(filePath, data.Content);
        }

        private void DeleteCodeDataFile(string name)
        {
            var filePath = Path.Combine(FolderPath, name + ".cs");
            File.Delete(filePath);
        }

        private void CreateCodeDataFile(CodeData data)
        {
            var filePath = Path.Combine(FolderPath, data.Name + ".cs");
            File.WriteAllText(filePath, data.Content);
        }

        public ICommand AddCommand => new RelayCommand(o =>
        {
            var vm = new SnippetAddFormViewModel();
            vm.Names = CodeDatas.Select(x => x.Name).ToList();
            vm.ShowDialog();

            if (vm.DialogResult == false) return;
            CreateCodeDataFile(vm.CodeData);
        });

        public ICommand DeleteCommand => new RelayCommand(o =>
        {
            if (SelectedItem == null) return;
            if (MessageBox.Show("Do you want to delete?", "Question", MessageBoxButton.OKCancel, MessageBoxImage.Question) != MessageBoxResult.OK)
                return;
            DeleteCodeDataFile(SelectedItem.Name);
        });

        public ICommand CopyCommand => new RelayCommand(o =>
        {
            if (SelectedItem == null) return;
            Clipboard.SetText(SelectedItem.Content);
        });

        public ICommand SaveCommand => new RelayCommand(o =>
        {
            foreach (var data in CodeDatas)
                ModifyCodeData(data);
        });
    }
}
