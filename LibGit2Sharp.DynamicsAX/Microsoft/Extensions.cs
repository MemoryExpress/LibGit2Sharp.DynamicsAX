//Microsoft has some good GIT extensions in some of it's git TFS libraries, some are copied here
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGit2Sharp.DynamicsAX.Microsoft
{
    [Flags]
    internal enum GitHistoryOptions
    {
        None = 0,
        //IncludeChanges = 1,
        FollowRenames = 2,
        FollowRenameEdits = 4,
        TopologicalTimeSort = 8,
        ExcludeMerges = 16,
        ExcludeUnnecessaryCommits = 32,
    }

    internal class TeamFoundationGitExtensions
    {
        public IList<Commit> GetHistory(Repository repository, Branch branch, string relativePath, object since, object until, int maxCount, GitHistoryOptions options = GitHistoryOptions.None)
        {
            List<Commit> list = new List<Commit>();
            //bool flag1 = (options & GitHistoryOptions.IncludeChanges) != GitHistoryOptions.None;
            string str = relativePath;
            bool hasRelativePath = !string.IsNullOrEmpty(str);
            bool excludeUnnecessaryCommits = hasRelativePath && (options & GitHistoryOptions.ExcludeUnnecessaryCommits) != GitHistoryOptions.None;
            HashSet<ObjectId> necessaryObjectIds = new HashSet<ObjectId>();
            if (since == null)
            {
                if (branch != null && branch.Tip != null)
                    since = (object)branch.Tip.Id.Sha;
                if (since == null)
                    since = (object)repository.Head;
            }
            CommitSortStrategies commitSortStrategies = CommitSortStrategies.Time;
            if ((options & GitHistoryOptions.TopologicalTimeSort) != GitHistoryOptions.None)
                commitSortStrategies |= CommitSortStrategies.Topological;
            CommitFilter filter = new CommitFilter()
            {
                Since = since,
                Until = until,
                SortBy = commitSortStrategies
            };
            foreach (Commit commit in (IEnumerable<Commit>)repository.Commits.QueryBy(filter))
            {
                string renameSourcePath = (string)null;
                ChangeKind? changeKind = new ChangeKind?();
                if (hasRelativePath)
                {
                    if (excludeUnnecessaryCommits && necessaryObjectIds.Count == 0)
                    {
                        TreeEntry treeEntry = commit.Tree[relativePath];
                        if (treeEntry != (TreeEntry)null)
                            necessaryObjectIds.Add(treeEntry.Target.Id);
                    }
                    changeKind = this.CommitContainsFile(repository, commit, str, options, necessaryObjectIds, out renameSourcePath);
                    if (!changeKind.HasValue || changeKind.Value == ChangeKind.Unmodified)
                    {
                        //trace "GitRepository.GetHistory Skipping commit {0} because it doesn't contain the file", commit.Id != (ObjectId)null ? (object)GitId.GetShortId(commit.Id.Sha) : (object)(string)null);
                        continue;
                    }
                }

                //trace "GitRepository.GetHistory Adding commit {0}", commit.Id != (ObjectId)null ? (object)GitId.GetShortId(commit.Id.Sha) : (object)(string)null);
                list.Add(commit);
                if (hasRelativePath && excludeUnnecessaryCommits && (changeKind.HasValue && changeKind.Value == ChangeKind.Added) && string.IsNullOrEmpty(renameSourcePath))
                {
                    //trace "GitRepository.GetHistory Breaking after finding changeKind={0}", changeKind.HasValue ? (object)changeKind.Value.ToString() : (object)"null");
                    break;
                }
                if (list.Count >= maxCount)
                {
                    //trace "GitRepository.GetHistory Breaking after reaching the commit maxCount={0}", (object)maxCount);
                    break;
                }
                if (!string.IsNullOrEmpty(renameSourcePath))
                    str = renameSourcePath;
            }
            //trace "GitRepository.GetHistory Found {0} commits", (object)list.Count);
            return (IList<Commit>)list.AsReadOnly();
        }

        private ChangeKind? CommitContainsFile(Repository repository, Commit commit, string relativePath, GitHistoryOptions options, HashSet<ObjectId> necessaryObjectIds, out string renameSourcePath)
        {
            renameSourcePath = (string)null;
            if ((GitObject)commit == (GitObject)null)
                return new ChangeKind?();
            if (Enumerable.Count<Commit>(commit.Parents) == 0)
            {
                TreeEntry treeEntry = commit[relativePath];
                if (!(treeEntry != (TreeEntry)null))
                    return new ChangeKind?();
                //trace "GitRepository.CommitContainsFile true id={0} objectId={1} relPath={2}", (object)GitId.GetShortId(commit.Id.Sha), (object)GitId.GetShortId(treeEntry.Target.Sha), (object)relativePath);
                return new ChangeKind?(ChangeKind.Added);
            }
            bool flag1 = (options & GitHistoryOptions.ExcludeMerges) != GitHistoryOptions.None;
            bool flag2 = (options & GitHistoryOptions.ExcludeUnnecessaryCommits) != GitHistoryOptions.None;
            bool flag3 = (options & GitHistoryOptions.FollowRenames) != GitHistoryOptions.None;
            List<ObjectId> list = (List<ObjectId>)null;
            Commit commit1 = Enumerable.First<Commit>(commit.Parents);
            string[] strArray = new string[1]
      {
        relativePath
      };
            TreeEntryChanges fileChange = repository.Diff.Compare<TreeChanges>(commit1.Tree, commit.Tree, (IEnumerable<string>)strArray, (ExplicitPathsOptions)null, new CompareOptions()
            {
                Similarity = SimilarityOptions.None
            })[relativePath];
            if (fileChange != null)
            {
                
                //TreeEntry treeEntry = commit.Tree[relativePath];
                //trace "GitRepository.CommitContainsFile true commitId={0} parentId={1} fileObjectId={2} parentObjectId={3} relPath={4}", commit.Id != (ObjectId)null ? (object)GitId.GetShortId(commit.Id.Sha) : (object)"null", commit1.Id != (ObjectId)null ? (object)GitId.GetShortId(commit1.Id.Sha) : (object)"null", fileChange.Oid != (ObjectId)null ? (object)GitId.GetShortId(fileChange.Oid.Sha) : (object)"null", fileChange.OldOid != (ObjectId)null ? (object)GitId.GetShortId(fileChange.OldOid.Sha) : (object)"null", (object)relativePath);
                
                bool flag4 = true;
                if (flag2)
                {
                    flag4 = necessaryObjectIds.Contains(fileChange.Oid);
                    if (flag4)
                    {
                        list = new List<ObjectId>();
                        list.Add(fileChange.OldOid);
                    }
                }
                bool flag5 = false;
                if (Enumerable.Count<Commit>(commit.Parents) > 1 && (flag1 || flag2 && flag4))
                {
                    TreeEntry treeEntry1 = commit.Tree[relativePath];
                    if (treeEntry1 != (TreeEntry)null && treeEntry1.Target != (GitObject)null)
                    {
                        foreach (Commit commit2 in commit.Parents)
                        {
                            if (!(commit2.Id == commit1.Id))
                            {
                                TreeEntry treeEntry2 = commit2.Tree[relativePath];
                                if (treeEntry2 != (TreeEntry)null && treeEntry2.Target != (GitObject)null)
                                {
                                    if (treeEntry2.Target.Id == treeEntry1.Target.Id)
                                        flag5 = true;
                                    else if (flag2 && flag4)
                                        list.Add(treeEntry2.Target.Id);
                                }
                            }
                        }
                    }
                }
                if (flag3 && fileChange.Status == ChangeKind.Added)
                {
                    TreeChanges treeChanges = repository.Diff.Compare<TreeChanges>(commit1.Tree, commit.Tree, (IEnumerable<string>)null, (ExplicitPathsOptions)null, new CompareOptions()
                    {
                        Similarity = SimilarityOptions.None
                    });
                    TreeEntryChanges renameSourceChange;
                    if (treeChanges != null && this.CheckForRename(treeChanges, fileChange, out renameSourceChange))
                        renameSourcePath = renameSourceChange.Path;
                }
                if (flag5 && string.IsNullOrEmpty(renameSourcePath))
                {
                    //trace "GitRepository.CommitContainsFile Skipping commitId={0} objectId={1} because it only contains merged content for the file", commit.Id != (ObjectId)null ? (object)GitId.GetShortId(commit.Id.Sha) : (object)"null", (object)GitId.GetShortId(fileChange.Oid.Sha));
                    return new ChangeKind?(ChangeKind.Unmodified);
                }
                if (flag2)
                {
                    if (!flag4 && string.IsNullOrEmpty(renameSourcePath))
                    {
                        //trace "GitRepository.CommitContainsFile Skipping commitId={0} objectId={1} because the target objectId is not necessary", commit.Id != (ObjectId)null ? (object)GitId.GetShortId(commit.Id.Sha) : (object)"null", (object)GitId.GetShortId(fileChange.Oid.Sha));
                        return new ChangeKind?(ChangeKind.Unmodified);
                    }
                    list.ForEach((Action<ObjectId>)(pfObjId => necessaryObjectIds.Add(pfObjId)));
                }
                return new ChangeKind?(fileChange.Status);
            }
            //trace "GitRepository.CommitContainsFile false id={0} relPath={1}", (object)GitId.GetShortId(commit.Id.Sha), (object)relativePath);
            return new ChangeKind?(ChangeKind.Unmodified);
        }

        private bool CheckForRename(TreeChanges treeChanges, TreeEntryChanges fileChange, out TreeEntryChanges renameSourceChange)
        {
            if (fileChange == null || fileChange.Status != ChangeKind.Added)
            {
                renameSourceChange = (TreeEntryChanges)null;
                return false;
            }
            List<TreeEntryChanges> list = Enumerable.ToList<TreeEntryChanges>(Enumerable.Where<TreeEntryChanges>(treeChanges.Deleted, (Func<TreeEntryChanges, bool>)(d => d.OldOid == fileChange.Oid)));
            if (list.Count == 1)
            {
                renameSourceChange = list[0];
                //trace "GitRepository.CheckForRename rename detected relTarget={0} relSource={1}", (object)fileChange.Path, (object)renameSourceChange.Path);
                return true;
            }
            if (list.Count > 1)
            {
                //trace "GitRepository.CheckForRename rename detected but there are {0} possible sources", (object)list.Count);
            }

            renameSourceChange = (TreeEntryChanges)null;
            return false;
        }
    }
}
