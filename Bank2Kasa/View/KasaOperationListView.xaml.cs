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

namespace Bank2Kasa.View
{
    /// <summary>
    /// Interaction logic for KasaOperationListView.xaml
    /// </summary>
    public partial class KasaOperationListView : UserControl
    {
        public KasaOperationListView()
        {
            InitializeComponent();
        }


        private void dgrMain_TargetUpdated(object sender, DataTransferEventArgs e)
        {
            // we want to scroll the datagrid rows to the first which is from selected month
            if (cmbMonth.SelectedIndex > 0) // if 0 = all months, no need to scroll 
            {
                foreach (var item in dgrMain.Items)
                    if (((ViewModel.OperationVM)item).Date.Month == cmbMonth.SelectedIndex)
                    {
                        dgrMain.ScrollIntoView(item);
                        return;
                    }
            }
        }
    }
}
