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

namespace Kajabity.Tools.Forms
{
    /// <summary>
    /// A base class to manage the lifecycle of a Document by creating new, empty documents, 
    /// Opening existing documents, tracking the modification state of documents and Saving
    /// documents.
    /// 
    /// Extend this class to implement methods to handle specific document types.
    /// </summary>
    public abstract class DocumentManager
    {
        private string defaultExtension = "txt";

        /// <summary>
        /// The filename extension (without the dot) for the type of files managed by this class.
        /// </summary>
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

        /// <summary>
        /// The default name or filename (without path or extension) for a file created by this class.
        /// </summary>
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
        
        private string filename = null;

        /// <summary>
        /// The filename of the current document.
        /// </summary>
        public string Filename
        {
            get
            {
                return filename;
            }
        }

        private bool newfile = true;

        /// <summary>
        ///	An indication of whether there is a filename applied.
        /// </summary>
        public bool NewFile
        {
            get
            {
                return newfile;
            }
        }
        
        /// <summary>
        /// Counts the number of documents openned by this instance of the Document Manager to 
        /// allow default filenames to be more unique.
        /// </summary>
        private int docCount = 0;
        
        protected Document document = null;

        /// <summary>
        /// Get the currently loaded document - or null if no document loaded.
        /// </summary>
        public Document Document
        {
            get
            {
                return document;
            }
        }
        
        /// <summary>
        /// Returns true this instance has a document.
        /// </summary>
        public bool Opened
        {
            get
            {
                return document != null;
            }
        }
        
        /// <summary>
        /// Tests if a document is Opened and is modified.
        /// </summary>
        public bool Modified
        {
            get
            {
                return Opened && document.Modified;
            }
        }

        //  ---------------------------------------------------------------------
        //  Constructors.
        //  ---------------------------------------------------------------------

        public DocumentManager()
        {
        }

        //  ---------------------------------------------------------------------
        //  Methods.
        //  ---------------------------------------------------------------------

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
