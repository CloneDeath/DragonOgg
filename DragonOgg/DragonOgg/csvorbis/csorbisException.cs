using System;

namespace csvorbis 
{
	public class csorbisException : Exception 
	{
		public csorbisException ()
			:base(){}
		public csorbisException (String s)
			:base("csorbis: "+s){}
	}
}
