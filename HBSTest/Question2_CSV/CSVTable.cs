using System;
using System.Collections.Generic;
using System.IO;

/*
 * NOTES:
 *
 * There is no universal standard CSV file format. Instead, different programs implement CSV files
 * in similar, but sometimes subtly different, ways. To make this implementation concrete, I decided
 * to reference the closest thing to a standard that there is for CSV, rfc4180:
 *
 * https://tools.ietf.org/html/rfc4180
 *
 * In particular, the above webpage has a very helpful BNF-like grammar that specifies the rfc4180
 * definition of a CSV file.
 *
 * Unfortunately, the rfc4180 grammar does not quite meet my needs. Specifically, 
 * 
 * 1) rfc4180 does not allow for both Windows and Unix style new lines!
 * 2) rfc4180 is more complicated than necessary: specifically,
 *    a) rfc4180 permits newlines inside of escaped fields
 *    b) rfc4180 makes headers optional (While this problem explicitly states: "The first row of each
 *       CSV file will contain the column headers that can be used ...")
 * 3) rfc4180 does not explicitly point out which non-terminals in the BNF grammar are the data fields 
 *
 * So I wrote a BNF which adresses these issues. It is this BNF grammar, which appears below,
 * that I implemented for this programming test. Note that in this grammar the following two
 * non-terminals are the data fields:
 *
 * escaped_data
 * non_escaped_data
 *
 * But being careful to replace pairs of embedded double-quotes with a single double-quote character
 * in escaped_data fields!
 *
 * 
 * Here's the BNF:
 *
 * file = header CRLF record *(CRLF record) [CRLF]
 * header = name *(COMMA name)
 * record = field *(COMMA field)
 * name = field
 * field = (escaped / non_escaped_data)
 * escaped = DQUOTE escaped_data DQUOTE
 * escaped_data = *(TEXTDATA / ',' / 2DQUOTE)
 * non_escaped_data = *TEXTDATA
 * DQUOTE =  '"'
 * CRLF = "\r\n" / '\n'
 * TEXTDATA = <any character not equal to ',', '"', '\n', or '\r'>
 *
 * 
 * A final note about non_escaped_data fields. Both rfc4180If any my implementation
 * do not permit the double-quote inside of a non-escaped field. If double-quote could occur
 * inside of a non-escaped field, then it would be easy to accidentally write an entry like:
 * 
 * a, "b", c
 *
 * In this example, the author probably meant the fields to be
 *
 * 'a', 'b', and 'c'
 *
 * but since the second entry (accidentally) starts with a space character we would instead
 * end up with:
 *
 * 'a', '"b"', and 'c'.
 *
 * So we just outlaw double-quotes in non-escaped fields.
 */

namespace Question2_CSV {

	public class CSVTable {

        private List<string> headers = new List<string>();
        private List<List<string>> records = new List<List<string>>();

        public List<string> Headers {
            get { return headers; }
			set { headers = value; }
        }
		public List<List<string>> Records {
            get { return records; }
			set { records = value; }
		}

		public void Read(string filename) {

            bool firstLine = true;

			using (StreamReader streamReader = new StreamReader(filename)) {
				while (streamReader.Peek() >= 0) {

                    // Note: StreamReader.ReadLine() handles both unix ("\n") and windows ("\r\n") style newlines
                    string line = streamReader.ReadLine();

                    List<string> record = parseRecord(line);

					if (firstLine) {
                        firstLine = false;
                        headers = record;
                        continue;
					}
					if (record.Count != headers.Count) {
                        throw new InvalidDataException($"Mismatched Records: Header contains {headers.Count} fields, but encountered a record with {record.Count} fields.");
					}
                    records.Add(record);
				}
			}

		}

		private List<string> parseRecord(string line) {
            List<string> dataFields = new List<string>();

            int index = 0;
            string dataField;

			// Since a record (i.e. line) could be blank, or could end with an empty field,
			// we keep parsing fields until index is *greater* than the line.Length
			while (index <= line.Length) {
				if (index == line.Length || line[index] != '"') {
                    (index, dataField) = parseNonEscapedField(line, index);
                } else {
                    (index, dataField) = parseEscapedField(line, index);
				}
				dataFields.Add(dataField);
			}

            return dataFields;
		}

        // Returns:
        // 1) The index of the start of the *next* dataField (or line.Length+1 if this is the final field)
        // 2) The current dataField (i.e. without leading or trailing '"' characters
        private (int, string) parseNonEscapedField(string line, int index) {
            int startOfDataIndex = index;
            int endOfDataIndex = -1;
            int indexOfStartOfNextField = -1;
            string data;

            while (true) {
                if (index >= line.Length || line[index] == ',') {
                    endOfDataIndex = index;
                    indexOfStartOfNextField = index + 1;
                    data = line.Substring(startOfDataIndex, endOfDataIndex - startOfDataIndex);
                    break;
                }

				if (line[index] == '"') {
					throw new InvalidDataException("A double-quote character ('\"') may not occur inside an unquoted field.");
				}
                index++;
            }

            return (indexOfStartOfNextField, data);
        }

        // Returns:
        // 1) The index of the start of the *next* dataField (or line.Length+1 if this is the final field)
        // 2) The current dataField (i.e. without leading or trailing '"' characters
        private (int, string) parseEscapedField(string line, int index) {
            int startOfDataIndex = -1;
            int endOfDataIndex = -1;
			int indexOfStartOfNextField = -1;
            string data;

            if (line[index] == '"') {
                index++;
                startOfDataIndex = index;
            } else {
                throw new Exception("Attempting to parse as string that does not begin with a double-quote character as if it were the start of an escaped data field.");
			}

			while (true) {
				if (index >= line.Length) {
                    throw new InvalidDataException("Invalid CSV file: Unmatched double-quote character.");
				}
				if (line[index]=='"') {
                    index++;
					if (index >= line.Length || line[index]==',') {
                        endOfDataIndex = index - 1; // minus 1 to account for '"'
                        indexOfStartOfNextField = index + 1;
                        data = line.Substring(startOfDataIndex, endOfDataIndex - startOfDataIndex);
                        data = data.Replace("\"\"", "\"");  // Replace escaped pairs of double-quotes with a single double-quote character
                        break;
					}
					if (line[index]!='"') {
                        throw new InvalidDataException("Invalid CSV file. Unescaped double-quote character inside of a data field.");
					}
				}
                index++;
			}

            return (indexOfStartOfNextField, data);
        }
	}

	//class CSV_Launcher {
	//	static void Main(string[] args) {
			
	//	}
	//}
}
