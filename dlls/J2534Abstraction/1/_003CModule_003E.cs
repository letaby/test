// Decompiled with JetBrains decompiler
// Type: <Module>
// Assembly: J2534Abstraction, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: F558D3F4-6D07-4AE0-B148-E7AD8371AFDC
// Assembly location: C:\Users\petra\Downloads\Архив (2)\J2534Abstraction.dll

using \u003CCppImplementationDetails\u003E;
using \u003CCrtImplementationDetails\u003E;
using J2534;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Threading;

#nullable disable
internal class \u003CModule\u003E
{
  public static __FnPtr<int (void*, ulong, void*, ulong)> __m2mep\u0040\u003Fmemcpy_s\u0040\u003FA0xb03e8ad9\u0040\u0040\u0024\u0024J0YAHQEAX_KQEBX1\u0040Z;
  public static __FnPtr<void (uint, uint, void*, void*)> __m2mep\u0040\u003FNativeCallbackFunction\u0040\u003FA0xb03e8ad9\u0040\u0040\u0024\u0024FYAXKKPEAX0\u0040Z;
  public static __FnPtr<J2534Error (uint, uint, uint&)> __m2mep\u0040\u003FGetConfig\u0040\u0040\u0024\u0024FYM\u003FAW4J2534Error\u0040J2534\u0040\u0040KKAE\u0024CAK\u0040Z;
  public static __FnPtr<J2534Error (uint, uint, uint)> __m2mep\u0040\u003FSetConfig\u0040\u0040\u0024\u0024FYM\u003FAW4J2534Error\u0040J2534\u0040\u0040KKK\u0040Z;
  public static unsafe int** __unep\u0040\u003FNativeCallbackFunction\u0040\u003FA0xb03e8ad9\u0040\u0040\u0024\u0024FYAXKKPEAX0\u0040Z;
  internal static int __\u0040\u0040_PchSym_\u004000\u0040UfhvihUkfyorxUhlfixvUhzkrUhRzkrUhlfixvUqCFDEzyhgizxgrlmUcGEUivovzhvUhgwzucOlyq\u00404B2008FD98C1DD4;
  internal static __s_GUID _GUID_cb2f6723_ab3a_11d2_9c40_00c04fa30a3e;
  internal static __s_GUID _GUID_cb2f6722_ab3a_11d2_9c40_00c04fa30a3e;
  [FixedAddressValueType]
  internal static int \u003FUninitialized\u0040CurrentDomain\u0040\u003CCrtImplementationDetails\u003E\u0040\u0040\u0024\u0024Q2HA;
  internal static __FnPtr<void ()> \u003FA0xfe5e1e65\u002E\u003FUninitialized\u0024initializer\u0024\u0040CurrentDomain\u0040\u003CCrtImplementationDetails\u003E\u0040\u0040\u0024\u0024Q2P6MXXZEA;
  [FixedAddressValueType]
  internal static Progress \u003FInitializedNative\u0040CurrentDomain\u0040\u003CCrtImplementationDetails\u003E\u0040\u0040\u0024\u0024Q2W4Progress\u00402\u0040A;
  internal static __FnPtr<void ()> \u003FA0xfe5e1e65\u002E\u003FInitializedNative\u0024initializer\u0024\u0040CurrentDomain\u0040\u003CCrtImplementationDetails\u003E\u0040\u0040\u0024\u0024Q2P6MXXZEA;
  internal static __s_GUID _GUID_90f1a06c_7712_4762_86b5_7a5eba6bdb02;
  internal static __s_GUID _GUID_90f1a06e_7712_4762_86b5_7a5eba6bdb02;
  [FixedAddressValueType]
  internal static Progress \u003FInitializedPerAppDomain\u0040CurrentDomain\u0040\u003CCrtImplementationDetails\u003E\u0040\u0040\u0024\u0024Q2W4Progress\u00402\u0040A;
  internal static bool \u003FEntered\u0040DefaultDomain\u0040\u003CCrtImplementationDetails\u003E\u0040\u00402_NA;
  internal static TriBool \u003FhasNative\u0040DefaultDomain\u0040\u003CCrtImplementationDetails\u003E\u0040\u00400W4TriBool\u00402\u0040A;
  internal static bool \u003FInitializedPerProcess\u0040DefaultDomain\u0040\u003CCrtImplementationDetails\u003E\u0040\u00402_NA;
  internal static int \u003FCount\u0040AllDomains\u0040\u003CCrtImplementationDetails\u003E\u0040\u00402HA;
  [FixedAddressValueType]
  internal static int \u003FInitialized\u0040CurrentDomain\u0040\u003CCrtImplementationDetails\u003E\u0040\u0040\u0024\u0024Q2HA;
  internal static bool \u003FInitializedNativeFromCCTOR\u0040DefaultDomain\u0040\u003CCrtImplementationDetails\u003E\u0040\u00402_NA;
  [FixedAddressValueType]
  internal static bool \u003FIsDefaultDomain\u0040CurrentDomain\u0040\u003CCrtImplementationDetails\u003E\u0040\u0040\u0024\u0024Q2_NA;
  [FixedAddressValueType]
  internal static Progress \u003FInitializedVtables\u0040CurrentDomain\u0040\u003CCrtImplementationDetails\u003E\u0040\u0040\u0024\u0024Q2W4Progress\u00402\u0040A;
  internal static bool \u003FInitializedNative\u0040DefaultDomain\u0040\u003CCrtImplementationDetails\u003E\u0040\u00402_NA;
  [FixedAddressValueType]
  internal static Progress \u003FInitializedPerProcess\u0040CurrentDomain\u0040\u003CCrtImplementationDetails\u003E\u0040\u0040\u0024\u0024Q2W4Progress\u00402\u0040A;
  internal static TriBool \u003FhasPerProcess\u0040DefaultDomain\u0040\u003CCrtImplementationDetails\u003E\u0040\u00400W4TriBool\u00402\u0040A;
  internal static \u0024ArrayType\u0024\u0024\u0024BY00Q6MPEBXXZ __xc_mp_z;
  internal static \u0024ArrayType\u0024\u0024\u0024BY00Q6MPEBXXZ __xi_vt_z;
  internal static __FnPtr<void ()> \u003FA0xfe5e1e65\u002E\u003FInitializedPerProcess\u0024initializer\u0024\u0040CurrentDomain\u0040\u003CCrtImplementationDetails\u003E\u0040\u0040\u0024\u0024Q2P6MXXZEA;
  internal static \u0024ArrayType\u0024\u0024\u0024BY00Q6MPEBXXZ __xc_ma_a;
  internal static \u0024ArrayType\u0024\u0024\u0024BY00Q6MPEBXXZ __xc_ma_z;
  internal static __FnPtr<void ()> \u003FA0xfe5e1e65\u002E\u003FInitializedPerAppDomain\u0024initializer\u0024\u0040CurrentDomain\u0040\u003CCrtImplementationDetails\u003E\u0040\u0040\u0024\u0024Q2P6MXXZEA;
  internal static \u0024ArrayType\u0024\u0024\u0024BY00Q6MPEBXXZ __xi_vt_a;
  internal static __FnPtr<void ()> \u003FA0xfe5e1e65\u002E\u003FInitialized\u0024initializer\u0024\u0040CurrentDomain\u0040\u003CCrtImplementationDetails\u003E\u0040\u0040\u0024\u0024Q2P6MXXZEA;
  internal static \u0024ArrayType\u0024\u0024\u0024BY00Q6MPEBXXZ __xc_mp_a;
  internal static __FnPtr<void ()> \u003FA0xfe5e1e65\u002E\u003FInitializedVtables\u0024initializer\u0024\u0040CurrentDomain\u0040\u003CCrtImplementationDetails\u003E\u0040\u0040\u0024\u0024Q2P6MXXZEA;
  internal static __FnPtr<void ()> \u003FA0xfe5e1e65\u002E\u003FIsDefaultDomain\u0024initializer\u0024\u0040CurrentDomain\u0040\u003CCrtImplementationDetails\u003E\u0040\u0040\u0024\u0024Q2P6MXXZEA;
  public static __FnPtr<void (System.Exception, System.Exception)> __m2mep\u0040\u003FThrowNestedModuleLoadException\u0040\u003CCrtImplementationDetails\u003E\u0040\u0040\u0024\u0024FYMXPE\u0024AAVException\u0040System\u0040\u00400\u0040Z;
  public static __FnPtr<void (string)> __m2mep\u0040\u003FThrowModuleLoadException\u0040\u003CCrtImplementationDetails\u003E\u0040\u0040\u0024\u0024FYMXPE\u0024AAVString\u0040System\u0040\u0040\u0040Z;
  public static __FnPtr<void (string, System.Exception)> __m2mep\u0040\u003FThrowModuleLoadException\u0040\u003CCrtImplementationDetails\u003E\u0040\u0040\u0024\u0024FYMXPE\u0024AAVString\u0040System\u0040\u0040PE\u0024AAVException\u00403\u0040\u0040Z;
  public static __FnPtr<void (EventHandler)> __m2mep\u0040\u003FRegisterModuleUninitializer\u0040\u003CCrtImplementationDetails\u003E\u0040\u0040\u0024\u0024FYMXPE\u0024AAVEventHandler\u0040System\u0040\u0040\u0040Z;
  public static __FnPtr<Guid (_GUID*)> __m2mep\u0040\u003FFromGUID\u0040\u003CCrtImplementationDetails\u003E\u0040\u0040\u0024\u0024FYM\u003FAVGuid\u0040System\u0040\u0040AEBU_GUID\u0040\u0040\u0040Z;
  public static __FnPtr<int (IUnknown**)> __m2mep\u0040\u003F__get_default_appdomain\u0040\u0040\u0024\u0024FYAJPEAPEAUIUnknown\u0040\u0040\u0040Z;
  public static __FnPtr<void (IUnknown*)> __m2mep\u0040\u003F__release_appdomain\u0040\u0040\u0024\u0024FYAXPEAUIUnknown\u0040\u0040\u0040Z;
  public static __FnPtr<AppDomain ()> __m2mep\u0040\u003FGetDefaultDomain\u0040\u003CCrtImplementationDetails\u003E\u0040\u0040\u0024\u0024FYMPE\u0024AAVAppDomain\u0040System\u0040\u0040XZ;
  public static __FnPtr<void (__FnPtr<int (void*)>, void*)> __m2mep\u0040\u003FDoCallBackInDefaultDomain\u0040\u003CCrtImplementationDetails\u003E\u0040\u0040\u0024\u0024FYAXP6AJPEAX\u0040Z0\u0040Z;
  public static __FnPtr<bool ()> __m2mep\u0040\u003F__scrt_is_safe_for_managed_code\u0040\u0040\u0024\u0024FYA_NXZ;
  public static __FnPtr<int (void*)> __m2mep\u0040\u003FDoNothing\u0040DefaultDomain\u0040\u003CCrtImplementationDetails\u003E\u0040\u0040\u0024\u0024FCAJPEAX\u0040Z;
  public static __FnPtr<bool ()> __m2mep\u0040\u003FHasPerProcess\u0040DefaultDomain\u0040\u003CCrtImplementationDetails\u003E\u0040\u0040\u0024\u0024FSA_NXZ;
  public static __FnPtr<bool ()> __m2mep\u0040\u003FHasNative\u0040DefaultDomain\u0040\u003CCrtImplementationDetails\u003E\u0040\u0040\u0024\u0024FSA_NXZ;
  public static __FnPtr<bool ()> __m2mep\u0040\u003FNeedsInitialization\u0040DefaultDomain\u0040\u003CCrtImplementationDetails\u003E\u0040\u0040\u0024\u0024FSA_NXZ;
  public static __FnPtr<bool ()> __m2mep\u0040\u003FNeedsUninitialization\u0040DefaultDomain\u0040\u003CCrtImplementationDetails\u003E\u0040\u0040\u0024\u0024FSA_NXZ;
  public static __FnPtr<void ()> __m2mep\u0040\u003FInitialize\u0040DefaultDomain\u0040\u003CCrtImplementationDetails\u003E\u0040\u0040\u0024\u0024FSAXXZ;
  public static __FnPtr<void (LanguageSupport*)> __m2mep\u0040\u003FInitializeVtables\u0040LanguageSupport\u0040\u003CCrtImplementationDetails\u003E\u0040\u0040\u0024\u0024FAEAAXXZ;
  public static __FnPtr<void (LanguageSupport*)> __m2mep\u0040\u003FInitializeDefaultAppDomain\u0040LanguageSupport\u0040\u003CCrtImplementationDetails\u003E\u0040\u0040\u0024\u0024FAEAAXXZ;
  public static __FnPtr<void (LanguageSupport*)> __m2mep\u0040\u003FInitializeNative\u0040LanguageSupport\u0040\u003CCrtImplementationDetails\u003E\u0040\u0040\u0024\u0024FAEAAXXZ;
  public static __FnPtr<void (LanguageSupport*)> __m2mep\u0040\u003FInitializePerProcess\u0040LanguageSupport\u0040\u003CCrtImplementationDetails\u003E\u0040\u0040\u0024\u0024FAEAAXXZ;
  public static __FnPtr<void (LanguageSupport*)> __m2mep\u0040\u003FInitializePerAppDomain\u0040LanguageSupport\u0040\u003CCrtImplementationDetails\u003E\u0040\u0040\u0024\u0024FAEAAXXZ;
  public static __FnPtr<void (LanguageSupport*)> __m2mep\u0040\u003FInitializeUninitializer\u0040LanguageSupport\u0040\u003CCrtImplementationDetails\u003E\u0040\u0040\u0024\u0024FAEAAXXZ;
  public static __FnPtr<void (LanguageSupport*)> __m2mep\u0040\u003F_Initialize\u0040LanguageSupport\u0040\u003CCrtImplementationDetails\u003E\u0040\u0040\u0024\u0024FAEAAXXZ;
  public static __FnPtr<void ()> __m2mep\u0040\u003FUninitializeAppDomain\u0040LanguageSupport\u0040\u003CCrtImplementationDetails\u003E\u0040\u0040\u0024\u0024FCAXXZ;
  public static __FnPtr<int (void*)> __m2mep\u0040\u003F_UninitializeDefaultDomain\u0040LanguageSupport\u0040\u003CCrtImplementationDetails\u003E\u0040\u0040\u0024\u0024FCAJPEAX\u0040Z;
  public static __FnPtr<void ()> __m2mep\u0040\u003FUninitializeDefaultDomain\u0040LanguageSupport\u0040\u003CCrtImplementationDetails\u003E\u0040\u0040\u0024\u0024FCAXXZ;
  public static __FnPtr<void (object, EventArgs)> __m2mep\u0040\u003FDomainUnload\u0040LanguageSupport\u0040\u003CCrtImplementationDetails\u003E\u0040\u0040\u0024\u0024FCMXPE\u0024AAVObject\u0040System\u0040\u0040PE\u0024AAVEventArgs\u00404\u0040\u0040Z;
  public static __FnPtr<void (LanguageSupport*, System.Exception)> __m2mep\u0040\u003FCleanup\u0040LanguageSupport\u0040\u003CCrtImplementationDetails\u003E\u0040\u0040\u0024\u0024FAEAMXPE\u0024AAVException\u0040System\u0040\u0040\u0040Z;
  public static __FnPtr<LanguageSupport* (LanguageSupport*)> __m2mep\u0040\u003F\u003F0LanguageSupport\u0040\u003CCrtImplementationDetails\u003E\u0040\u0040\u0024\u0024FQEAA\u0040XZ;
  public static __FnPtr<void (LanguageSupport*)> __m2mep\u0040\u003F\u003F1LanguageSupport\u0040\u003CCrtImplementationDetails\u003E\u0040\u0040\u0024\u0024FQEAA\u0040XZ;
  public static __FnPtr<void (LanguageSupport*)> __m2mep\u0040\u003FInitialize\u0040LanguageSupport\u0040\u003CCrtImplementationDetails\u003E\u0040\u0040\u0024\u0024FQEAAXXZ;
  public static __FnPtr<void ()> __m2mep\u0040\u003F\u002Ecctor\u0040\u0040\u0024\u0024FYMXXZ;
  public static __FnPtr<string (gcroot\u003CSystem\u003A\u003AString\u0020\u005E\u003E*)> __m2mep\u0040\u003F\u003FB\u003F\u0024gcroot\u0040PE\u0024AAVString\u0040System\u0040\u0040\u0040\u0040\u0024\u0024FQEBMPE\u0024AAVString\u0040System\u0040\u0040XZ;
  public static __FnPtr<gcroot\u003CSystem\u003A\u003AString\u0020\u005E\u003E* (gcroot\u003CSystem\u003A\u003AString\u0020\u005E\u003E*, string)> __m2mep\u0040\u003F\u003F4\u003F\u0024gcroot\u0040PE\u0024AAVString\u0040System\u0040\u0040\u0040\u0040\u0024\u0024FQEAMAEAU0\u0040PE\u0024AAVString\u0040System\u0040\u0040\u0040Z;
  public static __FnPtr<void (gcroot\u003CSystem\u003A\u003AString\u0020\u005E\u003E*)> __m2mep\u0040\u003F\u003F1\u003F\u0024gcroot\u0040PE\u0024AAVString\u0040System\u0040\u0040\u0040\u0040\u0024\u0024FQEAA\u0040XZ;
  public static __FnPtr<gcroot\u003CSystem\u003A\u003AString\u0020\u005E\u003E* (gcroot\u003CSystem\u003A\u003AString\u0020\u005E\u003E*)> __m2mep\u0040\u003F\u003F0\u003F\u0024gcroot\u0040PE\u0024AAVString\u0040System\u0040\u0040\u0040\u0040\u0024\u0024FQEAA\u0040XZ;
  public static unsafe int** __unep\u0040\u003FDoNothing\u0040DefaultDomain\u0040\u003CCrtImplementationDetails\u003E\u0040\u0040\u0024\u0024FCAJPEAX\u0040Z;
  public static unsafe int** __unep\u0040\u003F_UninitializeDefaultDomain\u0040LanguageSupport\u0040\u003CCrtImplementationDetails\u003E\u0040\u0040\u0024\u0024FCAJPEAX\u0040Z;
  internal static unsafe __FnPtr<void ()>* \u003FA0xca239242\u002E__onexitbegin_m;
  internal static ulong \u003FA0xca239242\u002E__exit_list_size;
  [FixedAddressValueType]
  internal static unsafe __FnPtr<void ()>* __onexitend_app_domain;
  [FixedAddressValueType]
  internal static unsafe void* \u003F_lock\u0040AtExitLock\u0040\u003CCrtImplementationDetails\u003E\u0040\u0040\u0024\u0024Q0PEAXEA;
  [FixedAddressValueType]
  internal static int \u003F_ref_count\u0040AtExitLock\u0040\u003CCrtImplementationDetails\u003E\u0040\u0040\u0024\u0024Q0HA;
  internal static unsafe __FnPtr<void ()>* \u003FA0xca239242\u002E__onexitend_m;
  [FixedAddressValueType]
  internal static ulong __exit_list_size_app_domain;
  [FixedAddressValueType]
  internal static unsafe __FnPtr<void ()>* __onexitbegin_app_domain;
  public static __FnPtr<ValueType ()> __m2mep\u0040\u003F_handle\u0040AtExitLock\u0040\u003CCrtImplementationDetails\u003E\u0040\u0040\u0024\u0024FCMPE\u0024AA__ZVGCHandle\u0040InteropServices\u0040Runtime\u0040System\u0040\u0040XZ;
  public static __FnPtr<void (object)> __m2mep\u0040\u003F_lock_Construct\u0040AtExitLock\u0040\u003CCrtImplementationDetails\u003E\u0040\u0040\u0024\u0024FCMXPE\u0024AAVObject\u0040System\u0040\u0040\u0040Z;
  public static __FnPtr<void (object)> __m2mep\u0040\u003F_lock_Set\u0040AtExitLock\u0040\u003CCrtImplementationDetails\u003E\u0040\u0040\u0024\u0024FCMXPE\u0024AAVObject\u0040System\u0040\u0040\u0040Z;
  public static __FnPtr<object ()> __m2mep\u0040\u003F_lock_Get\u0040AtExitLock\u0040\u003CCrtImplementationDetails\u003E\u0040\u0040\u0024\u0024FCMPE\u0024AAVObject\u0040System\u0040\u0040XZ;
  public static __FnPtr<void ()> __m2mep\u0040\u003F_lock_Destruct\u0040AtExitLock\u0040\u003CCrtImplementationDetails\u003E\u0040\u0040\u0024\u0024FCAXXZ;
  public static __FnPtr<bool ()> __m2mep\u0040\u003FIsInitialized\u0040AtExitLock\u0040\u003CCrtImplementationDetails\u003E\u0040\u0040\u0024\u0024FSA_NXZ;
  public static __FnPtr<void ()> __m2mep\u0040\u003FAddRef\u0040AtExitLock\u0040\u003CCrtImplementationDetails\u003E\u0040\u0040\u0024\u0024FSAXXZ;
  public static __FnPtr<void ()> __m2mep\u0040\u003FRemoveRef\u0040AtExitLock\u0040\u003CCrtImplementationDetails\u003E\u0040\u0040\u0024\u0024FSAXXZ;
  public static __FnPtr<void ()> __m2mep\u0040\u003FEnter\u0040AtExitLock\u0040\u003CCrtImplementationDetails\u003E\u0040\u0040\u0024\u0024FSAXXZ;
  public static __FnPtr<void ()> __m2mep\u0040\u003FExit\u0040AtExitLock\u0040\u003CCrtImplementationDetails\u003E\u0040\u0040\u0024\u0024FSAXXZ;
  public static __FnPtr<bool ()> __m2mep\u0040\u003F__global_lock\u0040\u003FA0xca239242\u0040\u0040\u0024\u0024FYA_NXZ;
  public static __FnPtr<bool ()> __m2mep\u0040\u003F__global_unlock\u0040\u003FA0xca239242\u0040\u0040\u0024\u0024FYA_NXZ;
  public static __FnPtr<bool ()> __m2mep\u0040\u003F__alloc_global_lock\u0040\u003FA0xca239242\u0040\u0040\u0024\u0024FYA_NXZ;
  public static __FnPtr<void ()> __m2mep\u0040\u003F__dealloc_global_lock\u0040\u003FA0xca239242\u0040\u0040\u0024\u0024FYAXXZ;
  public static __FnPtr<int (__FnPtr<void ()>, ulong*, __FnPtr<void ()>**, __FnPtr<void ()>**)> __m2mep\u0040\u003F_atexit_helper\u0040\u0040\u0024\u0024J0YMHP6MXXZPEA_KPEAPEAP6MXXZ2\u0040Z;
  public static __FnPtr<void ()> __m2mep\u0040\u003F_exit_callback\u0040\u0040\u0024\u0024J0YMXXZ;
  public static __FnPtr<int ()> __m2mep\u0040\u003F_initatexit_m\u0040\u0040\u0024\u0024J0YMHXZ;
  public static __FnPtr<__FnPtr<int ()> (__FnPtr<int ()>)> __m2mep\u0040\u003F_onexit_m\u0040\u0040\u0024\u0024J0YMP6MHXZP6MHXZ\u0040Z;
  public static __FnPtr<int (__FnPtr<void ()>)> __m2mep\u0040\u003F_atexit_m\u0040\u0040\u0024\u0024J0YMHP6MXXZ\u0040Z;
  public static __FnPtr<int ()> __m2mep\u0040\u003F_initatexit_app_domain\u0040\u0040\u0024\u0024J0YMHXZ;
  public static __FnPtr<void ()> __m2mep\u0040\u003F_app_exit_callback\u0040\u0040\u0024\u0024J0YMXXZ;
  public static __FnPtr<__FnPtr<int ()> (__FnPtr<int ()>)> __m2mep\u0040\u003F_onexit_m_appdomain\u0040\u0040\u0024\u0024J0YMP6MHXZP6MHXZ\u0040Z;
  public static __FnPtr<int (__FnPtr<void ()>)> __m2mep\u0040\u003F_atexit_m_appdomain\u0040\u0040\u0024\u0024J0YMHP6MXXZ\u0040Z;
  public static __FnPtr<int (__FnPtr<int ()>*, __FnPtr<int ()>*)> __m2mep\u0040\u003F_initterm_e\u0040\u0040\u0024\u0024FYMHPEAP6AHXZ0\u0040Z;
  public static __FnPtr<void (__FnPtr<void ()>*, __FnPtr<void ()>*)> __m2mep\u0040\u003F_initterm\u0040\u0040\u0024\u0024FYMXPEAP6AXXZ0\u0040Z;
  public static __FnPtr<ModuleHandle ()> __m2mep\u0040\u003FHandle\u0040ThisModule\u0040\u003CCrtImplementationDetails\u003E\u0040\u0040\u0024\u0024FCM\u003FAVModuleHandle\u0040System\u0040\u0040XZ;
  public static __FnPtr<void (__FnPtr<void* ()>*, __FnPtr<void* ()>*)> __m2mep\u0040\u003F_initterm_m\u0040\u0040\u0024\u0024FYMXPEBQ6MPEBXXZ0\u0040Z;
  public static __FnPtr<__FnPtr<void* ()> (__FnPtr<void* ()>)> __m2mep\u0040\u003F\u003F\u0024ResolveMethod\u0040\u0024\u0024A6MPEBXXZ\u0040ThisModule\u0040\u003CCrtImplementationDetails\u003E\u0040\u0040\u0024\u0024FSMP6MPEBXXZP6MPEBXXZ\u0040Z;
  public static __FnPtr<void (__FnPtr<void (void*)>, void*)> __m2mep\u0040\u003F___CxxCallUnwindDtor\u0040\u0040\u0024\u0024J0YMXP6MXPEAX\u0040Z0\u0040Z;
  public static __FnPtr<void (__FnPtr<void (void*)>, void*)> __m2mep\u0040\u003F___CxxCallUnwindDelDtor\u0040\u0040\u0024\u0024J0YMXP6MXPEAX\u0040Z0\u0040Z;
  public static __FnPtr<void (__FnPtr<void (void*, ulong, int, __FnPtr<void (void*)>)>, void*, ulong, int, __FnPtr<void (void*)>)> __m2mep\u0040\u003F___CxxCallUnwindVecDtor\u0040\u0040\u0024\u0024J0YMXP6MXPEAX_KHP6MX0\u0040Z\u0040Z01H2\u0040Z;
  internal static \u0024ArrayType\u0024\u0024\u0024BY0A\u0040P6AHXZ __xi_z;
  internal static __scrt_native_startup_state __scrt_current_native_startup_state;
  internal static unsafe void* __scrt_native_startup_lock;
  internal static \u0024ArrayType\u0024\u0024\u0024BY0A\u0040P6AXXZ __xc_a;
  internal static \u0024ArrayType\u0024\u0024\u0024BY0A\u0040P6AHXZ __xi_a;
  internal static uint __scrt_native_dllmain_reason;
  internal static \u0024ArrayType\u0024\u0024\u0024BY0A\u0040P6AXXZ __xc_z;

