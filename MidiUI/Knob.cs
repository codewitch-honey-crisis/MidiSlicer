using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace M
{
	/// <summary>
	/// Represents a knob control
	/// </summary>
	[Description("Represents a knob control")]
	[DefaultProperty("Value")]
	[DefaultEvent("ValueChanged")]
	public class Knob : Control
	{
		static readonly object _ValueChangedKey = new object();
		static readonly object _LargeChangeChangedKey = new object();
		static readonly object _MinimumChangedKey = new object();
		static readonly object _MaximumChangedKey = new object();
		static readonly object _BorderColorChangedKey = new object();
		static readonly object _KnobColorChangedKey = new object();
		static readonly object _PointerColorChangedKey = new object();
		static readonly object _PointerWidthChangedKey = new object();
		static readonly object _BorderWidthChangedKey = new object();
		static readonly object _PointerStartCapChangedKey = new object();
		static readonly object _PointerEndCapChangedKey = new object();
		static readonly object _MinimumAngleChangedKey = new object();
		static readonly object _MaximumAngleChangedKey = new object();
		static readonly object _HasTicksChangedKey = new object();
		static readonly object _TickHeightChangedKey = new object();
		static readonly object _TickWidthChangedKey = new object();
		static readonly object _TickColorChangedKey = new object();
		int _largeChange = 4;
		int _minimum = 0;
		int _maximum = 127;
		int _value=0;
		Color _borderColor = SystemColors.ControlDarkDark;
		Color _knobColor = SystemColors.Control;
		Color _pointerColor = SystemColors.ControlText;
		Color _tickColor = SystemColors.ControlDarkDark;
		int _pointerWidth = 1;
		int _borderWidth = 1;
		int _minimumAngle = 30;
		int _maximumAngle = 330;
		LineCap _pointerStartCap=LineCap.Round;
		LineCap _pointerEndCap=LineCap.Triangle;
		bool _dragging = false;
		Point _dragHit;
		bool _hasTicks = false;
		int _tickHeight = 2;
		int _tickWidth = 1;
		int[] _tickPositions;
		/// <summary>
		/// Creates a new instance of the control
		/// </summary>
		public Knob()
		{
			SetStyle(ControlStyles.Selectable,true);
			UpdateStyles();
			TabStop = true;
			DoubleBuffered = true;
			_RecomputeTicks();
		}
		/// <summary>
		/// Indicates the value of the control
		/// </summary>
		[Description("Indicates the value of the control")]
		[Category("Behavior")]
		[DefaultValue(0)]
		public int Value {
			get { return _value; }
			set {
				if (_value != value)
				{
					_value = value;
					Invalidate();
					OnValueChanged(EventArgs.Empty);
				}
			}
		}
		/// <summary>
		/// Raised with the value of Value changes
		/// </summary>
		[Description("Raised with the value of Value changes")]
		[Category("Behavior")]
		public event EventHandler ValueChanged {
			add { Events.AddHandler(_ValueChangedKey, value); }
			remove { Events.RemoveHandler(_ValueChangedKey, value); }
		}
		/// <summary>
		/// Called when the value of Value changes
		/// </summary>
		/// <param name="args">The event args to use</param>
		protected virtual void OnValueChanged(EventArgs args)
		{
			(Events[_ValueChangedKey] as EventHandler)?.Invoke(this, args);
		}

		/// <summary>
		/// Indicates the amount the control increments when the large modifiers are used
		/// </summary>
		[Description("Indicates the amount the control increments when the large modifiers are used")]
		[Category("Behavior")]
		[DefaultValue(4)]
		public int LargeChange {
			get { return _largeChange; }
			set {
				if (_largeChange != value)
				{
					_largeChange = value;
					_RecomputeTicks();
					Invalidate();
					OnLargeChangeChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Raised with the value of LargeChange changes
		/// </summary>
		[Description("Raised with the value of LargeChange changes")]
		[Category("Behavior")]
		public event EventHandler LargeChangeChanged {
			add { Events.AddHandler(_LargeChangeChangedKey, value); }
			remove { Events.RemoveHandler(_LargeChangeChangedKey, value); }
		}
		/// <summary>
		/// Called when the value of LargeChange changes
		/// </summary>
		/// <param name="args">The event args to use</param>
		protected virtual void OnLargeChangeChanged(EventArgs args)
		{
			(Events[_LargeChangeChangedKey] as EventHandler)?.Invoke(this, args);
		}

		/// <summary>
		/// Indicates the minimum value for the control
		/// </summary>
		[Description("Indicates the minimum value for the control")]
		[Category("Behavior")]
		[DefaultValue(1)]
		public int Minimum {
			get { return _minimum; }
			set {
				if (_minimum != value)
				{
					_minimum = value;
					_RecomputeTicks();
					Invalidate();
					OnMinimumChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Raised with the value of Minimum changes
		/// </summary>
		[Description("Raised with the value of Minimum changes")]
		[Category("Behavior")]
		public event EventHandler MinimumChanged {
			add { Events.AddHandler(_MinimumChangedKey, value); }
			remove { Events.RemoveHandler(_MinimumChangedKey, value); }
		}
		/// <summary>
		/// Called when the value of Minimum changes
		/// </summary>
		/// <param name="args">The event args to use</param>
		protected virtual void OnMinimumChanged(EventArgs args)
		{
			(Events[_MinimumChangedKey] as EventHandler)?.Invoke(this, args);
		}

		/// <summary>
		/// Indicates the maximum value for the control
		/// </summary>
		[Description("Indicates the maximum value for the control")]
		[Category("Behavior")]
		[DefaultValue(127)]
		public int Maximum {
			get { return _maximum; }
			set {
				if (_maximum != value)
				{
					_maximum = value;
					_RecomputeTicks();
					Invalidate();
					OnMaximumChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Raised with the value of Maximum changes
		/// </summary>
		[Description("Raised with the value of Maximum changes")]
		[Category("Behavior")]
		public event EventHandler MaximumChanged {
			add { Events.AddHandler(_MaximumChangedKey, value); }
			remove { Events.RemoveHandler(_MaximumChangedKey, value); }
		}
		/// <summary>
		/// Called when the value of Maximum changes
		/// </summary>
		/// <param name="args">The event args to use</param>
		protected virtual void OnMaximumChanged(EventArgs args)
		{
			(Events[_MaximumChangedKey] as EventHandler)?.Invoke(this, args);
		}
		/// <summary>
		/// Indicates the border color
		/// </summary>
		[Description("Indicates the border color of the control")]
		[Category("Appearance")]
		public Color BorderColor {
			get { return _borderColor; }
			set {
				if (value != _borderColor)
				{
					_borderColor = value;
					Invalidate();
					OnBorderColorChanged(EventArgs.Empty);
				}
			}
		}
		/// <summary>
		/// Raised when the value of BorderColor changes
		/// </summary>
		[Description("Raised when the value of BorderColor changes")]
		[Category("Behavior")]
		public event EventHandler BorderColorChanged {
			add { Events.AddHandler(_BorderColorChangedKey, value); }
			remove { Events.RemoveHandler(_BorderColorChangedKey, value); }
		}
		/// <summary>
		/// Called when the value of BorderColor changes
		/// </summary>
		/// <param name="args">The event args (not used)</param>
		protected virtual void OnBorderColorChanged(EventArgs args)
		{
			(Events[_BorderColorChangedKey] as EventHandler)?.Invoke(this, args);
		}
		/// <summary>
		/// Indicates the knob color
		/// </summary>
		[Description("Indicates the knob color of the control")]
		[Category("Appearance")]
		public Color KnobColor {
			get { return _knobColor; }
			set {
				if (value != _knobColor)
				{
					_knobColor = value;
					Invalidate();
					OnKnobColorChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Raised when the value of KnobColor changes
		/// </summary>
		[Description("Raised when the value of KnobColor changes")]
		[Category("Behavior")]
		public event EventHandler KnobColorChanged {
			add { Events.AddHandler(_KnobColorChangedKey, value); }
			remove { Events.RemoveHandler(_KnobColorChangedKey, value); }
		}
		/// <summary>
		/// Called when the value of KnobColor changes
		/// </summary>
		/// <param name="args">The event args (not used)</param>
		protected virtual void OnKnobColorChanged(EventArgs args)
		{
			(Events[_KnobColorChangedKey] as EventHandler)?.Invoke(this, args);
		}
		/// <summary>
		/// Indicates the pointer color
		/// </summary>
		[Description("Indicates the pointer color of the control")]
		[Category("Appearance")]
		public Color PointerColor {
			get { return _pointerColor; }
			set {
				if (value != _pointerColor)
				{
					_pointerColor = value;
					Invalidate();
					OnPointerColorChanged(EventArgs.Empty);
				}
			}
		}
		/// <summary>
		/// Raised when the value of PointerColor changes
		/// </summary>
		[Description("Raised when the value of PointerColor changes")]
		[Category("Behavior")]
		public event EventHandler PointerColorChanged {
			add { Events.AddHandler(_PointerColorChangedKey, value); }
			remove { Events.RemoveHandler(_PointerColorChangedKey, value); }
		}
		/// <summary>
		/// Called when the value of PointerColor changes
		/// </summary>
		/// <param name="args">The event args (not used)</param>
		protected virtual void OnPointerColorChanged(EventArgs args)
		{
			(Events[_PointerColorChangedKey] as EventHandler)?.Invoke(this, args);
		}
		/// <summary>
		/// Indicates the pointer width of the control
		/// </summary>
		[Description("Indicates the pointer width of the control")]
		[Category("Appearance")]
		[DefaultValue(1)]
		public int PointerWidth {
			get { return _pointerWidth; }
			set {
				if (_pointerWidth != value)
				{
					_pointerWidth = value;
					Invalidate();
					OnPointerWidthChanged(EventArgs.Empty);
				}
			}
		}
		/// <summary>
		/// Raised with the value of PointerWidth changes
		/// </summary>
		[Description("Raised with the value of PointerWidth changes")]
		[Category("Behavior")]
		public event EventHandler PointerWidthChanged {
			add { Events.AddHandler(_PointerWidthChangedKey, value); }
			remove { Events.RemoveHandler(_PointerWidthChangedKey, value); }
		}
		/// <summary>
		/// Called when the value of PointerWidth changes
		/// </summary>
		/// <param name="args">The event args to use</param>
		protected virtual void OnPointerWidthChanged(EventArgs args)
		{
			(Events[_PointerWidthChangedKey] as EventHandler)?.Invoke(this, args);
		}
		/// <summary>
		/// Indicates the border width of the control
		/// </summary>
		[Description("Indicates the border width of the control")]
		[Category("Appearance")]
		[DefaultValue(1)]
		public int BorderWidth {
			get { return _borderWidth; }
			set {
				if (_borderWidth != value)
				{
					_borderWidth = value;
					Invalidate();
					OnBorderWidthChanged(EventArgs.Empty);
				}
			}
		}
		/// <summary>
		/// Raised with the value of BorderWidth changes
		/// </summary>
		[Description("Raised with the borderWidth of BorderWidth changes")]
		[Category("Behavior")]
		public event EventHandler BorderWidthChanged {
			add { Events.AddHandler(_BorderWidthChangedKey, value); }
			remove { Events.RemoveHandler(_BorderWidthChangedKey, value); }
		}
		/// <summary>
		/// Called when the value of BorderWidth changes
		/// </summary>
		/// <param name="args">The event args to use</param>
		protected virtual void OnBorderWidthChanged(EventArgs args)
		{
			(Events[_BorderWidthChangedKey] as EventHandler)?.Invoke(this, args);
		}
		/// <summary>
		/// Indicates the pointer start line cap of the control
		/// </summary>
		[Description("Indicates the pointer start line cap of the control")]
		[Category("Appearance")]
		[DefaultValue(LineCap.Round)]
		public LineCap PointerStartCap {
			get { return _pointerStartCap; }
			set {
				if (_pointerStartCap != value)
				{
					_pointerStartCap = value;
					Invalidate();
					OnPointerStartCapChanged(EventArgs.Empty);
				}
			}
		}
		/// <summary>
		/// Raised with the value of PointerStartCap changes
		/// </summary>
		[Description("Raised with the value of PointerStartCap changes")]
		[Category("Behavior")]
		public event EventHandler PointerStartCapChanged {
			add { Events.AddHandler(_PointerStartCapChangedKey, value); }
			remove { Events.RemoveHandler(_PointerStartCapChangedKey, value); }
		}
		/// <summary>
		/// Called when the value of PointerStartCap changes
		/// </summary>
		/// <param name="args">The event args to use</param>
		protected virtual void OnPointerStartCapChanged(EventArgs args)
		{
			(Events[_PointerStartCapChangedKey] as EventHandler)?.Invoke(this, args);
		}
		/// <summary>
		/// Indicates the pointer end line cap of the control
		/// </summary>
		[Description("Indicates the pointer end line cap of the control")]
		[Category("Appearance")]
		[DefaultValue(LineCap.Triangle)]
		public LineCap PointerEndCap {
			get { return _pointerEndCap; }
			set {
				if (_pointerEndCap != value)
				{
					_pointerEndCap = value;
					Invalidate();
					OnPointerEndCapChanged(EventArgs.Empty);
				}
			}
		}
		/// <summary>
		/// Raised with the value of PointerEndCap changes
		/// </summary>
		[Description("Raised with the value of PointerEndCap changes")]
		[Category("Behavior")]
		public event EventHandler PointerEndCapChanged {
			add { Events.AddHandler(_PointerEndCapChangedKey, value); }
			remove { Events.RemoveHandler(_PointerEndCapChangedKey, value); }
		}
		/// <summary>
		/// Called when the value of PointerEndCap changes
		/// </summary>
		/// <param name="args">The event args to use</param>
		protected virtual void OnPointerEndCapChanged(EventArgs args)
		{
			(Events[_PointerEndCapChangedKey] as EventHandler)?.Invoke(this, args);
		}
		/// <summary>
		/// Indicates the minimum value for the control
		/// </summary>
		[Description("Indicates the minimum angle for the control")]
		[Category("Behavior")]
		[DefaultValue(30)]
		public int MinimumAngle {
			get { return _minimumAngle; }
			set {
				if (_minimumAngle != value)
				{
					_minimumAngle = value;
					Invalidate();
					OnMinimumAngleChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Raised with the value of MinimumAngle changes
		/// </summary>
		[Description("Raised with the value of MinimumAngle changes")]
		[Category("Behavior")]
		public event EventHandler MinimumAngleChanged {
			add { Events.AddHandler(_MinimumAngleChangedKey, value); }
			remove { Events.RemoveHandler(_MinimumAngleChangedKey, value); }
		}
		/// <summary>
		/// Called when the value of MinimumAngle changes
		/// </summary>
		/// <param name="args">The event args to use</param>
		protected virtual void OnMinimumAngleChanged(EventArgs args)
		{
			(Events[_MinimumAngleChangedKey] as EventHandler)?.Invoke(this, args);
		}

		/// <summary>
		/// Indicates the maximum angle for the control
		/// </summary>
		[Description("Indicates the maximum angle for the control")]
		[Category("Behavior")]
		[DefaultValue(330)]
		public int MaximumAngle {
			get { return _maximumAngle; }
			set {
				if (_maximumAngle != value)
				{
					_maximumAngle = value;
					Invalidate();
					OnMaximumAngleChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Raised with the value of MaximumAngle changes
		/// </summary>
		[Description("Raised with the value of MaximumAngle changes")]
		[Category("Behavior")]
		public event EventHandler MaximumAngleChanged {
			add { Events.AddHandler(_MaximumAngleChangedKey, value); }
			remove { Events.RemoveHandler(_MaximumAngleChangedKey, value); }
		}
		/// <summary>
		/// Called when the value of MaximumAngle changes
		/// </summary>
		/// <param name="args">The event args to use</param>
		protected virtual void OnMaximumAngleChanged(EventArgs args)
		{
			(Events[_MaximumAngleChangedKey] as EventHandler)?.Invoke(this, args);
		}
		internal bool TicksVisible {
			get { return _hasTicks && 0<_tickHeight && 0<_tickWidth; }
		}
		/// <summary>
		/// Indicates whether or not the control displays tick marks
		/// </summary>
		[Description("Indicates whether or not the control displays tick marks")]
		[Category("Appearance")]
		[DefaultValue(false)]
		public bool HasTicks {
			get { return _hasTicks; }
			set {
				if (_hasTicks != value)
				{
					_hasTicks = value;
					Invalidate();
					OnHasTicksChanged(EventArgs.Empty);
				}
			}
		}
		/// <summary>
		/// Raised with the value of HasTicks changes
		/// </summary>
		[Description("Raised with the value of HasTicks changes")]
		[Category("Behavior")]
		public event EventHandler HasTicksChanged {
			add { Events.AddHandler(_HasTicksChangedKey, value); }
			remove { Events.RemoveHandler(_HasTicksChangedKey, value); }
		}
		/// <summary>
		/// Called when the value of HasTicks changes
		/// </summary>
		/// <param name="args">The event args to use</param>
		protected virtual void OnHasTicksChanged(EventArgs args)
		{
			(Events[_HasTicksChangedKey] as EventHandler)?.Invoke(this, args);
		}

		/// <summary>
		/// Indicates the height of the tick marks
		/// </summary>
		[Description("Indicates the height of the tick marks")]
		[Category("Appearance")]
		[DefaultValue(2)]
		public int TickHeight {
			get { return _tickHeight; }
			set {
				if (_tickHeight != value)
				{
					_tickHeight = value;
					Invalidate();
					OnTickHeightChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Raised with the value of TickHeight changes
		/// </summary>
		[Description("Raised with the value of TickHeight changes")]
		[Category("Behavior")]
		public event EventHandler TickHeightChanged {
			add { Events.AddHandler(_TickHeightChangedKey, value); }
			remove { Events.RemoveHandler(_TickHeightChangedKey, value); }
		}
		/// <summary>
		/// Called when the value of TickHeight changes
		/// </summary>
		/// <param name="args">The event args to use</param>
		protected virtual void OnTickHeightChanged(EventArgs args)
		{
			(Events[_TickHeightChangedKey] as EventHandler)?.Invoke(this, args);
		}
		/// <summary>
		/// Indicates the width of the tick marks
		/// </summary>
		[Description("Indicates the width of the tick marks")]
		[Category("Appearance")]
		[DefaultValue(1)]
		public int TickWidth {
			get { return _tickWidth; }
			set {
				if (_tickWidth != value)
				{
					_tickWidth = value;
					Invalidate();
					OnTickWidthChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Raised with the value of TickWidth changes
		/// </summary>
		[Description("Raised with the value of TickWidth changes")]
		[Category("Behavior")]
		public event EventHandler TickWidthChanged {
			add { Events.AddHandler(_TickWidthChangedKey, value); }
			remove { Events.RemoveHandler(_TickWidthChangedKey, value); }
		}
		/// <summary>
		/// Called when the value of TickWidth changes
		/// </summary>
		/// <param name="args">The event args to use</param>
		protected virtual void OnTickWidthChanged(EventArgs args)
		{
			(Events[_TickWidthChangedKey] as EventHandler)?.Invoke(this, args);
		}

		/// <summary>
		/// Indicates the color of the tick marks
		/// </summary>
		[Description("Indicates the color of the tick marks")]
		[Category("Appearance")]
		public Color TickColor {
			get { return _tickColor; }
			set {
				if (_tickColor != value)
				{
					_tickColor = value;
					Invalidate();
					OnTickColorChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Raised with the value of TickColor changes
		/// </summary>
		[Description("Raised with the value of TickColor changes")]
		[Category("Behavior")]
		public event EventHandler TickColorChanged {
			add { Events.AddHandler(_TickColorChangedKey, value); }
			remove { Events.RemoveHandler(_TickColorChangedKey, value); }
		}
		/// <summary>
		/// Called when the value of TickColor changes
		/// </summary>
		/// <param name="args">The event args to use</param>
		protected virtual void OnTickColorChanged(EventArgs args)
		{
			(Events[_TickColorChangedKey] as EventHandler)?.Invoke(this, args);
		}

		/// <summary>
		/// Called when the control needs to be painted
		/// </summary>
		/// <param name="args">The event args</param>
		protected override void OnPaint(PaintEventArgs args)
		{
			const double PI = 3.141592653589793238462643d;
			base.OnPaint(args);
			var g = args.Graphics;
			float knobMinAngle = _minimumAngle;
			float knobMaxAngle = _maximumAngle;
			
			if (knobMinAngle < 0)
				knobMinAngle = 360 + knobMinAngle;
			if (knobMaxAngle <= 0)
				knobMaxAngle = 360 + knobMaxAngle;
			double ofs = 0.0;
			int mi = Minimum, mx = Maximum;
			var kr = (knobMaxAngle - knobMinAngle);
			double vr = mx - mi;
			double rr = kr / vr;
			if (0 > mi)
				ofs = -mi;
			var crr = ClientRectangle;
			// adjust the client rect so it doesn't overhang
			crr.Inflate(-1, -1);
			var orr = crr;
			if(TicksVisible)
			{
				crr.Inflate(new Size(-_tickHeight-2, -_tickHeight-2));
				
			}
			var size = (float)Math.Min(crr.Width-4, crr.Height-4);
			
			crr.X += 2;
			crr.Y += 2;
			var radius = size / 2f;
			var origin = new PointF(crr.Left +radius, crr.Top + radius);
			var brf = _GetCircleRect(origin.X, origin.Y, (radius - (_borderWidth / 2)));
			var rrf = _GetCircleRect(origin.X, origin.Y, (radius - (_borderWidth)));
			
			double q = ((Value + ofs) * rr) + knobMinAngle;
			double angle = (q + 90d);
			if (angle > 360.0)
				angle -= 360.0;
			double angrad = angle * (PI / 180d);
			double adj = 1;
			if (_pointerEndCap != LineCap.NoAnchor)
				adj += (_pointerWidth) / 2d;

			var x2 = (float)(origin.X + (radius - adj) * (float)Math.Cos(angrad));
			var y2 = (float)(origin.Y + (radius - adj) * (float)Math.Sin(angrad));

			using (var backBrush = new SolidBrush(BackColor))
			{
				using (var bgBrush = new SolidBrush(_knobColor))
				{
					using (var borderPen = new Pen(_borderColor, _borderWidth))
					{
						using (var pointerPen = new Pen(_pointerColor, _pointerWidth))
						{
							pointerPen.StartCap = _pointerStartCap;
							pointerPen.EndCap = _pointerEndCap;


							g.SmoothingMode = SmoothingMode.AntiAlias;

							// erase the background so it antialiases properly
							g.FillRectangle(backBrush, (float)orr.Left - 1, (float)orr.Top - 1, (float)orr.Width + 2, (float)orr.Height + 2);
							g.DrawEllipse(borderPen, brf); // draw the border
							g.FillEllipse(bgBrush, rrf);
							g.DrawLine(pointerPen, origin.X, origin.Y, x2, y2);
						}
					}
				}
			}
			
			if (TicksVisible)
			{
				using (var pen = new Pen(_tickColor, _tickWidth))
				{
					for (var i = 0; i < _tickPositions.Length; ++i)
					{
						var qq = ((_tickPositions[i] + ofs) * rr) + knobMinAngle + 90d;
						if (qq > 360.0)
							qq -= 360.0;
						var agrad = qq * (PI / 180d);
						var x1 = origin.X + (radius +2) * (float)Math.Cos(agrad);
						var y1 = origin.Y + (radius + 2) * (float)Math.Sin(agrad);
						x2 = origin.X + (radius + _tickHeight+2) * (float)Math.Cos(agrad);
						y2 = origin.Y + (radius + _tickHeight+2) * (float)Math.Sin(agrad);
						g.DrawLine(pen, x1, y1, x2, y2);
					}
				}
			}
			if (Focused)
				ControlPaint.DrawFocusRectangle(g, new Rectangle(0, 0, Math.Min(Width,Height), Math.Min(Width,Height)));
		}
		
		/// <summary>
		/// Called when a mouse button is pressed
		/// </summary>
		/// <param name="args"></param>
		protected override void OnMouseDown(MouseEventArgs args)
		{
			Focus();
			if (MouseButtons.Left == (args.Button & MouseButtons.Left)) {
				var crr = ClientRectangle;
				// adjust the client rect so it doesn't overhang
				crr.Inflate(-1, -1);
				if (TicksVisible)
					crr.Inflate(-_tickHeight-2, -_tickHeight-2);
				var size = (float)Math.Min(crr.Width - 4, crr.Height - 4);
				crr.X += 2;
				crr.Y += 2;
				var radius = size / 2f;
				var origin = new PointF(crr.Left + radius, crr.Top + radius);
				if (radius > _LineDistance(origin, new PointF(args.X, args.Y)))
				{

					_dragHit = args.Location;
					if (!_dragging)
					{
						_dragging = true;
						Focus();
						Invalidate();
					}
				}
			}
			base.OnMouseDown(args);
		}
		/// <summary>
		/// Called when a mouse button is released
		/// </summary>
		/// <param name="args">The event args</param>
		protected override void OnMouseUp(MouseEventArgs args)
		{
			// TODO: Implement Ctrl+Drag
			if (_dragging)
			{
				_dragging = false;
				int pos = Value;
				pos += _dragHit.Y - args.Location.Y; // delta
				int min=Minimum;
				int max=Maximum;
				if (pos < min) pos = min;
				if (pos > max) pos = max;
				Value = pos;
			}
			base.OnMouseUp(args);
		}
		/// <summary>
		/// Called when a mouse button is moved
		/// </summary>
		/// <param name="args">The event args</param>
		protected override void OnMouseMove(MouseEventArgs args)
		{
			// TODO: Improve Ctrl+Drag
			if (_dragging)
			{
				int opos = Value;
				int pos = opos;
				var delta = _dragHit.Y - args.Location.Y;
				if (Keys.Control == (ModifierKeys & Keys.Control))
					delta *= LargeChange;
				pos += delta;
				int min = Minimum;
				int max = Maximum;
				if (pos < min) pos = min;
				if (pos > max) pos = max;
				if (pos != opos)
				{
					if(Keys.Control==( ModifierKeys & Keys.Control))
					{
						var t = _tickPositions[0];
						var setVal = false;
						for(var i = 1;i<_tickPositions.Length;i++)
						{
							var t2 = _tickPositions[i]-1;
							if(pos>=t && pos<=t2)
							{
								var l = pos - t;
								var l2 = t2 - pos;
								if (l <= l2)
									Value = t;
								else
									Value = t2;
								setVal = true;
								break;
							}
							t = _tickPositions[i];
						}
						if (!setVal)
							Value = Maximum;
						
					} else
						Value = pos;
					_dragHit = args.Location;
				}
			}
			base.OnMouseMove(args);
		}
		/// <summary>
		/// Called when the mouse wheel is scrolled
		/// </summary>
		/// <param name="args">The event args</param>
		protected override void OnMouseWheel(MouseEventArgs args)
		{
			int pos;
			int m;
			var delta = args.Delta;
			if (0 < delta)
			{
				delta = 1;
				pos = Value;
				pos += delta;
				m = Maximum;
				if (pos > m)
					pos = m;
				Value = pos;
			}
			else if (0 > delta)
			{
				delta = -1;
				pos = Value;
				pos += delta;
				m = Minimum;
				if (pos < m)
					pos = m;
				Value = pos;

			}
			base.OnMouseWheel(args);
		}
		/// <summary>
		/// Called when a key is pressed
		/// </summary>
		/// <param name="args">The event args</param>
		protected override void OnKeyDown(KeyEventArgs args)
		{
			Focus();
			if(Keys.PageDown==(args.KeyCode & Keys.PageDown))
			{
				var v = Value;
				var i = 0;
				for(;i<_tickPositions.Length;i++)
				{
					var t = _tickPositions[i];
					if (t >= v)
						break;
				}
				if (1 > i)
					i = 1;
				Value = _tickPositions[i - 1];
			}
			if (Keys.PageUp == (args.KeyCode & Keys.PageUp))
			{
				var v = Value;
				var i = 0;
				for (; i < _tickPositions.Length; i++)
				{
					var t = _tickPositions[i];
					if (t > v)
						break;
				}
				if (_tickPositions.Length <= i)
					i = _tickPositions.Length - 1;
				Value = _tickPositions[i];
			}

			if (Keys.Home == (args.KeyCode & Keys.Home))
			{
				Value = Minimum;
			}
			if (Keys.End== (args.KeyCode & Keys.End))
			{
				Value = Maximum;
			}
			base.OnKeyDown(args);
		}
		/// <summary>
		/// Called when a command key is pressed
		/// </summary>
		/// <param name="msg">The message</param>
		/// <param name="keyData">The command key(s)</param>
		/// <returns>True if handled, otherwise false</returns>
		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			Focus();
			int pos;
			var handled = false;

			// BUG: Right arrow doesn't seem to be working!
			if (Keys.Up == (keyData & Keys.Up) || Keys.Right == (keyData & Keys.Right))
			{
				pos = Value+1;
				if (pos < Maximum)
				{
					Value = pos;
				}
				else
					Value = Maximum;
				handled = true;
			}
			
			if (Keys.Down == (keyData & Keys.Down) || Keys.Left == (keyData & Keys.Left))
			{
				pos = Value-1;
				if (pos > Minimum)
				{
					Value = pos;
				}
				else
					Value = Minimum;
				handled = true;
			}
			if (handled)
				return true;
			return base.ProcessCmdKey(ref msg, keyData);
		}
		/// <summary>
		/// Called when the control is resized
		/// </summary>
		/// <param name="args">The event arguments</param>
		protected override void OnResize(EventArgs args)
		{
			Invalidate();
			base.OnResize(args);
		}
		/// <summary>
		/// Called when the control's size changes
		/// </summary>
		/// <param name="args">The event args</param>
		protected override void OnSizeChanged(EventArgs args)
		{
			SuspendLayout();
			if (Height > Width)
				Height = Width;
			else if (Height < Width)
				Width = Height;
			ResumeLayout(true);
			base.OnSizeChanged(args);
		}
		/// <summary>
		/// Called when the control receives focus
		/// </summary>
		/// <param name="args">The event args</param>
		protected override void OnEnter(EventArgs args)
		{
			Invalidate();
			base.OnEnter(args);
		}
		/// <summary>
		/// Called when the control loses focus
		/// </summary>
		/// <param name="args">The event args</param>
		protected override void OnLeave(EventArgs args)
		{
			Invalidate();
			base.OnLeave(args);
		}
		
		static RectangleF _GetCircleRect(float x, float y, float r)
		{
			return new RectangleF(x - r, y - r, r * 2, r * 2);
		}
		static float _LineDistance(PointF p1, PointF p2)
		{
			var xdist = p1.X - p2.X;
			var ydist = p1.Y - p2.Y;
			return (float)Math.Sqrt(xdist * xdist + ydist * ydist);
		}
		void _RecomputeTicks()
		{
			var tickCount = (int)Math.Ceiling((double)((Maximum - Minimum + 1) / _largeChange));
			var ticks = new int[tickCount+1];
			ticks[0] = 0;
			var t = Minimum;
			for(var i = 1;i<ticks.Length;i++)
			{
				t += _largeChange;
				t = Math.Min(t, Maximum);
				ticks[i] = t;
			}
			_tickPositions = ticks;
		}
	}
}
