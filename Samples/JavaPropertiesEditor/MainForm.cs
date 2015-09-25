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

using Kajabity.Tools.Forms;
using Kajabity.Tools.Java;
using System;
using System.Windows.Forms;

namespace JavaPropertiesEditor
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class MainForm : SDIForm
	{
		public MainForm()
            : base( new JavaPropertiesDocumentManager() )
        {
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
		}

		public MainForm( string filename )
            : base( new JavaPropertiesDocumentManager() )
        {
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

            this.loadDocument( filename );
		}

		public string Status
		{
			set
			{
				if( value == null )
				{
					toolStripMessageLabel.Text = "Ready.";
				}
				else
				{
					toolStripMessageLabel.Text = value;
				}
			}
			get
			{
				return toolStripMessageLabel.Text;
			}
		}

        public override void DocumentChanged()
        {
			// Clear the list.
			this.listView1.Items.Clear();
			
			if( manager.Opened )
			{
				//	Update main form heading.
				this.Text = Application.ProductName + " - " + manager.Document.Name;
				JavaProperties properties = ((JavaPropertiesDocument) manager.Document).Properties;
				
				//	Update the content window.
				foreach( String key in properties.Keys )
				{
					ListViewItem item = new ListViewItem( key );
					String text = properties.GetProperty( key );
					item.SubItems.Add( new ListViewItem.ListViewSubItem( item, text ) );
					
					this.listView1.Items.Add( item );
				}
			}
			else
			{
				//	Update main form heading.
				this.Text = Application.ProductName;
			}

			//	Force a display update.
			this.Refresh();
		}

		void OnFileNew(object sender, System.EventArgs e)
		{
            this.FileNewClick( sender, e );
        }
		
		void OnFileOpen(object sender, System.EventArgs e)
		{
            this.FileOpenClick( sender, e );
        }
		
		void OnFileClose(object sender, EventArgs e)
		{
            this.FileCloseClick( sender, e );
		}
		
		void OnFileSave(object sender, EventArgs e)
		{
            this.FileSaveClick( sender, e );
        }
		
		void OnFileSaveAs(object sender, EventArgs e)
		{
            this.FileSaveAsClick( sender, e );
        }
		
		void OnFileExit(object sender, EventArgs e)
		{
            this.FileExitClick( sender, e );
        }
	}
}