  internal static unsafe int \u003FA0xb03e8ad9\u002Ememcpy_s(
    void* _Destination,
    ulong _DestinationSize,
    void* _Source,
    ulong _SourceSize)
  {
    if (_SourceSize == 0UL)
      return 0;
    if ((IntPtr) _Destination == IntPtr.Zero)
    {
      *\u003CModule\u003E._errno() = 22;
      \u003CModule\u003E._invalid_parameter_noinfo();
      return 22;
    }
    if ((IntPtr) _Source != IntPtr.Zero && _DestinationSize >= _SourceSize)
    {
      // ISSUE: cpblk instruction
      __memcpy((IntPtr) _Destination, (IntPtr) _Source, (long) _SourceSize);
      return 0;
    }
    // ISSUE: initblk instruction
    __memset((IntPtr) _Destination, 0, (long) _DestinationSize);
    if ((IntPtr) _Source == IntPtr.Zero)
    {
      *\u003CModule\u003E._errno() = 22;
      \u003CModule\u003E._invalid_parameter_noinfo();
      return 22;
    }
    if (_DestinationSize >= _SourceSize)
      return 22;
    *\u003CModule\u003E._errno() = 34;
    \u003CModule\u003E._invalid_parameter_noinfo();
    return 34;
  }

  internal static unsafe void \u003FA0xb03e8ad9\u002ENativeCallbackFunction(
    uint ChannelId,
    uint eventtype,
    void* Tag,
    void* ExtendedData)
  {
    PassThruMsg message = new PassThruMsg((PASSTHRU_MSG*) ExtendedData);
    Sid.PassthruCallback[ChannelId](message);
  }

