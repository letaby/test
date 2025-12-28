using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

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
		if (sapi != null)
		{
			Invoke(md, sender, e, sapi.ThreadMarshalControl);
		}
	}

	internal static void Invoke(MulticastDelegate md, object sender, EventArgs e, Control threadMarshalControl)
	{
		if ((object)md == null)
		{
			return;
		}
		Delegate[] invocationList = md.GetInvocationList();
		for (int i = 0; i < invocationList.Length; i++)
		{
			object[] args = new object[2] { sender, e };
			FireAndForget item = new FireAndForget(invocationList[i], args, threadMarshalControl);
			lock (threadBasedQueues)
			{
				int managedThreadId = Thread.CurrentThread.ManagedThreadId;
				if (!threadBasedQueues.TryGetValue(managedThreadId, out var value))
				{
					value = (threadBasedQueues[managedThreadId] = new Queue<FireAndForget>());
					ThreadPool.QueueUserWorkItem(InvokeQueue, Tuple.Create(managedThreadId, value));
				}
				value.Enqueue(item);
			}
		}
	}

	private static void InvokeQueue(object queueObject)
	{
		Tuple<int, Queue<FireAndForget>> tuple = queueObject as Tuple<int, Queue<FireAndForget>>;
		while (true)
		{
			FireAndForget fireAndForget = null;
			lock (threadBasedQueues)
			{
				fireAndForget = tuple.Item2.Peek();
			}
			fireAndForget.InvokeWrappedDelegate();
			lock (threadBasedQueues)
			{
				tuple.Item2.Dequeue();
				if (tuple.Item2.Count == 0)
				{
					threadBasedQueues.Remove(tuple.Item1);
					break;
				}
			}
		}
	}

	private FireAndForget(Delegate d, IEnumerable<object> args, Control threadMarshalControl)
	{
		this.d = d;
		argumentList = args.ToList().AsReadOnly();
		this.threadMarshalControl = threadMarshalControl;
	}

	private void InvokeWrappedDelegate()
	{
		if (d.Target != null && threadMarshalControl != null)
		{
			Delegate method = Delegate.CreateDelegate(d.GetType(), d.Target, d.Method.Name);
			threadMarshalControl.BeginInvoke(method, argumentList.ToArray());
		}
		else
		{
			d.Method.Invoke(d.Target, argumentList.ToArray());
		}
	}
}
