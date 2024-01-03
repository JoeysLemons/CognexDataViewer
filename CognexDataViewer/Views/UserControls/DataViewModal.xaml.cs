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
        private Helpers.SvgReader Reader { get; set; }
        public DataViewModal(ViewModels.DataViewModalViewModel viewModel)
        {
            ViewModel = viewModel;
            Reader = new Helpers.SvgReader();
            InitializeComponent();
        }

        private void PositionImage()
        {
            try
            {
                Reader.LoadSvg(ViewModel.ImageOverlayPath);
                List<string> svgTags = Reader.GetAllUniqueTags();
                double horizontalOffset = Int32.Parse(Reader.GetAttributeValueOfTag("image", "x"));
                double verticalOffset = Int32.Parse(Reader.GetAttributeValueOfTag("image", "y"));
                string viewboxRaw = Reader.GetAttributeValueOfTag("svg", "viewBox");
                if (viewboxRaw == null)
                {
                    Reader.AddAttributeToTag("svg", "viewBox", "0 0 2448 2048");
                    Reader.SaveSVG(ViewModel.ImageOverlayPath);
                    viewboxRaw = Reader.GetAttributeValueOfTag("svg", "viewBox");
                }
                List<string> viewboxOption = viewboxRaw.Split(" ").ToList();

                double imageWidth = Int32.Parse(viewboxOption[2]);
                double imageHeight = Int32.Parse(viewboxOption[3]);

                double heightOffset = svgImage.ActualHeight - imageHeight;
                double widthOffset = svgImage.ActualWidth - imageWidth;

                Canvas.SetTop(bmpImage, heightOffset + verticalOffset);
                Canvas.SetLeft(bmpImage, widthOffset + horizontalOffset);
            }
            catch (Exception)
            {

            }
            
        }

        private void bmpImage_Loaded(object sender, RoutedEventArgs e)
        {
            PositionImage();
        }

        private void OnImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            bmpImage.Source = new BitmapImage(new Uri("/Assets/ImageNotFound.bmp"));
        }

    }
}
