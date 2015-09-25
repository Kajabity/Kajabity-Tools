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

namespace HexViewer
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class MainForm : SDIForm
	{
		public MainForm()
			: base( new BinaryDocumentManager() )
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
			panel.BinaryDocument = (BinaryDocument) manager.Document;

			//	Force a display update.
			base.Refresh();
		}

		void NewToolStripMenuItemClick(object sender, EventArgs e)
		{
			this.FileNewClick(sender, e);
		}
		
		void OpenToolStripMenuItemClick(object sender, EventArgs e)
		{
			this.FileOpenClick(sender, e);
		}
		
		void SaveToolStripMenuItemClick(object sender, EventArgs e)
		{
			this.FileSaveClick(sender, e);
		}
		
		void SaveAsToolStripMenuItemClick(object sender, EventArgs e)
		{
			this.FileSaveAsClick(sender, e);
		}
		
		void PrintToolStripMenuItemClick(object sender, EventArgs e)
		{
			
		}
		
		void PrintPreviewToolStripMenuItemClick(object sender, EventArgs e)
		{
			
		}
		
		void ExitToolStripMenuItemClick(object sender, EventArgs e)
		{
			this.FileExitClick(sender, e);
		}
	}
}
