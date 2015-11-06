using System;
using System.Windows;
using System.Windows.Controls;
using LibGit2Sharp.DynamicsAX.ViewModels;


namespace LibGit2Sharp.DynamicsAX.Controls
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

        /*private void LoadResource(string uri)
        {
            ResourceDictionary myResourceDictionary = new ResourceDictionary();

            myResourceDictionary.Source = new Uri(uri, UriKind.Relative);
            Application.Current.Resources.MergedDictionaries.Add(myResourceDictionary);
        }

        private void LoadResources()
        {
            if (Application.Current == null)
            {
                var app = new Application();
                Trace.TraceInformation("Created fake WPF app.");
            }

            var resources = new[]
            {
                //<!-- Generic styles. -->
                "/Git-GUI;component/Styles/Colors.xaml",
                //<!-- WPF user controls. -->
                "/Git-GUI;component/Styles/Border.xaml",
                "/Git-GUI;component/Styles/Button.xaml",
                "/Git-GUI;component/Styles/ComboBox.xaml",
                "/Git-GUI;component/Styles/DataGrid.xaml",
                "/Git-GUI;component/Styles/GridSplitter.xaml",
                "/Git-GUI;component/Styles/TabItem.xaml",
                "/Git-GUI;component/Styles/LeftToolbar.xaml",
                "/Git-GUI;component/Styles/TopToolbar.xaml",
                "/Git-GUI;component/Styles/Window.xaml",
                "/Git-GUI;component/Styles/Panel.xaml",
                "/Git-GUI;component/Styles/TreeView.xaml",
                "/Git-GUI;component/Styles/SplitButton.xaml",
                "/Git-GUI;component/Styles/TextBlock.xaml",
                "/Git-GUI;component/Styles/Separator.xaml",
                "/Git-GUI;component/Styles/TextBox.xaml",
                "/Git-GUI;component/Styles/Scrollbar.xaml",
                //<!-- Custom user controls and elements. -->
                "/Git-GUI;component/Styles/NewTabList.xaml",
                "/Git-GUI;component/Styles/DisplayTags.xaml",
                "/Git-GUI;component/Styles/ChangesetHistoryDataGrid.xaml",

                "/Git-GUI;component/Templates/RepoTabContentTemplate.xaml",
                "/Git-GUI;component/Templates/RepoTabDashboardContentTemplate.xaml",
                "/Git-GUI;component/Templates/RepoTabTemplate.xaml",
                "/Git-GUI;component/Templates/RepoTabNewTemplate.xaml"
            };

            foreach (var resource in resources)
            {
                LoadResource(resource);
            }
        }*/

        public static readonly DependencyProperty RepositoryProperty = DependencyProperty.Register(
            "Repository", typeof (Repository), typeof (SynchronizeView), 
            new PropertyMetadata(default(Repository), OnRepositoryChanged));

        private static void OnRepositoryChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var @this = (SynchronizeView)dependencyObject;

            var repo = dependencyPropertyChangedEventArgs.NewValue as Repository;

            if (repo == null)
                @this.DataContext = null;
            else
                @this.DataContext = new RepositoryViewModel(repo);
        }

        public Repository Repository
        {
            get { return (Repository) GetValue(RepositoryProperty); }
            set { SetValue(RepositoryProperty, value); }
        }

        public static readonly DependencyProperty RepositoryPathProperty = DependencyProperty.Register(
            "RepositoryPath", typeof (string), typeof (SynchronizeView), 
            new PropertyMetadata(default(string), OnRepositoryPathChanged));

        private static void OnRepositoryPathChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var @this = (SynchronizeView)dependencyObject;
            var path = dependencyPropertyChangedEventArgs.NewValue as string;

            if (String.IsNullOrWhiteSpace(path))
            {
                @this.Repository = null;
            }
            else
            {
                @this.Repository = new Repository(path);
            }
        }

        public string RepositoryPath
        {
            get { return (string) GetValue(RepositoryPathProperty); }
            set { SetValue(RepositoryPathProperty, value); }
        }
    }
}
