// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage(
    "Style",
    "IDE1006:Naming Styles",
    Justification = "I don't think that these strings could potentially become language keywords",
    Scope = "type",
    Target = "~T:Photobox.Web.Migrations.addedimagemodelsdb"
)]
[assembly: SuppressMessage(
    "Style",
    "IDE1006:Naming Styles",
    Justification = "We want to allow lowercase names for migration files",
    Scope = "type",
    Target = "~T:Photobox.Web.Migrations.AddedNameOfDownscaledImage"
)]
