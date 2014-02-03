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
using System.Text;
using NUnit.Framework;

using Kajabity.Tools.Test;

namespace Kajabity.Tools.Java
{
	[TestFixture]
    public class JavaPropertyWriterTest : KajabityToolsTest
	{
		//  ---------------------------------------------------------------------
		//  Simply test that it can write all the properties that it reads.
		//  ---------------------------------------------------------------------

		[Test]
        public void TestJavaPropertyWriter()
		{
            const string filename = JavaTestDataDirectory + "mixed.properties";
            const string outName = JavaOutputDirectory + "test-writer.properties";

            FileStream inStream = null;
            FileStream outStream = null;
            try
            {
                //  Read the mixed properties file.
                Console.WriteLine("Loading " + filename);
                inStream = File.OpenRead(filename);

                JavaProperties properties1 = new JavaProperties();
                properties1.Load( inStream );
                inStream.Close();
                inStream = null;

                // Write it to a new file - using the JavaPropertiesWriter we are testing.
                outStream = File.OpenWrite(outName);
                properties1.Store(outStream, "Testing JavaPropertyWriter");
                outStream.Flush();
                outStream.Close();
                outStream = null;

                //  Now re-read the output file into a second set of properties
                inStream = File.OpenRead(outName);

                JavaProperties properties2 = new JavaProperties();
                properties2.Load(inStream);

                // Now compare the values - find, compare and remove each value in turn.
                foreach (String name in properties2.Keys)
                {
                    String value2 = properties2.GetProperty(name);

                    if (properties1.ContainsKey(name))
                    {
                        String value1 = properties1.GetProperty(name);

                        if (value1 == null)
                        {
                            if (value2 != null)
                            {
                                Assert.Fail("Original value is null, new value is : \"" + value2 + "\"");
                            }
                        }
                        else if (!value1.Equals(value2))
                        {
                            Assert.Fail("Original value is \"" + value1 + "\", new value is : \"" + value2 + "\"");
                        }

                        properties1.Remove( name );
                    }
                    else
                    {
                        Assert.Fail("Missing property: \"" + name + "\" (with value \"" + value2 + "\")");
                    }
                }

                if( properties1.Count > 0 )
                {
                    Assert.Fail( properties1.Count + " values missing from output file.");
                }
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
            finally
            {
                if (inStream != null)
                {
                    inStream.Close();
                }

                if (outStream != null)
                {
                    outStream.Close();
                }
            }
        }
	}
}
