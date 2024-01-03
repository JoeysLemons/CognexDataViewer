using CognexDataViewer.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.ComponentModel;
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
using System.Windows.Input;
using DataGrid = Wpf.Ui.Controls.DataGrid;
using System.Windows.Data;
using System.DirectoryServices;
using System.Reflection.PortableExecutable;
using Wpf.Ui.Mvvm.Interfaces;
using System.IO;

namespace CognexDataViewer.ViewModels
{
    public partial class DataViewModalViewModel: ObservableObject
    {
		public DataViewModalViewModel(DataTable displayTable, int selectedIndex, int jobId, string sortedColumnName, string direction) 
		{
			MaxIndex = displayTable.Rows.Count - 1;
            _jobId = jobId;
            _displayTable = displayTable;
			try
			{
                _displayTable.DefaultView.Sort = $"{sortedColumnName} {direction}";
            }
			catch (Exception)
			{
				Trace.WriteLine("Table is unsorted");
			}
			HideOverlay = false;
			SelectedIndex = selectedIndex;
		}
		#region Properties

		private Tag selectedTag;

		public Tag SelectedTag
		{
			get { return selectedTag; }
			set { selectedTag = value; }
		}

		public ICollectionView SortedDisplayTable { get; set; }

		private int selectedIndex;

		public int SelectedIndex
		{
			get { return selectedIndex; }
			set 
			{
				selectedIndex = value; 
				GetNewImage();
                GetMeasurements();
                OnPropertyChanged();
			}
		}

		private int _jobId;

		public int JobId
		{
			get { return _jobId; }
			set { _jobId = value; }
		}

		private DataTable _displayTable;
		public DataTable DisplayTable 
		{
			get {return _displayTable; } 
			set { _displayTable = value; } 
		}

		private string ImageDirectory { get; set; } = "C:\\Users\\jverstraete\\Desktop\\JunkChest\\Cognex\\FTP\\";
        #endregion

        #region ObservableProperties
        public ObservableCollection<TagMeasurement> Measurements { get; set; } = new ObservableCollection<TagMeasurement>();

		[ObservableProperty] private string image = $"2052_09-26-2023 12_20_51.bmp";

		[ObservableProperty] private string imageOverlay = "SVGWORKING.svg";
		
		[ObservableProperty] private string imagePath;

		[ObservableProperty] private string imageOverlayPath;

		[ObservableProperty] private DateTime timestamp;

        [ObservableProperty] private string title;

		[ObservableProperty] private bool hideOverlay;
		[ObservableProperty] private bool imageNotFound;
		
		[ObservableProperty] private int maxIndex;
        #endregion

        #region RelayCommands
        [RelayCommand]
		private void NextSelection()
		{
			if (SelectedIndex == maxIndex)
				return;
			try
			{
				SelectedIndex += 1;
				Trace.WriteLine($"Incrementing modal selected index... New Index: {SelectedIndex}");
			}
			catch (InvalidOperationException e)
			{
				Trace.WriteLine(e);
				ShowImageNotFound();	//Unimplemnted will eventually display the image not found image into the modal
			}
			catch (IndexOutOfRangeException e)
			{
				Trace.WriteLine($"IndexOutOfRange: {e.Message}");
				ShowImageNotFound();
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
			if (SelectedIndex == 0)
				return;
			try
			{
				SelectedIndex -= 1;
				Trace.WriteLine($"Decrementing modal selected index... New Index: {SelectedIndex}");
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

        #endregion

        #region Methods
        private void GetMeasurements() 
		{
            title = "Test Title";
			Measurements.Clear();
            for (int i = 0; i < DisplayTable.Columns.Count; i++)
            {
                string header = DisplayTable.Columns[i].ToString();
                DataView view = DisplayTable.DefaultView;
                DataRowView rowView = view[SelectedIndex];
				DataRow row = rowView.Row;
                string currentMeasurement = row.ItemArray[i].ToString();
                Measurements.Add(new TagMeasurement() { Name = header, Value = currentMeasurement });
            }
        }

		private DateTime GetTimestamp()
		{
            DataView view = DisplayTable.DefaultView;
            DataRowView rowView = view[SelectedIndex];
            DataRow row = rowView.Row;
			DateTime timestamp;
			if (DateTime.TryParse(row[0].ToString(), out timestamp))
            {
                Timestamp = timestamp;
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
				Timestamp = timestamp;
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
			ImagePath = "~/Assets/ImageNotFound.png";
        }

		private void GetNewImage()
		{
			DateTime timestamp = GetTimestamp();
			int tagId = DatabaseUtils.GetTagIdByName(DisplayTable.Columns[1].ToString(), JobId);
			Image = DatabaseUtils.GetAssociatedImage(timestamp, tagId) + ".bmp";
			ImagePath = ImageDirectory + Image;
			if (!File.Exists(ImagePath))
				ImageNotFound = false;
			else
				ImageNotFound = true;
			ImageOverlay = ChangeFileExtensionToSVG(Image);
			ImageOverlayPath = ImageDirectory + ImageOverlay;            
            FixViewBox();
		}

		private void FixViewBox()
		{
			SvgReader Reader = new SvgReader();
			Reader.LoadSvg(imageOverlayPath);
			string viewboxRaw = Reader.GetAttributeValueOfTag("svg", "viewBox");
			if (viewboxRaw == null)
			{
				Reader.AddAttributeToTag("svg", "viewBox", "0 0 2448 2048");
				Reader.SaveSVG(ImageOverlayPath);
				ImageOverlayPath = "";
                ImageOverlayPath = ImageDirectory + ImageOverlay;
            }
		}
		


        public void GetSorting()
		{
			
		}

        #endregion
    }
}
