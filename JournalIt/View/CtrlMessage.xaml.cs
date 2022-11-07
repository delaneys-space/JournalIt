using System.Windows;
using System.Windows.Controls;

namespace JournalIt.View
{
    /// <summary>
    /// Interaction logic for CtrlMessage.xaml
    /// </summary>
    public partial class CtrlMessage : UserControl
    {
        public CtrlMessage()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text",
                                                                                         typeof(string),
                                                                                         typeof(CtrlMessage),
                                                                                         new UIPropertyMetadata("Text"));

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

    }
}
