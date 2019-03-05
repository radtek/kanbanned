using Kanbanned.Models;
using Kanbanned.Packages;
using Kanbanned.ViewModels;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Kanbanned.UserControlsHelpers
{
    /// <summary>
    /// Interaction logic for Zadatak.xaml
    /// </summary>
    public partial class ZadatakGrid : Grid
    {
        
        private Point startPoint;
        private ApplicationViewModel vm;
        public ZadatakGrid(ApplicationViewModel vm)
        {
            InitializeComponent();
            this.vm = vm;
        }

        private void Grid_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            startPoint = e.GetPosition(null);
        }

        private void Grid_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            // Get the current mouse position
            Point mousePos = e.GetPosition(null);
            Vector diff = startPoint - mousePos;

            if (e.LeftButton == MouseButtonState.Pressed && (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                                                            Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))
            {
                // Get the dragged ListViewItem
                ZadatakGrid zad = sender as ZadatakGrid;

                Zadatak z = (Zadatak)zad.DataContext;

                //zakljucavanje zadatka u bazi
                //ukoliko zadatak nije ubacen onda
                //ne treba da se ispita ali onda mora da se ispita
                if (z.Id != 0)
                {
                    if (!Packages.PZadatak.IsZakljucan(z.Id))
                    {
                        Packages.PZadatak.Zakljucaj(z.Id);

                        // Initialize the drag & drop operation
                        DataObject dragData = new DataObject("myZad", z);
                        DragDrop.DoDragDrop(zad, dragData, DragDropEffects.Move);
                    }
                    else
                    {
                        MessageBox.Show(PPoruka.VratiPrevod("Z_LOCKED"));
                    }
                }
                else
                {
                    //ako necemo da zakljucava ovo ostavimo a ovo gore sve iskomentarisemo
                    //Initialize the drag & drop operation
                    DataObject dragData = new DataObject("myZad", z);
                    DragDrop.DoDragDrop(zad, dragData, DragDropEffects.Move);
                }
            }
        }
        // Helper to search up the VisualTree
        private static T FindAnchestor<T>(DependencyObject current)
            where T : DependencyObject
        {
            do
            {
                if (current is T)
                {
                    return (T)current;
                }
                current = VisualTreeHelper.GetParent(current);
            }
            while (current != null);
            return null;
        }

        //otvaranje detalja o zadatku
        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //dupli click za otvaranje detalja o zadatku
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 2)
            {
                ZadatakGrid zad = sender as ZadatakGrid;

                Zadatak z = (Zadatak)zad.DataContext;

                //prikazuju se detalji
                DetaljiZadatak dz = new DetaljiZadatak(z, this.vm.TrenutniProjekat);
            }
        }
    }
}
