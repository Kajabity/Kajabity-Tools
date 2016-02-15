using Kajabity.Tools.Csv;
using Kajabity.Tools.Forms;
using System.Diagnostics;
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


namespace CsvEditor
{
    public class CsvDocumentManager : DocumentManager
    {
        
        public CsvDocument CsvDocument
        {
            get
            {
                return (CsvDocument) document;
            }
        }

        public CsvDocumentManager()
        {
            DefaultName = "CsvDocument";
            DefaultExtension = "csv";
        }

        public override void NewDocument()
        {
            document = new CsvDocument();
            ((CsvDocument)document).Rows = new string[0][];

            base.NewDocument();
        }

        public override void Load( string filename )
        {
            Debug.WriteLine( "Loading " + filename );
            CsvDocument csvDoc = new CsvDocument(filename);

            FileStream fileStream = null;
            try
            {
                //Console.WriteLine("Loading " + filename);
                fileStream = File.OpenRead( filename );
                CsvReader reader = new CsvReader( fileStream );

                string[][] records = reader.ReadAll();
                csvDoc.Rows = records;
            }
            finally
            {
                if( fileStream != null )
                {
                    fileStream.Close();
                }
            }

            document = csvDoc;
            base.Load( filename );
        }

        public override void Save( string filename )
        {
            //FileInfo info = new FileInfo( filename );
            Debug.WriteLine("Saving " + filename);

            FileStream outStream = null;
                try
                {
                    outStream = File.OpenWrite(filename);
                    outStream.SetLength( 0L );

                    CsvWriter writer = new CsvWriter( outStream );
                    //writer.QuoteLimit = -1;
                    CsvDocument csvDoc = this.CsvDocument;

                    writer.WriteAll( csvDoc.Rows );
                    outStream.Flush();
                }
                finally
                {
                    if( outStream != null )
                    {
                        outStream.Close();
                    }
                }

            base.Save( filename );
        }
    }
}
