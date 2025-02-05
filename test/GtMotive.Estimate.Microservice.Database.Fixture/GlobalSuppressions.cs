// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Major Code Smell", "S1118:Utility classes should not have public constructors", Justification = "Constructor not required", Scope = "type", Target = "~T:GtMotive.Estimate.Microservice.Fixture.Database.Program")]
[assembly: SuppressMessage("Maintainability", "CA1515:Consider making public types internal", Justification = "Class referenced from outside", Scope = "module")]
[assembly: SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Args not used", Scope = "module")]
