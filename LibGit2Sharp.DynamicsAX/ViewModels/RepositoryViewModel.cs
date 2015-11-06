using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace LibGit2Sharp.DynamicsAX.ViewModels
{
    public class BranchGraphDetails
    {
        public Branch Branch { get; set; }

        public Color Color { get; set; }

        public int Index { get; set; }

        public BranchGraphDetails()
        {
        }
    }

    public class CommitGraphDetails
    {
        public Branch OwningBranch { get; set; }

        public IEnumerable<Branch> Branches { get; set; }  

        public Commit Commit { get; set; }
    }

    public class BranchGraphOrganizer
    {
        public class WorkingCommit
        {
            public class IndexedEnumerator<T> : IEnumerator<T>
            {
                private readonly IEnumerator<T> _enumerator;

                public IndexedEnumerator(IEnumerator<T> enumerator)
                {
                    if (enumerator == null) throw new ArgumentNullException("enumerator");
                    _enumerator = enumerator;
                    Index = -1;
                }

                public void Dispose()
                {
                    _enumerator.Dispose();
                }

                public bool MoveNext()
                {
                    var result = _enumerator.MoveNext();
                    if (result)
                        Index++;

                    return result;
                }

                public void Reset()
                {
                    _enumerator.Reset();
                    Index = -1;
                }

                public T Current
                {
                    get { return _enumerator.Current; }
                }

                object IEnumerator.Current
                {
                    get { return Current; }
                }

                public int Index { get; private set; }
            }

            private IndexedEnumerator<Commit> _commitEnumerator;

            public IndexedEnumerator<Commit> CommitEnumerator
            {
                get
                {
                    if (_commitEnumerator == null)
                    {
                        _commitEnumerator = new IndexedEnumerator<Commit>(Branch.Commits.GetEnumerator());
                        if (!_commitEnumerator.MoveNext())
                            throw new InvalidOperationException(string.Format("No commits in branch {0}", Branch.Name));//always to to the first one
                    }

                    return _commitEnumerator;
                }
            }

            public bool MoveNext()
            {
                return CommitEnumerator.MoveNext();
            }

            public Commit Commit
            {
                get { return CommitEnumerator.Current; }
            }

            public int DistanceFromTip
            {
                get { return CommitEnumerator.Index; }
            }

            public Branch Branch { get; set; }

            public DateTimeOffset When
            {
                get { return CommitEnumerator.Current.Author.When; }
            }
        }

        private static readonly Color[] BranchColors = typeof (Colors)
            .GetProperties()
            .Where(p => p.PropertyType == typeof (Color))
            .Select(p => (Color) p.GetValue(null))
            .Where(c =>
            {
                //is dark enough
                if (c.A != 255)
                    return false;

                return true;
            })
            .ToArray();

        private readonly ConcurrentDictionary<string, BranchGraphDetails> _branches = new ConcurrentDictionary<string, BranchGraphDetails>();

        private readonly ObservableCollection<CommitGraphDetails> _commits = new ObservableCollection<CommitGraphDetails>();

        private readonly IList<WorkingCommit> _workingSet = new List<WorkingCommit>();

        private readonly Repository _repository;

        public IEnumerable<CommitGraphDetails> Commits
        {
            get
            {
                return _commits;
            }
        }

        public BranchGraphOrganizer(Repository repository)
        {
            if(repository == null) throw new ArgumentNullException("repository");
            _repository = repository;

            //add the head of the local repo first, so it will always be on the left of the graph.
            GetBranchGraphDetails(repository.Head);

            foreach (var branch in repository.Branches)
            {
                _workingSet.Add(new WorkingCommit()
                {
                    Branch = branch
                });
            }
        }

        public void LoadNextCommits(int take)
        {
            int commitsTaken = 0;

            while (commitsTaken < take)
            {
                //get top commit
                var commitGroup = _workingSet.GroupBy(c => c.Commit)
                    .OrderByDescending(g => g.Key.Author.When)
                    .FirstOrDefault();

                if (commitGroup == null)
                {
                    //no more commits
                    break;
                }

                var branchCommits = commitGroup.OrderBy(c => c.DistanceFromTip);

                AddCommit(commitGroup.Key, branchCommits.First().Branch, branchCommits.Select(bc => bc.Branch));

                commitsTaken++;

                foreach (var workingCommit in branchCommits)
                {
                    if (!workingCommit.MoveNext())
                    {
                        //no more commits
                        _workingSet.Remove(workingCommit);
                    }
                }
            }
        }

        public CommitGraphDetails AddCommit(Commit commit, Branch owner, IEnumerable<Branch> branches)
        {
            var details = new CommitGraphDetails()
            {
                OwningBranch = owner,
                Branches = branches.ToList(),
                Commit = commit
            };

            _commits.Add(details);

            GetBranchGraphDetails(owner);

            return details;
        }

        /// <summary>
        /// Gets branch graph details for the given branch
        /// </summary>
        /// <param name="branch"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public BranchGraphDetails GetBranchGraphDetails(Branch branch)
        {
            if(branch == null) throw new ArgumentNullException("branch");

            var detail = _branches.AddOrUpdate(branch.Name, (s) =>
            {
                var index = _branches.Count;

                return new BranchGraphDetails()
                {
                    Branch = branch,
                    Color = BranchColors[index],
                    Index = index
                };
            },
                (s, b) => b);

            return detail;
        }
    }

    public class RepositoryViewModel : INotifyPropertyChanged
    {
        private ICollectionView _commits;
        private int _resultsPerPage = 150;
        private BranchGraphOrganizer _branchGraphOrganizer;

        public Repository Repository { get; set; }

        public ICollectionView Commits
        {
            get
            {
                if (_commits == null)
                {
                    _commits = CollectionViewSource.GetDefaultView(BranchGraphOrganizer.Commits);
                }

                return _commits;
            }
        }

        public BranchGraphOrganizer BranchGraphOrganizer
        {
            get
            {
                if (_branchGraphOrganizer == null)
                {
                    _branchGraphOrganizer = new BranchGraphOrganizer(Repository);
                    _branchGraphOrganizer.LoadNextCommits(_resultsPerPage);
                }

                return _branchGraphOrganizer;
            }
        }

        public RepositoryViewModel(Repository repository)
        {
            if(repository == null) throw new ArgumentNullException("repository");
            Repository = repository;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
