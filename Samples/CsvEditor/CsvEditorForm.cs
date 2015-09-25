using Kajabity.Tools.Csv;
using Kajabity.Tools.Forms;
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
using System.Text;
using System.Windows.Forms;

namespace CsvEditor
{
    public partial class CsvEditorForm : SDIForm
    {
        // Remember the maximum number of columns so we can add headings if they increase.
        /// <summary>
        // Constructor for the CSV Editor main form.
        /// </summary>
        public CsvEditorForm()
            : base( new CsvDocumentManager() )
        {
            InitializeComponent();
        }

        public override void DocumentChanged()
        {
            // Clear the list.
            this.listView.Items.Clear();
            this.listView.Columns.Clear();

            if( manager.Opened )
            {
                //	Update main form heading.
                this.Text = Application.ProductName + " - " + manager.Document.Name;

                CsvDocument doc = (CsvDocument) manager.Document;
                ListViewItem[] lvItems = new ListViewItem[ doc.Rows.Length ];
                int counter = 0;
                int maxColumns = 1;

                foreach( string[] row in doc.Rows )
                {
                    var lvItem = new ListViewItem( row );
                    lvItems[ counter ] = lvItem;
                    counter++;

                    if( row.Length > maxColumns )
                    {
                        maxColumns = row.Length;
                    }
                }

                for( int col = 0; col < maxColumns; col++ )
                {
                    this.listView.Columns.Add( columnName( col ) );
                }

                listView.BeginUpdate();
                listView.Items.AddRange( lvItems );
                listView.EndUpdate();

            }
            else
            {
                //	Update main form heading.
                this.Text = Application.ProductName;
            }

            //	Force a display update.
            //this.Refresh();
        }

        /// <summary>
        /// Convert a number into an alphabetic column header - A-Z, then AA, AB - ZZ, AAA...
        /// <para>
        /// This could be done much more efficiently, but not for this sample.
        /// </para>
        /// </summary>
        /// <param name="col">the index of the column starting at zero for A.</param>
        /// <returns>the alphabetic column name</returns>
        private string columnName( int col )
        {
            StringBuilder buf = new StringBuilder();
            do
            {
                buf.Append( (char) ( 'A' + ( col % 26 ) ) );
                col = col / 26;
            } while( col > 0 );

            return buf.ToString();
        }

        private void newToolStripMenuItem_Click( object sender, EventArgs e )
        {
            this.FileNewClick( sender, e );
        }

        private void openToolStripMenuItem_Click( object sender, EventArgs e )
        {
            try
            {
                this.FileOpenClick( sender, e );
            }
            catch( CsvParseException parseEx )
            {
                MessageBox.Show(this, parseEx.Message, "Error parsing file", MessageBoxButtons.OK, MessageBoxIcon.Error );
            }
            catch( Exception ex )
            {
                MessageBox.Show( this, ex.Message, "Error opening file", MessageBoxButtons.OK, MessageBoxIcon.Error );
            }
        }

        private void saveToolStripMenuItem_Click( object sender, EventArgs e )
        {
            this.FileSaveClick( sender, e );
        }

        private void saveAsToolStripMenuItem_Click( object sender, EventArgs e )
        {
            this.FileSaveAsClick( sender, e );
        }

        private void exitToolStripMenuItem_Click( object sender, EventArgs e )
        {
            this.FileExitClick( sender, e );
        }

        private void aboutToolStripMenuItem_Click( object sender, EventArgs e )
        {

        }
    }
}
