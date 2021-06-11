using System;
using System.Globalization;
using System.Windows;

namespace SnippetManager.ViewModels
{
    public class ViewModelBase : NotifyBase
    {
        public ViewModelBase()
        {
            InitView();
        }

        public FrameworkElement View { get; private set; }

        private void InitView()
        {
            var viewModelName = this.GetType().Name;
            var viewNameA = viewModelName.Replace("ViewModel", "View");
            var viewNameB = viewModelName.Replace("ViewModel", "");
            var viewModelFullName = this.GetType().FullName;
            var viewFullName = viewModelFullName.Replace(viewModelName, "").Replace(".ViewModels.", ".Views.");

            var viewAssemblyName = this.GetType().Assembly.FullName;
            var viewTypeNameA = string.Format(CultureInfo.InvariantCulture, "{0}{1},{2}", viewFullName, viewNameA, viewAssemblyName);
            var viewTypeNameB = string.Format(CultureInfo.InvariantCulture, "{0}{1},{2}", viewFullName, viewNameB, viewAssemblyName);

            var typeA = Type.GetType(viewTypeNameA, false, false);
            if (typeA != null)
            {
                if (!typeA.IsSubclassOf(typeof(FrameworkElement))) return;

                View = Activator.CreateInstance(typeA) as FrameworkElement;
                View.DataContext = this;
                return;
            }
            else
            {
                var typeB = Type.GetType(viewTypeNameB, false, false);
                if (typeB != null)
                {
                    if (!typeB.IsSubclassOf(typeof(FrameworkElement))) return;

                    View = Activator.CreateInstance(typeB) as FrameworkElement;
                    View.DataContext = this;
                }
            }
        }


        public void ShowDialog()
        {
            if (View == null) return;
            if (View is Window window)
            {
                window.ShowDialog();
            }
        }

        public void Show()
        {
            if (View == null) return;
            if (View is Window window)
            {
                window.Show();
            }
        }

        public void Close()
        {
            if (View == null) return;
            if (View is Window window)
            {
                window.Close();
            }
        }

    }
}
