using System;

namespace Warcaby
{
	public class Siec
	{	
		private double[][] w = new double[8][]; // wagi idące z wejścia do warstwy pośredniej
		private Komorka[] ukryte = new Komorka[8]; // komórki z warstwy ukrytej - 8
		private double [] u = new double[18]; // wejścia do sieci

		private double[][] v = new double[10][]; // wagi idące z warstwy pośredniej do warstwy wyjściowej
		private Komorka[] wyjściowe = new Komorka[10]; // komórki z warstwy wyjściowej - 10
		private double[] t = new double[8]; // obliczone funkcje aktywacji dla komórek warstwy pośredniej
		private double[] x = new double[10]; // obliczone funkcje aktywacji dla komórek warstwy wyjściowej
		private double[] deltaWyjściowa = new double[10]; // będzie 10 delt wyjściowych
		private double[] deltaPośrednia = new double[8]; // 8 delt pośrednich
		private double[] pochodnaWyjściowa = new double[10]; // 10 pochodnych wyjściowych
		private double[] pochodnaPośrednia = new double[8]; // 8 pochodnych pośrednich

		private double[][] trainingSet = new double[10000][]; //zestawy treningowe, aktualizują się podczas gry
		private int iloscZestawow; //aktualna ilosc zestawow treningowych;

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

