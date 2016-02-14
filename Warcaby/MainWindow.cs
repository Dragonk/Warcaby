using System;
using Gtk;

namespace Warcaby
{
	public partial class MainWindow: Gtk.Window
	{
		private int[] tablica = new int[18];
		private int[] tab = new int[18]; //bufor tablicy
		private int zlapany = -1;
		private bool vsGracz = false;
		private int tura;
		private Siec generuj = new Siec();
		public MainWindow () : base (Gtk.WindowType.Toplevel)
		{
			Build ();
			Button[] buttons = { this.button4, this.button5, this.button6, this.button7, this.button8, this.button9, this.button10, this.button11, this.button12, this.button13, this.button14, this.button15, this.button16, this.button17, this.button18, this.button19, this.button20, this.button21 };
			for (int i = 0; i < 18; i++) {
				buttons [i].Clicked += new EventHandler(buttonsClickEvent);
			}
			Reset ();
		}

		protected void OnDeleteEvent (object sender, DeleteEventArgs a)
		{
			Application.Quit ();
			a.RetVal = true;
		}

		protected void przeciwGracz (object sender, EventArgs e)
		{
			if (AlertPytanie ("Czy na pewno zacząć nową grę typu Gracz vs Gracz?") == true) {
				this.vsGracz = true;
				Reset ();
			}
		}

		protected void przeciwKomputer (object sender, EventArgs e)
		{
			if (AlertPytanie ("Czy na pewno zacząć nową grę typu Gracz vs Komputer?") == true) {
				this.vsGracz = false;
				Reset ();
			}
		}

		protected void OnButton1ClickEvent (object sender, EventArgs e)
		{
			if (AlertPytanie("Czy na pewno zacząć grę od nowa?") == true)
			Reset ();
		}

		protected void OnButton2ClickEvent (object sender, EventArgs e)
		{
			if (AlertPytanie("Czy na pewno zakończyć program?") == true)
			Environment.Exit(0);
		}

		protected void Reset()
		{
			for (int i = 0; i<18; i++){
				if (i < 6)
					this.tablica [i] = -1;
				else if (i > 11)
					this.tablica [i] = 1;
				else
					this.tablica[i] = 0;
			}
			if (this.vsGracz == true) {
				this.tura = 1;
				this.label1.LabelProp = "⊕ - Pion Gracza 2";
				this.label2.LabelProp = "Ω - Dama Gracza 2";
				this.label3.LabelProp = "Θ - Pion Gracza 1";
				this.label4.LabelProp = "Ψ - Dama Gracza 1";
				this.label5.LabelProp = "Tura Gracza 1";
				this.label8.LabelProp = "Podpowiedzi \ndla gracza 2:";
				this.label6.LabelProp = "Z pola numer: ";
				this.label7.LabelProp ="Na pole numer: ";
			} else {
				this.tura = 1;
				this.label1.LabelProp = "⊕ - Pion Komputera";
				this.label2.LabelProp = "Ω - Dama Komputera";
				this.label3.LabelProp = "Θ - Pion Gracza";
				this.label4.LabelProp = "Ψ - Dama Gracza";
				this.label8.LabelProp = "Ruch komputera:";
				this.label6.LabelProp = "Z pola numer: ";
				this.label7.LabelProp = "Na pole numer: ";
				this.label5.LabelProp = " ";
			}
			setButtons ();
		}

