using System;
using System.Runtime.Serialization;

namespace TagLib {
	/// <summary>
	///    This class extends <see cref="Exception" /> and is used to
	///    indicate that a file or tag is corrupt.
	/// </summary>
	/// <remarks>
	///    This exception will be thrown if invalid data interferes with the
	///    reading of the file or tag. One common example is in the (legal)
	///    downloading of media files with BitTorrent, in which case large
	///    portions of the file will contain zeroed bytes.
	/// </remarks>
	/// <example>
	///    <para>Catching an exception when creating a <see
	///    cref="File" />.</para>
	///    <code lang="C#">
	/// using System;
	/// using TagLib;
	///
	/// public class ExceptionTest
	/// {
	/// 	public static void Main ()
	/// 	{
	/// 		try {
	/// 			File file = File.Create ("partial.mp3"); // Partial download.
	/// 		} catch (CorruptFileException e) {
	/// 			Console.WriteLine ("That file is corrupt: {0}", e.ToString ());
	/// 		}
	///	}
	/// }
	///    </code>
	///    <code lang="C++">
	/// #using &lt;System.dll>
	/// #using &lt;taglib-sharp.dll>
	/// 
	/// using System;
	/// using TagLib;
	///
	/// void main ()
	/// {
	/// 	try {
	/// 		File file = File::Create ("partial.mp3"); // Partial download.
	/// 	} catch (CorruptFileException^ e) {
	/// 		Console::WriteLine ("That file is corrupt: {0}", e);
	/// 	}
	/// }
	///    </code>
	///    <code lang="VB">
	/// Imports System
	/// Imports TagLib
	///
	/// Public Class ExceptionTest
	/// 	Public Shared Sub Main ()
	/// 		Try
	/// 			file As File = File.Create ("partial.mp3") ' Partial download.
	/// 		Catch e As CorruptFileException
	/// 			Console.WriteLine ("That file is corrupt: {0}", e.ToString ());
	/// 		End Try
	///	End Sub
	/// End Class
	///    </code>
	///    <code lang="Boo">
	/// import System
	/// import TagLib
	///
	/// try:
	/// 	file As File = File.Create ("partial.mp3") # Partial download.
	/// catch e as CorruptFileException:
	/// 	Console.WriteLine ("That file is corrupt: {0}", e.ToString ());
	///    </code>
	/// </example>
	[Serializable]
	public class CorruptFileException : Exception
	{
		/// <summary>
		///    Constructs and initializes a new instance of <see
		///    cref="CorruptFileException" /> with a specified
		///    message.
		/// </summary>
		/// <param name="message">
		///    A <see cref="string" /> containing a message explaining
		///    the reason for the exception.
		/// </param>
		public CorruptFileException (string message) : base(message)
		{
		}
		
		/// <summary>
		///    Constructs and initializes a new instance of <see
		///    cref="CorruptFileException" /> with the default
		///    values.
		/// </summary>
		public CorruptFileException () : base()
		{
		}
		
		/// <summary>
		///    Constructs and initializes a new instance of <see
		///    cref="CorruptFileException" /> with a specified
		///    message containing a specified exception.
		/// </summary>
		/// <param name="message">
		///    A <see cref="string" /> containing a message explaining
		///    the reason for the exception.
		/// </param>
		/// <param name="innerException">
		///    A <see cref="Exception" /> object to be contained in the
		///    new exception. For example, previously caught exception.
		/// </param>
		public CorruptFileException (string message,
		                             Exception innerException)
			: base (message, innerException)
		{
		}
		
		/// <summary>
		///    Constructs and initializes a new instance of <see
		///    cref="CorruptFileException" /> from a specified
		///    serialization info and streaming context.
		/// </summary>
		/// <param name="info">
		///    A <see cref="SerializationInfo" /> object containing the
		///    serialized data to be used for the new instance.
		/// </param>
		/// <param name="context">
		///    A <see cref="StreamingContext" /> object containing the
		///    streaming context information for the new instance.
		/// </param>
		/// <remarks>
		///    This constructor is implemented because <see
		///    cref="CorruptFileException" /> implements the <see
		///    cref="ISerializable" /> interface.
		/// </remarks>
		protected CorruptFileException (SerializationInfo info,
		                                StreamingContext context)
			: base(info, context)
		{
		}
	}
}