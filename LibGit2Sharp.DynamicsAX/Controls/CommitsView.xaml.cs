using System;
using System.Collections.Generic;
using System.Linq;
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

namespace LibGit2Sharp.DynamicsAX.Controls
{
    /// <summary>
    /// Interaction logic for CommitsView.xaml
    /// </summary>
    public partial class CommitsView : UserControl
    {
        public CommitsView()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty RepositoryViewModelProperty = DependencyProperty.Register(
            "RepositoryViewModel", typeof (RepositoryViewModel), typeof (CommitsView), 
            new PropertyMetadata(default(RepositoryViewModel), OnRepositoryViewModelChanged));

        private static void OnRepositoryViewModelChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            CommitsView @this = (CommitsView) dependencyObject;

            @this.DataContext = dependencyPropertyChangedEventArgs.NewValue;
        }

        public RepositoryViewModel RepositoryViewModel
        {
            get { return (RepositoryViewModel) GetValue(RepositoryViewModelProperty); }
            set { SetValue(RepositoryViewModelProperty, value); }
        }

        private void Graph_OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Canvas graph = (Canvas)sender;

            CommitGraphDetails commit = (CommitGraphDetails)graph.DataContext;

            //get the branch this commit is on
            //var branches = RepositoryViewModel.Repository.ListBranchesContainingCommit(commit.Sha);
        }


    }
}
