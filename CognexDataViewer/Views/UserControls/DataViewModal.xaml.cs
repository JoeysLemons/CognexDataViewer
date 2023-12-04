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

namespace CognexDataViewer.Views.UserControls
{
    /// <summary>
    /// Interaction logic for DataViewModal.xaml
    /// </summary>
    public partial class DataViewModal : UserControl
    {
        public ViewModels.DataViewModalViewModel ViewModel { get; set; }
        public DataViewModal(ViewModels.DataViewModalViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();
        }
    }
}
