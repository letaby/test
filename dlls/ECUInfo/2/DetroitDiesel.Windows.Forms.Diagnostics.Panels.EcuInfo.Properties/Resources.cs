using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace DetroitDiesel.Windows.Forms.Diagnostics.Panels.EcuInfo.Properties;

[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
[DebuggerNonUserCode]
[CompilerGenerated]
internal class Resources
{
	private static ResourceManager resourceMan;

	private static CultureInfo resourceCulture;

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	internal static ResourceManager ResourceManager
	{
		get
		{
			if (resourceMan == null)
			{
				ResourceManager resourceManager = new ResourceManager("DetroitDiesel.Windows.Forms.Diagnostics.Panels.EcuInfo.Properties.Resources", typeof(Resources).Assembly);
				resourceMan = resourceManager;
			}
			return resourceMan;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	internal static CultureInfo Culture
	{
		get
		{
			return resourceCulture;
		}
		set
		{
			resourceCulture = value;
		}
	}

	internal static string EcuInfoViewControl_ShowMore => ResourceManager.GetString("EcuInfoViewControl_ShowMore", resourceCulture);

	internal static string GroupName_DeviceIdentification => ResourceManager.GetString("GroupName_DeviceIdentification", resourceCulture);

	internal static string PanelDescription => ResourceManager.GetString("PanelDescription", resourceCulture);

	internal static Bitmap PanelImage
	{
		get
		{
			object obj = ResourceManager.GetObject("PanelImage", resourceCulture);
			return (Bitmap)obj;
		}
	}

	internal static string PanelTitle => ResourceManager.GetString("PanelTitle", resourceCulture);

	internal static string TabPageText_Common => ResourceManager.GetString("TabPageText_Common", resourceCulture);

	internal static string TabPageText_Standard => ResourceManager.GetString("TabPageText_Standard", resourceCulture);

	internal static string TabPageText_StoredData => ResourceManager.GetString("TabPageText_StoredData", resourceCulture);

	internal Resources()
	{
	}
}
