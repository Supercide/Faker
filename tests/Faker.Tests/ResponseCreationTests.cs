using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Faker.Core;
using Faker.Core.Extensions;
using FFakerTests;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Faker.Tests {
    public class ResponseCreationTests
    {
        [Test]
        public void GivenRequest_WhenCreatingResponseFromStaticTemplate_ThenDoesNotExtractAnyFieldsToMerge()
        {
            Mock<IRequest> mockRequest = new Mock<IRequest>();
            Mock<ITemplate> mockTemplate = new Mock<ITemplate>();

            mockTemplate.Setup(x => x.Response)
                        .Returns("{ \"a\": \"1\",\"b\": \"2\",\"c\": \"3\" }");

            mockTemplate.Setup(x => x.MergeFields(It.IsAny<IEnumerable<IMergeField>>()))
                        .Returns("{ \"a\": \"1\",\"b\": \"2\",\"c\": \"3\" }");

            ResponseFactory factory = new ResponseFactory();

            factory.Create(mockRequest.Object, mockTemplate.Object);

            mockTemplate.Verify(x => x.MergeFields(It.Is<IEnumerable<IMergeField>>(mergeFields => !mergeFields.Any())));
        }

       [Test]
        public void GivenDynamicTemplate_WhenCreatingResponseFromObject_ThenCreatesResponse()
        {
            Mock<IRequest> mockRequest = new Mock<IRequest>();

            mockRequest.Setup(x => x.GetPropertyValueBy("a"))
                       .Returns("1");
            mockRequest.Setup(x => x.GetPropertyValueBy("b"))
                       .Returns("2");
            mockRequest.Setup(x => x.GetPropertyValueBy("c"))
                       .Returns("3");

            Mock<ITemplate> mockTemplate = new Mock<ITemplate>();

            mockTemplate.Setup(x => x.Response)
                        .Returns("{ \"Message\": \"Captured : {{a}}, {{b}}, {{c}}\" }");

            mockTemplate.Setup(x => x.MergeFields(It.IsAny<IEnumerable<IMergeField>>()))
                        .Returns("{ \"a\": \"1\",\"b\": \"2\",\"c\": \"3\" }");

            ResponseFactory factory = new ResponseFactory();

            factory.Create(mockRequest.Object, mockTemplate.Object);

            Func<IMergeField, bool> expectedMergFields = field => field.Property == "a" && field.Value == "1" && field.Token == "{{a}}"
                                                               || field.Property == "b" && field.Value == "2" && field.Token == "{{b}}"
                                                               || field.Property == "c" && field.Value == "3" && field.Token == "{{c}}";

            mockTemplate.Verify(x => x.MergeFields(It.Is(ValidatePredicate(expectedMergFields))));

        }

        private static Expression<Func<IEnumerable<IMergeField>, bool>> ValidatePredicate(Func<IMergeField, bool> predicate)
        {
            return mergeFields => mergeFields.All(predicate);
        }


        /*[Test]
       public void GivenDynamicTemplate_WhenCreatingResponseFromObjectUsingIndexes_ThenCreatesResponse()
       {
           var request = "{ \"a\": \"1\",\"b\": \"2\",\"c\": \"3\" }";
           var expected = "{ \"Message\": \"Captured : 1, 2, 3\" }";
           JsonTemplate template = new JsonTemplate
           {
               Response = "{ \"Message\": \"Captured : {{0}}, {{1}}, {{2}}\" }"
           };

           ResponseFactory factory = new ResponseFactory();

           var response = factory.Create(JsonRequest.Create(request), template);

           Assert.That(response, Is.EqualTo(expected));
       }

       [Test]
       public void GivenDynamicTemplate_WhenCreatingResponseFromNestedObject_ThenCreatesResponse()
       {
           var request = JsonConvert.SerializeObject(NestedObject.Create());
           var expected = "{ \"Message\": \"Captured : Elite, 1337, True, 2, Dam!, 1881, False, 5\" }";
           JsonTemplate template = new JsonTemplate
           {
               Response = "{ \"Message\": \"Captured : {{String}}, {{Number}}, {{Bool}}, {{Array[1]}}, {{Nested.String}}, {{Nested.Number}}, {{Nested.Bool}}, {{Nested.Array[1]}}\" }"
           };

           ResponseFactory factory = new ResponseFactory();

           var response = factory.Create(JsonRequest.Create(request), template);

           Assert.That(response, Is.EqualTo(expected));
       }

       [Test]
       public void GivenDynamicTemplate_WhenCreatingResponseFromNestedObjectUsingIndexes_ThenCreatesResponse()
       {
           var request = JsonConvert.SerializeObject(NestedObject.Create());
           var expected = "{ \"Message\": \"Captured : Elite, 1, 2, 3, 1337, True\" }";
           JsonTemplate template = new JsonTemplate
           {
               Response = "{ \"Message\": \"Captured : {{0}}, {{1}}, {{2}}, {{3}}, {{4}}, {{5}}\" }"
           };

           ResponseFactory factory = new ResponseFactory();

           var response = factory.Create(JsonRequest.Create(request), template);

           Assert.That(response, Is.EqualTo(expected));
       }


       [Test]
       public void GivenDynamicTemplate_WhenCreatingResponseFromArray_ThenCreatesResponse()
       {
           var request = "[ \"a\",\"b\",\"c\" ]";
           var expected = "{ \"Message\": \"Captured : a, b, c\" }";
           JsonTemplate template = new JsonTemplate
           {
               Response = "{ \"Message\": \"Captured : {{0}}, {{1}}, {{2}}\" }"
           };

           ResponseFactory factory = new ResponseFactory();

           var response = factory.Create(JsonRequest.Create(request), template);

           Assert.That(response, Is.EqualTo(expected));
       }

       [Test]
       public void GivenDynamicTemplate_WithRequestMissingToken_WhenCreatingResponseFromArray_ThenCreatesResponse_WithMissingTokenEmpty()
       {
           var request = "[ \"a\",\"b\",\"c\" ]";
           var expected = "{ \"Message\": \"Captured : a, b, c \" }";
           JsonTemplate template = new JsonTemplate
           {
               Response = "{ \"Message\": \"Captured : {{0}}, {{1}}, {{2}} {{3}}\" }"
           };

           ResponseFactory factory = new ResponseFactory();

           var response = factory.Create(JsonRequest.Create(request), template);

           Assert.That(response, Is.EqualTo(expected));
       }

       [Test]
       public void GivenDynamicTemplate_WithRequestMissingToken_WhenCreatingResponseFromObject_ThenCreatesResponse_WithMissingTokenEmpty()
       {
           var request = "{ \"a\": \"1\",\"b\": \"2\",\"c\": \"3\" }";
           var expected = "{ \"Message\": \"Captured : 1, 2, 3 \" }";
           JsonTemplate template = new JsonTemplate
           {
               Response = "{ \"Message\": \"Captured : {{a}}, {{b}}, {{c}} {{d}}\" }"
           };

           ResponseFactory factory = new ResponseFactory();

           var response = factory.Create(JsonRequest.Create(request), template);

           Assert.That(response, Is.EqualTo(expected));
       }*/
    }
}