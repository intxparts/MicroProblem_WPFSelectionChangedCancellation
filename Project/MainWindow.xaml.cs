using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace Project
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var viewModel = new MainWindowViewModel();
            this.DataContext = viewModel;
            this.MainListView.SelectionChanged += viewModel.OnSelectionChanged;
            this.MainListView.PreviewMouseDown += viewModel.OnPreviewMouseDown;
        }
    }

    public class Person
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }

    public sealed class MainWindowViewModel : INotifyPropertyChanged
    {
        private readonly List<Person> _people;

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        internal void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                foreach (var a in e.AddedItems)
                {
                    var person = a as Person;
                    if (person.Name == "Tim")
                    {
                        // cancel
                        e.Handled = true;
                        return;
                    }
                }
            }

        }

        internal void OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var myItem = (e.OriginalSource as FrameworkElement).DataContext as Person;

            if (myItem.Name == "Tim")
            {
                MessageBoxResult result = MessageBox.Show("Cannot select Tim", "", MessageBoxButton.OK, MessageBoxImage.Information);

                if (result == MessageBoxResult.OK)
                {
                    e.Handled = true;
                    return;
                }
                else
                {
                    //otherwise, reinvoke the click event
                    (e.Source as FrameworkElement).Dispatcher.BeginInvoke(
                       new Action(() =>
                       {
                           RoutedEventArgs args = new MouseButtonEventArgs(e.MouseDevice, 0, e.ChangedButton);
                           args.RoutedEvent = UIElement.MouseDownEvent;
                           (e.OriginalSource as UIElement).RaiseEvent(args);
                       }),
                       System.Windows.Threading.DispatcherPriority.Input);
                }
            }
        }

        public List<Person> People => _people;

        private Person _selectedPerson;
        public Person SelectedPerson
        {
            get { return _selectedPerson; }
            set
            {
                _selectedPerson = value;
                NotifyPropertyChanged(nameof(SelectedPerson));
            }
        }

        public MainWindowViewModel()
        {
            _people = new List<Person>()
            {
                new Person() { Name = "Rob", Age = 31 },
                new Person() { Name = "Sergei", Age = 52 },
                new Person() { Name = "Tim", Age = 19 }
            };
            _selectedPerson = _people.First();
        }


    }
}
