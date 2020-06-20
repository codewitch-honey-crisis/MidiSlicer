using System;
using System.Collections.Generic;

namespace M
{
	/// <summary>
	/// Represents a base class for MIDI input and output devices
	/// </summary>
#if MIDILIB
	public
#endif
	abstract class MidiDevice : IDisposable
	{
		/// <summary>
		/// Indicates the available MIDI output devices
		/// </summary>
		public static IList<MidiOutputDevice> Outputs {
			get {
				var count = MidiUtility.GetMidiOutputDeviceCount();
				var result = new List<MidiOutputDevice>(count);
				for (var i = 0;i<count;++i)
					result.Add(new MidiOutputDevice(i));
				return result;
			}
		}
		/// <summary>
		/// Indicates the available MIDI input devices
		/// </summary>
		public static IList<MidiInputDevice> Inputs {
			get {
				var count = MidiUtility.GetMidiInputDeviceCount();
				var result = new List<MidiInputDevice>(count);
				for (var i = 0; i < count; ++i)
					result.Add(new MidiInputDevice(i));
				return result;
			}
		}
		/// <summary>
		/// Indicates the name of the MIDI device
		/// </summary>
		public abstract string Name { get; }
		/// <summary>
		/// Indicates the index of the MIDI device
		/// </summary>
		public abstract int Index { get; }
		/// <summary>
		/// Indicates whether or not this device is open
		/// </summary>
		public abstract bool IsOpen { get; }
		/// <summary>
		/// Opens the MIDI device
		/// </summary>
		public abstract void Open();
		/// <summary>
		/// Closes the MIDI device
		/// </summary>
		public abstract void Close();

		void IDisposable.Dispose()
		{
			Close();
			GC.SuppressFinalize(this);
		}
		/// <summary>
		/// Closes the device if it's open
		/// </summary>
		~MidiDevice()
		{
			Close();
		}
	}
	/// <summary>
	/// Represents a MIDI output device
	/// </summary>
#if MIDILIB
	public
#endif
	sealed class MidiOutputDevice : MidiDevice
	{
		readonly string _name;
		readonly int _index;
		IntPtr _handle;
		internal MidiOutputDevice(int index)
		{
			_index = index;
			_name = MidiUtility.GetOutputDeviceName(index);
			_handle = IntPtr.Zero;
		}
		/// <summary>
		/// Indicates the name of the MIDI output device
		/// </summary>
		public override string Name => _name;
		/// <summary>
		/// Indicates the device index of the MIDI output device
		/// </summary>
		public override int Index => _index;
		/// <summary>
		/// Indicates whether or not this device is open
		/// </summary>
		public override bool IsOpen => IntPtr.Zero != _handle;
		/// <summary>
		/// Opens the MIDI output device
		/// </summary>
		public override void Open()
		{
			Close();
			_handle = MidiUtility.OpenOutputDevice(Index);
			if (IntPtr.Zero == _handle)
				throw new InvalidOperationException("Unable to open MIDI output device");
		}
		/// <summary>
		/// Closes the MIDI output device
		/// </summary>
		public override void Close()
		{
			if(IntPtr.Zero !=_handle)
			{
				MidiUtility.CloseOutputDevice(_handle);
				_handle = IntPtr.Zero;
			}
		}
		/// <summary>
		/// Sends a MIDI message to the device
		/// </summary>
		/// <param name="message"></param>
		public void Send(MidiMessage message)
		{
			if (IntPtr.Zero == _handle)
				throw new InvalidOperationException("The MIDI output device is not open.");
			MidiUtility.Send(_handle, message);
		}
	}
	/// <summary>
	/// Represents the arguments for an incoming MIDI event
	/// </summary>
#if MIDILIB
	public
#endif
	sealed class MidiEventArgs : EventArgs
	{
		/// <summary>
		/// Constructs a new MIDI event arguments instance
		/// </summary>
		/// <param name="message">The associated MIDI message</param>
		public MidiEventArgs(MidiMessage message)
		{
			Message = message;
		}
		/// <summary>
		/// Indicates the MIDI message associated with the event
		/// </summary>
		public MidiMessage Message { get;  }
	}
	/// <summary>
	/// Represents the event handler delegate for incoming MIDI messages
	/// </summary>
	/// <param name="sender">The sending object</param>
	/// <param name="args">A <see cref="MidiEventArgs"/> instance</param>
#if MIDILIB
	public
#endif
	delegate void MidiEventHandler(object sender, MidiEventArgs args);
	/// <summary>
	/// Represents a MIDI input device
	/// </summary>
#if MIDILIB
	public
#endif
	sealed class MidiInputDevice : MidiDevice
	{
		readonly string _name;
		readonly int _index;
		IntPtr _handle;
		/// <summary>
		/// Indicates the event that handles incoming MIDI messages
		/// </summary>
		public event MidiEventHandler Input;
		internal MidiInputDevice(int index)
		{
			_index = index;
			_name = MidiUtility.GetInputDeviceName(index);
			_handle = IntPtr.Zero;
		}
		/// <summary>
		/// Indicates the name of the MIDI output device
		/// </summary>
		public override string Name => _name;
		/// <summary>
		/// Indicates the device index of the MIDI output device
		/// </summary>
		public override int Index => _index;
		/// <summary>
		/// Indicates whether or not this device is open
		/// </summary>
		public override bool IsOpen => IntPtr.Zero != _handle;
		/// <summary>
		/// Opens the MIDI input device
		/// </summary>
		public override void Open()
		{
			Close();
			_handle = MidiUtility.OpenInputDevice(Index, _MidiInProc);
			if (IntPtr.Zero == _handle)
				throw new InvalidOperationException("Unable to open MIDI input device");
		}
		/// <summary>
		/// Closes the MIDI input device
		/// </summary>
		public override void Close()
		{
			if (IntPtr.Zero != _handle)
			{
				MidiUtility.CloseInputDevice(_handle);
				_handle = IntPtr.Zero;
			}
		}
		
		void _MidiInProc(IntPtr handle, int msg, int instance, int param1, int param2)
		{
			var m = MidiUtility.UnpackMessage(msg);
			Input?.Invoke(this, new MidiEventArgs(m));
		}
	}
}
