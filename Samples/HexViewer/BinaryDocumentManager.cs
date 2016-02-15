using Kajabity.Tools.Forms;
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
using System.Diagnostics;
using System.IO;

namespace HexViewer
{
	/// <summary>
	/// Description of BinaryDocumentManager.
	/// </summary>
	public class BinaryDocumentManager	: DocumentManager
	{

		public BinaryDocument BinaryDocument
		{
			get
			{
				return (BinaryDocument) document;
			}
		}

		public BinaryDocumentManager()
		{
			DefaultName = "file";
			DefaultExtension = "dat";
		}

		public override void NewDocument()
		{
			document = new BinaryDocument();

			base.NewDocument();
		}

		public override void Load( string filename )
		{
			Debug.WriteLine( "Loading " + filename );
			
			byte[] buffer;
			FileStream fileStream = new FileStream( filename, FileMode.Open, FileAccess.Read );
			try
			{
				int length = (int) fileStream.Length;  // get file length
				buffer = new byte[length];            // create buffer
				int count;                            // actual number of bytes read
				int sum = 0;                          // total number of bytes read

				// read until Read method returns 0 (end of the stream has been reached)
				while ((count = fileStream.Read(buffer, sum, length - sum)) > 0)
				{
					sum += count;  // sum is a buffer offset for next reading
				}
				
				document = new BinaryDocument( buffer );
			}
			finally
			{
				fileStream.Close();
			}

			base.Load( filename );
		}

		public override void Save( string filename )
		{
			base.Save( filename );
		}
	}
}
