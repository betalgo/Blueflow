using System;
using System.IO;

namespace Microsoft.OpenApi;

/// <summary>
/// A custom <see cref="T:System.IO.StreamWriter" /> which supports setting a <see cref="T:System.IFormatProvider" />.
/// </summary>
public class FormattingStreamWriter : StreamWriter
{
	private readonly IFormatProvider _formatProvider;

	/// <summary>
	/// The <see cref="T:System.IFormatProvider" /> associated with this <see cref="T:Microsoft.OpenApi.FormattingStreamWriter" />.
	/// </summary>
	public override IFormatProvider FormatProvider => _formatProvider;

	/// <summary>
	/// Initializes a new instance of the <see cref="T:Microsoft.OpenApi.FormattingStreamWriter" /> class.
	/// </summary>
	/// <param name="stream"></param>
	/// <param name="formatProvider"></param>
	public FormattingStreamWriter(Stream stream, IFormatProvider formatProvider)
		: base(stream)
	{
		_formatProvider = formatProvider;
	}
}