		protected void setButtons(){
			Button[] buttons = { this.button4, this.button5, this.button6, this.button7, this.button8, this.button9, this.button10, this.button11, this.button12, this.button13, this.button14, this.button15, this.button16, this.button17, this.button18, this.button19, this.button20, this.button21 };
			int gracz = 0;
			int komputer = 0;
	//Tworzenie damek
			if (checkbutton1.Active == false) {
				for (int i = 0; i < 3; i++) {
					if (this.tablica [i] == 1)
						this.tablica [i] = 2;
					if (this.tablica [17 - i] == -1)
						this.tablica [17 - i] = -2;
				}
				//Sprawdzanie czy pozotaly piony graczowi, badz komputerowi
				for (int i = 0; i < 18; i++) {
					if (this.tablica [i] == 1)
						gracz = gracz + 1;
					if (this.tablica [i] == 2)
						gracz = gracz + 1;
					if (this.tablica [i] == -1)
						komputer = komputer + 1;
					if (this.tablica [i] == -2)
						komputer = komputer + 1;
				}
				if (this.vsGracz == false) {
					if (gracz == 0) {
						Alert ("Wygrał gracz");
						this.label5.LabelProp = "Wygrał gracz";
						Reset ();
					}
					if (komputer == 0) {
						Alert ("Niestety przegrałeś tym razem...");
						this.label5.LabelProp = "Wygrał komputer";
						Reset ();
					}
				} else {
					if (gracz == 0) {
						Alert ("Wygrał gracz 1");
						this.label5.LabelProp = "Wygrał gracz 1";
						Reset ();
					}
					if (komputer == 0) {
						Alert ("Wygrał gracz 2");
						this.label5.LabelProp = "Wygrał gracz 2";
						Reset ();
					}
				}
			}
	//Aktualizowanie planszy
			for (int i = 0; i < 18; i++) {
				if (this.tablica [i] == 1)
					buttons [i].Label = '⊕'.ToString ();
				else if (this.tablica [i] == 2)
					buttons[i].Label = 'Ω'.ToString();
				else if (this.tablica [i] == -1)
					buttons[i].Label = 'Θ'.ToString();
				else if (this.tablica [i] == -2)
					buttons[i].Label = 'Ψ'.ToString();
				else
					buttons[i].Label = ' '.ToString();
			}
		}

		protected void Alert(string informacja) {
			MessageDialog dialog = new MessageDialog(null,
				DialogFlags.Modal,
				MessageType.Info,
				ButtonsType.Ok,
				informacja );
			dialog.Run(); 
			dialog.Destroy(); 
		}

		protected bool AlertPytanie(string pytanie) {
			bool a = new bool();
			MessageDialog dialog = new MessageDialog(null,
				DialogFlags.Modal,
				MessageType.Question,
				ButtonsType.YesNo,
				pytanie );
			if ((ResponseType) dialog.Run() == ResponseType.Yes)
				a = true;
			dialog.Destroy();
			return a;
		}

		protected void buttonsClickEvent (object sender,  EventArgs e)
		{
			Button[] buttons = { this.button4, this.button5, this.button6, this.button7, this.button8, this.button9, this.button10, this.button11, this.button12, this.button13, this.button14, this.button15, this.button16, this.button17, this.button18, this.button19, this.button20, this.button21 };
			int a = Array.IndexOf (buttons, (Button)sender);
			if (checkbutton1.Active == true) {
				if (this.tablica [a] == 2)
					this.tablica [a] = -2;
				else
					this.tablica [a] = this.tablica [a] + 1;
				setButtons ();
				
			}
			else {
				if (this.tura == 1) {
					graczJeden (a);
				} else if (this.tura == 2) {
					graczDwa (a, false);
				}
			}
		}

