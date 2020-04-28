using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace Question2_CSV {
	public class CSVTable_Tests {

		private const string baseDir = "/Users/brackenfern/Christian/Professional/Interviews/HareBrainedSchemes/HBSTest/Question2_CSV/Tests/TestData";

		private readonly CSVTable csvTable = new CSVTable();

		[Theory]
		[MemberData(nameof(ValidTestData))]
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

		public static IEnumerable<object[]> ValidTestData() {

			string[][] simpleTable = { new string[] {"column1", "column2", "column3"},
									   new string[] {"a", "b", "c"},
									   new string[] {"d", "e", "f"},
									   new string[] {"g", "h", "i"}};
			yield return new object[] { simpleTable, Path.Combine(baseDir, "test_1_normal.csv") };
			yield return new object[] { simpleTable, Path.Combine(baseDir, "test_2_doubleQuotesFirstLabel.csv") };
			yield return new object[] { simpleTable, Path.Combine(baseDir, "test_3_doubleQuotesMiddleLabel.csv") };
			yield return new object[] { simpleTable, Path.Combine(baseDir, "test_4_doubleQuotesFinalLabel.csv") };
			yield return new object[] { simpleTable, Path.Combine(baseDir, "test_5_doubleQuotesFirstField.csv") };
			yield return new object[] { simpleTable, Path.Combine(baseDir, "test_6_doubleQuotesMiddleField.csv") };
			yield return new object[] { simpleTable, Path.Combine(baseDir, "test_7_doubleQuotesFinalField.csv") };
			yield return new object[] { simpleTable, Path.Combine(baseDir, "test_8_doubleQuotesEverywhere.csv") };
			yield return new object[] { simpleTable, Path.Combine(baseDir, "test_9_newlineAtEnd.csv") };

			simpleTable = new string[][]{ new string[] {"column1", "column,2", "column3"},
										  new string[] {"a", "b", "c"},
										  new string[] {"d", "e", "f"},
										  new string[] {"g", "h", "i"}};
			yield return new object[] { simpleTable, Path.Combine(baseDir, "test_10_commaInLabel.csv") };

			simpleTable = new string[][]{ new string[] {"column1", "column\"2", "column3"},
										  new string[] {"a", "b", "c"},
										  new string[] {"d", "e", "f"},
										  new string[] {"g", "h", "i"}};
			yield return new object[] { simpleTable, Path.Combine(baseDir, "test_11_doubleQuotesInLabel.csv") };

			simpleTable = new string[][]{ new string[] {"column 1", "column 2", "column 3"},
										  new string[] {"a", "b", "c"},
										  new string[] {"d", "e", "f"},
										  new string[] {"g", "h", "i"}};
			yield return new object[] { simpleTable, Path.Combine(baseDir, "test_12_spaceInHeader.csv") };

			simpleTable = new string[][]{ new string[] {"column1", "column2", "column3"},
										  new string[] {"a a", "b", "c"},
										  new string[] {"d", "e e", "f"},
										  new string[] {"g", "h", "i i"}};
			yield return new object[] { simpleTable, Path.Combine(baseDir, "test_13_spaceInField.csv") };

			simpleTable = new string[][]{ new string[] {"", "column2", "column3"},
										  new string[] {"a", "b", "c"},
										  new string[] {"d", "e", "f"},
										  new string[] {"g", "h", "i"}};
			yield return new object[] { simpleTable, Path.Combine(baseDir, "test_14_emptyHeaderField.csv") };

			simpleTable = new string[][]{ new string[] {"column1", "column2", "column3"},
										  new string[] {"", "b", "c"},
										  new string[] {"d", "e", "f"},
										  new string[] {"g", "h", "i"}};
			yield return new object[] { simpleTable, Path.Combine(baseDir, "test_15_emptyDataField.csv") };

			simpleTable = new string[][]{ new string[] {"column1", "column2", "column3"},
										  new string[] {"a", "b", "c"},
										  new string[] {"d", "", "f"},
										  new string[] {"g", "h", "i"}};
			yield return new object[] { simpleTable, Path.Combine(baseDir, "test_16_emptyMiddleDataField.csv") };

			simpleTable = new string[][]{ new string[] {"column1", "column2", "column3"},
										  new string[] {"a", "b", ""},
										  new string[] {"d", "e", "f"},
										  new string[] {"g", "h", "i"}};
			yield return new object[] { simpleTable, Path.Combine(baseDir, "test_17_emptyFinalDataField.csv") };

			simpleTable = new string[][]{ new string[] {""},
										  new string[] {""},
										  new string[] {""},
										  new string[] {""}};
			yield return new object[] { simpleTable, Path.Combine(baseDir, "test_18_emptyButWith4NewLines.csv") };

			simpleTable = new string[][]{ new string[] {"column1", " column2 ", "column3"},
										  new string[] {"a", "b", "c"},
										  new string[] {"d", "e", "f"},
										  new string[] {"g", "h", "i"}};
			yield return new object[] { simpleTable, Path.Combine(baseDir, "test_19_spaceBeforeAndAfterLabel.csv") };
		}

		[Theory]
		[MemberData(nameof(InvalidTestData))]
		public void CSVParse_Invalid_Test(string expectedMessage, string filename) {
			Exception ex = Assert.Throws<InvalidDataException>( () => csvTable.Read(filename));
			Assert.Equal(expectedMessage, ex.Message);
		}

		public static IEnumerable<object[]> InvalidTestData() {

			yield return new object[] { "A double-quote character ('\"') may not occur inside an unquoted field.",
										Path.Combine(baseDir, "test_illegal_1_doubleQuoteInUnescapedField.csv")};

			yield return new object[] { "Invalid CSV file. Unescaped double-quote character inside of a data field.",
										Path.Combine(baseDir, "test_illegal_2_doubleQuoteNotEscapedInEscapedField.csv")};

			yield return new object[] { "Invalid CSV file. Unescaped double-quote character inside of a data field.",
										Path.Combine(baseDir, "test_illegal_3_doubleQuotesNotImmediatelyFollowedByComma.csv")};

			yield return new object[] { "A double-quote character ('\"') may not occur inside an unquoted field.",
										Path.Combine(baseDir, "test_illegal_4_doubleQuotesNotImmediatelyPrecededByComma.csv")};

			yield return new object[] { "Invalid CSV file: Unmatched double-quote character.",
										Path.Combine(baseDir, "test_illegal_5_unterminatedEscapedField.csv")};

			yield return new object[] { "Mismatched Records: Header contains 3 fields, but encountered a record with 2 fields.",
										Path.Combine(baseDir, "test_illegal_6_wrongNumberOfFields.csv")};
		}
	}
}
