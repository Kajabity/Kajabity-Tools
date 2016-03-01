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

namespace Kajabity.Tools.Forms
{
    /// <summary>
    /// Encapsulates the name of a type of document and it's default extension.
    /// Used in File Open/Save dialogs and to provide the default filename for
    /// New documents.
    /// </summary>
    public class DocumentType
    {
        /// <summary>
        /// The name of a type of document which is used both in the Open/Save 
        /// dialogs and as the basis for a default document name for new
        /// Documents.
        /// </summary>
        public string Name;

        /// <summary>
        /// The filename filter pattern to apply in File Open/Save dialogs.
        /// </summary>
        public string Pattern;

        //  ---------------------------------------------------------------------
        //  Constructors.
        //  ---------------------------------------------------------------------

        /// <summary>
        /// Default or Empty constructor.
        /// </summary>
        public DocumentType()
        {
        }

        //  ---------------------------------------------------------------------
        //  Methods.
        //  ---------------------------------------------------------------------

    }
}
