using System;
using System.Windows.Forms;
using System.Drawing;
namespace M
{
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
		MidiSequence _sequence;
		Color[] _channelColors;		
		
		public MidiVisualizer()
		{
			_channelColors = new Color[16];
			_DefaultChannelColors.CopyTo(_channelColors,0);
		}

		public Color[] ChannelColors {
			get { return _channelColors; }
			set { 
				// the designer lets you change the length of this array so what we do
				// is we copy the first 16 values. If it's less than 16, we take the 
				// remainder from the defaul colors.
				if(value.Length<16)
					Array.Copy(_DefaultChannelColors, value.Length, _channelColors, value.Length,16-value.Length);
				Array.Copy(value, 0, _channelColors, 0, value.Length);
				Refresh();
				OnChannelColorsChanged(EventArgs.Empty);
			}
		}
		public event EventHandler ChannelColorsChanged {
			add { Events.AddHandler(_ChannelColorsChangedKey, value); }
			remove { Events.RemoveHandler(_ChannelColorsChangedKey, value); }
		}
		protected virtual void OnChannelColorsChanged(EventArgs args)
		{
			(Events[_ChannelColorsChangedKey] as EventHandler)?.Invoke(this, args);
		}
		protected override void OnPaintBackground(PaintEventArgs args)
		{
			base.OnPaintBackground(args);
			var g = args.Graphics;
			using (var brush = new SolidBrush(Color.Black))
			{
				g.FillRectangle(brush,0,0,Width,Height);
			}
		}
		protected override void OnPaint(PaintEventArgs args)
		{
			base.OnPaint(args);
			var g = args.Graphics;
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
			}
			
		}
		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);
			Refresh();
		}
		public MidiSequence Sequence { 
			get {
				return _sequence;
			}
			set {
				_sequence = value;
				Refresh();
			}
		}
		private void InitializeComponent()
		{
			this.SuspendLayout();
			this.ResumeLayout(false);

		}
	}
}
