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
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Diagnostics;
using System.Threading;
using System.IO;

namespace WpfApp1
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		SqlConnection sqlConnection;
		SqlConnection sqlConnection_Edifecs;
	

		public MainWindow()
		{

			InitializeComponent();
			




		}



		private void SearchButton_Click(object sender, RoutedEventArgs e)
		{
			GuideLBX.Content = "In progress............";
			SearchBtn.IsEnabled = false;

			string conectionString = ConfigurationManager.ConnectionStrings["WpfApp1.Properties.Settings.masterConnectionString1"].ConnectionString;

			sqlConnection = new SqlConnection(conectionString);

			sqlConnection.Open();



			try
			{
				string query = " Select FileNm FROM EIDS_ENC.Encounters.ExtractSummaryLog where SysBatchLogKey = @BatchLogID";
				List<string> CSVLoad = new List<string>();
				CSVLoad.Clear();
				using (sqlConnection) {
					SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);

					sqlCommand.Parameters.AddWithValue("@BatchLogID", BatchLogTextBox.Text);


					var reader_Eids = sqlCommand.ExecuteReader();

				

						while (reader_Eids.Read())
						{

							CSVLoad.Add(reader_Eids.GetString(0) + ".DAT");

						}


					


					}

				sqlConnection.Close();
				if (CSVLoad.Count.Equals(0))
				{
					MessageBox.Show("CSV files not found.Check batchlog ID");
					MessageBoxLbl.Content = "File not found";
				}
				else
				{

					csvListBox.ItemsSource = CSVLoad;
					CsvFileValidation_ectracts(CSVLoad);

				}

			



			}


			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}



		}

		private void CSVStage_click(object sender, RoutedEventArgs e)
		{


			Process.Start(@"\\va01pstodfs003.corp.agp.ads\files\VA1\Private\ITS-TechServices\AMS\EnterpriseSvcs\Encounters\AF32634\Stage");
			LoadCSVFilesBTN.IsEnabled = true;
		}



		public void CsvFileValidation_ectracts( List<string> file)
		{
			
			MessageBoxLbl.Content = "In Validation............";




			try
			{
				string conectionString_Edifecs = ConfigurationManager.ConnectionStrings["WpfApp1.Properties.Settings.gbdrepoConnectionString_Edifecs"].ConnectionString;


				sqlConnection_Edifecs = new SqlConnection(conectionString_Edifecs);

				sqlConnection_Edifecs.Open();
				using (sqlConnection_Edifecs)
				{
					string query_Edifecs = "";
					string query_param = "";
					SqlCommand sqlCommand_Edifecs = new SqlCommand();


					for (int x = 0; x < file.Count; x++)
					{
						if (x != file.Count - 1)
						{
							query_param += $"@CSVFileName{x},";
						}
						else
						{
							query_param += $"@CSVFileName{x}";

						}
						sqlCommand_Edifecs.Parameters.AddWithValue($"CSVFileName{x}", file[x]);



					}

					query_Edifecs = $"select transmissionfilename from rrmencounter where transmissionfilename in ({query_param})";
					sqlCommand_Edifecs.CommandText = query_Edifecs;
					sqlCommand_Edifecs.Connection = sqlConnection_Edifecs;
					sqlCommand_Edifecs.CommandTimeout = 100;
					var reader = sqlCommand_Edifecs.ExecuteReader();


					List<string> DupFiles = new List<string>();
					CsvValidationLB.ItemsSource = DupFiles;

					////////////Test 
					while (DupFiles.Count > 0)
					{
						CsvValidationLB.Items.Remove(0);
					}
					while (reader.Read())
					{

						if (file.Contains(reader.GetString(0)))
						{

							file.Remove(reader.GetString(0));

							
							DupFiles.Add(reader.GetString(0));

						}
						

						
					}
					if (file.Count > 0)
					{

						csvListBox.ItemsSource = file;
						Movefile(file);

					}
					else
					{
						MessageBox.Show("All Files were perviously loaded.");

					}

				}
				sqlConnection_Edifecs.Close();
			}

			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString());

			}



		}
			public void Movefile(List<string> FileNames) {
			MessageBoxLbl.Content = "Moving files............";

			MessageBoxLbl.Content = FileNames[0];

			foreach (string file in FileNames)
			{


				//Change to the correct enviorment
				string FileToCopy = @"\\va01pstodfs003\Apps\eids_nonprod\FileTransfers\DEV\Archive\" + file ;
				String Destination = @"\\va01pstodfs003.corp.agp.ads\files\VA1\Private\ITS-TechServices\AMS\EnterpriseSvcs\Encounters\AF32634\" +file;
				System.IO.File.Copy(FileToCopy, Destination);

			}
			DefaultSetting();

		}

		public void DefaultSetting()
		{
			LoadCSVBTN.IsEnabled = false;
			LoadCSVFilesBTN.IsEnabled = false;

			SearchBtn.IsEnabled = true;
			BatchLogTextBox.IsEnabled = true;

			MessageBoxLbl.Content = "Ready for next batch ";


			GuideLBX.Content = "Complete:All validated failes have been moved Edifecs";


		}

		private void LoadCSVcheckBox_Checked(object sender, RoutedEventArgs e)
		{
			LoadCSVBTN.IsEnabled = true;
			SearchBtn.IsEnabled = false;
			BatchLogTextBox.IsEnabled = false;
			

		}


		private void LoadCSVcheckBox_UnChecked(object sender, RoutedEventArgs e)
		{
			LoadCSVBTN.IsEnabled = false;
			SearchBtn.IsEnabled = true;
			LoadCSVFilesBTN.IsEnabled = false;
			BatchLogTextBox.IsEnabled = true;



		}

		private void LoadCSVFilesBTN_Click(object sender, RoutedEventArgs e)
		{

			//Change to the correct eviroment 
			DirectoryInfo d = new DirectoryInfo(@"D:\Test");
			FileInfo[] Files = d.GetFiles("*.DAT"); 
			string str = "";
			foreach (FileInfo file in Files)
			{
				str = str + ", " + file.Name;
			}
		}
	}
}


			



	
