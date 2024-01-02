using CognexDataViewer.Helpers;
using CognexDataViewer.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using Wpf.Ui.Common.Interfaces;
using Wpf.Ui.Controls;
using Wpf.Ui.Mvvm.Contracts;
using Wpf.Ui.Mvvm.Services;

namespace CognexDataViewer.ViewModels
{
    public partial class DataViewModel : ObservableObject, INavigationAware
    {
        private bool _isInitialized = false;

        [ObservableProperty]
        public List<string> deviceList;

        [ObservableProperty]
        public List<string> jobList;

        private string selectedJob;

        public string SelectedJob
        {
            get { return selectedJob; }
            set 
            { 
                selectedJob = value;

                OnPropertyChanged(nameof(SelectedJob));
            }
        }


        private string selectedCamera = "No Camera Selected";

        public string SelectedCamera
        {
            get { return selectedCamera; }
            set 
            { 
                selectedCamera = value;
                JobList = PopulateJobDropdown();
                OnPropertyChanged(nameof(SelectedCamera));
            }
        }

        public string SortedColumnName { get; set; }
        public bool IsSortAscending { get; set; }
        private readonly ISnackbarService _snackbarService;

        public List<string> HeaderNames = new List<string>();

        public List<Tag> DisplayTags = new List<Tag>();

        private ObservableCollection<CameraData> sampleData = new ObservableCollection<CameraData>();

        public ObservableCollection<CameraData> SampleData
        {
            get { return sampleData; }
            set { sampleData = value; }
        }

        public List<TimestampGroupedTags> TagsGroupedByTime = new List<TimestampGroupedTags>();

        [ObservableProperty]
        public DataTable displayTable = new DataTable();

        private string startTime;

        public string StartTime
        {
            get { return startTime; }
            set 
            { 
                startTime = value;
                try
                {
                    StartDateTime = DateTime.Parse($"{StartDate} {StartTime}");
                }
                catch (Exception e)
                {
                    Trace.WriteLine("This is most likely being thrown from an incomplete time entry by a user. While user enters time if the time is not complete an error will arise. This is ok.");
                    Trace.WriteLine(e);
                }
                
                OnPropertyChanged();
            }
        }

        private string startDate;

        public string StartDate
        {
            get { return startDate; }
            set 
            {
                var indexOfSpace = value.IndexOf(' ');
                if (indexOfSpace != -1)
                    startDate = value.Substring(0, indexOfSpace);
                else
                    startDate = value;
                if (DateTime.TryParse($"{StartDate} {StartTime}", out DateTime result))
                {
                    StartDateTime = result;
                }
                OnPropertyChanged();
            }
        }

        private DateTime startDateTime;

        public DateTime StartDateTime
        {
            get 
            {
                return startDateTime; 
            }
            set
            {
                startDateTime = value;
                OnPropertyChanged();
            }
        }


        private string endTime;

        public string EndTime
        {
            get { return endTime; }
            set 
            {
                endTime = value;
                try
                {
                    EndDateTime = DateTime.Parse($"{EndDate} {EndTime}");
                }
                catch (Exception e)
                {
                    Trace.WriteLine("This is most likely being thrown from an incomplete time entry by a user. While user enters time if the time is not complete an error will arise. This is ok.");
                    Trace.WriteLine(e);
                }
                OnPropertyChanged();
            }
        }

        private string endDate;

        public string EndDate
        {
            get { return endDate; }
            set 
            {
                var indexOfSpace = value.IndexOf(' ');
                if (indexOfSpace != -1)
                    endDate = value.Substring(0, indexOfSpace);
                else
                    endDate = value;
                if (DateTime.TryParse($"{EndDate} {EndTime}", out DateTime result))
                {
                    EndDateTime = result;
                }
                OnPropertyChanged();
            }
        }

        private DateTime endDateTime;

        public DateTime EndDateTime
        {
            get 
            {
                return endDateTime;
            }
            set
            {
                endDateTime = value;
                OnPropertyChanged();
            }
        }

        private string errorMessage;

        public string ErrorMessage
        {
            get { return errorMessage; }
            set 
            {
                errorMessage = value;
                OnPropertyChanged();
                ErrorMessageChanged?.Invoke();
            }
        }

