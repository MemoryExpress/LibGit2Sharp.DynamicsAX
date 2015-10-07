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

namespace LibGit2Sharp.DynamicsAX.Controls.CommitsView
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

        private void ChangesetHistoryGrid_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            
        }
    }
}
