using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace M
{
    using WKeys = System.Windows.Forms.Keys;
    
    /// <summary>
    /// Indicates the sets of default keys to use for piano key hot keys
    /// </summary>
    public enum PianoBoxHotKeyDefaults
    {
        /// <summary>
        /// Use the top two rows of the keyboard
        /// </summary>
        TopRows,
        /// <summary>
        /// Use the bottom two rows of the keyboard
        /// </summary>
        BottomRows
    }
    /// <summary>
    /// Represents an interactive piano keyboard
    /// </summary>
    public class PianoBox : Control
    {
        // evanthandler keys for the events
        static readonly object _OctavesChangedKey = new object();
        static readonly object _OrientationChangedKey = new object();
        static readonly object _NoteOnColorChangedKey = new object();
        static readonly object _WhiteKeyColorChangedKey = new object();
        static readonly object _BlackKeyColorChangedKey = new object();
        static readonly object _BorderColorChangedKey = new object();
        static readonly object _HotKeysChangedKey = new object();
        static readonly object _PianoKeyDownKey = new object();
        static readonly object _PianoKeyUpKey = new object();

        static readonly Keys[][] _KeyDefaults = new Keys[][] { 
            new Keys[]
            {
                WKeys.Q,WKeys.D2,WKeys.W,WKeys.D3,WKeys.E,
                WKeys.R,WKeys.D5,WKeys.T,WKeys.D6,WKeys.Y,WKeys.D7,WKeys.U
            },
            new Keys[]
            {
                WKeys.Z,WKeys.S,WKeys.X,WKeys.D,WKeys.C,
                WKeys.V,WKeys.G,WKeys.B,WKeys.H,WKeys.N,WKeys.J,WKeys.M
            }
        };
        // member stuff
        int _octaves = 1;
        bool[] _keys = new bool[12];
        int _keyDown = -1;
        Orientation _orientation = Orientation.Horizontal;
        Color _noteHighlightColor = Color.Orange;
        Color _whiteKeyColor = Color.White;
        Color _blackKeyColor = Color.Black;
        Color _borderColor = Color.Black;
        Keys[] _hotKeys = new Keys[0];
        /// <summary>
        /// Creates  a new instance
        /// </summary>
        public PianoBox()
        {
            SetStyle(ControlStyles.Selectable, true);
            TabStop = true;
            DoubleBuffered = true;
            MapHotKeyDefaultsToOctave(PianoBoxHotKeyDefaults.TopRows, 4);
            MapHotKeyDefaultsToOctave(PianoBoxHotKeyDefaults.BottomRows, 5);
        }
        /// <summary>
        /// Indicates the number of octaves to be represented
        /// </summary>
        [Description("Indicates the number of octaves to be represented")]
        [Category("Behavior")]
        [DefaultValue(1)]
        public int Octaves {
            get { return _octaves; }
            set {
                if (1 > value || 12 < value)
                    throw new ArgumentOutOfRangeException();
                if (value != _octaves)
                {
                    _octaves = value;
                    var keys = new bool[_octaves * 12];
                    Array.Copy(_keys, 0, keys, 0, Math.Min(keys.Length, _keys.Length));
                    _keys = keys;
                    Refresh();
                    OnOctavesChanged(EventArgs.Empty);
                }
            }
        }
        /// <summary>
        /// Raised when the value of Octaves changes
        /// </summary>
        [Description("Raised when the value of Octaves changes")]
        [Category("Behavior")]
        public event EventHandler OctavesChanged {
            add { Events.AddHandler(_OctavesChangedKey, value); }
            remove { Events.RemoveHandler(_OctavesChangedKey, value); }
        }
        /// <summary>
        /// Called when the value of Octaves changes
        /// </summary>
        /// <param name="args">The event args (not used)</param>
        protected virtual void OnOctavesChanged(EventArgs args)
        {
            (Events[_OctavesChangedKey] as EventHandler)?.Invoke(this, args);
        }

        /// <summary>
        /// Indicates the orientation of the control
        /// </summary>
        [Description("Indicates the orientation of the control")]
        [Category("Appearance")]
        [DefaultValue(Orientation.Horizontal)]
        public Orientation Orientation {
            get { return _orientation; }
            set {
                if (value != _orientation)
                {
                    _orientation = value;
                    Refresh();
                    OnOrientationChanged(EventArgs.Empty);
                }
            }
        }
        /// <summary>
        /// Raised when the value of Orientation changes
        /// </summary>
        public event EventHandler OrientationChanged {
            add { Events.AddHandler(_OrientationChangedKey, value); }
            remove { Events.RemoveHandler(_OrientationChangedKey, value); }
        }
        /// <summary>
        /// Called when the value of Orientation changes
        /// </summary>
        /// <param name="args">The event args (not used)</param>
        protected virtual void OnOrientationChanged(EventArgs args)
        {
            (Events[_OrientationChangedKey] as EventHandler)?.Invoke(this, args);
        }

        /// <summary>
        /// Indicates the note highlight color
        /// </summary>
        [Description("Indicates the note highlight color")]
        [Category("Appearance")]
        public Color NoteHighlightColor {
            get { return _noteHighlightColor; }
            set {
                if (value != _noteHighlightColor)
                {
                    _noteHighlightColor = value;
                    Refresh();
                    OnNoteOnColorChanged(EventArgs.Empty);
                }
            }
        }
        /// <summary>
        /// Raised when the value of NoteOnColor changes
        /// </summary>
        [Description("Raised when the value of NoteOnColor changes")]
        [Category("Behavior")]
        public event EventHandler NoteOnColorChanged {
            add { Events.AddHandler(_NoteOnColorChangedKey, value); }
            remove { Events.RemoveHandler(_NoteOnColorChangedKey, value); }
        }
        /// <summary>
        /// Called when the value of NoteOnColor changes
        /// </summary>
        /// <param name="args">The event args (not used)</param>
        protected virtual void OnNoteOnColorChanged(EventArgs args)
        {
            (Events[_NoteOnColorChangedKey] as EventHandler)?.Invoke(this, args);
        }

        /// <summary>
        /// Indicates the white key color
        /// </summary>
        [Description("Indicates the white key color")]
        [Category("Appearance")]
        public Color WhiteKeyColor {
            get { return _whiteKeyColor; }
            set {
                if (value != _whiteKeyColor)
                {
                    _whiteKeyColor = value;
                    Refresh();
                    OnWhiteKeyColorChanged(EventArgs.Empty);
                }
            }
        }
        /// <summary>
        /// Raised when the value of WhiteKeyColor changes
        /// </summary>
        [Description("Raised when the value of WhiteKeyColor changes")]
        [Category("Behavior")]
        public event EventHandler WhiteKeyColorChanged {
            add { Events.AddHandler(_WhiteKeyColorChangedKey, value); }
            remove { Events.RemoveHandler(_WhiteKeyColorChangedKey, value); }
        }
        /// <summary>
        /// Called when the value of WhiteKeyColor changes
        /// </summary>
        /// <param name="args">The event args (not used)</param>
        protected virtual void OnWhiteKeyColorChanged(EventArgs args)
        {
            (Events[_WhiteKeyColorChangedKey] as EventHandler)?.Invoke(this, args);
        }
        /// <summary>
        /// Indicates the black key color
        /// </summary>
        [Description("Indicates the black key color of the control")]
        [Category("Appearance")]
        public Color BlackKeyColor {
            get { return _blackKeyColor; }
            set {
                if (value != _blackKeyColor)
                {
                    _blackKeyColor = value;
                    Refresh();
                    OnBlackKeyColorChanged(EventArgs.Empty);
                }
            }
        }
        /// <summary>
        /// Raised when the value of BlackKeyColor changes
        /// </summary>
        [Description("Raised when the value of BlackKeyColor changes")]
        [Category("Behavior")]
        public event EventHandler BlackKeyColorChanged {
            add { Events.AddHandler(_BlackKeyColorChangedKey, value); }
            remove { Events.RemoveHandler(_BlackKeyColorChangedKey, value); }
        }
        /// <summary>
        /// Called when the value of BlackKeyColor changes
        /// </summary>
        /// <param name="args">The event args (not used)</param>
        protected virtual void OnBlackKeyColorChanged(EventArgs args)
        {
            (Events[_BlackKeyColorChangedKey] as EventHandler)?.Invoke(this, args);
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
                    Refresh();
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
        /// Indicates the hotkey controls for the keyboard
        /// </summary>
        [Description("Indicates the hotkey controls for the keyboard")]
        [Category("Behavior")]
        public Keys[] HotKeys {
            get {
                return _hotKeys;
            }
            set {
                _hotKeys = value;
                OnHotKeysChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Copies a set of hotkey defaults to the specified octave
        /// </summary>
        /// <param name="defaultSet">A <see cref="PianoBoxHotKeyDefaults"/> value that indicates which set of defaults to use</param>
        /// <param name="octave">The octave to map the default set tp</param>
        public void MapHotKeyDefaultsToOctave(PianoBoxHotKeyDefaults defaultSet,int octave)
        {
            var minKeysCount = (octave + 1) * 12;            
            if(null==_hotKeys || _hotKeys.Length<minKeysCount)
            {
                var keys = new Keys[minKeysCount];
                if(null!=_hotKeys)
                    Array.Copy(_hotKeys, 0, keys, 0, _hotKeys.Length);
                _hotKeys = keys;
            }
            Array.Copy(_KeyDefaults[(int)defaultSet], 0, _hotKeys, 12 * octave, 12);
        }
        /// <summary>
        /// Raised when the value of HotKeys changes
        /// </summary>
        [Description("Raised when the value of HotKeys changes")]
        [Category("Behavior")]
        public event EventHandler HotKeysChanged {
            add { Events.AddHandler(_HotKeysChangedKey, value); }
            remove { Events.RemoveHandler(_HotKeysChangedKey, value); }
        }
        /// <summary>
        /// Called when the value of HotKeys changes
        /// </summary>
        /// <param name="args">The <see cref="EventArgs"/> to use</param>
        protected virtual void OnHotKeysChanged(EventArgs args)
        {
            (Events[_HotKeysChangedKey] as EventHandler)?.Invoke(this, args);
        }
        /// <summary>
        /// Retrieves the state of the specified key
        /// </summary>
        /// <param name="key">The key id</param>
        /// <returns>True if the key is pressed, otherwise false</returns>
        public bool GetKey(int key)
        {
            if (0 > key || _octaves * 12 <= key)
                throw new ArgumentOutOfRangeException("key");
            return _keys[key];
        }
        /// <summary>
        /// Sets the state of a key
        /// </summary>
        /// <param name="key">They key id</param>
        /// <param name="value">True if the key should be pressed, otherwise false</param>
        /// <param name="suppressEvent">True if no event should be raised from this action, otherwise false</param>
        public void SetKey(int key, bool value, bool suppressEvent=false)
        {
            if (0 > key || _octaves * 12 <= key)
                throw new ArgumentOutOfRangeException("key");
            if (value)
            {
                if (!_keys[key])
                {
                    _keys[key] = true;
                    Refresh();
                    if(!suppressEvent)
                        OnPianoKeyDown(new PianoKeyEventArgs(key));
                }
            }
            else
            {
                if (_keys[key])
                {
                    _keys[key] = false;
                    Refresh();
                    if (!suppressEvent)
                        OnPianoKeyUp(new PianoKeyEventArgs(key));
                }
            }
        }
        /// <summary>
        /// Raised when a piano key is struck
        /// </summary>
        [Description("Raised when a piano key is struck")]
        [Category("Action")]
        public event PianoKeyEventHandler PianoKeyDown {
            add { Events.AddHandler(_PianoKeyDownKey, value); }
            remove { Events.RemoveHandler(_PianoKeyDownKey, value); }
        }
        /// <summary>
        /// Called when a piano key is struck
        /// </summary>
        /// <param name="args">The <see cref="PianoKeyEventArgs"/> to use</param>
        protected virtual void OnPianoKeyDown(PianoKeyEventArgs args)
        {
            (Events[_PianoKeyDownKey] as PianoKeyEventHandler)?.Invoke(this, args);
        }
        /// <summary>
        /// Raised when a piano key is released
        /// </summary>
        [Description("Raised when a piano key is released")]
        [Category("Action")]
        public event PianoKeyEventHandler PianoKeyUp {
            add { Events.AddHandler(_PianoKeyUpKey, value); }
            remove { Events.RemoveHandler(_PianoKeyUpKey, value); }
        }
        /// <summary>
        /// Called when a piano key is released
        /// </summary>
        /// <param name="args">The <see cref="PianoKeyEventArgs"/> to use</param>
        protected virtual void OnPianoKeyUp(PianoKeyEventArgs args)
        {
            (Events[_PianoKeyUpKey] as PianoKeyEventHandler)?.Invoke(this, args);
        }
        /// <summary>
        /// Indicates the current state of each of the keys
        /// </summary>
        [Browsable(false)]
        public bool[] Keys {
            get {
                var result = new bool[_keys.Length];
                _keys.CopyTo(result, 0);
                return result;
            }
        }
        
        /// <summary>
        /// Called when the control is painted
        /// </summary>
        /// <param name="args">The <see cref="PaintEventArgs"/> to use</param>
        protected override void OnPaint(PaintEventArgs args)
        {
            base.OnPaint(args);
            var g = args.Graphics;
            var rect = new Rectangle(0, 0, Width - 1, Height - 1);
            using (var brush = new SolidBrush(_whiteKeyColor))
                g.FillRectangle(brush,args.ClipRectangle);
            // there are 7 white keys per octave
            var whiteKeyCount = 7 * _octaves;
            int key;
            // first we must paint the highlighted portions
            // TODO: Only paint if it's inside the ClipRectangle
            using (var selBrush = new SolidBrush(_noteHighlightColor))
            {
                if (Orientation.Horizontal == _orientation)
                {
                    var wkw = Width / whiteKeyCount;
                    var bkw = unchecked((int)Math.Max(3, wkw * .666666));
                    key = 0;
                    var ox = 0;
                    for (var i = 1; i < whiteKeyCount; ++i)
                    {
                        var x = i * wkw;
                        var k = i % 7;
                        if (3 != k && 0 != k)
                        {
                            if(_keys[key])
                                g.FillRectangle(selBrush, ox+1, 1, wkw - 1, Height-2);
                            ++key;
                            if(_keys[key])
                                g.FillRectangle(selBrush, x - (bkw / 2) + 1, 1, bkw - 1, unchecked((int)(Height * .666666)));
                            ++key;
                            if (_keys[key])
                                g.FillRectangle(selBrush, x, 1, wkw - 1, Height - 2);
                        }
                        else
                        {
                            if(_keys[key])
                                g.FillRectangle(selBrush, ox + 1, 1, wkw - 1, Height - 2);
                            ++key;
                            if(_keys[key])
                                g.FillRectangle(selBrush, x, 1, wkw - 1, Height - 2);
                        }
                        ox = x;
                    }
                    if(_keys[_keys.Length-1])
                    {
                        g.FillRectangle(selBrush, ox, 1, Width-ox- 1, Height - 2);
                    }
                } else // vertical 
                {
                    var wkh = Height / whiteKeyCount;
                    var bkh = unchecked((int)Math.Max(3, wkh * .666666));
                    key = _keys.Length-1;
                    var oy = 0;
                    for (var i = 1; i < whiteKeyCount; ++i)
                    {
                        var y = i * wkh;
                        var k = i % 7;
                        if (4 != k && 0 != k)
                        {
                            if (_keys[key])
                                g.FillRectangle(selBrush, 1, oy + 1, Width - 2, wkh - 1);
                            --key;
                            if(_keys[key])
                                g.FillRectangle(selBrush, 1, y - (bkh / 2) + 1, unchecked((int)(Width * .666666)) - 1, bkh - 2);
                            --key;
                            if(_keys[key])
                                g.FillRectangle(selBrush, 1, y , Width - 2, wkh - 1);
                        }
                        else
                        {
                            if (_keys[key])
                                g.FillRectangle(selBrush, 1, oy + 1, Width - 2, wkh - 1);
                            --key;
                            if(_keys[key])
                                g.FillRectangle(selBrush, 1, y, Width - 2, wkh - 1);
                        }
                        oy = y; 
                    }
                    if (_keys[0])
                    {
                        g.FillRectangle(selBrush, 1,oy, Width - 2, Height - oy-1);
                    }
                }
                // Now paint the black keys and the borders between keys
                using (var brush = new SolidBrush(_blackKeyColor))
                {
                    using (var pen = new Pen(_borderColor))
                    {
                        g.DrawRectangle(pen, rect);
                        if (Focused)
                        {
                            var rect2 = rect;
                            rect2.Inflate(-2, -2);
                            ControlPaint.DrawFocusRectangle(g,rect2);
                        }
                        if (Orientation.Horizontal == _orientation)
                        {
                            var wkw = Width / whiteKeyCount;
                            var bkw = unchecked((int)Math.Max(3, wkw * .666666));
                            key = 0;
                            for (var i = 1; i < whiteKeyCount; ++i)
                            {
                                var x = i * wkw;
                                var k = i % 7;
                                if (3 != k && 0 != k)
                                {
                                    g.DrawRectangle(pen, x - (bkw / 2), 0, bkw, unchecked((int)(Height * .666666)) + 1);
                                    ++key;
                                    if (!_keys[key])
                                        g.FillRectangle(brush, x - (bkw / 2) + 1, 1, bkw - 1, unchecked((int)(Height * .666666)));
                                    g.DrawLine(pen, x, 1 + unchecked((int)(Height * .666666)), x, Height - 2);
                                    ++key;
                                }
                                else
                                {
                                    g.DrawLine(pen, x, 1, x, Height - 2);
                                    ++key;
                                }
                            }
                        }
                        else // vertical
                        {
                            var wkh = Height / whiteKeyCount;
                            var bkh = unchecked((int)Math.Max(3, wkh * .666666));
                            key = _keys.Length - 1;
                            for (var i = 1; i < whiteKeyCount; ++i)
                            {
                                var y = i * wkh;
                                var k = i % 7;
                                if (4 != k && 0 != k)
                                {
                                    g.DrawRectangle(pen, 0, y - (bkh / 2), unchecked((int)(Width * .666666)), bkh - 1);
                                    --key;
                                    if(!_keys[key])
                                        g.FillRectangle(brush, 1, y - (bkh / 2) + 1, unchecked((int)(Width * .666666)) - 1, bkh - 2);
                                    g.DrawLine(pen, 1 + unchecked((int)(Width * .666666)), y, Width - 2, y);
                                    --key;
                                }
                                else
                                {
                                    g.DrawLine(pen, 1, y, Width - 2, y);
                                    --key;
                                }
                            }
                        }
                    }
                }
            }
        }
        
        /// <summary>
        /// Called when a key is pressed
        /// </summary>
        /// <param name="e">The event args</param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            Focus();
            base.OnKeyDown(e);
            if (null == _hotKeys)
                return;
            var i = Array.IndexOf(_hotKeys,e.KeyCode);
            if(-1<i)
                SetKey(i, true);
        }
        /// <summary>
        /// Called when a key is released
        /// </summary>
        /// <param name="e">The event args</param>
        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
            if (null == _hotKeys)
                return;
            var i = Array.IndexOf(_hotKeys, e.KeyCode);
            if (-1 < i)
                SetKey(i, false);
        }
        /// <summary>
        /// Called when a mouse button is pressed
        /// </summary>
        /// <param name="e">The <see cref="MouseEventArgs"/> to use</param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            Focus();
            base.OnMouseDown(e);
            FindForm().ActiveControl = this; 
            
            var ht = _HitTest(e.X, e.Y);
            if (ht != _keyDown)
            {
                _keyDown = ht;
                var b = _keys[_keyDown];
                if (!b)
                {
                    _keys[_keyDown] = true;
                    OnPianoKeyDown(new PianoKeyEventArgs(_keyDown));
                    Refresh();
                }
            }
            
        }
        /// <summary>
        /// Called when the mouse is moved
        /// </summary>
        /// <param name="e">The <see cref="MouseEventArgs"/> to use</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (MouseButtons.Left == e.Button)
            {
                var ht = _HitTest(e.X, e.Y);
                if (-1 < _keyDown && ht != _keyDown)
                {
                    var b = _keys[_keyDown];
                    if (b)
                    {
                        _keys[_keyDown] = false;
                        OnPianoKeyUp(new PianoKeyEventArgs(_keyDown));
                    }
                    _keyDown = ht;
                    b = _keys[_keyDown];
                    if (!b)
                    {
                        _keys[_keyDown] = true;
                        OnPianoKeyDown(new PianoKeyEventArgs(_keyDown));
                    }
                    Refresh();
                }
            }
            
        }
        /// <summary>
        /// Called when a mouse button is released
        /// </summary>
        /// <param name="e">The <see cref="MouseEventArgs"/> to use</param>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (0 > _keyDown)
                return;
            var b = _keys[_keyDown];
            if (b)
            {
                _keys[_keyDown] = false;
                OnPianoKeyUp(new PianoKeyEventArgs(_keyDown));
                Refresh();
            }
            
            _keyDown = -1;
        }
        // returns the key for a position
        int _HitTest(int x, int y)
        {
            var result = -1;
            var whiteKeyCount = 7 * _octaves;
            if (Orientation.Horizontal == Orientation)
            {
                var wkw = Width / whiteKeyCount;
                var oct = (x / wkw) / 7;
                if (y > Height * .666666)
                {
                    switch ((x / wkw) % 7)
                    {
                        case 0:
                            result = oct * 12;
                            break;
                        case 1:
                            result = oct * 12 + 2;
                            break;
                        case 2:
                            result = oct * 12 + 4;
                            break;
                        case 3:
                            result = oct * 12 + 5;
                            break;
                        case 4:
                            result = oct * 12 + 7;
                            break;
                        case 5:
                            result = oct * 12 + 9;
                            break;
                        case 6:
                            result = oct * 12 + 11;
                            break;
                    }
                    if (0 > result)
                        result = 0;
                    else if (result >= _octaves * 12)
                        result = _octaves * 12 - 1;
                    return result;

                }
                else
                {
                    var bkw = unchecked((int)(wkw * .666666));
                    switch ((x / wkw) % 7)
                    {
                        case 0:
                            if ((x % (wkw * 7)) > wkw - (bkw / 2) - 1)
                                result = oct * 12 + 1;
                            else
                                result = oct * 12;
                            break;
                        case 1:
                            if ((x % (wkw * 7)) < wkw + (bkw / 2))
                                result = oct * 12 + 1;
                            else if ((x % (wkw * 7)) > (wkw * 2) - (bkw / 2) - 1)
                                result = oct * 12 + 3;
                            else
                                result = oct * 12 + 2;
                            break;
                        case 2:
                            if ((x % (wkw * 7)) < (wkw * 2) + (bkw / 2))
                                result = oct * 12 + 3;
                            else
                                result = oct * 12 + 4;
                            break;
                        case 3:
                            if ((x % (wkw * 7)) > (wkw * 4) - (bkw / 2) - 1)
                                result = oct * 12 + 6;
                            else
                                result = oct * 12 + 5;
                            break;
                        case 4:
                            if ((x % (wkw * 7)) < (wkw * 4) + (bkw / 2))
                                result = oct * 12 + 6;
                            else if ((x % (wkw * 7)) > (wkw * 5) - (bkw / 2) - 1)
                                result = oct * 12 + 8;
                            else
                                result = oct * 12 + 7;
                            break;
                        case 5:
                            if ((x % (wkw * 7)) < (wkw * 5) + (bkw / 2))
                                result = oct * 12 + 8;
                            else if ((x % (wkw * 7)) > (wkw * 6) - (bkw / 2) - 1)
                                result = oct * 12 + 10;
                            else
                                result = oct * 12 + 9;
                            break;
                        case 6:
                            if ((x % (wkw * 7)) < (wkw * 6) + (bkw / 2))
                                result = oct * 12 + 10;
                            else
                                result = oct * 12 + 11;
                            break;
                    }
                    if (0 > result)
                        result = 0;
                    else if (result >= _octaves * 12)
                        result = _octaves * 12 - 1;
                    return result;
                }
            }
            else // vertical
            {
                var wkh = Height / whiteKeyCount;
                var oct = (_octaves - 1) - (y / wkh / 7);
                if (x > Width * .666666)
                {
                    switch ((y / wkh) % 7)
                    {
                        case 0:
                            result = oct * 12 + 11;
                            break;
                        case 1:
                            result = oct * 12 + 9;
                            break;
                        case 2:
                            result = oct * 12 + 7;
                            break;
                        case 3:
                            result = oct * 12 + 5;
                            break;
                        case 4:
                            result = oct * 12 + 4;
                            break;
                        case 5:
                            result = oct * 12 + 2;
                            break;
                        case 6:
                            result = oct * 12 + 0;
                            break;
                    }
                    if (0 > result)
                        result = 0;
                    else if (result >= _octaves * 12)
                        result = _octaves * 12 - 1;
                    return result;
                }
                else
                {
                    var bkh = unchecked((int)(wkh * .666666));
                    switch ((y / wkh) % 7)
                    {
                        case 0:
                            if ((y % (wkh * 7)) > wkh - (bkh / 2) - 1)
                                result = oct * 12 + 10;
                            else
                                result = oct * 12 + 11;
                            break;
                        case 1:
                            if ((y % (wkh * 7)) < wkh + (bkh / 2))
                                result = oct * 12 + 10;
                            else if ((y % (wkh * 7)) > (wkh * 2) - (bkh / 2) - 1)
                                result = oct * 12 + 8;
                            else
                                result = oct * 12 + 9;
                            break;
                        case 2:
                            if ((y % (wkh * 7)) < (wkh * 2) + (bkh / 2))
                                result = oct * 12 + 8;
                            else if ((y % (wkh * 7)) > (wkh * 3) - (bkh / 2) - 1)
                                result = oct * 12 + 6;
                            else
                                result = oct * 12 + 7;
                            break;
                        case 3:
                            if ((y % (wkh * 7)) < (wkh * 3) + (bkh / 2))
                                result = oct * 12 + 6;
                            else
                                result = oct * 12 + 5;
                            break;
                        case 4:
                            if ((y % (wkh * 7)) > (wkh * 5) - (bkh / 2) - 1)
                                result = oct * 12 + 3;
                            else
                                result = oct * 12 + 4;
                            break;
                        case 5:
                            if ((y % (wkh * 7)) < (wkh * 5) + (bkh / 2))
                                result = oct * 12 + 3;
                            else if ((y % (wkh * 7)) > (wkh * 6) - (bkh / 2) - 1)
                                result = oct * 12 + 1;
                            else
                                result = oct * 12 + 2;
                            break;
                        case 6:
                            if ((y % (wkh * 7)) < (wkh * 6) + (bkh / 2))
                                result = oct * 12 + 1;
                            else
                                result = oct * 12;
                            break;
                    }
                    if (0 > result)
                        result = 0;
                    else if (result >= _octaves * 12)
                        result = _octaves * 12 - 1;
                    return result;
                }
            }
        }
    }
    /// <summary>
    /// Represents the event arguments for a piano key related event
    /// </summary>
    public sealed class PianoKeyEventArgs : EventArgs
    {
        /// <summary>
        /// The target key
        /// </summary>
        public int Key { get; }
        /// <summary>
        /// Creates a new instance
        /// </summary>
        /// <param name="key">The target key</param>
        public PianoKeyEventArgs(int key)
        {
            Key = key;
        }
    }
    /// <summary>
    /// Indicates the handler for a piano key related event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public delegate void PianoKeyEventHandler(object sender, PianoKeyEventArgs args);
}
