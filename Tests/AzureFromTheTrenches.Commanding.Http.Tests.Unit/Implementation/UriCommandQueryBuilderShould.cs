using System;
using System.Collections.Generic;
using System.Text;
using AzureFromTheTrenches.Commanding.Http.Implementation;
using AzureFromTheTrenches.Commanding.Http.Tests.Unit.TestInfrastructure;
using Xunit;

namespace AzureFromTheTrenches.Commanding.Http.Tests.Unit.Implementation
{
    public class UriCommandQueryBuilderShould
    {
        [Fact]
        public void BuildQueryParameters()
        {
            SimpleCommand command = new SimpleCommand
            {
                Message = "hello",
                SomeNumber = 99
            };
            Uri uri = new Uri("http://localhost");
            UriCommandQueryBuilder testSubject = new UriCommandQueryBuilder();

            string result = testSubject.Query(uri, command);

            Assert.Equal("Message=hello&SomeNumber=99", result);
        }

        [Fact]
        public void StartWithAmpersandWhenExistingQueryParametersInUri()
        {
            SimpleCommand command = new SimpleCommand
            {
                Message = "hello",
                SomeNumber = 99
            };
            Uri uri = new Uri("http://localhost?existing=elephant");
            UriCommandQueryBuilder testSubject = new UriCommandQueryBuilder();

            string result = testSubject.Query(uri, command);

            Assert.Equal("&Message=hello&SomeNumber=99", result);
        }

        [Fact]
        public void UseFundamentalTypeRatherThanGenericType()
        {
            object command = new SimpleCommand
            {
                Message = "hello",
                SomeNumber = 99
            };
            Uri uri = new Uri("http://localhost");
            UriCommandQueryBuilder testSubject = new UriCommandQueryBuilder();

            string result = testSubject.Query(uri, command);

            Assert.Equal("Message=hello&SomeNumber=99", result);
        }
    }
}
