// Decompiled with JetBrains decompiler
// Type: Softing.Dts.MCDDbFlashSession
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;

#nullable disable
namespace Softing.Dts;

public interface MCDDbFlashSession : MCDDbObject, MCDNamedObject, MCDObject, IDisposable
{
  MCDDbFlashSessionClasses DbFlashSessionsClasses { get; }

  string FlashKey { get; }

  MCDDbFlashChecksums Checksums { get; }

  MCDDbFlashIdents DbExpectedIdents { get; }

  MCDDbFlashSecurities DbSecurities { get; }

  MCDDbFlashDataBlocks DbDataBlocks { get; }

  MCDDbFlashJob DbFlashJob { get; }

  uint Priority { get; }

  bool IsDownload { get; }

  MCDDbSpecialDataGroups DbSDGs { get; }

  MCDDbFlashJob GetDbFlashJobByLocation(MCDDbLocation pDtsDbLocation);
}
