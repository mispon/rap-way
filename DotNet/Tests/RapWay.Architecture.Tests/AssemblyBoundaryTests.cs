using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using RapWay.Application;
using RapWay.Domain;

namespace RapWay.Architecture.Tests
{
    [TestFixture]
    public sealed class AssemblyBoundaryTests
    {
        private static readonly string[] ForbiddenReferencePrefixes =
        {
            "Cysharp",
            "DG.Tweening",
            "MessagePipe",
            "Newtonsoft",
            "UniRx",
            "Unity",
            "VContainer"
        };

        [Test]
        public void DomainDoesNotReferenceUnityOrVendorAssemblies()
        {
            AssertNoForbiddenReferences(typeof(DomainAssembly).Assembly);
        }

        [Test]
        public void ApplicationDependsOnDomainWithoutUnityOrVendorAssemblies()
        {
            Assembly applicationAssembly = typeof(ApplicationAssembly).Assembly;

            AssertNoForbiddenReferences(applicationAssembly);
            Assert.That(
                applicationAssembly.GetReferencedAssemblies().Select(reference => reference.Name),
                Does.Contain(DomainAssembly.Name));
        }

        private static void AssertNoForbiddenReferences(Assembly assembly)
        {
            IReadOnlyCollection<string> forbiddenReferences = assembly
                .GetReferencedAssemblies()
                .Select(reference => reference.Name ?? string.Empty)
                .Where(reference => ForbiddenReferencePrefixes.Any(
                    prefix => reference.StartsWith(prefix, StringComparison.Ordinal)))
                .ToArray();

            Assert.That(
                forbiddenReferences,
                Is.Empty,
                $"{assembly.GetName().Name} has forbidden references: {string.Join(", ", forbiddenReferences)}");
        }
    }
}
