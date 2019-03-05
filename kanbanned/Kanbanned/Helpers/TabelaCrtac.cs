using Kanbanned.Helpers;
using Kanbanned.Models;
using Kanbanned.UserControlsHelpers;
using Kanbanned.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

using System.Windows.Media;

namespace Kanbanned
{
    public class TabelaCrtac
    {
        //mora da se prodje kroz celo stablo i da se izracuna max broj zadataka u deci i na osnovu toga odredi velicina svake kolone
        public static void Nacrtaj(Kontejner kolona, IKontejnerGrid parentGrid, ApplicationViewModel vm)
        {
            bool isKontejnerFaza = false;
            IKontejnerGrid kolonaGrid;
            Border bor1;
            Border bor2;

            if(kolona.GetType() == typeof(KontejnerFaza))
            {
                kolonaGrid = new KontejnerGrid(vm);
                bor1 = (Border)((Grid)kolonaGrid).Children[1];
            }
            else
            {
                kolonaGrid = new KontejnerZadGrid(vm);
                bor1 = (Border)((KontejnerZadGrid)kolonaGrid).Children[1];
            }

            if (parentGrid.GetType() == typeof(KontejnerGrid))
            {
                bor2 = (Border)((KontejnerGrid)parentGrid).Children[1];
                isKontejnerFaza = true;
            }
            else
            {
                bor2 = (Border)((KontejnerZadGrid)parentGrid).Children[1];
                isKontejnerFaza = false;
            }
            //stavlja se pokazivac na model podataka kolonu koja stoji iza kontrole
            ((Grid)kolonaGrid).Tag = kolona;
            //vvranic - treba da se testira da li ovo radi
            ((Grid)kolonaGrid).DataContext = vm.TrenutniProjekat;

            Grid kolonaContentGrid = (Grid)bor1.Child;
            Grid parentContentGrid = (Grid)bor2.Child;

            ((Grid)kolonaGrid).HorizontalAlignment = System.Windows.HorizontalAlignment.Center;

            IEnumerable<Label> tekst = FindVisualChildren<Label>((Grid)kolonaGrid);
            tekst.ElementAt(0).Content = kolona.Ime;

            int index = 0;
            //trazi index u listi dece na kom se nalazi trenutna kolona
            if (kolona.Roditelj != null)
            {
				// 10.06.2018. mstankovic
				//index = ((KontejnerFaza)kolona.Roditelj).Deca.IndexOf(kolona);
				index = (kolona.Roditelj).Deca.IndexOf(kolona);

				if (kolona.Roditelj.IsVerticalSplit)
                {
                    parentContentGrid.Children.Add((Grid)kolonaGrid);
                    ((Grid)kolonaGrid).SetValue(Grid.ColumnProperty, index);
                }
                else //deli se horizontalno
                {
                    parentContentGrid.Children.Add((Grid)kolonaGrid);
                    ((Grid)kolonaGrid).SetValue(Grid.RowProperty, index);
                }

            }

            //najpre se ispita da li trenutna kolona sadrzi kolone ili zadatke
            //jer se crtaju razlicito
            if (kolona.GetType() == typeof(KontejnerFaza))
            {
                //ako se deli vertikalno dodaju se definicije za kolone u zavisnosti od toga koliko ima dece
                if (((KontejnerFaza)kolona).IsVerticalSplit == true)
                {
                    for (int i = 0; i < ((KontejnerFaza)kolona).Deca.Count; i++)
                    {
                        ColumnDefinition c = new ColumnDefinition();
                        c.Width = System.Windows.GridLength.Auto;
                        kolonaContentGrid.ColumnDefinitions.Add(c);
                    }
                }
                else
                {
                    //ako se deli vertikalno dodaju se definicije za vrste u zavisnosti od toga koliko ima dece
                    for (int i = 0; i < ((KontejnerFaza)kolona).Deca.Count; i++)
                    {
                        RowDefinition r = new RowDefinition();
                        r.Height = System.Windows.GridLength.Auto;
                        kolonaContentGrid.RowDefinitions.Add(r);
                    }
                }

                foreach (Kontejner d in ((KontejnerFaza)kolona).Deca)
                {
                    //za svaku podfazu odradi ovo 
                    Nacrtaj(d, kolonaGrid, vm);
                }
            }
            else //ako je KontejnerZadataka
            {
               
                    // dodavanje Column definicija
                    for (int i = 0; i < ((KontejnerZadataka)kolona).SirinaPoZadacima; i++)
                    {
                        ColumnDefinition c = new ColumnDefinition();
                        c.Width = GridLength.Auto;
                        kolonaContentGrid.ColumnDefinitions.Add(c);
                    }

                    // dodavanje Row definicija
                    for (int i = 0; i < ((KontejnerZadataka)kolona).VisinaPoZadacima; i++)
                    {
                        RowDefinition r = new RowDefinition();
                        r.Height = GridLength.Auto;
                        kolonaContentGrid.RowDefinitions.Add(r);
                    }

                    // smestanje zadataka u matricu (tj. u grid)
                    for (int i = 0; i < ((KontejnerZadataka)kolona).Zadaci.Count; i++)
                    {
                        ZadatakGrid zad = new ZadatakGrid(vm);
                    
                        //stavlja se data context kontroli
                        zad.DataContext = ((KontejnerZadataka)kolona).Zadaci[i];
                        
                        kolonaContentGrid.Children.Add(zad);
                        zad.SetValue(Grid.RowProperty, ((KontejnerZadataka)kolona).Zadaci[i].Row);
                        zad.SetValue(Grid.ColumnProperty, ((KontejnerZadataka)kolona).Zadaci[i].Column);
                    }
            }        
        }

