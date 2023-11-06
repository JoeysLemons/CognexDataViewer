using CognexDataViewer.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace CognexDataViewer.ViewModels
{
    public partial class DataViewModalViewModel: ObservableObject
    {
		public DataViewModalViewModel(DataTable displayTable, int selectedIndex) 
		{
			_displayTable = displayTable;
			title = "Test Title";
			for (int i = 0; i < _displayTable.Columns.Count; i++)
			{
				string header = _displayTable.Columns[i].ToString();
				DataRow row = _displayTable.Rows[selectedIndex];
				string currentMeasurement = row.ItemArray[i].ToString();
				Measurements.Add(new TagMeasurement() { Name = header, Value = currentMeasurement });
			}
		}

		private Tag selectedTag;

		public Tag SelectedTag
		{
			get { return selectedTag; }
			set { selectedTag = value; }
		}

		private DataTable _displayTable;

		public ObservableCollection<TagMeasurement> Measurements { get; set; } = new ObservableCollection<TagMeasurement>();

		[ObservableProperty]
		public string image = "C:\\Users\\jverstraete\\Desktop\\JunkChest\\Cognex\\FTP\\2052_09-26-2023 12_20_51.bmp";

		[ObservableProperty]
		public string imageOverlay = "C:\\Users\\jverstraete\\Desktop\\JunkChest\\Cognex\\SVGWORKING.svg";

        [ObservableProperty]
		private string title;

		[RelayCommand]
		public void Close(object sender) 
		{
			this.Close(sender);
		}



	}
}
