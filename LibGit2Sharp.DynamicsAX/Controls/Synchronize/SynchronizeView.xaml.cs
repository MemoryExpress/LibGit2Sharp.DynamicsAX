using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LibGit2Sharp.DynamicsAX.ViewModels;

namespace LibGit2Sharp.DynamicsAX.Controls.Synchronize
{
    /// <summary>
    /// Interaction logic for Synchronize.xaml
    /// </summary>
    public partial class SynchronizeView : UserControl
    {
        public SynchronizeView()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty RepositoryProperty = DependencyProperty.Register(
            "Repository", typeof (Repository), typeof (SynchronizeView), 
            new PropertyMetadata(default(Repository), OnRepositoryChanged));

        private static void OnRepositoryChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var @this = (SynchronizeView)dependencyObject;
            var repository = dependencyPropertyChangedEventArgs.NewValue as Repository;

            if (repository == null)
            {
                @this.DataContext = null;
            }
            else
            {
                @this.DataContext = new RepositoryViewModel(repository);
            }
        }

        public Repository Repository
        {
            get { return (Repository) GetValue(RepositoryProperty); }
            set { SetValue(RepositoryProperty, value); }
        }
    }
}
