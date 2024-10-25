// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "The code will only be used on a windows device", Scope = "member", Target = "~M:Photobox.Lib.IPC.IPCNamedPipeServer.ConnectAsync~System.Threading.Tasks.Task")]
[assembly: SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "The code will only be used on a windows device", Scope = "member", Target = "~M:Photobox.Lib.IPC.IPCNamedPipeClient.StartStreamAsync~System.Threading.Tasks.Task")]
[assembly: SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>", Scope = "member", Target = "~M:Photobox.Lib.Printer.Printer.PrintAsync(System.String)~System.Threading.Tasks.Task")]
[assembly: SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>", Scope = "member", Target = "~M:Photobox.Lib.Camera.CameraBase.OnNewStreamImage(System.Drawing.Bitmap)")]
[assembly: SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>", Scope = "member", Target = "~M:Photobox.Lib.Printer.Printer.PrintAsync(SixLabors.ImageSharp.Image{SixLabors.ImageSharp.PixelFormats.Rgb24})~System.Threading.Tasks.Task")]
