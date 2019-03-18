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
using WendigoJaeger.TranslationTool.Data;
using WendigoJaeger.TranslationTool.Undo;

namespace WendigoJaeger.TranslationTool.Creators
{
    [ObjectCreator(typeof(RefObject))]
    public partial class GenericCreateWindow : Window, IObjectCreator
    {
        private Type _objectType;

        public object CreatedObject { get; set; }

        public Type ObjectType
        {
            get
            {
                return _objectType;
            }
            set
            {
                _objectType = value;

                Title = string.Format(Resource.windowGenericCreate, value.Name);
            }
        }

        public ProjectSettings ProjectSettings { get; set; }

        public GenericCreateWindow()
        {
            InitializeComponent();
        }

        private void buttonOK_Click(object sender, RoutedEventArgs e)
        {
            RefObject newRefObject = (RefObject)Activator.CreateInstance(ObjectType);
            newRefObject.Name = textName.Text;

            CreatedObject = newRefObject;

            DialogResult = true;
            Close();
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void textName_TextChanged(object sender, TextChangedEventArgs e)
        {
            updateButtonOKEnabledState();
        }

        private void updateButtonOKEnabledState()
        {
            buttonOK.IsEnabled = !string.IsNullOrEmpty(textName.Text);
        }

        private void textName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return && !string.IsNullOrEmpty(textName.Text))
            {
                buttonOK_Click(null, null);
            }
        }
    }
}
