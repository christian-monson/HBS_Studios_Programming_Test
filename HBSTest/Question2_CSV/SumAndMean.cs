using System;
using System.Collections.Generic;

namespace Question2_CSV {
	public class SumAndMean {

		private List<CSVTable> csvTables = new List<CSVTable>();

		public SumAndMean() { }

		private void ReadCSVFiles(string[] csvFilenames) {
			foreach (string csvFilename in csvFilenames) {
				CSVTable csvTable = new CSVTable();
				csvTable.Read(csvFilename);
				csvTables.Add(csvTable);
			}
		}

		// The values in columnName must be parsable as either plain numbers
		// or US currency values
		public (decimal, decimal) GetSumAndMean(string columnName) {
			decimal sum = 0;
			decimal mean = 0;
			int count = 0;

			foreach (CSVTable csvTable in csvTables) {
				List<string> column = csvTable.GetColumn(columnName);
				sum += SumColumn(column);
				count += column.Count;
			}

			mean = sum / count;

			return (sum, mean);
		}

		private decimal SumColumn(List<string> column) {
			decimal sum = 0;
			foreach (string entry in column) {
				decimal entryAsDecimal;
				bool success = decimal.TryParse(entry, out entryAsDecimal);

				if (!success) {
					entryAsDecimal = decimal.Parse(entry, System.Globalization.NumberStyles.Currency);
				}

				sum += entryAsDecimal;
			}

			return sum;
		}

		static void Main(string[] args) {

			SumAndMean sumAndMean = new SumAndMean();
			sumAndMean.ReadCSVFiles(args);
			(decimal sum, decimal mean) = sumAndMean.GetSumAndMean("Cost, Initial");

			Console.WriteLine("The sum and mean of the 'Cost, Initial' columns in the CSV files passed on the command line are:");
			Console.WriteLine($"  Sum: {sum}");
			Console.WriteLine($" Mean: {mean}");
		}
	}
}
