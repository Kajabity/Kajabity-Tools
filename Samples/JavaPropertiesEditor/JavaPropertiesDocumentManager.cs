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

using System.IO;
using Kajabity.Tools.Forms;


namespace JavaPropertiesEditor
{
	/// <summary>
	/// Description of JavaPropertiesDocumentManager.
	/// </summary>
	public class JavaPropertiesDocumentManager : DocumentManager
	{
		public JavaPropertiesDocumentManager()
		{
			DefaultName = "java";
			DefaultExtension = "properties";
		}
		
		public override void NewDocument()
		{
			document = new JavaPropertiesDocument( Filename );

			base.NewDocument();
		}

		public override void Load( string filename )
		{
			document = new JavaPropertiesDocument( filename );
			Stream stream = new FileStream( filename, FileMode.Open );
			((JavaPropertiesDocument) document).Properties.Load( stream );
			stream.Close();

			base.Load( filename );
		}

		public override void Save( string filename )
		{
			Stream stream = new FileStream( filename, FileMode.Create );
			((JavaPropertiesDocument) document).Properties.Store( stream, "Testing 'Store' method." );
			stream.Close();

			base.Save( filename );
		}
	}
}
