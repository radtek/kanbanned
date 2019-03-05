using Kanbanned.Models;
using Kanbanned.Packages;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanbanned.Helpers
{
    public class RefreshBackgroundWorker : BackgroundWorker
    {
        public Projekat TrProjekat { get; set; }
        public RefreshBackgroundWorker()
        {
            this.DoWork += RefreshBackgroundWorker_DoWork;
        }

        private void RefreshBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if(TrProjekat != null)
            {
                TrProjekat.TabelaProjekta = new Tabela(TrProjekat.Ime, TrProjekat.Opis, TrProjekat.Id);
                e.Result = true;
            }           
            else
            {
                e.Result = false;
            }
        }
    }
}
