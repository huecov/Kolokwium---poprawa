using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Web.Script.Serialization;
using System.Windows;

namespace Cards
{
    /// <summary>
    /// Main class for View Model
    /// TODO: follow guidelines
    /// </summary>
    public class CardsViewModel : ICardsViewModel, INotifyPropertyChanged
    {
        private readonly IDispatcher _dispatcher;
        public CardsViewModel(IDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        private ObservableCollection<ProbabilitySet> _evaluatedSets = null;
        public System.Collections.ObjectModel.ObservableCollection<ProbabilitySet> EvaluatedSets
        {
            get { return _evaluatedSets; }
        }

        public IList<CardRank> AvailableRanks
        {
            get 
            {
                //return null;
                return (IList<CardRank>)Enum.GetValues(typeof(CardRank));
            }
        }

        private IList<CardRank> _chosenRanks = null;
        public IList<CardRank> ChosenRanks
        {
            get
            {
                return _chosenRanks;
                //return (IList<CardRank>)Enum.GetValues(typeof(CardRank));
            }
            set
            {
                _chosenRanks = value;
                OnPropertyChanged("ChosenRanks");
            }
        }

        public IList<CardSuit> AvailableSuits
        {
            get 
            {
                return (IList<CardSuit>)Enum.GetValues(typeof(CardSuit));
            }
        }

        private IList<CardSuit> _chosenSuits = null;
        public IList<CardSuit> ChosenSuits
        {
            get
            {
                return _chosenSuits;
                //return (IList<CardSuit>)Enum.GetValues(typeof(CardSuit));
            }
            set
            {
                _chosenSuits = value;
                OnPropertyChanged("ChosenSuits");
            }
        }

        public ProbabilitySet HighestProbability { get; set; }

        public System.Windows.Input.ICommand SaveSearchesCommand
        {
            get { return new MyCommand(SaveSearchesCommandMethod); }
        }

        private void SaveSearchesCommandMethod()
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string s = serializer.Serialize(_evaluatedSets);
            using (StreamWriter writer = new StreamWriter("myLocalDB.txt"))
            {
                writer.WriteLine(s);
            }
        }

        public System.Windows.Input.ICommand CalcCommand
        {
            get { return new MyCommand(CalcCommandMethod); }
        }

        private void CalcCommandMethod()
        {
            if (ChosenSuits != null || ChosenRanks != null)
            {
                 var propab = ((ChosenSuits.Count() * AvailableRanks.Count()) + (ChosenRanks.Count() * AvailableSuits.Count()) - (ChosenSuits.Count() * ChosenRanks.Count())) / 52;

                var propabSet = new ProbabilitySet();
                propabSet.Suits = ChosenSuits;
                propabSet.Ranks = ChosenRanks;
                propabSet.Probability = propab;
                _evaluatedSets.Add(propabSet);
            }
            else
                MessageBox.Show("We cannot compute propab.");
        }

        public System.Windows.Input.ICommand ShowHighestCommand
        {
            get { return new MyCommand(ShowHighestCommandMethod); }
        }

        private void ShowHighestCommandMethod()
        {
            if (_evaluatedSets != null)
            {
                var hihgestPropab = _evaluatedSets.OrderByDescending(i => i.Probability).FirstOrDefault();//MaxBy(i => i.Probability);
                if (hihgestPropab != null)
                {
                    HighestProbability = hihgestPropab;
                    MessageBox.Show(hihgestPropab.Probability.ToString());
                }
            }
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
