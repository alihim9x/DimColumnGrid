using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using SingleData;
using Utility;

namespace Model.Form
{
    /// <summary>
    /// Interaction logic for InputForm.xaml
    /// </summary>
    public partial class InputForm : Window
    {
        private static FormData formData
        {
            get
            {
                return FormData.Instance;
            }
        }
        public InputForm()
        {
            InitializeComponent();
        }

        private void SelectRevitLink_Click(object sender, RoutedEventArgs e)
        {
            InputFormUtil.SelectRevitLink();
        }

        private void DimPileCap_Click(object sender, RoutedEventArgs e)
        {
            var pileCapsRv = FormData.Instance.SelectedPileCaps.ToList();
            //var activeView = ModelData.Instance.ActiveView;
            //InputFormUtil.Run(activeView);
            InputFormUtil.RunPileCapDim(pileCapsRv);
            var form = FormData.Instance.InputForm;
            form.Close();
        }

        private void SelectPileCap_Click(object sender, RoutedEventArgs e)
        {
            InputFormUtil.SelectPileCap();
        }

        private void DimSpunPile_Click(object sender, RoutedEventArgs e)
        {
            var pileCapsRv = formData.SelectedPileCaps.ToList();
            InputFormUtil.RunSpunPileDim(pileCapsRv);
            var form = FormData.Instance.InputForm;
            form.Close();
        }
    }
}
