﻿using System;
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
			string conectionString = ConfigurationManager.ConnectionStrings["WpfApp1.Properties.Settings.masterConnectionString1"].ConnectionString;

			string conectionString_Edifecs = ConfigurationManager.ConnectionStrings["WpfApp1.Properties.Settings.gbdrepoConnectionString_Edifecs"].ConnectionString;

		

		sqlConnection = new SqlConnection(conectionString);
			sqlConnection_Edifecs = new SqlConnection(conectionString_Edifecs);




		}



		private void SearchButton_Click(object sender, RoutedEventArgs e)
		{



			MessageBoxLbl.Content = "In progress............";
			sqlConnection.Open();



			try
			{
				string query = " Select FileNm FROM EIDS_ENC.Encounters.ExtractSummaryLog where SysBatchLogKey = @BatchLogID";
				List<string> CSVLoad = new List<string>();

				using (sqlConnection) {
					SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);

					sqlCommand.Parameters.AddWithValue("@BatchLogID", BatchLogTextBox.Text);


					var reader_Eids = sqlCommand.ExecuteReader();

				

						while (reader_Eids.Read())
						{

							CSVLoad.Add(reader_Eids.GetString(0));

						}


					


					}
			
					
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

		private void LoadCSV_click(object sender, RoutedEventArgs e)
		{


			Process.Start(@"\\va01pstodfs003.corp.agp.ads\files\VA1\Private\ITS-TechServices\AMS\EnterpriseSvcs\Encounters\AF32634\Stage");

		}



		public void CsvFileValidation_ectracts( List<string> file)
		{
			
			try
			{
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



					while (reader.Read())
					{

						if (file.Contains(reader.GetString(0)))
						{



							file.Remove(reader.GetString(0));
						}


					}
					if (file.Count > 0)
					{
						Movefile(file);

					}
					else
					{
						///write somthing for the user. 
					}

				}
			}

			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString());

			}



		}
			public void Movefile(List<string> FileNames) {

			MessageBoxLbl.Content = FileNames[0];




			//DefaultSetting();

		}

		public void DefaultSetting()
		{
			LoadCSVBTN.IsEnabled = false;
			SearchBtn.IsEnabled = true;
			//MessageBoxLbl.Content = "Ready for next batch 1";
			sqlConnection.Close();
			sqlConnection_Edifecs.Close();
		}

		private void LoadCSVcheckBox_Checked(object sender, RoutedEventArgs e)
		{
			LoadCSVBTN.IsEnabled = true;
			SearchBtn.IsEnabled = false;
			

		}


		private void LoadCSVcheckBox_UnChecked(object sender, RoutedEventArgs e)
		{
			LoadCSVBTN.IsEnabled = false;
			SearchBtn.IsEnabled = true;


		}
		
	}
}


			



	