  internal static unsafe J2534Error GetConfig(uint channelId, uint ioctlId, ref uint result)
  {
    SCONFIG sconfig;
    // ISSUE: cast to a reference type
    // ISSUE: explicit reference operation
    ^(int&) ref sconfig = (int) ioctlId;
    // ISSUE: cast to a reference type
    // ISSUE: explicit reference operation
    ^(int&) ((IntPtr) &sconfig + 4) = 0;
    SCONFIG_LIST sconfigList;
    // ISSUE: cast to a reference type
    // ISSUE: explicit reference operation
    ^(int&) ref sconfigList = 1;
    // ISSUE: cast to a reference type
    // ISSUE: explicit reference operation
    ^(long&) ((IntPtr) &sconfigList + 4) = (long) &sconfig;
    J2534Error config = (J2534Error) \u003CModule\u003E.PassThruIoctl(channelId, 1U, (void*) &sconfigList, (void*) 0L);
    if (config == J2534Error.NoError)
    {
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      result = (uint) ^(int&) ((IntPtr) &sconfig + 4);
    }
    return config;
  }

  internal static unsafe J2534Error SetConfig(uint channelId, uint parameter, uint value)
  {
    SCONFIG sconfig;
    // ISSUE: cast to a reference type
    // ISSUE: explicit reference operation
    ^(int&) ref sconfig = (int) parameter;
    // ISSUE: cast to a reference type
    // ISSUE: explicit reference operation
    ^(int&) ((IntPtr) &sconfig + 4) = (int) value;
    SCONFIG_LIST sconfigList;
    // ISSUE: cast to a reference type
    // ISSUE: explicit reference operation
    ^(int&) ref sconfigList = 1;
    // ISSUE: cast to a reference type
    // ISSUE: explicit reference operation
    ^(long&) ((IntPtr) &sconfigList + 4) = (long) &sconfig;
    return (J2534Error) \u003CModule\u003E.PassThruIoctl(channelId, 2U, (void*) &sconfigList, (void*) 0L);
  }

  internal static void \u003CCrtImplementationDetails\u003E\u002EThrowNestedModuleLoadException(
    System.Exception innerException,
    System.Exception nestedException)
  {
    throw new ModuleLoadExceptionHandlerException("A nested exception occurred after the primary exception that caused the C++ module to fail to load.\n", innerException, nestedException);
  }

  internal static void \u003CCrtImplementationDetails\u003E\u002EThrowModuleLoadException(
    string errorMessage)
  {
    throw new ModuleLoadException(errorMessage);
  }

  internal static void \u003CCrtImplementationDetails\u003E\u002EThrowModuleLoadException(
    string errorMessage,
    System.Exception innerException)
  {
    throw new ModuleLoadException(errorMessage, innerException);
  }

  internal static void \u003CCrtImplementationDetails\u003E\u002ERegisterModuleUninitializer(
    EventHandler handler)
  {
    ModuleUninitializer._ModuleUninitializer.AddHandler(handler);
  }

  [SecuritySafeCritical]
  internal static unsafe Guid \u003CCrtImplementationDetails\u003E\u002EFromGUID(_GUID* guid)
  {
    return new Guid((uint) *(int*) guid, *(ushort*) ((IntPtr) guid + 4L), *(ushort*) ((IntPtr) guid + 6L), *(byte*) ((IntPtr) guid + 8L), *(byte*) ((IntPtr) guid + 9L), *(byte*) ((IntPtr) guid + 10L), *(byte*) ((IntPtr) guid + 11L), *(byte*) ((IntPtr) guid + 12L), *(byte*) ((IntPtr) guid + 13L), *(byte*) ((IntPtr) guid + 14L), *(byte*) ((IntPtr) guid + 15L));
  }

  [SecurityCritical]
  internal static unsafe int __get_default_appdomain(IUnknown** ppUnk)
  {
    ICorRuntimeHost* icorRuntimeHostPtr1 = (ICorRuntimeHost*) 0L;
    int defaultAppdomain;
    try
    {
      Guid riid = \u003CModule\u003E.\u003CCrtImplementationDetails\u003E\u002EFromGUID((_GUID*) &\u003CModule\u003E._GUID_cb2f6722_ab3a_11d2_9c40_00c04fa30a3e);
      icorRuntimeHostPtr1 = (ICorRuntimeHost*) RuntimeEnvironment.GetRuntimeInterfaceAsIntPtr(\u003CModule\u003E.\u003CCrtImplementationDetails\u003E\u002EFromGUID((_GUID*) &\u003CModule\u003E._GUID_cb2f6723_ab3a_11d2_9c40_00c04fa30a3e), riid).ToPointer();
      goto label_4;
    }
    catch (System.Exception ex)
    {
      defaultAppdomain = Marshal.GetHRForException(ex);
    }
    if (defaultAppdomain < 0)
      goto label_5;
label_4:
    ICorRuntimeHost* icorRuntimeHostPtr2 = icorRuntimeHostPtr1;
    IUnknown** iunknownPtr = ppUnk;
    // ISSUE: cast to a function pointer type
    // ISSUE: function pointer call
    defaultAppdomain = __calli((__FnPtr<int (IntPtr, IUnknown**)>) *(long*) (*(long*) icorRuntimeHostPtr1 + 104L))((IntPtr) icorRuntimeHostPtr2, iunknownPtr);
    ICorRuntimeHost* icorRuntimeHostPtr3 = icorRuntimeHostPtr1;
    // ISSUE: cast to a function pointer type
    // ISSUE: function pointer call
    int num = (int) __calli((__FnPtr<uint (IntPtr)>) *(long*) (*(long*) icorRuntimeHostPtr3 + 16L /*0x10*/))((IntPtr) icorRuntimeHostPtr3);
label_5:
    return defaultAppdomain;
  }

  internal static unsafe void __release_appdomain(IUnknown* ppUnk)
  {
    IUnknown* iunknownPtr = ppUnk;
    // ISSUE: cast to a function pointer type
    // ISSUE: function pointer call
    int num = (int) __calli((__FnPtr<uint (IntPtr)>) *(long*) (*(long*) iunknownPtr + 16L /*0x10*/))((IntPtr) iunknownPtr);
  }

  [SecurityCritical]
  internal static unsafe AppDomain \u003CCrtImplementationDetails\u003E\u002EGetDefaultDomain()
  {
    IUnknown* ppUnk = (IUnknown*) 0L;
    int defaultAppdomain = \u003CModule\u003E.__get_default_appdomain(&ppUnk);
    if (defaultAppdomain >= 0)
    {
      try
      {
        return (AppDomain) Marshal.GetObjectForIUnknown(new IntPtr((void*) ppUnk));
      }
      finally
      {
        \u003CModule\u003E.__release_appdomain(ppUnk);
      }
    }
    else
    {
      Marshal.ThrowExceptionForHR(defaultAppdomain);
      return (AppDomain) null;
    }
  }

  [SecurityCritical]
  internal static unsafe void \u003CCrtImplementationDetails\u003E\u002EDoCallBackInDefaultDomain(
    __FnPtr<int (void*)> function,
    void* cookie)
  {
    Guid riid = \u003CModule\u003E.\u003CCrtImplementationDetails\u003E\u002EFromGUID((_GUID*) &\u003CModule\u003E._GUID_90f1a06c_7712_4762_86b5_7a5eba6bdb02);
    ICLRRuntimeHost* pointer = (ICLRRuntimeHost*) RuntimeEnvironment.GetRuntimeInterfaceAsIntPtr(\u003CModule\u003E.\u003CCrtImplementationDetails\u003E\u002EFromGUID((_GUID*) &\u003CModule\u003E._GUID_90f1a06e_7712_4762_86b5_7a5eba6bdb02), riid).ToPointer();
    try
    {
      AppDomain defaultDomain = \u003CModule\u003E.\u003CCrtImplementationDetails\u003E\u002EGetDefaultDomain();
      ICLRRuntimeHost* iclrRuntimeHostPtr = pointer;
      int id = defaultDomain.Id;
      __FnPtr<int (void*)> local = function;
      void* voidPtr = cookie;
      // ISSUE: cast to a function pointer type
      // ISSUE: function pointer call
      int errorCode = __calli((__FnPtr<int (IntPtr, uint, __FnPtr<int (void*)>, void*)>) *(long*) (*(long*) pointer + 64L /*0x40*/))((IntPtr) iclrRuntimeHostPtr, (uint) id, local, voidPtr);
      if (errorCode >= 0)
        return;
      Marshal.ThrowExceptionForHR(errorCode);
    }
    finally
    {
      ICLRRuntimeHost* iclrRuntimeHostPtr = pointer;
      // ISSUE: cast to a function pointer type
      // ISSUE: function pointer call
      int num = (int) __calli((__FnPtr<uint (IntPtr)>) *(long*) (*(long*) iclrRuntimeHostPtr + 16L /*0x10*/))((IntPtr) iclrRuntimeHostPtr);
    }
  }

  [return: MarshalAs(UnmanagedType.U1)]
  internal static bool __scrt_is_safe_for_managed_code()
  {
    return \u003CModule\u003E.__scrt_native_dllmain_reason > 1U;
  }