		public void nowyZestaw(int[] tablica, int poczatkowe, int docelowe) {
			if (this.iloscZestawow < 10000) {
				this.iloscZestawow += 1;
				this.trainingSet [iloscZestawow] = new double[28];
				int[] poleStart = new int[5];
				int[] poleKoniec = new int[5];
				int j = 4;
				for (int i = 0; i < 5; i++) {
					poleStart [j] = poczatkowe % 2;
					poleKoniec [j] = docelowe % 2;
					poczatkowe = poczatkowe / 2;
					docelowe = docelowe / 2;
					j = j - 1;
				}
				for (int i = 0; i < 18; i++) {
					this.trainingSet [iloscZestawow] [i] = tablica [i];
				}
				for (int i = 0; i < 5; i++) {
					this.trainingSet [iloscZestawow] [i + 18] = poleStart [i];
					this.trainingSet [iloscZestawow] [i + 23] = poleKoniec [i];
				}
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
			Random generator = new Random();
			double[] temp = new double[18];
			double[] tempp = new double[10];
			for(int i=0; i<20000; i++)
			{
				int ktory = generator.Next(0,this.iloscZestawow);
				for (int j = 0; j < 18; j++) {
					temp[j] = trainingSet[ktory][j];
				}
				for (int j = 0; j < 10; j++) {
					tempp[j] = trainingSet[ktory][j+18];
				}
				this.u = temp;
				this.propagacjaWPrzód();
				this.propagacjaWstecz(tempp);
				this.aktualizacjaWag();
			}	
		}

		public Siec()
		{
			this.trainingSet [0] = new double[28]
			{-1, -1, -1, -1, -1, 0, 0, 0, -1, 0, 0, 0, 1, 1, 1, 1, 1, 1, 0, 1, 1, 0, 1, 0, 1, 0, 1, 1};
			this.trainingSet [1] = new double[28]
			{-1, -1, -1, -1, 0, -1, -1, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 0, 1, 1, 0, 0, 0, 1, 0, 0, 1};
			this.trainingSet [2] = new double[28]
			{0, -1, -1, -1, -1, -1, -1, 0, 0, 1, 0, 0, 0, 1, 1, 1, 1, 1, 0, 1, 1, 1, 0, 0, 1, 0, 1, 1};
			this.trainingSet [3] = new double[28]
			{0, -1, -1, -1, -1, 0, -1, 0, -1, 1, 0, 1, 0, 1, 0, 1, 1, 1, 0, 1, 1, 0, 1, 0, 1, 0, 1, 0};
			this.trainingSet [4] = new double[28]
			{0, -1, -1, -1, -1, 0, 0, 0, -1, 1, 0, 1, 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 1, 0, 1, 0, 1, 0};
			this.trainingSet [5] = new double[28]
			{0, -1, 0, -1, -1, -1, 0, 0, -1, 1, 1, 1, 0, 0, 0, 1, 1, 0, 1, 0, 0, 0, 0, 0, 1, 1, 0, 1};
			this.trainingSet [6] = new double[28]
			{0, -1, 0, -1, 0, -1, 0, 0, -1, 1, 1, 1, 0, 1, 0, 1, 0, 0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 0};
			this.trainingSet [7] = new double[28]
			{0, 0, 0, -1, 0, -1, -1, 0, -1, 0, 1, 1, 0, 1, 0, 1, 0, 0, 0, 1, 0, 1, 1, 0, 0, 1, 1, 1};
			this.trainingSet [8] = new double[28]
			{0, 0, 0, -1, 0, 0, 0, 1, -1, -1, 1, 0, 0, 1, 0, 1, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 1, 0};
			this.trainingSet [9] = new double[28]
			{0, 0, 2, -1, 0, 0, 0, 0, -1, 0, 1, 0, -1, 1, 0, 1, 0, 0, 0, 1, 0, 1, 0, 0, 0, 1, 1, 0};
			this.trainingSet [10] = new double[28]
			{0, 0, 2, 0, 0, 0, 0, 0, -1, 0, 0, 0, -1, 1, 0, 1, 0, 0, 0, 1, 1, 0, 1, 0, 0, 1, 1, 0};
			this.trainingSet [11] = new double[28]
			{0, 0, 2, 0, 0, 0, 1, 0, -1, 0, 0, 0, 0, 0, 0, 1, -2, 0, 0, 0, 1, 1, 0, 0, 0, 1, 0, 0};
			this.trainingSet [12] = new double[28]
			{0, 0, 2, 0, 1, 0, 0, 0, 0, 0, 0, -1, 0, 0, 0, 1, -2, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0};
			this.trainingSet [13] = new double[28]
			{2, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, -1, 1, -2, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0};
			this.trainingSet [14] = new double[28]
			{0, 0, 2, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, -2, -2, 0, 0, 0, 1, 0, 0, 0, 1, 0, 1};
			this.trainingSet [15] = new double[28]
			{0, 0, 0, 0, 2, 2, 0, 0, 0, 0, 0, 0, 0, -2, 0, 1, -2, 0, 0, 0, 1, 0, 0, 0, 0, 1, 1, 0};
			this.trainingSet [16] = new double[28]
			{0, 0, 0, 0, 0, 2, 2, 0, 0, 0, 0, -2, 0, 0, 0, 1, -2, 0, 0, 0, 1, 0, 1, 0, 0, 1, 1, 1};
			this.trainingSet [17] = new double[28]
			{0, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 1, -2, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 1};
			this.trainingSet [18] = new double[28]
			{0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, -2, 0, 1, 0, 0, 0, 1, 1, 1, 1, 0, 1, 1, 0, 0};
			this.trainingSet [19] = new double[28]
			{0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 1, 1, 1};
			this.trainingSet [20] = new double[28]
			{-1, -1, -1, -1, -1, 0, 0, -1, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 0, 1, 1, 0, 1, 0, 1, 0, 1, 0};
			this.trainingSet [21] = new double[28]
			{-1, -1, 0, -1, -1, -1, 0, -1, 0, 0, 1, 0, 1, 0, 1, 1, 1, 1, 1, 0, 0, 0, 1, 0, 1, 1, 0, 1};
			this.trainingSet [22] = new double[28]
			{-1, -1, 0, -1, 0, -1, -1, -1, 0, 0, 1, 0, 1, 1, 1, 1, 1, 0, 0, 1, 1, 0, 1, 0, 1, 0, 1, 1};
			this.trainingSet [23] = new double[28]
			{-1, -1, 0, -1, 0, -1, 0, 0, 0, 0, 0, 1, 1, -1, 1, 1, 1, 0, 0, 1, 0, 1, 1, 0, 0, 1, 0, 0};
			this.trainingSet [24] = new double[28]
			{-1, -1, 0, -1, 1, -1, 0, 0, 0, 0, 0, 0, 1, 0, 1, 1, 1, -2, 0, 1, 1, 0, 0, 0, 1, 0, 0, 1};
			this.trainingSet [25] = new double[28]
			{0, -1, 0, -1, 0, -1, 0, -1, 0, 1, 0, 0, 0, 0, 1, 1, 1, -2, 0, 1, 1, 1, 1, 0, 1, 1, 0, 0};
			this.trainingSet [26] = new double[28]
			{0, 0, 0, -1, -1, -1, 0, -1, 0, 1, 0, 0, 1, 0, 1, 0, 1, -2, 0, 1, 1, 0, 0, 0, 1, 0, 1, 0};
			this.trainingSet [27] = new double[28]
			{0, 0, 0, -1, -1, -1, 0, 0, 0, 1, 0, 0, -1, 0, 1, 0, 1, -2, 0, 1, 0, 0, 1, 0, 0, 1, 1, 0};
			this.trainingSet [28] = new double[28]
			{0, 0, 0, -1, 0, -1, 0, 0, 0, -1, 0, 0, -1, 0, 1, 0, 1, -2, 0, 1, 1, 1, 0, 0, 1, 0, 1, 1};
			this.trainingSet [29] = new double[28]
			{0, 0, 0, 0, 0, -1, -1, 0, 0, -1, 0, 1, -1, 0, 0, 0, 1, -2, 0, 1, 0, 1, 1, 0, 1, 0, 0, 0};
			this.trainingSet [30] = new double[28]
			{0, 0, 0, 0, 0, 0, -1, -1, 1, -1, 0, 0, -1, 0, 0, 0, 1, -2, 1, 0, 0, 0, 0, 0, 1, 1, 0, 1};
			this.trainingSet [31] = new double[28]
			{0, 0, 0, 0, 0, 0, -1, -1, 1, -1, -2, 0, -1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 1};
			this.trainingSet [32] = new double[28]
			{0, 0, 0, 0, 0, 1, -1, 0, 0, -1, -2, -1, -1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 1, 0, 0, 0, 0, 1};
			this.trainingSet [33] = new double[28]
			{0, 2, 0, 0, 0, 0, -1, -2, 0, -1, 0, -1, -1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 1, 0, 0};
			this.trainingSet [34] = new double[28]
			{-1, -1, -1, -1, -1, 0, 0, 0, -1, 0, 0, 0, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 0, 0, 1, 0, 1, 1};
			this.trainingSet [35] = new double[28]
			{-1, 0, -1, -1, -1, -1, 0, 0, -1, 0, 0, 1, 1, 1, 0, 1, 1, 1, 0, 1, 1, 0, 0, 0, 1, 0, 1, 0};
			this.trainingSet [36] = new double[28]
			{-1, 0, -1, -1, 0, -1, -1, 0, -1, 0, 1, 1, 0, 1, 0, 1, 1, 1, 0, 1, 1, 1, 1, 0, 1, 1, 0, 0};
			this.trainingSet [37] = new double[28]
			{-1, 0, -1, -1, 0, -1, 0, 0, -1, -1, 1, 1, 1, 1, 0, 0, 1, 1, 1, 0, 0, 0, 1, 0, 1, 1, 1, 0};
			this.trainingSet [38] = new double[28]
			{0, 0, -1, -1, -1, -1, 0, 0, -1, -1, 1, 1, 1, 1, 1, 0, 1, 0, 0, 1, 0, 1, 1, 0, 0, 1, 1, 1};
			this.trainingSet [39] = new double[28]
			{0, 0, -1, -1, 0, -1, 0, 0, -1, -1, 1, 0, 1, 1, 1, 0, 1, 0, 0, 1, 1, 1, 0, 0, 0, 1, 1, 1};
			this.trainingSet [40] = new double[28]
			{0, 0, -1, -1, 0, -1, 0, 1, 0, -1, 1, 0, 1, 1, 0, 0, 1, 0, 0, 1, 1, 0, 1, 0, 1, 0, 0, 0};
			this.trainingSet [41] = new double[28]
			{0, 0, -1, 0, 0, -1, 0, 1, 1, -1, 1, 0, 1, 0, 0, 0, 1, 0, 0, 1, 0, 1, 0, 0, 0, 0, 1, 1};
			this.trainingSet [42] = new double[28]
			{0, 0, -1, 1, 0, 0, 0, 0, 1, -1, 0, 0, 1, 0, 0, 0, 1, 0, 0, 1, 1, 0, 0, 0, 0, 1, 1, 1};
			this.trainingSet [43] = new double[28]
			{0, 0, -1, 1, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0, 1};
			this.trainingSet [44] = new double[28]
			{0, 0, 0, 1, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1};
			this.trainingSet [45] = new double[28]
			{-1, -1, -1, -1, 0, -1, 0, -1, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 0, 1, 1, 0, 1, 0, 1, 0, 1, 1};
			this.trainingSet [46] = new double[28]
			{-1, -1, -1, -1, 0, 0, 0, -1, -1, 0, 0, 1, 1, 0, 1, 1, 1, 1, 1, 0, 0, 0, 1, 0, 1, 1, 0, 1};
			this.trainingSet [47] = new double[28]
			{-1, 0, -1, -1, -1, 0, 0, -1, -1, 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 1, 1, 0, 0, 0, 1, 0, 0, 1};
			this.trainingSet [48] = new double[28]
			{-1, 0, -1, 0, -1, 0, -1, -1, -1, 1, 0, 1, 0, 1, 1, 1, 1, 0, 0, 1, 1, 1, 1, 0, 1, 1, 0, 0};
			this.trainingSet [49] = new double[28]
			{-1, 0, 0, 0, -1, -1, -1, -1, -1, 1, 0, 1, 1, 1, 1, 0, 1, 0, 0, 1, 1, 0, 0, 0, 1, 0, 1, 0};
			this.trainingSet [50] = new double[28]
			{-1, 0, 0, 0, -1, -1, -1, 0, -1, 1, 0, 1, -1, 1, 1, 0, 1, 0, 0, 1, 1, 0, 1, 0, 1, 0, 1, 0};
			this.trainingSet [51] = new double[28]
			{-1, 0, 0, 0, -1, -1, 0, 0, -1, 1, 0, 1, -1, -1, 1, 0, 1, 0, 0, 1, 0, 0, 1, 0, 0, 1, 1, 0};
			this.trainingSet [52] = new double[28]
			{-1, 0, 0, 0, 0, -1, 0, 0, -1, -1, 0, 1, -1, -1, 1, 0, 1, 0, 0, 1, 0, 1, 1, 0, 0, 1, 1, 1};
			this.trainingSet [53] = new double[28]
			{-1, 0, 0, 0, 0, 0, 0, 0, -1, -1, -1, 0, -1, 0, 1, 0, 1, 0, 1, 0, 0, 0, 0, 0, 1, 0, 1, 1};
			this.trainingSet [54] = new double[28]
			{-1, 0, 0, 0, 0, 0, 0, 0, 0, -1, -1, 0, -1, -1, 1, 0, 0, 0, 0, 1, 1, 1, 0, 0, 1, 0, 1, 1};
			this.trainingSet [55] = new double[28]
			{-1, 0, 0, 0, 0, 0, 0, 0, 0, -1, -1, 1, -1, 0, 0, 0, 0, -2, 0, 1, 0, 1, 1, 0, 0, 1, 1, 1};
			this.trainingSet [56] = new double[28]
			{-1, 0, 0, 0, 0, 0, 0, 1, 0, -1, -1, 0, -1, 0, -2, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 1, 0, 1};
			this.trainingSet [57] = new double[28]
			{-1, 0, 0, 0, 0, 1, 0, 0, 0, -1, -1, -2, -1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 1, 0, 0, 0, 0, 1};
			this.trainingSet [58] = new double[28]
			{-1, 2, 0, 0, 0, 0, 0, -2, 0, -1, -1, 0, -1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 1, 0, 0};
			this.trainingSet [59] = new double[28]
			{-1, 0, 0, 0, 2, -2, 0, 0, 0, -1, -1, 0, -1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 1, 0};
			this.trainingSet [60] = new double[28]
			{-1, 0, 0, 0, 0, -2, 2, 0, 0, -1, 0, 0, -1, -1, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 1, 0, 1, 0};
			this.trainingSet [61] = new double[28]
			{-1, 0, 0, 0, 0, -2, 0, 0, 0, -1, 2, 0, 0, 0, 0, -2, 0, 0, 0, 1, 0, 1, 0, 1, 0, 0, 0, 1};
			this.trainingSet [62] = new double[28]
			{-1, 0, 0, 0, 0, -2, 0, 0, 0, -1, 0, 0, -2, 0, 0, 0, 0, 2, 1, 0, 0, 0, 1, 0, 1, 1, 0, 1};
			this.trainingSet [63] = new double[28]
			{-1, 0, 0, 0, 0, 0, 0, 0, -2, -1, 0, 0, -2, 2, 0, 0, 0, 0, 0, 1, 1, 0, 1, 1, 0, 0, 0, 1};
			this.trainingSet [64] = new double[28]
			{-1, 0, 0, 0, 0, 0, 0, 0, 0, -1, 0, -2, -2, 0, 0, 0, 0, 2, 1, 0, 0, 0, 1, 0, 1, 1, 0, 1};
			this.trainingSet [65] = new double[28]
			{-1, -1, -1, -1, -1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 1, 0, 1, 1};
			this.trainingSet [66] = new double[28]
			{-1, -1, 0, -1, -1, -1, 0, 0, 0, 0, 0, 1, 1, 0, 1, 1, 0, 1, 0, 1, 0, 1, 1, 0, 1, 0, 0, 0};
			this.trainingSet [67] = new double[28]
			{-1, -1, 0, -1, 0, -1, 0, -1, 1, 0, 0, 0, 1, 0, 1, 1, 0, 1, 0, 1, 1, 0, 0, 0, 1, 0, 0, 1};
			this.trainingSet [68] = new double[28]
			{0, -1, 0, -1, -1, -1, 0, -1, 1, 1, 0, 0, 0, 0, 1, 1, 0, 1, 0, 1, 1, 1, 1, 0, 1, 1, 0, 0};
			this.trainingSet [69] = new double[28]
			{0, -1, 0, 0, -1, -1, -1, -1, 1, 1, 0, 0, 1, 0, 1, 0, 0, 1, 1, 0, 0, 0, 1, 0, 1, 1, 0, 1};
			this.trainingSet [70] = new double[28]
			{0, -1, 0, 0, -1, -1, -1, 0, 1, 1, 0, 0, 1, 1, 1, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 1, 1, 1};
			this.trainingSet [71] = new double[28]
			{0, -1, 0, 0, -1, 0, -1, 0, 1, 1, 0, 0, 1, 1, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 1, 1, 1};
			this.trainingSet [72] = new double[28]
			{0, -1, 0, 0, 0, 0, 0, 0, 1, 1, 0, -1, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 0};
			this.trainingSet [73] = new double[28]
			{0, 0, 0, 0, 0, 0, -1, 0, 1, 0, 0, -1, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 1};
			this.trainingSet [74] = new double[28]
			{0, 0, 0, 0, 0, 1, -1, 0, 0, 0, 0, 0, 0, 0, 0, 0, -2, 0, 0, 0, 1, 0, 1, 0, 0, 0, 0, 1};
			this.trainingSet [75] = new double[28]
			{0, 2, 0, 0, 0, 0, -1, 0, 0, 0, 0, 0, 0, -2, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 1, 0, 0};
			this.trainingSet [76] = new double[28]
			{0, 0, 0, 0, 2, 0, 0, 0, 0, 0, -1, 0, 0, -2, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 1, 0};
			this.trainingSet [77] = new double[28]
			{0, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, -1, -2, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 1, 0, 0, 1};
			this.trainingSet [78] = new double[28]
			{0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0, 0, -2, 0, 0, -2, 0, 0, 1, 0, 0, 1, 0, 0, 1, 1, 0};
			this.trainingSet [79] = new double[28]
			{0, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0, -2, 0, 0, 0, 0, -2, 0, 0, 0, 1, 1, 0, 0, 1, 0, 1, 0};
			this.trainingSet [80] = new double[28]
			{0, 0, 0, 0, 0, 0, 0, 0, -2, 0, 2, 0, 0, 0, 0, 0, -2, 0, 0, 1, 0, 1, 0, 0, 0, 1, 1, 1};
			this.trainingSet [81] = new double[28]
			{0, 0, 0, 0, 0, 0, 0, 2, -2, 0, 0, 0, 0, -2, 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 1, 0, 0};
			this.trainingSet [82] = new double[28]
			{0, 0, 0, 0, 2, -2, 0, 0, 0, 0, 0, 0, 0, -2, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 1, 0};
			this.trainingSet [83] = new double[28]
			{0, 0, 0, 0, 0, 0, 2, -2, 0, 0, 0, 0, 0, -2, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 1, 1};
			this.trainingSet [84] = new double[28]
			{0, 0, 0, 2, 0, 0, 0, -2, 0, 0, 0, 0, 0, 0, 0, 0, -2, 0, 0, 0, 0, 1, 1, 0, 0, 1, 1, 0};
			this.trainingSet [85] = new double[28]
			{0, 0, 0, 0, 0, 0, 2, -2, 0, 0, 0, 0, -2, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 1, 1};
			this.trainingSet [86] = new double[28]
			{0, 0, 0, 2, -2, 0, 0, 0, 0, 0, 0, 0, -2, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0};
			this.trainingSet [87] = new double[28]
			{2, 0, 0, 0, 0, 0, 0, 0, 0, 0, -2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1};
			this.trainingSet [88] = new double[28]
			{-1, 0, -1, -1, -1, -1, -1, 0, 0, 1, 0, 0, 0, 1, 1, 1, 1, 1, 0, 1, 1, 0, 1, 0, 1, 0, 1, 1};
			this.trainingSet [89] = new double[28]
			{-1, 0, -1, -1, -1, 0, -1, 0, -1, 1, 0, 1, 0, 0, 1, 1, 1, 1, 0, 1, 1, 1, 1, 0, 1, 1, 0, 0};
			this.trainingSet [90] = new double[28]
			{-1, 0, 0, -1, -1, -1, -1, 0, -1, 1, 0, 1, 1, 0, 1, 0, 1, 1, 0, 1, 1, 0, 0, 0, 1, 0, 1, 0};
			this.trainingSet [91] = new double[28]
			{-1, 0, 0, -1, -1, -1, -1, 0, 0, 1, 1, 0, 0, 0, 1, 0, 1, 1, 1, 0, 0, 0, 0, 0, 1, 0, 1, 1};
			this.trainingSet [92] = new double[28]
			{-1, 0, 0, -1, -1, 0, -1, 0, -1, 1, 1, 1, 0, 0, 1, 0, 0, 1, 0, 1, 0, 1, 1, 0, 0, 1, 1, 1};
			this.trainingSet [93] = new double[28]
			{-1, 0, 0, -1, 0, 0, -1, 0, -1, 1, 1, 0, 0, 0, 1, 0, 0, 1, 0, 1, 1, 1, 0, 0, 0, 1, 1, 1};
			this.trainingSet [94] = new double[28]
			{-1, 0, 0, -1, 0, 0, 0, 1, -1, 1, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 1, 0, 1, 0, 1, 0};
			this.trainingSet [95] = new double[28]
			{-1, 0, 0, -1, 0, 0, 0, 1, 0, 1, 1, -1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 1, 0, 1};
			this.trainingSet [96] = new double[28]
			{0, 0, 0, -1, -1, 1, 0, 0, 0, 1, 1, -1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 1, 0, 0, 0, 0, 1};
			this.trainingSet [97] = new double[28]
			{0, 2, 0, -1, 0, 0, 0, 0, 0, 1, 1, 0, 0, -1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 1, 1, 0};
			this.trainingSet [98] = new double[28]
			{0, 0, 0, -1, 0, 0, 2, 0, 0, 1, 1, 0, 0, 0, 0, 0, -2, 0, 0, 0, 1, 1, 0, 0, 0, 1, 0, 0};
			this.trainingSet [99] = new double[28]
			{0, 0, 0, -1, 2, 0, 0, 0, 0, 1, 1, 0, 0, -2, 0, 0, 0, 0, 0, 1, 0, 0, 1, 0, 0, 1, 1, 0};
			this.trainingSet [100] = new double[28]
			{0, 0, 0, -1, 2, 0, 1, 0, 0, 0, 1, -2, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 1};
			this.trainingSet [101] = new double[28]
			{0, 2, 0, -1, 0, 0, 1, 0, 0, 0, 1, 0, 0, -2, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 1, 0, 1};
			this.trainingSet [102] = new double[28]
			{0, 0, 0, -1, 0, 2, 1, 0, 0, 0, 1, -2, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 1, 0, 0};
			this.trainingSet [103] = new double[28]
			{0, 0, 0, -1, 1, 2, 0, 0, -2, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 1, 0, 0, 0, 1, 0};
			this.trainingSet [104] = new double[28]
			{0, 0, 2, 0, 1, 0, 0, 0, -2, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 1, 0, 0, 0, 0, 1, 1};
			this.trainingSet [105] = new double[28]
			{0, 0, 2, 1, 1, -2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 1};
			this.trainingSet [106] = new double[28]
			{0, 2, 2, 1, 0, 0, 0, -2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 1, 0, 1};
			this.trainingSet [107] = new double[28]
			{0, 0, 2, 1, 0, 2, 0, 0, 0, 0, -2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0};
			this.trainingSet [108] = new double[28]
			{2, 0, 2, 0, 0, 2, -2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 1, 0, 0, 1, 1, 1};
			this.trainingSet [109] = new double[28]
			{2, 0, 2, 0, -2, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 1, 0, 1, 1};
			this.trainingSet [110] = new double[28]
			{2, -2, 2, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 1, 0, 1, 1, 0, 0, 1, 1, 1};
			this.trainingSet [111] = new double[28]
			{2, 0, 2, 0, 0, -2, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 1, 0, 1, 0};
			this.trainingSet [112] = new double[28]
			{2, 0, 2, 0, 0, 0, 0, 0, -2, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0};
			this.trainingSet [113] = new double[28]
			{0, 0, 2, 0, 2, 0, 0, 0, 0, 0, 2, -2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 1, 0, 1};
			this.trainingSet [114] = new double[28]
			{0, 0, 0, 0, 2, 2, 0, 0, -2, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 1};
			this.trainingSet [115] = new double[28]
			{0, 2, 0, 0, 0, 2, 0, 0, 0, 0, 2, -2, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 1, 0, 0, 1, 1, 1};
			this.trainingSet [116] = new double[28]
			{0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 1, 1, 0};
			this.iloscZestawow = 116;
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
