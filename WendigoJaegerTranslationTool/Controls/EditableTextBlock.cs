using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace WendigoJaeger.TranslationTool.Controls
{
    public class EditableTextBlockAdorner : Adorner
    {
        private readonly VisualCollection _collection;

        private readonly TextBox _textBox = new TextBox();
        private readonly TextBlock _textBlock;

        public event RoutedEventHandler TextBoxLostFocus
        {
            add
            {
                _textBox.LostFocus += value;
            }
            remove
            {
                _textBox.LostFocus -= value;
            }
        }

        public event KeyEventHandler TextBoxKeyUp
        {
            add
            {
                _textBox.KeyUp += value;
            }
            remove
            {
                _textBox.KeyUp -= value;
            }
        }

        public EditableTextBlockAdorner(EditableTextBlock adornedElement)
            : base(adornedElement)
        {
            _collection = new VisualCollection(this);

            _textBlock = adornedElement;

            Binding textBinding = new Binding("Text")
            {
                Source = adornedElement
            };

            _textBox.SetBinding(TextBox.TextProperty, textBinding);
            _textBox.AcceptsReturn = true;
            _textBox.KeyUp += onTextBoxKeyUp;

            _collection.Add(_textBox);
        }

        public void SelectAll()
        {
            _textBox.SelectAll();
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            _textBox.Arrange(new Rect(0, 0, _textBox.DesiredSize.Width * 1.2, _textBlock.DesiredSize.Height * 1.3));
            _textBox.Focus();
            return finalSize;
        }

        protected override Visual GetVisualChild(int index)
        {
            return _collection[index];
        }

        protected override int VisualChildrenCount => _collection.Count;

        private void onTextBoxKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                _textBox.Text = _textBox.Text.Replace("\r\n", string.Empty);

                BindingExpression textExpression = _textBox.GetBindingExpression(TextBox.TextProperty);
                if (textExpression != null)
                {
                    textExpression.UpdateSource();
                }
            }
        }
    }

    public class EditableTextBlock : TextBlock
    {
        private EditableTextBlockAdorner _adorner;

        private static readonly DependencyProperty IsEditingProperty = DependencyProperty.Register(nameof(IsEditing), typeof(bool), typeof(EditableTextBlock), new UIPropertyMetadata(false, onIsEditingChanged));
        public bool IsEditing
        {
            get
            {
                return (bool)GetValue(IsEditingProperty);
            }
            set
            {
                SetValue(IsEditingProperty, value);
            }
        }

        public void OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F2)
            {
                IsEditing = true;
            }
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 2)
            {
                IsEditing = true;
            }
        }

        private void onTextBoxKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                IsEditing = false;
            }
        }

        private void onTextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            IsEditing = false;
        }

        private static void onIsEditingChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            EditableTextBlock textBlock = dependencyObject as EditableTextBlock;
            if (textBlock != null)
            {
                AdornerLayer layer = AdornerLayer.GetAdornerLayer(textBlock);

                if (textBlock.IsEditing)
                {
                    if (textBlock._adorner == null)
                    {
                        textBlock._adorner = new EditableTextBlockAdorner(textBlock);

                        textBlock._adorner.TextBoxKeyUp += textBlock.onTextBoxKeyUp;
                        textBlock._adorner.TextBoxLostFocus += textBlock.onTextBoxLostFocus;
                    }

                    textBlock._adorner.SelectAll();

                    layer.Add(textBlock._adorner);
                }
                else
                {
                    Adorner[] adorners = layer.GetAdorners(textBlock);
                    if (adorners != null)
                    {
                        var editableAdorner = adorners.FirstOrDefault(x => x is EditableTextBlockAdorner);
                        if (editableAdorner != null)
                        {
                            layer.Remove(editableAdorner);
                        }
                    }

                    BindingExpression textExpression = textBlock.GetBindingExpression(TextProperty);
                    if (textExpression != null)
                    {
                        textExpression.UpdateTarget();
                    }
                }
            }
        }
    }
}