  [SecuritySafeCritical]
  internal static unsafe int \u003CCrtImplementationDetails\u003E\u002EDefaultDomain\u002EDoNothing(
    void* cookie)
  {
    GC.KeepAlive((object) int.MaxValue);
    return 0;
  }

  [SecuritySafeCritical]
  [return: MarshalAs(UnmanagedType.U1)]
  internal static unsafe bool \u003CCrtImplementationDetails\u003E\u002EDefaultDomain\u002EHasPerProcess()
  {
    if (\u003CModule\u003E.\u003FhasPerProcess\u0040DefaultDomain\u0040\u003CCrtImplementationDetails\u003E\u0040\u00400W4TriBool\u00402\u0040A != (TriBool) 2)
      return \u003CModule\u003E.\u003FhasPerProcess\u0040DefaultDomain\u0040\u003CCrtImplementationDetails\u003E\u0040\u00400W4TriBool\u00402\u0040A == (TriBool) -1;
    void** voidPtr = (void**) &\u003CModule\u003E.__xc_mp_a;
    if (ref \u003CModule\u003E.__xc_mp_a < ref \u003CModule\u003E.__xc_mp_z)
    {
      while (*(long*) voidPtr == 0L)
      {
        voidPtr += 8L;
        if ((IntPtr) voidPtr >= ref \u003CModule\u003E.__xc_mp_z)
          goto label_5;
      }
      \u003CModule\u003E.\u003FhasPerProcess\u0040DefaultDomain\u0040\u003CCrtImplementationDetails\u003E\u0040\u00400W4TriBool\u00402\u0040A = (TriBool) -1;
      return true;
    }
label_5:
    \u003CModule\u003E.\u003FhasPerProcess\u0040DefaultDomain\u0040\u003CCrtImplementationDetails\u003E\u0040\u00400W4TriBool\u00402\u0040A = (TriBool) 0;
    return false;
  }

  [SecuritySafeCritical]
  [return: MarshalAs(UnmanagedType.U1)]
  internal static unsafe bool \u003CCrtImplementationDetails\u003E\u002EDefaultDomain\u002EHasNative()
  {
    if (\u003CModule\u003E.\u003FhasNative\u0040DefaultDomain\u0040\u003CCrtImplementationDetails\u003E\u0040\u00400W4TriBool\u00402\u0040A != (TriBool) 2)
      return \u003CModule\u003E.\u003FhasNative\u0040DefaultDomain\u0040\u003CCrtImplementationDetails\u003E\u0040\u00400W4TriBool\u00402\u0040A == (TriBool) -1;
    void** voidPtr1 = (void**) &\u003CModule\u003E.__xi_a;
    if (ref \u003CModule\u003E.__xi_a < ref \u003CModule\u003E.__xi_z)
    {
      while (*(long*) voidPtr1 == 0L)
      {
        voidPtr1 += 8L;
        if ((IntPtr) voidPtr1 >= ref \u003CModule\u003E.__xi_z)
          goto label_5;
      }
      \u003CModule\u003E.\u003FhasNative\u0040DefaultDomain\u0040\u003CCrtImplementationDetails\u003E\u0040\u00400W4TriBool\u00402\u0040A = (TriBool) -1;
      return true;
    }
label_5:
    void** voidPtr2 = (void**) &\u003CModule\u003E.__xc_a;
    if (ref \u003CModule\u003E.__xc_a < ref \u003CModule\u003E.__xc_z)
    {
      while (*(long*) voidPtr2 == 0L)
      {
        voidPtr2 += 8L;
        if ((IntPtr) voidPtr2 >= ref \u003CModule\u003E.__xc_z)
          goto label_9;
      }
      \u003CModule\u003E.\u003FhasNative\u0040DefaultDomain\u0040\u003CCrtImplementationDetails\u003E\u0040\u00400W4TriBool\u00402\u0040A = (TriBool) -1;
      return true;
    }
label_9:
    \u003CModule\u003E.\u003FhasNative\u0040DefaultDomain\u0040\u003CCrtImplementationDetails\u003E\u0040\u00400W4TriBool\u00402\u0040A = (TriBool) 0;
    return false;
  }

  [SecuritySafeCritical]
  [return: MarshalAs(UnmanagedType.U1)]
  internal static bool \u003CCrtImplementationDetails\u003E\u002EDefaultDomain\u002ENeedsInitialization()
  {
    return \u003CModule\u003E.\u003CCrtImplementationDetails\u003E\u002EDefaultDomain\u002EHasPerProcess() && !\u003CModule\u003E.\u003FInitializedPerProcess\u0040DefaultDomain\u0040\u003CCrtImplementationDetails\u003E\u0040\u00402_NA || \u003CModule\u003E.\u003CCrtImplementationDetails\u003E\u002EDefaultDomain\u002EHasNative() && !\u003CModule\u003E.\u003FInitializedNative\u0040DefaultDomain\u0040\u003CCrtImplementationDetails\u003E\u0040\u00402_NA && \u003CModule\u003E.__scrt_current_native_startup_state == (__scrt_native_startup_state) 0;
  }

  [return: MarshalAs(UnmanagedType.U1)]
  internal static bool \u003CCrtImplementationDetails\u003E\u002EDefaultDomain\u002ENeedsUninitialization()
  {
    return \u003CModule\u003E.\u003FEntered\u0040DefaultDomain\u0040\u003CCrtImplementationDetails\u003E\u0040\u00402_NA;
  }

  [SecurityCritical]
  internal static unsafe void \u003CCrtImplementationDetails\u003E\u002EDefaultDomain\u002EInitialize()
  {
    // ISSUE: cast to a function pointer type
    \u003CModule\u003E.\u003CCrtImplementationDetails\u003E\u002EDoCallBackInDefaultDomain((__FnPtr<int (void*)>) (IntPtr) \u003CModule\u003E.__unep\u0040\u003FDoNothing\u0040DefaultDomain\u0040\u003CCrtImplementationDetails\u003E\u0040\u0040\u0024\u0024FCAJPEAX\u0040Z, (void*) 0L);
  }

  internal static void \u003FA0xfe5e1e65\u002E\u003F\u003F__E\u003FInitialized\u0040CurrentDomain\u0040\u003CCrtImplementationDetails\u003E\u0040\u0040\u0024\u0024Q2HA\u0040\u0040YMXXZ()
  {
    \u003CModule\u003E.\u003FInitialized\u0040CurrentDomain\u0040\u003CCrtImplementationDetails\u003E\u0040\u0040\u0024\u0024Q2HA = 0;
  }

  internal static void \u003FA0xfe5e1e65\u002E\u003F\u003F__E\u003FUninitialized\u0040CurrentDomain\u0040\u003CCrtImplementationDetails\u003E\u0040\u0040\u0024\u0024Q2HA\u0040\u0040YMXXZ()
  {
    \u003CModule\u003E.\u003FUninitialized\u0040CurrentDomain\u0040\u003CCrtImplementationDetails\u003E\u0040\u0040\u0024\u0024Q2HA = 0;
  }

  internal static void \u003FA0xfe5e1e65\u002E\u003F\u003F__E\u003FIsDefaultDomain\u0040CurrentDomain\u0040\u003CCrtImplementationDetails\u003E\u0040\u0040\u0024\u0024Q2_NA\u0040\u0040YMXXZ()
  {
    \u003CModule\u003E.\u003FIsDefaultDomain\u0040CurrentDomain\u0040\u003CCrtImplementationDetails\u003E\u0040\u0040\u0024\u0024Q2_NA = false;
  }

  internal static void \u003FA0xfe5e1e65\u002E\u003F\u003F__E\u003FInitializedVtables\u0040CurrentDomain\u0040\u003CCrtImplementationDetails\u003E\u0040\u0040\u0024\u0024Q2W4Progress\u00402\u0040A\u0040\u0040YMXXZ()
  {
    \u003CModule\u003E.\u003FInitializedVtables\u0040CurrentDomain\u0040\u003CCrtImplementationDetails\u003E\u0040\u0040\u0024\u0024Q2W4Progress\u00402\u0040A = (Progress) 0;
  }

  internal static void \u003FA0xfe5e1e65\u002E\u003F\u003F__E\u003FInitializedNative\u0040CurrentDomain\u0040\u003CCrtImplementationDetails\u003E\u0040\u0040\u0024\u0024Q2W4Progress\u00402\u0040A\u0040\u0040YMXXZ()
  {
    \u003CModule\u003E.\u003FInitializedNative\u0040CurrentDomain\u0040\u003CCrtImplementationDetails\u003E\u0040\u0040\u0024\u0024Q2W4Progress\u00402\u0040A = (Progress) 0;
  }

  internal static void \u003FA0xfe5e1e65\u002E\u003F\u003F__E\u003FInitializedPerProcess\u0040CurrentDomain\u0040\u003CCrtImplementationDetails\u003E\u0040\u0040\u0024\u0024Q2W4Progress\u00402\u0040A\u0040\u0040YMXXZ()
  {
    \u003CModule\u003E.\u003FInitializedPerProcess\u0040CurrentDomain\u0040\u003CCrtImplementationDetails\u003E\u0040\u0040\u0024\u0024Q2W4Progress\u00402\u0040A = (Progress) 0;
  }

  internal static void \u003FA0xfe5e1e65\u002E\u003F\u003F__E\u003FInitializedPerAppDomain\u0040CurrentDomain\u0040\u003CCrtImplementationDetails\u003E\u0040\u0040\u0024\u0024Q2W4Progress\u00402\u0040A\u0040\u0040YMXXZ()
  {
    \u003CModule\u003E.\u003FInitializedPerAppDomain\u0040CurrentDomain\u0040\u003CCrtImplementationDetails\u003E\u0040\u0040\u0024\u0024Q2W4Progress\u00402\u0040A = (Progress) 0;
  }

  [SecuritySafeCritical]
  [DebuggerStepThrough]
  internal static unsafe void \u003CCrtImplementationDetails\u003E\u002ELanguageSupport\u002EInitializeVtables(
    [In] LanguageSupport* obj0)
  {
    \u003CModule\u003E.gcroot\u003CSystem\u003A\u003AString\u0020\u005E\u003E\u002E\u003D((gcroot\u003CSystem\u003A\u003AString\u0020\u005E\u003E*) obj0, "The C++ module failed to load during vtable initialization.\n");
    \u003CModule\u003E.\u003FInitializedVtables\u0040CurrentDomain\u0040\u003CCrtImplementationDetails\u003E\u0040\u0040\u0024\u0024Q2W4Progress\u00402\u0040A = (Progress) 1;
    \u003CModule\u003E._initterm_m((__FnPtr<void* ()>*) &\u003CModule\u003E.__xi_vt_a, (__FnPtr<void* ()>*) &\u003CModule\u003E.__xi_vt_z);
    \u003CModule\u003E.\u003FInitializedVtables\u0040CurrentDomain\u0040\u003CCrtImplementationDetails\u003E\u0040\u0040\u0024\u0024Q2W4Progress\u00402\u0040A = (Progress) 2;
  }

  [SecuritySafeCritical]
  internal static unsafe void \u003CCrtImplementationDetails\u003E\u002ELanguageSupport\u002EInitializeDefaultAppDomain(
    [In] LanguageSupport* obj0)
  {
    \u003CModule\u003E.gcroot\u003CSystem\u003A\u003AString\u0020\u005E\u003E\u002E\u003D((gcroot\u003CSystem\u003A\u003AString\u0020\u005E\u003E*) obj0, "The C++ module failed to load while attempting to initialize the default appdomain.\n");
    \u003CModule\u003E.\u003CCrtImplementationDetails\u003E\u002EDefaultDomain\u002EInitialize();
  }

