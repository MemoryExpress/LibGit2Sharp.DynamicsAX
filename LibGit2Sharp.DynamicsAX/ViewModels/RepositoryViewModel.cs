using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LibGit2Sharp.DynamicsAX.ViewModels
{
    [Export(typeof(RepositoryViewModel))]
    public class RepositoryViewModel : INotifyPropertyChanged
    {
        private readonly Repository _repository;
        private IEnumerable<Commit> _commits;

        public IEnumerable<Commit> Commits
        {
            get { return _commits; }
            set
            {
                _commits = value;
                OnPropertyChanged();
            }
        }

        [ImportingConstructor]
        public RepositoryViewModel(Repository repository)
        {
            if (repository == null) throw new ArgumentNullException("repository");

            _repository = repository;

            Commits = _repository.Commits.Take(100).ToList();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
