/*
 * Copyright 2009-14 Simon J. Williams.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
 * http://www.kajabity.com
 */

using System;
using System.IO;
using Kajabity.Tools.Test;
using NUnit.Framework;

namespace Kajabity.Tools.Csv
{
	[TestFixture]
	public class CsvReaderTest : KajabityToolsTest
	{
        private const string EmptyTestFile = CsvTestDataDirectory + "empty.csv";
        private const string SimpleTestFile = CsvTestDataDirectory + "simple.csv";
        private const string ThreeBlankLinesTestFile = CsvTestDataDirectory + "three-blank-lines.csv";
        private const string EmptyFieldTestFile = CsvTestDataDirectory + "empty-field.csv";
        private const string FieldNamesTestFile = CsvTestDataDirectory + "field-names.csv";
        private const string QuotedTestFile = CsvTestDataDirectory + "quoted.csv";
        private const string QuotedLineBreaksTestFile = CsvTestDataDirectory + "quoted-linebreaks.csv";
        private const string SpacesTestFile = CsvTestDataDirectory + "spaces.csv";
        private const string DifferentNumberFieldsTestFile = CsvTestDataDirectory + "different-number-fields.csv";

        private const string MixedTestFile = CsvTestDataDirectory + "mixed.csv";
        
        // TODO: test reading error scenarios, 
        private const string UnixLineEndsTestFile = CsvTestDataDirectory + "unix-line-ends.csv";
        private const string ErrorQuotesTestFile = CsvTestDataDirectory + "error-quotes.csv";
		// TODO: test with alternate separator
		// TODO: Test reading fields to end of line with zero, one or more fields.