		protected void graczJeden (int numerPola) {
			Button[] buttons = { this.button4, this.button5, this.button6, this.button7, this.button8, this.button9, this.button10, this.button11, this.button12, this.button13, this.button14, this.button15, this.button16, this.button17, this.button18, this.button19, this.button20, this.button21 };
			if (this.zlapany == -1) {
				if ((this.tablica [numerPola] == -1) || (this.tablica [numerPola] == -2)) {
					this.zlapany = numerPola;
					buttons [numerPola].Label = buttons [numerPola].Label + " Wybrany";
				}
			} else if (this.tablica [numerPola] == 0) {
				if (this.tablica [this.zlapany] == -1) {
					//Poruszanie do przodu zwykłym pionem
					if ((this.zlapany == 0) || (this.zlapany == 1) || (this.zlapany == 6) || (this.zlapany == 7) || (this.zlapany == 12) || (this.zlapany == 13)) {
						if ((numerPola == this.zlapany + 3) || (numerPola == this.zlapany + 4))
							przeniesieniePionka (numerPola);
					}
					if ((this.zlapany == 2) || (this.zlapany == 3) || (this.zlapany == 8) || (this.zlapany == 9) || (this.zlapany == 14) || (this.zlapany == 15)) {
						if (numerPola == this.zlapany + 3)
							przeniesieniePionka (numerPola);
					}
					if ((this.zlapany == 4) || (this.zlapany == 5) || (this.zlapany == 10) || (this.zlapany == 11) || (this.zlapany == 16) || (this.zlapany == 17)) {
						if ((numerPola == this.zlapany + 2) || (numerPola == this.zlapany + 3))
							przeniesieniePionka (numerPola);
					}
					//Bicie do przodu zwykłym pionem
					if ((this.zlapany == 0) || (this.zlapany == 6) || (this.zlapany == 12) || (this.zlapany == 1) || (this.zlapany == 7) || (this.zlapany == 13)) {
						if ((this.tablica [this.zlapany + 4] == 1) || (this.tablica [this.zlapany + 4] == 2)) {
							if (numerPola == this.zlapany + 7) {
								this.tablica [this.zlapany + 4] = 0;
								przeniesieniePionka (numerPola);
							}
						}
					}
					if ((this.zlapany == 2) || (this.zlapany == 8) || (this.zlapany == 14) || (this.zlapany == 1) || (this.zlapany == 7) || (this.zlapany == 13)) {
						if ((this.tablica [this.zlapany + 3] == 1) || (this.tablica [this.zlapany + 3] == 2)) {
							if (numerPola == this.zlapany + 5) {
								this.tablica [this.zlapany + 3] = 0;
								przeniesieniePionka (numerPola);
							}
						}
					}
					if ((this.zlapany == 3) || (this.zlapany == 9) || (this.zlapany == 15) || (this.zlapany == 4) || (this.zlapany == 10)) {
						if ((this.tablica [this.zlapany + 3] == 1) || (this.tablica [this.zlapany + 3] == 2)) {
							if (numerPola == this.zlapany + 7) {
								this.tablica [this.zlapany + 3] = 0;
								przeniesieniePionka (numerPola);
							}
						}
					}
					if ((this.zlapany == 5) || (this.zlapany == 11) || (this.zlapany == 4) || (this.zlapany == 10)) {
						if ((this.tablica [this.zlapany + 2] == 1) || (this.tablica [this.zlapany + 2] == 2)) {
							if (numerPola == this.zlapany + 5) {
								this.tablica [this.zlapany + 2] = 0;
								przeniesieniePionka (numerPola);
							}
						}
					}
				} else if (this.tablica [this.zlapany] == -2) {
					//Poruszanie damą
					if ((this.zlapany == 0) || (this.zlapany == 1) || (this.zlapany == 6) || (this.zlapany == 7) || (this.zlapany == 12) || (this.zlapany == 13)) {
						if ((numerPola == this.zlapany - 2) || (numerPola == this.zlapany - 3) || (numerPola == this.zlapany + 3) || (numerPola == this.zlapany + 4))
							przeniesieniePionka (numerPola);
					}
					if ((this.zlapany == 2) || (this.zlapany == 3) || (this.zlapany == 8) || (this.zlapany == 9) || (this.zlapany == 14) || (this.zlapany == 15)) {
						if ((numerPola == this.zlapany - 3) || (numerPola == this.zlapany + 3))
							przeniesieniePionka (numerPola);
					}
					if ((this.zlapany == 4) || (this.zlapany == 5) || (this.zlapany == 10) || (this.zlapany == 11) || (this.zlapany == 16) || (this.zlapany == 17)) {
						if ((numerPola == this.zlapany - 3) || (numerPola == this.zlapany - 4) || (numerPola == this.zlapany + 2) || (numerPola == this.zlapany + 3))
							przeniesieniePionka (numerPola);
					}
					//Bicie damą
					if ((this.zlapany == 0) || (this.zlapany == 6) || (this.zlapany == 12) || (this.zlapany == 1) || (this.zlapany == 7) || (this.zlapany == 13)) {
						if ((this.tablica [this.zlapany - 2] == 1) || (this.tablica [this.zlapany - 2] == 2)) {
							if (numerPola == this.zlapany - 5) {
								this.tablica [this.zlapany - 2] = 0;
								przeniesieniePionka (numerPola);
							}
						}
					}
					if ((this.zlapany == 2) || (this.zlapany == 8) || (this.zlapany == 14) || (this.zlapany == 1) || (this.zlapany == 7) || (this.zlapany == 13)) {
						if ((this.tablica [this.zlapany - 3] == 1) || (this.tablica [this.zlapany - 3] == 2)) {
							if (numerPola == this.zlapany - 7) {
								this.tablica [this.zlapany - 3] = 0;
								przeniesieniePionka (numerPola);
							}
						}
					}
					if ((this.zlapany == 3) || (this.zlapany == 9) || (this.zlapany == 15) || (this.zlapany == 4) || (this.zlapany == 10) || (this.zlapany == 16)) {
						if ((this.tablica [this.zlapany - 3] == 1) || (this.tablica [this.zlapany - 3] == 2)) {
							if (numerPola == this.zlapany - 5) {
								this.tablica [this.zlapany - 3] = 0;
								przeniesieniePionka (numerPola);
							}
						}
					}
					if ((this.zlapany == 5) || (this.zlapany == 11) || (this.zlapany == 17) || (this.zlapany == 4) || (this.zlapany == 10) || (this.zlapany == 16)) {
						if ((this.tablica [this.zlapany - 4] == 1) || (this.tablica [this.zlapany - 4] == 2)) {
							if (numerPola == this.zlapany - 7) {
								this.tablica [this.zlapany - 4] = 0;
								przeniesieniePionka (numerPola);
							}
						}
					}
				}

				if ((this.zlapany == 0) || (this.zlapany == 6) || (this.zlapany == 12) || (this.zlapany == 1) || (this.zlapany == 7) || (this.zlapany == 13)) {
					if ((this.tablica [this.zlapany + 4] == 1) || (this.tablica [this.zlapany + 4] == 2)) {
						if (numerPola == this.zlapany + 7) {
							this.tablica [this.zlapany + 4] = 0;
							przeniesieniePionka (numerPola);
						}
					}
				}
				if ((this.zlapany == 2) || (this.zlapany == 8) || (this.zlapany == 14) || (this.zlapany == 1) || (this.zlapany == 7) || (this.zlapany == 13)) {
					if ((this.tablica [this.zlapany + 3] == 1) || (this.tablica [this.zlapany + 3] == 2)) {
						if (numerPola == this.zlapany + 5) {
							this.tablica [this.zlapany + 3] = 0;
							przeniesieniePionka (numerPola);
						}
					}
				}
				if ((this.zlapany == 3) || (this.zlapany == 9) || (this.zlapany == 4) || (this.zlapany == 10)) {
					if ((this.tablica [this.zlapany + 3] == 1) || (this.tablica [this.zlapany + 3] == 2)) {
						if (numerPola == this.zlapany + 7) {
							this.tablica [this.zlapany + 3] = 0;
							przeniesieniePionka (numerPola);
						}
					}
				}
				if ((this.zlapany == 5) || (this.zlapany == 11) || (this.zlapany == 4) || (this.zlapany == 10)) {
					if ((this.tablica [this.zlapany + 2] == 1) || (this.tablica [this.zlapany + 2] == 2)) {
						if (numerPola == this.zlapany + 5) {
							this.tablica [this.zlapany + 2] = 0;
							przeniesieniePionka (numerPola);
						}
					}
				}
			} else {
				this.zlapany = -1;
				setButtons ();
			}
		}