        public event Action ErrorMessageChanged;


        #region Commands

        [RelayCommand]
        public void ApplyViewerChanges()
        {
            try
            {
                if (startDateTime > endDateTime)
                    throw new ArgumentException("The start date must be earlier than the end date.");
                int jobID = DatabaseUtils.GetJobId(SelectedJob);
                DisplayTags = DatabaseUtils.GetMonitoredTagsFromJob(jobID);

            
                DisplayTags = DatabaseUtils.GetTagMeasurements(DisplayTags, StartDateTime, EndDateTime);
            }
            catch (ArgumentException e)
            {
                DisplayTags = DatabaseUtils.GetTagMeasurements(DisplayTags);
                ErrorMessage = e.Message;
            }
            catch (Exception e)
            {
                DisplayTags = DatabaseUtils.GetTagMeasurements(DisplayTags);
                //ErrorMessage = "Failed to parse start and end date time values. Please make sure that all boxes in the configuration section are filled out.";
            }
            DisplayTable = PopulateTable(DisplayTags);
        }

        public void UpdateDataViewer()
        {
            if (_isInitialized)
            {

            }
        }
        #endregion        

        public List<Tag> GetDefaultTags()
        {
            var tags = new List<Tag>();
            //DatabaseUtils.GetDefaultComputer(); //Commented out until Customer functionality is added to DB
            int cameraId = DatabaseUtils.GetDefaultCamera(10); //Hardcoding 10 for debug purposes eventually this value will come from the method above
            int jobId = DatabaseUtils.GetDefaultJob(cameraId);
            tags = DatabaseUtils.GetTagsFromJob(jobId);
            tags = DatabaseUtils.GetTagMeasurements(tags);
            return tags;
        }

        private int GetCameraId(string cameraName)
        {
            return DatabaseUtils.GetCameraIdByName(cameraName);
        }

        public static DataTable PopulateTable(List<Tag> tags)
        {
            DataTable dataTable = new DataTable();

            // Add "Timestamp" column
            dataTable.Columns.Add("Timestamp", typeof(string));

            // Add Tag columns
            foreach (var tag in tags)
            {
                dataTable.Columns.Add(tag.Name, typeof(string));
            }

            // Populate rows based on timestamps
            var allTimestamps = tags.SelectMany(tag => tag.Measurements.Select(m => m.Timestamp))
                                    .Distinct()
                                    .OrderBy(ts => DateTime.ParseExact(ts, "M/d/yyyy h:mm:ss.fff tt", CultureInfo.InvariantCulture))
                                    .ToList();

            foreach (var timestamp in allTimestamps)
            {
                DataRow row = dataTable.NewRow();
                row["Timestamp"] = timestamp;

                foreach (var tag in tags)
                {
                    var measurement = tag.Measurements.FirstOrDefault(m => m.Timestamp == timestamp);
                    row[tag.Name] = measurement != null ? measurement.Value : null;
                }

                dataTable.Rows.Add(row);
            }

            return dataTable;
        }


        private List<string> PopulateCameraDropdown()
        {
            List<string> cameras = new List<string>();
            cameras = DatabaseUtils.GetAllCameraNames(1);
            return cameras;
        }

        private List<string> PopulateJobDropdown()
        {
            List<string> jobs = new List<string>();
            jobs = DatabaseUtils.GetAllJobNames(SelectedCamera);
            return jobs;
        }


        public List<string> GetHeaderNames()
        {
            List<string> names = new List<string>();
            //Retrieve ID of the job we want to parse
            int jobId = DatabaseUtils.GetJobId(SelectedJob);
            //Get a list of all the tag names associated with that job
            names = DatabaseUtils.GetTagNames(jobId);

            return names;
        }

        public void OnNavigatedTo()
        {
            if (!_isInitialized)
                InitializeViewModel();
        }

        public void OnNavigatedFrom()
        {
        }

        private void InitializeViewModel()
        {
            DisplayTags = GetDefaultTags();
            DeviceList = PopulateCameraDropdown();
            DisplayTable = PopulateTable(DisplayTags);
            _isInitialized = true;
        }

        public DataViewModel()
        {
           
        }
    }
}
