using System;
using System.Collections;
using System.Collections.Generic;

namespace Transmitter.Serialize
{
	[Serializable]
	public abstract class Message {

		public abstract byte[] GetByteArray ();

		public abstract void Init (byte[] msg);

		protected bool hasInit;
	}
	
}