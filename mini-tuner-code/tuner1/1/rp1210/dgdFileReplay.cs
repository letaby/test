// Decompiled with JetBrains decompiler
// Type: rp1210.dgdFileReplay
// Assembly: TunerSolution, Version=1.0.0.142, Culture=neutral, PublicKeyToken=null
// MVID: 9D02C703-4AB8-4296-B056-FAFCB6EB03BA
// Assembly location: C:\Users\petra\Downloads\TunerSolution\TunerSolution.exe

using System.Collections.Concurrent;
using System.Diagnostics;

#nullable disable
namespace rp1210;

internal class dgdFileReplay
{
  public RP121032 J1939Instance { get; set; }

  public RP121032 J1587Instance { get; set; }

  public long TimeOffsetMs { get; set; }

  public ConcurrentQueue<J1939Message> TXQueue { get; set; }

  public bool Running { get; set; }

  public void dgdReplay()
  {
    Stopwatch stopwatch = Stopwatch.StartNew();
    while (this.Running)
    {
      J1939Message result;
      if (this.TXQueue.TryPeek(out result) && stopwatch.ElapsedMilliseconds > (long) result.TimeStamp - this.TimeOffsetMs && this.TXQueue.TryDequeue(out result))
      {
        byte[] fpchClientMessage = RP121032.EncodeJ1939Message(result);
        int num = (int) this.J1939Instance.RP1210_SendMessage(fpchClientMessage, (short) fpchClientMessage.Length, (short) 0, (short) 1);
      }
    }
  }
}
