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
				if(0x90==(ev.Message.Status & 0xF0))
				{
					var mw = ev.Message as MidiMessageWord;
					if (minNote > mw.Data1)
						minNote = mw.Data1;
					if (maxNote < mw.Data1)
						maxNote = mw.Data1;
				}
				len += ev.Position;
			}
			if (0 == len || minNote > maxNote)
				return;
			var pptx = Width / (double)len;
			var ppty = Height / ((maxNote - minNote) + 1);
			var noteMap = _sequence.ToNoteMap();
			for(var i = 0;i<noteMap.Count;++i)
			{
				var note = noteMap[i];
				using(var brush = new SolidBrush(_channelColors[note.Channel]))
				{
					var n = note.NoteId - minNote;
					g.FillRectangle(brush, unchecked((int)Math.Round(note.Position * pptx))+1, Height-(n+1)* ppty-1, unchecked((int)Math.Round(note.Length * pptx)), ppty);
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
