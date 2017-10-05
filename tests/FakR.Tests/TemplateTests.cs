﻿using System;
using FakR.Core;
using Moq;
using NUnit.Framework;

namespace FakR.Tests
{
    [TestFixture]
    public class TemplateTests
    {
        [Test]
        public void GivenKnownContent_WhenRetrievingTemplates_ThenCallsTemplateStoreWithNameSpace()
        {
            Uri expectedNamespace = new Uri("http://localhost/anyvalue");

            Mock<ITemplateStore> mockTemplateStore = new Mock<ITemplateStore>();

            TemplateMatcher templateMatcher = new TemplateMatcher(mockTemplateStore.Object);

            templateMatcher.Match(string.Empty, expectedNamespace);

            mockTemplateStore.Verify(x => x.GetTemplates(It.Is<Uri>(@namespace => @namespace == expectedNamespace)), Times.Once);
        }

        [Test]
        public void GivenMultipleTemplates_WhenRetrievingTemplates_ThenReturnsTemplateWithMostMatchedFields()
        {
            string content = "{ \"a\": \"1\",\"b\": \"2\",\"c\": \"3\" }";
            var templateOne = CreateTemplate("{ \"a\":\"1\" }");
            var templateTwo = CreateTemplate("{ \"a\": \"1\",\"b\": \"2\" }");
            var templateThree = CreateTemplate("{ \"a\": \"1\",\"b\": \"2\",\"c\": \"3\", \"d\": \"4\" }");
            var expectedTemplate = CreateTemplate("{ \"a\": \"1\",\"b\": \"2\",\"c\": \"3\" }");

            Mock<ITemplateStore> mockTemplateStore = new Mock<ITemplateStore>();
            mockTemplateStore.Setup(x => x.GetTemplates(It.IsAny<Uri>())).Returns(new [] {templateOne, templateTwo, templateThree, expectedTemplate });

            TemplateMatcher templateMatcher = new TemplateMatcher(mockTemplateStore.Object);

            var actualTemplate = templateMatcher.Match(content, new Uri("http://anything"));

            Assert.That(actualTemplate.Incoming, Is.EqualTo(expectedTemplate.Incoming));
        }

        private static Template CreateTemplate(string incomingPattern)
        {
            Template templateOne = new Template
            {
                Incoming = incomingPattern
            };

            return templateOne;
        }
    }
}
