using System.CodeDom.Compiler;

namespace System.Text.RegularExpressions.Generated;

[GeneratedCode("System.Text.RegularExpressions.Generator", "8.0.13.2707")]
internal sealed class _003CRegexGenerator_g_003EF372293D74F2F37DF39F66BF77797D52DA230767FFF375F6C4111DF72DE8B2285__StatusCodeRegex_0 : Regex
{
	private sealed class RunnerFactory : RegexRunnerFactory
	{
		private sealed class Runner : RegexRunner
		{
			protected override void Scan(ReadOnlySpan<char> inputSpan)
			{
				if (TryFindNextPossibleStartingPosition(inputSpan) && !TryMatchAtCurrentPosition(inputSpan))
				{
					runtextpos = inputSpan.Length;
				}
			}

			private bool TryFindNextPossibleStartingPosition(ReadOnlySpan<char> inputSpan)
			{
				int num = runtextpos;
				if (num <= inputSpan.Length - 3 && num == 0)
				{
					return true;
				}
				runtextpos = inputSpan.Length;
				return false;
			}

			private bool TryMatchAtCurrentPosition(ReadOnlySpan<char> inputSpan)
			{
				int num = runtextpos;
				int start = num;
				ReadOnlySpan<char> readOnlySpan = inputSpan.Slice(num);
				if (num != 0)
				{
					return false;
				}
				if (readOnlySpan.IsEmpty || !char.IsBetween(readOnlySpan[0], '1', '5'))
				{
					return false;
				}
				int num2 = num;
				if ((uint)readOnlySpan.Length >= 3u && char.IsAsciiDigit(readOnlySpan[1]) && char.IsAsciiDigit(readOnlySpan[2]))
				{
					num += 3;
					readOnlySpan = inputSpan.Slice(num);
				}
				else
				{
					num = num2;
					readOnlySpan = inputSpan.Slice(num);
					if ((uint)readOnlySpan.Length < 3u || (readOnlySpan[1] | 0x20) != 120 || (readOnlySpan[2] | 0x20) != 120)
					{
						return false;
					}
					num += 3;
					readOnlySpan = inputSpan.Slice(num);
				}
				if (num < inputSpan.Length - 1 || ((uint)num < (uint)inputSpan.Length && inputSpan[num] != '\n'))
				{
					return false;
				}
				runtextpos = num;
				Capture(0, start, num);
				return true;
			}
		}

		protected override RegexRunner CreateInstance()
		{
			return new Runner();
		}
	}

	internal static readonly _003CRegexGenerator_g_003EF372293D74F2F37DF39F66BF77797D52DA230767FFF375F6C4111DF72DE8B2285__StatusCodeRegex_0 Instance = new _003CRegexGenerator_g_003EF372293D74F2F37DF39F66BF77797D52DA230767FFF375F6C4111DF72DE8B2285__StatusCodeRegex_0();

	private _003CRegexGenerator_g_003EF372293D74F2F37DF39F66BF77797D52DA230767FFF375F6C4111DF72DE8B2285__StatusCodeRegex_0()
	{
		pattern = "^[1-5](?>[0-9]{2}|[xX]{2})$";
		roptions = RegexOptions.None;
		internalMatchTimeout = TimeSpan.FromMilliseconds(100.0);
		factory = new RunnerFactory();
		capsize = 1;
	}
}
