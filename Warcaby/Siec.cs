using System;

namespace Warcaby
{
	public class Siec
	{	
		private double[][] w = new double[8][]; // wagi idące z wejścia do warstwy pośredniej
		private Komorka[] ukryte = new Komorka[8]; // komórki z warstwy ukrytej - 8
		private double [] u = new double[18]; // wejścia do sieci

		private double[][] v = new double[10][]; // wagi idące z warstwy pośredniej do warstwy wyjściowej
		private Komorka[] wyjściowe = new Komorka[10]; // komórki z warstwy wyjściowej - 7
		private double[] t = new double[8]; // obliczone funkcje aktywacji dla komórek warstwy pośredniej
		private double[] x = new double[10]; // obliczone funkcje aktywacji dla komórek warstwy wyjściowej
		private double[] deltaWyjściowa = new double[10]; // będzie 7 delt wyjściowych
		private double[] deltaPośrednia = new double[8]; // 8 delt pośrednich
		private double[] pochodnaWyjściowa = new double[10]; // 7 pochodnych wyjściowych
		private double[] pochodnaPośrednia = new double[8]; // 8 pochodnych pośrednich

		private int _nowePole;
		private int _starePole;

		public int nowePole
		{
			get { return _nowePole; }
			set { _nowePole = value; }
		}

		public int starePole
		{
			get { return _starePole; }
			set { _starePole = value; }
		}

		public void generowanieRuchu(int[] tablica)
		{
			double[] tab = new double[18];
			int[] poleStart = new int[5];
			int[] poleKoniec = new int[5];
			int a;
			int j = 4;
			for(int i=0; i<18; i++)
			{
				tab[i] = tablica[i];
			}
			this.u = tab;
			this.propagacjaWPrzód();
			this.starePole = 0;
			this.nowePole = 0;
			for (int i = 0; i < 5; i++) {
				poleStart[i] = System.Convert.ToInt16(Math.Round(this.x[i],0));
				poleKoniec[i] = System.Convert.ToInt16(Math.Round(this.x[i+5],0));
				a = System.Convert.ToInt16(Math.Round(Math.Pow ((double) 2, (double) j)));
				this.starePole += poleStart[i]*a;
				this.nowePole += poleKoniec[i]*a;
				j=j-1;
			}
		}

		private void inicjalizacjaUkrytych() // metoda tworzy warstwę ukrytą
		{
			// wszystkie wejścia wchodzą plus 18 wag
			for (int i = 0; i < 8; i++) 
			{
				this.ukryte [i] = new Komorka (ref this.u, ref this.w [i]);
			}
		}

		private void inicjalizacjaWyjściowych() // metoda tworzy warstwę wyjściową
		{
			for (int i = 0; i < 10; i++) {
				this.wyjściowe [i] = new Komorka (ref this.t, ref this.v [i]);
			}
		}

		private void propagacjaWPrzód()
		{
			for(int i = 0; i<8; i++)
			{
				this.t[i] = this.ukryte[i].funkcjaAktywacji(); // obliczamy najpierw warstwę ukrytą
			}
			for(int i = 0; i<10; i++)
			{
				this.x[i] = this.wyjściowe[i].funkcjaAktywacji(); // a potem warstwę wyjściową
			}
		}

		private void propagacjaWstecz(double[] C) // C to pożądana odpowiedź neuronu Ci[0] to odpowiedź wyjściowego neuronu 0 itd.
		{
			for(int i = 0; i < 10; i++) // obliczanie delty wyjściowej
			{
				this.pochodnaWyjściowa[i] = this.x[i] * (1-this.x[i]);
				this.deltaWyjściowa[i] = (C[i]-this.x[i])*this.pochodnaWyjściowa[i];
			} // obliczone wszystkie delty wyjściowe
			/*
		 * wszystkie delty wyjściowe * wagi wychodząca z komórki
		 * */
			for(int i = 0; i < 8; i++)
			{
				this.pochodnaPośrednia[i] = this.t[i] * (1-this.t[i]);
				double suma = 0; // ze wzoru
				for(int j = 0; j < 10; j++)
					suma += this.deltaWyjściowa[j]+this.v[j][i];
				this.deltaPośrednia[i] = suma * this.pochodnaPośrednia[i];
			} // obliczone wszystkie delty pośrednie
		}

		private void aktualizacjaWag()
		{
			for(int i = 0; i < 8; i++)
			{
				for(int j=0; j < 18; j++)
					this.w[i][j] = this.w[i][j] + this.deltaPośrednia[i] * this.u[j] * 0.2; // aktualizacja wag na pierwszym poziomie
			}

			for(int i = 0; i < 10; i++)
			{
				for(int j=0; j < 8; j++)
					this.v[i][j] = this.v[i][j] + this.deltaWyjściowa[i]* this.t[j] * 0.2; // aktualizacja wag na pierwszym poziomie
			}
		}

		public void trenuj() 
		{
			double[,] trainingSet = new double[12,28] {
				{-1,-1,-1,-1,-1,-1,0,0,0,0,0,0,1,1,1,1,1,1,0,1,1,0,1,0,1,0,1,0},
				{-1,-1,-1,0,-1,-1,-1,0,0,1,0,0,0,1,1,1,1,1,1,0,0,0,1,0,1,1,0,1},
				{-1,-1,-1,0,-1,0,-1,-1,0,1,0,0,1,1,1,1,0,1,0,1,1,1,0,0,1,0,1,1},
				{-1,-1,-1,0,-1,0,0,-1,0,1,0,0,1,-1,1,1,0,1,1,0,0,1,0,0,0,1,1,0},
				{-1,-1,0,0,-1,0,0,-1,0,1,0,0,1,0,1,1,0,0,0,1,1,0,1,0,1,0,1,1},
				{-1,-1,0,0,-1,0,0,0,0,1,0,0,-1,0,1,1,0,0,0,1,1,1,1,0,1,1,0,0},
				{-1,-1,0,0,0,0,0,-1,0,1,0,1,-1,0,0,1,0,0,0,1,1,0,0,0,0,1,0,1},
				{0,-1,0,0,0,0,0,-1,0,1,0,0,-1,0,0,1,0,0,1,0,0,0,0,0,0,1,1,0},
				{0,0,0,0,0,0,0,0,-1,1,0,0,0,0,0,0,0,0,0,1,0,1,0,0,0,1,1,1},
				{0,0,0,0,0,0,1,0,0,0,0,-1,0,0,0,0,0,0,0,0,1,1,1,0,0,1,0,1},
				{0,0,0,0,1,0,0,0,0,0,0,0,0,-1,0,0,0,0,0,0,1,0,1,0,0,0,0,1},
				{2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,-2,0,0,0,0,0,1,0,0,1,0,0}
			};
			Random generator = new Random();
			double[] temp = new double[18];
			double[] tempp = new double[10];
			for(int i=0; i<20000; i++)
			{
				int ktory = generator.Next(0,11);
				for (int j = 0; j < 18; j++) {
					temp[j] = trainingSet[ktory, j];
				}
				for (int j = 0; j < 10; j++) {
					tempp[j] = trainingSet[ktory, j+18];
				}
				this.u = temp;
				this.propagacjaWPrzód();
				this.propagacjaWstecz(tempp);
				this.aktualizacjaWag();
			}	
		}

		public Siec()
		{
			for (int i = 0; i < 8; i++)
				this.w [i] = new double[18];
			for (int i = 0; i < 10; i++)
				this.v[i] = new double[8];
			this.inicjalizacjaUkrytych();
			this.inicjalizacjaWyjściowych();
			this.trenuj ();
		}
	};
}
