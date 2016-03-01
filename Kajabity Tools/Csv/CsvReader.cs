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
        private const int STATE_Field = 1;
        private const int STATE_Quoted = 2;
        private const int STATE_DoubleQuote = 3;
        private const int STATE_SkipTrailingSpace = 4;
        private const int STATE_EndField = 5;
        private const int STATE_EndLine = 6;
        private const int STATE_EndFile = 7;

        /// <summary>
        /// Used in debug and error reporting.
        /// </summary>
        private static readonly string[] StateNames =
        {
            "Start", "Field", "Quoted", "Double Quote", "SkipTrailingSpace", "End of Field", "End of Line", "End of File"
        };

        //	The different types of matcher used.
        private const int MATCH_none = 0;
        private const int MATCH_EOF = 1;
        private const int MATCH_Separator = 2;
        private const int MATCH_LineFeed = 3;
        private const int MATCH_DoubleQuote = 4;
        private const int MATCH_WhiteSpace = 5;
        private const int MATCH_Any = 6;

        //	Actions performed when a character is matched.
        private const int ACTION_none = 0;
        private const int ACTION_SaveField = 1;
        private const int ACTION_SaveLine = 2;
        private const int ACTION_AppendToField = 3;
        private const int ACTION_AppendLineFeedToField = 4;

        /// <summary>
        /// The State Machine - an array of states, each an array of transitions, and each of those 
        /// an array of integers grouped in threes - { match condition, next state, action to perform }.
        /// </summary>
        private static readonly int[][] States =
        {
            new int[]{//STATE_Start
                //MATCH_WhiteSpace,       STATE_Start,            ACTION_none,
                MATCH_Separator,        STATE_EndField,         ACTION_SaveField,
                MATCH_DoubleQuote,      STATE_Quoted,           ACTION_none,
                MATCH_LineFeed,         STATE_EndLine,          ACTION_SaveLine,
                MATCH_EOF,              STATE_EndFile,          ACTION_SaveLine,
                MATCH_Any,              STATE_Field,            ACTION_AppendToField,
            },
            new int[]{//STATE_Field
                MATCH_Separator,        STATE_EndField,         ACTION_SaveField,
                MATCH_LineFeed,         STATE_EndLine,          ACTION_SaveLine,
                MATCH_EOF,              STATE_EndFile,          ACTION_SaveLine,
                MATCH_Any,              STATE_Field,            ACTION_AppendToField,
            },
            new int[]{//STATE_Quoted
                MATCH_DoubleQuote,      STATE_DoubleQuote,      ACTION_none,
                MATCH_EOF,              STATE_EndFile,          ACTION_SaveLine,
                MATCH_LineFeed,         STATE_Quoted,           ACTION_AppendLineFeedToField,
                MATCH_Any,              STATE_Quoted,           ACTION_AppendToField,
            },
            new int[]{//STATE_DoubleQuote
                MATCH_DoubleQuote,      STATE_Quoted,           ACTION_AppendToField,
                MATCH_EOF,              STATE_EndFile,          ACTION_SaveLine,
                MATCH_Separator,        STATE_EndField,         ACTION_SaveField,
                MATCH_LineFeed,         STATE_EndLine,          ACTION_SaveLine,
                //MATCH_WhiteSpace,       STATE_SkipTrailingSpace,ACTION_none,
            },
            new int[]{//STATE_SkipTrailingSpace
                MATCH_EOF,              STATE_EndFile,          ACTION_SaveLine,
                MATCH_Separator,        STATE_EndField,         ACTION_SaveField,
                MATCH_LineFeed,         STATE_EndLine,          ACTION_SaveLine,
                MATCH_WhiteSpace,       STATE_SkipTrailingSpace,ACTION_none,
            },
            new int[]{//STATE_EndField
                MATCH_none,             STATE_Start,            ACTION_none,
            },
            new int[]{//STATE_EndLine
                MATCH_none,             STATE_Start,            ACTION_none,
            },
            //STATE_EndFile - no state transitions.
        };

        #endregion

        //	---------------------------------------------------------------------
        //  Constants
        //	---------------------------------------------------------------------

        /// <summary>
        /// The size of the buffer used to read the input data.
        /// </summary>	
        private const int BufferSize = 1000;

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
        /// Gets or sets the separator character used in the CSV stream - 
        /// default value is a comma (',').
        /// </summary>
        public int Separator = CsvConstants.DEFAULT_SEPARATOR_CHAR;

        /// <summary>
        /// Gets or sets the quote character used in the CSV stream - default 
        /// value is a double quote ('"').
        /// </summary>
        public int Quote = CsvConstants.DEFAULT_QUOTE_CHAR;

        //	---------------------------------------------------------------------
        //  Working data.
        //	---------------------------------------------------------------------

        /// <summary>
        /// The starting state for the parser engine.
        /// </summary>
        private int state = STATE_Start;

        /// <summary>
        /// The input stream that characters are read and parsed from.
        /// </summary>
        private BufferedStream inStream = null;

        /// <summary>
        /// Stores the next character after it's been peeked until it is removed by NextChar().
        /// </summary>
        private int savedChar;

        /// <summary>
        /// A flag to indicate whether or not there is a savedChar.
        /// </summary>
        private bool saved = false;

        /// <summary>
        /// A variable to hold on to the 2nd LineFeed character - if there is one.
        /// </summary>
        private int ExtraLinefeedChar = 0;

        //  ---------------------------------------------------------------------
        //  Constructors.
        //  ---------------------------------------------------------------------

        /// <summary>
        /// Construct a inStream.
        /// </summary>
        /// <param name="stream">The input stream to read from.</param>
        public CsvReader(Stream stream)
        {
            inStream = new BufferedStream(stream, BufferSize);
        }

        //  ---------------------------------------------------------------------
        //  Methods.
        //  ---------------------------------------------------------------------

        /// <summary>
        /// Reads the next field on the current line - or null after the end of the line.  The
        /// field will not be part of the next ReadLine or ReadFile.
        /// </summary>
        /// <returns>the next field or null after the end of the record.</returns>
        public string ReadField()
        {
            // Check we haven't passed the end of the line/file.
            if (state > STATE_EndField)
            {
                return null;
            }

            // Parse the next field.
            Parse(STATE_EndField);

            // Return and remove the last field.
            string field = (string)fieldList[fieldList.Count - 1];
            //Debug.WriteLine( "ReadField: \"" + field + "\"" );
            fieldList.RemoveAt(fieldList.Count - 1);
            return field;
        }

        /// <summary>
        /// Read to the end of the current record, if any.
        /// </summary>
        /// <returns>The current record - or null if at end of file.</returns>
        public string[] ReadRecord()
        {
            // Check we haven't passed the end of the file.
            if (state > STATE_EndLine)
            {
                return null;
            }

            // Parse to the end of the current line.
            Parse(STATE_EndLine);

            // Return and remove the last field.
            string[] record = (string[])rowList[rowList.Count - 1];
            rowList.RemoveAt(rowList.Count - 1);
            return record;
        }

        /// <summary>
        /// Reads all fields and records from the CSV input stream from the current location.
        /// </summary>
        /// <returns>an array of string arrays, each row representing a row of values from the CSV file - or null if
        /// already at the end of the file.</returns>
        public string[][] ReadAll()
        {
            // Check we haven't passed the end of the file.
            if (state == STATE_EndFile)
            {
                return null;
            }

            // Parse to the end of the file.
            Parse(STATE_EndFile);

            // Return and remove the last field.
            string[][] records = (string[][])rowList.ToArray(typeof(string[]));
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
        private void Parse(int finalSate)
        {
            if (finalSate >= STATE_EndFile)
            {
                finalSate = STATE_EndFile;
            }

            bool lambda = false;
            int ch = -1;
            do
            {
                bool matched = false;

                if (lambda)
                {
                    lambda = false;
                }
                else
                {
                    ch = NextChar();
                }

                for (int s = 0; s < States[state].Length; s += 3)
                {
                    if (Matches(States[state][s], ch))
                    {
                        //Debug.WriteLine( stateNames[ state ] + ", " + (s/3) + ", " + ch + (ch>20?" (" + (char) ch + ")" : "") );
                        matched = true;

                        if (States[state][s] == MATCH_none)
                        {
                            lambda = true;
                        }

                        DoAction(States[state][s + 2], ch);

                        state = States[state][s + 1];
                        break;
                    }
                }

                if (!matched)
                {
                    throw new CsvParseException("Unexpected character at state " + StateNames[state] + ": <<<" + (char)ch + ">>>");
                }
            }
            while (state < finalSate);
        }

        /// <summary>
        /// Tests if the current character matches a test (one of the MATCH_* tests).
        /// </summary>
        /// <param name="match">The number of the MATCH_* test to try.</param>
        /// <param name="ch">The character to test.</param>
        /// <returns></returns>
        private bool Matches(int match, int ch)
        {
            ExtraLinefeedChar = 0;

            switch (match)
            {
                case MATCH_none:
                    return true;

                case MATCH_Separator:
                    return ch == Separator;

                case MATCH_EOF:
                    return ch == -1;

                case MATCH_LineFeed:
                    if (ch == '\r')
                    {
                        if (PeekChar() == '\n')
                        {
                            ExtraLinefeedChar = '\n';
                            saved = false;
                        }
                        return true;
                    }
                    if (ch == '\n')
                    {
                        return true;
                    }
                    return false;

                case MATCH_DoubleQuote:
                    return ch == Quote;

                case MATCH_WhiteSpace:
                    return ch == ' ' || ch == '\t' || ch == '\v';

                case MATCH_Any:
                    return true;

                default:  // Allows the match char to be the 'MATCH' parameter.
                          //return ch == match;
                    return false;
            }
        }

        /// <summary>
        /// Performs the action associated with a state transition.
        /// </summary>
        /// <param name="action">The number of the action to perform.</param>
        /// <param name="ch">The character matched in the state.</param>
        private void DoAction(int action, int ch)
        {
            switch (action)
            {
                case ACTION_none:
                    break;

                case ACTION_SaveField:  // Append the field to the fieldList as a String.
                                        //Debug.WriteLine( "ACTION_SaveField: \"" + fieldBuilder.ToString() + "\"" );
                    fieldList.Add(fieldBuilder.ToString());
                    fieldBuilder.Length = 0;
                    break;

                case ACTION_SaveLine:   // Append the line to the rowList as an array of strings.
                                        //Debug.Write( "ACTION_SaveLine: \"" + fieldBuilder.ToString() + "\"" );
                    fieldList.Add(fieldBuilder.ToString());
                    fieldBuilder.Length = 0;

                    //Debug.WriteLine( " - " + fieldList.Count + " fields" );
                    rowList.Add(fieldList.ToArray(typeof(string)));
                    fieldList.Clear();
                    break;

                case ACTION_AppendToField:
                    fieldBuilder.Append((char)ch);
                    break;

                case ACTION_AppendLineFeedToField:
                    fieldBuilder.Append((char)ch).Append((char)ExtraLinefeedChar);
                    break;
            }
        }

        /// <summary>
        /// Returns and removes the next character from the input stream - including any that have been peeked and pushed back.
        /// </summary>
        /// <returns>The next character from the stream.</returns>
        private int NextChar()
        {
            if (saved)
            {
                saved = false;
                return savedChar;
            }

            return inStream.ReadByte();
        }

        /// <summary>
        /// Retuns but doesn't remove the next character from the stream.  
        /// This character will be returned every time this method is called until it is returned by NextChar().
        /// </summary>
        /// <returns>The next character from the stream.</returns>
        private int PeekChar()
        {
            if (saved)
            {
                return savedChar;
            }

            saved = true;
            return savedChar = inStream.ReadByte();
        }
    }
}
