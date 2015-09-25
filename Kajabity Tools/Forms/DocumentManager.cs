/*
 * Copyright 2009-15 Williams Technologies Limtied.
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

namespace Kajabity.Tools.Forms
{
	/// <summary>
	/// Description of DocumentManager.
	/// </summary>
	public abstract class DocumentManager
	{
		private string defaultExtension = "txt";
		public string DefaultExtension
		{
			get
			{
				return defaultExtension;
			}
			set
			{
				defaultExtension = value;
			}
		}
		
		private string defaultName = "Document";
		public string DefaultName
		{
			get
			{
				return defaultName;
			}
			set
			{
				defaultName = value;
			}
		}
		
		//	The filename of the current document.
		private string filename = null;
		public string Filename
		{
			get
			{
				return filename;
			}
		}

		//	An indication of whether there is a filename applied.
		private bool newfile = true;
		public bool NewFile
		{
			get
			{
				return newfile;
			}
		}
		
		private int docCount = 0;
		
		protected Document document = null;
		public Document Document
		{
			get
			{
				return document;
			}
		}
		
		public bool Opened
		{
			get
			{
				return document != null;
			}
		}
		
		public bool Modified
		{
			get
			{
				return Opened ? document.Modified : false;
			}
		}

		public DocumentManager()
		{
		}

		public virtual void NewDocument()
		{
			string documentName = String.Format( defaultName + "." + defaultExtension, ++docCount );
			filename = documentName;
			newfile = true;

			if( document != null )
			{
				document.Name = documentName;
			}
		}

		public virtual void Load( string filename )
		{
			this.filename = filename;
			newfile = false;

			if( document != null )
			{
				FileInfo fileInfo = new FileInfo(filename);
				document.Name = fileInfo.Name;
				document.Modified = false;
			}
		}

		public virtual void Save( string filename )
		{
			this.filename = filename;
			newfile = false;

			if( document != null )
			{
				FileInfo fileInfo = new FileInfo(filename);
				document.Name = fileInfo.Name;
				document.Modified = false;
			}
		}

		public virtual void Close()
		{
			document = null;
			newfile = false;
		}
	}
}
