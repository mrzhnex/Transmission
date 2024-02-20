using System;
using System.Diagnostics;

namespace AudioVisualizationLibrary.Classes
{
	public static partial class MathF
	{
		/// <summary>
		/// https://www.codeproject.com/Articles/30180/Simple-Signal-Generator#_articleTop
		/// </summary>
		internal class SignalGenerator
		{
			#region [ Properties ... ]

			/// <summary>
			/// Signal Type.
			/// </summary>
			public SignalType SignalType = SignalType.Sine;

			/// <summary>
			/// Signal Frequency.
			/// </summary>
			public double Frequency = 1.0;

			/// <summary>
			/// Signal Phase.
			/// </summary>
			public double Phase = 0.0;

			/// <summary>
			/// Signal Amplitude.
			/// </summary>
			public double Amplitude = 1.0;

			/// <summary>
			/// Signal Offset.
			/// </summary>
			public double Offset = 0.0;

			private double invert = 1; // Yes=-1, No=1
			/// <summary>
			/// Signal Inverted?
			/// </summary>
			public bool Invert
			{
				get { return invert == -1; }
				set { invert = value ? -1 : 1; }
			}

			/// <summary>
			/// GetValue Callback?
			/// </summary>
			public GetValueDelegate GetValueCallback = null;

			#endregion  [ Properties ]

			#region [ Private ... ]

			/// <summary>
			/// Random provider for noise generator
			/// </summary>
			private readonly Random random = new Random();

			/// <summary>
			/// Time the signal generator was started
			/// </summary>
			protected long startTime = Stopwatch.GetTimestamp();

			/// <summary>
			/// Ticks per second on this CPU
			/// </summary>
			protected long ticksPerSecond = Stopwatch.Frequency;

			#endregion  [ Private ]

			#region [ Public ... ]

			public delegate double GetValueDelegate(double time);

			public SignalGenerator(SignalType initialSignalType)
			{
				this.SignalType = initialSignalType;
			}

			public SignalGenerator() { }

#if DEBUG
			public double GetValue(double time)
#else
			private double GetValue(double time)
#endif
			{
				double value = 0.0;
				double t = this.Frequency * time + this.Phase;

				switch (this.SignalType)
				{ // http://en.wikipedia.org/wiki/Waveform
					case SignalType.Sine: // sin( 2 * pi * t )
					case SignalType.MultiSine:
						value = Math.Sin(2.0 * Math.PI * t);
						break;
					case SignalType.Square: // sign( sin( 2 * pi * t ) )
						value = Math.Sign(Math.Sin(2.0 * Math.PI * t));
						break;
					case SignalType.Triangle: // 2 * abs( t - 2 * floor( t / 2 ) - 1 ) - 1
					case SignalType.MultiTriangle:
						value = 1.0 - 4.0 * Math.Abs(Math.Round(t - 0.25) - (t - 0.25));
						break;
					case SignalType.Sawtooth: // 2 * ( t/a - floor( t/a + 1/2 ) )
						value = 2.0 * (t - Math.Floor(t + 0.5));
						break;
					case SignalType.Pulse: // http://en.wikipedia.org/wiki/Pulse_wave
						value = (Math.Abs(Math.Sin(2 * Math.PI * t)) < 1.0 - 10E-3) ? 0 : 1;
						break;
					case SignalType.WhiteNoise: // http://en.wikipedia.org/wiki/White_noise
						value = 2 * random.Next(int.MaxValue) / int.MaxValue - 1.0;
						break;
					case SignalType.GaussNoise: // http://en.wikipedia.org/wiki/Gaussian_noise
						value = StatisticFunction.NORMINV(random.Next(int.MaxValue) / int.MaxValue, 0.0, 0.4);
						break;
					case SignalType.DigitalNoise: //Binary Bit Generators
						value = random.Next(2);
						break;

					case SignalType.UserDefined:
						value = (this.GetValueCallback == null) ? (0.0) : this.GetValueCallback(t);
						break;
				}

				return (invert * this.Amplitude * value + this.Offset);
			}

			public double GetValue()
			{
				double time = (Stopwatch.GetTimestamp() - startTime) / ticksPerSecond;
				return GetValue(time);
			}

			public void Reset()
			{
				startTime = Stopwatch.GetTimestamp();
			}

			public void Synchronize(SignalGenerator instance)
			{
				startTime = instance.startTime;
				ticksPerSecond = instance.ticksPerSecond;
			}