		protected bool graczDwa (int numerPola, bool pomoc)
		{
			Button[] buttons = { this.button4, this.button5, this.button6, this.button7, this.button8, this.button9, this.button10, this.button11, this.button12, this.button13, this.button14, this.button15, this.button16, this.button17, this.button18, this.button19, this.button20, this.button21 };
			if ((numerPola < 18) && (numerPola > -1)) {
				if (this.zlapany == -1) {
					if ((this.tablica [numerPola] == 1) || (this.tablica [numerPola] == 2)) {
						this.zlapany = numerPola;
						buttons [numerPola].Label = buttons [numerPola].Label + " Wybrany";
						return false;
					}
				} else if (this.tablica [numerPola] == 0) {
					if (this.tablica [this.zlapany] == 1) {
						//Poruszanie do przodu zwykłym pionem
						if ((this.zlapany == 6) || (this.zlapany == 7) || (this.zlapany == 12) || (this.zlapany == 13)) {
							if ((numerPola == this.zlapany - 2) || (numerPola == this.zlapany - 3))
							if (pomoc == true)
								return true;
							else {
								przeniesieniePionka (numerPola);
								return true;
							}
						}
						if ((this.zlapany == 3) || (this.zlapany == 8) || (this.zlapany == 9) || (this.zlapany == 14) || (this.zlapany == 15)) {
							if (numerPola == this.zlapany - 3)
							if (pomoc == true)
								return true;
							else {
								przeniesieniePionka (numerPola);
								return true;
							}
						}
						if ((this.zlapany == 4) || (this.zlapany == 5) || (this.zlapany == 10) || (this.zlapany == 11) || (this.zlapany == 16) || (this.zlapany == 17)) {
							if ((numerPola == this.zlapany - 3) || (numerPola == this.zlapany - 4))
							if (pomoc == true)
								return true;
							else {
								przeniesieniePionka (numerPola);
								return true;
							}
						}
						//Bicie do przodu zwykłym pionem
						if ((this.zlapany == 6) || (this.zlapany == 12) || (this.zlapany == 7) || (this.zlapany == 13)) {
							if ((this.tablica [this.zlapany - 2] == -1) || (this.tablica [this.zlapany - 2] == -2)) {
								if (numerPola == this.zlapany - 5) {
									if (pomoc == true)
										return true;
									else {
										this.tablica [this.zlapany - 2] = 0;
										przeniesieniePionka (numerPola);
										return true;
									}
								}
							}
						}
						if ((this.zlapany == 8) || (this.zlapany == 14) || (this.zlapany == 7) || (this.zlapany == 13)) {
							if ((this.tablica [this.zlapany - 3] == -1) || (this.tablica [this.zlapany - 3] == -2)) {
								if (numerPola == this.zlapany - 7) {
									if (pomoc == true)
										return true;
									else {
										this.tablica [this.zlapany - 3] = 0;
										przeniesieniePionka (numerPola);
										return true;
									}
								}
							}
						}
						if ((this.zlapany == 9) || (this.zlapany == 15) || (this.zlapany == 10) || (this.zlapany == 16)) {
							if ((this.tablica [this.zlapany - 3] == -1) || (this.tablica [this.zlapany - 3] == -2)) {
								if (numerPola == this.zlapany - 5) {
									if (pomoc == true)
										return true;
									else {
										this.tablica [this.zlapany - 3] = 0;
										przeniesieniePionka (numerPola);
										return true;
									}
								}
							}
						}
						if ((this.zlapany == 11) || (this.zlapany == 17) || (this.zlapany == 10) || (this.zlapany == 16)) {
							if ((this.tablica [this.zlapany - 4] == -1) || (this.tablica [this.zlapany - 4] == -2)) {
								if (numerPola == this.zlapany - 7) {
									if (pomoc == true)
										return true;
									else {
										this.tablica [this.zlapany - 4] = 0;
										przeniesieniePionka (numerPola);
										return true;
									}
								}
							}
						}
					} else if (this.tablica [this.zlapany] == 2) {
						//Poruszanie damą
						if ((this.zlapany == 0) || (this.zlapany == 1) || (this.zlapany == 6) || (this.zlapany == 7) || (this.zlapany == 12) || (this.zlapany == 13)) {
							if ((numerPola == this.zlapany - 2) || (numerPola == this.zlapany - 3) || (numerPola == this.zlapany + 3) || (numerPola == this.zlapany + 4))
							if (pomoc == true)
								return true;
							else {
								przeniesieniePionka (numerPola);
								return true;
							}
						}
						if ((this.zlapany == 2) || (this.zlapany == 3) || (this.zlapany == 8) || (this.zlapany == 9) || (this.zlapany == 14) || (this.zlapany == 15)) {
							if ((numerPola == this.zlapany - 3) || (numerPola == this.zlapany + 3))
							if (pomoc == true)
								return true;
							else {
								przeniesieniePionka (numerPola);
								return true;
							}
						}
						if ((this.zlapany == 4) || (this.zlapany == 5) || (this.zlapany == 10) || (this.zlapany == 11) || (this.zlapany == 16) || (this.zlapany == 17)) {
							if ((numerPola == this.zlapany - 3) || (numerPola == this.zlapany - 4) || (numerPola == this.zlapany + 2) || (numerPola == this.zlapany + 3))
							if (pomoc == true)
								return true;
							else {
								przeniesieniePionka (numerPola);
								return true;
							}
						}
						//Bicie damą
						if ((this.zlapany == 6) || (this.zlapany == 12) || (this.zlapany == 7) || (this.zlapany == 13)) {
							if ((this.tablica [this.zlapany - 2] == -1) || (this.tablica [this.zlapany - 2] == -2)) {
								if (numerPola == this.zlapany - 5) {
									if (pomoc == true)
										return true;
									else {
										this.tablica [this.zlapany - 2] = 0;
										przeniesieniePionka (numerPola);
										return true;
									}
								}
							}
						}
						if ((this.zlapany == 8) || (this.zlapany == 14) || (this.zlapany == 7) || (this.zlapany == 13)) {
							if ((this.tablica [this.zlapany - 3] == -1) || (this.tablica [this.zlapany - 3] == -2)) {
								if (numerPola == this.zlapany - 7) {
									if (pomoc == true)
										return true;
									else {
										this.tablica [this.zlapany - 3] = 0;
										przeniesieniePionka (numerPola);
										return true;
									}
								}
							}
						}
						if ((this.zlapany == 9) || (this.zlapany == 15) || (this.zlapany == 10) || (this.zlapany == 16)) {
							if ((this.tablica [this.zlapany - 3] == -1) || (this.tablica [this.zlapany - 3] == -2)) {
								if (numerPola == this.zlapany - 5) {
									if (pomoc == true)
										return true;
									else {
										this.tablica [this.zlapany - 3] = 0;
										przeniesieniePionka (numerPola);
										return true;
									}
								}
							}
						}
						if ((this.zlapany == 11) || (this.zlapany == 17) || (this.zlapany == 10) || (this.zlapany == 16)) {
							if ((this.tablica [this.zlapany - 4] == -1) || (this.tablica [this.zlapany - 4] == -2)) {
								if (numerPola == this.zlapany - 7) {
									if (pomoc == true)
										return true;
									else {
										this.tablica [this.zlapany - 4] = 0;
										przeniesieniePionka (numerPola);
										return true;
									}
								}
							}
						}
						if ((this.zlapany == 0) || (this.zlapany == 6) || (this.zlapany == 1) || (this.zlapany == 7)) {
							if ((this.tablica [this.zlapany + 4] == -1) || (this.tablica [this.zlapany + 4] == -2)) {
								if (numerPola == this.zlapany + 7) {
									if (pomoc == true)
										return true;
									else {
										this.tablica [this.zlapany + 4] = 0;
										przeniesieniePionka (numerPola);
										return true;
									}
								}
							}
						}
						if ((this.zlapany == 2) || (this.zlapany == 8) || (this.zlapany == 1) || (this.zlapany == 7)) {
							if ((this.tablica [this.zlapany + 3] == -1) || (this.tablica [this.zlapany + 3] == -2)) {
								if (numerPola == this.zlapany + 5) {
									if (pomoc == true)
										return true;
									else {
										this.tablica [this.zlapany + 3] = 0;
										przeniesieniePionka (numerPola);
										return true;
									}
								}
							}
						}
						if ((this.zlapany == 3) || (this.zlapany == 9) || (this.zlapany == 4) || (this.zlapany == 10)) {
							if ((this.tablica [this.zlapany + 3] == -1) || (this.tablica [this.zlapany + 3] == -2)) {
								if (numerPola == this.zlapany + 7) {
									if (pomoc == true)
										return true;
									else {
										this.tablica [this.zlapany + 3] = 0;
										przeniesieniePionka (numerPola);
										return true;
									}
								}
							}
						}
						if ((this.zlapany == 5) || (this.zlapany == 11) || (this.zlapany == 4) || (this.zlapany == 10)) {
							if ((this.tablica [this.zlapany + 2] == -1) || (this.tablica [this.zlapany + 2] == -2)) {
								if (numerPola == this.zlapany + 5) {
									if (pomoc == true)
										return true;
									else {
										this.tablica [this.zlapany + 2] = 0;
										przeniesieniePionka (numerPola);
										return true;
									}
								}
							}
						}
					}
				}
				this.zlapany = -1;
				setButtons ();
				return false;
			}
			this.zlapany = -1;
			setButtons ();
			return false;
		}

