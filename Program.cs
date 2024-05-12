using System;
using System.ComponentModel.Design;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using Microsoft.VisualBasic;
using System.IO;

namespace Football_bets
{
	class Program
	{
		static double 		result, rate, balance, temp_rate, bet;
		static bool			correct_result = true, session;
		static bool[]?		correct_match = new bool[File.ReadLines(@"zapasy.txt").Count()];
		static bool[]?		picked_match = new bool[File.ReadLines(@"zapasy.txt").Count()];
		static bool[]?		used_match = new bool[File.ReadLines(@"zapasy.txt").Count()];
		static string[][]?	picked_rate = new string[File.ReadLines(@"zapasy.txt").Count()][];
		static string[][]?	data;

		//funkce pro resetovani vybranych zapasu a kurzu
		static void Reset()
		{
			int	i;

			i = 0;
			rate = 1.00;
			correct_result = true;
			while (picked_match != null && i < picked_match.Length)
			{
				picked_match[i] = false;
				i++;
			}
		}

		//funkce pro zkontrolovani zbycvajicich zapasu, jestli uzivatel vsechny zvyuzil tak se aplikace vypne
		static void Check_matches()
		{
			int		i;
			string	array;
			string?	line;

			i = 0;
			while (used_match != null && i < used_match.Length)
			{
				if (used_match[i] == true)
					i++;
				else
					return;
			}
			if (used_match != null && i == used_match.Length)
			{
				Console.WriteLine("\n------------------------------------------------------------------------------");
				Console.WriteLine("|                  Vycerpal jste vsechny dostupne zapasy!                    |");
				Console.WriteLine("------------------------------------------------------------------------------");
				Console.Write("\n  Konecny zustatek uctu: ");
				Console.ForegroundColor = ConsoleColor.Cyan;
				Console.WriteLine(balance.ToString("#,0.00") + " Kc");
				Console.ForegroundColor = ConsoleColor.White;
				Console.Write("\nPro zobrazeni posledniho tiketu stisknete ");
				Yellow_enter();
				Console.WriteLine("");
				using (StreamReader sr = new StreamReader(@"posledni_sazka.txt"))
				{
					while (!sr.EndOfStream)
					{
						line = sr.ReadLine();
						if (line != null)
						{
							array = line;
							Console.WriteLine(array);
						}
					}
				}
				Console.Write("\nPro pokracovani ukonceni aplikace stisknete ");
				Yellow_enter();
				Console.WriteLine("\n------------------------------------------------------------------------------");
				Console.WriteLine("|                                                                            |");
				Console.WriteLine("|     Dekujeme za ucast v aplikaci pro sazeni na Champions League 23/24!     |");
				Console.Write("|                       ");
				Console.ForegroundColor = ConsoleColor.Green;
				Console.Write("'VSADTE BARAK, VYHRAJTE DVA!'");
				Console.ForegroundColor = ConsoleColor.White;
				Console.WriteLine("                        |");
				Console.WriteLine("|                                                                            |");
				Console.WriteLine("------------------------------------------------------------------------------");
				Console.WriteLine("\n			                     ___");
				Console.WriteLine("			 o__        o__     |   |\\");
				Console.WriteLine("			/|          /\\      |   |X\\");
				Console.WriteLine("			/ > o        <\\     |   |XX\\");
				Console.WriteLine("------------------------------------------------------------------------------");
				Console.Write("\nPro ukonceni aplikace stisknete ");
				Yellow_enter();
				Console.Write("\nVypinani aplikace, vyckejte prosim.");
				Thread.Sleep(500);
				Console.Write(".");
				Thread.Sleep(500);
				Console.Write(".");
				Thread.Sleep(750);
				Console.Clear();
				Environment.Exit(0);
			}
		}

		//funkce pro vypis zluteho enteru pro pokracovani dale
		static void Yellow_enter()
		{
			Console.Write("libovolnou ");
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.Write("KLAVESU");
			Console.ForegroundColor = ConsoleColor.White;
			Console.Write(".");
			Console.ReadKey();
			Console.Clear();
		}

