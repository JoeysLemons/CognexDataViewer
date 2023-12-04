using CognexDataViewer.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Xml.Linq;

namespace CognexDataViewer.Helpers
{
    public class DatabaseUtils
    {
        public static string ConnectionString { get; set; } = @"Data Source=(localdb)\EdgeHistorian;Initial Catalog=EdgeHistorian;Integrated Security=True";

        /// <summary>
        /// Tests the DB connection
        /// </summary>
        /// <returns>True if connection was successful</returns>
        public static bool TestConnection()
        {
            bool connected = false;
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
                {
                    sqlConnection.Open();
                    connected = true;
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return connected;
        } 

        /// <summary>
        /// Fetches the ID associated with a job name
        /// </summary>
        /// <param name="JobName">Name of the job you want the ID of.</param>
        /// <returns>Integer representing the ID of the provided job. If no ID was found returns -1</returns>
        public static int GetJobId(string jobName)
        {
            try
            {
                int jobId = -1;
                using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
                {
                    sqlConnection.Open();
                    string queryString = @"SELECT id FROM Jobs WHERE Job_Name = @jobName";
                    using (SqlCommand command = new SqlCommand(queryString, sqlConnection))
                    {
                        command.Parameters.AddWithValue("@jobName", jobName);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                                jobId = reader.GetInt32(0);
                        }
                    }
                }
                return jobId;
            }
            catch (SqlException e)
            {
                Console.WriteLine($"Failed to return an ID from the provided Job Name. {e.Message}");
                throw;
            }
        }

        /// <summary>
        /// This method retrieves the names of all the tags associated with a given job
        /// </summary>
        /// <param name="jobId">The ID of the job you would like to parse tag names from</param>
        /// <returns>A list of strings containing the names of all the tags associated with a provided job If no tags were found returns empty list</returns>
        public static List<string> GetTagNames(int jobId)
        {
            try
            {
                List<string> tags = new List<string>();
                using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
                {
                    sqlConnection.Open();
                    string queryString = @"SELECT Name FROM MonitoredTags WHERE Job_id = @_jobId";
                    using (SqlCommand command = new SqlCommand(queryString, sqlConnection))
                    {
                        command.Parameters.AddWithValue("@_jobId", jobId);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                tags.Add(reader.GetString(0));
                            }
                        }
                    }
                }

