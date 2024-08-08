using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using EFCore.BulkExtensions;
using ExcelExportetWpf.Helpers;
using ExcelExportetWpf.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;

namespace ExcelExportetWpf
{

	public partial class MainWindow : Window, INotifyPropertyChanged
	{
		private ObservableCollection<BakuBus> _buses = new ObservableCollection<BakuBus>();

		public ObservableCollection<BakuBus> Buses
		{
			get => _buses;
			set
			{
				if (_buses != value)
				{
					_buses = value;
					OnPropertyChanged();
				}
			}
		}

		private readonly DispatcherTimer _timer;

		// Property Changed Event
		public event PropertyChangedEventHandler? PropertyChanged;
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null!)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		public MainWindow()
		{
			DataContext = this;
			InitializeComponent();
			FilePathTextBox.Text = @"D:\VS\ExcelExportetWpf\Assets\filtered_output_new.xlsx";

			ExportDataButton_Click(this, null!);

			_timer = new DispatcherTimer
			{
				Interval = TimeSpan.FromSeconds(5)
			};

			_timer.Tick += Timer_Tick!;
			_timer.Start();
		}


		static async Task ClearAllBusesAsync(BakuBusContext context)
		{
			try
			{
				var allBuses = await context.BakuBuses!.ToListAsync(); // Fetch all records
				await context.BulkDeleteAsync(allBuses);

			}
			catch (Exception ex)
			{

				Console.WriteLine(ex.Message);
			}
		}

		private void BrowseButton_Click(object sender, RoutedEventArgs e)
		{
			var openFileDialog = new OpenFileDialog
			{
				Filter = "Excel Files (*.xlsx)|*.xlsx",
				Title = "Select an Excel File"
			};

			if (openFileDialog.ShowDialog() == true)
			{
				FilePathTextBox.Text = openFileDialog.FileName;
			}
			ExportDataButton.Focus();
		}

		private async void ExportDataButton_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				string filePath = FilePathTextBox.Text;
				if (!File.Exists(filePath))
				{
					FilePathTextBox.Clear();
					FilePathTextBox.Focus();
					throw new FileNotFoundException("Current file doesn't exist 😕. Please, try again");
				}

				ExportDataButton.IsEnabled = false;
				StatusTextBlock.Text = "Exporting data, please wait ... 😴";

				try
				{
					var count = await ImportDataAsync(filePath);
					StatusTextBlock.Text = $"Data imported successfully 🧐 ({count} entities affected)";
				}
				catch (Exception ex)
				{
					StatusTextBlock.Text = ex.Message;
				}

			}

			catch (Exception ex)
			{
				StatusTextBlock.Text = ex.Message;
			}
			finally
			{
				ExportDataButton.IsEnabled = true;
			}
		}

		public async Task<int> ImportDataAsync(string filePath)
		{
			try
			{

				string excelFilePath = FilePathTextBox.Text;

				var busList = ExcelExporter.ReadBakuBusesFromExcel(filePath);

				using var context = new BakuBusContext();
				await ClearAllBusesAsync(context);
				await context.AddRangeAsync(busList);
				await context.SaveChangesAsync();

				// Update the collection on the UI thread
				Application.Current.Dispatcher.Invoke(() =>
				{
					Buses.Clear();
					foreach (var bus in busList)
					{
						Buses.Add(bus);
					}
				});
				return busList.Count;
			}
			catch (Exception ex)
			{
				throw new ApplicationException($"Error importing data {ex.Message}");
			}
		}
		private void Timer_Tick(object sender, EventArgs e)
		{
			ExportDataButton_Click(this, null!);
		}
	}
}