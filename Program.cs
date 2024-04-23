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
		static int 			selection;
		static double 		result, rate, balance, home_rate, away_rate, draw_rate, temp_rate, bet;
		static bool			correct_result = true; 
		static bool[]?		correct_match = new bool[File.ReadLines(@"zapasy.txt").Count()];
		static bool[]?		picked_match = new bool[File.ReadLines(@"zapasy.txt").Count()];	
		static char 		c;
		static string? 		input, line;
		static string[]? 	line_item, picked_rate;
		static string[][]?	data;

        //funkce pro resetovani vybranych zapasu a kurzu
		static void Reset()
		{
			rate = 1.00;
			correct_result = true;
		}

		//funkce pro zkontrolovani zbycvajicich zapasu, jestli uzivatel vsechny zvyuzil tak se aplikace vypne
		static void Check_matches()
		{
			int	i;

			i = 0;
			while (picked_match != null && i < picked_match.Length)
			{
				if (picked_match[i] == true)
					i++;
			}
			if (picked_match != null && i == picked_match.Length)
			{
				Console.WriteLine("\n------------------------------------------------------------------------------");
				Console.WriteLine("|                  Vycerpal jste vsechny dostupne zapasy!                    |");
				Console.WriteLine("------------------------------------------------------------------------------");
				Console.Write("  Konecny zustatek uctu: ");
				Console.ForegroundColor = ConsoleColor.Cyan;
				Console.WriteLine(balance.ToString("0.00") + " Kc");
				Console.ForegroundColor = ConsoleColor.White;
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
			}
			Console.Write("\nVypinani aplikace, vyckejte prosim.");
			Thread.Sleep(500);
			Console.Write(".");
			Thread.Sleep(500);
			Console.Write(".");
			Thread.Sleep(750);
			Console.Clear();
			Environment.Exit(0);
		}

		//funkce pro vypis zluteho enteru
		static void Yellow_enter()
		{
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.Write("ENTER");
			Console.ForegroundColor = ConsoleColor.White;
			Console.Write(".");
			Console.ReadLine();
			Console.Clear();
		}

		//funkce pro vypis erroru pri spatnem vstupu z funkce Select_rate ktera pak vypise 
		static void Error_select_rate(int line_num)
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine("\n------------------------------------------------------------------------------");
			Console.WriteLine("|                        Vybrany kurz neni v nabidce!                        |");
			Console.WriteLine("------------------------------------------------------------------------------");
			Console.ForegroundColor = ConsoleColor.White;
			Console.Write("\nPro zopakovani vyberu stisknete ");
			Yellow_enter();
			if (data != null)
			{
				Console.WriteLine("\n------------------------------------------------------------------------------");
				Console.WriteLine(" " + data[line_num][0] + " \t\t\t" + data[line_num][1] + " \t\t" + data[line_num][2]);
				Console.WriteLine("  " + data[line_num][6] + " \t\t\t " + data[line_num][7] + " \t\t\t  " + data[line_num][8]);
				Console.ForegroundColor = ConsoleColor.Yellow;
				Console.WriteLine("   [1]  \t\t\t  [x] \t\t\t   [2]");
				Console.ForegroundColor = ConsoleColor.White;
				Console.WriteLine("------------------------------------------------------------------------------");
				Console.Write("\nVyberte si prosim z nasledujicich prilezitosti: ");
			}
		}

		//funkce pro vyber kurzu zapasu ktery se prida k aktualnimu kurzu a zaroven zapamatovani volby uzivatele
		static double Select_rate(int line_num, double h, double d, double a)
		{
			int	i;

			i = 0;
			input = Console.ReadLine();
			while (!char.TryParse(input, out c))
			{
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
					return rate;
				}
				Error_select_rate(line_num);
				input = Console.ReadLine();
			}
			picked_rate = new string[3] {"0","0","0"};
			if (picked_match != null)
				picked_match[line_num] = true;
			switch (c)
			{
				case '1':
					if (picked_rate != null)
						picked_rate[0] = "1";
					temp_rate = h;
					return h;
				case 'x':
					if (picked_rate != null)
						picked_rate[1] = "1";
					temp_rate = d;
					return d;
				case '2':
					if (picked_rate != null)
						picked_rate[2] = "1";
					temp_rate = a;
					return a;
				default:
					Error_select_rate(line_num);
					Select_rate(line_num, h, d, a);
					return temp_rate;
			}
		} 

		//vypis erroru pri spatnem vstupu ze Select_match menu
		static void Error_select_match()
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine("\n------------------------------------------------------------------------------");
			Console.WriteLine("|                          Vybrany zapas neexistuje!                         |");
			Console.WriteLine("------------------------------------------------------------------------------");
			Console.ForegroundColor = ConsoleColor.White;
			Console.Write("\nPro zopakovani vyberu stisknete ");
			Yellow_enter();
		}

		//funkce pro vypis menu s vyberem zapasu ktere jsou nacteny ze souboru kde kazdej zapas ma vlastni index v poli data
		static void Select_match()
		{
			int	i;

			Console.Clear();
			Console.WriteLine("\n------------------------------------------------------------------------------");
			Console.WriteLine("|                                                                            |");
			Console.WriteLine("|                                VYBER ZAPASU                                |");
			Console.WriteLine("|                                                                            |");
			Console.WriteLine("------------------------------------------------------------------------------");
			Console.WriteLine("|                                                                            |");
			using (StreamReader sr = new StreamReader(@"zapasy.txt"))
			{
				i = 0;
				data = new string[File.ReadLines(@"zapasy.txt").Count()][];
				while (!sr.EndOfStream)
				{
					line = sr.ReadLine();
					if (line != null)
					{
						line_item = line.Split(';');
						data[i] = line_item;
						if (picked_match != null && picked_match[i] == true)
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
				Console.WriteLine(rate.ToString("0.00"));
				Console.ForegroundColor = ConsoleColor.White;
			}
			Console.WriteLine("------------------------------------------------------------------------------");
			Console.Write("\nVyberte si prosim z nasledujicich zapasu: ");
			Input_select_match();
		}

		//funkce ktera zajisti spravny vstup od uzivatele z funkce match_select (nemuze vybrat zapas ktery uz vybral, zapas ktery neexistuje)
		static void Input_select_match() 
		{
			input = Console.ReadLine();
			while (!int.TryParse(input, out selection))
			{
				Error_select_match();
				Select_match();
			}
			if (selection == 0)
			{
				Main_menu();
				return;
			}
			if (data != null && selection >= 1 && selection <= data.Length && picked_match != null && picked_match[selection - 1] == true)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("\n------------------------------------------------------------------------------");
				Console.WriteLine("|                       Tento zapas jste jiz vybral!                         |");
				Console.WriteLine("------------------------------------------------------------------------------");
				Console.ForegroundColor = ConsoleColor.White;
				Console.Write("\nPro zopakovani vyberu stisknete ");
				Yellow_enter();
				Select_match();
				return;
			}
			else if (data != null && selection >= 1 && selection <= data.Length)
			{
				selection--;
				home_rate = double.Parse(data[selection][6]);
				draw_rate = double.Parse(data[selection][7]);
				away_rate = double.Parse(data[selection][8]);
				Console.Clear();
				Console.WriteLine("\n------------------------------------------------------------------------------");
				Console.WriteLine(" " + data[selection][0] + " \t\t\t" + data[selection][1] + " \t\t" + data[selection][2]);
				Console.WriteLine("  " + data[selection][6] + " \t\t\t " + data[selection][7] + " \t\t\t  " + data[selection][8]);
				Console.ForegroundColor = ConsoleColor.Yellow;
				Console.WriteLine("   [1]  \t\t\t  [x] \t\t\t   [2]");
				Console.ForegroundColor = ConsoleColor.White;
				Console.WriteLine("------------------------------------------------------------------------------");
				Console.Write("\nVyberte si prosim z nasledujicich prilezitosti: ");
				temp_rate = Select_rate(selection, home_rate, draw_rate, away_rate );
				rate = Math.Round(rate * temp_rate, 2);
				if (picked_rate != null && correct_match != null && data[selection][3] == picked_rate[0] && data[selection][4] == picked_rate[1] && data[selection][5] == picked_rate[2])
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
			Console.WriteLine(balance.ToString("0.00") + " Kc");
			Console.ForegroundColor = ConsoleColor.White;
			Console.WriteLine("------------------------------------------------------------------------------");
		}
		
		//funkce pro vklad penez ktera zaroven zajistuje spravny vstup (minimalni vklad 100, castka musi mit maximalne dve desetinna cisla)
		//povoleny pocet pokusu je 5 a pri spravnem vstupu se uzivatelovi prictou penize a je poslan zpatky do main menu
		static void Deposit_money()
		{
			int	i;
			
			i = 0;
			Deposit_money_menu();
			Console.Write("\nZadejte castku kterou chcete vlozit: ");
			input = Console.ReadLine();
			while (!double.TryParse(input, out result) || result < 100 || !(result == Math.Round(result, 2)) || result > 100000 || balance + result > 1000000)
			{
				Deposit_money_menu();
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
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine("\n------------------------------------------------------------------------------");
			Console.WriteLine("                      Vklad "+ result.ToString("0.00") +" Kc probehl uspesne!                      ");
			Console.WriteLine("------------------------------------------------------------------------------");
			Console.ForegroundColor = ConsoleColor.White;
			Console.Write("\nPro pokracovani zpatky do menu stisknete ");
			Yellow_enter();
			Main_menu();
		}

		//funkce pro vypis menu ve funkci bet
		static void Bet_menu()
		{
			Console.Clear();
			Console.WriteLine("\n------------------------------------------------------------------------------");
			Console.WriteLine("|                                                                            |");
			Console.WriteLine("|                               VSAZENI TIKETU                               |");
			Console.WriteLine("|                                                                            |");
			Console.WriteLine("------------------------------------------------------------------------------");
			if (rate != 1.00)
			{
				Console.Write("  Kurz tiketu:   ");
				Console.ForegroundColor = ConsoleColor.Cyan;
				Console.WriteLine(rate.ToString("0.00"));
				Console.ForegroundColor = ConsoleColor.White;
			}
			Console.Write("  Zustatek uctu: ");
			Console.ForegroundColor = ConsoleColor.Cyan;
			Console.WriteLine(balance.ToString("0.00") + " Kc");
			Console.ForegroundColor = ConsoleColor.White;
			Console.WriteLine("------------------------------------------------------------------------------");
		}
		
		//funkce pro vsazeni tiketu, osetreni vstupu, nasledne vypise vyledek vyherni/proherni
		static void Bet()
		{
			int	i;

			i = 0;
			input = Console.ReadLine();
			while (!double.TryParse(input, out bet) || bet > balance || bet < 10.00)
			{
				Bet_menu();
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
					Console.WriteLine("|         Zadana castka je mensi nez minimalni hodnota sazky 10 Kc!         |");
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
				balance += result;
				Bet_menu();
				Console.ForegroundColor = ConsoleColor.Green;
				Console.WriteLine("\n------------------------------------------------------------------------------");
				Console.WriteLine("               Vas tiket byl uspesny, vyhral jste " + result.ToString("0.00") + " Kc");
				Console.WriteLine("------------------------------------------------------------------------------");
				Console.ForegroundColor = ConsoleColor.White;
			}
			else
			{
				balance -= bet;
				Bet_menu();
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("\n------------------------------------------------------------------------------");
				Console.WriteLine("               Vas tiket byl proherni, dekujeme za " + bet.ToString("0.00") + " Kc");
				Console.WriteLine("------------------------------------------------------------------------------");
				Console.ForegroundColor = ConsoleColor.White;
			}
			Console.Write("\nPro pokracovani do menu stisknete ");
			Yellow_enter();
			Check_matches();
			Reset();
			Main_menu();
		}
		
		//funkce pro vypis erroru v main menu
		static void Error_main_menu()
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine("\n------------------------------------------------------------------------------");
			Console.WriteLine("|                         Zadana moznost neexistuje!                         |"); 
			Console.WriteLine("------------------------------------------------------------------------------");
			Console.ForegroundColor = ConsoleColor.White;
			Console.Write("\nPro zopakovani moznosti stisknete ");
			Yellow_enter();
			Main_menu();
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
			Console.Write("| Vsadit tiket                                                           ");
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.Write("[3]");
			Console.ForegroundColor = ConsoleColor.White;
			Console.WriteLine(" |");
			Console.WriteLine("|                                                                            |");
			Console.Write("| Smazat tiket                                                           ");
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.Write("[4]");
			Console.ForegroundColor = ConsoleColor.White;
			Console.WriteLine(" |");
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
				Console.WriteLine(rate.ToString("0.00"));
				Console.ForegroundColor = ConsoleColor.White;
			}
			Console.Write("  Zustatek uctu: ");
			Console.ForegroundColor = ConsoleColor.Cyan;
			Console.WriteLine(balance.ToString("0.00") + " Kc");
			Console.ForegroundColor = ConsoleColor.White;
			Console.WriteLine("------------------------------------------------------------------------------\n");
			Console.Write("Vyberte si prosim moznost z menu: ");
			Input_main_menu();
		}

		//funkce ktera zajisti spravny vstup uzivatele z hlavniho menu za pomoci tryparse a switch a pote podle spravneho vstupu nasmeruje uzivatele dal
		static void Input_main_menu()
		{
			input = Console.ReadLine();
			if (!int.TryParse(input, out selection))
			{
				Error_main_menu();
				return;
			}
			switch (selection)
			{
				case 1:
					Select_match();
					break;
				case 2:
					Deposit_money();
					break;
				case 3:
					Bet_menu();
					if (rate == 1.00)
					{
						Console.ForegroundColor = ConsoleColor.Red;
						Console.WriteLine("\n\n------------------------------------------------------------------------------");
						Console.WriteLine("|              Vas tiket neobsahuje zadne sazkove prilezitosti!              |");
						Console.WriteLine("------------------------------------------------------------------------------");
						Console.ForegroundColor = ConsoleColor.White;
						Console.Write("\nPro vyber prilezitosti stisknete ");
						Yellow_enter();
						Select_match();
						return;
					}
					if (balance == 0)
					{
						Console.ForegroundColor = ConsoleColor.Red;
						Console.WriteLine("\n------------------------------------------------------------------------------");
						Console.WriteLine("|      Nemate dostatece prostredky pro vsazeni tiketu, minimalne 100 Kc      |");
						Console.WriteLine("------------------------------------------------------------------------------");
						Console.ForegroundColor = ConsoleColor.White;
						Console.Write("\nPro vklad penez stisknete ");
						Yellow_enter();
						Deposit_money();
						return;
					}
					Console.Write("\nZadejte castku kterou chcete vsadit: ");
					Bet();
					break;
				case 0:
					Console.Clear();
					Console.Write("\nVypinani aplikace, vyckejte prosim.");
					Thread.Sleep(500);
					Console.Write(".");
					Thread.Sleep(500);
					Console.Write(".");
					Thread.Sleep(750);
					Console.Clear();
					return;
				default:
					Error_main_menu();
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