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

using System.IO;

namespace Kajabity.Tools.Csv
{
	/// <summary>
	/// A class for writing a table of data to an output stream using the Comma
	/// Separated Value (CSV) format.
	/// </summary>
	public class CsvWriter
	{
		//  ---------------------------------------------------------------------
		//  Settings
		//  ---------------------------------------------------------------------

		/// <summary>
		/// Any string containing the following characters needs to be quoted and
		/// any quote characters doubled up.
		/// </summary>
		private static char[] escapeChars = new char[] { '\n', '\r', '"', ',' };
		
		/// <summary>
		/// An explicit newline string matching the standard.
		/// </summary>
		private static char[] newLine = new char[] { '\r', '\n' };

		//  ---------------------------------------------------------------------
		//  Options
		//  ---------------------------------------------------------------------

		/// <summary>
		/// Gets or sets the separator character used in the file - default
		/// value is a comma (",").
		/// </summary>
		public char Separator = ',';

		/// <summary>
		/// The class checks each field to see if it needs to be quoted - unless
		/// the string is longer than quoteLimit.  Used for performance.  Set to
		/// 0 to ensure all except empty or null fields are quoted.
		/// </summary>
		public int QuoteLimit = 1000;

		//  ---------------------------------------------------------------------
		//  Working data.
		//  ---------------------------------------------------------------------

		/// <summary>
		/// The output stream the data is written to - wraps the output stream
		/// provided in the constructor.
		/// </summary>
		private StreamWriter writer = null;

		/// <summary>
		/// Count of the number of fields written to a line - used to determine
		/// when to append a separator.
		/// </summary>
		private int fieldCount = 0;

		/// <summary>
		/// Count of the number of records written - used to determine when to
		/// add a new line before the start of the next.
		/// </summary>
		private int recordCount = 0;

		/// <summary>
		/// Used to ensure minimal flushing - only flush if zero.  Flushing is needed
		/// because we are wrapping the underlying stream with our own.  Flushing by the
		/// caller doesn't cut it.
		/// </summary>
		private int flush = 0;
		
		//  ---------------------------------------------------------------------
		//  Constructors.
		//  ---------------------------------------------------------------------

		/// <summary>
		/// Construct a new to output CSV data to the stream provided.
		/// </summary>
		/// <param name="stream">CSV data will be written to this stream.</param>
		public CsvWriter( Stream stream )
		{
			// Write the date in the US-ASCII format (code page 20127)
			writer = new StreamWriter( stream, System.Text.Encoding.GetEncoding( 20127 ) );
		}

		//  ---------------------------------------------------------------------
		//  Public methods.
		//  ---------------------------------------------------------------------

		/// <summary>
		/// Write all records to the output CSV stream.
		/// </summary>
		/// <param name="records">an array of string arrays containing the
		/// records to be written.  Each string becomes a single comma separated
		/// field, each array of strings becomes a record.</param>
		public void WriteAll( string[][] records )
		{
			flush++;
			if( records != null && records.Length > 0 )
			{
				foreach( string[] record in records )
				{
					WriteRecord( record );
				}
			}

			if( --flush == 0 )
			{
				writer.Flush();
			}
		}

		/// <summary>
		/// Writes a single record to the output stream appending a new line to the end
		/// of the previous line if this is not the first record.
		/// </summary>
		/// <param name="record">The record to be written - an array of string fields.</param>
		public void WriteRecord( string[] record )
		{
			flush++;
			if( record != null && record.Length > 0 )
			{
				if( recordCount++ > 0 )
				{
					writer.Write( newLine );
					fieldCount = 0;
				}

				foreach( string field in record )
				{
					WriteField( field );
				}

				if( --flush == 0 )
				{
					writer.Flush();
				}
			}
		}

		/// <summary>
		/// Write a single field to the output stream appending a separator if
		/// it is not the first field on the line.
		/// </summary>
		/// <param name="field"></param>
		public void WriteField( string field )
		{
			flush++;
			if( fieldCount++ > 0 )
			{
				writer.Write( Separator );
			}

			WriteFieldEscaped( field );
			

			if( --flush == 0 )
			{
				writer.Flush();
			}
		}

		//  ---------------------------------------------------------------------
		//  Private methods.
		//  ---------------------------------------------------------------------

		/// <summary>
		/// This is the worker method that writes fields to the file and quotes
		/// them if necessary. If the field is null or an empty string, nothing
		/// is written (not even quoted).
		/// </summary>
		/// <param name="field">a field to be written</param>
		private void WriteFieldEscaped( string field )
		{
			if( string.IsNullOrEmpty(field) )
			{
				return;
			}

			if( field.Length > QuoteLimit || field.IndexOfAny( escapeChars ) >= 0 )
			{
				writer.Write( '"' );
				foreach( char ch in field )
				{
					writer.Write( ch );
					if( ch == '"' )
					{
						writer.Write( '"' );
					}
				}
				writer.Write( '"' );
			}
			else
			{
				writer.Write( field );
			}
		}
	}
}