        public static Grid InvokeNacrtaj(Tabela tabela, ApplicationViewModel vm)
        {
            // objekat za XAML Grid koji sadrzi zaglavlje i sadrzaj
            KontejnerGrid tabelaGrid = new KontejnerGrid(vm);
            tabelaGrid.Tag = tabela.RootKolona;

            //vvranic - i ovde isto kao i gore
            tabelaGrid.DataContext = vm.TrenutniProjekat;

            Border bord = (Border)tabelaGrid.Children[1];
            Grid tabelaContentGrid = (Grid)bord.Child;

            tabelaGrid.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;   // float left

            //ubacen border pa mora ovako da se izvuce
            Border border = ((Border)((Grid)tabelaGrid.Children[0]).Children[0]);
            ((Label)((Grid)border.Child).Children[0]).Content = tabela.Naslov;

			// 10.06.2018. mstankovic
			//for (int i = 0; i < ((KontejnerFaza)tabela.RootKolona).Deca.Count; i++)
			for (int i = 0; i < (tabela.RootKolona).Deca.Count; i++)
			{
                ColumnDefinition c = new ColumnDefinition();
                c.Width = System.Windows.GridLength.Auto;
                tabelaContentGrid.ColumnDefinitions.Add(c);
            }
			// 10.06.2018. mstankovic
			//foreach (Kontejner k in ((KontejnerFaza)tabela.RootKolona).Deca)
			foreach (Kontejner k in (tabela.RootKolona).Deca)
			{
                Nacrtaj(k, tabelaGrid, vm);
            }
            return tabelaGrid;
        }

		public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
		{
			if (depObj != null)
			{
				for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
				{
					DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
					if (child != null && child is T)
					{
						yield return (T)child;
					}

					foreach (T childOfChild in FindVisualChildren<T>(child))
					{
						yield return childOfChild;
					}
				}
			}
		}

		// 10.06.2018. mstankovic (privremeno uklonjena funkcija nakon uklanjanja propertija IsVerticalSplit iz klase Kontejner)
		/*
        public int NadjiSirinu(Kontejner kontejner)
        {
            //nakon izracunavanja treba da se prodje kroz listu i svoj deci stavi dobijena sirina

            int sirina = 0;
            foreach(Kontejner k in ((KontejnerFaza)kontejner).Deca)
            {
                //ako se horizontalno splituju
                if(!k.IsVerticalSplit)
                {
                    //ako je kontejner zadataka samo se ispita da li je on max
                    if (k.GetType() == typeof(KontejnerZadataka))
                    {
                        sirina += k.SirinaPoZadacima;
                    }
                    //ako je kontejner faza onda mora rekurzivno da se odradi dok se ne ispitaju max
                    else
                    {
                        int sirinaKF = NadjiSirinu(k);
                        if(sirinaKF > sirina)
                        {
                            sirina = sirinaKF;
                        }
                    }
                }         
            }
            return sirina;
        }
		*/

		// 10.06.2018. mstankovic (privremeno uklonjena funkcija nakon uklanjanja propertija IsVerticalSplit iz klase Kontejner)
		/*
        public int NadjiVisinu(Kontejner kontejner)
        {
            int visina = 0;
            foreach (Kontejner k in ((KontejnerFaza)kontejner).Deca)
            {
                //ako se horizontalno splituju
                if (k.IsVerticalSplit)
                {
                    //ako je kontejner zadataka samo se ispita da li je on max
                    if (k.GetType() == typeof(KontejnerZadataka))
                    {
                        if(visina < k.VisinaPoZadacima)
                        {
                            visina = k.VisinaPoZadacima;
                        }
                    }
                    //ako je kontejner faza onda mora rekurzivno da se odradi dok se ne ispitaju max
                    else
                    {
                        int visinaKF = NadjiVisinu(k);
                        if (visinaKF > visina)
                        {
                            visina = visinaKF;
                        }
                    }
                }
            }
            return visina;
        }
      */

	}
}
