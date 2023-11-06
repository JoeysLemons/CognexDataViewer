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

namespace CognexDataViewer.Views.Pages
{
    /// <summary>
    /// Interaction logic for DataView.xaml
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
            Trace.WriteLine($"Selected Index: {selectedIndex}");
            var modalViewModel = new ViewModels.DataViewModalViewModel(ViewModel.DisplayTable, selectedIndex);
            DataViewModal modal = new DataViewModal(modalViewModel);
            UiWindow window = new UiWindow();
            window.Content = modal;
            window.DataContext = modalViewModel;
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            window.Show();
        }
    }
}
