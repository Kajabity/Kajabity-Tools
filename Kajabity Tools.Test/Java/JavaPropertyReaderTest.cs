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
using System.Text;

namespace Kajabity.Tools.Java
{
    [TestFixture]
    public class JavaPropertyReaderTest : KajabityToolsTest
    {
        private string EmptyTestFile;
        private string BlankTestFile;
        private string CommentsTestFile;
        private string DuplicateTestFile;
        private string LineBreaksTestFile;
        private string MixedTestFile;
        private string SeparatorsTestFile;
        private string SpecialCharactersTestFile;
        private string NonAciiSymbolsUtf8TestFile;
        private string NonAsciiSymbolsNativeToAsciiTestFile;

        /// <summary>
        /// The directory where a copy of the Java test data input files are placed.
        /// </summary>
        protected static string JavaTestDataDirectory;

        /// <summary>
        /// The directory where a copy of the Java test data input files are placed.
        /// </summary>
        protected static string JavaOutputDirectory;

        [OneTimeSetUp]
        public void SetUp()
        {
            Assembly assem = Assembly.GetExecutingAssembly();
            string assemblyPath = Directory.GetParent(assem.Location).FullName;
            string testDataDirectory = Path.Combine(assemblyPath, "Test Data");
            string outputDirectory = Path.Combine(assemblyPath, "Output");

            JavaTestDataDirectory = Path.Combine(testDataDirectory, "Java");
            JavaOutputDirectory = Path.Combine(outputDirectory, "Java");

            if (!Directory.Exists(JavaOutputDirectory))
            {
                Console.WriteLine("Creating Java Properties output directory :" + JavaOutputDirectory);
                Directory.CreateDirectory(JavaOutputDirectory);
            }

            EmptyTestFile = Path.Combine(JavaTestDataDirectory, "empty.properties");
            BlankTestFile = Path.Combine(JavaTestDataDirectory, "blank.properties");
            CommentsTestFile = Path.Combine(JavaTestDataDirectory, "comments.properties");
            DuplicateTestFile = Path.Combine(JavaTestDataDirectory, "duplicate.properties");
            LineBreaksTestFile = Path.Combine(JavaTestDataDirectory, "line-breaks.properties");
            MixedTestFile = Path.Combine(JavaTestDataDirectory, "mixed.properties");
            SeparatorsTestFile = Path.Combine(JavaTestDataDirectory, "separators.properties");
            SpecialCharactersTestFile = Path.Combine(JavaTestDataDirectory, "special-characters.properties");
            NonAciiSymbolsUtf8TestFile = Path.Combine(JavaTestDataDirectory, "non-ascii-symbols-utf8.properties");
            NonAsciiSymbolsNativeToAsciiTestFile = Path.Combine(JavaTestDataDirectory, "non-ascii-symbols-native2ascii.properties");
        }

