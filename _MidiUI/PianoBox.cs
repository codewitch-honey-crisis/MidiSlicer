using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace M
{
    public class PianoBox : Control
    {
        static readonly object _OctavesChangedKey = new object();
        static readonly object _OrientationChangedKey = new object();
        static readonly object _NoteOnColorChangedKey = new object();
        static readonly object _WhiteKeyColorChangedKey = new object();
        static readonly object _BlackKeyColorChangedKey = new object();
        static readonly object _BorderColorChangedKey = new object();
        static readonly object _PianoKeyDownKey = new object();
        static readonly object _PianoKeyUpKey = new object();
        int _octaves=1;
        bool[] _keys = new bool[12];
        int _keyDown = -1;
        // cheesy way to do this but it works
        List<Rectangle> _keyRects = new List<Rectangle>();
        Orientation _orientation = System.Windows.Forms.Orientation.Horizontal;
        Color _noteOnColor = Color.Orange;
        Color _whiteKeyColor = Color.White;
        Color _blackKeyColor = Color.Black;
        Color _borderColor = Color.Black;
        public PianoBox()
        {
            DoubleBuffered = true;
        }
        /// <summary>
        /// Indicates the number of octaves to be represented
        /// </summary>
        public int Octaves {
            get { return _octaves;  }
            set {
                if (1 > value || 12<value)
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
        /// Raised with the value of Octaves changes
        /// </summary>
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
            (Events[_OctavesChangedKey] as EventHandler)?.Invoke(this,args);
        }

        /// <summary>
        /// Indicates the orientation of the control
        /// </summary>
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
        /// Raised with the value of Orientation changes
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
        /// Indicates the note on color
        /// </summary>
        public Color NoteOnColor {
            get { return _noteOnColor; }
            set {
                if (value != _noteOnColor)
                {
                    _noteOnColor = value;
                    Refresh();
                    OnNoteOnColorChanged(EventArgs.Empty);
                }
            }
        }
        /// <summary>
        /// Raised with the value of NoteOnColor changes
        /// </summary>
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
        /// Raised with the value of WhiteKeyColor changes
        /// </summary>
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
        /// Raised with the value of BlackKeyColor changes
        /// </summary>
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
        /// Raised with the value of BorderColor changes
        /// </summary>
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

        
        public bool GetKey(int key)
        {
            if (0 > key || _octaves * 12 <= key)
                throw new ArgumentOutOfRangeException("key");
            return _keys[key];
        }
        public void SetKey(int key, bool value)
        {
            if (0 > key || _octaves * 12 <= key)
                throw new ArgumentOutOfRangeException("key");
            if(value)
            {
                if(!_keys[key])
                {
                    _keys[key] = true;
                    OnPianoKeyDown(new PianoKeyEventArgs(key));
                }
            } else
            {
                if (_keys[key])
                {
                    _keys[key] = false;
                    OnPianoKeyUp(new PianoKeyEventArgs(key));
                }
            }
        }
        
        public event PianoKeyEventHandler PianoKeyDown {
            add { Events.AddHandler(_PianoKeyDownKey, value); }
            remove { Events.RemoveHandler(_PianoKeyDownKey, value); }
        }
        protected virtual void OnPianoKeyDown(PianoKeyEventArgs args)
        {
            (Events[_PianoKeyDownKey] as PianoKeyEventHandler)?.Invoke(this, args);
        }
        public event PianoKeyEventHandler PianoKeyUp {
            add { Events.AddHandler(_PianoKeyUpKey, value); }
            remove { Events.RemoveHandler(_PianoKeyUpKey, value); }
        }
        protected virtual void OnPianoKeyUp(PianoKeyEventArgs args)
        {
            (Events[_PianoKeyUpKey] as PianoKeyEventHandler)?.Invoke(this, args);
        }
        /// <summary>
        /// Retrieves a list of all the key states
        /// </summary>
        public bool[] Keys {
            get {
                var result = new bool[_keys.Length];
                _keys.CopyTo(result, 0);
                return result;
            }
        }

        protected override void OnPaintBackground(PaintEventArgs args)
        {
            base.OnPaintBackground(args);
            _keyRects.Clear();
            var g = args.Graphics;
            var rect = new Rectangle(0, 0, Width-1, Height-1);
            using (var brush = new SolidBrush(_whiteKeyColor)) 
                g.FillRectangle(brush, rect);
            using (var brush = new SolidBrush(_blackKeyColor))
            {
                using (var pen = new Pen(_borderColor))
                {
                    g.DrawRectangle(pen, rect);
                    var whiteKeyCount = 7 * _octaves;
                    if (Orientation.Horizontal == _orientation)
                    {
                        var wkw = Width / whiteKeyCount;
                        var bkw = unchecked((int)Math.Max(3, wkw * .666666));
                        for (var i = 1; i < whiteKeyCount; ++i)
                        {
                            var x = i * wkw;
                            var k = i % 7;
                            if (3 != k && 0!=k)
                            {
                                g.DrawRectangle(pen, x - (bkw / 2), 0, bkw, unchecked((int)(Height * .666666))+1);
                                g.FillRectangle(brush, x - (bkw / 2)+1, 1, bkw-1, unchecked((int)(Height * .666666)));
                                g.DrawLine(pen, x, 1 + unchecked((int)(Height * .666666)), x, Height - 2);
                            }
                            else
                                g.DrawLine(pen, x, 1, x, Height - 2);
                        }
                    } else
                    {
                        var wkh = Height / whiteKeyCount;
                        var bkh = unchecked((int)Math.Max(3, wkh * .666666));
                        for (var i = 1; i < whiteKeyCount; ++i)
                        {
                            var y = i * wkh;
                            var k = i % 7;
                            if (4 != k && 0!=k)
                            {
                                g.DrawRectangle(pen,0, y - (bkh / 2) , unchecked((int)(Width * .666666)) , bkh - 1);
                                g.FillRectangle(brush, 1, y - (bkh / 2)+1, unchecked((int)(Width* .666666))-1,bkh-2);
                                g.DrawLine(pen, 1 + unchecked((int)(Width * .666666)),y,Width - 2,y);
                            }
                            else
                                g.DrawLine(pen, 1,y, Width - 2,y);
                        }
                    }
                }
            }
        }
        bool _IsWhiteKey(int key)
        {
            key = key % 12;
            switch(key)
            {
                case 0:
                case 2:
                case 4:
                case 5:
                case 7:
                case 9:
                case 11:
                    return true;
            }
            return false;
        }
        protected override void OnPaint(PaintEventArgs args)
        {
            base.OnPaint(args);
            var g = args.Graphics;
            var whiteKeyCount = 7 * _octaves;
            using (var brush = new SolidBrush(_noteOnColor))
            {
                if (Orientation.Horizontal == _orientation)
                {
                    var wkw = Width / whiteKeyCount;
                    var bkw = unchecked((int)Math.Max(3, wkw * .666666));
                    for(var i = 0;i<_octaves;++i)
                    {
                        var root = 12 * i;
                        if (_keys[0+root])
                        {
                            var ofs = i * 7 * wkw;
                            g.FillRectangle(brush, 1+ofs, 1, wkw - (bkw / 2)-1, unchecked((int)(Height * .666666)));
                            g.FillRectangle(brush, 1+ofs, unchecked((int)(Height * .666666)) + 1, wkw - 1, Height - unchecked((int)(Height * .666666)) - 2);
                        } 
                        if(_keys[1+root])
                        {
                            var ofs = (1+i * 7) * wkw - (bkw/2);
                            g.FillRectangle(brush, 1 + ofs, 1, bkw - 2, unchecked((int)(Height * .666666)) - 1);
                        }
                        if (_keys[2+root])
                        {
                            var ofs = (1+ i * 7) * wkw;
                            g.FillRectangle(brush, ofs + (bkw / 2),1, (bkw/2)+1,  unchecked((int)(Height * .666666)));
                            g.FillRectangle(brush, 1 + ofs, unchecked((int)(Height * .666666)) + 1, wkw - 1, Height - unchecked((int)(Height * .666666)) - 2);
                        }
                        if (_keys[3 + root])
                        {
                            var ofs = (2 + i * 7) * wkw - (bkw / 2);
                            g.FillRectangle(brush, 1 + ofs, 1, bkw - 2, unchecked((int)(Height * .666666)) - 1);
                        }
                        if (_keys[4 + root])
                        {
                            var ofs = (2 + i * 7) * wkw;
                            g.FillRectangle(brush, (bkw/2) + ofs, 1, wkw - (bkw / 2), unchecked((int)(Height * .666666)));
                            g.FillRectangle(brush, 1 + ofs, unchecked((int)(Height * .666666)) + 1, wkw - 1, Height - unchecked((int)(Height * .666666)) - 2);
                        }
                        if (_keys[5 + root])
                        {
                            var ofs = (3 + i * 7) * wkw;
                            g.FillRectangle(brush, 1 + ofs, 1, wkw - (bkw / 2)-1, unchecked((int)(Height * .666666)));
                            g.FillRectangle(brush, 1 + ofs, unchecked((int)(Height * .666666)) + 1, wkw - 1, Height - unchecked((int)(Height * .666666)) - 2);
                        }
                        if (_keys[6 + root])
                        {
                            var ofs = (4 + i * 7) * wkw - (bkw / 2);
                            g.FillRectangle(brush, 1 + ofs, 1, bkw - 2, unchecked((int)(Height * .666666)) - 1);
                        }
                        if (_keys[7 + root])
                        {
                            var ofs = (4 + i * 7) * wkw;
                            g.FillRectangle(brush, ofs + (bkw / 2), 1, (bkw / 2) + 1, unchecked((int)(Height * .666666)));
                            g.FillRectangle(brush, 1 + ofs, unchecked((int)(Height * .666666)) + 1, wkw - 1, Height - unchecked((int)(Height * .666666)) - 2);
                        }
                        if (_keys[8 + root])
                        {
                            var ofs = (5 + i * 7) * wkw - (bkw / 2);
                            g.FillRectangle(brush, 1 + ofs, 1, bkw - 2, unchecked((int)(Height * .666666)) - 1);
                        }
                        if (_keys[9 + root])
                        {
                            var ofs = (5 + i * 7) * wkw;
                            g.FillRectangle(brush, ofs + (bkw / 2), 1, (bkw / 2) + 1, unchecked((int)(Height * .666666)));
                            g.FillRectangle(brush, 1 + ofs, unchecked((int)(Height * .666666)) + 1, wkw - 1, Height - unchecked((int)(Height * .666666)) - 2);
                        }
                        if (_keys[10 + root])
                        {
                            var ofs = (6 + i * 7) * wkw - (bkw / 2);
                            g.FillRectangle(brush, 1 + ofs, 1, bkw - 2, unchecked((int)(Height * .666666)) - 1);
                        }
                        if (_keys[11 + root])
                        {
                            var ofs = (6 + i * 7) * wkw;
                            var w = wkw - (bkw / 2);
                            if (_octaves - 1 == i)
                                w = Width - ofs - (bkw/2)-1;
                            g.FillRectangle(brush, (bkw / 2) + ofs, 1, w, unchecked((int)(Height * .666666)));
                            w = wkw - 1;
                            if (_octaves - 1 == i)
                                w = Width - ofs - 2;
                            g.FillRectangle(brush, 1 + ofs, unchecked((int)(Height * .666666)) + 1, w, Height - unchecked((int)(Height * .666666)) - 2);
                        }
                    }    
                } else // vertical
                {
                    var wkh = Height / whiteKeyCount;
                    var bkh = unchecked((int)Math.Max(3, wkh * .666666));
                    for (var i = 0; i < _octaves; ++i)
                    {
                        var root = 12 * i;
                        if (_keys[0 + root])
                        {
                            var hd = 1;
                            if(0==i)
                            {
                                hd = Height - (_octaves * 7 * wkh);
                            }
                            var ofs = ((_octaves-i-1) * 7 +6)* wkh;
                            g.FillRectangle(brush, 1, ofs+(bkh/2)+1, unchecked((int)(Width * .666666)), wkh - (bkh / 2) +hd-2);
                            g.FillRectangle(brush, unchecked((int)(Width * .666666)) + 1,1 + ofs, Width - unchecked((int)(Width * .666666)) - 2,wkh -2+hd);
                        }
                        if (_keys[1 + root])
                        {
                            var ofs = ( (_octaves-i-1) * 7+6) * wkh - (bkh / 2);
                            g.FillRectangle(brush, 1, 1 + ofs, unchecked((int)(Width * .666666)) - 1, bkh - 2);
                        }
                        if (_keys[2 + root])
                        {
                            var ofs = ( (_octaves-i-1) * 7+5) * wkh;
                            g.FillRectangle(brush,1, ofs + (bkh / 2)+1, unchecked((int)(Width* .666666)), (bkh / 2) + 1);
                            g.FillRectangle(brush,unchecked((int)(Width*.666666))+1, 1 + ofs, Width-unchecked((int)(Width * .666666))-2 , wkh - 1);
                        }
                        if (_keys[3 + root])
                        {
                            var ofs = ((_octaves - i - 1) * 7 + 5) * wkh - (bkh/2);
                            g.FillRectangle(brush, 1, 1 + ofs, unchecked((int)(Width * .666666)) - 1, bkh - 2);
                        }
                        if (_keys[4 + root])
                        {
                            var ofs = ((_octaves - i - 1) * 7 + 4) * wkh;

                            g.FillRectangle(brush, 1, 1+ofs  ,unchecked((int)(Width * .666666)), bkh );
                            g.FillRectangle(brush, unchecked((int)(Width * .666666)) + 1, 1 + ofs, Width - unchecked((int)(Width * .666666)) - 2, wkh - 1);
                        }
                        if (_keys[5 + root])
                        {
                            var ofs = ((_octaves - i - 1) * 7 + 3) * wkh;
                            g.FillRectangle(brush, 1, ofs + (bkh / 2) + 1, unchecked((int)(Width * .666666)), wkh - (bkh / 2) -1);
                            g.FillRectangle(brush, unchecked((int)(Width * .666666)) + 1, 1 + ofs, Width - unchecked((int)(Width * .666666)) - 2, wkh - 1);
                        }
                        if (_keys[6 + root])
                        {
                            var ofs = ((_octaves - i - 1) * 7 + 3) * wkh - (bkh / 2);
                            g.FillRectangle(brush, 1, 1 + ofs, unchecked((int)(Width * .666666)) - 1, bkh - 2);
                        }
                        if (_keys[7 + root])
                        {
                            var ofs = ((_octaves - i - 1) * 7 + 2) * wkh;
                            g.FillRectangle(brush, 1, ofs + (bkh / 2) + 1, unchecked((int)(Width * .666666)), (bkh / 2) + 1);
                            g.FillRectangle(brush, unchecked((int)(Width * .666666)) + 1, 1 + ofs, Width - unchecked((int)(Width * .666666)) - 2, wkh - 1);
                        }
                        if (_keys[8 + root])
                        {
                            var ofs = ((_octaves - i - 1) * 7 + 2) * wkh - (bkh / 2);
                            g.FillRectangle(brush, 1, 1 + ofs, unchecked((int)(Width * .666666)) - 1, bkh - 2);
                        }
                        if (_keys[9 + root])
                        {
                            var ofs = ((_octaves - i - 1) * 7 + 1) * wkh;
                            g.FillRectangle(brush, 1, ofs + (bkh / 2) + 1, unchecked((int)(Width * .666666)), (bkh / 2) + 1);
                            g.FillRectangle(brush, unchecked((int)(Width * .666666)) + 1, 1 + ofs, Width - unchecked((int)(Width * .666666)) - 2, wkh - 1);
                        }
                        if (_keys[10 + root])
                        {
                            var ofs = ((_octaves - i - 1) * 7 + 1) * wkh - (bkh / 2);
                            g.FillRectangle(brush, 1, 1 + ofs, unchecked((int)(Width * .666666)) - 1, bkh - 2);
                        }
                        if (_keys[11 + root])
                        {
                            var ofs = (_octaves - i - 1) * 7 * wkh;
                            g.FillRectangle(brush, 1, 1 + ofs, unchecked((int)(Width * .666666)), bkh);
                            g.FillRectangle(brush, unchecked((int)(Width * .666666)) + 1, 1 + ofs, Width - unchecked((int)(Width * .666666)) - 2, wkh - 1);
                        }
                    }
                }
            }
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
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
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if(e.Button==MouseButtons.Left)
            {
                var ht = _HitTest(e.X, e.Y);
                if (-1 < _keyDown && ht!=_keyDown)
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
                        Refresh();
                    }
                }
            }
        }
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
                    if (result >= _octaves * 12)
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
                    return result;
                }
            }
        }

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
    }
    public sealed class PianoKeyEventArgs : EventArgs
    {
        public int Key { get; }
        public PianoKeyEventArgs(int key)
        {
            Key = key;
        }
    }
    public delegate void PianoKeyEventHandler(object sender, PianoKeyEventArgs args);
}