		private void przeniesieniePionka(int a){
			if ((this.tura == 1) && (this.vsGracz == true)) {
				if (this.tablica [this.zlapany] == -1) {
					this.tablica [this.zlapany] = 0;
					this.tablica [a] = -1;
					this.zlapany = -1;
					this.tura = 2;
					this.label5.LabelProp = "Tura Gracza 2";
					setButtons ();
					podpowiedz ();
				} else if (this.tablica [this.zlapany] == -2) {
					this.tablica [this.zlapany] = 0;
					this.tablica [a] = -2;
					this.zlapany = -1;
					this.tura = 2;
					this.label5.LabelProp = "Tura Gracza 2";
					setButtons ();
					podpowiedz ();
				}
			} else if ((this.tura == 2) && (this.vsGracz == true)) {
				if (this.tablica [this.zlapany] == 1) {
					this.generuj.nowyZestaw(this.tab,this.zlapany,a);
					this.tablica [this.zlapany] = 0;
					this.tablica [a] = 1;
					this.zlapany = -1;
					this.tura = 1;
					this.label5.LabelProp = "Tura Gracza 1";
					setButtons ();
				} else if (this.tablica [this.zlapany] == 2) {
					this.generuj.nowyZestaw(this.tab,this.zlapany,a);
					this.tablica [this.zlapany] = 0;
					this.tablica [a] = 2;
					this.zlapany = -1;
					this.tura = 1;
					this.label5.LabelProp = "Tura Gracza 1";
					setButtons ();
				}
			} else if ((this.tura == 1) && (this.vsGracz == false)) {
				if (this.tablica [this.zlapany] == -1) {
					this.tablica [this.zlapany] = 0;
					this.tablica [a] = -1;
					this.zlapany = -1;
					this.tura = 2;
					setButtons ();
					ruchKomputera ();
				} else if (this.tablica [this.zlapany] == -2) {
					this.tablica [this.zlapany] = 0;
					this.tablica [a] = -2;
					this.zlapany = -1;
					this.tura = 2;
					setButtons ();
					ruchKomputera ();
				}
			} else if ((this.tura == 2) && (this.vsGracz == false)) {
			if (this.tablica [this.zlapany] == 1) {
					this.generuj.nowyZestaw(this.tab,this.zlapany,a);
					this.tablica [this.zlapany] = 0;
					this.tablica [a] = 1;
					this.zlapany = -1;
					this.tura = 1;
					setButtons ();
				} else if (this.tablica [this.zlapany] == 2) {
					this.generuj.nowyZestaw(this.tab,this.zlapany,a);
					this.tablica [this.zlapany] = 0;
					this.tablica [a] = 2;
					this.zlapany = -1;
					this.tura = 1;
					setButtons ();
				}
			}
		}

