using System;
using csogg;

namespace csvorbis 
{
	class Residue2 : Residue0
	{
		override public int forward(Block vb,Object vl, float[][] fin, int ch)
		{
			return 0;
		}

		override public int inverse(Block vb, Object vl, float[][] fin, int[] nonzero, int ch)
		{
			//System.err.println("Residue0.inverse");
			int i=0;
			for(i=0;i<ch;i++)if(nonzero[i]!=0)break;
			if(i==ch)return(0); /* no nonzero vectors */

			return(_2inverse(vb, vl, fin, ch));
		}
	}
}