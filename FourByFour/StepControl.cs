using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;

namespace FourByFour
{
	public partial class StepControl : UserControl
	{
		_StepList _steps;
		public StepControl()
		{
			InitializeComponent();
			CheckBox ch;
			var left = 0;
			for (var k = 0; k < 2; ++k)
			{
				for (var i = 0; i < 4; ++i)
				{
					for (var j = 0; j < 4; ++j)
					{
						ch = new CheckBox();
						Controls.Add(ch);
						ch.Appearance = Appearance.Button;
						ch.Size = new Size(16, Height);
						ch.Location = new Point(left, 0);
						ch.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
						left += ch.Size.Width;
					}
					left += 4;
				}
			}
			_steps = new _StepList(Controls);
			MinimumSize = new Size(left, MinimumSize.Height);
		}
		public IList<bool> Steps { get { return _steps; } }
		private sealed class _StepList : IList<bool>
		{
			readonly ControlCollection _controls;
			internal _StepList(ControlCollection controls)
			{
				_controls = controls;
			}
			public bool this[int index] 
			{
				get {
					var ch = _controls[index] as CheckBox;
					return ch.Checked;
				}
				set {
					var ch = _controls[index] as CheckBox;
					ch.Checked = value;
				}

			}

			public int Count { get { return _controls.Count; } }
			public bool IsReadOnly { get { return false; } }

			public void Add(bool item)
			{
				throw new NotSupportedException("The list is fixed size");
			}

			public void Clear()
			{
				throw new NotSupportedException("The list is fixed size");
			}

			public bool Contains(bool item)
			{
				for(int ic=_controls.Count,i = 0;i<ic;++i)
				{
					var ch = _controls[i] as CheckBox;
					if (item == ch.Checked)
						return true;
				}
				return false;
			}

			public void CopyTo(bool[] array, int arrayIndex)
			{
				for(int ic=_controls.Count,i=0;i<ic; ++i)
					array[arrayIndex + i] = (_controls[i] as CheckBox).Checked;
			}

			public IEnumerator<bool> GetEnumerator()
			{
				for (int ic = _controls.Count, i = 0; i < ic; ++i)
					yield return (_controls[i] as CheckBox).Checked;
			}

			public int IndexOf(bool item)
			{
				for (int ic = _controls.Count, i = 0; i < ic; ++i)
				{
					var ch = _controls[i] as CheckBox;
					if (item == ch.Checked)
						return i;
				}
				return -1;
			}

			public void Insert(int index, bool item)
			{
				throw new NotSupportedException("The list is fixed size");
			}

			public bool Remove(bool item)
			{
				throw new NotSupportedException("The list is fixed size");
			}

			public void RemoveAt(int index)
			{
				throw new NotSupportedException("The list is fixed size");
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}
		}
	}
}
