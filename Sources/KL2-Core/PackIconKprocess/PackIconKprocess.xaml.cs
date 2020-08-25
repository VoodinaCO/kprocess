﻿using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Kprocess.PackIconKprocess
{
    /// <summary>
    /// Logique d'interaction pour PackIconKprocess.xaml
    /// </summary>
    public partial class PackIconKprocess : UserControl
    {
        public static readonly DependencyProperty KindProperty = DependencyProperty.Register(nameof(Kind), typeof(PackIconKprocessKind), typeof(PackIconKprocess), new PropertyMetadata(PackIconKprocessKind.Gear));

        public PackIconKprocessKind Kind
        {
            get { return (PackIconKprocessKind)GetValue(KindProperty); }
            set { SetValue(KindProperty, value); }
        }

        public PackIconKprocess()
        {
            InitializeComponent();
        }

        public static IDictionary<PackIconKprocessKind, string> IconData = new Dictionary<PackIconKprocessKind, string>
        {
            [PackIconKprocessKind.Gear] = "M11.299979,24.800003C10.700004,24.800003 10.100029,25.100006 9.7000046,25.699997 9.10003,26.600006 9.4000168,27.800003 10.200005,28.300003 11.100028,28.900009 12.299978,28.600006 12.799978,27.800003 13.400014,26.900009 13.100026,25.699997 12.299978,25.199997 12.100027,24.900009 11.700003,24.800003 11.299979,24.800003z M11.200004,21.5L11.600028,23.100006 11.600028,23.199997C11.900015,23.199997,12.200003,23.300003,12.49999,23.400009L13.49999,22.100006 14.999989,23.100006 14.200001,24.5C14.400013,24.699997,14.600025,25,14.700001,25.199997L14.799976,25.199997 16.400012,25 16.7,26.699997 14.999989,27C14.999989,27.300003,14.900013,27.600006,14.799976,27.900009L16.100024,28.900009 15.100025,30.400009 13.700002,29.600006C13.49999,29.800003,13.200002,30,12.99999,30.100006L12.99999,30.199997 13.200002,31.800003 11.499991,32 10.999991,30.5C10.700004,30.5,10.400016,30.400009,10.100029,30.300003L9.10003,31.600006 7.6000309,30.600006 8.4000178,29.199997C8.2000056,29,7.9999933,28.699997,7.9000182,28.5L7.7999816,28.5 6.2000075,28.699997 5.9000197,27 7.4999943,26.600006 7.6000309,26.600006C7.6000309,26.300003,7.700006,26,7.7999816,25.699997L7.700006,25.699997 6.4000192,24.699997 7.4000187,23.199997 8.7999811,24C8.9999933,23.800003,9.2999802,23.600006,9.4999924,23.5L9.4999924,23.400009 9.2999802,21.800003z M24.199994,16.5C23.199994,16.5 22.29997,17.100006 21.900008,18 21.400008,19.199997 21.999984,20.699997 23.199994,21.199997 24.400006,21.699997 25.900004,21.100006 26.400004,19.900009 26.900004,18.699997 26.299968,17.199997 25.100017,16.699997 24.799968,16.600006 24.499982,16.5 24.199994,16.5z M22.79997,12.300003L23.699994,14.199997 23.699994,14.300003 24.799968,14.300003 24.799968,14.199997 25.799968,12.400009 27.900003,13.300003 27.19999,15.199997 27.19999,15.300003C27.499978,15.5,27.799966,15.800003,27.999978,16.100006L28.100015,16.100006 30.100013,15.400009 30.900001,17.5 29.100015,18.400009 28.999978,18.400009 28.999978,19.5 29.100015,19.5 30.900001,20.400009 29.999976,22.5 27.999978,21.900009 27.900003,21.900009C27.69999,22.199997,27.400003,22.5,27.100015,22.699997L27.100015,22.800003 27.799966,24.800003 25.699992,25.600006 24.799968,23.800003 24.799968,23.699997 23.699994,23.699997 23.699994,23.800003 22.79997,25.600006 20.699996,24.699997 21.299972,22.699997 21.299972,22.600006C20.999984,22.400009,20.699996,22.100006,20.499984,21.800003L20.400008,21.800003 18.40001,22.400009 17.600022,20.300003 19.499986,19.400009 19.600022,19.400009 19.600022,18.300003 19.499986,18.300003 17.699998,17.300003 18.600022,15.199997 20.499984,15.900009 20.60002,15.900009C20.799972,15.600006,21.10002,15.300003,21.400008,15.100006L21.400008,15 20.799972,13z M9.10003,5.6999969C7.2999821,5.6999969 5.799983,7.1999969 5.799983,9 5.799983,10.800003 7.2999821,12.300003 9.10003,12.300003 10.900016,12.300003 12.400015,10.800003 12.400015,9 12.400015,7.1999969 10.900016,5.6999969 9.10003,5.6999969z M7.6000309,0L10.700004,0 10.900016,2.8000031 10.900016,2.9000092C11.400016,3,11.900015,3.1999969,12.299978,3.5L12.400015,3.4000092 14.499989,1.5 16.7,3.6999969 14.900013,5.9000092 14.799976,6C15.100025,6.4000092,15.299976,6.9000092,15.400012,7.4000092L15.499988,7.4000092 18.299974,7.6000061 18.299974,10.699997 15.499988,10.900009 15.400012,10.900009C15.299976,11.400009,15.100025,11.900009,14.799976,12.300003L14.900013,12.400009 16.799975,14.5 14.600025,16.699997 12.400015,14.900009 12.299978,14.800003C11.900015,15.100006,11.400016,15.199997,10.900016,15.400009L10.900016,15.5 10.700004,18.300003 7.6000309,18.300003 7.4000187,15.5 7.4000187,15.400009C6.9000192,15.300003,6.4000192,15.100006,5.9999953,14.800003L5.9000197,14.900009 3.799985,16.699997 1.6000352,14.5 3.4999972,12.400009 3.6000338,12.300003C3.2999852,11.900009,3.2000096,11.400009,2.9999976,10.900009L2.7999859,10.900009 0,10.600006 0,7.5 2.7999859,7.3000031 2.9000221,7.3000031C2.9999976,6.8000031,3.2000096,6.3000031,3.4999972,5.9000092L3.4000221,5.9000092 1.6000352,3.6999969 3.799985,1.5 5.9000197,3.4000092 5.9999953,3.5C6.4000192,3.1999969,6.7999826,3,7.2999821,2.9000092L7.2999821,2.8000031z"
        };
    }
}
