using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using LibGit2Sharp.DynamicsAX.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LibGit2Sharp.DynamicsAX.Tests
{
    [TestClass]
    public class UnitTest1
    {


        [TestMethod]
        public void TestMethod1()
        {
            var repo = new Repository(@"C:\Memx\MemxRetail");

            var stopWatch = Stopwatch.StartNew();

            var org = new BranchGraphOrganizer(repo);

            org.LoadNextCommits();

            stopWatch.Stop();
            
        }
    }
}