  [SecuritySafeCritical]
  [DebuggerStepThrough]
  internal static unsafe void \u003CCrtImplementationDetails\u003E\u002ELanguageSupport\u002EInitializeNative(
    [In] LanguageSupport* obj0)
  {
    \u003CModule\u003E.gcroot\u003CSystem\u003A\u003AString\u0020\u005E\u003E\u002E\u003D((gcroot\u003CSystem\u003A\u003AString\u0020\u005E\u003E*) obj0, "The C++ module failed to load during native initialization.\n");
    \u003CModule\u003E.__security_init_cookie();
    \u003CModule\u003E.\u003FInitializedNative\u0040DefaultDomain\u0040\u003CCrtImplementationDetails\u003E\u0040\u00402_NA = true;
    if (!\u003CModule\u003E.__scrt_is_safe_for_managed_code())
      \u003CModule\u003E.abort();
    if (\u003CModule\u003E.__scrt_current_native_startup_state == (__scrt_native_startup_state) 1)
      \u003CModule\u003E.abort();
    if (\u003CModule\u003E.__scrt_current_native_startup_state != (__scrt_native_startup_state) 0)
      return;
    \u003CModule\u003E.\u003FInitializedNative\u0040CurrentDomain\u0040\u003CCrtImplementationDetails\u003E\u0040\u0040\u0024\u0024Q2W4Progress\u00402\u0040A = (Progress) 1;
    \u003CModule\u003E.__scrt_current_native_startup_state = (__scrt_native_startup_state) 1;
    if (\u003CModule\u003E._initterm_e((__FnPtr<int ()>*) &\u003CModule\u003E.__xi_a, (__FnPtr<int ()>*) &\u003CModule\u003E.__xi_z) != 0)
      \u003CModule\u003E.\u003CCrtImplementationDetails\u003E\u002EThrowModuleLoadException(\u003CModule\u003E.gcroot\u003CSystem\u003A\u003AString\u0020\u005E\u003E\u002E\u002EPE\u0024AAVString\u0040System\u0040\u0040((gcroot\u003CSystem\u003A\u003AString\u0020\u005E\u003E*) obj0));
    \u003CModule\u003E._initterm((__FnPtr<void ()>*) &\u003CModule\u003E.__xc_a, (__FnPtr<void ()>*) &\u003CModule\u003E.__xc_z);
    \u003CModule\u003E.__scrt_current_native_startup_state = (__scrt_native_startup_state) 2;
    \u003CModule\u003E.\u003FInitializedNativeFromCCTOR\u0040DefaultDomain\u0040\u003CCrtImplementationDetails\u003E\u0040\u00402_NA = true;
    \u003CModule\u003E.\u003FInitializedNative\u0040CurrentDomain\u0040\u003CCrtImplementationDetails\u003E\u0040\u0040\u0024\u0024Q2W4Progress\u00402\u0040A = (Progress) 2;
  }

  [SecurityCritical]
  [DebuggerStepThrough]
  internal static unsafe void \u003CCrtImplementationDetails\u003E\u002ELanguageSupport\u002EInitializePerProcess(
    [In] LanguageSupport* obj0)
  {
    \u003CModule\u003E.gcroot\u003CSystem\u003A\u003AString\u0020\u005E\u003E\u002E\u003D((gcroot\u003CSystem\u003A\u003AString\u0020\u005E\u003E*) obj0, "The C++ module failed to load during process initialization.\n");
    \u003CModule\u003E.\u003FInitializedPerProcess\u0040CurrentDomain\u0040\u003CCrtImplementationDetails\u003E\u0040\u0040\u0024\u0024Q2W4Progress\u00402\u0040A = (Progress) 1;
    \u003CModule\u003E._initatexit_m();
    \u003CModule\u003E._initterm_m((__FnPtr<void* ()>*) &\u003CModule\u003E.__xc_mp_a, (__FnPtr<void* ()>*) &\u003CModule\u003E.__xc_mp_z);
    \u003CModule\u003E.\u003FInitializedPerProcess\u0040CurrentDomain\u0040\u003CCrtImplementationDetails\u003E\u0040\u0040\u0024\u0024Q2W4Progress\u00402\u0040A = (Progress) 2;
    \u003CModule\u003E.\u003FInitializedPerProcess\u0040DefaultDomain\u0040\u003CCrtImplementationDetails\u003E\u0040\u00402_NA = true;
  }

  [DebuggerStepThrough]
  [SecurityCritical]
  internal static unsafe void \u003CCrtImplementationDetails\u003E\u002ELanguageSupport\u002EInitializePerAppDomain(
    [In] LanguageSupport* obj0)
  {
    \u003CModule\u003E.gcroot\u003CSystem\u003A\u003AString\u0020\u005E\u003E\u002E\u003D((gcroot\u003CSystem\u003A\u003AString\u0020\u005E\u003E*) obj0, "The C++ module failed to load during appdomain initialization.\n");
    \u003CModule\u003E.\u003FInitializedPerAppDomain\u0040CurrentDomain\u0040\u003CCrtImplementationDetails\u003E\u0040\u0040\u0024\u0024Q2W4Progress\u00402\u0040A = (Progress) 1;
    \u003CModule\u003E._initatexit_app_domain();
    \u003CModule\u003E._initterm_m((__FnPtr<void* ()>*) &\u003CModule\u003E.__xc_ma_a, (__FnPtr<void* ()>*) &\u003CModule\u003E.__xc_ma_z);
    \u003CModule\u003E.\u003FInitializedPerAppDomain\u0040CurrentDomain\u0040\u003CCrtImplementationDetails\u003E\u0040\u0040\u0024\u0024Q2W4Progress\u00402\u0040A = (Progress) 2;
  }

  [SecurityCritical]
  [DebuggerStepThrough]
  internal static unsafe void \u003CCrtImplementationDetails\u003E\u002ELanguageSupport\u002EInitializeUninitializer(
    [In] LanguageSupport* obj0)
  {
    \u003CModule\u003E.gcroot\u003CSystem\u003A\u003AString\u0020\u005E\u003E\u002E\u003D((gcroot\u003CSystem\u003A\u003AString\u0020\u005E\u003E*) obj0, "The C++ module failed to load during registration for the unload events.\n");
    \u003CModule\u003E.\u003CCrtImplementationDetails\u003E\u002ERegisterModuleUninitializer(new EventHandler(\u003CModule\u003E.\u003CCrtImplementationDetails\u003E\u002ELanguageSupport\u002EDomainUnload));
  }

