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
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;

namespace Kajabity.Tools.Forms
{
    /// <summary>
    /// Description of SDIForm.
    /// </summary>
    public class SDIForm : Form
    {
        protected DocumentManager manager;

        protected bool backup = false;

        //  ---------------------------------------------------------------------
        //  Constructors.
        //  ---------------------------------------------------------------------

        public SDIForm()
        {
            this.Load += new EventHandler( SDIForm_Load );
        }

        public SDIForm( DocumentManager manager )
        {
            this.manager = manager;
            this.Load += new EventHandler( SDIForm_Load );
        }

        //  ---------------------------------------------------------------------
        //  Methods.
        //  ---------------------------------------------------------------------

        private void SDIForm_Load( object sender, System.EventArgs e )
        {
            DocumentChanged();
        }

        public void FileNewClick( object sender, System.EventArgs e )
        {
            if( canCloseDocument( sender, e ) )
            {
                newDocument();
            }
        }

        public void FileOpenClick( object sender, System.EventArgs e )
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "Open " + Application.ProductName + " file";
            dialog.Filter = Application.ProductName + " files (*." + manager.DefaultExtension + ")|*." + manager.DefaultExtension +
                "|All files (*.*)|*.*";

            //			dialog.InitialDirectory = @"C:\";
            dialog.AddExtension = true;
            dialog.CheckFileExists = true;
            dialog.CheckPathExists = true;
            //			dialog.ShowHelp = true; // Need to handle 'HelpRequest' event.
            dialog.ReadOnlyChecked = false;
            dialog.DefaultExt = manager.DefaultExtension;

            if( dialog.ShowDialog() == DialogResult.OK && canCloseDocument( sender, e ) )
            {
                Debug.WriteLine( manager.DefaultExtension + " file: " + dialog.FileName );

                loadDocument( dialog.FileName );
            }
        }

        public void FileCloseClick( object sender, EventArgs e )
        {
            if( canCloseDocument( sender, e ) )
            {
                closeDocument();
            }
        }

        public void FileSaveClick( object sender, EventArgs e )
        {
            if( manager.NewFile )
            {
                FileSaveAsClick( sender, e );
            }
            else
            {
                saveDocument( manager.Filename );
            }
        }

        public void FileSaveAsClick( object sender, EventArgs e )
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.DefaultExt = manager.DefaultExtension;
            dialog.Title = "Save " + Application.ProductName + " file";
            dialog.Filter = Application.ProductName + " files (*." + manager.DefaultExtension + ")|*." + manager.DefaultExtension +
                "|All files (*.*)|*.*";
            dialog.FileName = manager.Filename;

            if( dialog.ShowDialog( this ) == DialogResult.OK )
            {
                Debug.WriteLine( manager.DefaultExtension + " file: " + dialog.FileName );

                saveDocument( dialog.FileName );
            }
        }

        public void FileExitClick( object sender, EventArgs e )
        {
            if( canCloseDocument( sender, e ) )
            {
                Application.Exit();
            }
        }

        public virtual void DocumentChanged()
        {
            //	Force a display update.
            this.Refresh();
        }

        private void newDocument()
        {
            manager.NewDocument();

            //	Refresh display.
            DocumentChanged();
        }

        protected void loadDocument( string filename )
        {
            try
            {
                //	load the file
                manager.Load( filename );

                // add successfully opened file to MRU list
                //mruManager.Add( filename );

                //	Refresh display.
                DocumentChanged();
            }
            catch( Exception ex )
            {
                // Report the error
                Debug.WriteLine( ex.ToString() );

                // remove file from MRU list - if it exists
                //mruManager.Remove(filename);

                // Let the calling methods handle it.
                throw ex;
            }
        }

        private void saveDocument( string filename )
        {
            if( backup && File.Exists( filename ) )
            {
                string backupFilename = filename + "~";

                if( File.Exists( backupFilename ) )
                {
                    File.Delete( backupFilename );
                }

                File.Move( filename, backupFilename );
            }

            manager.Save( filename );

            //	Refresh display.
            DocumentChanged();
        }

        private void closeDocument()
        {
            manager.Close();

            //	Refresh display.
            DocumentChanged();
        }

        /// <summary>
        /// A "helper" menu action to try to close the currently loaded file if there is one.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        protected bool canCloseDocument( object sender, System.EventArgs e )
        {
            if( manager.Modified )
            {
                DialogResult result = MessageBox.Show( this, Application.ProductName + " file " + manager.Filename + " has been modified!\n\nDo you want to save it?",
                                                      Application.ProductName,
                                                      MessageBoxButtons.YesNoCancel,
                                                      MessageBoxIcon.Exclamation );

                if( result == DialogResult.Yes )
                {
                    FileSaveClick( sender, e );
                }
                else if( result == DialogResult.Cancel )
                {
                    return false;
                }
            }

            closeDocument();

            return true;
        }
    }
}
