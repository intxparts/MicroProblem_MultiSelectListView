using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace WpfApp3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new ViewModel();
        }
    }

    public class Person : INotifyPropertyChanged
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public List<Card> Cards { get; set; }

        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                NotifyPropertyChanged(nameof(IsSelected));
            }
        }


        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class Card : INotifyPropertyChanged
    {
        private BrushConverter _bc;
        public Card()
        {
            _bc = new BrushConverter();
        }

        public string HexColorValue { get; set; }
        public object ColorBrush { get { return _bc.ConvertFromString($"#{HexColorValue}"); } }

        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                NotifyPropertyChanged(nameof(IsSelected));
            }
        }

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }


    public class ViewModel
    {
        private readonly List<Person> _people;
        public List<Person> People => _people;

        private readonly DelegateCommand _personSelectionCommand;
        public DelegateCommand PersonSelectionCommand => _personSelectionCommand;

        private readonly DelegateCommand _cardSelectionCommand;
        public DelegateCommand CardSelectionCommand => _cardSelectionCommand;

        public ViewModel()
        {
            _personSelectionCommand = new DelegateCommand(OnPersonSelectionChanged);
            _cardSelectionCommand = new DelegateCommand(OnCardSelectionChanged);

            _people = new List<Person>()
            {
                new Person()
                {
                    Name = "John",
                    Cards = new List<Card>()
                    {
                        new Card() { HexColorValue = "0000FF" },
                        new Card() { HexColorValue = "FFFF00" },
                    }
                },
                new Person()
                {
                    Name = "Jacob",
                    Cards = new List<Card>()
                    {
                        new Card() { HexColorValue = "6600FF" },
                    }
                },
                new Person()
                {
                    Name = "JingleHeimerSchmidt",
                    Cards = new List<Card>()
                    {
                        new Card() { HexColorValue = "000000" },
                        new Card() { HexColorValue = "FF6600" },
                        new Card() { HexColorValue = "FFFFFF" },
                    }
                }
            };
        }

        private void OnCardSelectionChanged()
        {

        }

        private void OnPersonSelectionChanged()
        {

        }
    }

    public class DelegateCommand : ICommand
    {
        private Action _action;
        private Func<bool> _canExecuteCb;

        public DelegateCommand(Action action)
            : this(action, () => true)
        {
        }

        public DelegateCommand(Action action, Func<bool> canExecuteCB)
        {
            _action = action;
            _canExecuteCb = canExecuteCB;
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        public bool CanExecute(object parameter)
        {
            return _canExecuteCb.Invoke();
        }

        public void Execute(object parameter)
        {
            _action.Invoke();
        }

        public event EventHandler CanExecuteChanged;
    }
}