			#endregion [ Public ]
		}

		#region [ Enums ... ]

		public enum SignalType
		{
			Normal,
			Sine,
			MultiSine,
			MultiTriangle,
			Square,
			Triangle,
			Sawtooth,

			Pulse,
			WhiteNoise,    // random between -1 and 1
			GaussNoise,    // random between -1 and 1 with normal distribution
			DigitalNoise,

			UserDefined    // user defined between -1 and 1	}
		}

		#endregion [ Enums ]

		#region [ Statistic ... ]
		internal class StatisticFunction
		{
			// http://geeks.netindonesia.net/blogs/anwarminarso/archive/2008/01/13/normsinv-function-in-c-inverse-cumulative-standard-normal-distribution-function.aspx
			// http://home.online.no/~pjacklam/notes/invnorm/impl/misra/normsinv.html

			public static double Mean(double[] values)
			{
				double tot = 0;
				foreach (double val in values)
					tot += val;

				return (tot / values.Length);
			}

			public static double StandardDeviation(double[] values)
			{
				return Math.Sqrt(Variance(values));
			}

			public static double Variance(double[] values)
			{
				double m = Mean(values);
				double result = 0;
				foreach (double d in values)
					result += Math.Pow((d - m), 2);

				return (result / values.Length);
			}

			//
			// Lower tail quantile for standard normal distribution function.
			//
			// This function returns an approximation of the inverse cumulative
			// standard normal distribution function.  I.e., given P, it returns
			// an approximation to the X satisfying P = Pr{Z <= X} where Z is a
			// random variable from the standard normal distribution.
			//
			// The algorithm uses a minimax approximation by rational functions
			// and the result has a relative error whose absolute value is less
			// than 1.15e-9.
			//
			// Author:      Peter J. Acklam
			// (Javascript version by Alankar Misra @ Digital Sutras (alankar@digitalsutras.com))
			// Time-stamp:  2003-05-05 05:15:14
			// E-mail:      pjacklam@online.no
			// WWW URL:     http://home.online.no/~pjacklam

			// An algorithm with a relative error less than 1.15*10-9 in the entire region.

			public static double NORMSINV(double p)
			{
				// Coefficients in rational approximations
				double[] a = {-3.969683028665376e+01,  2.209460984245205e+02,
				-2.759285104469687e+02,  1.383577518672690e+02,
				-3.066479806614716e+01,  2.506628277459239e+00};

				double[] b = {-5.447609879822406e+01,  1.615858368580409e+02,
				-1.556989798598866e+02,  6.680131188771972e+01,
				-1.328068155288572e+01 };

				double[] c = {-7.784894002430293e-03, -3.223964580411365e-01,
				-2.400758277161838e+00, -2.549732539343734e+00,
				4.374664141464968e+00,  2.938163982698783e+00};

				double[] d = { 7.784695709041462e-03,  3.224671290700398e-01,
				2.445134137142996e+00,  3.754408661907416e+00};

				// Define break-points.
				double plow = 0.02425;
				double phigh = 1 - plow;

				// Rational approximation for lower region:
				if (p < plow)
				{
					double q = Math.Sqrt(-2 * Math.Log(p));
					return (((((c[0] * q + c[1]) * q + c[2]) * q + c[3]) * q + c[4]) * q + c[5]) /
						((((d[0] * q + d[1]) * q + d[2]) * q + d[3]) * q + 1);
				}

				// Rational approximation for upper region:
				if (phigh < p)
				{
					double q = Math.Sqrt(-2 * Math.Log(1 - p));
					return -(((((c[0] * q + c[1]) * q + c[2]) * q + c[3]) * q + c[4]) * q + c[5]) /
						((((d[0] * q + d[1]) * q + d[2]) * q + d[3]) * q + 1);
				}

				// Rational approximation for central region:
				{
					double q = p - 0.5;
					double r = q * q;
					return (((((a[0] * r + a[1]) * r + a[2]) * r + a[3]) * r + a[4]) * r + a[5]) * q /
						(((((b[0] * r + b[1]) * r + b[2]) * r + b[3]) * r + b[4]) * r + 1);
				}
			}


			public static double NORMINV(double probability, double mean, double standard_deviation)
			{
				return (NORMSINV(probability) * standard_deviation + mean);
			}

			public static double NORMINV(double probability, double[] values)
			{
				return NORMINV(probability, Mean(values), StandardDeviation(values));
			}

		}
		#endregion [ Statistic ]
	}

}
