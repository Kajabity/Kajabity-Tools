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
using Kajabity.Tools.Test;
using NUnit.Framework;

namespace Kajabity.Tools.Csv
{
	[TestFixture]
	public class CsvReaderTest : KajabityToolsTest
	{
        private const string MixedTestFile = CsvTestDataDirectory + "mixed.csv";
        
        // TODO: test reading empty, one line, several lines, error scenarios, qoted, unquoted, embedded comma, CR, LF, quote,
		// TODO: test with alternate separator
		// TODO: Test reading fields to end of line with zero, one or more fields.
		[Test]
		public void TestReadAll()
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
		public void TestReadFieldAndRecord()
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