        [Test]
        public void TestCsvEmptyFile()
        {
            FileStream fileStream = null;
            const string filename = EmptyTestFile;

            try
            {
                Console.WriteLine("Loading " + filename);
                fileStream = File.OpenRead(filename);
                CsvReader reader = new CsvReader(fileStream);

                string[][] records = reader.ReadAll();
                int line = 0;

                foreach (string[] record in records)
                {
                    Console.WriteLine(++line + ":" + ToString(record));
                }

                //TODO: Not sure here - should there be zero records or one empty record with an empty field?
                //Assert.IsTrue(records.Length == 0, "Should be no records in " + filename);
                Assert.IsTrue(records.Length == 1, "Wrong number of record in " + filename);
                Assert.IsTrue(records[0].Length == 1, "Wrong number of items on the first record");
                Assert.IsTrue(records[0][0].Length == 0, "Should be an empty string");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
            finally
            {
                if (fileStream != null)
                {
                    fileStream.Close();
                }
            }
        }

        [Test]
        public void TestCsvSimpleFile()
        {
            FileStream fileStream = null;
            const string filename = SimpleTestFile;

            try
            {
                Console.WriteLine("Loading " + filename);
                fileStream = File.OpenRead(filename);
                CsvReader reader = new CsvReader(fileStream);

                string[][] records = reader.ReadAll();
                int line = 0;

                foreach (string[] record in records)
                {
                    Console.WriteLine(++line + ":" + ToString(record));
                }

                Assert.IsTrue(records.Length == 2, "Wrong number of records in " + filename);
                Assert.IsTrue(CompareStringArray( new string[] {"aaa","bbb","ccc"}, records[0]), "the first record");
                Assert.IsTrue(CompareStringArray(new string[] { "xxx", "yyy", "zzz" }, records[1]), "the second record");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
            finally
            {
                if (fileStream != null)
                {
                    fileStream.Close();
                }
            }
        }

        [Test]
        public void TestCsvThreeBlankLinesFile()
        {
            FileStream fileStream = null;
            const string filename = ThreeBlankLinesTestFile;

            try
            {
                Console.WriteLine("Loading " + filename);
                fileStream = File.OpenRead(filename);
                CsvReader reader = new CsvReader(fileStream);

                string[][] records = reader.ReadAll();
                int line = 0;

                foreach (string[] record in records)
                {
                    Console.WriteLine(++line + ":" + ToString(record));
                }

                Assert.IsTrue(records.Length == 3, "Wrong number of records in " + filename);
                Assert.IsTrue(records[0].Length == 1, "Wrong number of items on the first record");
                Assert.IsTrue(records[0][0].Length == 0, "Should be an empty string");
                Assert.IsTrue(records[1].Length == 1, "Wrong number of items on the second record");
                Assert.IsTrue(records[1][0].Length == 0, "Should be an empty string");
                Assert.IsTrue(records[2].Length == 1, "Wrong number of items on the third record");
                Assert.IsTrue(records[2][0].Length == 0, "Should be an empty string");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
            finally
            {
                if (fileStream != null)
                {
                    fileStream.Close();
                }
            }
        }

        [Test]
        public void TestCsvEmptyFieldFile()
        {
            FileStream fileStream = null;
            const string filename = EmptyFieldTestFile;

            try
            {
                Console.WriteLine("Loading " + filename);
                fileStream = File.OpenRead(filename);
                CsvReader reader = new CsvReader(fileStream);

                string[][] records = reader.ReadAll();
                int line = 0;

                foreach (string[] record in records)
                {
                    Console.WriteLine(++line + ":" + ToString(record));
                }

                Assert.IsTrue(records.Length == 4, "Wrong number of records in " + filename);

                int index = 0;
                Assert.IsTrue(records[index].Length == 3, "Wrong number of items on record " + (index + 1));
                Assert.IsTrue(CompareStringArray(new string[] { "aaa", "bbb", "ccc" }, records[index]), "contents of record " + (index + 1));

                index++;
                Assert.IsTrue(records[index].Length == 3, "Wrong number of items on record " + (index + 1));
                Assert.IsTrue(CompareStringArray(new string[] { "", "eee", "fff" }, records[index]), "contents of record " + (index + 1));

                index++;
                Assert.IsTrue(records[index].Length == 3, "Wrong number of items on record " + (index + 1));
                Assert.IsTrue(CompareStringArray(new string[] { "ggg", "", "jjj" }, records[index]), "contents of record " + (index + 1));

                index++;
                Assert.IsTrue(records[index].Length == 3, "Wrong number of items on record " + (index + 1));
                Assert.IsTrue(CompareStringArray(new string[] { "xxx", "yyy", "" }, records[index]), "contents of record " + (index + 1));
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
            finally
            {
                if (fileStream != null)
                {
                    fileStream.Close();
                }
            }
        }

        [Test]
        public void TestCsvQuotedFile()
        {
            FileStream fileStream = null;
            const string filename = QuotedTestFile;

            try
            {
                Console.WriteLine("Loading " + filename);
                fileStream = File.OpenRead(filename);
                CsvReader reader = new CsvReader(fileStream);

                string[][] records = reader.ReadAll();
                int line = 0;

                foreach (string[] record in records)
                {
                    Console.WriteLine(++line + ":" + ToString(record));
                }

                Assert.IsTrue(records.Length == 2, "Wrong number of records in " + filename);

                int index = 0;
                Assert.IsTrue(records[index].Length == 2, "Wrong number of items on record " + (index + 1));
                Assert.IsTrue(CompareStringArray(new string[] { "2lines, 2 fields, With, commas", "With \"Quotes\"" }, records[index]), "contents of record " + (index + 1));

                index++;
                Assert.IsTrue(records[index].Length == 2, "Wrong number of items on record " + (index + 1));
                Assert.IsTrue(CompareStringArray(new string[] { "With	Tabs", "Quotes\" and \"	\"TABS AND,commas" }, records[index]), "contents of record " + (index + 1));
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
            finally
            {
                if (fileStream != null)
                {
                    fileStream.Close();
                }
            }
        }

        [Test]
        public void TestCsvQuotedLineBreaksFile()
        {
            FileStream fileStream = null;
            const string filename = QuotedLineBreaksTestFile;

            try
            {
                Console.WriteLine("Loading " + filename);
                fileStream = File.OpenRead(filename);
                CsvReader reader = new CsvReader(fileStream);

                string[][] records = reader.ReadAll();
                int line = 0;

                foreach (string[] record in records)
                {
                    Console.WriteLine(++line + ":" + ToString(record));
                }

                Assert.IsTrue(records.Length == 1, "Wrong number of records in " + filename);

                int index = 0;
                Assert.IsTrue(records[index].Length == 3, "Wrong number of items on record " + (index + 1));
                Assert.IsTrue(CompareStringArray(new string[] { "A longer entry with some new\r\nlines\r\neven\r\n\r\na blank one.", "",
                    "Quotes\r\n\" and \r\n\"\t\"TABS \r\nAND,commas" }, records[index]), "contents of record " + (index + 1));
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
            finally
            {
                if (fileStream != null)
                {
                    fileStream.Close();
                }
            }
        }

        [Test]
        public void TestCsvFieldNamesFile()
        {
            FileStream fileStream = null;
            const string filename = FieldNamesTestFile;

            try
            {
                Console.WriteLine("Loading " + filename);
                fileStream = File.OpenRead(filename);
                CsvReader reader = new CsvReader(fileStream);

                string[][] records = reader.ReadAll();
                int line = 0;

                foreach (string[] record in records)
                {
                    Console.WriteLine(++line + ":" + ToString(record));
                }

                Assert.IsTrue(records.Length == 3, "Wrong number of records in " + filename);
                Assert.IsTrue(CompareStringArray(new string[] { "Title", "Forename", "Last Name", "Age" }, records[0]), "the first record");
                Assert.IsTrue(CompareStringArray(new string[] { "Mr.", "John", "Smith", "21" }, records[1]), "the second record");
                Assert.IsTrue(CompareStringArray(new string[] { "Mrs.", "Jane", "Doe-Jones", "42" }, records[2]), "the third record");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
            finally
            {
                if (fileStream != null)
                {
                    fileStream.Close();
                }
            }
        }

        [Test]
        public void TestCsvSpacesFile()
        {
            FileStream fileStream = null;
            const string filename = SpacesTestFile;

            try
            {
                Console.WriteLine("Loading " + filename);
                fileStream = File.OpenRead(filename);
                CsvReader reader = new CsvReader(fileStream);

                string[][] records = reader.ReadAll();
                int line = 0;

                foreach (string[] record in records)
                {
                    Console.WriteLine(++line + ":" + ToString(record));
                }

                Assert.IsTrue(records.Length == 1, "Wrong number of records in " + filename);
                Assert.IsTrue(CompareStringArray(new string[] { "trailing ", " leading", " both " }, records[0]), "the first record");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
            finally
            {
                if (fileStream != null)
                {
                    fileStream.Close();
                }
            }
        }

        [Test]
        public void TestCsvDifferentNumberFieldsFile()
        {
            FileStream fileStream = null;
            const string filename = DifferentNumberFieldsTestFile;

            try
            {
                Console.WriteLine("Loading " + filename);
                fileStream = File.OpenRead(filename);
                CsvReader reader = new CsvReader(fileStream);

                string[][] records = reader.ReadAll();
                int line = 0;

                foreach (string[] record in records)
                {
                    Console.WriteLine(++line + ":" + ToString(record));
                }

                Assert.IsTrue(records.Length == 4, "Wrong number of records in " + filename);

                int index = 0;
                Assert.IsTrue(records[index].Length == 3, "Wrong number of items on record " + (index + 1));
                Assert.IsTrue(CompareStringArray(new string[] { "A", "B", "C" }, records[index]), "contents of record " + (index + 1));

                index++;
                Assert.IsTrue(records[index].Length == 4, "Wrong number of items on record " + (index + 1));
                Assert.IsTrue(CompareStringArray(new string[] { "a", "b", "c", "d" }, records[index]), "contents of record " + (index + 1));

                index++;
                Assert.IsTrue(records[index].Length == 2, "Wrong number of items on record " + (index + 1));
                Assert.IsTrue(CompareStringArray(new string[] { "9", "8" }, records[index]), "contents of record " + (index + 1));

                index++;
                Assert.IsTrue(records[index].Length == 5, "Wrong number of items on record " + (index + 1));
                Assert.IsTrue(CompareStringArray(new string[] { "1", "2", "3", "4", "5" }, records[index]), "contents of record " + (index + 1));
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
            finally
            {
                if (fileStream != null)
                {
                    fileStream.Close();
                }
            }
        }

        [Test]
		public void TestCsvReadAll()
		{
			FileStream fileStream = null;
			try
			{
                Console.WriteLine( "Loading " + MixedTestFile );
                fileStream = File.OpenRead( MixedTestFile );
				CsvReader reader = new CsvReader( fileStream );

				string [][] records = reader.ReadAll();
				int line = 0;

				foreach( string[] record in records )
				{
					Console.WriteLine( ++line + ":" + ToString( record ) );
				}
			}
            catch (Exception ex)
            {
                Assert.Fail( ex.Message );
            }
            finally
			{
				if( fileStream != null )
				{
					fileStream.Close();
				}
			}
		}

		[Test]
        public void TestCsvReadFieldAndRecord()
		{
            Console.WriteLine("Loading " + MixedTestFile );
			FileStream fileStream = null;
			try
			{
                fileStream = File.OpenRead(MixedTestFile);
				CsvReader reader = new CsvReader( fileStream );

				Console.WriteLine( "Line 1, Field 1: \"" + reader.ReadField() + "\"" );

				Console.WriteLine( "Rest of Line 1: \"" + ToString( reader.ReadRecord() ) + "\"" );

				Console.WriteLine( "Rest of File: " );

				string [][] records = reader.ReadAll();
				int line = 0;
				foreach( string[] record in records )
				{
					Console.WriteLine( ++line + ":" + ToString( record ) );
				}
			}
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
            finally
			{
				if( fileStream != null )
				{
					fileStream.Close();
				}
			}
		}

	}
}
