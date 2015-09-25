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
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Kajabity.Tools.Forms;

namespace TextEditor
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class MainForm : SDIForm
	{
		public MainForm() :
			base( new TextDocumentManager() )
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
		}

		public override void DocumentChanged()
		{
			if( manager.Opened )
			{
				textBox.Text = ((TextDocumentManager) manager).TextDocument.Text;
			}

			//	Force a display update.
			Refresh();
		}
		
		private void UpdateDocument()
		{
			if( textBox.Modified)
			{
				((TextDocumentManager) manager).TextDocument.Text = textBox.Text;
			}
		}

		void NewToolStripMenuItemClick(object sender, EventArgs e)
		{
			FileNewClick( sender, e );
		}
		
		void OpenToolStripMenuItemClick(object sender, EventArgs e)
		{
			FileOpenClick( sender, e );
		}
		
		void SaveToolStripMenuItemClick(object sender, EventArgs e)
		{
			UpdateDocument();
			FileSaveClick( sender, e );
		}
		
		void SaveAsToolStripMenuItemClick(object sender, EventArgs e)
		{
			UpdateDocument();
			FileSaveAsClick( sender, e );
		}
		
		void PageSetupToolStripMenuItemClick(object sender, EventArgs e)
		{
			UpdateDocument();
		}
		
		void PrintToolStripMenuItemClick(object sender, EventArgs e)
		{
			UpdateDocument();
		}
		
		void PrintPreviewToolStripMenuItemClick(object sender, EventArgs e)
		{
			UpdateDocument();
		}
		
		void ExitToolStripMenuItemClick(object sender, EventArgs e)
		{
			UpdateDocument();
			FileExitClick( sender, e );
		}
		
		void PrintToolStripButtonClick(object sender, EventArgs e)
		{
			
		}
		
		void CutToolStripButtonClick(object sender, EventArgs e)
		{
			textBox.Cut();
		}
		
		void CopyToolStripButtonClick(object sender, EventArgs e)
		{
			textBox.Copy();
		}
		
		void PasteToolStripButtonClick(object sender, EventArgs e)
		{
			textBox.Paste();
		}
		
		void UndoToolStripMenuItemClick(object sender, EventArgs e)
		{
			textBox.Undo();
		}
		
		void RedoToolStripMenuItemClick(object sender, EventArgs e)
		{
		}
		
		void SelectAllToolStripMenuItemClick(object sender, EventArgs e)
		{
			textBox.SelectAll();
		}
		
		void DeleteToolStripMenuItemClick(object sender, EventArgs e)
		{
			textBox.SelectedText = "";
		}
		
		void EditToolStripMenuItemDropDownOpening(object sender, EventArgs e)
		{
			undoToolStripMenuItem.Enabled = textBox.CanUndo;
		}
		
		void BottomToolStripPanelClick(object sender, System.EventArgs e)
		{
			
		}
	}
}
