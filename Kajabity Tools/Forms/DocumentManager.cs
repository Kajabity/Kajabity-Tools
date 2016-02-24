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
        /// The filename of the current document.  Will be null of no current document.
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
        ///	Returns true if the current document (if any) is new and unsaved.
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
        /// allow default filenames to be a little more unique.
        /// </summary>
        private int docCount = 0;

        /// <summary>
        /// Holds a reference to the currently loaded document - or null if none loaded.
        /// </summary>
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
        /// Tests if a document is both Opened and modified.
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

        /// <summary>
        /// Construct an empty DocumentManager instance.
        /// </summary>
        public DocumentManager()
        {
        }

        //  ---------------------------------------------------------------------
        //  Methods.
        //  ---------------------------------------------------------------------

        /// <summary>
        /// Performs base initialisation of the document name and status of a 
        /// new document.  Override this method to provide initialisation for
        /// specific Document types then call this base class 
        /// once document set (<code>base.NewDocument();</code>).
        /// </summary>
        public virtual void NewDocument()
        {
            string documentName = String.Format(defaultName + "." + defaultExtension, ++docCount);
            filename = documentName;
            newfile = true;

            if (document != null)
            {
                document.Name = documentName;
            }
        }

        /// <summary>
        /// Performs base class setup of loaded filename and status.  Override 
        /// to load a file into a Document instance then call this base class 
        /// once document set (<code>base.Load(filename);</code>).
        /// </summary>
        /// <param name="filename">filename and path loaded</param>
        public virtual void Load(string filename)
        {
            this.filename = filename;
            newfile = false;

            if (document != null)
            {
                FileInfo fileInfo = new FileInfo(filename);
                document.Name = fileInfo.Name;
                document.Modified = false;
            }
        }

        /// <summary>
        /// Performs base class setup of Saved filename and status.  Override 
        /// to save a file from a Document instance then call this base class 
        /// (<code>base.Save(filename);</code>).
        /// </summary>
        /// <param name="filename">filename and path to save to</param>
        public virtual void Save(string filename)
        {
            this.filename = filename;
            newfile = false;

            if (document != null)
            {
                FileInfo fileInfo = new FileInfo(filename);
                document.Name = fileInfo.Name;
                document.Modified = false;
            }
        }

        /// <summary>
        /// Close any currently open document - setting state appropriately.  
        /// Override this method to perform any necessary cleanup (releasing 
        /// resources, etc.) for the Document instance then call this base 
        /// class (<code>base.Close();</code>).
        /// </summary>
        public virtual void Close()
        {
            document = null;
            newfile = false;
            filename = null;
        }
    }
}
