using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Reflection;
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

namespace LibGit2Sharp.DynamicsAX.Shell
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Repository _repository;

        public MainWindow()
        {
            InitializeComponent();

            _repository = new Repository(@"C:\AX\Repos\POS\AXPOS");

            SynchronizeView.Repository = _repository;
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            _repository.Dispose();
        }
    }
}
