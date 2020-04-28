using System;
using System.Collections.Generic;
using Xunit;

namespace Question2_CSV {
	public class CSVTable_Tests {

		private readonly CSVTable csvTable = new CSVTable();

		[Theory]
		[MemberData(nameof(TestData))]
		public void CSVParse_Test(List<List<string>> expected, string filename) {
			csvTable.Read(filename);
			Assert.True(simpleTableMatchesCSVTable(expected, csvTable));
		}

		private bool simpleTableMatchesCSVTable(List<List<string>> expected, CSVTable csvTable) {

			if (expected.Count != csvTable.Records.Count+1) {
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

				if (expectedRow.Count != csvTableRow.Count) {
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
			DistanceToRectCalculator.Rect test_rect = new DistanceToRectCalculator.Rect {
				minX = 100f,
				minY = 200f,
				maxX = 300f,
				maxY = 400f
			};

			// Test the 9 quadrants
			yield return new object[] { 222.27f, 1f, 1f, test_rect };
			yield return new object[] { 199f, 150f, 1f, test_rect };
			yield return new object[] { 727.74f, 1000f, 1f, test_rect };
			yield return new object[] { 99f, 1f, 250f, test_rect };
			yield return new object[] { 0f, 150f, 250f, test_rect };
			yield return new object[] { 700f, 1000f, 250f, test_rect };
			yield return new object[] { 608.11f, 1f, 1000f, test_rect };
			yield return new object[] { 600f, 150f, 1000f, test_rect };
			yield return new object[] { 921.95f, 1000f, 1000f, test_rect };

			// Test an "edge" case (lol)
			yield return new object[] { 0f, 100f, 250f, test_rect };

			// Test a "corner" case (lol!!)
			yield return new object[] { 0f, 100f, 200f, test_rect };

			// Test a point with a negative x coordinate
			yield return new object[] { 200f, -100f, 250f, test_rect };

			// Test a rectangle with negative positions
			DistanceToRectCalculator.Rect test_rect2 = new DistanceToRectCalculator.Rect {
				minX = -100f,
				minY = 200f,
				maxX = 300f,
				maxY = 400f
			};
			yield return new object[] { 900f, -1000f, 250f, test_rect2 };
		}

		[Fact]
		public void DistanceToRect_InvalidRect_Test() {
			// minX larger than maxX, will fail in the Assert in DistanceToRect
			DistanceToRectCalculator.Rect test_rect = new DistanceToRectCalculator.Rect {
				minX = 1000f,
				minY = 200f,
				maxX = 300f,
				maxY = 400f
			};

			Exception ex = Assert.Throws<ArgumentException>(() => DistanceToRectCalculator.DistanceToRect(1f, 1f, ref test_rect));
		}
	}
}
