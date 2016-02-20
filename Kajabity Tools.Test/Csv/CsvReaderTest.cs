/*
 * Copyright 2009-15 Williams Technologies Limited.
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
 * Kajbity is a trademark of Williams Technologies Limited.
 *
 * http://www.kajabity.com
 */

using System;
using System.IO;
using Kajabity.Tools.Test;
using NUnit.Framework;
using System.Reflection;

namespace Kajabity.Tools.Csv
{
    [TestFixture]
    public class CsvReaderTest : KajabityToolsTest
    {
        private string EmptyTestFile;
        private string SimpleTestFile;
        private string ThreeBlankLinesTestFile;
        private string EmptyFieldTestFile;
        private string FieldNamesTestFile;
        private string QuotedTestFile;
        private string QuotedLineBreaksTestFile;
        private string SpacesTestFile;
        private string DifferentNumberFieldsTestFile;

        private string MixedTestFile;

        // TODO: test reading error scenarios, 
        private string UnixLineEndsTestFile;
        private string ErrorQuotesTestFile;
        // TODO: test with alternate separator
        // TODO: Test reading fields to end of line with zero, one or more fields.


        /// <summary>
        /// The directory where a copy of the CSV test data input files are placed.
        /// </summary>
        protected static string CsvTestDataDirectory = "Cheese";

        /// <summary>
        /// The directory where a copy of the CSV test data input files are placed.
        /// </summary>
        protected static string CsvOutputDirectory;

        [OneTimeSetUp]
        public void SetUp()
        {
            Assembly assem = Assembly.GetExecutingAssembly();
            string assemblyPath = Directory.GetParent(assem.Location).FullName;
            string testDataDirectory = Path.Combine(assemblyPath, "Test Data");
            string outputDirectory = Path.Combine(assemblyPath, "Output");

            //Directory.CreateDirectory( OutputDirectory );

            CsvTestDataDirectory = Path.Combine(testDataDirectory, "Csv");
            CsvOutputDirectory = Path.Combine(outputDirectory, "Csv");

            if (!Directory.Exists(CsvOutputDirectory))
            {
                Console.WriteLine("Creating CSV output directory :" + CsvOutputDirectory);
                Directory.CreateDirectory(CsvOutputDirectory);
            }

            EmptyTestFile = Path.Combine(CsvTestDataDirectory, "empty.csv");
            SimpleTestFile = Path.Combine(CsvTestDataDirectory, "simple.csv");
            ThreeBlankLinesTestFile = Path.Combine(CsvTestDataDirectory, "three-blank-lines.csv");
            EmptyFieldTestFile = Path.Combine(CsvTestDataDirectory, "empty-field.csv");
            FieldNamesTestFile = Path.Combine(CsvTestDataDirectory, "field-names.csv");
            QuotedTestFile = Path.Combine(CsvTestDataDirectory, "quoted.csv");
            QuotedLineBreaksTestFile = Path.Combine(CsvTestDataDirectory, "quoted-linebreaks.csv");
            SpacesTestFile = Path.Combine(CsvTestDataDirectory, "spaces.csv");
            DifferentNumberFieldsTestFile = Path.Combine(CsvTestDataDirectory, "different-number-fields.csv");
            MixedTestFile = Path.Combine(CsvTestDataDirectory, "mixed.csv");
            UnixLineEndsTestFile = Path.Combine(CsvTestDataDirectory, "unix-line-ends.csv");
            ErrorQuotesTestFile = Path.Combine(CsvTestDataDirectory, "error-quotes.csv");
        }



        [Test]
        public void TestCsvEmptyFile()
        {
            FileStream fileStream = null;
            string filename = EmptyTestFile;

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
            string filename = SimpleTestFile;

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
                Assert.IsTrue(CompareStringArray(new string[] { "aaa", "bbb", "ccc" }, records[0]), "the first record");
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
            string filename = ThreeBlankLinesTestFile;

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
            string filename = EmptyFieldTestFile;

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
            string filename = QuotedTestFile;

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
            string filename = QuotedLineBreaksTestFile;

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
                Assert.IsTrue(CompareStringArray(new string[] 
                {
                    "A longer entry with some new" + Environment.NewLine + 
                    "lines" + Environment.NewLine + 
                    "even" + Environment.NewLine + 
                    "" + Environment.NewLine + 
                    "a blank one.",
                    "",
                    "Quotes" + Environment.NewLine + 
                    "\" and " + Environment.NewLine + 
                    "\"\t\"TABS " + Environment.NewLine + 
                    "AND,commas" }, records[index]), "contents of record " + (index + 1));
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
            string filename = FieldNamesTestFile;

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
            string filename = SpacesTestFile;

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
            string filename = DifferentNumberFieldsTestFile;

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
                Console.WriteLine("Loading " + MixedTestFile);
                fileStream = File.OpenRead(MixedTestFile);
                CsvReader reader = new CsvReader(fileStream);

                string[][] records = reader.ReadAll();
                int line = 0;

                foreach (string[] record in records)
                {
                    Console.WriteLine(++line + ":" + ToString(record));
                }
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
        public void TestCsvReadFieldAndRecord()
        {
            Console.WriteLine("Loading " + MixedTestFile);
            FileStream fileStream = null;
            try
            {
                fileStream = File.OpenRead(MixedTestFile);
                CsvReader reader = new CsvReader(fileStream);

                Console.WriteLine("Line 1, Field 1: \"" + reader.ReadField() + "\"");

                Console.WriteLine("Rest of Line 1: \"" + ToString(reader.ReadRecord()) + "\"");

                Console.WriteLine("Rest of File: ");

                string[][] records = reader.ReadAll();
                int line = 0;
                foreach (string[] record in records)
                {
                    Console.WriteLine(++line + ":" + ToString(record));
                }
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

    }
}
