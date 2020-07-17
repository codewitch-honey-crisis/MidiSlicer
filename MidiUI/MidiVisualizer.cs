using System;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using System.Threading;

namespace M
{
	/// <summary>
	/// Represents a MIDI visualizer
	/// </summary>
	public class MidiVisualizer : Control
	{
		static readonly Color[] _DefaultChannelColors = new Color[] {
			Color.LightGreen,
			Color.LightGoldenrodYellow,
			Color.LightBlue,
			Color.LightCyan,
			Color.LightPink,
			Color.LightGray,
			Color.Magenta,
			Color.Orange,
			Color.DarkGreen,
			Color.Brown,
			Color.DarkBlue,
			Color.DarkCyan,
			Color.HotPink,
			Color.DarkGray,
			Color.DarkMagenta,
			Color.DarkOrange
		};
		static readonly object _ChannelColorsChangedKey=new object();
		static readonly object _SequenceChangedKey = new object();
		static readonly object _CursorColorChangedKey = new object();
		static readonly object _ShowCursorChangedKey = new object();
		static readonly object _CursorPositionChangedKey = new object();
		MidiSequence _sequence;
		Color[] _channelColors;
		Color _cursorColor;
		bool _showCursor;
		int _cursorPosition;
		/// <summary>
		/// Creates a new instance
		/// </summary>
		public MidiVisualizer()
		{
			this.SetStyle(ControlStyles.DoubleBuffer |
			 ControlStyles.UserPaint |
			 ControlStyles.AllPaintingInWmPaint,
			 true);
			this.UpdateStyles();
			_channelColors = new Color[16];
			_DefaultChannelColors.CopyTo(_channelColors,0);
			_cursorColor = Color.DarkGoldenrod;
			_showCursor = false;
			_cursorPosition = 0;
			BackColor = Color.Black;
			
			Thread.CurrentThread.Priority = ThreadPriority.Highest;
			
		}
		/// <summary>
		/// Indicates the color to use for drawing each channel
		/// </summary>
		[Description("Indicates the color to use for drawing each channel")]
		[Category("Appearance")]
		public Color[] ChannelColors {
			get { return _channelColors; }
			set { 
				// the designer lets you change the length of this array so what we do
				// is we copy the first 16 values. If it's less than 16, we take the 
				// remainder from the defaul colors.
				if(value.Length<16)
					Array.Copy(_DefaultChannelColors, value.Length, _channelColors, value.Length,16-value.Length);
				Array.Copy(value, 0, _channelColors, 0, value.Length);
				Invalidate();
				OnChannelColorsChanged(EventArgs.Empty);
			}
		}
		/// <summary>
		/// Raised when the value of ChannelColors changes
		/// </summary>
		[Description("Raised when the value of ChannelColors changes")]
		[Category("Behavior")]
		public event EventHandler ChannelColorsChanged {
			add { Events.AddHandler(_ChannelColorsChangedKey, value); }
			remove { Events.RemoveHandler(_ChannelColorsChangedKey, value); }
		}
		/// <summary>
		/// Called when the value of ChannelColors changes
		/// </summary>
		/// <param name="args">The event args</param>
		protected virtual void OnChannelColorsChanged(EventArgs args)
		{
			(Events[_ChannelColorsChangedKey] as EventHandler)?.Invoke(this, args);
		}
		/// <summary>
		/// Indicates the MIDI sequence to render
		/// </summary>
		[Description("Indicates the MIDI sequence to render")]
		[Category("Behavior")]
		public MidiSequence Sequence {
			get {
				return _sequence;
			}
			set {
				if (value != _sequence)
				{
					_sequence = value;
					Invalidate();
					OnSequenceChanged(EventArgs.Empty);
				}
			}
		}
		/// <summary>
		/// Raised when the value of Sequence changes
		/// </summary>
		[Description("Raised when the value of Sequence changes")]
		[Category("Behavior")]
		public event EventHandler SequenceChanged {
			add { Events.AddHandler(_SequenceChangedKey, value); }
			remove { Events.RemoveHandler(_SequenceChangedKey, value); }
		}
		/// <summary>
		/// Called when the value of Sequence changes
		/// </summary>
		/// <param name="args"></param>
		protected virtual void OnSequenceChanged(EventArgs args)
		{
			(Events[_SequenceChangedKey] as EventHandler)?.Invoke(this, args);
		}
		/// <summary>
		/// Indicates the color of the cursor
		/// </summary>
		[Description("Indicates the color of the cursor")]
		[Category("Appearance")]
		public Color CursorColor {
			get {
				return _cursorColor;
			}
			set {
				if(value!=_cursorColor)
				{
					_cursorColor = value;
					if(_showCursor)
						Invalidate();
					OnCursorColorChanged(EventArgs.Empty);
				}
			}
		}
		/// <summary>
		/// Raised when the value of CursorColor changes
		/// </summary>
		[Description("Raised when the value of CursorColor changes")]
		[Category("Behavior")]
		public event EventHandler CursorColorChanged {
			add { Events.AddHandler(_CursorColorChangedKey, value); }
			remove { Events.RemoveHandler(_CursorColorChangedKey, value); }
		}
		/// <summary>
		/// Called when the value of CursorColor changes
		/// </summary>
		/// <param name="args">The event args</param>
		protected virtual void OnCursorColorChanged(EventArgs args)
		{
			(Events[_CursorColorChangedKey] as EventHandler)?.Invoke(this, args);
		}
		/// <summary>
		/// Indicates the position of the cursor in MIDI ticks
		/// </summary>
		[Description("Indicates the position of the cursor in MIDI ticks")]
		[Category("Behavior")]
		[DefaultValue(0)]
		public int CursorPosition {
			get {
				return _cursorPosition;
			}
			set {
				if (value != _cursorPosition)
				{
					_cursorPosition = value;
					if (_showCursor)
						Invalidate();
					OnCursorPositionChanged(EventArgs.Empty);
				}
			}
		}
		/// <summary>
		/// Raised when the value of CursorPosition changes
		/// </summary>
		[Description("Raised when the value of CursorPosition changes")]
		[Category("Behavior")]
		public event EventHandler CursorPositionChanged {
			add { Events.AddHandler(_CursorPositionChangedKey, value); }
			remove { Events.RemoveHandler(_CursorPositionChangedKey, value); }
		}
		/// <summary>
		/// Called when the value of CursorPosition changes
		/// </summary>
		/// <param name="args">The event args</param>
		protected virtual void OnCursorPositionChanged(EventArgs args)
		{
			(Events[_CursorPositionChangedKey] as EventHandler)?.Invoke(this, args);
		}
		/// <summary>
		/// Indicates whether or not to show the cursor
		/// </summary>
		[Description("Indicates whether or not to show the cursor")]
		[Category("Appearance")]
		[DefaultValue(false)]
		public bool ShowCursor {
			get {
				return _showCursor;
			}
			set {
				if (value != _showCursor)
				{
					_showCursor = value;
					if (_showCursor)
						Invalidate();
					OnShowCursorChanged(EventArgs.Empty);
				}
			}
		}
		/// <summary>
		/// Raised when the value of ShowCursor changes
		/// </summary>
		[Description("Raised when the value of ShowCursor changes")]
		[Category("Behavior")]
		public event EventHandler ShowCursorChanged {
			add { Events.AddHandler(_ShowCursorChangedKey, value); }
			remove { Events.RemoveHandler(_ShowCursorChangedKey, value); }
		}
		/// <summary>
		/// Called when the value of ShowCursor changes
		/// </summary>
		/// <param name="args">The event args</param>
		protected virtual void OnShowCursorChanged(EventArgs args)
		{
			(Events[_ShowCursorChangedKey] as EventHandler)?.Invoke(this, args);
		}
		