		//menu pro funkci select rate
		static void Select_rate_menu(int selection)
		{
			if (data != null)
			{
				Console.Clear();
				Console.WriteLine("\n------------------------------------------------------------------------------");
				Console.WriteLine("|                                                                            |");
				Console.WriteLine("|                             VYBER PRILEZITOSTI                             |");
				Console.WriteLine("|                                                                            |");
				Console.WriteLine("------------------------------------------------------------------------------");
				Console.WriteLine(" " + data[selection][0] + " \t\t\t" + data[selection][1] + " \t\t\t" + data[selection][2]);
				Console.WriteLine("  " + data[selection][6] + " \t\t\t " + data[selection][7] + " \t\t\t  " + data[selection][8]);
				Console.ForegroundColor = ConsoleColor.Yellow;
				Console.WriteLine("   [1]  \t\t\t  [x] \t\t\t   [2]");
				Console.ForegroundColor = ConsoleColor.White;
				if (rate != 1.00)
				{
					Console.WriteLine("------------------------------------------------------------------------------");
					Console.Write("  Kurz tiketu:   ");
					Console.ForegroundColor = ConsoleColor.Cyan;
					Console.WriteLine(rate.ToString("#,0.00"));
					Console.ForegroundColor = ConsoleColor.White;
				}
				Console.WriteLine("------------------------------------------------------------------------------");
				Console.Write("\nVyberte si prosim z nasledujicich prilezitosti: ");
			}
		}

		//funkce pro vyber kurzu prilezitosti zapasu ktery se prida k tiketu a zaroven zapamatovani volby uzivatele, take kontroluje povoleny pocet spatnych vstupu
		static double Input_select_rate(int selection, double home_rate, double draw_rate, double away_rate, int i)
		{
			string?	input;

			input = Console.ReadLine();
			if (picked_rate != null)
				picked_rate[selection] = new string[3] {"0","0","0"};
			switch (input)
			{
				case "1":
					if (picked_rate != null && picked_match != null)
					{
						picked_rate[selection][0] = "1";
						picked_match[selection] = true;
					}
					return home_rate;
				case "x":
					if (picked_rate != null && picked_match != null)
					{
						picked_rate[selection][1] = "1";
						picked_match[selection] = true;
					}
					return draw_rate;
				case "X":
					if (picked_rate != null && picked_match != null)
					{
						picked_rate[selection][1] = "1";
						picked_match[selection] = true;
					}
					return draw_rate;
				case "2":
					if (picked_rate != null && picked_match != null)
					{
						picked_rate[selection][2] = "1";
						picked_match[selection] = true;
					}
					return away_rate;
				default:
					if (++i == 5)
					{
						Console.ForegroundColor = ConsoleColor.Red;
						Console.WriteLine("\n------------------------------------------------------------------------------");
						Console.WriteLine("|                   Byl prekrocen povoleny pocet pokusu!                     |"); 
						Console.WriteLine("------------------------------------------------------------------------------");
						Console.ForegroundColor = ConsoleColor.White;
						Console.Write("\nPro pokracovani k vyberu zapasu stisknete ");
						Yellow_enter();
						Select_match();
						return 1;
					}
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine("\n------------------------------------------------------------------------------");
					Console.WriteLine("|                        Vybrany kurz neni v nabidce!                        |");
					Console.WriteLine("------------------------------------------------------------------------------");
					Console.ForegroundColor = ConsoleColor.White;
					Console.Write("\nPro zopakovani vyberu stisknete ");
					Yellow_enter();
					Select_rate_menu(selection);
					temp_rate = Input_select_rate(selection, home_rate, draw_rate, away_rate, i);
					return temp_rate;
			}
		} 

		//funkce pro vypis erroru po spatne nactenem souboru (contents byly upraveny)
		static void File_error()
		{
			Console.Clear();
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine("\n------------------------------------------------------------------------------");
			Console.WriteLine("|                  Nacitany soubor byl upraven nebo smazan,                  |\n|                kvuli tomu nebude program spravne fungovat!                 |");
			Console.WriteLine("------------------------------------------------------------------------------");
			Console.ForegroundColor = ConsoleColor.White;
			Console.Write("\nPro vypnuti aplikace stisknete ");
			Yellow_enter();
			Environment.Exit(0);
		}

		//funkce pro vypis erroru pri spatnem vstupu z funkce Input_select_match 
		static void Error_select_match()
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine("\n------------------------------------------------------------------------------");
			Console.WriteLine("|                         Vybrana moznost neexistuje!                        |");
			Console.WriteLine("------------------------------------------------------------------------------");
			Console.ForegroundColor = ConsoleColor.White;
			Console.Write("\nPro zopakovani vyberu stisknete ");
			Yellow_enter();
		}

