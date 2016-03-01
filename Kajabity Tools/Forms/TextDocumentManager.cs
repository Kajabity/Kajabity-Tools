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
using System.Diagnostics;

namespace Kajabity.Tools.Forms
{
    /// <summary>
    /// A Document Manager for Text Documents.
    /// </summary>
    public class TextDocumentManager: DocumentManager
    {
        /// <summary>
        /// Return the currently loaded document (cast to a TextDocument) or 
        /// null if no document loaded.
        /// </summary>
        public TextDocument TextDocument
        {
            get
            {
                return (TextDocument) document;
            }
        }

        //  ---------------------------------------------------------------------
        //  Constructors.
        //  ---------------------------------------------------------------------

        /// <summary>
        /// Construct a TextDocumentManager setting the default document name and extension.
        /// </summary>
        public TextDocumentManager()
        {
            DefaultName = "Text Document";
            DefaultExtension = "txt";
        }

        //  ---------------------------------------------------------------------
        //  Methods.
        //  ---------------------------------------------------------------------

        /// <summary>
        /// Create a new Text document.
        /// </summary>
        public override void NewDocument()
        {
            document = new TextDocument();

            base.NewDocument();
        }

        /// <summary>
        /// Loads a text document from filename.
        /// </summary>
        /// <param name="filename">the name of the file to load as a text document.</param>
        public override void Load( string filename )
        {
            Debug.WriteLine( "Loading " + filename );

            TextReader reader = new StreamReader( filename );

            TextDocument td = new TextDocument();
            td.Text = reader.ReadToEnd();
            reader.Close();

            document = td;
            base.Load( filename );
        }

        /// <summary>
        /// Saves the currently loaded text document to filename.
        /// </summary>
        /// <param name="filename">the name of the file to save the text document into.</param>
        public override void Save( string filename )
        {
            TextWriter writer = new StreamWriter( filename );

            writer.Write( ((TextDocument) document).Text );
            writer.Close();

            base.Save( filename );
        }
    }
}
