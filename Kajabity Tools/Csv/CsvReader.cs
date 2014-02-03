/*
 * Copyright 2009-14 Simon J. Williams.
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
 * http://www.kajabity.com
 */

using System;
using System.CodeDom;
using System.Collections;
using System.IO;
using System.Text;

namespace Kajabity.Tools.Csv
{
    /// <summary>
    /// This class reads CSV formatted data from an input stream.  It can read individual fields in a row, 
    /// a full row or the whole file.  Data can only be read once - so if the first field in a row is read, 
    /// it won't be part of the row if that is read next.
    /// 
    /// The term Row is used rather than line because quoted fields can include line breaks (real, not 
    /// escaped) so that one row may be spread across multiple lines.
    /// </summary>
    public class CsvReader
    {
        //	---------------------------------------------------------------------
		#region The State Machine (ATNP)        
		//	---------------------------------------------------------------------

        //	All the states.
        private const int STATE_Start = 0;
        private const int STATE_Quoted = 1;
        private const int STATE_DoubleQuote = 2;
        private const int STATE_EndField = 3;
        private const int STATE_EndLine = 4;
        private const int STATE_EndFile = 5;

        //	Used in debug and error reporting.
        private static string [] stateNames = new string[]
        {
            "Start", "Quoted", "Double Quote", "End of Field", "End of Line", "End of File"
        };

        //	The different types of matcher used.
    	private const int MATCH_none = 0;
        private const int MATCH_EOF = 1;
        private const int MATCH_Separator = 2;
        private const int MATCH_LineFeed = 3;
        private const int MATCH_DoubleQuote = 4;
        private const int MATCH_Any = 5;

        //	Actions performed when a character is matched.
        private const int ACTION_none = 0;
        private const int ACTION_SaveField = 1;
        private const int ACTION_SaveLine = 2;
        private const int ACTION_AppendToField = 3;

        /// <summary>
        /// The State Machine - an array of states, each an array of transitions, and each of those 
        /// an array of integers grouped in threes - { match condition, next state, action to perform }.
        /// </summary>
        private static int [][] states = new int[][]
        {
            new int[]{//STATE_Start
                MATCH_Separator,        STATE_EndField,         ACTION_SaveField,
                MATCH_LineFeed,         STATE_EndLine,          ACTION_SaveLine,
                MATCH_DoubleQuote,      STATE_Quoted,           ACTION_none,
                MATCH_EOF,              STATE_EndFile,          ACTION_SaveLine,
                MATCH_Any,              STATE_Start,            ACTION_AppendToField,
            },
            new int[]{//STATE_Quoted
                MATCH_DoubleQuote,      STATE_DoubleQuote,      ACTION_none,
                MATCH_EOF,              STATE_EndFile,          ACTION_SaveLine,
                MATCH_Any,              STATE_Quoted,           ACTION_AppendToField,
            },
            new int[]{//STATE_DoubleQuote
                MATCH_DoubleQuote,      STATE_Quoted,           ACTION_AppendToField,
                MATCH_EOF,              STATE_EndFile,          ACTION_SaveLine,
                MATCH_Separator,        STATE_EndField,         ACTION_SaveField,
                MATCH_LineFeed,         STATE_EndLine,          ACTION_SaveLine,
            },
            new int[]{//STATE_EndField
                MATCH_none,             STATE_Start,            ACTION_none,
            },
            new int[]{//STATE_EndLine
                MATCH_none,             STATE_Start,            ACTION_none,
            },
        };

        #endregion

        //	---------------------------------------------------------------------
        //  Constants
        //	---------------------------------------------------------------------

        //	The size of the buffer used to read the input data.
        private const int bufferSize =  1000;

        //	---------------------------------------------------------------------
        //  The result.
        //	---------------------------------------------------------------------

        private StringBuilder fieldBuilder = new StringBuilder();
        private ArrayList fieldList = new ArrayList();
        private ArrayList rowList = new ArrayList();

        //	---------------------------------------------------------------------
        //  Options
        //	---------------------------------------------------------------------

        /// <summary>
        /// Gets or sets the separator character used in the CSV stream - default value is a
        /// comma (",").
        /// </summary>
        public int Separator = ',';

        //	---------------------------------------------------------------------
        //  Working data.
        //	---------------------------------------------------------------------

        /// <summary>
        /// The starting state for the parser engine.
        /// </summary>
        private int state = STATE_Start;

        private BufferedStream reader = null;
        private int savedChar;
        private bool saved = false;

        //  ---------------------------------------------------------------------
        //  Constructors.
        //  ---------------------------------------------------------------------

        /// <summary>
        /// Construct a reader.
        /// </summary>
        /// <param name="stream">The input stream to read from.</param>
        public CsvReader( Stream stream )
        {
            reader = new BufferedStream( stream, bufferSize );
        }

