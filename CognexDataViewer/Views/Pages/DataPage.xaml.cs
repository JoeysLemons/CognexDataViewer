using CognexDataViewer.Models;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows;
using Wpf.Ui.Common.Interfaces;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Linq;
using System;
using Wpf.Ui.Mvvm.Contracts;
using CognexDataViewer.Views.UserControls;
using Wpf.Ui.Controls;
using MenuItem = System.Windows.Controls.MenuItem;
using DataGrid = System.Windows.Controls.DataGrid;
using System.Diagnostics;
using CognexDataViewer.Helpers;
using System.ComponentModel;

namespace CognexDataViewer.Views.Pages
{
    /// <summary>
    /// Interaction logic for SortedDisplayTable.xaml
    /// </summary>
    public partial class DataPage : INavigableView<ViewModels.DataViewModel>
    {
        private readonly ISnackbarService _snackbarService;
        public ViewModels.DataViewModel ViewModel
        {
            get;
        }

        private void DataGrid_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            ContextMenu contextMenu = new ContextMenu();
            foreach (var column in DataGrid.Columns)
            {
                MenuItem menuItem = new MenuItem
                {
                    Header = column.Header,
                    IsCheckable = true,
                    IsChecked = column.Visibility == Visibility.Visible
                };
                menuItem.Click += (s, args) =>
                {
                    MenuItem clickedItem = s as MenuItem;
                    if (clickedItem != null)
                    {
                        int index = contextMenu.Items.IndexOf(clickedItem);
                        if (index >= 0 && index < DataGrid.Columns.Count)
                        {
                            DataGrid.Columns[index].Visibility = clickedItem.IsChecked ? Visibility.Visible : Visibility.Collapsed;
                        }
                    }
                };
                contextMenu.Items.Add(menuItem);
            }
            DataGrid.ContextMenu = contextMenu;
        }

        //Selects the first option in the job dropdown when a camera is selected
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            JobSelectionDropDown.SelectedIndex = 0;
        }

        public int GetColumnIndexByName(DataGrid grid, string columnName)
        {
            return grid.Columns
                       .Select((column, index) => new { column.Header, index })
                       .Where(x => x.Header.ToString() == columnName)
                       .Select(x => x.index)
                       .FirstOrDefault(-1); // Returns -1 if not found
        }


        public DataPage(ViewModels.DataViewModel viewModel, ISnackbarService snackbarService)
        {
            ViewModel = viewModel;
            InitializeComponent();
            CameraSelectionDropDown.SelectedIndex = 0;
            _snackbarService = snackbarService;
            _snackbarService.SetSnackbarControl(rootSnackbar);
            ViewModel.ErrorMessageChanged += DisplaySnackbar;
        }

        public event EventHandler SnackbarEvent;
        public void DisplaySnackbar()
        {
            _snackbarService.Show(ViewModel.ErrorMessage, "", Wpf.Ui.Common.SymbolRegular.ErrorCircle20);
        }


        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var dataGrid = sender as DataGrid;
            if (dataGrid == null)
                return;
            var selectedIndex = dataGrid.SelectedIndex;
            var sortDescriptions = dataGrid.Items.SortDescriptions;
            if (selectedIndex < 0) return;
            int jobId = DatabaseUtils.GetJobId(ViewModel.SelectedJob);
            Trace.WriteLine($"Selected Index: {selectedIndex}");
            var modalViewModel = new ViewModels.DataViewModalViewModel(ViewModel.DisplayTable, selectedIndex,jobId, ViewModel.SortedColumnName, ViewModel.SortDirection);
            DataViewModal modal = new DataViewModal(modalViewModel);
            UiWindow window = new UiWindow
            {
                Content = modal,
                ExtendsContentIntoTitleBar= true,
                DataContext= modalViewModel,
                WindowStyle= WindowStyle.None,
                WindowBackdropType=Wpf.Ui.Appearance.BackgroundType.Mica,
                WindowStartupLocation= WindowStartupLocation.CenterScreen
            };
            window.Show();
            dataGrid.UnselectAll();
        }

        private void DataGrid_Sorting(object sender, DataGridSortingEventArgs e)
        {
            var column = e.Column;
            var direction = column.SortDirection;
            // Assuming ascending sort if no direction is set
            bool isAscending = direction != ListSortDirection.Descending;

            // Update your ViewModel's properties
            ViewModel.SortedColumnName = column.Header.ToString();
            ViewModel.IsSortAscending = isAscending;
            if (direction != null)
            {
                ViewModel.SortDirection = column.SortDirection == ListSortDirection.Ascending ? "ASC" : "DESC";
            }
            
        }
    }
}
