using System;
using System.Collections.Generic;
using Xunit;

namespace Question2_CSV {
	public class CSVTable_Tests {

		private const string baseDir = "/Users/brackenfern/Christian/Professional/Interviews/HareBrainedSchemes/HBSTest/Question2_CSV/Tests/TestData";

		private readonly CSVTable csvTable = new CSVTable();

		[Theory]
		[MemberData(nameof(TestData))]
		public void CSVParse_Test(string[][] expected, string filename) {
			csvTable.Read(filename);
			Assert.True(simpleTableMatchesCSVTable(expected, csvTable));
		}

		private bool simpleTableMatchesCSVTable(string[][] expected, CSVTable csvTable) {

			if (expected.Length != csvTable.Records.Count+1) {
				return false;
			}

			List<string> csvTableRow;
			int recordCounter = -1;
			bool firstRow = true;
			foreach (var expectedRow in expected) {
				if (firstRow) {
					firstRow = false;
					csvTableRow = csvTable.Headers;
				} else {
					recordCounter++;
					csvTableRow = csvTable.Records[recordCounter];
				}

				if (expectedRow.Length != csvTableRow.Count) {
					return false;
				}

				string csvTableEntry;
				int columnCounter = -1;
				foreach (string expectedEntry in expectedRow) {
					columnCounter++;
					csvTableEntry = csvTableRow[columnCounter];

					if (expectedEntry != csvTableEntry) {
						return false;
					}
				}
			}

			return true;
		}

		public static IEnumerable<object[]> TestData() {



			string[][] simpleTable = { new string[] {"column1", "column2", "column3"},
									   new string[] {"a", "b", "c"},
									   new string[] {"d", "e", "f"},
									   new string[] {"g", "h", "i"}};
			yield return new object[] { simpleTable, baseDir + "/test_normal.csv" };
		}

		//[Fact]
		//public void DistanceToRect_InvalidRect_Test() {
		//	// minX larger than maxX, will fail in the Assert in DistanceToRect
		//	DistanceToRectCalculator.Rect test_rect = new DistanceToRectCalculator.Rect {
		//		minX = 1000f,
		//		minY = 200f,
		//		maxX = 300f,
		//		maxY = 400f
		//	};

		//	Exception ex = Assert.Throws<ArgumentException>(() => DistanceToRectCalculator.DistanceToRect(1f, 1f, ref test_rect));
		//}
	}
}
