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

namespace CognexDataViewer.ViewModels
{
    public partial class DataViewModalViewModel: ObservableObject
    {
		public DataViewModalViewModel(DataTable displayTable, int selectedIndex) 
		{
			SelectedIndex = selectedIndex;
			_displayTable = displayTable;
			GetMeasurements();
		}

		private Tag selectedTag;

		public Tag SelectedTag
		{
			get { return selectedTag; }
			set { selectedTag = value; }
		}

		public int SelectedIndex { get; set; }

		private DataTable _displayTable;
		public DataTable DisplayTable 
		{
			get {return _displayTable; } 
			set { _displayTable = value; } 
		}

		public ObservableCollection<TagMeasurement> Measurements { get; set; } = new ObservableCollection<TagMeasurement>();

		[ObservableProperty]
		public string image = "C:\\Users\\jverstraete\\Desktop\\JunkChest\\Cognex\\FTP\\2052_09-26-2023 12_20_51.bmp";

		[ObservableProperty]
		public string imageOverlay = "C:\\Users\\jverstraete\\Desktop\\JunkChest\\Cognex\\SVGWORKING.svg";

        [ObservableProperty]
		private string title;

		[RelayCommand]
		public void NextSelection()
		{
			SelectedIndex += 1;
			Trace.WriteLine($"Incrementing modal selected index... New Index: {SelectedIndex}");
			GetMeasurements();
		}
		[RelayCommand]
		public void PrevSelection()
		{
			SelectedIndex -= 1;
			Trace.WriteLine($"Decrementing modal selected index... New Index: {SelectedIndex}");
			GetMeasurements();
		}


		public void GetMeasurements() 
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

	}
}