		protected void podpowiedz () {
			bool back;
			int ilosc = 0;
			for (int i = 0; i < 18; i++) {
				if ((this.tablica [i] == 1) || (this.tablica [i] == 2))
					ilosc = ilosc + 1;
			}
			if (ilosc > 0) {
				this.tab = this.tablica;
				//Odwołanie do sztucznej inteligencji
				do {
					this.generuj.trenuj();
					this.generuj.generowanieRuchu (this.tablica);
					back = this.graczDwa (this.generuj.starePole, true);
					back = this.graczDwa (this.generuj.nowePole, true);
					back = true;
					this.label6.LabelProp = "Z pola numer: " + (this.generuj.starePole+1);
					this.label7.LabelProp ="Na pole numer: " + (this.generuj.nowePole+1);
					this.zlapany = -1;
					setButtons ();
				} while (back == false);
			}
		}

		protected void ruchKomputera () {
			int ilosc = 0;
			for (int i = 0; i < 18; i++) {
				if ((this.tablica [i] == 1) || (this.tablica [i] == 2))
					ilosc = ilosc + 1;
			}
			if (ilosc>0)
			if ((this.tura == 2) && (this.vsGracz == false)) {
				this.tab = this.tablica;
			//Odwołanie do sztucznej inteligencji
				do {
					this.generuj.trenuj();
					this.generuj.generowanieRuchu (this.tablica);
					this.graczDwa (this.generuj.starePole, false);
					this.graczDwa (this.generuj.nowePole, false);
					this.label6.LabelProp = "Z pola numer: " + (this.generuj.starePole+1);
					this.label7.LabelProp ="Na pole numer: " + (this.generuj.nowePole+1);
					this.zlapany = -1;
				} while (this.tura == 2);
			}
		}
	}
}