        /// <summary>
        /// Reads the next field on the current line - or null after the end of the line.  The
        /// field will not be part of the next ReadLine or ReadFile.
        /// </summary>
        /// <returns>the next field or null after the end of the record.</returns>
        public string ReadField()
        {
            // Check we haven't passed the end of the line/file.
            if( state > STATE_EndField )
            {
                return null;
            }

            // Parse the next field.
            parse( STATE_EndField );

            // Return and remove the last field.
            string field = (string) fieldList[ fieldList.Count - 1 ];
            //Debug.WriteLine( "ReadField: \"" + field + "\"" );
            fieldList.RemoveAt( fieldList.Count - 1 );
            return field;
        }

        /// <summary>
        /// Read to the end of the current record, if any.
        /// </summary>
        /// <returns>The current record - or null if at end of file.</returns>
        public string [] ReadRecord()
        {
            // Check we haven't passed the end of the file.
            if( state > STATE_EndLine )
            {
                return null;
            }

            // Parse the next field.
            parse( STATE_EndLine );

            // Return and remove the last field.
            string [] record = (string []) rowList[ rowList.Count - 1 ];
            rowList.RemoveAt( rowList.Count - 1 );
            return record;
        }

        /// <summary>
        /// Reads all fields and records from the CSV input stream.
        /// </summary>
        /// <returns></returns>
        public string [][] ReadAll()
        {
            // Check we haven't passed the end of the file.
            if( state == STATE_EndFile )
            {
                return null;
            }

            // Parse the next field.
            parse( STATE_EndFile );

            // Return and remove the last field.
            string [][] records = (string [][]) rowList.ToArray( typeof (string []) );
            rowList.Clear();
            return records;
        }

        /// <summary>
        /// Parse the input CSV stream from the current position until the final state is 
        /// reached.  Intended to allow parsing to End of Field, End of Record or End of File.
        /// </summary>
        /// <param name="finalSate">Specify where the parser should stop (or pause) 
        /// by indicating which state to finish on.</param>
        /// <exception cref="T:CsvParseException">Thrown when an unexpected/invalid character 
        /// is encountered in the input stream.</exception>
        private void parse( int finalSate )
        {
            if( finalSate >= STATE_EndFile )
            {
                finalSate = STATE_EndFile;
            }

            bool lambda = false;
            int ch = -1;
            do
            {
                bool matched = false;

                if( lambda )
                {
                    lambda = false;
                }
                else
                {
                    ch = nextChar();
                }

                for( int s = 0; s < states[ state ].Length; s += 3 )
                {
                    if( matches( states[ state ][ s ], ch ) )
                    {
                        //Debug.WriteLine( stateNames[ state ] + ", " + (s/3) + ", " + ch + (ch>20?" (" + (char) ch + ")" : "") );
                        matched = true;

                        if( states[ state ][ s ] == MATCH_none )
                        {
                            lambda = true;
                        }

                        doAction( states[ state ][ s + 2 ], ch );

                        state = states[ state ][ s + 1 ];
                        break;
                    }
                }

                if( !matched )
                {
                    throw new CsvParseException( "Unexpected character at " + 1 + ": <<<" + ch + ">>>" );
                }
            }
            while( state < finalSate );
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="match"></param>
        /// <param name="ch"></param>
        /// <returns></returns>
        private bool matches( int match, int ch )
        {
            switch( match )
            {
            case MATCH_none:
                return true;

            case MATCH_Separator:
                return ch == Separator;

            case MATCH_EOF:
                return ch == -1;

            case MATCH_LineFeed:
                if( ch == '\r' )
                {
                    if( peekChar() == '\n')
                    {
                        saved = false;
                    }
                    return true;
                }
                else if( ch == '\n' )
                {
                    return true;
                }
                return false;

            case MATCH_DoubleQuote:
                return ch == '"';

            case MATCH_Any:
                return true;

            default:  // Allows the match char to be the 'MATCH' parameter.
                //return ch == match;
                return false;
            }
        }

        private void doAction( int action, int ch )
        {
            switch( action )
            {
            case ACTION_none:
                break;

            case ACTION_SaveField:  // Append the field to the fieldList as a String.
                //Debug.WriteLine( "ACTION_SaveField: \"" + fieldBuilder.ToString() + "\"" );
                fieldList.Add( fieldBuilder.ToString() );
                fieldBuilder.Length = 0;
                break;

            case ACTION_SaveLine:   // Append the line to the rowList as an array of strings.
                //Debug.Write( "ACTION_SaveLine: \"" + fieldBuilder.ToString() + "\"" );
                fieldList.Add( fieldBuilder.ToString() );
                fieldBuilder.Length = 0;

                //Debug.WriteLine( " - " + fieldList.Count + " fields" );
                rowList.Add( fieldList.ToArray( typeof (string) ) );
                fieldList.Clear();
                break;

            case ACTION_AppendToField:
                fieldBuilder.Append( (char) ch );
                break;

            default:
                break;
            }
        }

        private int nextChar()
        {
            if( saved )
            {
                saved = false;
                return savedChar;
            }

            return reader.ReadByte();
        }

        private int peekChar()
        {
            if( saved )
            {
                return savedChar;
            }

            saved = true;
            return savedChar = reader.ReadByte();
        }
    }
}
