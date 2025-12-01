using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace LandSecure
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LandSearchPage : Page
    {
        public LandSearchModel Filters { get; set; } = new LandSearchModel();

        public List<LandProperty> AllProperties { get; set; }
        public List<LandProperty> FilteredProperties { get; set; }

        public LandSearchPage()
        {
            this.InitializeComponent();
        }

        public class SearchModel
        {
            public string Location { get; set; }
            public string Size { get; set; }

            public int Price {get; set;}
        }

       
    }
}
