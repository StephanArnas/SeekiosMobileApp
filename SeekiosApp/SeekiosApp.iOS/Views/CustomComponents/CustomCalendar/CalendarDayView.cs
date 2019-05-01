//	New implementations, refactoring and restyling - FactoryMind || http://factorymind.com 
//  Converted to MonoTouch on 1/22/09 - Eduardo Scoz || http://escoz.com
//  Originally reated by Devin Ross on 7/28/09  - tapku.com || http://github.com/devinross/tapkulibrary
//
/*
 
 Permission is hereby granted, free of charge, to any person
 obtaining a copy of this software and associated documentation
 files (the "Software"), to deal in the Software without
 restriction, including without limitation the rights to use,
 copy, modify, merge, publish, distribute, sublicense, and/or sell
 copies of the Software, and to permit persons to whom the
 Software is furnished to do so, subject to the following
 conditions:
 
 The above copyright notice and this permission notice shall be
 included in all copies or substantial portions of the Software.
 
 THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
 OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
 NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
 HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
 WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
 OTHER DEALINGS IN THE SOFTWARE.
 
 */

using System;
using UIKit;

namespace SeekiosApp.iOS.CustomComponents.FMCalendar
{
	sealed class CalendarDayView : UIView
	{
		string text;
		int fontSize = 18;

		public DateTime Date {get;set;}
		bool _active, _today, _selected, _marked, _available;
		public bool Available {get {return _available; } set {_available = value; SetNeedsDisplay(); }}
		public string Text {get { return text; } set { text = value; SetNeedsDisplay(); } }
		public bool Active {get { return _active; } set { _active = value; SetNeedsDisplay();  } }
		public bool Today {get { return _today; } set { _today = value; SetNeedsDisplay(); } }
		public bool Selected {get { return _selected; } set { _selected = value; SetNeedsDisplay(); } }
		public bool Marked {get { return _marked; } set { _marked = value; SetNeedsDisplay(); }  }

		public UIColor SelectionColor { get; set; }
		public UIColor TodayCircleColor { get; set; }

		public CalendarDayView()
		{
			BackgroundColor = UIColor.White;
			TodayCircleColor = UIColor.Red;
			SelectionColor = UIColor.Red;
		}

		public override void Draw (CoreGraphics.CGRect rect)
		{
			PerformDraw ();
		}

		void PerformDraw()
		{
			UIColor color = UIColor.Black;

			if (!Active || !Available)
			{
				color = UIColor.LightGray;
				if(Selected)
					color = Today ? UIColor.White : SelectionColor;
			} else if (Today && Selected)
			{
				color = UIColor.White;
			} else if (Today)
			{
				color = UIColor.White;
			} else if (Selected)
			{
				color = SelectionColor;
			}
				

			if (Today)
			{
				var context = UIGraphics.GetCurrentContext();
				var todaySize = (float) Math.Min (Bounds.Height, Bounds.Width);
				if (todaySize > 50)
					todaySize = 50;
				todaySize = Math.Min (fontSize * 2, todaySize);

				TodayCircleColor.SetColor ();

				context.SetLineWidth(0);

				context.AddEllipseInRect(new CoreGraphics.CGRect((Bounds.Width / 2) - (todaySize / 2), (Bounds.Height / 2) - (todaySize / 2), todaySize, todaySize));

				context.FillPath();
			}

			color.SetColor ();

			Text.DrawString (new CoreGraphics.CGRect (0, (Bounds.Height / 2) - (fontSize / 2), Bounds.Width, Bounds.Height),
				UIFont.SystemFontOfSize (fontSize), UILineBreakMode.WordWrap,
				UITextAlignment.Center);

			if (Marked)
			{
				var context = UIGraphics.GetCurrentContext();
				if (Selected && !Today)
					SelectionColor.SetColor ();
				else if (Today)
					UIColor.White.SetColor ();
				else if (!Active || !Available)
					UIColor.LightGray.SetColor ();
				else
					UIColor.Black.SetColor ();

				context.SetLineWidth(0);

				context.AddEllipseInRect(new CoreGraphics.CGRect(Frame.Size.Width/2 - 2, (Bounds.Height / 2) + (fontSize / 2) + 5, 4, 4));

				context.FillPath();
			}
		}
	}
}