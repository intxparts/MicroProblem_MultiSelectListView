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

        private readonly DelegateCommand _controlLoadedCommand;
        public DelegateCommand ControlLoadedCommand => _controlLoadedCommand;

        private readonly DelegateCommand _selectionCommand;
        public DelegateCommand SelectionCommand => _selectionCommand;

        private List<Person> _selectedPeopleCache;
        private List<Card> _selectedCardCache;
        private Dictionary<Card, Person> _cardPeopleTable;
        private int _maxNumSelections = 2;

        public ViewModel()
        {
            _selectionCommand = new DelegateCommand(OnSelectionChanged);
            _controlLoadedCommand = new DelegateCommand(OnControlLoaded);
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

        private bool _isLoaded = false;
        private void OnControlLoaded()
        {
            if (_isLoaded)
                return;

            _people[0].IsSelected = true;
            _people[0].Cards[0].IsSelected = true;

            _selectedCardCache = new List<Card>();
            _selectedPeopleCache = new List<Person>();
            _cardPeopleTable = new Dictionary<Card, Person>();
            foreach (var p in _people)
            {
                if (p.IsSelected)
                {
                    _selectedPeopleCache.Add(p);
                }

                foreach (var c in p.Cards)
                {
                    if (c.IsSelected)
                        _selectedCardCache.Add(c);

                    _cardPeopleTable.Add(c, p);
                }
            }

            _isLoaded = true;
        }

        private void OnSelectionChanged()
        {
            int selectedCacheCardCount = _selectedCardCache.Count;
            int selectedUICardCount = 0;
            foreach (var p in _people)
            {
                foreach (var c in p.Cards)
                {
                    if (c.IsSelected)
                        ++selectedUICardCount;
                }
            }

            // card added
            if (selectedUICardCount > selectedCacheCardCount)
            {
                Card toAdd = null;
                // get the selected one
                foreach (var p in _people)
                {
                    foreach (var c in p.Cards)
                    {
                        if (c.IsSelected && !_selectedCardCache.Contains(c))
                        {
                            toAdd = c;
                            break;
                        }
                    }
                }
                _selectedCardCache.Add(toAdd);

                var person = _cardPeopleTable[toAdd];
                if (!person.IsSelected)
                {
                    person.IsSelected = true;
                }

                // remove the oldest selection
                if (selectedUICardCount > _maxNumSelections)
                {
                    var toRemove = _selectedCardCache[0];
                    _selectedCardCache.Remove(toRemove);
                    toRemove.IsSelected = false;

                    var removedCardPerson = _cardPeopleTable[toRemove];
                    if (!removedCardPerson.Cards.Any(c => c.IsSelected))
                        removedCardPerson.IsSelected = false;
                }
            }
            // card removed
            else if (selectedUICardCount < selectedCacheCardCount)
            {
                // check to see if its the last item

                // prevent this case and reselect the last item in the cache
                if (selectedUICardCount == 0)
                {
                    _selectedCardCache[0].IsSelected = true;
                }
                // just go ahead and remove the card
                else
                {
                    Card toRemove = null;
                    foreach (var c in _selectedCardCache)
                    {
                        if (!c.IsSelected)
                        {
                            toRemove = c;
                            break;
                        }
                    }

                    _selectedCardCache.Remove(toRemove);

                    var removedCardPerson = _cardPeopleTable[toRemove];
                    if (!removedCardPerson.Cards.Any(c => c.IsSelected))
                        removedCardPerson.IsSelected = false;
                }
            }
            else
            {
                // do nothing
            }
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
