using System;
using System.Collections.Generic;

namespace Sandbox
{
	public abstract class UndoRemove
	{
		public abstract bool Delete();
		public abstract void Replace( object obj );
		public abstract bool ObjectEquals( object obj );
		public abstract bool IsValid();
	}
}
