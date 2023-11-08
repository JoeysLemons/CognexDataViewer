using SharpVectors.Converters;
using SharpVectors.Renderers.Wpf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace CognexDataViewer.Helpers
{
    public class SvgToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string svgPath)
            {
                try
                {
                    Uri svgUri = new Uri(svgPath, UriKind.RelativeOrAbsolute);

                    // In case the URI is not absolute, assume it is a path relative to the application's resources.
                    if (!svgUri.IsAbsoluteUri)
                    {
                        svgPath = Path.Combine(Environment.CurrentDirectory, svgPath);
                        svgUri = new Uri(svgPath, UriKind.Absolute);
                    }

                    // Use the file stream to avoid locking the file.
                    using (FileStream fileStream = new FileStream(svgUri.LocalPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        // Create a new SVG file reader based on the file stream.
                        WpfDrawingSettings settings = new WpfDrawingSettings();
                        settings.IncludeRuntime = true;
                        settings.TextAsGeometry = false; // Set it to true if you have text rendering issues

                        var reader = new FileSvgReader(settings);
                        DrawingGroup drawing = reader.Read(fileStream);

                        if (drawing != null)
                        {
                            return new DrawingImage(drawing);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Log or handle the exception as needed.
                    // For now, we'll just write the message to the debug output.
                    Debug.WriteLine("Error converting SVG to ImageSource: " + ex.Message);
                }
            }

            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("SvgToImageSourceConverter is a one-way converter.");
        }
    }

}
