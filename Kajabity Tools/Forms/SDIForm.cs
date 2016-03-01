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
        /// <summary>
        /// A reference to an instance of the DocumentManager used to load and save
        /// documents for the application.
        /// </summary>
        protected DocumentManager manager;


        /// <summary>
        /// If true, when saving a document any existing file with the same name 
        /// will be renamed with a trailing '~' character as a backup.  Any 
        /// previous backup (i.e. with the same backup filename) will be deleted.
        /// </summary>
        protected bool backup = false;

        //  ---------------------------------------------------------------------
        //  Constructors.
        //  ---------------------------------------------------------------------

        /// <summary>
        /// Default constructor - required to enable the Visual Editor for Forms
        /// in Visual Studio to work.
        /// </summary>
        public SDIForm()
        {
            this.Load += new EventHandler( SDIForm_Load );
        }

        /// <summary>
        /// Construct an SDIForm providing an instance of a document manager.
        /// </summary>
        /// <param name="manager">the DocumentManager to be used by this form.</param>
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
            newDocument();
            DocumentChanged();
        }

        /// <summary>
        /// A handler for the File->New command.  
        /// Closes any currently open document (prompting the user to save it if modified).
        /// Creates a new instance of the managed document type.
        /// Call this method your form's 'OnClick()' handler.
        /// </summary>
        /// <param name="sender">the object that sent the event - e.g. menu item or tool bar button.</param>
        /// <param name="e">Any additional arguments to the event.</param>
        public void FileNewClick( object sender, System.EventArgs e )
        {
            if( canCloseDocument( sender, e ) )
            {
                newDocument();
            }
        }

        /// <summary>
        /// A handler for the File->Open command.   
        /// Uses the OpenFileDialog to select and open a file.
        /// Closes any currently open document (prompting the user to save it if modified).
        /// Call this method your form's 'OnClick()' handler.
        /// </summary>
        /// <param name="sender">the object that sent the event - e.g. menu item or tool bar button.</param>
        /// <param name="e">Any additional arguments to the event.</param>
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

        /// <summary>
        /// A handler for the File->Close command.  
        /// Closes any currently open document (prompting the user to save it if modified).
        /// Call this method your form's 'OnClick()' handler.
        /// </summary>
        /// <param name="sender">the object that sent the event - e.g. menu item or tool bar button.</param>
        /// <param name="e">Any additional arguments to the event.</param>
        public void FileCloseClick( object sender, EventArgs e )
        {
            if( canCloseDocument( sender, e ) )
            {
                closeDocument();
            }
        }

        /// <summary>
        /// A handler for the File->Save command.  
        /// Saves the currently open document - prompting for a filename if it is a new document.
        /// Call this method your form's 'OnClick()' handler.
        /// </summary>
        /// <param name="sender">the object that sent the event - e.g. menu item or tool bar button.</param>
        /// <param name="e">Any additional arguments to the event.</param>
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

        /// <summary>
        /// A handler for the File->Save As command.  
        /// Prompts the user for a filename and path and saves the document to it.
        /// Call this method your form's 'OnClick()' handler.
        /// </summary>
        /// <param name="sender">the object that sent the event - e.g. menu item or tool bar button.</param>
        /// <param name="e">Any additional arguments to the event.</param>
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

        /// <summary>
        /// A handler for the File->Exit command.  
        /// Exits the application, closing any currently open document (prompting the user to save it if modified).
        /// Call this method your form's 'OnClick()' handler.
        /// </summary>
        /// <param name="sender">the object that sent the event - e.g. menu item or tool bar button.</param>
        /// <param name="e">Any additional arguments to the event.</param>
        public void FileExitClick( object sender, EventArgs e )
        {
            if( canCloseDocument( sender, e ) )
            {
                Application.Exit();
            }
        }

        /// <summary>
        /// Called whenever a document is created or loaded (or closed).
        /// Override to provide specific Forms handling when the document is changed.
        /// </summary>
        public virtual void DocumentChanged()
        {
            //	Force a display update.
            this.Refresh();
        }

        /// <summary>
        /// Helper to create a new document - triggers DocumentChanged().
        /// </summary>
        private void newDocument()
        {
            manager.NewDocument();

            //	Refresh display.
            DocumentChanged();
        }

        /// <summary>
        /// Helper to load a document - triggers DocumentChanged().  
        /// </summary>
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

        /// <summary>
        /// Helper to save a document - triggers DocumentChanged().
        /// </summary>
        /// <param name="filename">the filename and path to save the document into</param>
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

        /// <summary>
        /// Helper to close any currently open document - triggers DocumentChanged().
        /// </summary>
        private void closeDocument()
        {
            manager.Close();

            //	Refresh display.
            DocumentChanged();
        }

        /// <summary>
        /// A "helper" menu action to try to close the currently loaded file if there is one.
        /// The method will display a prompt to the user to save the file if modified.
        /// </summary>
        /// <param name="sender">the object that sent the event - e.g. menu item or tool bar button.</param>
        /// <param name="e">Any additional arguments to the event.</param>
        /// <returns>returns true if the document is closed.</returns>
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
