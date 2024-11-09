using RestSharpServices;
using System.Net;
using System.Reflection.Emit;
using System.Text.Json;
using RestSharp;
using RestSharp.Authenticators;
using NUnit.Framework.Internal;
using RestSharpServices.Models;
using System;

namespace TestGitHubApi
{
    public class TestGitHubApi
    {
        private GitHubApiClient client;
        string repoName = "test-nakov-repo";
        int lastCreatedIssueNumber;
        long lastCreatedCOmmentId;


        [SetUp]
        public void Setup()
        {            
            client = new GitHubApiClient("https://api.github.com/repos/testnakov/", "Dzhantov", "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");
        }


        [Test, Order (1)]
        public void Test_GetAllIssuesFromARepo()
        {

            var issues = client.GetAllIssues(repoName);

            Assert.That(issues.Count, Is.GreaterThan(0), "there should be more then 1 issue");

            foreach(var issue in issues)
            {
                Assert.That(issue.Id, Is.GreaterThan(0), "issue ID should be bigger than zero");
                Assert.That(issue.Number, Is.GreaterThan(0), "Issue number should be bigger than 0");
                Assert.That(issue.Title, Is.Not.Empty, "issue title should not be empty");
            }
        }

        [Test, Order (2)]
        public void Test_GetIssueByValidNumber()
        {
            //Arrange
            int issueNumber = 1;

            //Act
            var issue = client.GetIssueByNumber(repoName,issueNumber);

            //Assert
            Assert.IsNotNull(issue, "The data from the response should not be null");
            Assert.That(issue.Id, Is.GreaterThan(0), "issue ID should be bigger than zero");
            Assert.That(issue.Number, Is.GreaterThan(0), "Issue number should be bigger than 0");
        }
        
        [Test, Order (3)]
        public void Test_GetAllLabelsForIssue()
        {
            int issueNumber = 6;

            var labels = client.GetAllLabelsForIssue(repoName, issueNumber);

            Assert.That(labels, Has.Count.GreaterThan(0), "labels count should be greater than zero");

            foreach (var label in labels)
            {
                Assert.Multiple(() =>
                {
                    Assert.That(label.Id, Is.GreaterThan(0), " label ID should be bigger than 0");
                    Assert.That(label.Name, Is.Not.Empty, "Label name should not be empty");
                });

                Console.WriteLine("Label: " + label.Id + " - Name: " + label.Name);
            }
        }

        [Test, Order (4)]
        public void Test_GetAllCommentsForIssue()
        {
            int issueNumber = 6;

            var comments = client.GetAllCommentsForIssue(repoName, issueNumber);

            Assert.That(comments, Has.Count.GreaterThan(0), "Comments count should be bigger than 0");

            foreach (var comment in comments)
            {
                Assert.Multiple(() =>
                {
                    Assert.That(comment.Id, Is.GreaterThan(0), "Comment ID should be bigger than 0");
                    Assert.That(comment.Body, Is.Not.Empty, "Comment body should not be empty");
                });

                Console.WriteLine("Comment: " + comment.Id + " - Body: " + comment.Body);
            }
        }

        [Test, Order(5)]
        public void Test_CreateGitHubIssue()
        {
            //Arrange
            string title = "some random title";
            string body = "some random body";

            //Act
            var issue = client.CreateIssue(repoName, title, body);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(issue.Id, Is.GreaterThan(0));
                Assert.That(issue.Number, Is.GreaterThan(0));
                Assert.That(issue.Title, Is.EqualTo(title));
                Assert.That(issue.Body, Is.EqualTo(body));
            });

            Console.WriteLine(issue.Number);
            lastCreatedIssueNumber = issue.Number;
        }

        [Test, Order (6)]
        public void Test_CreateCommentOnGitHubIssue()
        {
            //Arrange
            var commentBody = "test comment122";

            //Act
            var comment = client.CreateCommentOnGitHubIssue(repoName, lastCreatedIssueNumber, commentBody);

            //Assert
            Assert.That(comment.Body, Is.EqualTo(commentBody));

            Console.WriteLine(comment.Id);
            lastCreatedCOmmentId = comment.Id;
        }

        [Test, Order (7)]
        public void Test_GetCommentById()
        {
            var comment = client.GetCommentById(repoName, lastCreatedCOmmentId);

            //Assert
            Assert.IsNotNull(comment);
            Assert.That(comment.Id, Is.EqualTo(lastCreatedCOmmentId));
        }


        [Test, Order (8)]
        public void Test_EditCommentOnGitHubIssue()
        {
            //Arrange
            string newBody = "updatedBody";
            //Act
            var comment = client.EditCommentOnGitHubIssue(repoName, lastCreatedCOmmentId, newBody);
            //Assert
            Assert.IsNotNull(comment);
            Assert.That(comment.Id, Is.EqualTo(lastCreatedCOmmentId));
            Assert.That(comment.Body, Is.EqualTo(newBody));

        }

        [Test, Order (9)]
        public void Test_DeleteCommentOnGitHubIssue()
        {
            //Act
            var result = client.DeleteCommentOnGitHubIssue(repoName, lastCreatedCOmmentId);

            //Asseert
            Assert.IsTrue(result, "The comment should be deleted successfully");
        }


    }
}

