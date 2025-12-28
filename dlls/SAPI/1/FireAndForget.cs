// Decompiled with JetBrains decompiler
// Type: SapiLayer1.FireAndForget
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

#nullable disable
namespace SapiLayer1;

internal sealed class FireAndForget
{
  private static Dictionary<int, Queue<FireAndForget>> threadBasedQueues = new Dictionary<int, Queue<FireAndForget>>();
  private Delegate d;
  private IList<object> argumentList;
  private Control threadMarshalControl;

  internal static void Invoke(MulticastDelegate md, object sender, EventArgs e)
  {
    Sapi sapi = Sapi.GetSapi();
    if (sapi == null)
      return;
    FireAndForget.Invoke(md, sender, e, sapi.ThreadMarshalControl);
  }

  internal static void Invoke(
    MulticastDelegate md,
    object sender,
    EventArgs e,
    Control threadMarshalControl)
  {
    if ((object) md == null)
      return;
    foreach (Delegate invocation in md.GetInvocationList())
    {
      object[] args = new object[2]{ sender, (object) e };
      FireAndForget fireAndForget = new FireAndForget(invocation, (IEnumerable<object>) args, threadMarshalControl);
      lock (FireAndForget.threadBasedQueues)
      {
        int managedThreadId = Thread.CurrentThread.ManagedThreadId;
        Queue<FireAndForget> fireAndForgetQueue;
        if (!FireAndForget.threadBasedQueues.TryGetValue(managedThreadId, out fireAndForgetQueue))
        {
          FireAndForget.threadBasedQueues[managedThreadId] = fireAndForgetQueue = new Queue<FireAndForget>();
          ThreadPool.QueueUserWorkItem(new WaitCallback(FireAndForget.InvokeQueue), (object) Tuple.Create<int, Queue<FireAndForget>>(managedThreadId, fireAndForgetQueue));
        }
        fireAndForgetQueue.Enqueue(fireAndForget);
      }
    }
  }

  private static void InvokeQueue(object queueObject)
  {
    Tuple<int, Queue<FireAndForget>> tuple = queueObject as Tuple<int, Queue<FireAndForget>>;
label_1:
    FireAndForget fireAndForget = (FireAndForget) null;
    lock (FireAndForget.threadBasedQueues)
      fireAndForget = tuple.Item2.Peek();
    fireAndForget.InvokeWrappedDelegate();
    lock (FireAndForget.threadBasedQueues)
    {
      tuple.Item2.Dequeue();
      if (tuple.Item2.Count == 0)
        FireAndForget.threadBasedQueues.Remove(tuple.Item1);
      else
        goto label_1;
    }
  }

  private FireAndForget(Delegate d, IEnumerable<object> args, Control threadMarshalControl)
  {
    this.d = d;
    this.argumentList = (IList<object>) args.ToList<object>().AsReadOnly();
    this.threadMarshalControl = threadMarshalControl;
  }

  private void InvokeWrappedDelegate()
  {
    if (this.d.Target != null && this.threadMarshalControl != null)
      this.threadMarshalControl.BeginInvoke(Delegate.CreateDelegate(this.d.GetType(), this.d.Target, this.d.Method.Name), this.argumentList.ToArray<object>());
    else
      this.d.Method.Invoke(this.d.Target, this.argumentList.ToArray<object>());
  }
}
