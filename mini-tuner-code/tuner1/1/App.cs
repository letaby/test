// Decompiled with JetBrains decompiler
// Type: TunerSolution.App
// Assembly: TunerSolution, Version=1.0.0.142, Culture=neutral, PublicKeyToken=null
// MVID: 9D02C703-4AB8-4296-B056-FAFCB6EB03BA
// Assembly location: C:\Users\petra\Downloads\TunerSolution\TunerSolution.exe

using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Windows;

#nullable disable
namespace TunerSolution;

public class App : Application
{
  [DebuggerNonUserCode]
  [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
  public void InitializeComponent()
  {
    this.StartupUri = new Uri("MainWindow.xaml", UriKind.Relative);
  }

  [STAThread]
  [DebuggerNonUserCode]
  [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
  public static void Main()
  {
    App app = new App();
    app.InitializeComponent();
    app.Run();
  }
}