		/// <summary>
		/// Paints the control
		/// </summary>
		/// <param name="args">The event arguments</param>
		protected override void OnPaint(PaintEventArgs args)
		{
			base.OnPaint(args);
			var g = args.Graphics;
			using (var brush = new SolidBrush(BackColor))
			{
				g.FillRectangle(brush, args.ClipRectangle);
			}
			if (null == _sequence)
				return;
			var len = 0;
			var minNote = 127;
			var maxNote = 0;
			foreach (var ev in _sequence.Events)
			{
				// found note on
				if(0x90==(ev.Message.Status & 0xF0))
				{
					var mw = ev.Message as MidiMessageWord;
					// update minimum and maximum notes
					if (minNote > mw.Data1)
						minNote = mw.Data1;
					if (maxNote < mw.Data1)
						maxNote = mw.Data1;
				}
				// update the length
				len += ev.Position;
			}
			if (0 == len || minNote > maxNote)
				return;
			
			// with what we just gathered now we have the scaling:
			var pptx = Width / (double)len;
			var ppty = Height / ((maxNote - minNote) + 1);
			var crect = args.ClipRectangle;

			// get a note map for easy drawing
			var noteMap = _sequence.ToNoteMap();
			for(var i = 0;i<noteMap.Count;++i)
			{
				var note = noteMap[i];
				var x = unchecked((int)Math.Round(note.Position * pptx)) + 1;
				if (x > crect.X + crect.Width)
					break; // we're done because there's nothing left within the visible area
				var y = Height - (note.NoteId - minNote + 1) * ppty - 1;
				var w = unchecked((int)Math.Round(note.Length * pptx));
				var h = ppty;
				if (crect.IntersectsWith(new Rectangle(x, y, w, h)))
				{
					// choose the color based on the note's channel
					using (var brush = new SolidBrush(_channelColors[note.Channel]))
					{
						// draw our rect based on scaling and note pos and len
						g.FillRectangle(
							brush,
							x,
							y,
							w,
							h);
						// 3d effect, but it slows down rendering a lot.
						// should be okay since we're only rendering
						// a small window at once usually
						if (2 < ppty && 2 < w)
						{
							using(var pen = new Pen(Color.FromArgb(127,Color.White)))
							{
								g.DrawLine(pen, x, y, w + x, y);
								g.DrawLine(pen, x, y+1, x, y+h-1);
							}
							using (var pen = new Pen(Color.FromArgb(127, Color.Black)))
							{
								g.DrawLine(pen, x, y+h-1, w + x, y+h-1);
								g.DrawLine(pen, x+w, y + 1, x+w, y + h);
							}
						}
					}
				}
				var xt = unchecked((int)Math.Round(_cursorPosition * pptx));
				var currect = new Rectangle(xt, 0, unchecked((int)Math.Max(pptx, 1)), Height);
				if (_showCursor && crect.IntersectsWith(currect) && -1<_cursorPosition&& _cursorPosition<len)
					using(var curBrush = new SolidBrush(_cursorColor))
						g.FillRectangle(curBrush, currect);
			}
		}
		/// <summary>
		/// Called when the control is resized
		/// </summary>
		/// <param name="args">The event arguments</param>
		protected override void OnResize(EventArgs args)
		{
			base.OnResize(args);
			Invalidate();
		}
		
	}
}
