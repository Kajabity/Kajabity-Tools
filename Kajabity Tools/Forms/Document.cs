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

namespace Kajabity.Tools.Forms
{
    /// <summary>
    /// An abstract base class for all types of document that can be created, 
    /// loaded, modified and saved by the SDIForm methods.  Extend this class
    /// to add properties and methods appropriate for your particular type of document
    /// or data.
    /// </summary>
    public abstract class Document
    {
        private bool modified = false;

        /// <summary>
        /// Gets or sets a flag to indicate if the contents of the document
        /// have been modified and may require saving.
        /// </summary>
        public bool Modified
        {
            get
            {
                return modified;
            }
            set
            {
                modified = value;
            }
        }

        private string name;
        
        /// <summary>
        /// Gets or sets the name of the document - usually derived from the filename.
        /// </summary>
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }

        //  ---------------------------------------------------------------------
        //  Constructors.
        //  ---------------------------------------------------------------------
            
        /// <summary>
        /// Empty contructor used to create an unnamed document - the name will be set later.
        /// </summary>
        public Document()
        {
        }

        /// <summary>
        /// Construct a document providing a name.  The name may be changed later - e.g. when saved.
        /// </summary>
        /// <param name="name">The name of the document</param>
        public Document( string name )
        {
            this.name = name;
        }
    }
}
