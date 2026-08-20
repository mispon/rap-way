using System;
using RapWay.Domain;

namespace RapWay.Application
{
    public static class ApplicationAssembly
    {
        public const string Name = "RapWay.Application";

        public static Type DomainMarkerType => typeof(DomainAssembly);
    }
}
