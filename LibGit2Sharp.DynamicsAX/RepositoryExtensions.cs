using System;
using System.Collections.Generic;
using System.Linq;

namespace LibGit2Sharp.DynamicsAX
{
    public static class RepositoryExtensions
    {
        /// <summary>
        /// Reverts a single modified file in the working directory
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="filePath"></param>
        public static void RevertFile(this Repository repository, string filePath)
        {
            if(string.IsNullOrEmpty(filePath)) throw new ArgumentNullException("filePath");

            repository.RevertFile("HEAD", filePath);
        }

        /// <summary>
        /// reverts a single file to a given branch or commit spec
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="committishOrBranchSpec"></param>
        /// <param name="filePath"></param>
        public static void RevertFile(this Repository repository, string committishOrBranchSpec, string filePath)
        {
            if(string.IsNullOrEmpty(filePath)) throw new ArgumentNullException("filePath");
            if(string.IsNullOrEmpty(committishOrBranchSpec)) throw new ArgumentNullException("committishOrBranchSpec");

            repository.CheckoutPaths(committishOrBranchSpec, new String[] {filePath}, new CheckoutOptions() {CheckoutModifiers = CheckoutModifiers.Force});
        }

        /// <summary>
        /// Filters the list of commits by those affecting the given path
        /// </summary>
        /// <param name="commits"></param>
        /// <param name="relativePath"></param>
        /// <returns></returns>
        public static IEnumerable<Commit> AffectsPath(this IEnumerable<Commit> commits, string relativePath)
        {
            if(string.IsNullOrEmpty(relativePath)) throw new ArgumentNullException("relativePath");

            foreach (var commit in commits)
            {
                if (commit.Tree[relativePath] != null)
                    yield return commit;
            }
        }

        public static IEnumerable<Branch> ListBranchesContainingCommit(this Repository repo, string commitSha, bool localOnly = false)
        {
            var heads = (IEnumerable<Reference>)repo.Refs;
            
            if(localOnly)
                heads = heads.Where(r => r.IsLocalBranch());

            var commit = repo.Lookup<Commit>(commitSha);
            var localHeadsContainingTheCommit = repo.Refs.ReachableFrom(heads, new[] { commit });

            return localHeadsContainingTheCommit
                .Select(branchRef => repo.Branches[branchRef.CanonicalName]);
        }


    }
}
