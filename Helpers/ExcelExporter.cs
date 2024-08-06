using System;
using System.Collections.Generic;
using System.IO;
using ExcelExportetWpf.Models;
using OfficeOpenXml;

namespace ExcelExportetWpf.Helpers
{
	public class ExcelExporter
	{


		public static List<BakuBus> ReadBakuBusesFromExcel(string filePath)
		{
			var buses = new List<BakuBus>();

			// Make sure to use the EPPlus license context
			ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
			try
			{
				
				using (var package = new ExcelPackage(new FileInfo(filePath)))
				{
					var worksheet = package.Workbook.Worksheets[0]; // Assumes data is on the first sheet

					int columnCount = worksheet.Dimension.End.Column;

					// Expected number of columns
					const int expectedColumnCount = 4;


					if (columnCount != expectedColumnCount)
					{
						throw new FormatException($"The columns in file are not equal to expected. Expected {expectedColumnCount}, but got {columnCount}.");
					}

					// Start from row 2 if row 1 contains headers
					for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
					{
						var plate = string.IsNullOrWhiteSpace(worksheet.Cells[row, 1]?.Text) ? null : worksheet.Cells[row, 1].Text;
						var tagno = string.IsNullOrWhiteSpace(worksheet.Cells[row, 2]?.Text) ? null : worksheet.Cells[row, 2].Text;
						var vehicleid = string.IsNullOrWhiteSpace(worksheet.Cells[row, 3]?.Text) ? null : worksheet.Cells[row, 3].Text;
						var cardno = string.IsNullOrWhiteSpace(worksheet.Cells[row, 4]?.Text) ? null : worksheet.Cells[row, 4].Text;

						var bus = new BakuBus(plate!, tagno!, vehicleid!, cardno!);
						buses.Add(bus);
					}
				}
			}

			catch (FileNotFoundException ex)
			{
				throw new FileNotFoundException($"File error: {ex.Message}");
			}
			catch (IOException ex)
			{
				throw new IOException($"I/O error: {ex.Message}");
			}
			catch (FormatException ex)
			{
				throw new FormatException ($"Format exception: {ex.Message}");
			}
			catch (Exception ex)
			{
				throw new Exception($"An unexpected error occurred: {ex.Message}");
			}
			return buses;
		}
	}
}
