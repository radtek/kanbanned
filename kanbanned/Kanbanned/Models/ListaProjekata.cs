using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanbanned.Models
{
    public class ListaProjekata : ObservableCollection<Projekat>
    {
        public ListaProjekata() : base()
        {
            this._title = "Projekti";
        }
        private String _title;

        public String PageTitle
        {
            get { return _title; }
            set { _title = value; }
        }
        

    }
}
