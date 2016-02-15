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

using Kajabity.Tools.Test;
using NUnit.Framework;
using System;
using System.IO;

namespace Kajabity.Tools.Csv
{
	[TestFixture]
	public class CsvWriterTest : KajabityToolsTest
	{
		//  ---------------------------------------------------------------------
		//  Test CSV Reader
		//  ---------------------------------------------------------------------

		[Test]
		public void TestCsvWriter()
		{
			const string filename = CsvTestDataDirectory + "mixed.csv";
			FileStream inStream = null;
			FileStream outStream = null;
			try
			{
                Console.WriteLine( "Loading " + filename );
				inStream = File.OpenRead( filename );
				CsvReader reader = new CsvReader( inStream );
				string [][] records = reader.ReadAll();

				const string outName = CsvOutputDirectory + "test-writer.csv";
				outStream = File.OpenWrite( outName );
				outStream.SetLength( 0L );

				CsvWriter writer = new CsvWriter( outStream );
				//writer.QuoteLimit = -1;

				writer.WriteAll( records );
				outStream.Flush();
			}
            catch (Exception ex)
            {
                Assert.Fail( ex.Message );
            }
            finally
			{
				if( inStream != null )
				{
					inStream.Close();
				}

				if( outStream != null )
				{
					outStream.Close();
				}
			}
		}
		
		[Test]
		public void TestWriteRecord()
		{
            string filename = CsvOutputDirectory + "test-write-record.csv";
			string [] record = new string[] { "AAAA", "BBBB", "CCCC" };
			const int lenRecord = 14; // Strings, commas.

            Stream stream = null;
			try
			{
				//	Create the temp file (or overwrite if already there).
				stream = File.Open( filename, FileMode.OpenOrCreate, FileAccess.ReadWrite );
				stream.SetLength( 0 );
				stream.Close();

				//	Check it's empty.
				FileInfo info = new FileInfo( filename );
				Assert.AreEqual( 0, info.Length, "File length not zero." );
				
				//  Open for append
				stream = File.OpenWrite( filename );
				
				//	Append a record.
				CsvWriter writer = new CsvWriter( stream );
				writer.WriteRecord( record );
				stream.Flush();
				stream.Close();

				//	Check it's not empty.
				info = new FileInfo( filename );
				Assert.AreEqual( lenRecord, info.Length, "File length not increased." );
			}
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
            finally
			{
				if( stream != null )
				{
					stream.Close();
					//File.Delete( filename );  // Keep it for debugging.
				}
			}
		}

        [Test]
        public void TestWriteAlternateSeparator()
        {
            string filename = CsvOutputDirectory + "test-write-alternate-separator.csv";
            string[] record = new string[] { "AA,AA original separator", "BB|BB new separator", "CCCC" };
            const int lenRecord = 14; // Strings, commas.

            Stream stream = null;
            try
            {
                Console.WriteLine( "Creating empty " + filename );
                //	Create the temp file (or overwrite if already there).
                stream = File.Open( filename, FileMode.OpenOrCreate, FileAccess.ReadWrite );
                stream.SetLength( 0 );
                stream.Close();

                //	Check it's empty.
                FileInfo info = new FileInfo( filename );
                Assert.AreEqual( 0, info.Length, "File length not zero." );

                //  Open for append
                Console.WriteLine( "Writing " + filename );
                stream = File.OpenWrite( filename );

                //	Append a record.
                CsvWriter writer = new CsvWriter( stream );
                writer.Separator = '|';
                writer.WriteRecord( record );
                stream.Flush();
                stream.Close();

                Console.WriteLine( "Loading " + filename );
                stream = File.OpenRead( filename );
                CsvReader reader = new CsvReader( stream );
                reader.Separator = '|';
                string[][] records = reader.ReadAll();

                Assert.AreEqual( 1, records.Length, "Should only be one record." );

                Console.WriteLine( "REad :" + ToString( records[ 0 ] ) );

                Assert.AreEqual( record.Length, records[0].Length, "Should be " + record.Length + " fields in record." );

                for( int fieldNo = 0; fieldNo < record.Length; fieldNo++ )
                {
                    Assert.AreEqual( record[ fieldNo ], records[ 0 ][ fieldNo ], "Field " + record.Length + " Should be " + record[ fieldNo ] );
                }
            }
            catch( Exception ex )
            {
                Assert.Fail( ex.Message );
            }
            finally
            {
                if( stream != null )
                {
                    stream.Close();
                    //File.Delete( filename );  // Keep it for debugging.
                }
            }
        }
    }
}