		//funkce pro vypis menu s vyberem zapasu ktere jsou nacteny ze souboru kde kazdej zapas ma vlastni index v poli data[] a svoje informace v druhym poli data[][]
		static void Select_match()
		{
			int			i;
			double		garbage;
			string?		line;
			string[]? 	line_item;

			try
			{
				if (!File.Exists("zapasy.txt"))
					File_error();
			}
			catch (Exception e)
			{
				Console.Write($"Error: {e.Message}");
			 	Yellow_enter();
				File_error();
			}
			i = 0;
			data = new string[File.ReadLines(@"zapasy.txt").Count()][];
			Console.Clear();
			Console.WriteLine("\n------------------------------------------------------------------------------");
			Console.WriteLine("|                                                                            |");
			Console.WriteLine("|                                VYBER ZAPASU                                |");
			Console.WriteLine("|                                                                            |");
			Console.WriteLine("------------------------------------------------------------------------------");
			Console.WriteLine("|                                                                            |");
			using (StreamReader sr = new StreamReader(@"zapasy.txt"))
			{
				while (!sr.EndOfStream)
				{
					line = sr.ReadLine();
					if (line != null)
					{
						line_item = line.Split(';');
						data[i] = line_item;

						//kontrola informaci v souboru
						if ((data[i][3] != "0" && data[i][3] != "1")
							|| (data[i][4] != "0" && data[i][4] != "1")
							|| (data[i][5] != "0" && data[i][5] != "1"))
							File_error();
						if ((data[i][3] == "1" && (data[i][4] == "1" || data[i][5] == "1"))
							|| (data[i][4] == "1" && (data[i][3] == "1" || data[i][5] == "1"))
							|| (data[i][5] == "1" && (data[i][3] == "1" || data[i][4] == "1")))
								File_error();
						if ((!double.TryParse(data[i][6], out garbage))
							|| (!double.TryParse(data[i][7], out garbage))
							|| (!double.TryParse(data[i][8], out garbage)))
								File_error();

						//kontrola vybranych zapasu
						if (picked_match != null && picked_match[i] == true)
						{
							Console.Write("| ");
							Console.BackgroundColor = ConsoleColor.DarkGray;
							Console.Write(data[i][0] + " \t Vs \t\t " + data[i][2]);
						}
						//kontrola POUZITYCH zapasu
						else if (used_match != null && used_match[i] == true)
						{
							Console.Write("| ");
							Console.ForegroundColor = ConsoleColor.DarkGray;
							Console.Write(data[i][0] + " \t Vs \t\t " + data[i][2]);
						}
						else
						{
							Console.Write("| " + data[i][0] + " \t Vs \t\t " + data[i][2]);
							Console.ForegroundColor = ConsoleColor.Yellow;
						}
						Console.Write(" \t\t [" + (i + 1) + "]");
						Console.BackgroundColor = ConsoleColor.Black;
						Console.ForegroundColor = ConsoleColor.White;
						Console.WriteLine(" |");
						Console.WriteLine("|                                                                            |");
					}
					i++;
				}
			}
			Console.WriteLine("------------------------------------------------------------------------------");
			Console.WriteLine("|                                                                            |");
			Console.Write("| Hlavni menu                                                            ");
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.Write("[0]");
			Console.ForegroundColor = ConsoleColor.White;
			Console.WriteLine(" |");
			Console.WriteLine("|                                                                            |");
			if (rate != 1.00)
			{
				Console.WriteLine("------------------------------------------------------------------------------");
				Console.Write("  Kurz tiketu:   ");
				Console.ForegroundColor = ConsoleColor.Cyan;
				Console.WriteLine(rate.ToString("#,0.00"));
				Console.ForegroundColor = ConsoleColor.White;
			}
			Console.WriteLine("------------------------------------------------------------------------------");
			Console.Write("\nVyberte si prosim z nasledujicich moznosti: ");
			Input_select_match();
		}

