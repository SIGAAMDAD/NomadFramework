/*
===========================================================================
The Nomad Framework
Copyright (C) 2025-2026 Noah Van Til

This Source Code Form is subject to the terms of the Mozilla Public
License, v2. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.

This software is provided "as is", without warranty of any kind,
express or implied, including but not limited to the warranties
of merchantability, fitness for a particular purpose and noninfringement.
===========================================================================
*/

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;

#if false
namespace FMOD
{
    /*
    ===================================================================================

    FMODNative

    ===================================================================================
    */
    [SuppressUnmanagedCodeSecurity]
    internal static unsafe partial class FMODNative
    {
        private static readonly Lazy<IntPtr> s_studioModule = new(() => NativeLibrary.Load(Studio.STUDIO_VERSION.dll));
        private static readonly Lazy<IntPtr> s_coreModule = new(() => NativeLibrary.Load(VERSION.dll));

        private static delegate* unmanaged[Cdecl]<IntPtr*, uint, RESULT> s_studioSystemCreatePtr;
        private static delegate* unmanaged[Cdecl]<IntPtr, int, Studio.INITFLAGS, INITFLAGS, IntPtr, RESULT> s_studioSystemInitializePtr;
        private static delegate* unmanaged[Cdecl]<IntPtr, RESULT> s_studioSystemReleasePtr;
        private static delegate* unmanaged[Cdecl]<IntPtr, RESULT> s_studioSystemUpdatePtr;
        private static delegate* unmanaged[Cdecl]<IntPtr, Studio.ADVANCEDSETTINGS*, RESULT> s_studioSystemGetAdvancedSettingsPtr;
        private static delegate* unmanaged[Cdecl]<IntPtr, GUID*, IntPtr*, RESULT> s_studioSystemGetBankByIdPtr;
        private static delegate* unmanaged[Cdecl]<IntPtr, int*, RESULT> s_studioSystemGetBankCountPtr;
        private static delegate* unmanaged[Cdecl]<IntPtr, IntPtr*, RESULT> s_studioSystemGetCoreSystemPtr;
        private static delegate* unmanaged[Cdecl]<IntPtr, int, ATTRIBUTES_3D*, IntPtr, RESULT> s_studioSystemGetListenerAttributesNullPtr;
        private static delegate* unmanaged[Cdecl]<IntPtr, int, ATTRIBUTES_3D*, VECTOR*, RESULT> s_studioSystemGetListenerAttributesPtr;
        private static delegate* unmanaged[Cdecl]<IntPtr, int, float*, RESULT> s_studioSystemGetListenerWeightPtr;
        private static delegate* unmanaged[Cdecl]<IntPtr, Studio.MEMORY_USAGE*, RESULT> s_studioSystemGetMemoryUsagePtr;
        private static delegate* unmanaged[Cdecl]<IntPtr, int*, RESULT> s_studioSystemGetNumListenersPtr;
        private static delegate* unmanaged[Cdecl]<IntPtr, int*, RESULT> s_studioSystemGetParameterDescriptionCountPtr;
        private static delegate* unmanaged[Cdecl]<IntPtr, RESULT> s_studioSystemFlushCommandsPtr;
        private static delegate* unmanaged[Cdecl]<IntPtr, RESULT> s_studioSystemFlushSampleLoadingPtr;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static T LoadStudioFunction<T>(string name)
            where T : Delegate
        {
            return Marshal.GetDelegateForFunctionPointer<T>(NativeLibrary.GetExport(s_studioModule.Value, name));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static IntPtr LoadStudioExport(string name)
        {
            return NativeLibrary.GetExport(s_studioModule.Value, name);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static T LoadCoreFunction<T>(string name)
            where T : Delegate
        {
            return Marshal.GetDelegateForFunctionPointer<T>(NativeLibrary.GetExport(s_coreModule.Value, name));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static IntPtr LoadCoreExport(string name)
        {
            return NativeLibrary.GetExport(s_coreModule.Value, name);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_System_Create(ref Studio.System system, uint version)
        {
            s_studioSystemCreatePtr ??= (delegate* unmanaged[Cdecl]<IntPtr*, uint, RESULT>)LoadStudioExport("FMOD_Studio_System_Create");
            IntPtr handle;
            RESULT result = s_studioSystemCreatePtr(&handle, version);
            system.handle = handle;
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_System_Initialize(ref Studio.System system, int maxChannels, Studio.INITFLAGS studioFlags, INITFLAGS flags, IntPtr extraDriverData)
        {
            s_studioSystemInitializePtr ??= (delegate* unmanaged[Cdecl]<IntPtr, int, Studio.INITFLAGS, INITFLAGS, IntPtr, RESULT>)LoadStudioExport("FMOD_Studio_System_Initialize");
            return s_studioSystemInitializePtr(system.handle, maxChannels, studioFlags, flags, extraDriverData);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_System_Release(ref Studio.System system)
        {
            s_studioSystemReleasePtr ??= (delegate* unmanaged[Cdecl]<IntPtr, RESULT>)LoadStudioExport("FMOD_Studio_System_Release");
            return s_studioSystemReleasePtr(system.handle);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_System_Update(ref Studio.System system)
        {
            s_studioSystemUpdatePtr ??= (delegate* unmanaged[Cdecl]<IntPtr, RESULT>)LoadStudioExport("FMOD_Studio_System_Update");
            return s_studioSystemUpdatePtr(system.handle);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_System_GetAdvancedSettings(ref Studio.System system, out Studio.ADVANCEDSETTINGS settings)
        {
            settings = default;
            settings.cbsize = Marshal.SizeOf<Studio.ADVANCEDSETTINGS>();
            s_studioSystemGetAdvancedSettingsPtr ??= (delegate* unmanaged[Cdecl]<IntPtr, Studio.ADVANCEDSETTINGS*, RESULT>)LoadStudioExport("FMOD_Studio_System_GetAdvancedSettings");
            fixed (Studio.ADVANCEDSETTINGS* settingsPtr = &settings)
            {
                return s_studioSystemGetAdvancedSettingsPtr(system.handle, settingsPtr);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_System_GetBank(ref Studio.System system, string path, out Studio.Bank bank)
        {
            bank = default;
            using (StringHelper.ThreadSafeEncoding encoder = StringHelper.GetFreeHelper())
            {
                RESULT result = Studio_System_GetBank(system.handle, encoder.byteFromStringUTF8(path), out IntPtr handle);
                bank.handle = handle;
                return result;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_System_GetBankByID(ref Studio.System system, ref GUID id, out Studio.Bank bank)
        {
            bank = default;
            s_studioSystemGetBankByIdPtr ??= (delegate* unmanaged[Cdecl]<IntPtr, GUID*, IntPtr*, RESULT>)LoadStudioExport("FMOD_Studio_System_GetBankByID");
            IntPtr handle;
            fixed (GUID* idPtr = &id)
            {
                RESULT result = s_studioSystemGetBankByIdPtr(system.handle, idPtr, &handle);
                bank.handle = handle;
                return result;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_System_GetBankCount(ref Studio.System system, out int count)
        {
            s_studioSystemGetBankCountPtr ??= (delegate* unmanaged[Cdecl]<IntPtr, int*, RESULT>)LoadStudioExport("FMOD_Studio_System_GetBankCount");
            count = 0;
            fixed (int* countPtr = &count)
            {
                return s_studioSystemGetBankCountPtr(system.handle, countPtr);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_System_GetCoreSystem(ref Studio.System system, ref System coreSystem)
        {
            s_studioSystemGetCoreSystemPtr ??= (delegate* unmanaged[Cdecl]<IntPtr, IntPtr*, RESULT>)LoadStudioExport("FMOD_Studio_System_GetCoreSystem");
            IntPtr handle;
            RESULT result = s_studioSystemGetCoreSystemPtr(system.handle, &handle);
            coreSystem.handle = handle;
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_System_GetEvent(ref Studio.System system, string path, out Studio.EventDescription eventDescription)
        {
            eventDescription = default;
            using (StringHelper.ThreadSafeEncoding encoder = StringHelper.GetFreeHelper())
            {
                RESULT result = Studio_System_GetEvent(system.handle, encoder.byteFromStringUTF8(path), out IntPtr handle);
                eventDescription.handle = handle;
                return result;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_System_GetListenerAttributes(ref Studio.System system, int listener, out ATTRIBUTES_3D attributes)
        {
            s_studioSystemGetListenerAttributesNullPtr ??= (delegate* unmanaged[Cdecl]<IntPtr, int, ATTRIBUTES_3D*, IntPtr, RESULT>)LoadStudioExport("FMOD_Studio_System_GetListenerAttributes");
            attributes = default;
            fixed (ATTRIBUTES_3D* attributesPtr = &attributes)
            {
                return s_studioSystemGetListenerAttributesNullPtr(system.handle, listener, attributesPtr, IntPtr.Zero);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_System_GetListenerAttributes(ref Studio.System system, int listener, out ATTRIBUTES_3D attributes, out VECTOR attenuationPosition)
        {
            s_studioSystemGetListenerAttributesPtr ??= (delegate* unmanaged[Cdecl]<IntPtr, int, ATTRIBUTES_3D*, VECTOR*, RESULT>)LoadStudioExport("FMOD_Studio_System_GetListenerAttributes");
            attributes = default;
            attenuationPosition = default;
            fixed (ATTRIBUTES_3D* attributesPtr = &attributes)
            fixed (VECTOR* attenuationPositionPtr = &attenuationPosition)
            {
                return s_studioSystemGetListenerAttributesPtr(system.handle, listener, attributesPtr, attenuationPositionPtr);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_System_GetListenerWeight(ref Studio.System system, int listener, out float weight)
        {
            s_studioSystemGetListenerWeightPtr ??= (delegate* unmanaged[Cdecl]<IntPtr, int, float*, RESULT>)LoadStudioExport("FMOD_Studio_System_GetListenerWeight");
            weight = 0.0f;
            fixed (float* weightPtr = &weight)
            {
                return s_studioSystemGetListenerWeightPtr(system.handle, listener, weightPtr);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_System_GetMemoryUsage(ref Studio.System system, out Studio.MEMORY_USAGE memoryUsage)
        {
            s_studioSystemGetMemoryUsagePtr ??= (delegate* unmanaged[Cdecl]<IntPtr, Studio.MEMORY_USAGE*, RESULT>)LoadStudioExport("FMOD_Studio_System_GetMemoryUsage");
            memoryUsage = default;
            fixed (Studio.MEMORY_USAGE* memoryUsagePtr = &memoryUsage)
            {
                return s_studioSystemGetMemoryUsagePtr(system.handle, memoryUsagePtr);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_System_GetNumListeners(ref Studio.System system, out int numListeners)
        {
            s_studioSystemGetNumListenersPtr ??= (delegate* unmanaged[Cdecl]<IntPtr, int*, RESULT>)LoadStudioExport("FMOD_Studio_System_GetNumListeners");
            numListeners = 0;
            fixed (int* numListenersPtr = &numListeners)
            {
                return s_studioSystemGetNumListenersPtr(system.handle, numListenersPtr);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_System_GetParameterByName(ref Studio.System system, string name, out float value, out float finalValue)
        {
            using (StringHelper.ThreadSafeEncoding encoder = StringHelper.GetFreeHelper())
            {
                return Studio_System_GetParameterByName(system.handle, encoder.byteFromStringUTF8(name), out value, out finalValue);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_System_GetParameterDescriptionByName(ref Studio.System system, string name, out Studio.PARAMETER_DESCRIPTION parameter)
        {
            using (StringHelper.ThreadSafeEncoding encoder = StringHelper.GetFreeHelper())
            {
                return Studio_System_GetParameterDescriptionByName(system.handle, encoder.byteFromStringUTF8(name), out parameter);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_System_GetParameterDescriptionCount(ref Studio.System system, out int count)
        {
            s_studioSystemGetParameterDescriptionCountPtr ??= (delegate* unmanaged[Cdecl]<IntPtr, int*, RESULT>)LoadStudioExport("FMOD_Studio_System_GetParameterDescriptionCount");
            count = 0;
            fixed (int* countPtr = &count)
            {
                return s_studioSystemGetParameterDescriptionCountPtr(system.handle, countPtr);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_System_FlushCommands(ref Studio.System system)
        {
            s_studioSystemFlushCommandsPtr ??= (delegate* unmanaged[Cdecl]<IntPtr, RESULT>)LoadStudioExport("FMOD_Studio_System_FlushCommands");
            return s_studioSystemFlushCommandsPtr(system.handle);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_System_FlushSampleLoading(ref Studio.System system)
        {
            s_studioSystemFlushSampleLoadingPtr ??= (delegate* unmanaged[Cdecl]<IntPtr, RESULT>)LoadStudioExport("FMOD_Studio_System_FlushSampleLoading");
            return s_studioSystemFlushSampleLoadingPtr(system.handle);
        }
    }
}
#endif
