using Kanbanned.Helpers;
using Kanbanned.Packages;
using Kanbanned.UserControlsHelpers;
using Kanbanned.ViewModels;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using Kanbanned.Models;
using Oracle.ManagedDataAccess.Client;

namespace Kanbanned.Views
{
    /// <summary>
    /// Interaction logic for ApplicationView.xaml
    /// </summary>
    public partial class ApplicationView : MetroWindow
    {
        public ApplicationView(MainViewModel mvm)
        {
            InitializeComponent();
            
            //Packages.PKorisnik.Ucitaj_Podatke(ref k);
            //List<Models.Projekat> l = k.Projekti;

            ApplicationViewModel appVM = new ApplicationViewModel(mvm);
            appVM.AppView = this;
            this.DataContext = appVM;

            this.Show();

            this.ProjekatDugmiciManage(false, "");

            //test bgworker
            RadSaBazom.RefreshTabele.RunWorkerCompleted += RefreshTabele_RunWorkerCompleted;
        }

        /// <summary>
        /// Nije u skladu sa MVVM
        /// Alternativa je da se napravi DependencyProperty za Click komandu koji posle moze da se
        /// iskoristi kao property kontrole koja treba da salje komandu
        /// za sada neka stoji ovako
        /// U zavisnosti od toga na koji element u nav baru se klikne
        /// menja se trenutni ViewModel i prikazuje se njegov View 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListViewMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListView lv = sender as ListView;
            if (lv.SelectedIndex != -1)
            {
                ListViewItem li = lv.SelectedItem as ListViewItem;
                switch (li.Name)
                {
                    case "Home":
                        //osvezava se lista projekata
                        ((ApplicationViewModel)this.DataContext).CurrentPageViewModel = ((ApplicationViewModel)this.DataContext).PageViewModels[0];
                        ((StartViewModel)(((ApplicationViewModel)this.DataContext).CurrentPageViewModel)).OsveziListuProjekata();
                        ((ApplicationViewModel)this.DataContext).PostaviCurrentViewModel();
                        this.ProjekatDugmiciManage(false, "");
                        break;
                    case "Table":
                        ((ApplicationViewModel)this.DataContext).PostaviTrenutniProjekat();
                        // u zavisnosti da li se kreira ili je kreiran projekat treba drugacije da se prikazu dugmici
                        if(((ApplicationViewModel)this.DataContext).TrenutniProjekat != null)
                        {
                            if (((ApplicationViewModel)this.DataContext).TrenutniProjekat.Id == 0)
                            {
                                this.ProjekatDugmiciManage(true, "create");
                            }
                            else
                            {
                                this.ProjekatDugmiciManage(true, "open");
                            }                           
                        }                      
                        break;
                    case "Settings":
                        //za izmenu jezika
                        BiranjeJezika Jezik = new BiranjeJezika();
                        if (Jezik.ShowDialog() == true)
                        {
                            if (Jezik.Jezik.Equals("EN") && Globals.Jezik.Equals("RS"))
                            {
                                Globals.Jezik = "EN";
                                LoginDemo.App.SelectCulture("en");
                            }
                            else if (Jezik.Jezik.Equals("RS") && Globals.Jezik.Equals("EN"))
                            {
                                Globals.Jezik = "RS";
                                LoginDemo.App.SelectCulture("sr");
                            }
                        }
                        Jezik.Close();
                        break;
                    case "Account":
                        KorisnikDetalji kd = new KorisnikDetalji();
                        if (kd.ShowDialog() == true)
                        {                           
                            if (kd.DetaljiIliSifra)
                            {
								try
								{
									PKorisnik.PromeniDetalje(Korisnik.KorisnickoIme, kd.Ime, kd.Prezime, kd.Kompanija);
									PKorisnik.Ucitaj_Podatke();
								}
								catch (OracleException oraError)
								{
									try
									{
										System.Windows.MessageBox.Show(PPoruka.VratiPrevod(oraError.Number.ToString()));
									}
									catch (OracleException err)
									{
										System.Windows.MessageBox.Show("Greska");
									}
								}
							}
                            else
                            {
								try
								{
									PKorisnik.PromeniLozinku(Korisnik.KorisnickoIme, kd.NovaSifra.Password);
								}
								catch (OracleException oraError)
								{
									try
									{
										System.Windows.MessageBox.Show(PPoruka.VratiPrevod(oraError.Number.ToString()));
									}
									catch (OracleException err)
									{
										System.Windows.MessageBox.Show("Greska");
									}
								}
							}
                        }
                        kd.Close();
                        break;
                    case "Logout":
                        ((ApplicationViewModel)this.DataContext).AppView.Hide();
                        ((ApplicationViewModel)this.DataContext).mainViewModel.ShowAction();
                        ((ApplicationViewModel)this.DataContext).AppView.Close();
                        break;
                    default:
                        MessageBox.Show("" + li.Name);
                        break;
                }
                lv.SelectedIndex = -1;
            }
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //kada se ide na X da se zatvori prozor, ujedno se zatvori i MainView koji je skriven
            //ako je hide-ovan prozor onda znaci da se ide na logout
            if (this.Visibility == Visibility.Visible)
            {
                ((ApplicationViewModel)this.DataContext).mainViewModel.CloseAction();
            }