		//funkce ktera zajisti spravny vstup od uzivatele z funkce match_select (nemuze vybrat zapas ktery uz vybral a zapas ktery neexistuje)
		static void Input_select_match() 
		{
			int		selection;
			string?	input;
			double	home_rate, away_rate, draw_rate;

			input = Console.ReadLine();
			if (!int.TryParse(input, out selection))
			{
				Error_select_match();
				Select_match();
				return;
			}
			if (selection == 0)
			{
				Main_menu();
				return;
			}

			//kontrola jestli si uzivatel nevybral uz vybrany zapas
			if (data != null && selection >= 1 && selection <= data.Length && picked_match != null && picked_match[selection - 1] == true)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("\n------------------------------------------------------------------------------");
				Console.WriteLine("|                     Tento zapas jiz mate na tiketu!                        |");
				Console.WriteLine("------------------------------------------------------------------------------");
				Console.ForegroundColor = ConsoleColor.White;
				Console.Write("\nPro zopakovani vyberu stisknete ");
				Yellow_enter();
				Select_match();
				return;
			}

			//kontrola jestli si uzivatel nevybral jiz POUZITY zapas
			else if (data != null && selection >= 1 && selection <= data.Length && used_match != null && used_match[selection - 1] == true)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("\n------------------------------------------------------------------------------");
				Console.WriteLine("|                       Tento zapas jste jiz vsadil!                         |");
				Console.WriteLine("------------------------------------------------------------------------------");
				Console.ForegroundColor = ConsoleColor.White;
				Console.Write("\nPro zopakovani vyberu stisknete ");
				Yellow_enter();
				Select_match();
				return;
			}

			//nacteni prilezitosti vybraneho zapasu a zaroven vyvolani fuknce Select_rate pro zapamatovani vybraneho kurzu
			else if (data != null && selection >= 1 && selection <= data.Length)
			{
				selection--;
				home_rate = double.Parse(data[selection][6]);
				draw_rate = double.Parse(data[selection][7]);
				away_rate = double.Parse(data[selection][8]);
				Select_rate_menu(selection);

				//ulozeni vybraneho kurzu 
				temp_rate = Input_select_rate(selection, home_rate, draw_rate, away_rate, 0);
				rate = Math.Round(rate * temp_rate, 2);

				//vyhodnoceni prilezitosti
				if (picked_rate != null && correct_match != null && data[selection][3] == picked_rate[selection][0] && data[selection][4] == picked_rate[selection][1] && data[selection][5] == picked_rate[selection][2])
				{
					correct_match[selection] = true;
				}
				else
				{
					if (correct_match != null)
						correct_match[selection] = false;
					correct_result = false;
				}
				Select_match();
			}
			else
			{
				Error_select_match();
				Select_match();
			}
		}

		//funkce pro zobrazeni menu funkce deposit money
		static void Deposit_money_menu()
		{
			Console.Clear();
			Console.WriteLine("\n------------------------------------------------------------------------------");
			Console.WriteLine("|                                                                            |");
			Console.WriteLine("|                                VKLAD PENEZ                                 |");
			Console.WriteLine("|                                                                            |");
			Console.WriteLine("------------------------------------------------------------------------------");
			Console.Write("  Zustatek uctu: ");
			Console.ForegroundColor = ConsoleColor.Cyan;
			Console.WriteLine(balance.ToString("#,0.00") + " Kc");
			Console.ForegroundColor = ConsoleColor.White;
			Console.WriteLine("------------------------------------------------------------------------------");
		}
		
		//funkce pro vklad penez ktera zaroven zajistuje spravny vstup (minimalni vklad 100, castka musi mit maximalne dve desetinna cisla)
		//povoleny pocet pokusu je 5 a pri spravnem vstupu se uzivatelovi prictou penize a je poslan zpatky do main menu
		static void Deposit_money()
		{
			int		i;
			string?	input;
			
			i = 0;
			Deposit_money_menu();
			Console.Write("\nZadejte castku kterou chcete vlozit: ");
			input = Console.ReadLine();
			while (!double.TryParse(input, out result) || result < 100 || !(result == Math.Round(result, 2)) || result > 100000 || balance + result > 1000000)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("\n------------------------------------------------------------------------------");
				if (++i == 5)
				{
					Console.WriteLine("|                   Byl prekrocen povoleny pocet pokusu!                     |"); 
					Console.WriteLine("------------------------------------------------------------------------------");
					Console.ForegroundColor = ConsoleColor.White;
					Console.Write("\nPro pokracovani do menu stisknete ");
					Yellow_enter();
					Main_menu();
					return;
				}
				else if (double.TryParse(input, out result) && result < 100)
					Console.WriteLine("|            Zadana castka je mensi nez minimalni vklad 100 Kc!              |");
				else if (double.TryParse(input, out result) && result > 100000)
					Console.WriteLine("|            Zadana castka je vetsi nez maximalni vklad 100 000 Kc!          |");
				else if (double.TryParse(input, out result) && result + balance > 1000000)
				{
					Console.WriteLine("|     Celkovy zustatek uctu by presahoval maximalni hodnotu 1 000 000 Kc!    |");
				}
				else if (!(result == Math.Round(result, 2)))
					Console.WriteLine("|             Zadana castka neni ve spravnem formatu (x,xx) Kc!              |");
				else
					Console.WriteLine("|                            Chybne zadana castka!                           |");
				Console.WriteLine("------------------------------------------------------------------------------");
				Console.ForegroundColor = ConsoleColor.White;
				Console.Write("\nPro zopakovani vkladu stisknete ");
				Yellow_enter();
				Deposit_money_menu();
				Console.Write("\nZadejte castku kterou chcete vlozit: ");
				input = Console.ReadLine();
			}
			balance += result;
			Deposit_money_menu();
			Console.Write("\nZadejte castku kterou chcete vlozit: " + result);
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine("\n\n------------------------------------------------------------------------------");
			Console.WriteLine("                      Vklad "+ result.ToString("#,0.00") +" Kc probehl uspesne!                      ");
			Console.WriteLine("------------------------------------------------------------------------------");
			Console.ForegroundColor = ConsoleColor.White;
			Console.Write("\nPro pokracovani zpatky do menu stisknete ");
			Yellow_enter();
			Main_menu();
		}

		//funkce pro vypis aktualniho tiketu a vybranych prilezitosti uzivatele
		static void Show_ticket()
		{
			int	i;

			i = 0;
			Console.Clear();
			Console.WriteLine("\n------------------------------------------------------------------------------");
			Console.WriteLine("|                                                                            |");
			Console.WriteLine("|                                   TIKET                                    |");
			Console.WriteLine("|                                                                            |");
			Console.WriteLine("------------------------------------------------------------------------------");
			while (picked_match != null && picked_rate != null && data != null && i < picked_match.Length)
			{
				if (picked_match[i] == true)
				{
					Console.ForegroundColor = ConsoleColor.DarkGreen;
					Console.WriteLine("  " + data[i][0] + " \t - \t " + data[i][2]);
					Console.ForegroundColor = ConsoleColor.White;
					if (picked_rate[i][0] == "1")
					{
						Console.WriteLine("  " + data[i][0]);
						Console.WriteLine(" " + data[i][6]);
					}
					if (picked_rate[i][1] == "1")
					{
						Console.WriteLine("  " + data[i][1]);
						Console.WriteLine(" " + data[i][7]);
					}
					if (picked_rate[i][2] == "1")
					{
						Console.WriteLine("  " + data[i][2]);
						Console.WriteLine(" " + data[i][8]);
					}
					Console.WriteLine("------------------------------------------------------------------------------");
				}
				i++;
			}
			Console.Write("  Kurz tiketu:   ");
			Console.ForegroundColor = ConsoleColor.Cyan;
			Console.WriteLine(rate.ToString("#,0.00"));
			Console.ForegroundColor = ConsoleColor.White;
		}

		//funkce pro nacteni posledniho vsazeneho tiketu
		static void Load_last_ticket()
		{
			string	array;
			string? line;

			Console.Clear();
			Console.WriteLine("");
			using (StreamReader sr = new StreamReader(@"posledni_sazka.txt"))
			{
				while (!sr.EndOfStream)
				{
					line = sr.ReadLine();
					if (line != null)
					{
						array = line;
						Console.WriteLine(array);
					}
				}
			}
			Console.Write("\nPro pokracovani zpatky do menu stisknete ");
			Yellow_enter();
			Main_menu();
		}

		//funkce pro ulozeni posledni sazky do souboru
		static void Save_bet()
		{
			int i;

			i = 0;
			using (StreamWriter sw = new StreamWriter(@"posledni_sazka.txt"))
			{
				sw.WriteLine("------------------------------------------------------------------------------");
				sw.WriteLine("|                                                                            |");
				sw.WriteLine("|                                   TIKET                                    |");
				sw.WriteLine("|                                                                            |");
				sw.WriteLine("------------------------------------------------------------------------------");
				while (picked_match != null && i < picked_match.Length)
				{
					if (picked_match[i] == true && data != null && picked_rate != null)
					{
						if (used_match != null)
							used_match[i] = true;

						sw.Write("  " + data[i][0] + "      -      " + data[i][2]);
						if (correct_match != null && correct_match[i] == true)
							sw.WriteLine("                                ✓");
						else
							sw.WriteLine("                                𐄂");
						if (picked_rate[i][0] == "1")
						{
							sw.WriteLine("  " + data[i][0]);
							sw.WriteLine(" " + data[i][6]);
						}
						if (picked_rate[i][1] == "1")
						{
							sw.WriteLine("  " + data[i][1]);
							sw.WriteLine(" " + data[i][7]);
						}
						if (picked_rate[i][2] == "1")
						{
							sw.WriteLine("  " + data[i][2]);
							sw.WriteLine(" " + data[i][8]);
						}
						sw.WriteLine("------------------------------------------------------------------------------");
					}
					i++;
				}
				sw.WriteLine("  Celkovy kurz:        " + rate);
				sw.WriteLine("  Vklad:               " + bet + " Kc");
				if (correct_result == true)
					sw.WriteLine("  Skutecna vyhra:      " + result + " Kc");
				else
					sw.WriteLine("  Skutecna vyhra:      0 Kc");
				sw.Flush();
			}
		}

		//funkce pro vypis menu ve funkci bet
		static void Bet_menu()
		{
			Show_ticket();
			Console.Write("  Zustatek uctu: ");
			Console.ForegroundColor = ConsoleColor.Cyan;
			Console.WriteLine(balance.ToString("#,0.00") + " Kc");
			Console.ForegroundColor = ConsoleColor.White;
			Console.WriteLine("------------------------------------------------------------------------------");
		}
		
		//funkce pro vsazeni tiketu, osetreni vstupu, nasledne vypise vyledek vyherni/proherni
		static void Bet()
		{
			int		i;
			string? input;

			i = 0;
			input = Console.ReadLine();
			while (!double.TryParse(input, out bet) || bet > balance || bet < 10.00)
			{
				if (++i == 5)
				{
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine("\n------------------------------------------------------------------------------");
					Console.WriteLine("|                   Byl prekrocen povoleny pocet pokusu!                     |"); 
					Console.WriteLine("------------------------------------------------------------------------------");
					Console.ForegroundColor = ConsoleColor.White;
					Console.Write("\nPro pokracovani do menu stisknete ");
					Yellow_enter();
					Main_menu();
					return;
				}
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("\n------------------------------------------------------------------------------");
				if (double.TryParse(input, out bet) && bet < 10.00)
					Console.WriteLine("|         Zadana castka je mensi nez minimalni hodnota sazky 10 Kc!          |");
				else if (double.TryParse(input, out bet) && bet > balance)
					Console.WriteLine("|               Zadana castka prevysuje vas aktualni stav uctu!              |");
				else if (!(bet == Math.Round(bet, 2)))
					Console.WriteLine("|         Zadana castka neslnuje maximalne dve desetinna mista!         |");
				else
					Console.WriteLine("|                            Chybne zadana castka!                           |");
				Console.WriteLine("------------------------------------------------------------------------------");
				Console.ForegroundColor = ConsoleColor.White;
				Console.Write("\nPro zopakovani hodnoty stisknete ");
				Yellow_enter();
				Bet_menu();
				Console.Write("\nZadejte castku kterou chcete vsadit: ");
				input = Console.ReadLine();
			}
			if (correct_result == true)
			{
				result = Math.Round(bet * rate, 2);
				balance -= bet;
				balance += result;
				Bet_menu();
				Console.Write("\nZadejte castku kterou chcete vsadit: " + bet);
				Console.ForegroundColor = ConsoleColor.Green;
				Console.WriteLine("\n\n------------------------------------------------------------------------------");
				Console.WriteLine("               Vas tiket byl uspesny, vyhral jste " + result.ToString("#,0.00") + " Kc");
				Console.WriteLine("------------------------------------------------------------------------------");
				Console.ForegroundColor = ConsoleColor.White;
			}
			else
			{
				balance -= bet;
				Bet_menu();
				Console.Write("\nZadejte castku kterou chcete vsadit: " + bet);
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("\n\n------------------------------------------------------------------------------");
				Console.WriteLine("               Vas tiket byl proherni, dekujeme za " + bet.ToString("#,0.00") + " Kc");
				Console.WriteLine("------------------------------------------------------------------------------");
				Console.ForegroundColor = ConsoleColor.White;
			}
			Console.Write("\nPro pokracovani do menu stisknete ");
			session = true;
			Yellow_enter();
			Save_bet();
			Check_matches();
			Reset();
			Main_menu();
		}
		
		//funkce pro vypis menu tiketu
		static void Ticket_menu()
		{
			string?	input;

			Console.WriteLine("------------------------------------------------------------------------------");
			Console.WriteLine("|                                                                            |");
			Console.Write("| Vsadit tiket ");
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.Write("[1]");
			Console.ForegroundColor = ConsoleColor.White;
			Console.Write("              Smazat tiket ");
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.Write("[2]");
			Console.ForegroundColor = ConsoleColor.White;
			Console.Write("             Hlavni menu ");
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.Write("[0]");
			Console.ForegroundColor = ConsoleColor.White;
			Console.WriteLine(" |");
			Console.WriteLine("|                                                                            |");
			Console.WriteLine("------------------------------------------------------------------------------");
			Console.Write("\nVyberte si prosim z nasledujicich moznosti: ");
			input = Console.ReadLine();
			switch (input)
			{
				case "1":
					if (balance == 0)
					{
						Console.ForegroundColor = ConsoleColor.Red;
						Console.WriteLine("\n------------------------------------------------------------------------------");
						Console.WriteLine("|             Nemate dostatecne prostredky pro vsazeni tiketu!               |");
						Console.WriteLine("------------------------------------------------------------------------------");
						Console.ForegroundColor = ConsoleColor.White;
						Console.Write("\nPro vklad penez stisknete ");
						Yellow_enter();
						Deposit_money();
						return;
					}
					Bet_menu();
					Console.Write("\nZadejte castku kterou chcete vsadit: ");
					Bet();
					break;
				case "2":
					Reset();
					Main_menu();
					break;
				case "0":
					Main_menu();
					break;
				default:
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine("\n------------------------------------------------------------------------------");
					Console.WriteLine("|                         Zadana moznost neexistuje!                         |"); 
					Console.WriteLine("------------------------------------------------------------------------------");
					Console.ForegroundColor = ConsoleColor.White;
					Console.Write("\nPro zopakovani moznosti stisknete ");
					Yellow_enter();
					Show_ticket();
					Ticket_menu();
					break;
			}
		}

		//funkce pro vypis hlavniho menu do konzole
		static void Main_menu()
		{
			Console.Clear();
			Console.WriteLine("\n------------------------------------------------------------------------------");
			Console.WriteLine("|                                                                            |");
			Console.WriteLine("|                                    MENU                                    |");
			Console.WriteLine("|                                                                            |");
			Console.WriteLine("------------------------------------------------------------------------------");
			Console.WriteLine("|                                                                            |");
			Console.Write("| Otevrit vyber zapasu                                                   ");
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.Write("[1]");
			Console.ForegroundColor = ConsoleColor.White;
			Console.WriteLine(" |");
			Console.WriteLine("|                                                                            |");
			Console.Write("| Vlozit penize na ucet                                                  ");
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.Write("[2]");
			Console.ForegroundColor = ConsoleColor.White;
			Console.WriteLine(" |");
			Console.WriteLine("|                                                                            |");
			Console.Write("| Zobrazit aktualni tiket                                                ");
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.Write("[3]");
			Console.ForegroundColor = ConsoleColor.White;
			Console.WriteLine(" |");
			Console.WriteLine("|                                                                            |");
			Console.Write("| Posledni vsazeny tiket                                                 ");
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.Write("[4]");
			Console.ForegroundColor = ConsoleColor.White;
			Console.WriteLine(" |");
			Console.WriteLine("|                                                                            |");
			Console.WriteLine("------------------------------------------------------------------------------");
			Console.WriteLine("|                                                                            |");
			Console.Write("| Vypnuti aplikace                                                       ");
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.Write("[0]");
			Console.ForegroundColor = ConsoleColor.White;
			Console.WriteLine(" |");
			Console.WriteLine("|                                                                            |");
			Console.WriteLine("------------------------------------------------------------------------------");
			if (rate != 1.00)
			{
				Console.Write("  Kurz tiketu:   ");
				Console.ForegroundColor = ConsoleColor.Cyan;
				Console.WriteLine(rate.ToString("#,0.00"));
				Console.ForegroundColor = ConsoleColor.White;
			}
			Console.Write("  Zustatek uctu: ");
			Console.ForegroundColor = ConsoleColor.Cyan;
			Console.WriteLine(balance.ToString("#,0.00") + " Kc");
			Console.ForegroundColor = ConsoleColor.White;
			Console.WriteLine("------------------------------------------------------------------------------\n");
			Console.Write("Vyberte si prosim moznost z menu: ");
			Input_main_menu();
		}

		//funkce ktera zajisti spravny vstup uzivatele z hlavniho menu za pomoci tryparse a switch a pote podle spravneho vstupu nasmeruje uzivatele dal
		static void Input_main_menu()
		{
			string?	input;

			input = Console.ReadLine();
			switch (input)
			{
				case "1":
					Select_match();
					break;
				case "2":
					Deposit_money();
					break;
				case "3":
					if (rate == 1.00)
					{
						Console.ForegroundColor = ConsoleColor.Red;
						Console.WriteLine("\n------------------------------------------------------------------------------");
						Console.WriteLine("|              Vas tiket neobsahuje zadne sazkove prilezitosti!              |");
						Console.WriteLine("------------------------------------------------------------------------------");
						Console.ForegroundColor = ConsoleColor.White;
						Console.Write("\nPro vyber zapasu a prilezitosti stisknete ");
						Yellow_enter();
						Select_match();
						break;
					}					
					Show_ticket();
					Ticket_menu();
					break;
				case "4":
					if (session == false)
					{
						Console.ForegroundColor = ConsoleColor.Red;
						Console.WriteLine("\n------------------------------------------------------------------------------");
						Console.WriteLine("|                      Zatim jste zadny tiket nevsadil!                      |");
						Console.WriteLine("------------------------------------------------------------------------------");
						Console.ForegroundColor = ConsoleColor.White;
						Console.Write("\nPro zopakovani vyberu stisknete ");
						Yellow_enter();
						Main_menu();
						break;
					}
					Load_last_ticket();
					Main_menu();
					break;
				case "0":
					Console.Clear();
					Console.Write("\nVypinani aplikace, vyckejte prosim.");
					Thread.Sleep(500);
					Console.Write(".");
					Thread.Sleep(500);
					Console.Write(".");
					Thread.Sleep(750);
					Console.Clear();
					Environment.Exit(0);
					return;
				default:
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine("\n------------------------------------------------------------------------------");
					Console.WriteLine("|                         Zadana moznost neexistuje!                         |"); 
					Console.WriteLine("------------------------------------------------------------------------------");
					Console.ForegroundColor = ConsoleColor.White;
					Console.Write("\nPro zopakovani moznosti stisknete ");
					Yellow_enter();
					Main_menu();
					break;
			}
		}

		/*MAIN PROGRAMU*/
		static void Main() 
		{
			result = 0.00;
			rate = 1.00;
			balance = 0.00;
			Console.ForegroundColor = ConsoleColor.White;
			Console.BackgroundColor = ConsoleColor.Black;
			Console.Clear();
			Console.WriteLine("\n------------------------------------------------------------------------------");
			Console.WriteLine("|                                                                            |");
			Console.WriteLine("|          Vitejte v aplikaci pro sazeni na Champions League 23/24!          |");
			Console.Write("|                       ");
			Console.ForegroundColor = ConsoleColor.Green;
			Console.Write("'VSADTE BARAK, VYHRAJTE DVA!'");
			Console.ForegroundColor = ConsoleColor.White;
			Console.WriteLine("                        |");
			Console.WriteLine("|                                                                            |");
			Console.WriteLine("------------------------------------------------------------------------------");
			Console.WriteLine("\n			                     ___");
			Console.WriteLine("			 o__        o__     |   |\\");
			Console.WriteLine("			/|          /\\      |   |X\\");
			Console.WriteLine("			/ > o        <\\     |   |XX\\");
			Console.WriteLine("------------------------------------------------------------------------------");
			Console.Write("\nPro pokracovani do menu stisknete ");
			Yellow_enter();
			Main_menu();
		}
	}
}