        [Test]
        public void TestEmptyFile()
        {
            FileStream fileStream = null;
            try
            {
                Console.WriteLine("Loading " + EmptyTestFile);

                fileStream = new FileStream(EmptyTestFile, FileMode.Open);
                JavaProperties properties = new JavaProperties();
                properties.Load(fileStream);
                fileStream.Close();

                Assert.IsEmpty(properties);
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

        /// <summary>
        /// Check that empty lines are not interpreted as a value also.
        /// </summary>
        [Test]
        public void TestBlankFile()
        {
            FileStream fileStream = null;
            try
            {
                Console.WriteLine("Loading " + BlankTestFile);

                fileStream = new FileStream(BlankTestFile, FileMode.Open);
                JavaProperties properties = new JavaProperties();
                properties.Load(fileStream);
                fileStream.Close();

                Assert.IsEmpty(properties);
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
        public void TestComments()
        {
            FileStream fileStream = null;
            try
            {
                Console.WriteLine("Loading " + CommentsTestFile);

                fileStream = new FileStream(CommentsTestFile, FileMode.Open);
                JavaProperties properties = new JavaProperties();
                properties.Load(fileStream);
                fileStream.Close();

                //Assert.Equals( properties.Count, 3 );
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
        public void TestDuplicates()
        {
            FileStream fileStream = null;
            try
            {
                Console.WriteLine("Loading " + DuplicateTestFile);

                fileStream = new FileStream(DuplicateTestFile, FileMode.Open);
                JavaProperties properties = new JavaProperties();
                properties.Load(fileStream);
                fileStream.Close();

                Assert.AreEqual(properties.Count, 1);
                Assert.IsTrue("c".Equals(properties.GetProperty("a")));
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
        public void TestLineBreaks()
        {
            FileStream fileStream = null;
            try
            {
                Console.WriteLine("Loading " + LineBreaksTestFile);

                fileStream = new FileStream(LineBreaksTestFile, FileMode.Open);
                JavaProperties properties = new JavaProperties();
                properties.Load(fileStream);
                fileStream.Close();

                //Assert.Equals( properties.Count, 3 );
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
        public void TestSeparators()
        {
            FileStream fileStream = null;
            try
            {
                Console.WriteLine("Loading " + SeparatorsTestFile);

                fileStream = new FileStream(SeparatorsTestFile, FileMode.Open);
                JavaProperties properties = new JavaProperties();
                properties.Load(fileStream);
                fileStream.Close();

                //Assert.IsEmpty(properties);
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
        public void TestSpecialCharacters()
        {
            FileStream fileStream = null;
            try
            {
                Console.WriteLine("Loading " + SpecialCharactersTestFile);

                fileStream = new FileStream(SpecialCharactersTestFile, FileMode.Open);
                JavaProperties properties = new JavaProperties();
                properties.Load(fileStream);
                fileStream.Close();

                //Assert.IsEmpty(properties);
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
        public void TestUtf8NonAsciiSymbols()
        {
            FileStream utf8FileStream = null;
            FileStream isoFileStream = null;
            try
            {
                Console.WriteLine("Loading " + NonAciiSymbolsUtf8TestFile);

                // A file containing non-ASCII characters, which is saved using the utf8 encoding
                utf8FileStream = new FileStream(NonAciiSymbolsUtf8TestFile, FileMode.Open);

                Console.WriteLine("Loading " + NonAsciiSymbolsNativeToAsciiTestFile);

                // A file with the same data as above, but processed with the native2ascii tool from jdk 1.8, and stored in ISO-8859-1. This will work correctly.
                isoFileStream = new FileStream(NonAsciiSymbolsNativeToAsciiTestFile, FileMode.Open);

                // Java properties read from the utf8 file with correct encoding provided
                JavaProperties utf8PropertiesCorrect = new JavaProperties();

                // we explicitly specify the encoding, so that UTF8 characters are read using the UTF8 encoding and the data will be correct
                utf8PropertiesCorrect.Load(utf8FileStream, Encoding.UTF8);

                JavaProperties utf8PropertiesIncorrect = new JavaProperties();
                utf8FileStream.Seek(0, SeekOrigin.Begin);// reset the stream position from the previous loading.
                // we do not set the encoding, so the data will not appear correctly - UTF8 characters will be read usign the default ISO-8859-1 encoding
                // this is to ensure that setting the encoding makes a difference
                utf8PropertiesIncorrect.Load(utf8FileStream);

                
                JavaProperties isoProperties = new JavaProperties();
                isoProperties.Load(isoFileStream);

                foreach (var key in utf8PropertiesCorrect.Keys)
                {
                    // Asert the correct file is identical to its native2ascii version
                    Assert.AreEqual(utf8PropertiesCorrect[key], isoProperties[key]);

                    if (key.Equals("AsciiText"))
                    {
                        // Assert that not-using the proper encoding will not corrupt pure ASCII data
                        Assert.AreEqual(utf8PropertiesCorrect[key], utf8PropertiesIncorrect[key]);
                        Assert.AreEqual(utf8PropertiesCorrect[key], isoProperties[key]);
                    }
                    else
                    {
                        // Assert that not-using the proper encoding will corrupt data
                        Assert.AreNotEqual(utf8PropertiesIncorrect[key], utf8PropertiesCorrect[key]);
                        Assert.AreNotEqual(utf8PropertiesIncorrect[key], isoProperties[key]);
                    }
                    
                }
                isoFileStream.Close();

                //Assert.IsEmpty(properties);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
            finally
            {
                if (utf8FileStream != null)
                {
                    utf8FileStream.Close();
                }
                if (isoFileStream != null)
                {
                    isoFileStream.Close();
                }
            }
        }

        [Test]
        public void TestMixed()
        {
            FileStream fileStream = null;
            try
            {
                Console.WriteLine("Loading " + MixedTestFile);

                fileStream = new FileStream(MixedTestFile, FileMode.Open);
                JavaProperties properties = new JavaProperties();
                properties.Load(fileStream);
                fileStream.Close();

                //Assert.IsEmpty(properties);
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