            //DBConnection.con.Close();         
        }

        /// <summary>
        /// Dinamicki postavlja tabelu kao trenutni content 
        /// </summary>
        /// <param name="control"></param>
        public void SetContent(Grid control)
        {
            ContentControl kontent = (ContentControl)this.FindChild<ContentControl>("contentCurrentVM");
            kontent.Content = control;
            kontent.UpdateLayout();
        }
        private static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
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

        //biranje jezika iz drop down menija
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //za izmenu jezika
            BiranjeJezika Jezik = new BiranjeJezika();
            if(Jezik.ShowDialog() == true)
            {
                if (Jezik.Jezik.Equals("EN") && Globals.Jezik.Equals("RS"))
                {
                    Globals.Jezik = "EN";
                    LoginDemo.App.SelectCulture("en");
                }
                else if (Jezik.Jezik.Equals("RS") && Globals.Jezik.Equals("EN"))
                {
                    Globals.Jezik = "RS";
                    LoginDemo.App.SelectCulture("sr");
                }
            }
            Jezik.Close();
        }

        //logout iz drop down menija
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ((ApplicationViewModel)this.DataContext).AppView.Hide();
            ((ApplicationViewModel)this.DataContext).mainViewModel.ShowAction();
            ((ApplicationViewModel)this.DataContext).AppView.Close();
        }

        /// <summary>
        /// Prikaz dugmica za refresh, save as template i show chart
        /// </summary>
        /// <param name="flag">Enable = 1, Disable = 0</param>
        /// <param name="type">"create", "open"</param>
        public void ProjekatDugmiciManage(bool flag, String type)
        {
            IEnumerable<Grid> grids = FindVisualChildren<Grid>(this);
            IEnumerable<StackPanel> sp = FindVisualChildren<StackPanel>(grids.ElementAt(0));
            StackPanel spanel = sp.First(panel => panel.Name == "ProjekatDugmici");
            //sakrij sve
            if (flag == false)
            {
                if (spanel != null) spanel.Visibility = Visibility.Hidden;
            }
            else
            {
                if(spanel != null)
                {
                    spanel.Visibility = Visibility.Visible;
                    IEnumerable<ListView> lv = FindVisualChildren<ListView>(spanel);
                    IEnumerable<ListViewItem> items = FindVisualChildren<ListViewItem>(lv.ElementAt(0));
                    ListViewItem refresh = items.First(x => x.Name == "RefreshTable");
                    ListViewItem save = items.First(x => x.Name == "SaveAsTemplate");
                    ListViewItem done = items.First(x => x.Name == "Done");
                    //ListViewItem notify = items.First(x => x.Name == "Notifications");
                    ListViewItem history = items.First(x => x.Name == "History");
                    if (type.Equals("create"))
                    {
                        refresh.Visibility = Visibility.Hidden;
                        //notify.Visibility = Visibility.Hidden;
                        save.Visibility = Visibility.Visible;
                        done.Visibility = Visibility.Visible;
                        history.Visibility = Visibility.Hidden;
                    }
                    else if (type.Equals("open"))
                    {
                        refresh.Visibility = Visibility.Visible;
                        //notify.Visibility = Visibility.Visible;
                        save.Visibility = Visibility.Visible;
                        done.Visibility = Visibility.Hidden;
                        history.Visibility = Visibility.Visible;
                    }
                }                
            }
        }
        //top bar list view
        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListView lv = sender as ListView;
            if (lv.SelectedIndex != -1)
            {
                ListViewItem li = lv.SelectedItem as ListViewItem;
                switch (li.Name)
                {
                    case "RefreshTable":
                        //background worker                                          
                        RadSaBazom.RefreshTabele.RunWorkerAsync();

                        //Projekat proj = ((ApplicationViewModel)this.DataContext).TrenutniProjekat;
                        ////prvo bi trebalo da pogleda logove i ako je bilo promena onda tek da uradi refresh
                        //proj.TabelaProjekta = new Tabela(proj.Ime, proj.Opis, proj.Id);
                        //Grid gr = TabelaCrtac.InvokeNacrtaj(((ApplicationViewModel)this.DataContext).TrenutniProjekat.TabelaProjekta, ((ApplicationViewModel)this.DataContext));
                        ////this.parentAppVM.AppView.ProjekatDugmiciManage(true, "create");
                        //((ApplicationViewModel)this.DataContext).AppView.SetContent(gr);
                        break;

                    case "Notifications":
                        //List<String> Obavestenja = ((ApplicationViewModel)this.DataContext).TrenutniProjekat.UcitajIstorijuIzmena(frm.Tip, int.Parse(frm.BrojPromena.ToString()));
                        //Notifications ipv = new Notifications();
                        //ipv.DataContext = Obavestenja;
                        //ipv.ShowDialog();
                        break;

					case "SaveAsTemplate":
                        Projekat temp = ((ApplicationViewModel)this.DataContext).TrenutniProjekat;
                        if (temp.Id != 0)
                        {
                            try
                            {
                                PProjekat.SacuvajKaoTemplejt(temp.Id, temp.Ime, temp.Opis);
								try
								{
									System.Windows.MessageBox.Show(PPoruka.VratiPrevod("TMP_S"));
								}
								catch (Oracle.ManagedDataAccess.Client.OracleException er)
								{
									System.Windows.MessageBox.Show("Templejt uspesno snimljen.");
								}
							}
                            catch (OracleException oraError)
                            {
								try
								{
									System.Windows.MessageBox.Show(PPoruka.VratiPrevod(oraError.Number.ToString()));
								}
								catch (OracleException err)
								{
									System.Windows.MessageBox.Show("Greska");
								}
							}
                            catch (System.Exception error)
                            {
                                System.Windows.MessageBox.Show("Greska");
                            }
                        }
                        else
                        {
							PProjekat.SacuvajKaoTemplejt(temp.Id, temp.Ime, temp.Opis);
							try
							{
								System.Windows.MessageBox.Show(PPoruka.VratiPrevod("MUST_S_P"));
							}
							catch (Oracle.ManagedDataAccess.Client.OracleException er)
							{
								System.Windows.MessageBox.Show("Morate snimiti projekat.");
							}
						}
                        break;

                    case "Done":
						try
						{
							RadSaBazom.KreirajProjekat(((ApplicationViewModel)this.DataContext).TrenutniProjekat);
							try
							{
								System.Windows.MessageBox.Show(PPoruka.VratiPrevod("P_SUC_C"));
							}
							catch (OracleException er)
							{
								System.Windows.MessageBox.Show("Projekat uspesno kreiran");
							}
						}
						catch (OracleException oraError)
						{
							System.Windows.MessageBox.Show(PPoruka.VratiPrevod(oraError.Number.ToString()));
						}
						
						break;

                    case "History":
                        IzborIstorijeIzmena frm = new IzborIstorijeIzmena();
                        if(frm.ShowDialog() == true)
                        {
                            // refreshuje se lista izmena
                            if(int.Parse(frm.BrojPromena.ToString()) != 0)
                            {
                                List<String> Izmene = ((ApplicationViewModel)this.DataContext).TrenutniProjekat.UcitajIstorijuIzmena(frm.Tip, int.Parse(frm.BrojPromena.ToString()));
                                IstorijaPromenaView ipv = new IstorijaPromenaView();
                                ipv.DataContext = Izmene;
                                ipv.ShowDialog();
                            }
                            else
                            {
								try
								{
									MessageBox.Show(PPoruka.VratiPrevod("WR_INPUT"));
								}
								catch (Exception er)
								{
									MessageBox.Show("Neispravan unos.");
								}
                            }
                        }
                       
						break;

					default:
                        MessageBox.Show("" + li.Name);
                        break;                 
                }
                lv.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Nacrta se ponovo tabela kad se izvrsi ucitavanje tabele iz baze
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RefreshTabele_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if((bool)e.Result == true)
            {
                //kada se ucita iz baze nova tabela
                Grid gr = TabelaCrtac.InvokeNacrtaj(((ApplicationViewModel)this.DataContext).TrenutniProjekat.TabelaProjekta, ((ApplicationViewModel)this.DataContext));
                ((ApplicationViewModel)this.DataContext).AppView.SetContent(gr);

				try
				{
					MessageBox.Show(PPoruka.VratiPrevod("TBL_REFRSH"));
				}
				catch (Exception er)
				{
					MessageBox.Show("Tabela je osvezena.");
				}
            }           
        }

        // profil iz drop down menija
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            KorisnikDetalji kd = new KorisnikDetalji();
            if (kd.ShowDialog() == true)
            {
                if (kd.DetaljiIliSifra)
                {
					try
					{
						PKorisnik.PromeniDetalje(Korisnik.KorisnickoIme, kd.Ime, kd.Prezime, kd.Kompanija);
						PKorisnik.Ucitaj_Podatke();
					}
					catch (OracleException oraError)
					{
						try
						{
							System.Windows.MessageBox.Show(PPoruka.VratiPrevod(oraError.Number.ToString()));
						}
						catch (OracleException err)
						{
							System.Windows.MessageBox.Show("Greska");
						}
					}
				}
                else
                {
					try
					{
						PKorisnik.PromeniLozinku(Korisnik.KorisnickoIme, kd.NovaSifra.Password);
					}
					catch (OracleException oraError)
					{
						try
						{
							System.Windows.MessageBox.Show(PPoruka.VratiPrevod(oraError.Number.ToString()));
						}
						catch (OracleException err)
						{
							System.Windows.MessageBox.Show("Greska");
						}
					}
                }
            }
            kd.Close();
        }
    }
}