  [SecurityCritical]
  [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
  [DebuggerStepThrough]
  internal static unsafe void \u003CCrtImplementationDetails\u003E\u002ELanguageSupport\u002E_Initialize(
    [In] LanguageSupport* obj0)
  {
    \u003CModule\u003E.\u003FIsDefaultDomain\u0040CurrentDomain\u0040\u003CCrtImplementationDetails\u003E\u0040\u0040\u0024\u0024Q2_NA = AppDomain.CurrentDomain.IsDefaultAppDomain();
    \u003CModule\u003E.\u003FEntered\u0040DefaultDomain\u0040\u003CCrtImplementationDetails\u003E\u0040\u00402_NA = \u003CModule\u003E.\u003FIsDefaultDomain\u0040CurrentDomain\u0040\u003CCrtImplementationDetails\u003E\u0040\u0040\u0024\u0024Q2_NA || \u003CModule\u003E.\u003FEntered\u0040DefaultDomain\u0040\u003CCrtImplementationDetails\u003E\u0040\u00402_NA;
    void* fiberPtrId = \u003CModule\u003E._getFiberPtrId();
    int num1 = 0;
    int num2 = 0;
    int num3 = 0;
    RuntimeHelpers.PrepareConstrainedRegions();
    try
    {
      while (num2 == 0)
      {
        try
        {
        }
        finally
        {
          // ISSUE: cast to a reference type
          void* voidPtr = (void*) Interlocked.CompareExchange((long&) ref \u003CModule\u003E.__scrt_native_startup_lock, (long) fiberPtrId, 0L);
          if ((IntPtr) voidPtr == IntPtr.Zero)
            num2 = 1;
          else if (voidPtr == fiberPtrId)
          {
            num1 = 1;
            num2 = 1;
          }
        }
        if (num2 == 0)
          \u003CModule\u003E.Sleep(1000U);
      }
      \u003CModule\u003E.\u003CCrtImplementationDetails\u003E\u002ELanguageSupport\u002EInitializeVtables(obj0);
      if (\u003CModule\u003E.\u003FIsDefaultDomain\u0040CurrentDomain\u0040\u003CCrtImplementationDetails\u003E\u0040\u0040\u0024\u0024Q2_NA)
      {
        \u003CModule\u003E.\u003CCrtImplementationDetails\u003E\u002ELanguageSupport\u002EInitializeNative(obj0);
        \u003CModule\u003E.\u003CCrtImplementationDetails\u003E\u002ELanguageSupport\u002EInitializePerProcess(obj0);
      }
      else
        num3 = \u003CModule\u003E.\u003CCrtImplementationDetails\u003E\u002EDefaultDomain\u002ENeedsInitialization() ? 1 : num3;
    }
    finally
    {
      if (num1 == 0)
      {
        // ISSUE: cast to a reference type
        Interlocked.Exchange((long&) ref \u003CModule\u003E.__scrt_native_startup_lock, 0L);
      }
    }
    if (num3 != 0)
      \u003CModule\u003E.\u003CCrtImplementationDetails\u003E\u002ELanguageSupport\u002EInitializeDefaultAppDomain(obj0);
    \u003CModule\u003E.\u003CCrtImplementationDetails\u003E\u002ELanguageSupport\u002EInitializePerAppDomain(obj0);
    \u003CModule\u003E.\u003FInitialized\u0040CurrentDomain\u0040\u003CCrtImplementationDetails\u003E\u0040\u0040\u0024\u0024Q2HA = 1;
    \u003CModule\u003E.\u003CCrtImplementationDetails\u003E\u002ELanguageSupport\u002EInitializeUninitializer(obj0);
  }

  [SecurityCritical]
  internal static void \u003CCrtImplementationDetails\u003E\u002ELanguageSupport\u002EUninitializeAppDomain()
  {
    \u003CModule\u003E._app_exit_callback();
  }

  [SecurityCritical]
  internal static unsafe int \u003CCrtImplementationDetails\u003E\u002ELanguageSupport\u002E_UninitializeDefaultDomain(
    void* cookie)
  {
    \u003CModule\u003E._exit_callback();
    \u003CModule\u003E.\u003FInitializedPerProcess\u0040DefaultDomain\u0040\u003CCrtImplementationDetails\u003E\u0040\u00402_NA = false;
    if (\u003CModule\u003E.\u003FInitializedNativeFromCCTOR\u0040DefaultDomain\u0040\u003CCrtImplementationDetails\u003E\u0040\u00402_NA)
    {
      \u003CModule\u003E._cexit();
      \u003CModule\u003E.__scrt_current_native_startup_state = (__scrt_native_startup_state) 0;
      \u003CModule\u003E.\u003FInitializedNativeFromCCTOR\u0040DefaultDomain\u0040\u003CCrtImplementationDetails\u003E\u0040\u00402_NA = false;
    }
    \u003CModule\u003E.\u003FInitializedNative\u0040DefaultDomain\u0040\u003CCrtImplementationDetails\u003E\u0040\u00402_NA = false;
    return 0;
  }

  [SecurityCritical]
  internal static unsafe void \u003CCrtImplementationDetails\u003E\u002ELanguageSupport\u002EUninitializeDefaultDomain()
  {
    if (!\u003CModule\u003E.\u003CCrtImplementationDetails\u003E\u002EDefaultDomain\u002ENeedsUninitialization())
      return;
    if (AppDomain.CurrentDomain.IsDefaultAppDomain())
    {
      \u003CModule\u003E.\u003CCrtImplementationDetails\u003E\u002ELanguageSupport\u002E_UninitializeDefaultDomain((void*) 0L);
    }
    else
    {
      // ISSUE: cast to a function pointer type
      \u003CModule\u003E.\u003CCrtImplementationDetails\u003E\u002EDoCallBackInDefaultDomain((__FnPtr<int (void*)>) (IntPtr) \u003CModule\u003E.__unep\u0040\u003F_UninitializeDefaultDomain\u0040LanguageSupport\u0040\u003CCrtImplementationDetails\u003E\u0040\u0040\u0024\u0024FCAJPEAX\u0040Z, (void*) 0L);
    }
  }

  [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
  [PrePrepareMethod]
  [SecurityCritical]
  internal static void \u003CCrtImplementationDetails\u003E\u002ELanguageSupport\u002EDomainUnload(
    object A_0,
    EventArgs A_1)
  {
    if (\u003CModule\u003E.\u003FInitialized\u0040CurrentDomain\u0040\u003CCrtImplementationDetails\u003E\u0040\u0040\u0024\u0024Q2HA == 0 || Interlocked.Exchange(ref \u003CModule\u003E.\u003FUninitialized\u0040CurrentDomain\u0040\u003CCrtImplementationDetails\u003E\u0040\u0040\u0024\u0024Q2HA, 1) != 0)
      return;
    int num = Interlocked.Decrement(ref \u003CModule\u003E.\u003FCount\u0040AllDomains\u0040\u003CCrtImplementationDetails\u003E\u0040\u00402HA) == 0 ? 1 : 0;
    \u003CModule\u003E.\u003CCrtImplementationDetails\u003E\u002ELanguageSupport\u002EUninitializeAppDomain();
    if ((byte) num == (byte) 0)
      return;
    \u003CModule\u003E.\u003CCrtImplementationDetails\u003E\u002ELanguageSupport\u002EUninitializeDefaultDomain();
  }

  [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
  [DebuggerStepThrough]
  [SecurityCritical]
  internal static unsafe void \u003CCrtImplementationDetails\u003E\u002ELanguageSupport\u002ECleanup(
    [In] LanguageSupport* obj0,
    System.Exception innerException)
  {
    try
    {
      bool flag = Interlocked.Decrement(ref \u003CModule\u003E.\u003FCount\u0040AllDomains\u0040\u003CCrtImplementationDetails\u003E\u0040\u00402HA) == 0;
      \u003CModule\u003E.\u003CCrtImplementationDetails\u003E\u002ELanguageSupport\u002EUninitializeAppDomain();
      if (!flag)
        return;
      \u003CModule\u003E.\u003CCrtImplementationDetails\u003E\u002ELanguageSupport\u002EUninitializeDefaultDomain();
    }
    catch (System.Exception ex)
    {
      \u003CModule\u003E.\u003CCrtImplementationDetails\u003E\u002EThrowNestedModuleLoadException(innerException, ex);
    }
    catch
    {
      \u003CModule\u003E.\u003CCrtImplementationDetails\u003E\u002EThrowNestedModuleLoadException(innerException, (System.Exception) null);
    }
  }

  [SecurityCritical]
  internal static unsafe LanguageSupport* \u003CCrtImplementationDetails\u003E\u002ELanguageSupport\u002E\u007Bctor\u007D(
    [In] LanguageSupport* obj0)
  {
    \u003CModule\u003E.gcroot\u003CSystem\u003A\u003AString\u0020\u005E\u003E\u002E\u007Bctor\u007D((gcroot\u003CSystem\u003A\u003AString\u0020\u005E\u003E*) obj0);
    return obj0;
  }

  [SecurityCritical]
  internal static unsafe void \u003CCrtImplementationDetails\u003E\u002ELanguageSupport\u002E\u007Bdtor\u007D(
    [In] LanguageSupport* obj0)
  {
    \u003CModule\u003E.gcroot\u003CSystem\u003A\u003AString\u0020\u005E\u003E\u002E\u007Bdtor\u007D((gcroot\u003CSystem\u003A\u003AString\u0020\u005E\u003E*) obj0);
  }

  [SecurityCritical]
  [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
  [DebuggerStepThrough]
  internal static unsafe void \u003CCrtImplementationDetails\u003E\u002ELanguageSupport\u002EInitialize(
    [In] LanguageSupport* obj0)
  {
    bool flag = false;
    RuntimeHelpers.PrepareConstrainedRegions();
    try
    {
      \u003CModule\u003E.gcroot\u003CSystem\u003A\u003AString\u0020\u005E\u003E\u002E\u003D((gcroot\u003CSystem\u003A\u003AString\u0020\u005E\u003E*) obj0, "The C++ module failed to load.\n");
      RuntimeHelpers.PrepareConstrainedRegions();
      try
      {
      }
      finally
      {
        Interlocked.Increment(ref \u003CModule\u003E.\u003FCount\u0040AllDomains\u0040\u003CCrtImplementationDetails\u003E\u0040\u00402HA);
        flag = true;
      }
      \u003CModule\u003E.\u003CCrtImplementationDetails\u003E\u002ELanguageSupport\u002E_Initialize(obj0);
    }
    catch (System.Exception ex)
    {
      if (flag)
        \u003CModule\u003E.\u003CCrtImplementationDetails\u003E\u002ELanguageSupport\u002ECleanup(obj0, ex);
      \u003CModule\u003E.\u003CCrtImplementationDetails\u003E\u002EThrowModuleLoadException(\u003CModule\u003E.gcroot\u003CSystem\u003A\u003AString\u0020\u005E\u003E\u002E\u002EPE\u0024AAVString\u0040System\u0040\u0040((gcroot\u003CSystem\u003A\u003AString\u0020\u005E\u003E*) obj0), ex);
    }
    catch
    {
      if (flag)
        \u003CModule\u003E.\u003CCrtImplementationDetails\u003E\u002ELanguageSupport\u002ECleanup(obj0, (System.Exception) null);
      \u003CModule\u003E.\u003CCrtImplementationDetails\u003E\u002EThrowModuleLoadException(\u003CModule\u003E.gcroot\u003CSystem\u003A\u003AString\u0020\u005E\u003E\u002E\u002EPE\u0024AAVString\u0040System\u0040\u0040((gcroot\u003CSystem\u003A\u003AString\u0020\u005E\u003E*) obj0), (System.Exception) null);
    }
  }

  [SecurityCritical]
  [DebuggerStepThrough]
  static unsafe \u003CModule\u003E()
  {
    LanguageSupport languageSupport;
    \u003CModule\u003E.\u003CCrtImplementationDetails\u003E\u002ELanguageSupport\u002E\u007Bctor\u007D(&languageSupport);
    // ISSUE: fault handler
    try
    {
      \u003CModule\u003E.\u003CCrtImplementationDetails\u003E\u002ELanguageSupport\u002EInitialize(&languageSupport);
    }
    __fault
    {
      // ISSUE: method pointer
      // ISSUE: cast to a function pointer type
      \u003CModule\u003E.___CxxCallUnwindDtor((__FnPtr<void (void*)>) __methodptr(\u003CCrtImplementationDetails\u003E\u002ELanguageSupport\u002E\u007Bdtor\u007D), (void*) &languageSupport);
    }
    \u003CModule\u003E.\u003CCrtImplementationDetails\u003E\u002ELanguageSupport\u002E\u007Bdtor\u007D(&languageSupport);
  }

  [SecuritySafeCritical]
  internal static unsafe string gcroot\u003CSystem\u003A\u003AString\u0020\u005E\u003E\u002E\u002EPE\u0024AAVString\u0040System\u0040\u0040(
    [In] gcroot\u003CSystem\u003A\u003AString\u0020\u005E\u003E* obj0)
  {
    return (string) ((GCHandle) new IntPtr((void*) *(long*) obj0)).Target;
  }

  [SecurityCritical]
  [DebuggerStepThrough]
  internal static unsafe gcroot\u003CSystem\u003A\u003AString\u0020\u005E\u003E* gcroot\u003CSystem\u003A\u003AString\u0020\u005E\u003E\u002E\u003D(
    [In] gcroot\u003CSystem\u003A\u003AString\u0020\u005E\u003E* obj0,
    string t)
  {
    ((GCHandle) new IntPtr((void*) *(long*) obj0)).Target = (object) t;
    return obj0;
  }

  [SecurityCritical]
  [DebuggerStepThrough]
  internal static unsafe void gcroot\u003CSystem\u003A\u003AString\u0020\u005E\u003E\u002E\u007Bdtor\u007D(
    [In] gcroot\u003CSystem\u003A\u003AString\u0020\u005E\u003E* obj0)
  {
    ((GCHandle) new IntPtr((void*) *(long*) obj0)).Free();
    *(long*) obj0 = 0L;
  }

  [SecuritySafeCritical]
  [DebuggerStepThrough]
  internal static unsafe gcroot\u003CSystem\u003A\u003AString\u0020\u005E\u003E* gcroot\u003CSystem\u003A\u003AString\u0020\u005E\u003E\u002E\u007Bctor\u007D(
    [In] gcroot\u003CSystem\u003A\u003AString\u0020\u005E\u003E* obj0)
  {
    IntPtr num = (IntPtr) GCHandle.Alloc((object) null);
    *(long*) obj0 = (long) num.ToPointer();
    return obj0;
  }

  [DebuggerStepThrough]
  [SecurityCritical]
  internal static unsafe ValueType \u003CCrtImplementationDetails\u003E\u002EAtExitLock\u002E_handle()
  {
    return (IntPtr) \u003CModule\u003E.\u003F_lock\u0040AtExitLock\u0040\u003CCrtImplementationDetails\u003E\u0040\u0040\u0024\u0024Q0PEAXEA != IntPtr.Zero ? (ValueType) GCHandle.FromIntPtr(new IntPtr(\u003CModule\u003E.\u003F_lock\u0040AtExitLock\u0040\u003CCrtImplementationDetails\u003E\u0040\u0040\u0024\u0024Q0PEAXEA)) : (ValueType) null;
  }

  [SecurityCritical]
  [DebuggerStepThrough]
  internal static unsafe void \u003CCrtImplementationDetails\u003E\u002EAtExitLock\u002E_lock_Construct(
    object value)
  {
    \u003CModule\u003E.\u003F_lock\u0040AtExitLock\u0040\u003CCrtImplementationDetails\u003E\u0040\u0040\u0024\u0024Q0PEAXEA = (void*) 0L;
    \u003CModule\u003E.\u003CCrtImplementationDetails\u003E\u002EAtExitLock\u002E_lock_Set(value);
  }

  [DebuggerStepThrough]
  [SecurityCritical]
  internal static unsafe void \u003CCrtImplementationDetails\u003E\u002EAtExitLock\u002E_lock_Set(
    object value)
  {
    ValueType valueType = \u003CModule\u003E.\u003CCrtImplementationDetails\u003E\u002EAtExitLock\u002E_handle();
    if (valueType == null)
      \u003CModule\u003E.\u003F_lock\u0040AtExitLock\u0040\u003CCrtImplementationDetails\u003E\u0040\u0040\u0024\u0024Q0PEAXEA = GCHandle.ToIntPtr(GCHandle.Alloc(value)).ToPointer();
    else
      ((GCHandle) valueType).Target = value;
  }

  [DebuggerStepThrough]
  [SecurityCritical]
  internal static object \u003CCrtImplementationDetails\u003E\u002EAtExitLock\u002E_lock_Get()
  {
    ValueType valueType = \u003CModule\u003E.\u003CCrtImplementationDetails\u003E\u002EAtExitLock\u002E_handle();
    return valueType != null ? ((GCHandle) valueType).Target : (object) null;
  }

  [SecurityCritical]
  [DebuggerStepThrough]
  internal static unsafe void \u003CCrtImplementationDetails\u003E\u002EAtExitLock\u002E_lock_Destruct()
  {
    ValueType valueType = \u003CModule\u003E.\u003CCrtImplementationDetails\u003E\u002EAtExitLock\u002E_handle();
    if (valueType == null)
      return;
    ((GCHandle) valueType).Free();
    \u003CModule\u003E.\u003F_lock\u0040AtExitLock\u0040\u003CCrtImplementationDetails\u003E\u0040\u0040\u0024\u0024Q0PEAXEA = (void*) 0L;
  }

  [DebuggerStepThrough]
  [SecurityCritical]
  [return: MarshalAs(UnmanagedType.U1)]
  internal static bool \u003CCrtImplementationDetails\u003E\u002EAtExitLock\u002EIsInitialized()
  {
    return \u003CModule\u003E.\u003CCrtImplementationDetails\u003E\u002EAtExitLock\u002E_lock_Get() != null;
  }

  [SecurityCritical]
  [DebuggerStepThrough]
  internal static void \u003CCrtImplementationDetails\u003E\u002EAtExitLock\u002EAddRef()
  {
    if (!\u003CModule\u003E.\u003CCrtImplementationDetails\u003E\u002EAtExitLock\u002EIsInitialized())
    {
      \u003CModule\u003E.\u003CCrtImplementationDetails\u003E\u002EAtExitLock\u002E_lock_Construct(new object());
      \u003CModule\u003E.\u003F_ref_count\u0040AtExitLock\u0040\u003CCrtImplementationDetails\u003E\u0040\u0040\u0024\u0024Q0HA = 0;
    }
    ++\u003CModule\u003E.\u003F_ref_count\u0040AtExitLock\u0040\u003CCrtImplementationDetails\u003E\u0040\u0040\u0024\u0024Q0HA;
  }

  [SecurityCritical]
  [DebuggerStepThrough]
  internal static void \u003CCrtImplementationDetails\u003E\u002EAtExitLock\u002ERemoveRef()
  {
    \u003CModule\u003E.\u003F_ref_count\u0040AtExitLock\u0040\u003CCrtImplementationDetails\u003E\u0040\u0040\u0024\u0024Q0HA += -1;
    if (\u003CModule\u003E.\u003F_ref_count\u0040AtExitLock\u0040\u003CCrtImplementationDetails\u003E\u0040\u0040\u0024\u0024Q0HA != 0)
      return;
    \u003CModule\u003E.\u003CCrtImplementationDetails\u003E\u002EAtExitLock\u002E_lock_Destruct();
  }

  [SecurityCritical]
  [DebuggerStepThrough]
  internal static void \u003CCrtImplementationDetails\u003E\u002EAtExitLock\u002EEnter()
  {
    Monitor.Enter(\u003CModule\u003E.\u003CCrtImplementationDetails\u003E\u002EAtExitLock\u002E_lock_Get());
  }

  [DebuggerStepThrough]
  [SecurityCritical]
  internal static void \u003CCrtImplementationDetails\u003E\u002EAtExitLock\u002EExit()
  {
    Monitor.Exit(\u003CModule\u003E.\u003CCrtImplementationDetails\u003E\u002EAtExitLock\u002E_lock_Get());
  }

  [DebuggerStepThrough]
  [SecurityCritical]
  [return: MarshalAs(UnmanagedType.U1)]
  internal static bool \u003FA0xca239242\u002E__global_lock()
  {
    bool flag = false;
    if (\u003CModule\u003E.\u003CCrtImplementationDetails\u003E\u002EAtExitLock\u002EIsInitialized())
    {
      \u003CModule\u003E.\u003CCrtImplementationDetails\u003E\u002EAtExitLock\u002EEnter();
      flag = true;
    }
    return flag;
  }

  [DebuggerStepThrough]
  [SecurityCritical]
  [return: MarshalAs(UnmanagedType.U1)]
  internal static bool \u003FA0xca239242\u002E__global_unlock()
  {
    bool flag = false;
    if (\u003CModule\u003E.\u003CCrtImplementationDetails\u003E\u002EAtExitLock\u002EIsInitialized())
    {
      \u003CModule\u003E.\u003CCrtImplementationDetails\u003E\u002EAtExitLock\u002EExit();
      flag = true;
    }
    return flag;
  }

  [DebuggerStepThrough]
  [SecurityCritical]
  [return: MarshalAs(UnmanagedType.U1)]
  internal static bool \u003FA0xca239242\u002E__alloc_global_lock()
  {
    \u003CModule\u003E.\u003CCrtImplementationDetails\u003E\u002EAtExitLock\u002EAddRef();
    return \u003CModule\u003E.\u003CCrtImplementationDetails\u003E\u002EAtExitLock\u002EIsInitialized();
  }

  [DebuggerStepThrough]
  [SecurityCritical]
  internal static void \u003FA0xca239242\u002E__dealloc_global_lock()
  {
    \u003CModule\u003E.\u003CCrtImplementationDetails\u003E\u002EAtExitLock\u002ERemoveRef();
  }

  [SecurityCritical]
  internal static unsafe int _atexit_helper(
    __FnPtr<void ()> func,
    ulong* __pexit_list_size,
    __FnPtr<void ()>** __ponexitend_e,
    __FnPtr<void ()>** __ponexitbegin_e)
  {
    // ISSUE: cast to a function pointer type
    __FnPtr<void ()> local1 = (__FnPtr<void ()>) 0L;
    if (func == null)
      return -1;
    int num1;
    if (\u003CModule\u003E.\u003FA0xca239242\u002E__global_lock())
    {
      try
      {
        __FnPtr<void ()>* _Ptr1 = (__FnPtr<void ()>*) \u003CModule\u003E.DecodePointer((void*) *(long*) __ponexitbegin_e);
        __FnPtr<void ()>* local2 = (__FnPtr<void ()>*) \u003CModule\u003E.DecodePointer((void*) *(long*) __ponexitend_e);
        long num2 = (long) ((IntPtr) local2 - (IntPtr) _Ptr1);
        if (*__pexit_list_size - 1UL < (ulong) (num2 >>> 3))
        {
          try
          {
            ulong num3 = *__pexit_list_size * 8UL;
            ulong num4 = num3 < 4096UL /*0x1000*/ ? num3 : 4096UL /*0x1000*/;
            IntPtr cb = new IntPtr((int) ((long) num3 + (long) num4));
            IntPtr num5 = Marshal.ReAllocHGlobal(new IntPtr((void*) _Ptr1), cb);
            local2 = (__FnPtr<void ()>*) ((IntPtr) num5.ToPointer() + num2);
            _Ptr1 = (__FnPtr<void ()>*) num5.ToPointer();
            ulong num6 = *__pexit_list_size;
            ulong num7 = 512UL /*0x0200*/ < num6 ? 512UL /*0x0200*/ : num6;
            *__pexit_list_size = num6 + num7;
          }
          catch (OutOfMemoryException ex)
          {
            IntPtr cb = new IntPtr((int) ((long) *__pexit_list_size * 8L + 12L));
            IntPtr num8 = Marshal.ReAllocHGlobal(new IntPtr((void*) _Ptr1), cb);
            local2 = (__FnPtr<void ()>*) ((IntPtr) num8.ToPointer() - (IntPtr) _Ptr1 + (IntPtr) local2);
            _Ptr1 = (__FnPtr<void ()>*) num8.ToPointer();
            ulong* numPtr = __pexit_list_size;
            long num9 = (long) *numPtr + 4L;
            *numPtr = (ulong) num9;
          }
        }
        *(long*) local2 = (long) func;
        __FnPtr<void ()>* _Ptr2 = (__FnPtr<void ()>*) ((IntPtr) local2 + 8L);
        local1 = func;
        *(long*) __ponexitbegin_e = (long) \u003CModule\u003E.EncodePointer((void*) _Ptr1);
        *(long*) __ponexitend_e = (long) \u003CModule\u003E.EncodePointer((void*) _Ptr2);
      }
      catch (OutOfMemoryException ex)
      {
      }
      finally
      {
        \u003CModule\u003E.\u003FA0xca239242\u002E__global_unlock();
      }
      if (local1 != null)
      {
        num1 = 0;
        goto label_12;
      }
    }
    num1 = -1;
label_12:
    return num1;
  }

  [SecurityCritical]
  internal static unsafe void _exit_callback()
  {
    if (\u003CModule\u003E.\u003FA0xca239242\u002E__exit_list_size == 0UL)
      return;
    __FnPtr<void ()>* local1 = (__FnPtr<void ()>*) \u003CModule\u003E.DecodePointer((void*) \u003CModule\u003E.\u003FA0xca239242\u002E__onexitbegin_m);
    __FnPtr<void ()>* local2 = (__FnPtr<void ()>*) \u003CModule\u003E.DecodePointer((void*) \u003CModule\u003E.\u003FA0xca239242\u002E__onexitend_m);
    if ((IntPtr) local1 != -1L && (IntPtr) local1 != IntPtr.Zero && (IntPtr) local2 != IntPtr.Zero)
    {
      __FnPtr<void ()>* local3 = local1;
      __FnPtr<void ()>* local4 = local2;
      while (true)
      {
        __FnPtr<void ()>* local5;
        __FnPtr<void ()>* local6;
        do
        {
          do
          {
            local2 -= 8L;
            if (local2 < local1)
              goto label_7;
          }
          while (*(long*) local2 == (IntPtr) \u003CModule\u003E.EncodePointer((void*) 0L));
          void* voidPtr = \u003CModule\u003E.DecodePointer((void*) *(long*) local2);
          *(long*) local2 = (long) \u003CModule\u003E.EncodePointer((void*) 0L);
          // ISSUE: cast to a function pointer type
          // ISSUE: function pointer call
          __calli((__FnPtr<void ()>) (IntPtr) voidPtr)();
          local5 = (__FnPtr<void ()>*) \u003CModule\u003E.DecodePointer((void*) \u003CModule\u003E.\u003FA0xca239242\u002E__onexitbegin_m);
          local6 = (__FnPtr<void ()>*) \u003CModule\u003E.DecodePointer((void*) \u003CModule\u003E.\u003FA0xca239242\u002E__onexitend_m);
        }
        while (local3 == local5 && local4 == local6);
        local3 = local5;
        local1 = local5;
        local4 = local6;
        local2 = local6;
      }
label_7:
      Marshal.FreeHGlobal(new IntPtr((void*) local1));
    }
    \u003CModule\u003E.\u003FA0xca239242\u002E__dealloc_global_lock();
  }

  [SecurityCritical]
  [DebuggerStepThrough]
  internal static unsafe int _initatexit_m()
  {
    if (!\u003CModule\u003E.\u003FA0xca239242\u002E__alloc_global_lock())
      return 0;
    \u003CModule\u003E.\u003FA0xca239242\u002E__onexitbegin_m = (__FnPtr<void ()>*) \u003CModule\u003E.EncodePointer(Marshal.AllocHGlobal(256 /*0x0100*/).ToPointer());
    \u003CModule\u003E.\u003FA0xca239242\u002E__onexitend_m = \u003CModule\u003E.\u003FA0xca239242\u002E__onexitbegin_m;
    \u003CModule\u003E.\u003FA0xca239242\u002E__exit_list_size = 32UL /*0x20*/;
    return 1;
  }

  internal static __FnPtr<int ()> _onexit_m(__FnPtr<int ()> _Function)
  {
    // ISSUE: cast to a function pointer type
    // ISSUE: cast to a function pointer type
    return \u003CModule\u003E._atexit_m((__FnPtr<void ()>) _Function) != -1 ? _Function : (__FnPtr<int ()>) 0L;
  }

  [SecurityCritical]
  internal static unsafe int _atexit_m(__FnPtr<void ()> func)
  {
    // ISSUE: cast to a function pointer type
    return \u003CModule\u003E._atexit_helper((__FnPtr<void ()>) (IntPtr) \u003CModule\u003E.EncodePointer((void*) func), &\u003CModule\u003E.\u003FA0xca239242\u002E__exit_list_size, &\u003CModule\u003E.\u003FA0xca239242\u002E__onexitend_m, &\u003CModule\u003E.\u003FA0xca239242\u002E__onexitbegin_m);
  }

  [DebuggerStepThrough]
  [SecurityCritical]
  internal static unsafe int _initatexit_app_domain()
  {
    if (\u003CModule\u003E.\u003FA0xca239242\u002E__alloc_global_lock())
    {
      \u003CModule\u003E.__onexitbegin_app_domain = (__FnPtr<void ()>*) \u003CModule\u003E.EncodePointer(Marshal.AllocHGlobal(256 /*0x0100*/).ToPointer());
      \u003CModule\u003E.__onexitend_app_domain = \u003CModule\u003E.__onexitbegin_app_domain;
      \u003CModule\u003E.__exit_list_size_app_domain = 32UL /*0x20*/;
    }
    return 1;
  }

  [SecurityCritical]
  [HandleProcessCorruptedStateExceptions]
  internal static unsafe void _app_exit_callback()
  {
    if (\u003CModule\u003E.__exit_list_size_app_domain == 0UL)
      return;
    __FnPtr<void ()>* local1 = (__FnPtr<void ()>*) \u003CModule\u003E.DecodePointer((void*) \u003CModule\u003E.__onexitbegin_app_domain);
    __FnPtr<void ()>* local2 = (__FnPtr<void ()>*) \u003CModule\u003E.DecodePointer((void*) \u003CModule\u003E.__onexitend_app_domain);
    try
    {
      if ((IntPtr) local1 == -1L || (IntPtr) local1 == IntPtr.Zero || (IntPtr) local2 == IntPtr.Zero)
        return;
      __FnPtr<void ()>* local3 = local1;
      __FnPtr<void ()>* local4 = local2;
      while (true)
      {
        __FnPtr<void ()>* local5;
        __FnPtr<void ()>* local6;
        do
        {
          do
          {
            local2 -= 8L;
          }
          while (local2 >= local1 && *(long*) local2 == (IntPtr) \u003CModule\u003E.EncodePointer((void*) 0L));
          if (local2 >= local1)
          {
            // ISSUE: cast to a function pointer type
            __FnPtr<void ()> local7 = (__FnPtr<void ()>) (IntPtr) \u003CModule\u003E.DecodePointer((void*) *(long*) local2);
            *(long*) local2 = (long) \u003CModule\u003E.EncodePointer((void*) 0L);
            // ISSUE: function pointer call
            __calli(local7)();
            local5 = (__FnPtr<void ()>*) \u003CModule\u003E.DecodePointer((void*) \u003CModule\u003E.__onexitbegin_app_domain);
            local6 = (__FnPtr<void ()>*) \u003CModule\u003E.DecodePointer((void*) \u003CModule\u003E.__onexitend_app_domain);
          }
          else
            goto label_12;
        }
        while (local3 == local5 && local4 == local6);
        local3 = local5;
        local1 = local5;
        local4 = local6;
        local2 = local6;
      }
label_12:;
    }
    finally
    {
      Marshal.FreeHGlobal(new IntPtr((void*) local1));
      \u003CModule\u003E.\u003FA0xca239242\u002E__dealloc_global_lock();
    }
  }

  [SecurityCritical]
  internal static __FnPtr<int ()> _onexit_m_appdomain(__FnPtr<int ()> _Function)
  {
    // ISSUE: cast to a function pointer type
    // ISSUE: cast to a function pointer type
    return \u003CModule\u003E._atexit_m_appdomain((__FnPtr<void ()>) _Function) != -1 ? _Function : (__FnPtr<int ()>) 0L;
  }

  [DebuggerStepThrough]
  [SecurityCritical]
  internal static unsafe int _atexit_m_appdomain(__FnPtr<void ()> func)
  {
    // ISSUE: cast to a function pointer type
    return \u003CModule\u003E._atexit_helper((__FnPtr<void ()>) (IntPtr) \u003CModule\u003E.EncodePointer((void*) func), &\u003CModule\u003E.__exit_list_size_app_domain, &\u003CModule\u003E.__onexitend_app_domain, &\u003CModule\u003E.__onexitbegin_app_domain);
  }

  [SuppressUnmanagedCodeSecurity]
  [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
  [SecurityCritical]
  [DllImport("KERNEL32.dll")]
  public static extern unsafe void* DecodePointer(void* _Ptr);

  [SecurityCritical]
  [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
  [SuppressUnmanagedCodeSecurity]
  [DllImport("KERNEL32.dll")]
  public static extern unsafe void* EncodePointer(void* _Ptr);

  [DebuggerStepThrough]
  [SecurityCritical]
  internal static unsafe int _initterm_e(__FnPtr<int ()>* pfbegin, __FnPtr<int ()>* pfend)
  {
    int num1 = 0;
    if (pfbegin < pfend)
    {
      while (num1 == 0)
      {
        ulong num2 = (ulong) *(long*) pfbegin;
        if (num2 != 0UL)
        {
          // ISSUE: cast to a function pointer type
          // ISSUE: function pointer call
          num1 = __calli((__FnPtr<int ()>) (long) num2)();
        }
        pfbegin += 8L;
        if (pfbegin >= pfend)
          break;
      }
    }
    return num1;
  }

  [SecurityCritical]
  [DebuggerStepThrough]
  internal static unsafe void _initterm(__FnPtr<void ()>* pfbegin, __FnPtr<void ()>* pfend)
  {
    if (pfbegin >= pfend)
      return;
    do
    {
      ulong num = (ulong) *(long*) pfbegin;
      if (num != 0UL)
      {
        // ISSUE: cast to a function pointer type
        // ISSUE: function pointer call
        __calli((__FnPtr<void ()>) (long) num)();
      }
      pfbegin += 8L;
    }
    while (pfbegin < pfend);
  }

  [DebuggerStepThrough]
  internal static ModuleHandle \u003CCrtImplementationDetails\u003E\u002EThisModule\u002EHandle()
  {
    return typeof (ThisModule).Module.ModuleHandle;
  }

  [DebuggerStepThrough]
  [SecurityCritical]
  [SecurityPermission(SecurityAction.Assert, UnmanagedCode = true)]
  internal static unsafe void _initterm_m(__FnPtr<void* ()>* pfbegin, __FnPtr<void* ()>* pfend)
  {
    if (pfbegin >= pfend)
      return;
    do
    {
      ulong methodToken = (ulong) *(long*) pfbegin;
      if (methodToken != 0UL)
      {
        // ISSUE: cast to a function pointer type
        // ISSUE: function pointer call
        void* voidPtr = __calli(\u003CModule\u003E.\u003CCrtImplementationDetails\u003E\u002EThisModule\u002EResolveMethod\u003Cvoid\u0020const\u0020\u002A\u0020__clrcall\u0028void\u0029\u003E((__FnPtr<void* ()>) (long) methodToken))();
      }
      pfbegin += 8L;
    }
    while (pfbegin < pfend);
  }

  [SecurityCritical]
  [DebuggerStepThrough]
  internal static unsafe __FnPtr<void* ()> \u003CCrtImplementationDetails\u003E\u002EThisModule\u002EResolveMethod\u003Cvoid\u0020const\u0020\u002A\u0020__clrcall\u0028void\u0029\u003E(
    __FnPtr<void* ()> methodToken)
  {
    // ISSUE: cast to a function pointer type
    return (__FnPtr<void* ()>) (IntPtr) \u003CModule\u003E.\u003CCrtImplementationDetails\u003E\u002EThisModule\u002EHandle().ResolveMethodHandle((int) methodToken).GetFunctionPointer().ToPointer();
  }

  [SecurityCritical]
  [HandleProcessCorruptedStateExceptions]
  [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
  [SecurityPermission(SecurityAction.Assert, UnmanagedCode = true)]
  internal static unsafe void ___CxxCallUnwindDtor(__FnPtr<void (void*)> pDtor, void* pThis)
  {
    try
    {
      void* voidPtr = pThis;
      // ISSUE: function pointer call
      __calli(pDtor)(voidPtr);
    }
    catch (System.Exception ex) when (\u003CModule\u003E.__FrameUnwindFilter((_EXCEPTION_POINTERS*) Marshal.GetExceptionPointers()) != 0)
    {
    }
  }

  [HandleProcessCorruptedStateExceptions]
  [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
  [SecurityCritical]
  [SecurityPermission(SecurityAction.Assert, UnmanagedCode = true)]
  internal static unsafe void ___CxxCallUnwindDelDtor(__FnPtr<void (void*)> pDtor, void* pThis)
  {
    try
    {
      void* voidPtr = pThis;
      // ISSUE: function pointer call
      __calli(pDtor)(voidPtr);
    }
    catch (System.Exception ex) when (\u003CModule\u003E.__FrameUnwindFilter((_EXCEPTION_POINTERS*) Marshal.GetExceptionPointers()) != 0)
    {
    }
  }

  [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
  [SecurityCritical]
  [HandleProcessCorruptedStateExceptions]
  [SecurityPermission(SecurityAction.Assert, UnmanagedCode = true)]
  internal static unsafe void ___CxxCallUnwindVecDtor(
    __FnPtr<void (void*, ulong, int, __FnPtr<void (void*)>)> pVecDtor,
    void* ptr,
    ulong size,
    int count,
    __FnPtr<void (void*)> pDtor)
  {
    try
    {
      void* voidPtr = ptr;
      long num1 = (long) size;
      int num2 = count;
      __FnPtr<void (void*)> local = pDtor;
      // ISSUE: function pointer call
      __calli(pVecDtor)(voidPtr, (ulong) num1, num2, local);
    }
    catch (System.Exception ex) when (\u003CModule\u003E.__FrameUnwindFilter((_EXCEPTION_POINTERS*) Marshal.GetExceptionPointers()) != 0)
    {
    }
  }

  [SuppressUnmanagedCodeSecurity]
  [MethodImpl(MethodImplOptions.Unmanaged | MethodImplOptions.PreserveSig, MethodCodeType = MethodCodeType.Native)]
  internal static extern unsafe void* @new([In] ulong obj0);

  [SuppressUnmanagedCodeSecurity]
  [DllImport("", EntryPoint = "", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
  [MethodImpl(MethodImplOptions.Unmanaged, MethodCodeType = MethodCodeType.Native)]
  internal static extern unsafe int PassThruReadMsgs(
    [In] uint obj0,
    [In] PASSTHRU_MSG* obj1,
    [In] uint* obj2,
    [In] uint obj3);

  [SuppressUnmanagedCodeSecurity]
  [DllImport("", EntryPoint = "", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
  [MethodImpl(MethodImplOptions.Unmanaged, MethodCodeType = MethodCodeType.Native)]
  internal static extern unsafe int PassThruWriteMsgs(
    [In] uint obj0,
    [In] PASSTHRU_MSG* obj1,
    [In] uint* obj2,
    [In] uint obj3);

  [SuppressUnmanagedCodeSecurity]
  [DllImport("", EntryPoint = "", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
  [MethodImpl(MethodImplOptions.Unmanaged, MethodCodeType = MethodCodeType.Native)]
  internal static extern unsafe int PassThruGetLastError([In] sbyte* obj0);

  [SuppressUnmanagedCodeSecurity]
  [DllImport("", EntryPoint = "", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
  [MethodImpl(MethodImplOptions.Unmanaged, MethodCodeType = MethodCodeType.Native)]
  internal static extern int PassThruStopMsgFilter([In] uint obj0, [In] uint obj1);

  [SuppressUnmanagedCodeSecurity]
  [DllImport("", EntryPoint = "", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
  [MethodImpl(MethodImplOptions.Unmanaged, MethodCodeType = MethodCodeType.Native)]
  internal static extern unsafe int PassThruStartMsgFilter(
    [In] uint obj0,
    [In] uint obj1,
    [In] PASSTHRU_MSG* obj2,
    [In] PASSTHRU_MSG* obj3,
    [In] PASSTHRU_MSG* obj4,
    [In] uint* obj5);

  [SuppressUnmanagedCodeSecurity]
  [DllImport("", EntryPoint = "", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
  [MethodImpl(MethodImplOptions.Unmanaged, MethodCodeType = MethodCodeType.Native)]
  internal static extern unsafe int PassThruConnect([In] uint obj0, [In] uint obj1, [In] uint* obj2);

  [SuppressUnmanagedCodeSecurity]
  [DllImport("", EntryPoint = "", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
  [MethodImpl(MethodImplOptions.Unmanaged, MethodCodeType = MethodCodeType.Native)]
  internal static extern int PassThruDisconnect([In] uint obj0);

  [SuppressUnmanagedCodeSecurity]
  [DllImport("", EntryPoint = "", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
  [MethodImpl(MethodImplOptions.Unmanaged, MethodCodeType = MethodCodeType.Native)]
  internal static extern int PassThruClose();

  [SuppressUnmanagedCodeSecurity]
  [DllImport("", EntryPoint = "", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
  [MethodImpl(MethodImplOptions.Unmanaged, MethodCodeType = MethodCodeType.Native)]
  internal static extern int PassThruOpen();

  [SuppressUnmanagedCodeSecurity]
  [MethodImpl(MethodImplOptions.Unmanaged | MethodImplOptions.PreserveSig, MethodCodeType = MethodCodeType.Native)]
  internal static extern unsafe void delete([In] void* obj0);

  [SuppressUnmanagedCodeSecurity]
  [MethodImpl(MethodImplOptions.Unmanaged | MethodImplOptions.PreserveSig, MethodCodeType = MethodCodeType.Native)]
  internal static extern unsafe void* new\u005B\u005D([In] ulong obj0);

  [SuppressUnmanagedCodeSecurity]
  [DllImport("", EntryPoint = "", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
  [MethodImpl(MethodImplOptions.Unmanaged, MethodCodeType = MethodCodeType.Native)]
  internal static extern unsafe int PassThruIoctl([In] uint obj0, [In] uint obj1, [In] void* obj2, [In] void* obj3);

  [SuppressUnmanagedCodeSecurity]
  [DllImport("", EntryPoint = "", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
  [MethodImpl(MethodImplOptions.Unmanaged, MethodCodeType = MethodCodeType.Native)]
  internal static extern void _invalid_parameter_noinfo();

  [SuppressUnmanagedCodeSecurity]
  [DllImport("", EntryPoint = "", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
  [MethodImpl(MethodImplOptions.Unmanaged, MethodCodeType = MethodCodeType.Native)]
  internal static extern unsafe int* _errno();

  [SuppressUnmanagedCodeSecurity]
  [MethodImpl(MethodImplOptions.Unmanaged | MethodImplOptions.PreserveSig, MethodCodeType = MethodCodeType.Native)]
  internal static extern unsafe void* _getFiberPtrId();

  [SuppressUnmanagedCodeSecurity]
  [DllImport("", EntryPoint = "", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
  [MethodImpl(MethodImplOptions.Unmanaged, MethodCodeType = MethodCodeType.Native)]
  internal static extern void _cexit();

  [SuppressUnmanagedCodeSecurity]
  [DllImport("", EntryPoint = "", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
  [MethodImpl(MethodImplOptions.Unmanaged, MethodCodeType = MethodCodeType.Native)]
  internal static extern void Sleep([In] uint obj0);

  [SuppressUnmanagedCodeSecurity]
  [DllImport("", EntryPoint = "", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
  [MethodImpl(MethodImplOptions.Unmanaged, MethodCodeType = MethodCodeType.Native)]
  internal static extern void abort();

  [SuppressUnmanagedCodeSecurity]
  [MethodImpl(MethodImplOptions.Unmanaged | MethodImplOptions.PreserveSig, MethodCodeType = MethodCodeType.Native)]
  internal static extern void __security_init_cookie();

  [SuppressUnmanagedCodeSecurity]
  [DllImport("", EntryPoint = "", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
  [MethodImpl(MethodImplOptions.Unmanaged, MethodCodeType = MethodCodeType.Native)]
  internal static extern unsafe int __FrameUnwindFilter([In] _EXCEPTION_POINTERS* obj0);
}
