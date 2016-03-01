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
using System.Drawing;
using System.Windows.Forms;

namespace HexViewer
{
	/// <summary>
	/// Description of HexPanel.
	/// </summary>
	public class HexPanel : Panel
	{
		private int nRows = 1;
		private int nCols = 16;
		private int padding = 4;
		private SizeF sizeAddress;

		private BinaryDocument document;
		public BinaryDocument BinaryDocument
		{
			get
			{
				return document;
			}
			set
			{
				document = value;
				Graphics g = this.CreateGraphics();

				string sample = "000000";
				Font fontText = new Font( this.Font.FontFamily, this.Font.Size, FontStyle.Regular );
				sizeAddress = g.MeasureString( sample, fontText );

				int width = (int)(sizeAddress.Width * (2 + nCols * 2/3) + 2 * padding);
				
				if( document != null )
				{
					nRows = (document.Data.Length + nCols - 1)/nCols;
				}
				else
				{
					nRows = 1;
				}
				
				int height = (int)(sizeAddress.Height * nRows + 2 * padding);
				
				this.AutoScrollMinSize = new Size( width, height );
				this.AutoScrollMargin = new Size( (int) sizeAddress.Width / 6, (int) sizeAddress.Height );

				Debug.WriteLine( "Size=" + this.Size + ", sizeAddress=" + sizeAddress );
				Debug.WriteLine( "Rows=" + nRows + ", AutoScrollMinSize=" + this.AutoScrollMinSize );
			}
		}

		public HexPanel()
		{
			this.BackColor = Color.White;
		}

		protected override void OnPaint( PaintEventArgs ev )
		{
			base.OnPaint( ev );

//			Brush brushBack     = new SolidBrush( Color.Aqua );
//			ev.Graphics.FillRectangle( brushBack, ClientRectangle );
			
			// Add a blank padded border around the displayed chart.
			Draw( ev.Graphics, Rectangle.Inflate( ClientRectangle, -padding, -padding ) );
		}

		public void Draw( Graphics g, Rectangle rectClient )
		{
			Brush brushBack     = new SolidBrush( BackColor );
			Brush brushAddress  = new SolidBrush( Color.Black );
			Brush brushBytes    = new SolidBrush( Color.Blue );
			Brush brushChars    = new SolidBrush( Color.DarkGreen );
			
			Font fontText = new Font( this.Font.FontFamily, this.Font.Size, FontStyle.Regular );

			StringFormat format = new StringFormat();
			format.Alignment = StringAlignment.Near;
			format.LineAlignment = StringAlignment.Near;

			//  Wipe the whole background.
			g.FillRectangle( brushBack, rectClient );
			int offset = 0;
			float X = AutoScrollPosition.X + padding;
			float Y = AutoScrollPosition.Y + padding;
			
			if( document != null )
			{
				do
				{
					if( Y + sizeAddress.Height > 0 )
					{
						g.DrawString( offset.ToString( "X6" ), fontText, brushAddress, X, Y, format );

						int limit = Math.Min( offset + nCols, document.Data.Length );
						
						int b = offset;
						X += sizeAddress.Width;
						do
						{
							X += sizeAddress.Width / 2;
							g.DrawString( document.Data[ b++ ].ToString( "X2" ), fontText, brushBytes, X, Y, format );
						} while( b < limit );
						
						b = offset;
						X += sizeAddress.Width / 2;
						do
						{
							char ch = (char) document.Data[ b++ ];
							if( ch > 32 && ch < 128 )
							{
								g.DrawString( ch.ToString(), fontText, brushBytes, X, Y, format );
							}
							X += sizeAddress.Width / 6;
						} while( b < limit );
					}

					offset += nCols;
					Y += sizeAddress.Height;
					X = AutoScrollPosition.X + padding;
				} while( offset < document.Data.Length );
			}
			
			g.Flush();
		}
	}
}