                return tags;
            }
            catch (SqlException e)
            {
                Console.WriteLine($"Failed to get tag name {e.Message}");
                throw;
            }
        }

        public string GetTagMeasurementRange(string Name)
        {
            string measurement = string.Empty;

            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
                {
                    sqlConnection.Open();
                    DataTable results = new DataTable();


                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e);
                throw;
            }
            

            return measurement;
        }

        public static List<Tag> GetTagMeasurements(List<Tag> tags, DateTime startDate, DateTime endDate)
        {
            string query = @"SELECT Value, Timestamp FROM MonitoredTagValues WHERE Tag_id = @tag_id AND Timestamp BETWEEN @start_date AND @end_date;";
            foreach (Tag tag in tags)
            {
                try
                {
                    using SqlConnection sqlConnection = new SqlConnection(ConnectionString);
                    sqlConnection.Open();
                    using SqlCommand command = new SqlCommand(query, sqlConnection);
                    command.Parameters.AddWithValue("@tag_id", tag.Id);
                    command.Parameters.AddWithValue("@start_date", startDate);
                    command.Parameters.AddWithValue("@end_date", endDate);
                    using SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        try
                        {
                            string value = reader["Value"].ToString();
                            DateTime dateTime = (DateTime)reader["Timestamp"];
                            string timestamp = dateTime.ToString("M/d/yyyy h:mm:ss.fff tt");


                            TagMeasurement measurement = new TagMeasurement()
                            {
                                Value = value,
                                Timestamp = timestamp
                            };
                            tag.Measurements.Add(measurement);
                        }
                        catch (NullReferenceException e)
                        {
                            Console.Write(e);
                            throw;
                        }
                    }
                }
                catch (SqlException e)
                {
                    Console.WriteLine(e.Message);
                    throw;
                }
            }
            return tags;
        }

        public static List<Tag> GetTagMeasurements(List<Tag> tags)
        {
            string query = @"SELECT Value, Timestamp FROM MonitoredTagValues WHERE Tag_id = @tag_id";
            foreach (Tag tag in tags)
            {
                try
                {
                    using SqlConnection sqlConnection = new SqlConnection(ConnectionString);
                    sqlConnection.Open();
                    using SqlCommand command = new SqlCommand(query, sqlConnection);
                    command.Parameters.AddWithValue("@tag_id", tag.Id);
                    using SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        try
                        {
                            string value = reader["Value"].ToString();
                            DateTime dateTime = (DateTime)reader["Timestamp"];
                            string timestamp = dateTime.ToString("M/d/yyyy h:mm:ss.fff tt");


                            TagMeasurement measurement = new TagMeasurement()
                            {
                                Value = value,
                                Timestamp = timestamp
                            };
                            tag.Measurements.Add(measurement);
                        }
                        catch (NullReferenceException e)
                        {
                            Console.Write(e);
                            throw;
                        }
                    }
                }
                catch (SqlException e)
                {
                    Console.WriteLine(e.Message);
                    throw;
                }
            }
            return tags;
        }

        /// <summary>
        /// This method is used to get a list of the deafult tags associated with the deafult job. Will be used to retrieve the default data so that the data viewer is not empty on form load
        /// </summary>
        /// <param name="jobId">the ID of the deafult job associated with the customer</param>
        /// <returns>A list of all the tags associated with the default job. Will return an empty collection if no tags were found</returns>
        public static List<Tag> GetTagsFromJob(int jobId)
        {
            try
            {
                List<Tag> tags = new List<Tag>();
                string query = @"SELECT Name, Id FROM MonitoredTags WHERE Job_id = @job_id";  // Fixed the query syntax here

                using SqlConnection sqlConnection = new SqlConnection(ConnectionString);
                sqlConnection.Open();

                using SqlCommand command = new SqlCommand(query, sqlConnection);
                command.Parameters.AddWithValue("@job_id", jobId);  // Added @ before job_id

                using SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    try
                    {
                        string name = reader["Name"].ToString();
                        int id = int.Parse(reader["Id"].ToString());

                        Tag tag = new Tag(name) { Id = id };
                        tags.Add(tag);
                    }
                    catch (NullReferenceException e)
                    {
                        Console.Write(e);
                        throw;
                    }
                }

                return tags;
            }
            catch (SqlException e)
            {
                Console.WriteLine(e);
                throw;
            }
        }


        public static List<Tag> GetMonitoredTagsFromJob(int jobId)
        {
            try
            {
                List<Tag> tags = new List<Tag>();
                string query = @"SELECT Name, Id FROM MonitoredTags WHERE Job_id = @job_id AND Monitored = 1";  // Fixed the query syntax here

                using SqlConnection sqlConnection = new SqlConnection(ConnectionString);
                sqlConnection.Open();

                using SqlCommand command = new SqlCommand(query, sqlConnection);
                command.Parameters.AddWithValue("@job_id", jobId);  // Added @ before job_id

                using SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    try
                    {
                        string name = reader["Name"].ToString();
                        int id = int.Parse(reader["Id"].ToString());

                        Tag tag = new Tag(name) { Id = id };
                        tags.Add(tag);
                    }
                    catch (NullReferenceException e)
                    {
                        Console.Write(e);
                        throw;
                    }
                }

                return tags;
            }
            catch (SqlException e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        /// <summary>
        /// This method is used to get a default Job ID for when the data viewer page is first loaded and no options are selected
        /// This is used to help display a default set of data so that the data table is never empty.
        /// </summary>
        /// <param name="cameraId">The ID of the default camera associated with the customer</param>
        /// <returns>The id for the default job if one was found. If no job was found then a -1 is returned indicating an error</returns>
        public static int GetDefaultJob(int cameraId)
        {
            try
            {
                string query = @"SELECT TOP 1 * FROM Jobs WHERE Camera_id = @camera_id";
                using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
                {
                    sqlConnection.Open();
                    using (SqlCommand command = new SqlCommand(query, sqlConnection))
                    {
                        command.Parameters.AddWithValue("@camera_id", cameraId);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                                return reader.GetInt32(0);
                            else
                                return -1;
                        }
                    }
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e);
                return -1;
            }
        }


        /// <summary>
        /// This method is used to get a default camera ID for when the data viewer page is first loaded and no options are selected
        /// This is used to help display a default set of data so that the data table is never empty.
        /// </summary>
        /// <param name="pcId">The ID of the default PC associated with the customer</param>
        /// <returns>The id for the default camera if one was found. If no camera was found then a -1 is returned indicating an error</returns>
        public static int GetDefaultCamera(int pcId)
        {
            try
            {
                string query = @"SELECT TOP 1 * FROM Cameras WHERE PC_id = @pcID";
                using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
                {
                    sqlConnection.Open();
                    using (SqlCommand command = new SqlCommand(query, sqlConnection))
                    {
                        command.Parameters.AddWithValue("@pcID", pcId);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                                return reader.GetInt32(0);
                            else
                                return -1;
                        }
                    }
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e);
                return -1;
            }
        }
        /// <summary>
        /// This method is used to get a default Computer ID for when the data viewer page is first loaded and no options are selected
        /// This is used to help display a default set of data so that the data table is never empty
        /// </summary>
        /// <param name="customerId">The ID associated with the customer using the application</param>
        /// <returns>The ID for the default computer if one was found. If no computer is found then a -1 is returned indicating an error</returns>
        public static int GetDefaultComputer(int customerId)
        {
            try
            {
                string query = @"SELECT TOP 1 * FROM Computers WHERE Customer_id = @customer_id";
                using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
                {
                    sqlConnection.Open();
                    using (SqlCommand command = new SqlCommand(query, sqlConnection))
                    {
                        command.Parameters.AddWithValue("@customer_id", customerId);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                                return reader.GetInt32(0);
                            else
                                return -1;
                        }
                    }
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e);
                return -1;
            }
        }

        public static List<string> GetAllCameraNames(int customerId)
        {
            try
            {
                //Get all the computers associated with the customer
                //Create list to hold all computer IDs associated with the customer
                List<int> computerIds = new List<int>();
                //Create list to hold all camera names
                List<string> cameras = new List<string>();
                //Query designed to select all computer IDs associated with a customer
                string query = @"SELECT id FROM Computers WHERE Customer_id = @customer_id";
                using SqlConnection sqlConnection = new SqlConnection(ConnectionString);
                sqlConnection.Open();
                using SqlCommand command = new SqlCommand(query, sqlConnection);
                command.Parameters.AddWithValue("@customer_id", customerId);
                using SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    computerIds.Add(reader.GetInt32(0));
                }
                reader.Close();
                query = @"SELECT Name FROM Cameras WHERE PC_id = @pc_id";
                command.CommandText = query;
                
                foreach (int computer in computerIds)
                {
                    command.Parameters.AddWithValue("@pc_id", computer);
                    using SqlDataReader cameraReader = command.ExecuteReader();
                    while (cameraReader.Read())
                    {
                        cameras.Add(cameraReader.GetString(0));
                    }
                }
                return cameras;
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        public static List<string> GetAllJobNames(string camera)
        {
            try
            {
                List<string> jobs = new List<string>();
                string query = @"SELECT Job_Name FROM Jobs WHERE Camera_id = @camera_id";
                using SqlConnection sqlConnection = new SqlConnection(ConnectionString);
                sqlConnection.Open();
                using SqlCommand command = new SqlCommand(query, sqlConnection);
                int cameraId = GetCameraIdByName(camera);
                command.Parameters.AddWithValue("@camera_id", cameraId);
                using SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    jobs.Add(reader.GetString(0));
                }
                
                return jobs;
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }
        public static int GetCameraIdByName(string name)
        {
            using (SqlConnection SqlConnection = new SqlConnection(ConnectionString))
            {
                SqlConnection.Open();
                string selectCameraByEndpoint = "SELECT id FROM Cameras WHERE Name = @name";
                using (SqlCommand command = new SqlCommand(selectCameraByEndpoint, SqlConnection))
                {
                    command.Parameters.AddWithValue("@name", name);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        return reader.Read() ? reader.GetInt32(0) : -1;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the image taken 
        /// </summary>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        public static string GetAssociatedImage(DateTime timestamp, int tagId)
        {
            using (SqlConnection SqlConnection = new SqlConnection(ConnectionString))
            {
                SqlConnection.Open();
                string queryString = @"SELECT Associated_Image FROM MonitoredTagValues WHERE Timestamp = @timestamp AND Tag_id = @tag_id";
                using (SqlCommand command = new SqlCommand(queryString, SqlConnection))
                {
                    command.Parameters.AddWithValue("@timestamp", timestamp);
                    command.Parameters.AddWithValue("@tag_id", tagId);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        return reader.Read() ? reader.GetString(0) : string.Empty;
                    }
                }
            }
        }

        public static int GetTagIdByName(string name, int jobId)
        {
            using (SqlConnection SqlConnection = new SqlConnection(ConnectionString))
            {
                SqlConnection.Open();
                string selectCameraByEndpoint = "SELECT id FROM MonitoredTags WHERE Name = @name AND Job_id = @jobId";
                using (SqlCommand command = new SqlCommand(selectCameraByEndpoint, SqlConnection))
                {
                    command.Parameters.AddWithValue("@name", name);
                    command.Parameters.AddWithValue("@jobId", jobId);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        return reader.Read() ? reader.GetInt32(0) : -1;
                    }
                }
            }
        }
    }
}
