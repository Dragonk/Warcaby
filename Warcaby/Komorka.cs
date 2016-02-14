using System;

namespace Warcaby
{
	public class Komorka
	{
		private double[] wejscia;
		private double[] wagi;

		public Komorka(ref double[] wejscia, ref double[] wagi)
		{
			this.wejscia = wejscia;
			this.wagi = wagi;
		}

		public double funkcjaAktywacji()
		{
			double wynik = 0;
			for(int i = 0; i < wejscia.Length; i++)
			{
				wynik += wejscia[i]*wagi[i];
			}
			return 1/(1+Math.Exp(-1*wynik));
		}
	}
}

