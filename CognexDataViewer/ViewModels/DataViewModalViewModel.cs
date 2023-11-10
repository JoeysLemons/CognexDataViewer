using CognexDataViewer.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using CognexDataViewer.Helpers;
using Wpf.Ui.Controls;

namespace CognexDataViewer.ViewModels
{
    public partial class DataViewModalViewModel: ObservableObject
    {
		public DataViewModalViewModel(DataTable displayTable, int selectedIndex) 
		{
			SelectedIndex = selectedIndex;
			_displayTable = displayTable;
			imagePath = ImageDirectory + Image;
			imageOverlayPath = ImageDirectory + ImageOverlay;
			GetMeasurements();
		}

		private Tag selectedTag;

		public Tag SelectedTag
		{
			get { return selectedTag; }
			set { selectedTag = value; }
		}

		private int SelectedIndex { get; set; }

		private DataTable _displayTable;
		public DataTable DisplayTable 
		{
			get {return _displayTable; } 
			set { _displayTable = value; } 
		}

		private string ImageDirectory { get; set; } = "C:\\Users\\jverstraete\\Desktop\\JunkChest\\Cognex\\FTP\\";

		private ObservableCollection<TagMeasurement> Measurements { get; set; } = new ObservableCollection<TagMeasurement>();

		[ObservableProperty] private string image = $"2052_09-26-2023 12_20_51.bmp";

		[ObservableProperty] private string imageOverlay = "SVGWORKING.svg";
		
		[ObservableProperty] private string imagePath;

		[ObservableProperty] private string imageOverlayPath;

        [ObservableProperty] private string title;

		[RelayCommand]
		private void NextSelection()
		{
			try
			{
				SelectedIndex += 1;
				GetNewImage();
				Trace.WriteLine($"Incrementing modal selected index... New Index: {SelectedIndex}");
				GetMeasurements();
			}
			catch (InvalidOperationException e)
			{
				Console.WriteLine(e);
				ShowImageNotFound();	//Unimplemnted will eventually display the image not found image into the modal
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				throw;
			}
			
		}
		[RelayCommand]
		private void PrevSelection()
		{
			try
			{
				SelectedIndex -= 1;
				GetNewImage();
				Trace.WriteLine($"Decrementing modal selected index... New Index: {SelectedIndex}");
				GetMeasurements();
			}
			catch (InvalidOperationException e)
			{
				Console.WriteLine(e);
				ShowImageNotFound();	//Unimplemnted will eventually display the image not found image into the modal
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				throw;
			}
			
		}


		private void GetMeasurements() 
		{
            title = "Test Title";
			Measurements.Clear();
            for (int i = 0; i < DisplayTable.Columns.Count; i++)
            {
                string header = DisplayTable.Columns[i].ToString();
                DataRow row = DisplayTable.Rows[SelectedIndex];
                string currentMeasurement = row.ItemArray[i].ToString();
                Measurements.Add(new TagMeasurement() { Name = header, Value = currentMeasurement });
            }
        }

		private DateTime GetTimestamp()
		{
			DataRow row = DisplayTable.Rows[SelectedIndex];
			DateTime timestamp;
			if (DateTime.TryParse(row[0].ToString(), out timestamp))
			{
				return timestamp;
			}

			throw new InvalidOperationException($"Failed to parse DateTime from row {SelectedIndex}. Possible missing timestamp.");
		}
		
		private DateTime GetTimestamp(int index)
		{
			DataRow row = DisplayTable.Rows[index];
			DateTime timestamp;
			if (DateTime.TryParse(row[0].ToString(), out timestamp))
			{
				return timestamp;
			}

			throw new InvalidOperationException($"Failed to parse DateTime from row {index}. Possible missing timestamp.");
		}

		private string ChangeFileExtensionToSVG(string filename)
		{
			string convertedFilename;
			convertedFilename = filename.Substring(0, filename.Length - 3);
			Trace.WriteLine($"Subbed Filename: {convertedFilename}");
			return convertedFilename + "svg";
		}

		private void ShowImageNotFound()
		{
			
		}

		private void GetNewImage()
		{
			DateTime timestamp = GetTimestamp();
			int tagId = DatabaseUtils.GetTagIdByName(DisplayTable.Columns[1].ToString());
			Image = DatabaseUtils.GetAssociatedImage(timestamp, tagId);
			ImagePath = ImageDirectory + Image;
			ImageOverlay = ChangeFileExtensionToSVG(Image);
			ImageOverlayPath = ImageDirectory + ImageOverlay;
		}

	}
}
