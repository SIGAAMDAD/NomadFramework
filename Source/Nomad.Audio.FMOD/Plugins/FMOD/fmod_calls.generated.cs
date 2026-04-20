/*
==========================================================================
The Nomad Framework
Copyright (C) 2025-2026 Noah Van Til

This Source Code Form is subject to the terms of the Mozilla Public
License, v2. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.

This software is provided "as is", without warranty of any kind,
express or implied, including but not limited to the warranties
of merchantability, fitness for a particular purpose and noninfringement.
==========================================================================
*/

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#if false
namespace FMOD
{
    internal static unsafe partial class FMODNative
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_ParseID_Delegate_1( byte[] idString, out GUID id );
        private static Studio_ParseID_Delegate_1 s_Studio_ParseID_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_System_Create_Delegate_1( out IntPtr system, uint headerversion );
        private static Studio_System_Create_Delegate_1 s_Studio_System_Create_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate bool Studio_System_IsValid_Delegate_1( IntPtr system );
        private static Studio_System_IsValid_Delegate_1 s_Studio_System_IsValid_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_System_SetAdvancedSettings_Delegate_1( IntPtr system, ref Studio.ADVANCEDSETTINGS settings );
        private static Studio_System_SetAdvancedSettings_Delegate_1 s_Studio_System_SetAdvancedSettings_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_System_GetAdvancedSettings_Delegate_1( IntPtr system, out Studio.ADVANCEDSETTINGS settings );
        private static Studio_System_GetAdvancedSettings_Delegate_1 s_Studio_System_GetAdvancedSettings_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_System_Initialize_Delegate_1( IntPtr system, int maxchannels, Studio.INITFLAGS studioflags, FMOD.INITFLAGS flags, IntPtr extradriverdata );
        private static Studio_System_Initialize_Delegate_1 s_Studio_System_Initialize_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_System_Release_Delegate_1( IntPtr system );
        private static Studio_System_Release_Delegate_1 s_Studio_System_Release_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_System_Update_Delegate_1( IntPtr system );
        private static Studio_System_Update_Delegate_1 s_Studio_System_Update_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_System_GetCoreSystem_Delegate_1( IntPtr system, out IntPtr coresystem );
        private static Studio_System_GetCoreSystem_Delegate_1 s_Studio_System_GetCoreSystem_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_System_GetEvent_Delegate_1( IntPtr system, byte[] path, out IntPtr _event );
        private static Studio_System_GetEvent_Delegate_1 s_Studio_System_GetEvent_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_System_GetBus_Delegate_1( IntPtr system, byte[] path, out IntPtr bus );
        private static Studio_System_GetBus_Delegate_1 s_Studio_System_GetBus_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_System_GetVCA_Delegate_1( IntPtr system, byte[] path, out IntPtr vca );
        private static Studio_System_GetVCA_Delegate_1 s_Studio_System_GetVCA_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_System_GetBank_Delegate_1( IntPtr system, byte[] path, out IntPtr bank );
        private static Studio_System_GetBank_Delegate_1 s_Studio_System_GetBank_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_System_GetEventByID_Delegate_1( IntPtr system, ref GUID id, out IntPtr _event );
        private static Studio_System_GetEventByID_Delegate_1 s_Studio_System_GetEventByID_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_System_GetBusByID_Delegate_1( IntPtr system, ref GUID id, out IntPtr bus );
        private static Studio_System_GetBusByID_Delegate_1 s_Studio_System_GetBusByID_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_System_GetVCAByID_Delegate_1( IntPtr system, ref GUID id, out IntPtr vca );
        private static Studio_System_GetVCAByID_Delegate_1 s_Studio_System_GetVCAByID_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_System_GetBankByID_Delegate_1( IntPtr system, ref GUID id, out IntPtr bank );
        private static Studio_System_GetBankByID_Delegate_1 s_Studio_System_GetBankByID_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_System_GetSoundInfo_Delegate_1( IntPtr system, byte[] key, out Studio.SOUND_INFO info );
        private static Studio_System_GetSoundInfo_Delegate_1 s_Studio_System_GetSoundInfo_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_System_GetParameterDescriptionByName_Delegate_1( IntPtr system, byte[] name, out Studio.PARAMETER_DESCRIPTION parameter );
        private static Studio_System_GetParameterDescriptionByName_Delegate_1 s_Studio_System_GetParameterDescriptionByName_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_System_GetParameterDescriptionByID_Delegate_1( IntPtr system, Studio.PARAMETER_ID id, out Studio.PARAMETER_DESCRIPTION parameter );
        private static Studio_System_GetParameterDescriptionByID_Delegate_1 s_Studio_System_GetParameterDescriptionByID_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_System_GetParameterLabelByName_Delegate_1( IntPtr system, byte[] name, int labelindex, IntPtr label, int size, out int retrieved );
        private static Studio_System_GetParameterLabelByName_Delegate_1 s_Studio_System_GetParameterLabelByName_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_System_GetParameterLabelByID_Delegate_1( IntPtr system, Studio.PARAMETER_ID id, int labelindex, IntPtr label, int size, out int retrieved );
        private static Studio_System_GetParameterLabelByID_Delegate_1 s_Studio_System_GetParameterLabelByID_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_System_GetParameterByID_Delegate_1( IntPtr system, Studio.PARAMETER_ID id, out float value, out float finalvalue );
        private static Studio_System_GetParameterByID_Delegate_1 s_Studio_System_GetParameterByID_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_System_SetParameterByID_Delegate_1( IntPtr system, Studio.PARAMETER_ID id, float value, bool ignoreseekspeed );
        private static Studio_System_SetParameterByID_Delegate_1 s_Studio_System_SetParameterByID_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_System_SetParameterByIDWithLabel_Delegate_1( IntPtr system, Studio.PARAMETER_ID id, byte[] label, bool ignoreseekspeed );
        private static Studio_System_SetParameterByIDWithLabel_Delegate_1 s_Studio_System_SetParameterByIDWithLabel_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_System_SetParametersByIDs_Delegate_1( IntPtr system, Studio.PARAMETER_ID[] ids, float[] values, int count, bool ignoreseekspeed );
        private static Studio_System_SetParametersByIDs_Delegate_1 s_Studio_System_SetParametersByIDs_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_System_GetParameterByName_Delegate_1( IntPtr system, byte[] name, out float value, out float finalvalue );
        private static Studio_System_GetParameterByName_Delegate_1 s_Studio_System_GetParameterByName_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_System_SetParameterByName_Delegate_1( IntPtr system, byte[] name, float value, bool ignoreseekspeed );
        private static Studio_System_SetParameterByName_Delegate_1 s_Studio_System_SetParameterByName_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_System_SetParameterByNameWithLabel_Delegate_1( IntPtr system, byte[] name, byte[] label, bool ignoreseekspeed );
        private static Studio_System_SetParameterByNameWithLabel_Delegate_1 s_Studio_System_SetParameterByNameWithLabel_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_System_LookupID_Delegate_1( IntPtr system, byte[] path, out GUID id );
        private static Studio_System_LookupID_Delegate_1 s_Studio_System_LookupID_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_System_LookupPath_Delegate_1( IntPtr system, ref GUID id, IntPtr path, int size, out int retrieved );
        private static Studio_System_LookupPath_Delegate_1 s_Studio_System_LookupPath_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_System_GetNumListeners_Delegate_1( IntPtr system, out int numlisteners );
        private static Studio_System_GetNumListeners_Delegate_1 s_Studio_System_GetNumListeners_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_System_SetNumListeners_Delegate_1( IntPtr system, int numlisteners );
        private static Studio_System_SetNumListeners_Delegate_1 s_Studio_System_SetNumListeners_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_System_GetListenerAttributes_Delegate_1( IntPtr system, int listener, out ATTRIBUTES_3D attributes, IntPtr zero );
        private static Studio_System_GetListenerAttributes_Delegate_1 s_Studio_System_GetListenerAttributes_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_System_GetListenerAttributes_Delegate_2( IntPtr system, int listener, out ATTRIBUTES_3D attributes, out VECTOR attenuationposition );
        private static Studio_System_GetListenerAttributes_Delegate_2 s_Studio_System_GetListenerAttributes_2;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_System_SetListenerAttributes_Delegate_1( IntPtr system, int listener, ref ATTRIBUTES_3D attributes, IntPtr zero );
        private static Studio_System_SetListenerAttributes_Delegate_1 s_Studio_System_SetListenerAttributes_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_System_SetListenerAttributes_Delegate_2( IntPtr system, int listener, ref ATTRIBUTES_3D attributes, ref VECTOR attenuationposition );
        private static Studio_System_SetListenerAttributes_Delegate_2 s_Studio_System_SetListenerAttributes_2;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_System_GetListenerWeight_Delegate_1( IntPtr system, int listener, out float weight );
        private static Studio_System_GetListenerWeight_Delegate_1 s_Studio_System_GetListenerWeight_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_System_SetListenerWeight_Delegate_1( IntPtr system, int listener, float weight );
        private static Studio_System_SetListenerWeight_Delegate_1 s_Studio_System_SetListenerWeight_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_System_LoadBankFile_Delegate_1( IntPtr system, byte[] filename, Studio.LOAD_BANK_FLAGS flags, out IntPtr bank );
        private static Studio_System_LoadBankFile_Delegate_1 s_Studio_System_LoadBankFile_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_System_LoadBankMemory_Delegate_1( IntPtr system, IntPtr buffer, int length, Studio.LOAD_MEMORY_MODE mode, Studio.LOAD_BANK_FLAGS flags, out IntPtr bank );
        private static Studio_System_LoadBankMemory_Delegate_1 s_Studio_System_LoadBankMemory_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_System_LoadBankCustom_Delegate_1( IntPtr system, ref Studio.BANK_INFO info, Studio.LOAD_BANK_FLAGS flags, out IntPtr bank );
        private static Studio_System_LoadBankCustom_Delegate_1 s_Studio_System_LoadBankCustom_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_System_UnloadAll_Delegate_1( IntPtr system );
        private static Studio_System_UnloadAll_Delegate_1 s_Studio_System_UnloadAll_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_System_FlushCommands_Delegate_1( IntPtr system );
        private static Studio_System_FlushCommands_Delegate_1 s_Studio_System_FlushCommands_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_System_FlushSampleLoading_Delegate_1( IntPtr system );
        private static Studio_System_FlushSampleLoading_Delegate_1 s_Studio_System_FlushSampleLoading_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_System_StartCommandCapture_Delegate_1( IntPtr system, byte[] filename, Studio.COMMANDCAPTURE_FLAGS flags );
        private static Studio_System_StartCommandCapture_Delegate_1 s_Studio_System_StartCommandCapture_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_System_StopCommandCapture_Delegate_1( IntPtr system );
        private static Studio_System_StopCommandCapture_Delegate_1 s_Studio_System_StopCommandCapture_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_System_LoadCommandReplay_Delegate_1( IntPtr system, byte[] filename, Studio.COMMANDREPLAY_FLAGS flags, out IntPtr replay );
        private static Studio_System_LoadCommandReplay_Delegate_1 s_Studio_System_LoadCommandReplay_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_System_GetBankCount_Delegate_1( IntPtr system, out int count );
        private static Studio_System_GetBankCount_Delegate_1 s_Studio_System_GetBankCount_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_System_GetBankList_Delegate_1( IntPtr system, IntPtr[] array, int capacity, out int count );
        private static Studio_System_GetBankList_Delegate_1 s_Studio_System_GetBankList_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_System_GetParameterDescriptionCount_Delegate_1( IntPtr system, out int count );
        private static Studio_System_GetParameterDescriptionCount_Delegate_1 s_Studio_System_GetParameterDescriptionCount_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_System_GetParameterDescriptionList_Delegate_1( IntPtr system, [Out] Studio.PARAMETER_DESCRIPTION[] array, int capacity, out int count );
        private static Studio_System_GetParameterDescriptionList_Delegate_1 s_Studio_System_GetParameterDescriptionList_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_System_GetCPUUsage_Delegate_1( IntPtr system, out Studio.CPU_USAGE usage, out FMOD.CPU_USAGE usage_core );
        private static Studio_System_GetCPUUsage_Delegate_1 s_Studio_System_GetCPUUsage_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_System_GetBufferUsage_Delegate_1( IntPtr system, out Studio.BUFFER_USAGE usage );
        private static Studio_System_GetBufferUsage_Delegate_1 s_Studio_System_GetBufferUsage_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_System_ResetBufferUsage_Delegate_1( IntPtr system );
        private static Studio_System_ResetBufferUsage_Delegate_1 s_Studio_System_ResetBufferUsage_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_System_SetCallback_Delegate_1( IntPtr system, Studio.SYSTEM_CALLBACK callback, Studio.SYSTEM_CALLBACK_TYPE callbackmask );
        private static Studio_System_SetCallback_Delegate_1 s_Studio_System_SetCallback_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_System_GetUserData_Delegate_1( IntPtr system, out IntPtr userdata );
        private static Studio_System_GetUserData_Delegate_1 s_Studio_System_GetUserData_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_System_SetUserData_Delegate_1( IntPtr system, IntPtr userdata );
        private static Studio_System_SetUserData_Delegate_1 s_Studio_System_SetUserData_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_System_GetMemoryUsage_Delegate_1( IntPtr system, out Studio.MEMORY_USAGE memoryusage );
        private static Studio_System_GetMemoryUsage_Delegate_1 s_Studio_System_GetMemoryUsage_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate bool Studio_EventDescription_IsValid_Delegate_1( IntPtr eventdescription );
        private static Studio_EventDescription_IsValid_Delegate_1 s_Studio_EventDescription_IsValid_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_EventDescription_GetID_Delegate_1( IntPtr eventdescription, out GUID id );
        private static Studio_EventDescription_GetID_Delegate_1 s_Studio_EventDescription_GetID_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_EventDescription_GetPath_Delegate_1( IntPtr eventdescription, IntPtr path, int size, out int retrieved );
        private static Studio_EventDescription_GetPath_Delegate_1 s_Studio_EventDescription_GetPath_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_EventDescription_GetParameterDescriptionCount_Delegate_1( IntPtr eventdescription, out int count );
        private static Studio_EventDescription_GetParameterDescriptionCount_Delegate_1 s_Studio_EventDescription_GetParameterDescriptionCount_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_EventDescription_GetParameterDescriptionByIndex_Delegate_1( IntPtr eventdescription, int index, out Studio.PARAMETER_DESCRIPTION parameter );
        private static Studio_EventDescription_GetParameterDescriptionByIndex_Delegate_1 s_Studio_EventDescription_GetParameterDescriptionByIndex_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_EventDescription_GetParameterDescriptionByName_Delegate_1( IntPtr eventdescription, byte[] name, out Studio.PARAMETER_DESCRIPTION parameter );
        private static Studio_EventDescription_GetParameterDescriptionByName_Delegate_1 s_Studio_EventDescription_GetParameterDescriptionByName_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_EventDescription_GetParameterDescriptionByID_Delegate_1( IntPtr eventdescription, Studio.PARAMETER_ID id, out Studio.PARAMETER_DESCRIPTION parameter );
        private static Studio_EventDescription_GetParameterDescriptionByID_Delegate_1 s_Studio_EventDescription_GetParameterDescriptionByID_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_EventDescription_GetParameterLabelByIndex_Delegate_1( IntPtr eventdescription, int index, int labelindex, IntPtr label, int size, out int retrieved );
        private static Studio_EventDescription_GetParameterLabelByIndex_Delegate_1 s_Studio_EventDescription_GetParameterLabelByIndex_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_EventDescription_GetParameterLabelByName_Delegate_1( IntPtr eventdescription, byte[] name, int labelindex, IntPtr label, int size, out int retrieved );
        private static Studio_EventDescription_GetParameterLabelByName_Delegate_1 s_Studio_EventDescription_GetParameterLabelByName_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_EventDescription_GetParameterLabelByID_Delegate_1( IntPtr eventdescription, Studio.PARAMETER_ID id, int labelindex, IntPtr label, int size, out int retrieved );
        private static Studio_EventDescription_GetParameterLabelByID_Delegate_1 s_Studio_EventDescription_GetParameterLabelByID_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_EventDescription_GetUserPropertyCount_Delegate_1( IntPtr eventdescription, out int count );
        private static Studio_EventDescription_GetUserPropertyCount_Delegate_1 s_Studio_EventDescription_GetUserPropertyCount_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_EventDescription_GetUserPropertyByIndex_Delegate_1( IntPtr eventdescription, int index, out Studio.USER_PROPERTY property );
        private static Studio_EventDescription_GetUserPropertyByIndex_Delegate_1 s_Studio_EventDescription_GetUserPropertyByIndex_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_EventDescription_GetUserProperty_Delegate_1( IntPtr eventdescription, byte[] name, out Studio.USER_PROPERTY property );
        private static Studio_EventDescription_GetUserProperty_Delegate_1 s_Studio_EventDescription_GetUserProperty_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_EventDescription_GetLength_Delegate_1( IntPtr eventdescription, out int length );
        private static Studio_EventDescription_GetLength_Delegate_1 s_Studio_EventDescription_GetLength_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_EventDescription_GetMinMaxDistance_Delegate_1( IntPtr eventdescription, out float min, out float max );
        private static Studio_EventDescription_GetMinMaxDistance_Delegate_1 s_Studio_EventDescription_GetMinMaxDistance_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_EventDescription_GetSoundSize_Delegate_1( IntPtr eventdescription, out float size );
        private static Studio_EventDescription_GetSoundSize_Delegate_1 s_Studio_EventDescription_GetSoundSize_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_EventDescription_IsSnapshot_Delegate_1( IntPtr eventdescription, out bool snapshot );
        private static Studio_EventDescription_IsSnapshot_Delegate_1 s_Studio_EventDescription_IsSnapshot_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_EventDescription_IsOneshot_Delegate_1( IntPtr eventdescription, out bool oneshot );
        private static Studio_EventDescription_IsOneshot_Delegate_1 s_Studio_EventDescription_IsOneshot_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_EventDescription_IsStream_Delegate_1( IntPtr eventdescription, out bool isStream );
        private static Studio_EventDescription_IsStream_Delegate_1 s_Studio_EventDescription_IsStream_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_EventDescription_Is3D_Delegate_1( IntPtr eventdescription, out bool is3D );
        private static Studio_EventDescription_Is3D_Delegate_1 s_Studio_EventDescription_Is3D_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_EventDescription_IsDopplerEnabled_Delegate_1( IntPtr eventdescription, out bool doppler );
        private static Studio_EventDescription_IsDopplerEnabled_Delegate_1 s_Studio_EventDescription_IsDopplerEnabled_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_EventDescription_HasSustainPoint_Delegate_1( IntPtr eventdescription, out bool sustainPoint );
        private static Studio_EventDescription_HasSustainPoint_Delegate_1 s_Studio_EventDescription_HasSustainPoint_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_EventDescription_CreateInstance_Delegate_1( IntPtr eventdescription, out IntPtr instance );
        private static Studio_EventDescription_CreateInstance_Delegate_1 s_Studio_EventDescription_CreateInstance_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_EventDescription_GetInstanceCount_Delegate_1( IntPtr eventdescription, out int count );
        private static Studio_EventDescription_GetInstanceCount_Delegate_1 s_Studio_EventDescription_GetInstanceCount_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_EventDescription_GetInstanceList_Delegate_1( IntPtr eventdescription, IntPtr[] array, int capacity, out int count );
        private static Studio_EventDescription_GetInstanceList_Delegate_1 s_Studio_EventDescription_GetInstanceList_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_EventDescription_LoadSampleData_Delegate_1( IntPtr eventdescription );
        private static Studio_EventDescription_LoadSampleData_Delegate_1 s_Studio_EventDescription_LoadSampleData_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_EventDescription_UnloadSampleData_Delegate_1( IntPtr eventdescription );
        private static Studio_EventDescription_UnloadSampleData_Delegate_1 s_Studio_EventDescription_UnloadSampleData_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_EventDescription_GetSampleLoadingState_Delegate_1( IntPtr eventdescription, out Studio.LOADING_STATE state );
        private static Studio_EventDescription_GetSampleLoadingState_Delegate_1 s_Studio_EventDescription_GetSampleLoadingState_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_EventDescription_ReleaseAllInstances_Delegate_1( IntPtr eventdescription );
        private static Studio_EventDescription_ReleaseAllInstances_Delegate_1 s_Studio_EventDescription_ReleaseAllInstances_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_EventDescription_SetCallback_Delegate_1( IntPtr eventdescription, Studio.EVENT_CALLBACK callback, Studio.EVENT_CALLBACK_TYPE callbackmask );
        private static Studio_EventDescription_SetCallback_Delegate_1 s_Studio_EventDescription_SetCallback_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_EventDescription_GetUserData_Delegate_1( IntPtr eventdescription, out IntPtr userdata );
        private static Studio_EventDescription_GetUserData_Delegate_1 s_Studio_EventDescription_GetUserData_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_EventDescription_SetUserData_Delegate_1( IntPtr eventdescription, IntPtr userdata );
        private static Studio_EventDescription_SetUserData_Delegate_1 s_Studio_EventDescription_SetUserData_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate bool Studio_EventInstance_IsValid_Delegate_1( IntPtr _event );
        private static Studio_EventInstance_IsValid_Delegate_1 s_Studio_EventInstance_IsValid_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_EventInstance_GetDescription_Delegate_1( IntPtr _event, out IntPtr description );
        private static Studio_EventInstance_GetDescription_Delegate_1 s_Studio_EventInstance_GetDescription_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_EventInstance_GetSystem_Delegate_1( IntPtr _event, out IntPtr system );
        private static Studio_EventInstance_GetSystem_Delegate_1 s_Studio_EventInstance_GetSystem_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_EventInstance_GetVolume_Delegate_1( IntPtr _event, out float volume, IntPtr zero );
        private static Studio_EventInstance_GetVolume_Delegate_1 s_Studio_EventInstance_GetVolume_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_EventInstance_GetVolume_Delegate_2( IntPtr _event, out float volume, out float finalvolume );
        private static Studio_EventInstance_GetVolume_Delegate_2 s_Studio_EventInstance_GetVolume_2;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_EventInstance_SetVolume_Delegate_1( IntPtr _event, float volume );
        private static Studio_EventInstance_SetVolume_Delegate_1 s_Studio_EventInstance_SetVolume_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_EventInstance_GetPitch_Delegate_1( IntPtr _event, out float pitch, IntPtr zero );
        private static Studio_EventInstance_GetPitch_Delegate_1 s_Studio_EventInstance_GetPitch_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_EventInstance_GetPitch_Delegate_2( IntPtr _event, out float pitch, out float finalpitch );
        private static Studio_EventInstance_GetPitch_Delegate_2 s_Studio_EventInstance_GetPitch_2;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_EventInstance_SetPitch_Delegate_1( IntPtr _event, float pitch );
        private static Studio_EventInstance_SetPitch_Delegate_1 s_Studio_EventInstance_SetPitch_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_EventInstance_Get3DAttributes_Delegate_1( IntPtr _event, out ATTRIBUTES_3D attributes );
        private static Studio_EventInstance_Get3DAttributes_Delegate_1 s_Studio_EventInstance_Get3DAttributes_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_EventInstance_Set3DAttributes_Delegate_1( IntPtr _event, ref ATTRIBUTES_3D attributes );
        private static Studio_EventInstance_Set3DAttributes_Delegate_1 s_Studio_EventInstance_Set3DAttributes_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_EventInstance_GetListenerMask_Delegate_1( IntPtr _event, out uint mask );
        private static Studio_EventInstance_GetListenerMask_Delegate_1 s_Studio_EventInstance_GetListenerMask_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_EventInstance_SetListenerMask_Delegate_1( IntPtr _event, uint mask );
        private static Studio_EventInstance_SetListenerMask_Delegate_1 s_Studio_EventInstance_SetListenerMask_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_EventInstance_GetProperty_Delegate_1( IntPtr _event, Studio.EVENT_PROPERTY index, out float value );
        private static Studio_EventInstance_GetProperty_Delegate_1 s_Studio_EventInstance_GetProperty_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_EventInstance_SetProperty_Delegate_1( IntPtr _event, Studio.EVENT_PROPERTY index, float value );
        private static Studio_EventInstance_SetProperty_Delegate_1 s_Studio_EventInstance_SetProperty_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_EventInstance_GetReverbLevel_Delegate_1( IntPtr _event, int index, out float level );
        private static Studio_EventInstance_GetReverbLevel_Delegate_1 s_Studio_EventInstance_GetReverbLevel_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_EventInstance_SetReverbLevel_Delegate_1( IntPtr _event, int index, float level );
        private static Studio_EventInstance_SetReverbLevel_Delegate_1 s_Studio_EventInstance_SetReverbLevel_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_EventInstance_GetPaused_Delegate_1( IntPtr _event, out bool paused );
        private static Studio_EventInstance_GetPaused_Delegate_1 s_Studio_EventInstance_GetPaused_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_EventInstance_SetPaused_Delegate_1( IntPtr _event, bool paused );
        private static Studio_EventInstance_SetPaused_Delegate_1 s_Studio_EventInstance_SetPaused_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_EventInstance_Start_Delegate_1( IntPtr _event );
        private static Studio_EventInstance_Start_Delegate_1 s_Studio_EventInstance_Start_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_EventInstance_Stop_Delegate_1( IntPtr _event, Studio.STOP_MODE mode );
        private static Studio_EventInstance_Stop_Delegate_1 s_Studio_EventInstance_Stop_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_EventInstance_GetTimelinePosition_Delegate_1( IntPtr _event, out int position );
        private static Studio_EventInstance_GetTimelinePosition_Delegate_1 s_Studio_EventInstance_GetTimelinePosition_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_EventInstance_SetTimelinePosition_Delegate_1( IntPtr _event, int position );
        private static Studio_EventInstance_SetTimelinePosition_Delegate_1 s_Studio_EventInstance_SetTimelinePosition_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_EventInstance_GetPlaybackState_Delegate_1( IntPtr _event, out Studio.PLAYBACK_STATE state );
        private static Studio_EventInstance_GetPlaybackState_Delegate_1 s_Studio_EventInstance_GetPlaybackState_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_EventInstance_GetChannelGroup_Delegate_1( IntPtr _event, out IntPtr group );
        private static Studio_EventInstance_GetChannelGroup_Delegate_1 s_Studio_EventInstance_GetChannelGroup_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_EventInstance_GetMinMaxDistance_Delegate_1( IntPtr _event, out float min, out float max );
        private static Studio_EventInstance_GetMinMaxDistance_Delegate_1 s_Studio_EventInstance_GetMinMaxDistance_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_EventInstance_Release_Delegate_1( IntPtr _event );
        private static Studio_EventInstance_Release_Delegate_1 s_Studio_EventInstance_Release_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_EventInstance_IsVirtual_Delegate_1( IntPtr _event, out bool virtualstate );
        private static Studio_EventInstance_IsVirtual_Delegate_1 s_Studio_EventInstance_IsVirtual_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_EventInstance_GetParameterByName_Delegate_1( IntPtr _event, byte[] name, out float value, out float finalvalue );
        private static Studio_EventInstance_GetParameterByName_Delegate_1 s_Studio_EventInstance_GetParameterByName_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_EventInstance_SetParameterByName_Delegate_1( IntPtr _event, byte[] name, float value, bool ignoreseekspeed );
        private static Studio_EventInstance_SetParameterByName_Delegate_1 s_Studio_EventInstance_SetParameterByName_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_EventInstance_SetParameterByNameWithLabel_Delegate_1( IntPtr _event, byte[] name, byte[] label, bool ignoreseekspeed );
        private static Studio_EventInstance_SetParameterByNameWithLabel_Delegate_1 s_Studio_EventInstance_SetParameterByNameWithLabel_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_EventInstance_GetParameterByID_Delegate_1( IntPtr _event, Studio.PARAMETER_ID id, out float value, out float finalvalue );
        private static Studio_EventInstance_GetParameterByID_Delegate_1 s_Studio_EventInstance_GetParameterByID_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_EventInstance_SetParameterByID_Delegate_1( IntPtr _event, Studio.PARAMETER_ID id, float value, bool ignoreseekspeed );
        private static Studio_EventInstance_SetParameterByID_Delegate_1 s_Studio_EventInstance_SetParameterByID_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_EventInstance_SetParameterByIDWithLabel_Delegate_1( IntPtr _event, Studio.PARAMETER_ID id, byte[] label, bool ignoreseekspeed );
        private static Studio_EventInstance_SetParameterByIDWithLabel_Delegate_1 s_Studio_EventInstance_SetParameterByIDWithLabel_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_EventInstance_SetParametersByIDs_Delegate_1( IntPtr _event, Studio.PARAMETER_ID[] ids, float[] values, int count, bool ignoreseekspeed );
        private static Studio_EventInstance_SetParametersByIDs_Delegate_1 s_Studio_EventInstance_SetParametersByIDs_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_EventInstance_KeyOff_Delegate_1( IntPtr _event );
        private static Studio_EventInstance_KeyOff_Delegate_1 s_Studio_EventInstance_KeyOff_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_EventInstance_SetCallback_Delegate_1( IntPtr _event, Studio.EVENT_CALLBACK callback, Studio.EVENT_CALLBACK_TYPE callbackmask );
        private static Studio_EventInstance_SetCallback_Delegate_1 s_Studio_EventInstance_SetCallback_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_EventInstance_GetUserData_Delegate_1( IntPtr _event, out IntPtr userdata );
        private static Studio_EventInstance_GetUserData_Delegate_1 s_Studio_EventInstance_GetUserData_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_EventInstance_SetUserData_Delegate_1( IntPtr _event, IntPtr userdata );
        private static Studio_EventInstance_SetUserData_Delegate_1 s_Studio_EventInstance_SetUserData_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_EventInstance_GetCPUUsage_Delegate_1( IntPtr _event, out uint exclusive, out uint inclusive );
        private static Studio_EventInstance_GetCPUUsage_Delegate_1 s_Studio_EventInstance_GetCPUUsage_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_EventInstance_GetMemoryUsage_Delegate_1( IntPtr _event, out Studio.MEMORY_USAGE memoryusage );
        private static Studio_EventInstance_GetMemoryUsage_Delegate_1 s_Studio_EventInstance_GetMemoryUsage_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate bool Studio_Bus_IsValid_Delegate_1( IntPtr bus );
        private static Studio_Bus_IsValid_Delegate_1 s_Studio_Bus_IsValid_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_Bus_GetID_Delegate_1( IntPtr bus, out GUID id );
        private static Studio_Bus_GetID_Delegate_1 s_Studio_Bus_GetID_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_Bus_GetPath_Delegate_1( IntPtr bus, IntPtr path, int size, out int retrieved );
        private static Studio_Bus_GetPath_Delegate_1 s_Studio_Bus_GetPath_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_Bus_GetVolume_Delegate_1( IntPtr bus, out float volume, out float finalvolume );
        private static Studio_Bus_GetVolume_Delegate_1 s_Studio_Bus_GetVolume_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_Bus_SetVolume_Delegate_1( IntPtr bus, float volume );
        private static Studio_Bus_SetVolume_Delegate_1 s_Studio_Bus_SetVolume_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_Bus_GetPaused_Delegate_1( IntPtr bus, out bool paused );
        private static Studio_Bus_GetPaused_Delegate_1 s_Studio_Bus_GetPaused_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_Bus_SetPaused_Delegate_1( IntPtr bus, bool paused );
        private static Studio_Bus_SetPaused_Delegate_1 s_Studio_Bus_SetPaused_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_Bus_GetMute_Delegate_1( IntPtr bus, out bool mute );
        private static Studio_Bus_GetMute_Delegate_1 s_Studio_Bus_GetMute_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_Bus_SetMute_Delegate_1( IntPtr bus, bool mute );
        private static Studio_Bus_SetMute_Delegate_1 s_Studio_Bus_SetMute_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_Bus_StopAllEvents_Delegate_1( IntPtr bus, Studio.STOP_MODE mode );
        private static Studio_Bus_StopAllEvents_Delegate_1 s_Studio_Bus_StopAllEvents_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_Bus_LockChannelGroup_Delegate_1( IntPtr bus );
        private static Studio_Bus_LockChannelGroup_Delegate_1 s_Studio_Bus_LockChannelGroup_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_Bus_UnlockChannelGroup_Delegate_1( IntPtr bus );
        private static Studio_Bus_UnlockChannelGroup_Delegate_1 s_Studio_Bus_UnlockChannelGroup_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_Bus_GetChannelGroup_Delegate_1( IntPtr bus, out IntPtr group );
        private static Studio_Bus_GetChannelGroup_Delegate_1 s_Studio_Bus_GetChannelGroup_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_Bus_GetCPUUsage_Delegate_1( IntPtr bus, out uint exclusive, out uint inclusive );
        private static Studio_Bus_GetCPUUsage_Delegate_1 s_Studio_Bus_GetCPUUsage_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_Bus_GetMemoryUsage_Delegate_1( IntPtr bus, out Studio.MEMORY_USAGE memoryusage );
        private static Studio_Bus_GetMemoryUsage_Delegate_1 s_Studio_Bus_GetMemoryUsage_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_Bus_GetPortIndex_Delegate_1( IntPtr bus, out ulong index );
        private static Studio_Bus_GetPortIndex_Delegate_1 s_Studio_Bus_GetPortIndex_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_Bus_SetPortIndex_Delegate_1( IntPtr bus, ulong index );
        private static Studio_Bus_SetPortIndex_Delegate_1 s_Studio_Bus_SetPortIndex_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate bool Studio_VCA_IsValid_Delegate_1( IntPtr vca );
        private static Studio_VCA_IsValid_Delegate_1 s_Studio_VCA_IsValid_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_VCA_GetID_Delegate_1( IntPtr vca, out GUID id );
        private static Studio_VCA_GetID_Delegate_1 s_Studio_VCA_GetID_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_VCA_GetPath_Delegate_1( IntPtr vca, IntPtr path, int size, out int retrieved );
        private static Studio_VCA_GetPath_Delegate_1 s_Studio_VCA_GetPath_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_VCA_GetVolume_Delegate_1( IntPtr vca, out float volume, out float finalvolume );
        private static Studio_VCA_GetVolume_Delegate_1 s_Studio_VCA_GetVolume_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_VCA_SetVolume_Delegate_1( IntPtr vca, float volume );
        private static Studio_VCA_SetVolume_Delegate_1 s_Studio_VCA_SetVolume_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate bool Studio_Bank_IsValid_Delegate_1( IntPtr bank );
        private static Studio_Bank_IsValid_Delegate_1 s_Studio_Bank_IsValid_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_Bank_GetID_Delegate_1( IntPtr bank, out GUID id );
        private static Studio_Bank_GetID_Delegate_1 s_Studio_Bank_GetID_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_Bank_GetPath_Delegate_1( IntPtr bank, IntPtr path, int size, out int retrieved );
        private static Studio_Bank_GetPath_Delegate_1 s_Studio_Bank_GetPath_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_Bank_Unload_Delegate_1( IntPtr bank );
        private static Studio_Bank_Unload_Delegate_1 s_Studio_Bank_Unload_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_Bank_LoadSampleData_Delegate_1( IntPtr bank );
        private static Studio_Bank_LoadSampleData_Delegate_1 s_Studio_Bank_LoadSampleData_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_Bank_UnloadSampleData_Delegate_1( IntPtr bank );
        private static Studio_Bank_UnloadSampleData_Delegate_1 s_Studio_Bank_UnloadSampleData_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_Bank_GetLoadingState_Delegate_1( IntPtr bank, out Studio.LOADING_STATE state );
        private static Studio_Bank_GetLoadingState_Delegate_1 s_Studio_Bank_GetLoadingState_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_Bank_GetSampleLoadingState_Delegate_1( IntPtr bank, out Studio.LOADING_STATE state );
        private static Studio_Bank_GetSampleLoadingState_Delegate_1 s_Studio_Bank_GetSampleLoadingState_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_Bank_GetStringCount_Delegate_1( IntPtr bank, out int count );
        private static Studio_Bank_GetStringCount_Delegate_1 s_Studio_Bank_GetStringCount_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_Bank_GetStringInfo_Delegate_1( IntPtr bank, int index, out GUID id, IntPtr path, int size, out int retrieved );
        private static Studio_Bank_GetStringInfo_Delegate_1 s_Studio_Bank_GetStringInfo_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_Bank_GetEventCount_Delegate_1( IntPtr bank, out int count );
        private static Studio_Bank_GetEventCount_Delegate_1 s_Studio_Bank_GetEventCount_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_Bank_GetEventList_Delegate_1( IntPtr bank, IntPtr[] array, int capacity, out int count );
        private static Studio_Bank_GetEventList_Delegate_1 s_Studio_Bank_GetEventList_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_Bank_GetBusCount_Delegate_1( IntPtr bank, out int count );
        private static Studio_Bank_GetBusCount_Delegate_1 s_Studio_Bank_GetBusCount_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_Bank_GetBusList_Delegate_1( IntPtr bank, IntPtr[] array, int capacity, out int count );
        private static Studio_Bank_GetBusList_Delegate_1 s_Studio_Bank_GetBusList_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_Bank_GetVCACount_Delegate_1( IntPtr bank, out int count );
        private static Studio_Bank_GetVCACount_Delegate_1 s_Studio_Bank_GetVCACount_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_Bank_GetVCAList_Delegate_1( IntPtr bank, IntPtr[] array, int capacity, out int count );
        private static Studio_Bank_GetVCAList_Delegate_1 s_Studio_Bank_GetVCAList_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_Bank_GetUserData_Delegate_1( IntPtr bank, out IntPtr userdata );
        private static Studio_Bank_GetUserData_Delegate_1 s_Studio_Bank_GetUserData_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_Bank_SetUserData_Delegate_1( IntPtr bank, IntPtr userdata );
        private static Studio_Bank_SetUserData_Delegate_1 s_Studio_Bank_SetUserData_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate bool Studio_CommandReplay_IsValid_Delegate_1( IntPtr replay );
        private static Studio_CommandReplay_IsValid_Delegate_1 s_Studio_CommandReplay_IsValid_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_CommandReplay_GetSystem_Delegate_1( IntPtr replay, out IntPtr system );
        private static Studio_CommandReplay_GetSystem_Delegate_1 s_Studio_CommandReplay_GetSystem_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_CommandReplay_GetLength_Delegate_1( IntPtr replay, out float length );
        private static Studio_CommandReplay_GetLength_Delegate_1 s_Studio_CommandReplay_GetLength_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_CommandReplay_GetCommandCount_Delegate_1( IntPtr replay, out int count );
        private static Studio_CommandReplay_GetCommandCount_Delegate_1 s_Studio_CommandReplay_GetCommandCount_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_CommandReplay_GetCommandInfo_Delegate_1( IntPtr replay, int commandindex, out Studio.COMMAND_INFO info );
        private static Studio_CommandReplay_GetCommandInfo_Delegate_1 s_Studio_CommandReplay_GetCommandInfo_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_CommandReplay_GetCommandString_Delegate_1( IntPtr replay, int commandIndex, IntPtr buffer, int length );
        private static Studio_CommandReplay_GetCommandString_Delegate_1 s_Studio_CommandReplay_GetCommandString_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_CommandReplay_GetCommandAtTime_Delegate_1( IntPtr replay, float time, out int commandIndex );
        private static Studio_CommandReplay_GetCommandAtTime_Delegate_1 s_Studio_CommandReplay_GetCommandAtTime_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_CommandReplay_SetBankPath_Delegate_1( IntPtr replay, byte[] bankPath );
        private static Studio_CommandReplay_SetBankPath_Delegate_1 s_Studio_CommandReplay_SetBankPath_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_CommandReplay_Start_Delegate_1( IntPtr replay );
        private static Studio_CommandReplay_Start_Delegate_1 s_Studio_CommandReplay_Start_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_CommandReplay_Stop_Delegate_1( IntPtr replay );
        private static Studio_CommandReplay_Stop_Delegate_1 s_Studio_CommandReplay_Stop_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_CommandReplay_SeekToTime_Delegate_1( IntPtr replay, float time );
        private static Studio_CommandReplay_SeekToTime_Delegate_1 s_Studio_CommandReplay_SeekToTime_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_CommandReplay_SeekToCommand_Delegate_1( IntPtr replay, int commandIndex );
        private static Studio_CommandReplay_SeekToCommand_Delegate_1 s_Studio_CommandReplay_SeekToCommand_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_CommandReplay_GetPaused_Delegate_1( IntPtr replay, out bool paused );
        private static Studio_CommandReplay_GetPaused_Delegate_1 s_Studio_CommandReplay_GetPaused_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_CommandReplay_SetPaused_Delegate_1( IntPtr replay, bool paused );
        private static Studio_CommandReplay_SetPaused_Delegate_1 s_Studio_CommandReplay_SetPaused_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_CommandReplay_GetPlaybackState_Delegate_1( IntPtr replay, out Studio.PLAYBACK_STATE state );
        private static Studio_CommandReplay_GetPlaybackState_Delegate_1 s_Studio_CommandReplay_GetPlaybackState_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_CommandReplay_GetCurrentCommand_Delegate_1( IntPtr replay, out int commandIndex, out float currentTime );
        private static Studio_CommandReplay_GetCurrentCommand_Delegate_1 s_Studio_CommandReplay_GetCurrentCommand_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_CommandReplay_Release_Delegate_1( IntPtr replay );
        private static Studio_CommandReplay_Release_Delegate_1 s_Studio_CommandReplay_Release_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_CommandReplay_SetFrameCallback_Delegate_1( IntPtr replay, Studio.COMMANDREPLAY_FRAME_CALLBACK callback );
        private static Studio_CommandReplay_SetFrameCallback_Delegate_1 s_Studio_CommandReplay_SetFrameCallback_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_CommandReplay_SetLoadBankCallback_Delegate_1( IntPtr replay, Studio.COMMANDREPLAY_LOAD_BANK_CALLBACK callback );
        private static Studio_CommandReplay_SetLoadBankCallback_Delegate_1 s_Studio_CommandReplay_SetLoadBankCallback_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_CommandReplay_SetCreateInstanceCallback_Delegate_1( IntPtr replay, Studio.COMMANDREPLAY_CREATE_INSTANCE_CALLBACK callback );
        private static Studio_CommandReplay_SetCreateInstanceCallback_Delegate_1 s_Studio_CommandReplay_SetCreateInstanceCallback_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_CommandReplay_GetUserData_Delegate_1( IntPtr replay, out IntPtr userdata );
        private static Studio_CommandReplay_GetUserData_Delegate_1 s_Studio_CommandReplay_GetUserData_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Studio_CommandReplay_SetUserData_Delegate_1( IntPtr replay, IntPtr userdata );
        private static Studio_CommandReplay_SetUserData_Delegate_1 s_Studio_CommandReplay_SetUserData_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT System_Create_Delegate_1(out IntPtr system, uint headerversion);
        private static System_Create_Delegate_1 s_System_Create_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Memory_Initialize_Delegate_1(IntPtr poolmem, int poollen, MEMORY_ALLOC_CALLBACK useralloc, MEMORY_REALLOC_CALLBACK userrealloc, MEMORY_FREE_CALLBACK userfree, MEMORY_TYPE memtypeflags);
        private static Memory_Initialize_Delegate_1 s_Memory_Initialize_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Memory_GetStats_Delegate_1(out int currentalloced, out int maxalloced, bool blocking);
        private static Memory_GetStats_Delegate_1 s_Memory_GetStats_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Debug_Initialize_Delegate_1(DEBUG_FLAGS flags, DEBUG_MODE mode, DEBUG_CALLBACK callback, byte[] filename);
        private static Debug_Initialize_Delegate_1 s_Debug_Initialize_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Thread_SetAttributes_Delegate_1(THREAD_TYPE type, THREAD_AFFINITY affinity, THREAD_PRIORITY priority, THREAD_STACK_SIZE stacksize);
        private static Thread_SetAttributes_Delegate_1 s_Thread_SetAttributes_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT System_Release_Delegate_1(IntPtr system);
        private static System_Release_Delegate_1 s_System_Release_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT System_SetOutput_Delegate_1(IntPtr system, OUTPUTTYPE output);
        private static System_SetOutput_Delegate_1 s_System_SetOutput_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT System_GetOutput_Delegate_1(IntPtr system, out OUTPUTTYPE output);
        private static System_GetOutput_Delegate_1 s_System_GetOutput_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT System_GetNumDrivers_Delegate_1(IntPtr system, out int numdrivers);
        private static System_GetNumDrivers_Delegate_1 s_System_GetNumDrivers_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT System_GetDriverInfo_Delegate_1(IntPtr system, int id, IntPtr name, int namelen, out Guid guid, out int systemrate, out SPEAKERMODE speakermode, out int speakermodechannels);
        private static System_GetDriverInfo_Delegate_1 s_System_GetDriverInfo_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT System_SetDriver_Delegate_1(IntPtr system, int driver);
        private static System_SetDriver_Delegate_1 s_System_SetDriver_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT System_GetDriver_Delegate_1(IntPtr system, out int driver);
        private static System_GetDriver_Delegate_1 s_System_GetDriver_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT System_SetSoftwareChannels_Delegate_1(IntPtr system, int numsoftwarechannels);
        private static System_SetSoftwareChannels_Delegate_1 s_System_SetSoftwareChannels_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT System_GetSoftwareChannels_Delegate_1(IntPtr system, out int numsoftwarechannels);
        private static System_GetSoftwareChannels_Delegate_1 s_System_GetSoftwareChannels_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT System_SetSoftwareFormat_Delegate_1(IntPtr system, int samplerate, SPEAKERMODE speakermode, int numrawspeakers);
        private static System_SetSoftwareFormat_Delegate_1 s_System_SetSoftwareFormat_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT System_GetSoftwareFormat_Delegate_1(IntPtr system, out int samplerate, out SPEAKERMODE speakermode, out int numrawspeakers);
        private static System_GetSoftwareFormat_Delegate_1 s_System_GetSoftwareFormat_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT System_SetDSPBufferSize_Delegate_1(IntPtr system, uint bufferlength, int numbuffers);
        private static System_SetDSPBufferSize_Delegate_1 s_System_SetDSPBufferSize_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT System_GetDSPBufferSize_Delegate_1(IntPtr system, out uint bufferlength, out int numbuffers);
        private static System_GetDSPBufferSize_Delegate_1 s_System_GetDSPBufferSize_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT System_SetFileSystem_Delegate_1(IntPtr system, FILE_OPEN_CALLBACK useropen, FILE_CLOSE_CALLBACK userclose, FILE_READ_CALLBACK userread, FILE_SEEK_CALLBACK userseek, FILE_ASYNCREAD_CALLBACK userasyncread, FILE_ASYNCCANCEL_CALLBACK userasynccancel, int blockalign);
        private static System_SetFileSystem_Delegate_1 s_System_SetFileSystem_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT System_AttachFileSystem_Delegate_1(IntPtr system, FILE_OPEN_CALLBACK useropen, FILE_CLOSE_CALLBACK userclose, FILE_READ_CALLBACK userread, FILE_SEEK_CALLBACK userseek);
        private static System_AttachFileSystem_Delegate_1 s_System_AttachFileSystem_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT System_SetAdvancedSettings_Delegate_1(IntPtr system, ref ADVANCEDSETTINGS settings);
        private static System_SetAdvancedSettings_Delegate_1 s_System_SetAdvancedSettings_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT System_GetAdvancedSettings_Delegate_1(IntPtr system, ref ADVANCEDSETTINGS settings);
        private static System_GetAdvancedSettings_Delegate_1 s_System_GetAdvancedSettings_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT System_SetCallback_Delegate_1(IntPtr system, SYSTEM_CALLBACK callback, SYSTEM_CALLBACK_TYPE callbackmask);
        private static System_SetCallback_Delegate_1 s_System_SetCallback_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT System_SetPluginPath_Delegate_1(IntPtr system, byte[] path);
        private static System_SetPluginPath_Delegate_1 s_System_SetPluginPath_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT System_LoadPlugin_Delegate_1(IntPtr system, byte[] filename, out uint handle, uint priority);
        private static System_LoadPlugin_Delegate_1 s_System_LoadPlugin_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT System_UnloadPlugin_Delegate_1(IntPtr system, uint handle);
        private static System_UnloadPlugin_Delegate_1 s_System_UnloadPlugin_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT System_GetNumNestedPlugins_Delegate_1(IntPtr system, uint handle, out int count);
        private static System_GetNumNestedPlugins_Delegate_1 s_System_GetNumNestedPlugins_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT System_GetNestedPlugin_Delegate_1(IntPtr system, uint handle, int index, out uint nestedhandle);
        private static System_GetNestedPlugin_Delegate_1 s_System_GetNestedPlugin_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT System_GetNumPlugins_Delegate_1(IntPtr system, PLUGINTYPE plugintype, out int numplugins);
        private static System_GetNumPlugins_Delegate_1 s_System_GetNumPlugins_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT System_GetPluginHandle_Delegate_1(IntPtr system, PLUGINTYPE plugintype, int index, out uint handle);
        private static System_GetPluginHandle_Delegate_1 s_System_GetPluginHandle_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT System_GetPluginInfo_Delegate_1(IntPtr system, uint handle, out PLUGINTYPE plugintype, IntPtr name, int namelen, out uint version);
        private static System_GetPluginInfo_Delegate_1 s_System_GetPluginInfo_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT System_SetOutputByPlugin_Delegate_1(IntPtr system, uint handle);
        private static System_SetOutputByPlugin_Delegate_1 s_System_SetOutputByPlugin_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT System_GetOutputByPlugin_Delegate_1(IntPtr system, out uint handle);
        private static System_GetOutputByPlugin_Delegate_1 s_System_GetOutputByPlugin_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT System_CreateDSPByPlugin_Delegate_1(IntPtr system, uint handle, out IntPtr dsp);
        private static System_CreateDSPByPlugin_Delegate_1 s_System_CreateDSPByPlugin_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT System_GetDSPInfoByPlugin_Delegate_1(IntPtr system, uint handle, out IntPtr description);
        private static System_GetDSPInfoByPlugin_Delegate_1 s_System_GetDSPInfoByPlugin_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT System_RegisterDSP_Delegate_1(IntPtr system, ref DSP_DESCRIPTION description, out uint handle);
        private static System_RegisterDSP_Delegate_1 s_System_RegisterDSP_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT System_Init_Delegate_1(IntPtr system, int maxchannels, INITFLAGS flags, IntPtr extradriverdata);
        private static System_Init_Delegate_1 s_System_Init_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT System_Close_Delegate_1(IntPtr system);
        private static System_Close_Delegate_1 s_System_Close_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT System_Update_Delegate_1(IntPtr system);
        private static System_Update_Delegate_1 s_System_Update_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT System_SetSpeakerPosition_Delegate_1(IntPtr system, SPEAKER speaker, float x, float y, bool active);
        private static System_SetSpeakerPosition_Delegate_1 s_System_SetSpeakerPosition_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT System_GetSpeakerPosition_Delegate_1(IntPtr system, SPEAKER speaker, out float x, out float y, out bool active);
        private static System_GetSpeakerPosition_Delegate_1 s_System_GetSpeakerPosition_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT System_SetStreamBufferSize_Delegate_1(IntPtr system, uint filebuffersize, TIMEUNIT filebuffersizetype);
        private static System_SetStreamBufferSize_Delegate_1 s_System_SetStreamBufferSize_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT System_GetStreamBufferSize_Delegate_1(IntPtr system, out uint filebuffersize, out TIMEUNIT filebuffersizetype);
        private static System_GetStreamBufferSize_Delegate_1 s_System_GetStreamBufferSize_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT System_Set3DSettings_Delegate_1(IntPtr system, float dopplerscale, float distancefactor, float rolloffscale);
        private static System_Set3DSettings_Delegate_1 s_System_Set3DSettings_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT System_Get3DSettings_Delegate_1(IntPtr system, out float dopplerscale, out float distancefactor, out float rolloffscale);
        private static System_Get3DSettings_Delegate_1 s_System_Get3DSettings_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT System_Set3DNumListeners_Delegate_1(IntPtr system, int numlisteners);
        private static System_Set3DNumListeners_Delegate_1 s_System_Set3DNumListeners_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT System_Get3DNumListeners_Delegate_1(IntPtr system, out int numlisteners);
        private static System_Get3DNumListeners_Delegate_1 s_System_Get3DNumListeners_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT System_Set3DListenerAttributes_Delegate_1(IntPtr system, int listener, ref VECTOR pos, ref VECTOR vel, ref VECTOR forward, ref VECTOR up);
        private static System_Set3DListenerAttributes_Delegate_1 s_System_Set3DListenerAttributes_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT System_Get3DListenerAttributes_Delegate_1(IntPtr system, int listener, out VECTOR pos, out VECTOR vel, out VECTOR forward, out VECTOR up);
        private static System_Get3DListenerAttributes_Delegate_1 s_System_Get3DListenerAttributes_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT System_Set3DRolloffCallback_Delegate_1(IntPtr system, CB_3D_ROLLOFF_CALLBACK callback);
        private static System_Set3DRolloffCallback_Delegate_1 s_System_Set3DRolloffCallback_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT System_MixerSuspend_Delegate_1(IntPtr system);
        private static System_MixerSuspend_Delegate_1 s_System_MixerSuspend_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT System_MixerResume_Delegate_1(IntPtr system);
        private static System_MixerResume_Delegate_1 s_System_MixerResume_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT System_GetDefaultMixMatrix_Delegate_1(IntPtr system, SPEAKERMODE sourcespeakermode, SPEAKERMODE targetspeakermode, float[] matrix, int matrixhop);
        private static System_GetDefaultMixMatrix_Delegate_1 s_System_GetDefaultMixMatrix_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT System_GetSpeakerModeChannels_Delegate_1(IntPtr system, SPEAKERMODE mode, out int channels);
        private static System_GetSpeakerModeChannels_Delegate_1 s_System_GetSpeakerModeChannels_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT System_GetVersion_Delegate_1(IntPtr system, out uint version, out uint buildnumber);
        private static System_GetVersion_Delegate_1 s_System_GetVersion_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT System_GetOutputHandle_Delegate_1(IntPtr system, out IntPtr handle);
        private static System_GetOutputHandle_Delegate_1 s_System_GetOutputHandle_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT System_GetChannelsPlaying_Delegate_1(IntPtr system, out int channels, IntPtr zero);
        private static System_GetChannelsPlaying_Delegate_1 s_System_GetChannelsPlaying_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT System_GetChannelsPlaying_Delegate_2(IntPtr system, out int channels, out int realchannels);
        private static System_GetChannelsPlaying_Delegate_2 s_System_GetChannelsPlaying_2;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT System_GetCPUUsage_Delegate_1(IntPtr system, out CPU_USAGE usage);
        private static System_GetCPUUsage_Delegate_1 s_System_GetCPUUsage_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT System_GetFileUsage_Delegate_1(IntPtr system, out Int64 sampleBytesRead, out Int64 streamBytesRead, out Int64 otherBytesRead);
        private static System_GetFileUsage_Delegate_1 s_System_GetFileUsage_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT System_CreateSound_Delegate_1(IntPtr system, byte[] name_or_data, MODE mode, ref CREATESOUNDEXINFO exinfo, out IntPtr sound);
        private static System_CreateSound_Delegate_1 s_System_CreateSound_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT System_CreateSound_Delegate_2(IntPtr system, IntPtr name_or_data, MODE mode, ref CREATESOUNDEXINFO exinfo, out IntPtr sound);
        private static System_CreateSound_Delegate_2 s_System_CreateSound_2;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT System_CreateStream_Delegate_1(IntPtr system, byte[] name_or_data, MODE mode, ref CREATESOUNDEXINFO exinfo, out IntPtr sound);
        private static System_CreateStream_Delegate_1 s_System_CreateStream_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT System_CreateStream_Delegate_2(IntPtr system, IntPtr name_or_data, MODE mode, ref CREATESOUNDEXINFO exinfo, out IntPtr sound);
        private static System_CreateStream_Delegate_2 s_System_CreateStream_2;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT System_CreateDSP_Delegate_1(IntPtr system, ref DSP_DESCRIPTION description, out IntPtr dsp);
        private static System_CreateDSP_Delegate_1 s_System_CreateDSP_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT System_CreateDSPByType_Delegate_1(IntPtr system, DSP_TYPE type, out IntPtr dsp);
        private static System_CreateDSPByType_Delegate_1 s_System_CreateDSPByType_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT System_CreateDSPConnection_Delegate_1(IntPtr system, DSPCONNECTION_TYPE type, out IntPtr connection);
        private static System_CreateDSPConnection_Delegate_1 s_System_CreateDSPConnection_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT System_CreateChannelGroup_Delegate_1(IntPtr system, byte[] name, out IntPtr channelgroup);
        private static System_CreateChannelGroup_Delegate_1 s_System_CreateChannelGroup_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT System_CreateSoundGroup_Delegate_1(IntPtr system, byte[] name, out IntPtr soundgroup);
        private static System_CreateSoundGroup_Delegate_1 s_System_CreateSoundGroup_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT System_CreateReverb3D_Delegate_1(IntPtr system, out IntPtr reverb);
        private static System_CreateReverb3D_Delegate_1 s_System_CreateReverb3D_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT System_PlaySound_Delegate_1(IntPtr system, IntPtr sound, IntPtr channelgroup, bool paused, out IntPtr channel);
        private static System_PlaySound_Delegate_1 s_System_PlaySound_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT System_PlayDSP_Delegate_1(IntPtr system, IntPtr dsp, IntPtr channelgroup, bool paused, out IntPtr channel);
        private static System_PlayDSP_Delegate_1 s_System_PlayDSP_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT System_GetChannel_Delegate_1(IntPtr system, int channelid, out IntPtr channel);
        private static System_GetChannel_Delegate_1 s_System_GetChannel_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT System_GetDSPInfoByType_Delegate_1(IntPtr system, DSP_TYPE type, out IntPtr description);
        private static System_GetDSPInfoByType_Delegate_1 s_System_GetDSPInfoByType_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT System_GetMasterChannelGroup_Delegate_1(IntPtr system, out IntPtr channelgroup);
        private static System_GetMasterChannelGroup_Delegate_1 s_System_GetMasterChannelGroup_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT System_GetMasterSoundGroup_Delegate_1(IntPtr system, out IntPtr soundgroup);
        private static System_GetMasterSoundGroup_Delegate_1 s_System_GetMasterSoundGroup_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT System_AttachChannelGroupToPort_Delegate_1(IntPtr system, PORT_TYPE portType, ulong portIndex, IntPtr channelgroup, bool passThru);
        private static System_AttachChannelGroupToPort_Delegate_1 s_System_AttachChannelGroupToPort_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT System_DetachChannelGroupFromPort_Delegate_1(IntPtr system, IntPtr channelgroup);
        private static System_DetachChannelGroupFromPort_Delegate_1 s_System_DetachChannelGroupFromPort_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT System_SetReverbProperties_Delegate_1(IntPtr system, int instance, ref REVERB_PROPERTIES prop);
        private static System_SetReverbProperties_Delegate_1 s_System_SetReverbProperties_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT System_GetReverbProperties_Delegate_1(IntPtr system, int instance, out REVERB_PROPERTIES prop);
        private static System_GetReverbProperties_Delegate_1 s_System_GetReverbProperties_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT System_LockDSP_Delegate_1(IntPtr system);
        private static System_LockDSP_Delegate_1 s_System_LockDSP_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT System_UnlockDSP_Delegate_1(IntPtr system);
        private static System_UnlockDSP_Delegate_1 s_System_UnlockDSP_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT System_GetRecordNumDrivers_Delegate_1(IntPtr system, out int numdrivers, out int numconnected);
        private static System_GetRecordNumDrivers_Delegate_1 s_System_GetRecordNumDrivers_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT System_GetRecordDriverInfo_Delegate_1(IntPtr system, int id, IntPtr name, int namelen, out Guid guid, out int systemrate, out SPEAKERMODE speakermode, out int speakermodechannels, out DRIVER_STATE state);
        private static System_GetRecordDriverInfo_Delegate_1 s_System_GetRecordDriverInfo_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT System_GetRecordPosition_Delegate_1(IntPtr system, int id, out uint position);
        private static System_GetRecordPosition_Delegate_1 s_System_GetRecordPosition_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT System_RecordStart_Delegate_1(IntPtr system, int id, IntPtr sound, bool loop);
        private static System_RecordStart_Delegate_1 s_System_RecordStart_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT System_RecordStop_Delegate_1(IntPtr system, int id);
        private static System_RecordStop_Delegate_1 s_System_RecordStop_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT System_IsRecording_Delegate_1(IntPtr system, int id, out bool recording);
        private static System_IsRecording_Delegate_1 s_System_IsRecording_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT System_CreateGeometry_Delegate_1(IntPtr system, int maxpolygons, int maxvertices, out IntPtr geometry);
        private static System_CreateGeometry_Delegate_1 s_System_CreateGeometry_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT System_SetGeometrySettings_Delegate_1(IntPtr system, float maxworldsize);
        private static System_SetGeometrySettings_Delegate_1 s_System_SetGeometrySettings_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT System_GetGeometrySettings_Delegate_1(IntPtr system, out float maxworldsize);
        private static System_GetGeometrySettings_Delegate_1 s_System_GetGeometrySettings_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT System_LoadGeometry_Delegate_1(IntPtr system, IntPtr data, int datasize, out IntPtr geometry);
        private static System_LoadGeometry_Delegate_1 s_System_LoadGeometry_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT System_GetGeometryOcclusion_Delegate_1(IntPtr system, ref VECTOR listener, ref VECTOR source, out float direct, out float reverb);
        private static System_GetGeometryOcclusion_Delegate_1 s_System_GetGeometryOcclusion_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT System_SetNetworkProxy_Delegate_1(IntPtr system, byte[] proxy);
        private static System_SetNetworkProxy_Delegate_1 s_System_SetNetworkProxy_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT System_GetNetworkProxy_Delegate_1(IntPtr system, IntPtr proxy, int proxylen);
        private static System_GetNetworkProxy_Delegate_1 s_System_GetNetworkProxy_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT System_SetNetworkTimeout_Delegate_1(IntPtr system, int timeout);
        private static System_SetNetworkTimeout_Delegate_1 s_System_SetNetworkTimeout_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT System_GetNetworkTimeout_Delegate_1(IntPtr system, out int timeout);
        private static System_GetNetworkTimeout_Delegate_1 s_System_GetNetworkTimeout_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT System_SetUserData_Delegate_1(IntPtr system, IntPtr userdata);
        private static System_SetUserData_Delegate_1 s_System_SetUserData_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT System_GetUserData_Delegate_1(IntPtr system, out IntPtr userdata);
        private static System_GetUserData_Delegate_1 s_System_GetUserData_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Sound_Release_Delegate_1(IntPtr sound);
        private static Sound_Release_Delegate_1 s_Sound_Release_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Sound_GetSystemObject_Delegate_1(IntPtr sound, out IntPtr system);
        private static Sound_GetSystemObject_Delegate_1 s_Sound_GetSystemObject_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Sound_Lock_Delegate_1(IntPtr sound, uint offset, uint length, out IntPtr ptr1, out IntPtr ptr2, out uint len1, out uint len2);
        private static Sound_Lock_Delegate_1 s_Sound_Lock_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Sound_Unlock_Delegate_1(IntPtr sound, IntPtr ptr1, IntPtr ptr2, uint len1, uint len2);
        private static Sound_Unlock_Delegate_1 s_Sound_Unlock_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Sound_SetDefaults_Delegate_1(IntPtr sound, float frequency, int priority);
        private static Sound_SetDefaults_Delegate_1 s_Sound_SetDefaults_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Sound_GetDefaults_Delegate_1(IntPtr sound, out float frequency, out int priority);
        private static Sound_GetDefaults_Delegate_1 s_Sound_GetDefaults_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Sound_Set3DMinMaxDistance_Delegate_1(IntPtr sound, float min, float max);
        private static Sound_Set3DMinMaxDistance_Delegate_1 s_Sound_Set3DMinMaxDistance_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Sound_Get3DMinMaxDistance_Delegate_1(IntPtr sound, out float min, out float max);
        private static Sound_Get3DMinMaxDistance_Delegate_1 s_Sound_Get3DMinMaxDistance_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Sound_Set3DConeSettings_Delegate_1(IntPtr sound, float insideconeangle, float outsideconeangle, float outsidevolume);
        private static Sound_Set3DConeSettings_Delegate_1 s_Sound_Set3DConeSettings_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Sound_Get3DConeSettings_Delegate_1(IntPtr sound, out float insideconeangle, out float outsideconeangle, out float outsidevolume);
        private static Sound_Get3DConeSettings_Delegate_1 s_Sound_Get3DConeSettings_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Sound_Set3DCustomRolloff_Delegate_1(IntPtr sound, ref VECTOR points, int numpoints);
        private static Sound_Set3DCustomRolloff_Delegate_1 s_Sound_Set3DCustomRolloff_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Sound_Get3DCustomRolloff_Delegate_1(IntPtr sound, out IntPtr points, out int numpoints);
        private static Sound_Get3DCustomRolloff_Delegate_1 s_Sound_Get3DCustomRolloff_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Sound_GetSubSound_Delegate_1(IntPtr sound, int index, out IntPtr subsound);
        private static Sound_GetSubSound_Delegate_1 s_Sound_GetSubSound_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Sound_GetSubSoundParent_Delegate_1(IntPtr sound, out IntPtr parentsound);
        private static Sound_GetSubSoundParent_Delegate_1 s_Sound_GetSubSoundParent_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Sound_GetName_Delegate_1(IntPtr sound, IntPtr name, int namelen);
        private static Sound_GetName_Delegate_1 s_Sound_GetName_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Sound_GetLength_Delegate_1(IntPtr sound, out uint length, TIMEUNIT lengthtype);
        private static Sound_GetLength_Delegate_1 s_Sound_GetLength_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Sound_GetFormat_Delegate_1(IntPtr sound, out SOUND_TYPE type, out SOUND_FORMAT format, out int channels, out int bits);
        private static Sound_GetFormat_Delegate_1 s_Sound_GetFormat_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Sound_GetNumSubSounds_Delegate_1(IntPtr sound, out int numsubsounds);
        private static Sound_GetNumSubSounds_Delegate_1 s_Sound_GetNumSubSounds_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Sound_GetNumTags_Delegate_1(IntPtr sound, out int numtags, out int numtagsupdated);
        private static Sound_GetNumTags_Delegate_1 s_Sound_GetNumTags_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Sound_GetTag_Delegate_1(IntPtr sound, byte[] name, int index, out TAG tag);
        private static Sound_GetTag_Delegate_1 s_Sound_GetTag_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Sound_GetOpenState_Delegate_1(IntPtr sound, out OPENSTATE openstate, out uint percentbuffered, out bool starving, out bool diskbusy);
        private static Sound_GetOpenState_Delegate_1 s_Sound_GetOpenState_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Sound_ReadData_Delegate_1(IntPtr sound, byte[] buffer, uint length, IntPtr zero);
        private static Sound_ReadData_Delegate_1 s_Sound_ReadData_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Sound_ReadData_Delegate_2(IntPtr sound, byte[] buffer, uint length, out uint read);
        private static Sound_ReadData_Delegate_2 s_Sound_ReadData_2;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Sound_SeekData_Delegate_1(IntPtr sound, uint pcm);
        private static Sound_SeekData_Delegate_1 s_Sound_SeekData_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Sound_SetSoundGroup_Delegate_1(IntPtr sound, IntPtr soundgroup);
        private static Sound_SetSoundGroup_Delegate_1 s_Sound_SetSoundGroup_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Sound_GetSoundGroup_Delegate_1(IntPtr sound, out IntPtr soundgroup);
        private static Sound_GetSoundGroup_Delegate_1 s_Sound_GetSoundGroup_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Sound_GetNumSyncPoints_Delegate_1(IntPtr sound, out int numsyncpoints);
        private static Sound_GetNumSyncPoints_Delegate_1 s_Sound_GetNumSyncPoints_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Sound_GetSyncPoint_Delegate_1(IntPtr sound, int index, out IntPtr point);
        private static Sound_GetSyncPoint_Delegate_1 s_Sound_GetSyncPoint_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Sound_GetSyncPointInfo_Delegate_1(IntPtr sound, IntPtr point, IntPtr name, int namelen, out uint offset, TIMEUNIT offsettype);
        private static Sound_GetSyncPointInfo_Delegate_1 s_Sound_GetSyncPointInfo_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Sound_AddSyncPoint_Delegate_1(IntPtr sound, uint offset, TIMEUNIT offsettype, byte[] name, out IntPtr point);
        private static Sound_AddSyncPoint_Delegate_1 s_Sound_AddSyncPoint_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Sound_DeleteSyncPoint_Delegate_1(IntPtr sound, IntPtr point);
        private static Sound_DeleteSyncPoint_Delegate_1 s_Sound_DeleteSyncPoint_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Sound_SetMode_Delegate_1(IntPtr sound, MODE mode);
        private static Sound_SetMode_Delegate_1 s_Sound_SetMode_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Sound_GetMode_Delegate_1(IntPtr sound, out MODE mode);
        private static Sound_GetMode_Delegate_1 s_Sound_GetMode_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Sound_SetLoopCount_Delegate_1(IntPtr sound, int loopcount);
        private static Sound_SetLoopCount_Delegate_1 s_Sound_SetLoopCount_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Sound_GetLoopCount_Delegate_1(IntPtr sound, out int loopcount);
        private static Sound_GetLoopCount_Delegate_1 s_Sound_GetLoopCount_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Sound_SetLoopPoints_Delegate_1(IntPtr sound, uint loopstart, TIMEUNIT loopstarttype, uint loopend, TIMEUNIT loopendtype);
        private static Sound_SetLoopPoints_Delegate_1 s_Sound_SetLoopPoints_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Sound_GetLoopPoints_Delegate_1(IntPtr sound, out uint loopstart, TIMEUNIT loopstarttype, out uint loopend, TIMEUNIT loopendtype);
        private static Sound_GetLoopPoints_Delegate_1 s_Sound_GetLoopPoints_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Sound_GetMusicNumChannels_Delegate_1(IntPtr sound, out int numchannels);
        private static Sound_GetMusicNumChannels_Delegate_1 s_Sound_GetMusicNumChannels_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Sound_SetMusicChannelVolume_Delegate_1(IntPtr sound, int channel, float volume);
        private static Sound_SetMusicChannelVolume_Delegate_1 s_Sound_SetMusicChannelVolume_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Sound_GetMusicChannelVolume_Delegate_1(IntPtr sound, int channel, out float volume);
        private static Sound_GetMusicChannelVolume_Delegate_1 s_Sound_GetMusicChannelVolume_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Sound_SetMusicSpeed_Delegate_1(IntPtr sound, float speed);
        private static Sound_SetMusicSpeed_Delegate_1 s_Sound_SetMusicSpeed_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Sound_GetMusicSpeed_Delegate_1(IntPtr sound, out float speed);
        private static Sound_GetMusicSpeed_Delegate_1 s_Sound_GetMusicSpeed_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Sound_SetUserData_Delegate_1(IntPtr sound, IntPtr userdata);
        private static Sound_SetUserData_Delegate_1 s_Sound_SetUserData_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Sound_GetUserData_Delegate_1(IntPtr sound, out IntPtr userdata);
        private static Sound_GetUserData_Delegate_1 s_Sound_GetUserData_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Channel_SetFrequency_Delegate_1(IntPtr channel, float frequency);
        private static Channel_SetFrequency_Delegate_1 s_Channel_SetFrequency_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Channel_GetFrequency_Delegate_1(IntPtr channel, out float frequency);
        private static Channel_GetFrequency_Delegate_1 s_Channel_GetFrequency_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Channel_SetPriority_Delegate_1(IntPtr channel, int priority);
        private static Channel_SetPriority_Delegate_1 s_Channel_SetPriority_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Channel_GetPriority_Delegate_1(IntPtr channel, out int priority);
        private static Channel_GetPriority_Delegate_1 s_Channel_GetPriority_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Channel_SetPosition_Delegate_1(IntPtr channel, uint position, TIMEUNIT postype);
        private static Channel_SetPosition_Delegate_1 s_Channel_SetPosition_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Channel_GetPosition_Delegate_1(IntPtr channel, out uint position, TIMEUNIT postype);
        private static Channel_GetPosition_Delegate_1 s_Channel_GetPosition_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Channel_SetChannelGroup_Delegate_1(IntPtr channel, IntPtr channelgroup);
        private static Channel_SetChannelGroup_Delegate_1 s_Channel_SetChannelGroup_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Channel_GetChannelGroup_Delegate_1(IntPtr channel, out IntPtr channelgroup);
        private static Channel_GetChannelGroup_Delegate_1 s_Channel_GetChannelGroup_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Channel_SetLoopCount_Delegate_1(IntPtr channel, int loopcount);
        private static Channel_SetLoopCount_Delegate_1 s_Channel_SetLoopCount_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Channel_GetLoopCount_Delegate_1(IntPtr channel, out int loopcount);
        private static Channel_GetLoopCount_Delegate_1 s_Channel_GetLoopCount_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Channel_SetLoopPoints_Delegate_1(IntPtr channel, uint loopstart, TIMEUNIT loopstarttype, uint loopend, TIMEUNIT loopendtype);
        private static Channel_SetLoopPoints_Delegate_1 s_Channel_SetLoopPoints_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Channel_GetLoopPoints_Delegate_1(IntPtr channel, out uint loopstart, TIMEUNIT loopstarttype, out uint loopend, TIMEUNIT loopendtype);
        private static Channel_GetLoopPoints_Delegate_1 s_Channel_GetLoopPoints_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Channel_IsVirtual_Delegate_1(IntPtr channel, out bool isvirtual);
        private static Channel_IsVirtual_Delegate_1 s_Channel_IsVirtual_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Channel_GetCurrentSound_Delegate_1(IntPtr channel, out IntPtr sound);
        private static Channel_GetCurrentSound_Delegate_1 s_Channel_GetCurrentSound_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Channel_GetIndex_Delegate_1(IntPtr channel, out int index);
        private static Channel_GetIndex_Delegate_1 s_Channel_GetIndex_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Channel_GetSystemObject_Delegate_1(IntPtr channel, out IntPtr system);
        private static Channel_GetSystemObject_Delegate_1 s_Channel_GetSystemObject_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Channel_Stop_Delegate_1(IntPtr channel);
        private static Channel_Stop_Delegate_1 s_Channel_Stop_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Channel_SetPaused_Delegate_1(IntPtr channel, bool paused);
        private static Channel_SetPaused_Delegate_1 s_Channel_SetPaused_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Channel_GetPaused_Delegate_1(IntPtr channel, out bool paused);
        private static Channel_GetPaused_Delegate_1 s_Channel_GetPaused_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Channel_SetVolume_Delegate_1(IntPtr channel, float volume);
        private static Channel_SetVolume_Delegate_1 s_Channel_SetVolume_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Channel_GetVolume_Delegate_1(IntPtr channel, out float volume);
        private static Channel_GetVolume_Delegate_1 s_Channel_GetVolume_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Channel_SetVolumeRamp_Delegate_1(IntPtr channel, bool ramp);
        private static Channel_SetVolumeRamp_Delegate_1 s_Channel_SetVolumeRamp_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Channel_GetVolumeRamp_Delegate_1(IntPtr channel, out bool ramp);
        private static Channel_GetVolumeRamp_Delegate_1 s_Channel_GetVolumeRamp_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Channel_GetAudibility_Delegate_1(IntPtr channel, out float audibility);
        private static Channel_GetAudibility_Delegate_1 s_Channel_GetAudibility_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Channel_SetPitch_Delegate_1(IntPtr channel, float pitch);
        private static Channel_SetPitch_Delegate_1 s_Channel_SetPitch_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Channel_GetPitch_Delegate_1(IntPtr channel, out float pitch);
        private static Channel_GetPitch_Delegate_1 s_Channel_GetPitch_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Channel_SetMute_Delegate_1(IntPtr channel, bool mute);
        private static Channel_SetMute_Delegate_1 s_Channel_SetMute_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Channel_GetMute_Delegate_1(IntPtr channel, out bool mute);
        private static Channel_GetMute_Delegate_1 s_Channel_GetMute_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Channel_SetReverbProperties_Delegate_1(IntPtr channel, int instance, float wet);
        private static Channel_SetReverbProperties_Delegate_1 s_Channel_SetReverbProperties_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Channel_GetReverbProperties_Delegate_1(IntPtr channel, int instance, out float wet);
        private static Channel_GetReverbProperties_Delegate_1 s_Channel_GetReverbProperties_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Channel_SetLowPassGain_Delegate_1(IntPtr channel, float gain);
        private static Channel_SetLowPassGain_Delegate_1 s_Channel_SetLowPassGain_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Channel_GetLowPassGain_Delegate_1(IntPtr channel, out float gain);
        private static Channel_GetLowPassGain_Delegate_1 s_Channel_GetLowPassGain_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Channel_SetMode_Delegate_1(IntPtr channel, MODE mode);
        private static Channel_SetMode_Delegate_1 s_Channel_SetMode_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Channel_GetMode_Delegate_1(IntPtr channel, out MODE mode);
        private static Channel_GetMode_Delegate_1 s_Channel_GetMode_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Channel_SetCallback_Delegate_1(IntPtr channel, CHANNELCONTROL_CALLBACK callback);
        private static Channel_SetCallback_Delegate_1 s_Channel_SetCallback_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Channel_IsPlaying_Delegate_1(IntPtr channel, out bool isplaying);
        private static Channel_IsPlaying_Delegate_1 s_Channel_IsPlaying_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Channel_SetPan_Delegate_1(IntPtr channel, float pan);
        private static Channel_SetPan_Delegate_1 s_Channel_SetPan_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Channel_SetMixLevelsOutput_Delegate_1(IntPtr channel, float frontleft, float frontright, float center, float lfe, float surroundleft, float surroundright, float backleft, float backright);
        private static Channel_SetMixLevelsOutput_Delegate_1 s_Channel_SetMixLevelsOutput_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Channel_SetMixLevelsInput_Delegate_1(IntPtr channel, float[] levels, int numlevels);
        private static Channel_SetMixLevelsInput_Delegate_1 s_Channel_SetMixLevelsInput_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Channel_SetMixMatrix_Delegate_1(IntPtr channel, float[] matrix, int outchannels, int inchannels, int inchannel_hop);
        private static Channel_SetMixMatrix_Delegate_1 s_Channel_SetMixMatrix_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Channel_GetMixMatrix_Delegate_1(IntPtr channel, float[] matrix, out int outchannels, out int inchannels, int inchannel_hop);
        private static Channel_GetMixMatrix_Delegate_1 s_Channel_GetMixMatrix_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Channel_GetDSPClock_Delegate_1(IntPtr channel, out ulong dspclock, out ulong parentclock);
        private static Channel_GetDSPClock_Delegate_1 s_Channel_GetDSPClock_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Channel_SetDelay_Delegate_1(IntPtr channel, ulong dspclock_start, ulong dspclock_end, bool stopchannels);
        private static Channel_SetDelay_Delegate_1 s_Channel_SetDelay_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Channel_GetDelay_Delegate_1(IntPtr channel, out ulong dspclock_start, out ulong dspclock_end, IntPtr zero);
        private static Channel_GetDelay_Delegate_1 s_Channel_GetDelay_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Channel_GetDelay_Delegate_2(IntPtr channel, out ulong dspclock_start, out ulong dspclock_end, out bool stopchannels);
        private static Channel_GetDelay_Delegate_2 s_Channel_GetDelay_2;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Channel_AddFadePoint_Delegate_1(IntPtr channel, ulong dspclock, float volume);
        private static Channel_AddFadePoint_Delegate_1 s_Channel_AddFadePoint_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Channel_SetFadePointRamp_Delegate_1(IntPtr channel, ulong dspclock, float volume);
        private static Channel_SetFadePointRamp_Delegate_1 s_Channel_SetFadePointRamp_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Channel_RemoveFadePoints_Delegate_1(IntPtr channel, ulong dspclock_start, ulong dspclock_end);
        private static Channel_RemoveFadePoints_Delegate_1 s_Channel_RemoveFadePoints_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Channel_GetFadePoints_Delegate_1(IntPtr channel, ref uint numpoints, ulong[] point_dspclock, float[] point_volume);
        private static Channel_GetFadePoints_Delegate_1 s_Channel_GetFadePoints_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Channel_GetDSP_Delegate_1(IntPtr channel, int index, out IntPtr dsp);
        private static Channel_GetDSP_Delegate_1 s_Channel_GetDSP_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Channel_AddDSP_Delegate_1(IntPtr channel, int index, IntPtr dsp);
        private static Channel_AddDSP_Delegate_1 s_Channel_AddDSP_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Channel_RemoveDSP_Delegate_1(IntPtr channel, IntPtr dsp);
        private static Channel_RemoveDSP_Delegate_1 s_Channel_RemoveDSP_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Channel_GetNumDSPs_Delegate_1(IntPtr channel, out int numdsps);
        private static Channel_GetNumDSPs_Delegate_1 s_Channel_GetNumDSPs_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Channel_SetDSPIndex_Delegate_1(IntPtr channel, IntPtr dsp, int index);
        private static Channel_SetDSPIndex_Delegate_1 s_Channel_SetDSPIndex_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Channel_GetDSPIndex_Delegate_1(IntPtr channel, IntPtr dsp, out int index);
        private static Channel_GetDSPIndex_Delegate_1 s_Channel_GetDSPIndex_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Channel_Set3DAttributes_Delegate_1(IntPtr channel, ref VECTOR pos, ref VECTOR vel);
        private static Channel_Set3DAttributes_Delegate_1 s_Channel_Set3DAttributes_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Channel_Get3DAttributes_Delegate_1(IntPtr channel, out VECTOR pos, out VECTOR vel);
        private static Channel_Get3DAttributes_Delegate_1 s_Channel_Get3DAttributes_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Channel_Set3DMinMaxDistance_Delegate_1(IntPtr channel, float mindistance, float maxdistance);
        private static Channel_Set3DMinMaxDistance_Delegate_1 s_Channel_Set3DMinMaxDistance_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Channel_Get3DMinMaxDistance_Delegate_1(IntPtr channel, out float mindistance, out float maxdistance);
        private static Channel_Get3DMinMaxDistance_Delegate_1 s_Channel_Get3DMinMaxDistance_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Channel_Set3DConeSettings_Delegate_1(IntPtr channel, float insideconeangle, float outsideconeangle, float outsidevolume);
        private static Channel_Set3DConeSettings_Delegate_1 s_Channel_Set3DConeSettings_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Channel_Get3DConeSettings_Delegate_1(IntPtr channel, out float insideconeangle, out float outsideconeangle, out float outsidevolume);
        private static Channel_Get3DConeSettings_Delegate_1 s_Channel_Get3DConeSettings_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Channel_Set3DConeOrientation_Delegate_1(IntPtr channel, ref VECTOR orientation);
        private static Channel_Set3DConeOrientation_Delegate_1 s_Channel_Set3DConeOrientation_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Channel_Get3DConeOrientation_Delegate_1(IntPtr channel, out VECTOR orientation);
        private static Channel_Get3DConeOrientation_Delegate_1 s_Channel_Get3DConeOrientation_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Channel_Set3DCustomRolloff_Delegate_1(IntPtr channel, ref VECTOR points, int numpoints);
        private static Channel_Set3DCustomRolloff_Delegate_1 s_Channel_Set3DCustomRolloff_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Channel_Get3DCustomRolloff_Delegate_1(IntPtr channel, out IntPtr points, out int numpoints);
        private static Channel_Get3DCustomRolloff_Delegate_1 s_Channel_Get3DCustomRolloff_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Channel_Set3DOcclusion_Delegate_1(IntPtr channel, float directocclusion, float reverbocclusion);
        private static Channel_Set3DOcclusion_Delegate_1 s_Channel_Set3DOcclusion_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Channel_Get3DOcclusion_Delegate_1(IntPtr channel, out float directocclusion, out float reverbocclusion);
        private static Channel_Get3DOcclusion_Delegate_1 s_Channel_Get3DOcclusion_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Channel_Set3DSpread_Delegate_1(IntPtr channel, float angle);
        private static Channel_Set3DSpread_Delegate_1 s_Channel_Set3DSpread_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Channel_Get3DSpread_Delegate_1(IntPtr channel, out float angle);
        private static Channel_Get3DSpread_Delegate_1 s_Channel_Get3DSpread_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Channel_Set3DLevel_Delegate_1(IntPtr channel, float level);
        private static Channel_Set3DLevel_Delegate_1 s_Channel_Set3DLevel_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Channel_Get3DLevel_Delegate_1(IntPtr channel, out float level);
        private static Channel_Get3DLevel_Delegate_1 s_Channel_Get3DLevel_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Channel_Set3DDopplerLevel_Delegate_1(IntPtr channel, float level);
        private static Channel_Set3DDopplerLevel_Delegate_1 s_Channel_Set3DDopplerLevel_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Channel_Get3DDopplerLevel_Delegate_1(IntPtr channel, out float level);
        private static Channel_Get3DDopplerLevel_Delegate_1 s_Channel_Get3DDopplerLevel_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Channel_Set3DDistanceFilter_Delegate_1(IntPtr channel, bool custom, float customLevel, float centerFreq);
        private static Channel_Set3DDistanceFilter_Delegate_1 s_Channel_Set3DDistanceFilter_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Channel_Get3DDistanceFilter_Delegate_1(IntPtr channel, out bool custom, out float customLevel, out float centerFreq);
        private static Channel_Get3DDistanceFilter_Delegate_1 s_Channel_Get3DDistanceFilter_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Channel_SetUserData_Delegate_1(IntPtr channel, IntPtr userdata);
        private static Channel_SetUserData_Delegate_1 s_Channel_SetUserData_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Channel_GetUserData_Delegate_1(IntPtr channel, out IntPtr userdata);
        private static Channel_GetUserData_Delegate_1 s_Channel_GetUserData_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT ChannelGroup_Release_Delegate_1(IntPtr channelgroup);
        private static ChannelGroup_Release_Delegate_1 s_ChannelGroup_Release_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT ChannelGroup_AddGroup_Delegate_1(IntPtr channelgroup, IntPtr group, bool propagatedspclock, IntPtr zero);
        private static ChannelGroup_AddGroup_Delegate_1 s_ChannelGroup_AddGroup_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT ChannelGroup_AddGroup_Delegate_2(IntPtr channelgroup, IntPtr group, bool propagatedspclock, out IntPtr connection);
        private static ChannelGroup_AddGroup_Delegate_2 s_ChannelGroup_AddGroup_2;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT ChannelGroup_GetNumGroups_Delegate_1(IntPtr channelgroup, out int numgroups);
        private static ChannelGroup_GetNumGroups_Delegate_1 s_ChannelGroup_GetNumGroups_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT ChannelGroup_GetGroup_Delegate_1(IntPtr channelgroup, int index, out IntPtr group);
        private static ChannelGroup_GetGroup_Delegate_1 s_ChannelGroup_GetGroup_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT ChannelGroup_GetParentGroup_Delegate_1(IntPtr channelgroup, out IntPtr group);
        private static ChannelGroup_GetParentGroup_Delegate_1 s_ChannelGroup_GetParentGroup_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT ChannelGroup_GetName_Delegate_1(IntPtr channelgroup, IntPtr name, int namelen);
        private static ChannelGroup_GetName_Delegate_1 s_ChannelGroup_GetName_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT ChannelGroup_GetNumChannels_Delegate_1(IntPtr channelgroup, out int numchannels);
        private static ChannelGroup_GetNumChannels_Delegate_1 s_ChannelGroup_GetNumChannels_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT ChannelGroup_GetChannel_Delegate_1(IntPtr channelgroup, int index, out IntPtr channel);
        private static ChannelGroup_GetChannel_Delegate_1 s_ChannelGroup_GetChannel_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT ChannelGroup_GetSystemObject_Delegate_1(IntPtr channelgroup, out IntPtr system);
        private static ChannelGroup_GetSystemObject_Delegate_1 s_ChannelGroup_GetSystemObject_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT ChannelGroup_Stop_Delegate_1(IntPtr channelgroup);
        private static ChannelGroup_Stop_Delegate_1 s_ChannelGroup_Stop_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT ChannelGroup_SetPaused_Delegate_1(IntPtr channelgroup, bool paused);
        private static ChannelGroup_SetPaused_Delegate_1 s_ChannelGroup_SetPaused_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT ChannelGroup_GetPaused_Delegate_1(IntPtr channelgroup, out bool paused);
        private static ChannelGroup_GetPaused_Delegate_1 s_ChannelGroup_GetPaused_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT ChannelGroup_SetVolume_Delegate_1(IntPtr channelgroup, float volume);
        private static ChannelGroup_SetVolume_Delegate_1 s_ChannelGroup_SetVolume_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT ChannelGroup_GetVolume_Delegate_1(IntPtr channelgroup, out float volume);
        private static ChannelGroup_GetVolume_Delegate_1 s_ChannelGroup_GetVolume_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT ChannelGroup_SetVolumeRamp_Delegate_1(IntPtr channelgroup, bool ramp);
        private static ChannelGroup_SetVolumeRamp_Delegate_1 s_ChannelGroup_SetVolumeRamp_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT ChannelGroup_GetVolumeRamp_Delegate_1(IntPtr channelgroup, out bool ramp);
        private static ChannelGroup_GetVolumeRamp_Delegate_1 s_ChannelGroup_GetVolumeRamp_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT ChannelGroup_GetAudibility_Delegate_1(IntPtr channelgroup, out float audibility);
        private static ChannelGroup_GetAudibility_Delegate_1 s_ChannelGroup_GetAudibility_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT ChannelGroup_SetPitch_Delegate_1(IntPtr channelgroup, float pitch);
        private static ChannelGroup_SetPitch_Delegate_1 s_ChannelGroup_SetPitch_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT ChannelGroup_GetPitch_Delegate_1(IntPtr channelgroup, out float pitch);
        private static ChannelGroup_GetPitch_Delegate_1 s_ChannelGroup_GetPitch_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT ChannelGroup_SetMute_Delegate_1(IntPtr channelgroup, bool mute);
        private static ChannelGroup_SetMute_Delegate_1 s_ChannelGroup_SetMute_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT ChannelGroup_GetMute_Delegate_1(IntPtr channelgroup, out bool mute);
        private static ChannelGroup_GetMute_Delegate_1 s_ChannelGroup_GetMute_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT ChannelGroup_SetReverbProperties_Delegate_1(IntPtr channelgroup, int instance, float wet);
        private static ChannelGroup_SetReverbProperties_Delegate_1 s_ChannelGroup_SetReverbProperties_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT ChannelGroup_GetReverbProperties_Delegate_1(IntPtr channelgroup, int instance, out float wet);
        private static ChannelGroup_GetReverbProperties_Delegate_1 s_ChannelGroup_GetReverbProperties_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT ChannelGroup_SetLowPassGain_Delegate_1(IntPtr channelgroup, float gain);
        private static ChannelGroup_SetLowPassGain_Delegate_1 s_ChannelGroup_SetLowPassGain_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT ChannelGroup_GetLowPassGain_Delegate_1(IntPtr channelgroup, out float gain);
        private static ChannelGroup_GetLowPassGain_Delegate_1 s_ChannelGroup_GetLowPassGain_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT ChannelGroup_SetMode_Delegate_1(IntPtr channelgroup, MODE mode);
        private static ChannelGroup_SetMode_Delegate_1 s_ChannelGroup_SetMode_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT ChannelGroup_GetMode_Delegate_1(IntPtr channelgroup, out MODE mode);
        private static ChannelGroup_GetMode_Delegate_1 s_ChannelGroup_GetMode_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT ChannelGroup_SetCallback_Delegate_1(IntPtr channelgroup, CHANNELCONTROL_CALLBACK callback);
        private static ChannelGroup_SetCallback_Delegate_1 s_ChannelGroup_SetCallback_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT ChannelGroup_IsPlaying_Delegate_1(IntPtr channelgroup, out bool isplaying);
        private static ChannelGroup_IsPlaying_Delegate_1 s_ChannelGroup_IsPlaying_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT ChannelGroup_SetPan_Delegate_1(IntPtr channelgroup, float pan);
        private static ChannelGroup_SetPan_Delegate_1 s_ChannelGroup_SetPan_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT ChannelGroup_SetMixLevelsOutput_Delegate_1(IntPtr channelgroup, float frontleft, float frontright, float center, float lfe, float surroundleft, float surroundright, float backleft, float backright);
        private static ChannelGroup_SetMixLevelsOutput_Delegate_1 s_ChannelGroup_SetMixLevelsOutput_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT ChannelGroup_SetMixLevelsInput_Delegate_1(IntPtr channelgroup, float[] levels, int numlevels);
        private static ChannelGroup_SetMixLevelsInput_Delegate_1 s_ChannelGroup_SetMixLevelsInput_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT ChannelGroup_SetMixMatrix_Delegate_1(IntPtr channelgroup, float[] matrix, int outchannels, int inchannels, int inchannel_hop);
        private static ChannelGroup_SetMixMatrix_Delegate_1 s_ChannelGroup_SetMixMatrix_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT ChannelGroup_GetMixMatrix_Delegate_1(IntPtr channelgroup, float[] matrix, out int outchannels, out int inchannels, int inchannel_hop);
        private static ChannelGroup_GetMixMatrix_Delegate_1 s_ChannelGroup_GetMixMatrix_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT ChannelGroup_GetDSPClock_Delegate_1(IntPtr channelgroup, out ulong dspclock, out ulong parentclock);
        private static ChannelGroup_GetDSPClock_Delegate_1 s_ChannelGroup_GetDSPClock_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT ChannelGroup_SetDelay_Delegate_1(IntPtr channelgroup, ulong dspclock_start, ulong dspclock_end, bool stopchannels);
        private static ChannelGroup_SetDelay_Delegate_1 s_ChannelGroup_SetDelay_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT ChannelGroup_GetDelay_Delegate_1(IntPtr channelgroup, out ulong dspclock_start, out ulong dspclock_end, IntPtr zero);
        private static ChannelGroup_GetDelay_Delegate_1 s_ChannelGroup_GetDelay_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT ChannelGroup_GetDelay_Delegate_2(IntPtr channelgroup, out ulong dspclock_start, out ulong dspclock_end, out bool stopchannels);
        private static ChannelGroup_GetDelay_Delegate_2 s_ChannelGroup_GetDelay_2;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT ChannelGroup_AddFadePoint_Delegate_1(IntPtr channelgroup, ulong dspclock, float volume);
        private static ChannelGroup_AddFadePoint_Delegate_1 s_ChannelGroup_AddFadePoint_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT ChannelGroup_SetFadePointRamp_Delegate_1(IntPtr channelgroup, ulong dspclock, float volume);
        private static ChannelGroup_SetFadePointRamp_Delegate_1 s_ChannelGroup_SetFadePointRamp_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT ChannelGroup_RemoveFadePoints_Delegate_1(IntPtr channelgroup, ulong dspclock_start, ulong dspclock_end);
        private static ChannelGroup_RemoveFadePoints_Delegate_1 s_ChannelGroup_RemoveFadePoints_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT ChannelGroup_GetFadePoints_Delegate_1(IntPtr channelgroup, ref uint numpoints, ulong[] point_dspclock, float[] point_volume);
        private static ChannelGroup_GetFadePoints_Delegate_1 s_ChannelGroup_GetFadePoints_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT ChannelGroup_GetDSP_Delegate_1(IntPtr channelgroup, int index, out IntPtr dsp);
        private static ChannelGroup_GetDSP_Delegate_1 s_ChannelGroup_GetDSP_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT ChannelGroup_AddDSP_Delegate_1(IntPtr channelgroup, int index, IntPtr dsp);
        private static ChannelGroup_AddDSP_Delegate_1 s_ChannelGroup_AddDSP_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT ChannelGroup_RemoveDSP_Delegate_1(IntPtr channelgroup, IntPtr dsp);
        private static ChannelGroup_RemoveDSP_Delegate_1 s_ChannelGroup_RemoveDSP_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT ChannelGroup_GetNumDSPs_Delegate_1(IntPtr channelgroup, out int numdsps);
        private static ChannelGroup_GetNumDSPs_Delegate_1 s_ChannelGroup_GetNumDSPs_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT ChannelGroup_SetDSPIndex_Delegate_1(IntPtr channelgroup, IntPtr dsp, int index);
        private static ChannelGroup_SetDSPIndex_Delegate_1 s_ChannelGroup_SetDSPIndex_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT ChannelGroup_GetDSPIndex_Delegate_1(IntPtr channelgroup, IntPtr dsp, out int index);
        private static ChannelGroup_GetDSPIndex_Delegate_1 s_ChannelGroup_GetDSPIndex_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT ChannelGroup_Set3DAttributes_Delegate_1(IntPtr channelgroup, ref VECTOR pos, ref VECTOR vel);
        private static ChannelGroup_Set3DAttributes_Delegate_1 s_ChannelGroup_Set3DAttributes_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT ChannelGroup_Get3DAttributes_Delegate_1(IntPtr channelgroup, out VECTOR pos, out VECTOR vel);
        private static ChannelGroup_Get3DAttributes_Delegate_1 s_ChannelGroup_Get3DAttributes_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT ChannelGroup_Set3DMinMaxDistance_Delegate_1(IntPtr channelgroup, float mindistance, float maxdistance);
        private static ChannelGroup_Set3DMinMaxDistance_Delegate_1 s_ChannelGroup_Set3DMinMaxDistance_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT ChannelGroup_Get3DMinMaxDistance_Delegate_1(IntPtr channelgroup, out float mindistance, out float maxdistance);
        private static ChannelGroup_Get3DMinMaxDistance_Delegate_1 s_ChannelGroup_Get3DMinMaxDistance_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT ChannelGroup_Set3DConeSettings_Delegate_1(IntPtr channelgroup, float insideconeangle, float outsideconeangle, float outsidevolume);
        private static ChannelGroup_Set3DConeSettings_Delegate_1 s_ChannelGroup_Set3DConeSettings_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT ChannelGroup_Get3DConeSettings_Delegate_1(IntPtr channelgroup, out float insideconeangle, out float outsideconeangle, out float outsidevolume);
        private static ChannelGroup_Get3DConeSettings_Delegate_1 s_ChannelGroup_Get3DConeSettings_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT ChannelGroup_Set3DConeOrientation_Delegate_1(IntPtr channelgroup, ref VECTOR orientation);
        private static ChannelGroup_Set3DConeOrientation_Delegate_1 s_ChannelGroup_Set3DConeOrientation_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT ChannelGroup_Get3DConeOrientation_Delegate_1(IntPtr channelgroup, out VECTOR orientation);
        private static ChannelGroup_Get3DConeOrientation_Delegate_1 s_ChannelGroup_Get3DConeOrientation_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT ChannelGroup_Set3DCustomRolloff_Delegate_1(IntPtr channelgroup, ref VECTOR points, int numpoints);
        private static ChannelGroup_Set3DCustomRolloff_Delegate_1 s_ChannelGroup_Set3DCustomRolloff_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT ChannelGroup_Get3DCustomRolloff_Delegate_1(IntPtr channelgroup, out IntPtr points, out int numpoints);
        private static ChannelGroup_Get3DCustomRolloff_Delegate_1 s_ChannelGroup_Get3DCustomRolloff_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT ChannelGroup_Set3DOcclusion_Delegate_1(IntPtr channelgroup, float directocclusion, float reverbocclusion);
        private static ChannelGroup_Set3DOcclusion_Delegate_1 s_ChannelGroup_Set3DOcclusion_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT ChannelGroup_Get3DOcclusion_Delegate_1(IntPtr channelgroup, out float directocclusion, out float reverbocclusion);
        private static ChannelGroup_Get3DOcclusion_Delegate_1 s_ChannelGroup_Get3DOcclusion_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT ChannelGroup_Set3DSpread_Delegate_1(IntPtr channelgroup, float angle);
        private static ChannelGroup_Set3DSpread_Delegate_1 s_ChannelGroup_Set3DSpread_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT ChannelGroup_Get3DSpread_Delegate_1(IntPtr channelgroup, out float angle);
        private static ChannelGroup_Get3DSpread_Delegate_1 s_ChannelGroup_Get3DSpread_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT ChannelGroup_Set3DLevel_Delegate_1(IntPtr channelgroup, float level);
        private static ChannelGroup_Set3DLevel_Delegate_1 s_ChannelGroup_Set3DLevel_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT ChannelGroup_Get3DLevel_Delegate_1(IntPtr channelgroup, out float level);
        private static ChannelGroup_Get3DLevel_Delegate_1 s_ChannelGroup_Get3DLevel_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT ChannelGroup_Set3DDopplerLevel_Delegate_1(IntPtr channelgroup, float level);
        private static ChannelGroup_Set3DDopplerLevel_Delegate_1 s_ChannelGroup_Set3DDopplerLevel_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT ChannelGroup_Get3DDopplerLevel_Delegate_1(IntPtr channelgroup, out float level);
        private static ChannelGroup_Get3DDopplerLevel_Delegate_1 s_ChannelGroup_Get3DDopplerLevel_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT ChannelGroup_Set3DDistanceFilter_Delegate_1(IntPtr channelgroup, bool custom, float customLevel, float centerFreq);
        private static ChannelGroup_Set3DDistanceFilter_Delegate_1 s_ChannelGroup_Set3DDistanceFilter_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT ChannelGroup_Get3DDistanceFilter_Delegate_1(IntPtr channelgroup, out bool custom, out float customLevel, out float centerFreq);
        private static ChannelGroup_Get3DDistanceFilter_Delegate_1 s_ChannelGroup_Get3DDistanceFilter_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT ChannelGroup_SetUserData_Delegate_1(IntPtr channelgroup, IntPtr userdata);
        private static ChannelGroup_SetUserData_Delegate_1 s_ChannelGroup_SetUserData_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT ChannelGroup_GetUserData_Delegate_1(IntPtr channelgroup, out IntPtr userdata);
        private static ChannelGroup_GetUserData_Delegate_1 s_ChannelGroup_GetUserData_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT SoundGroup_Release_Delegate_1(IntPtr soundgroup);
        private static SoundGroup_Release_Delegate_1 s_SoundGroup_Release_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT SoundGroup_GetSystemObject_Delegate_1(IntPtr soundgroup, out IntPtr system);
        private static SoundGroup_GetSystemObject_Delegate_1 s_SoundGroup_GetSystemObject_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT SoundGroup_SetMaxAudible_Delegate_1(IntPtr soundgroup, int maxaudible);
        private static SoundGroup_SetMaxAudible_Delegate_1 s_SoundGroup_SetMaxAudible_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT SoundGroup_GetMaxAudible_Delegate_1(IntPtr soundgroup, out int maxaudible);
        private static SoundGroup_GetMaxAudible_Delegate_1 s_SoundGroup_GetMaxAudible_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT SoundGroup_SetMaxAudibleBehavior_Delegate_1(IntPtr soundgroup, SOUNDGROUP_BEHAVIOR behavior);
        private static SoundGroup_SetMaxAudibleBehavior_Delegate_1 s_SoundGroup_SetMaxAudibleBehavior_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT SoundGroup_GetMaxAudibleBehavior_Delegate_1(IntPtr soundgroup, out SOUNDGROUP_BEHAVIOR behavior);
        private static SoundGroup_GetMaxAudibleBehavior_Delegate_1 s_SoundGroup_GetMaxAudibleBehavior_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT SoundGroup_SetMuteFadeSpeed_Delegate_1(IntPtr soundgroup, float speed);
        private static SoundGroup_SetMuteFadeSpeed_Delegate_1 s_SoundGroup_SetMuteFadeSpeed_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT SoundGroup_GetMuteFadeSpeed_Delegate_1(IntPtr soundgroup, out float speed);
        private static SoundGroup_GetMuteFadeSpeed_Delegate_1 s_SoundGroup_GetMuteFadeSpeed_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT SoundGroup_SetVolume_Delegate_1(IntPtr soundgroup, float volume);
        private static SoundGroup_SetVolume_Delegate_1 s_SoundGroup_SetVolume_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT SoundGroup_GetVolume_Delegate_1(IntPtr soundgroup, out float volume);
        private static SoundGroup_GetVolume_Delegate_1 s_SoundGroup_GetVolume_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT SoundGroup_Stop_Delegate_1(IntPtr soundgroup);
        private static SoundGroup_Stop_Delegate_1 s_SoundGroup_Stop_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT SoundGroup_GetName_Delegate_1(IntPtr soundgroup, IntPtr name, int namelen);
        private static SoundGroup_GetName_Delegate_1 s_SoundGroup_GetName_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT SoundGroup_GetNumSounds_Delegate_1(IntPtr soundgroup, out int numsounds);
        private static SoundGroup_GetNumSounds_Delegate_1 s_SoundGroup_GetNumSounds_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT SoundGroup_GetSound_Delegate_1(IntPtr soundgroup, int index, out IntPtr sound);
        private static SoundGroup_GetSound_Delegate_1 s_SoundGroup_GetSound_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT SoundGroup_GetNumPlaying_Delegate_1(IntPtr soundgroup, out int numplaying);
        private static SoundGroup_GetNumPlaying_Delegate_1 s_SoundGroup_GetNumPlaying_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT SoundGroup_SetUserData_Delegate_1(IntPtr soundgroup, IntPtr userdata);
        private static SoundGroup_SetUserData_Delegate_1 s_SoundGroup_SetUserData_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT SoundGroup_GetUserData_Delegate_1(IntPtr soundgroup, out IntPtr userdata);
        private static SoundGroup_GetUserData_Delegate_1 s_SoundGroup_GetUserData_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT DSP_Release_Delegate_1(IntPtr dsp);
        private static DSP_Release_Delegate_1 s_DSP_Release_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT DSP_GetSystemObject_Delegate_1(IntPtr dsp, out IntPtr system);
        private static DSP_GetSystemObject_Delegate_1 s_DSP_GetSystemObject_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT DSP_AddInput_Delegate_1(IntPtr dsp, IntPtr input, IntPtr zero, DSPCONNECTION_TYPE type);
        private static DSP_AddInput_Delegate_1 s_DSP_AddInput_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT DSP_AddInput_Delegate_2(IntPtr dsp, IntPtr input, out IntPtr connection, DSPCONNECTION_TYPE type);
        private static DSP_AddInput_Delegate_2 s_DSP_AddInput_2;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT DSP_AddInputPreallocated_Delegate_1(IntPtr dsp, IntPtr input, out IntPtr connection);
        private static DSP_AddInputPreallocated_Delegate_1 s_DSP_AddInputPreallocated_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT DSP_DisconnectFrom_Delegate_1(IntPtr dsp, IntPtr target, IntPtr connection);
        private static DSP_DisconnectFrom_Delegate_1 s_DSP_DisconnectFrom_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT DSP_DisconnectAll_Delegate_1(IntPtr dsp, bool inputs, bool outputs);
        private static DSP_DisconnectAll_Delegate_1 s_DSP_DisconnectAll_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT DSP_GetNumInputs_Delegate_1(IntPtr dsp, out int numinputs);
        private static DSP_GetNumInputs_Delegate_1 s_DSP_GetNumInputs_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT DSP_GetNumOutputs_Delegate_1(IntPtr dsp, out int numoutputs);
        private static DSP_GetNumOutputs_Delegate_1 s_DSP_GetNumOutputs_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT DSP_GetInput_Delegate_1(IntPtr dsp, int index, out IntPtr input, out IntPtr inputconnection);
        private static DSP_GetInput_Delegate_1 s_DSP_GetInput_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT DSP_GetOutput_Delegate_1(IntPtr dsp, int index, out IntPtr output, out IntPtr outputconnection);
        private static DSP_GetOutput_Delegate_1 s_DSP_GetOutput_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT DSP_SetActive_Delegate_1(IntPtr dsp, bool active);
        private static DSP_SetActive_Delegate_1 s_DSP_SetActive_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT DSP_GetActive_Delegate_1(IntPtr dsp, out bool active);
        private static DSP_GetActive_Delegate_1 s_DSP_GetActive_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT DSP_SetBypass_Delegate_1(IntPtr dsp, bool bypass);
        private static DSP_SetBypass_Delegate_1 s_DSP_SetBypass_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT DSP_GetBypass_Delegate_1(IntPtr dsp, out bool bypass);
        private static DSP_GetBypass_Delegate_1 s_DSP_GetBypass_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT DSP_SetWetDryMix_Delegate_1(IntPtr dsp, float prewet, float postwet, float dry);
        private static DSP_SetWetDryMix_Delegate_1 s_DSP_SetWetDryMix_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT DSP_GetWetDryMix_Delegate_1(IntPtr dsp, out float prewet, out float postwet, out float dry);
        private static DSP_GetWetDryMix_Delegate_1 s_DSP_GetWetDryMix_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT DSP_SetChannelFormat_Delegate_1(IntPtr dsp, CHANNELMASK channelmask, int numchannels, SPEAKERMODE source_speakermode);
        private static DSP_SetChannelFormat_Delegate_1 s_DSP_SetChannelFormat_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT DSP_GetChannelFormat_Delegate_1(IntPtr dsp, out CHANNELMASK channelmask, out int numchannels, out SPEAKERMODE source_speakermode);
        private static DSP_GetChannelFormat_Delegate_1 s_DSP_GetChannelFormat_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT DSP_GetOutputChannelFormat_Delegate_1(IntPtr dsp, CHANNELMASK inmask, int inchannels, SPEAKERMODE inspeakermode, out CHANNELMASK outmask, out int outchannels, out SPEAKERMODE outspeakermode);
        private static DSP_GetOutputChannelFormat_Delegate_1 s_DSP_GetOutputChannelFormat_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT DSP_Reset_Delegate_1(IntPtr dsp);
        private static DSP_Reset_Delegate_1 s_DSP_Reset_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT DSP_SetCallback_Delegate_1(IntPtr dsp, DSP_CALLBACK callback);
        private static DSP_SetCallback_Delegate_1 s_DSP_SetCallback_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT DSP_SetParameterFloat_Delegate_1(IntPtr dsp, int index, float value);
        private static DSP_SetParameterFloat_Delegate_1 s_DSP_SetParameterFloat_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT DSP_SetParameterInt_Delegate_1(IntPtr dsp, int index, int value);
        private static DSP_SetParameterInt_Delegate_1 s_DSP_SetParameterInt_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT DSP_SetParameterBool_Delegate_1(IntPtr dsp, int index, bool value);
        private static DSP_SetParameterBool_Delegate_1 s_DSP_SetParameterBool_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT DSP_SetParameterData_Delegate_1(IntPtr dsp, int index, byte[] data, uint length);
        private static DSP_SetParameterData_Delegate_1 s_DSP_SetParameterData_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT DSP_GetParameterFloat_Delegate_1(IntPtr dsp, int index, out float value, IntPtr valuestr, int valuestrlen);
        private static DSP_GetParameterFloat_Delegate_1 s_DSP_GetParameterFloat_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT DSP_GetParameterInt_Delegate_1(IntPtr dsp, int index, out int value, IntPtr valuestr, int valuestrlen);
        private static DSP_GetParameterInt_Delegate_1 s_DSP_GetParameterInt_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT DSP_GetParameterBool_Delegate_1(IntPtr dsp, int index, out bool value, IntPtr valuestr, int valuestrlen);
        private static DSP_GetParameterBool_Delegate_1 s_DSP_GetParameterBool_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT DSP_GetParameterData_Delegate_1(IntPtr dsp, int index, out IntPtr data, out uint length, IntPtr valuestr, int valuestrlen);
        private static DSP_GetParameterData_Delegate_1 s_DSP_GetParameterData_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT DSP_GetNumParameters_Delegate_1(IntPtr dsp, out int numparams);
        private static DSP_GetNumParameters_Delegate_1 s_DSP_GetNumParameters_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT DSP_GetParameterInfo_Delegate_1(IntPtr dsp, int index, out IntPtr desc);
        private static DSP_GetParameterInfo_Delegate_1 s_DSP_GetParameterInfo_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT DSP_GetDataParameterIndex_Delegate_1(IntPtr dsp, int datatype, out int index);
        private static DSP_GetDataParameterIndex_Delegate_1 s_DSP_GetDataParameterIndex_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT DSP_ShowConfigDialog_Delegate_1(IntPtr dsp, IntPtr hwnd, bool show);
        private static DSP_ShowConfigDialog_Delegate_1 s_DSP_ShowConfigDialog_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT DSP_GetInfo_Delegate_1(IntPtr dsp, IntPtr name, out uint version, out int channels, out int configwidth, out int configheight);
        private static DSP_GetInfo_Delegate_1 s_DSP_GetInfo_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT DSP_GetType_Delegate_1(IntPtr dsp, out DSP_TYPE type);
        private static DSP_GetType_Delegate_1 s_DSP_GetType_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT DSP_GetIdle_Delegate_1(IntPtr dsp, out bool idle);
        private static DSP_GetIdle_Delegate_1 s_DSP_GetIdle_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT DSP_SetUserData_Delegate_1(IntPtr dsp, IntPtr userdata);
        private static DSP_SetUserData_Delegate_1 s_DSP_SetUserData_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT DSP_GetUserData_Delegate_1(IntPtr dsp, out IntPtr userdata);
        private static DSP_GetUserData_Delegate_1 s_DSP_GetUserData_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT DSPConnection_GetInput_Delegate_1(IntPtr dspconnection, out IntPtr input);
        private static DSPConnection_GetInput_Delegate_1 s_DSPConnection_GetInput_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT DSPConnection_GetOutput_Delegate_1(IntPtr dspconnection, out IntPtr output);
        private static DSPConnection_GetOutput_Delegate_1 s_DSPConnection_GetOutput_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT DSPConnection_SetMix_Delegate_1(IntPtr dspconnection, float volume);
        private static DSPConnection_SetMix_Delegate_1 s_DSPConnection_SetMix_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT DSPConnection_GetMix_Delegate_1(IntPtr dspconnection, out float volume);
        private static DSPConnection_GetMix_Delegate_1 s_DSPConnection_GetMix_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT DSPConnection_SetMixMatrix_Delegate_1(IntPtr dspconnection, float[] matrix, int outchannels, int inchannels, int inchannel_hop);
        private static DSPConnection_SetMixMatrix_Delegate_1 s_DSPConnection_SetMixMatrix_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT DSPConnection_GetMixMatrix_Delegate_1(IntPtr dspconnection, float[] matrix, out int outchannels, out int inchannels, int inchannel_hop);
        private static DSPConnection_GetMixMatrix_Delegate_1 s_DSPConnection_GetMixMatrix_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT DSPConnection_GetType_Delegate_1(IntPtr dspconnection, out DSPCONNECTION_TYPE type);
        private static DSPConnection_GetType_Delegate_1 s_DSPConnection_GetType_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT DSPConnection_SetUserData_Delegate_1(IntPtr dspconnection, IntPtr userdata);
        private static DSPConnection_SetUserData_Delegate_1 s_DSPConnection_SetUserData_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT DSPConnection_GetUserData_Delegate_1(IntPtr dspconnection, out IntPtr userdata);
        private static DSPConnection_GetUserData_Delegate_1 s_DSPConnection_GetUserData_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Geometry_Release_Delegate_1(IntPtr geometry);
        private static Geometry_Release_Delegate_1 s_Geometry_Release_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Geometry_AddPolygon_Delegate_1(IntPtr geometry, float directocclusion, float reverbocclusion, bool doublesided, int numvertices, VECTOR[] vertices, out int polygonindex);
        private static Geometry_AddPolygon_Delegate_1 s_Geometry_AddPolygon_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Geometry_GetNumPolygons_Delegate_1(IntPtr geometry, out int numpolygons);
        private static Geometry_GetNumPolygons_Delegate_1 s_Geometry_GetNumPolygons_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Geometry_GetMaxPolygons_Delegate_1(IntPtr geometry, out int maxpolygons, out int maxvertices);
        private static Geometry_GetMaxPolygons_Delegate_1 s_Geometry_GetMaxPolygons_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Geometry_GetPolygonNumVertices_Delegate_1(IntPtr geometry, int index, out int numvertices);
        private static Geometry_GetPolygonNumVertices_Delegate_1 s_Geometry_GetPolygonNumVertices_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Geometry_SetPolygonVertex_Delegate_1(IntPtr geometry, int index, int vertexindex, ref VECTOR vertex);
        private static Geometry_SetPolygonVertex_Delegate_1 s_Geometry_SetPolygonVertex_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Geometry_GetPolygonVertex_Delegate_1(IntPtr geometry, int index, int vertexindex, out VECTOR vertex);
        private static Geometry_GetPolygonVertex_Delegate_1 s_Geometry_GetPolygonVertex_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Geometry_SetPolygonAttributes_Delegate_1(IntPtr geometry, int index, float directocclusion, float reverbocclusion, bool doublesided);
        private static Geometry_SetPolygonAttributes_Delegate_1 s_Geometry_SetPolygonAttributes_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Geometry_GetPolygonAttributes_Delegate_1(IntPtr geometry, int index, out float directocclusion, out float reverbocclusion, out bool doublesided);
        private static Geometry_GetPolygonAttributes_Delegate_1 s_Geometry_GetPolygonAttributes_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Geometry_SetActive_Delegate_1(IntPtr geometry, bool active);
        private static Geometry_SetActive_Delegate_1 s_Geometry_SetActive_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Geometry_GetActive_Delegate_1(IntPtr geometry, out bool active);
        private static Geometry_GetActive_Delegate_1 s_Geometry_GetActive_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Geometry_SetRotation_Delegate_1(IntPtr geometry, ref VECTOR forward, ref VECTOR up);
        private static Geometry_SetRotation_Delegate_1 s_Geometry_SetRotation_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Geometry_GetRotation_Delegate_1(IntPtr geometry, out VECTOR forward, out VECTOR up);
        private static Geometry_GetRotation_Delegate_1 s_Geometry_GetRotation_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Geometry_SetPosition_Delegate_1(IntPtr geometry, ref VECTOR position);
        private static Geometry_SetPosition_Delegate_1 s_Geometry_SetPosition_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Geometry_GetPosition_Delegate_1(IntPtr geometry, out VECTOR position);
        private static Geometry_GetPosition_Delegate_1 s_Geometry_GetPosition_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Geometry_SetScale_Delegate_1(IntPtr geometry, ref VECTOR scale);
        private static Geometry_SetScale_Delegate_1 s_Geometry_SetScale_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Geometry_GetScale_Delegate_1(IntPtr geometry, out VECTOR scale);
        private static Geometry_GetScale_Delegate_1 s_Geometry_GetScale_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Geometry_Save_Delegate_1(IntPtr geometry, IntPtr data, out int datasize);
        private static Geometry_Save_Delegate_1 s_Geometry_Save_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Geometry_SetUserData_Delegate_1(IntPtr geometry, IntPtr userdata);
        private static Geometry_SetUserData_Delegate_1 s_Geometry_SetUserData_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Geometry_GetUserData_Delegate_1(IntPtr geometry, out IntPtr userdata);
        private static Geometry_GetUserData_Delegate_1 s_Geometry_GetUserData_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Reverb3D_Release_Delegate_1(IntPtr reverb3d);
        private static Reverb3D_Release_Delegate_1 s_Reverb3D_Release_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Reverb3D_Set3DAttributes_Delegate_1(IntPtr reverb3d, ref VECTOR position, float mindistance, float maxdistance);
        private static Reverb3D_Set3DAttributes_Delegate_1 s_Reverb3D_Set3DAttributes_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Reverb3D_Get3DAttributes_Delegate_1(IntPtr reverb3d, ref VECTOR position, ref float mindistance, ref float maxdistance);
        private static Reverb3D_Get3DAttributes_Delegate_1 s_Reverb3D_Get3DAttributes_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Reverb3D_SetProperties_Delegate_1(IntPtr reverb3d, ref REVERB_PROPERTIES properties);
        private static Reverb3D_SetProperties_Delegate_1 s_Reverb3D_SetProperties_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Reverb3D_GetProperties_Delegate_1(IntPtr reverb3d, ref REVERB_PROPERTIES properties);
        private static Reverb3D_GetProperties_Delegate_1 s_Reverb3D_GetProperties_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Reverb3D_SetActive_Delegate_1(IntPtr reverb3d, bool active);
        private static Reverb3D_SetActive_Delegate_1 s_Reverb3D_SetActive_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Reverb3D_GetActive_Delegate_1(IntPtr reverb3d, out bool active);
        private static Reverb3D_GetActive_Delegate_1 s_Reverb3D_GetActive_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Reverb3D_SetUserData_Delegate_1(IntPtr reverb3d, IntPtr userdata);
        private static Reverb3D_SetUserData_Delegate_1 s_Reverb3D_SetUserData_1;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate RESULT Reverb3D_GetUserData_Delegate_1(IntPtr reverb3d, out IntPtr userdata);
        private static Reverb3D_GetUserData_Delegate_1 s_Reverb3D_GetUserData_1;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_ParseID( byte[] idString, out GUID id )
        {
            s_Studio_ParseID_1 ??= LoadStudioFunction<Studio_ParseID_Delegate_1>("FMOD_Studio_ParseID");
            return s_Studio_ParseID_1(idString, out id);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_System_Create( out IntPtr system, uint headerversion )
        {
            s_Studio_System_Create_1 ??= LoadStudioFunction<Studio_System_Create_Delegate_1>("FMOD_Studio_System_Create");
            return s_Studio_System_Create_1(out system, headerversion);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Studio_System_IsValid( IntPtr system )
        {
            s_Studio_System_IsValid_1 ??= LoadStudioFunction<Studio_System_IsValid_Delegate_1>("FMOD_Studio_System_IsValid");
            return s_Studio_System_IsValid_1(system);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_System_SetAdvancedSettings( IntPtr system, ref Studio.ADVANCEDSETTINGS settings )
        {
            s_Studio_System_SetAdvancedSettings_1 ??= LoadStudioFunction<Studio_System_SetAdvancedSettings_Delegate_1>("FMOD_Studio_System_SetAdvancedSettings");
            return s_Studio_System_SetAdvancedSettings_1(system, ref settings);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_System_GetAdvancedSettings( IntPtr system, out Studio.ADVANCEDSETTINGS settings )
        {
            s_Studio_System_GetAdvancedSettings_1 ??= LoadStudioFunction<Studio_System_GetAdvancedSettings_Delegate_1>("FMOD_Studio_System_GetAdvancedSettings");
            return s_Studio_System_GetAdvancedSettings_1(system, out settings);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_System_Initialize( IntPtr system, int maxchannels, Studio.INITFLAGS studioflags, FMOD.INITFLAGS flags, IntPtr extradriverdata )
        {
            s_Studio_System_Initialize_1 ??= LoadStudioFunction<Studio_System_Initialize_Delegate_1>("FMOD_Studio_System_Initialize");
            return s_Studio_System_Initialize_1(system, maxchannels, studioflags, flags, extradriverdata);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_System_Release( IntPtr system )
        {
            s_Studio_System_Release_1 ??= LoadStudioFunction<Studio_System_Release_Delegate_1>("FMOD_Studio_System_Release");
            return s_Studio_System_Release_1(system);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_System_Update( IntPtr system )
        {
            s_Studio_System_Update_1 ??= LoadStudioFunction<Studio_System_Update_Delegate_1>("FMOD_Studio_System_Update");
            return s_Studio_System_Update_1(system);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_System_GetCoreSystem( IntPtr system, out IntPtr coresystem )
        {
            s_Studio_System_GetCoreSystem_1 ??= LoadStudioFunction<Studio_System_GetCoreSystem_Delegate_1>("FMOD_Studio_System_GetCoreSystem");
            return s_Studio_System_GetCoreSystem_1(system, out coresystem);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_System_GetEvent( IntPtr system, byte[] path, out IntPtr _event )
        {
            s_Studio_System_GetEvent_1 ??= LoadStudioFunction<Studio_System_GetEvent_Delegate_1>("FMOD_Studio_System_GetEvent");
            return s_Studio_System_GetEvent_1(system, path, out _event);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_System_GetBus( IntPtr system, byte[] path, out IntPtr bus )
        {
            s_Studio_System_GetBus_1 ??= LoadStudioFunction<Studio_System_GetBus_Delegate_1>("FMOD_Studio_System_GetBus");
            return s_Studio_System_GetBus_1(system, path, out bus);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_System_GetVCA( IntPtr system, byte[] path, out IntPtr vca )
        {
            s_Studio_System_GetVCA_1 ??= LoadStudioFunction<Studio_System_GetVCA_Delegate_1>("FMOD_Studio_System_GetVCA");
            return s_Studio_System_GetVCA_1(system, path, out vca);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_System_GetBank( IntPtr system, byte[] path, out IntPtr bank )
        {
            s_Studio_System_GetBank_1 ??= LoadStudioFunction<Studio_System_GetBank_Delegate_1>("FMOD_Studio_System_GetBank");
            return s_Studio_System_GetBank_1(system, path, out bank);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_System_GetEventByID( IntPtr system, ref GUID id, out IntPtr _event )
        {
            s_Studio_System_GetEventByID_1 ??= LoadStudioFunction<Studio_System_GetEventByID_Delegate_1>("FMOD_Studio_System_GetEventByID");
            return s_Studio_System_GetEventByID_1(system, ref id, out _event);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_System_GetBusByID( IntPtr system, ref GUID id, out IntPtr bus )
        {
            s_Studio_System_GetBusByID_1 ??= LoadStudioFunction<Studio_System_GetBusByID_Delegate_1>("FMOD_Studio_System_GetBusByID");
            return s_Studio_System_GetBusByID_1(system, ref id, out bus);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_System_GetVCAByID( IntPtr system, ref GUID id, out IntPtr vca )
        {
            s_Studio_System_GetVCAByID_1 ??= LoadStudioFunction<Studio_System_GetVCAByID_Delegate_1>("FMOD_Studio_System_GetVCAByID");
            return s_Studio_System_GetVCAByID_1(system, ref id, out vca);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_System_GetBankByID( IntPtr system, ref GUID id, out IntPtr bank )
        {
            s_Studio_System_GetBankByID_1 ??= LoadStudioFunction<Studio_System_GetBankByID_Delegate_1>("FMOD_Studio_System_GetBankByID");
            return s_Studio_System_GetBankByID_1(system, ref id, out bank);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_System_GetSoundInfo( IntPtr system, byte[] key, out Studio.SOUND_INFO info )
        {
            s_Studio_System_GetSoundInfo_1 ??= LoadStudioFunction<Studio_System_GetSoundInfo_Delegate_1>("FMOD_Studio_System_GetSoundInfo");
            return s_Studio_System_GetSoundInfo_1(system, key, out info);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_System_GetParameterDescriptionByName( IntPtr system, byte[] name, out Studio.PARAMETER_DESCRIPTION parameter )
        {
            s_Studio_System_GetParameterDescriptionByName_1 ??= LoadStudioFunction<Studio_System_GetParameterDescriptionByName_Delegate_1>("FMOD_Studio_System_GetParameterDescriptionByName");
            return s_Studio_System_GetParameterDescriptionByName_1(system, name, out parameter);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_System_GetParameterDescriptionByID( IntPtr system, Studio.PARAMETER_ID id, out Studio.PARAMETER_DESCRIPTION parameter )
        {
            s_Studio_System_GetParameterDescriptionByID_1 ??= LoadStudioFunction<Studio_System_GetParameterDescriptionByID_Delegate_1>("FMOD_Studio_System_GetParameterDescriptionByID");
            return s_Studio_System_GetParameterDescriptionByID_1(system, id, out parameter);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_System_GetParameterLabelByName( IntPtr system, byte[] name, int labelindex, IntPtr label, int size, out int retrieved )
        {
            s_Studio_System_GetParameterLabelByName_1 ??= LoadStudioFunction<Studio_System_GetParameterLabelByName_Delegate_1>("FMOD_Studio_System_GetParameterLabelByName");
            return s_Studio_System_GetParameterLabelByName_1(system, name, labelindex, label, size, out retrieved);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_System_GetParameterLabelByID( IntPtr system, Studio.PARAMETER_ID id, int labelindex, IntPtr label, int size, out int retrieved )
        {
            s_Studio_System_GetParameterLabelByID_1 ??= LoadStudioFunction<Studio_System_GetParameterLabelByID_Delegate_1>("FMOD_Studio_System_GetParameterLabelByID");
            return s_Studio_System_GetParameterLabelByID_1(system, id, labelindex, label, size, out retrieved);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_System_GetParameterByID( IntPtr system, Studio.PARAMETER_ID id, out float value, out float finalvalue )
        {
            s_Studio_System_GetParameterByID_1 ??= LoadStudioFunction<Studio_System_GetParameterByID_Delegate_1>("FMOD_Studio_System_GetParameterByID");
            return s_Studio_System_GetParameterByID_1(system, id, out value, out finalvalue);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_System_SetParameterByID( IntPtr system, Studio.PARAMETER_ID id, float value, bool ignoreseekspeed )
        {
            s_Studio_System_SetParameterByID_1 ??= LoadStudioFunction<Studio_System_SetParameterByID_Delegate_1>("FMOD_Studio_System_SetParameterByID");
            return s_Studio_System_SetParameterByID_1(system, id, value, ignoreseekspeed);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_System_SetParameterByIDWithLabel( IntPtr system, Studio.PARAMETER_ID id, byte[] label, bool ignoreseekspeed )
        {
            s_Studio_System_SetParameterByIDWithLabel_1 ??= LoadStudioFunction<Studio_System_SetParameterByIDWithLabel_Delegate_1>("FMOD_Studio_System_SetParameterByIDWithLabel");
            return s_Studio_System_SetParameterByIDWithLabel_1(system, id, label, ignoreseekspeed);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_System_SetParametersByIDs( IntPtr system, Studio.PARAMETER_ID[] ids, float[] values, int count, bool ignoreseekspeed )
        {
            s_Studio_System_SetParametersByIDs_1 ??= LoadStudioFunction<Studio_System_SetParametersByIDs_Delegate_1>("FMOD_Studio_System_SetParametersByIDs");
            return s_Studio_System_SetParametersByIDs_1(system, ids, values, count, ignoreseekspeed);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_System_GetParameterByName( IntPtr system, byte[] name, out float value, out float finalvalue )
        {
            s_Studio_System_GetParameterByName_1 ??= LoadStudioFunction<Studio_System_GetParameterByName_Delegate_1>("FMOD_Studio_System_GetParameterByName");
            return s_Studio_System_GetParameterByName_1(system, name, out value, out finalvalue);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_System_SetParameterByName( IntPtr system, byte[] name, float value, bool ignoreseekspeed )
        {
            s_Studio_System_SetParameterByName_1 ??= LoadStudioFunction<Studio_System_SetParameterByName_Delegate_1>("FMOD_Studio_System_SetParameterByName");
            return s_Studio_System_SetParameterByName_1(system, name, value, ignoreseekspeed);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_System_SetParameterByNameWithLabel( IntPtr system, byte[] name, byte[] label, bool ignoreseekspeed )
        {
            s_Studio_System_SetParameterByNameWithLabel_1 ??= LoadStudioFunction<Studio_System_SetParameterByNameWithLabel_Delegate_1>("FMOD_Studio_System_SetParameterByNameWithLabel");
            return s_Studio_System_SetParameterByNameWithLabel_1(system, name, label, ignoreseekspeed);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_System_LookupID( IntPtr system, byte[] path, out GUID id )
        {
            s_Studio_System_LookupID_1 ??= LoadStudioFunction<Studio_System_LookupID_Delegate_1>("FMOD_Studio_System_LookupID");
            return s_Studio_System_LookupID_1(system, path, out id);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_System_LookupPath( IntPtr system, ref GUID id, IntPtr path, int size, out int retrieved )
        {
            s_Studio_System_LookupPath_1 ??= LoadStudioFunction<Studio_System_LookupPath_Delegate_1>("FMOD_Studio_System_LookupPath");
            return s_Studio_System_LookupPath_1(system, ref id, path, size, out retrieved);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_System_GetNumListeners( IntPtr system, out int numlisteners )
        {
            s_Studio_System_GetNumListeners_1 ??= LoadStudioFunction<Studio_System_GetNumListeners_Delegate_1>("FMOD_Studio_System_GetNumListeners");
            return s_Studio_System_GetNumListeners_1(system, out numlisteners);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_System_SetNumListeners( IntPtr system, int numlisteners )
        {
            s_Studio_System_SetNumListeners_1 ??= LoadStudioFunction<Studio_System_SetNumListeners_Delegate_1>("FMOD_Studio_System_SetNumListeners");
            return s_Studio_System_SetNumListeners_1(system, numlisteners);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_System_GetListenerAttributes( IntPtr system, int listener, out ATTRIBUTES_3D attributes, IntPtr zero )
        {
            s_Studio_System_GetListenerAttributes_1 ??= LoadStudioFunction<Studio_System_GetListenerAttributes_Delegate_1>("FMOD_Studio_System_GetListenerAttributes");
            return s_Studio_System_GetListenerAttributes_1(system, listener, out attributes, zero);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_System_GetListenerAttributes( IntPtr system, int listener, out ATTRIBUTES_3D attributes, out VECTOR attenuationposition )
        {
            s_Studio_System_GetListenerAttributes_2 ??= LoadStudioFunction<Studio_System_GetListenerAttributes_Delegate_2>("FMOD_Studio_System_GetListenerAttributes");
            return s_Studio_System_GetListenerAttributes_2(system, listener, out attributes, out attenuationposition);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_System_SetListenerAttributes( IntPtr system, int listener, ref ATTRIBUTES_3D attributes, IntPtr zero )
        {
            s_Studio_System_SetListenerAttributes_1 ??= LoadStudioFunction<Studio_System_SetListenerAttributes_Delegate_1>("FMOD_Studio_System_SetListenerAttributes");
            return s_Studio_System_SetListenerAttributes_1(system, listener, ref attributes, zero);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_System_SetListenerAttributes( IntPtr system, int listener, ref ATTRIBUTES_3D attributes, ref VECTOR attenuationposition )
        {
            s_Studio_System_SetListenerAttributes_2 ??= LoadStudioFunction<Studio_System_SetListenerAttributes_Delegate_2>("FMOD_Studio_System_SetListenerAttributes");
            return s_Studio_System_SetListenerAttributes_2(system, listener, ref attributes, ref attenuationposition);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_System_GetListenerWeight( IntPtr system, int listener, out float weight )
        {
            s_Studio_System_GetListenerWeight_1 ??= LoadStudioFunction<Studio_System_GetListenerWeight_Delegate_1>("FMOD_Studio_System_GetListenerWeight");
            return s_Studio_System_GetListenerWeight_1(system, listener, out weight);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_System_SetListenerWeight( IntPtr system, int listener, float weight )
        {
            s_Studio_System_SetListenerWeight_1 ??= LoadStudioFunction<Studio_System_SetListenerWeight_Delegate_1>("FMOD_Studio_System_SetListenerWeight");
            return s_Studio_System_SetListenerWeight_1(system, listener, weight);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_System_LoadBankFile( IntPtr system, byte[] filename, Studio.LOAD_BANK_FLAGS flags, out IntPtr bank )
        {
            s_Studio_System_LoadBankFile_1 ??= LoadStudioFunction<Studio_System_LoadBankFile_Delegate_1>("FMOD_Studio_System_LoadBankFile");
            return s_Studio_System_LoadBankFile_1(system, filename, flags, out bank);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_System_LoadBankMemory( IntPtr system, IntPtr buffer, int length, Studio.LOAD_MEMORY_MODE mode, Studio.LOAD_BANK_FLAGS flags, out IntPtr bank )
        {
            s_Studio_System_LoadBankMemory_1 ??= LoadStudioFunction<Studio_System_LoadBankMemory_Delegate_1>("FMOD_Studio_System_LoadBankMemory");
            return s_Studio_System_LoadBankMemory_1(system, buffer, length, mode, flags, out bank);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_System_LoadBankCustom( IntPtr system, ref Studio.BANK_INFO info, Studio.LOAD_BANK_FLAGS flags, out IntPtr bank )
        {
            s_Studio_System_LoadBankCustom_1 ??= LoadStudioFunction<Studio_System_LoadBankCustom_Delegate_1>("FMOD_Studio_System_LoadBankCustom");
            return s_Studio_System_LoadBankCustom_1(system, ref info, flags, out bank);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_System_UnloadAll( IntPtr system )
        {
            s_Studio_System_UnloadAll_1 ??= LoadStudioFunction<Studio_System_UnloadAll_Delegate_1>("FMOD_Studio_System_UnloadAll");
            return s_Studio_System_UnloadAll_1(system);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_System_FlushCommands( IntPtr system )
        {
            s_Studio_System_FlushCommands_1 ??= LoadStudioFunction<Studio_System_FlushCommands_Delegate_1>("FMOD_Studio_System_FlushCommands");
            return s_Studio_System_FlushCommands_1(system);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_System_FlushSampleLoading( IntPtr system )
        {
            s_Studio_System_FlushSampleLoading_1 ??= LoadStudioFunction<Studio_System_FlushSampleLoading_Delegate_1>("FMOD_Studio_System_FlushSampleLoading");
            return s_Studio_System_FlushSampleLoading_1(system);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_System_StartCommandCapture( IntPtr system, byte[] filename, Studio.COMMANDCAPTURE_FLAGS flags )
        {
            s_Studio_System_StartCommandCapture_1 ??= LoadStudioFunction<Studio_System_StartCommandCapture_Delegate_1>("FMOD_Studio_System_StartCommandCapture");
            return s_Studio_System_StartCommandCapture_1(system, filename, flags);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_System_StopCommandCapture( IntPtr system )
        {
            s_Studio_System_StopCommandCapture_1 ??= LoadStudioFunction<Studio_System_StopCommandCapture_Delegate_1>("FMOD_Studio_System_StopCommandCapture");
            return s_Studio_System_StopCommandCapture_1(system);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_System_LoadCommandReplay( IntPtr system, byte[] filename, Studio.COMMANDREPLAY_FLAGS flags, out IntPtr replay )
        {
            s_Studio_System_LoadCommandReplay_1 ??= LoadStudioFunction<Studio_System_LoadCommandReplay_Delegate_1>("FMOD_Studio_System_LoadCommandReplay");
            return s_Studio_System_LoadCommandReplay_1(system, filename, flags, out replay);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_System_GetBankCount( IntPtr system, out int count )
        {
            s_Studio_System_GetBankCount_1 ??= LoadStudioFunction<Studio_System_GetBankCount_Delegate_1>("FMOD_Studio_System_GetBankCount");
            return s_Studio_System_GetBankCount_1(system, out count);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_System_GetBankList( IntPtr system, IntPtr[] array, int capacity, out int count )
        {
            s_Studio_System_GetBankList_1 ??= LoadStudioFunction<Studio_System_GetBankList_Delegate_1>("FMOD_Studio_System_GetBankList");
            return s_Studio_System_GetBankList_1(system, array, capacity, out count);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_System_GetParameterDescriptionCount( IntPtr system, out int count )
        {
            s_Studio_System_GetParameterDescriptionCount_1 ??= LoadStudioFunction<Studio_System_GetParameterDescriptionCount_Delegate_1>("FMOD_Studio_System_GetParameterDescriptionCount");
            return s_Studio_System_GetParameterDescriptionCount_1(system, out count);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_System_GetParameterDescriptionList( IntPtr system, [Out] Studio.PARAMETER_DESCRIPTION[] array, int capacity, out int count )
        {
            s_Studio_System_GetParameterDescriptionList_1 ??= LoadStudioFunction<Studio_System_GetParameterDescriptionList_Delegate_1>("FMOD_Studio_System_GetParameterDescriptionList");
            return s_Studio_System_GetParameterDescriptionList_1(system, array, capacity, out count);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_System_GetCPUUsage( IntPtr system, out Studio.CPU_USAGE usage, out FMOD.CPU_USAGE usage_core )
        {
            s_Studio_System_GetCPUUsage_1 ??= LoadStudioFunction<Studio_System_GetCPUUsage_Delegate_1>("FMOD_Studio_System_GetCPUUsage");
            return s_Studio_System_GetCPUUsage_1(system, out usage, out usage_core);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_System_GetBufferUsage( IntPtr system, out Studio.BUFFER_USAGE usage )
        {
            s_Studio_System_GetBufferUsage_1 ??= LoadStudioFunction<Studio_System_GetBufferUsage_Delegate_1>("FMOD_Studio_System_GetBufferUsage");
            return s_Studio_System_GetBufferUsage_1(system, out usage);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_System_ResetBufferUsage( IntPtr system )
        {
            s_Studio_System_ResetBufferUsage_1 ??= LoadStudioFunction<Studio_System_ResetBufferUsage_Delegate_1>("FMOD_Studio_System_ResetBufferUsage");
            return s_Studio_System_ResetBufferUsage_1(system);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_System_SetCallback( IntPtr system, Studio.SYSTEM_CALLBACK callback, Studio.SYSTEM_CALLBACK_TYPE callbackmask )
        {
            s_Studio_System_SetCallback_1 ??= LoadStudioFunction<Studio_System_SetCallback_Delegate_1>("FMOD_Studio_System_SetCallback");
            return s_Studio_System_SetCallback_1(system, callback, callbackmask);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_System_GetUserData( IntPtr system, out IntPtr userdata )
        {
            s_Studio_System_GetUserData_1 ??= LoadStudioFunction<Studio_System_GetUserData_Delegate_1>("FMOD_Studio_System_GetUserData");
            return s_Studio_System_GetUserData_1(system, out userdata);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_System_SetUserData( IntPtr system, IntPtr userdata )
        {
            s_Studio_System_SetUserData_1 ??= LoadStudioFunction<Studio_System_SetUserData_Delegate_1>("FMOD_Studio_System_SetUserData");
            return s_Studio_System_SetUserData_1(system, userdata);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_System_GetMemoryUsage( IntPtr system, out Studio.MEMORY_USAGE memoryusage )
        {
            s_Studio_System_GetMemoryUsage_1 ??= LoadStudioFunction<Studio_System_GetMemoryUsage_Delegate_1>("FMOD_Studio_System_GetMemoryUsage");
            return s_Studio_System_GetMemoryUsage_1(system, out memoryusage);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Studio_EventDescription_IsValid( IntPtr eventdescription )
        {
            s_Studio_EventDescription_IsValid_1 ??= LoadStudioFunction<Studio_EventDescription_IsValid_Delegate_1>("FMOD_Studio_EventDescription_IsValid");
            return s_Studio_EventDescription_IsValid_1(eventdescription);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_EventDescription_GetID( IntPtr eventdescription, out GUID id )
        {
            s_Studio_EventDescription_GetID_1 ??= LoadStudioFunction<Studio_EventDescription_GetID_Delegate_1>("FMOD_Studio_EventDescription_GetID");
            return s_Studio_EventDescription_GetID_1(eventdescription, out id);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_EventDescription_GetPath( IntPtr eventdescription, IntPtr path, int size, out int retrieved )
        {
            s_Studio_EventDescription_GetPath_1 ??= LoadStudioFunction<Studio_EventDescription_GetPath_Delegate_1>("FMOD_Studio_EventDescription_GetPath");
            return s_Studio_EventDescription_GetPath_1(eventdescription, path, size, out retrieved);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_EventDescription_GetParameterDescriptionCount( IntPtr eventdescription, out int count )
        {
            s_Studio_EventDescription_GetParameterDescriptionCount_1 ??= LoadStudioFunction<Studio_EventDescription_GetParameterDescriptionCount_Delegate_1>("FMOD_Studio_EventDescription_GetParameterDescriptionCount");
            return s_Studio_EventDescription_GetParameterDescriptionCount_1(eventdescription, out count);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_EventDescription_GetParameterDescriptionByIndex( IntPtr eventdescription, int index, out Studio.PARAMETER_DESCRIPTION parameter )
        {
            s_Studio_EventDescription_GetParameterDescriptionByIndex_1 ??= LoadStudioFunction<Studio_EventDescription_GetParameterDescriptionByIndex_Delegate_1>("FMOD_Studio_EventDescription_GetParameterDescriptionByIndex");
            return s_Studio_EventDescription_GetParameterDescriptionByIndex_1(eventdescription, index, out parameter);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_EventDescription_GetParameterDescriptionByName( IntPtr eventdescription, byte[] name, out Studio.PARAMETER_DESCRIPTION parameter )
        {
            s_Studio_EventDescription_GetParameterDescriptionByName_1 ??= LoadStudioFunction<Studio_EventDescription_GetParameterDescriptionByName_Delegate_1>("FMOD_Studio_EventDescription_GetParameterDescriptionByName");
            return s_Studio_EventDescription_GetParameterDescriptionByName_1(eventdescription, name, out parameter);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_EventDescription_GetParameterDescriptionByID( IntPtr eventdescription, Studio.PARAMETER_ID id, out Studio.PARAMETER_DESCRIPTION parameter )
        {
            s_Studio_EventDescription_GetParameterDescriptionByID_1 ??= LoadStudioFunction<Studio_EventDescription_GetParameterDescriptionByID_Delegate_1>("FMOD_Studio_EventDescription_GetParameterDescriptionByID");
            return s_Studio_EventDescription_GetParameterDescriptionByID_1(eventdescription, id, out parameter);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_EventDescription_GetParameterLabelByIndex( IntPtr eventdescription, int index, int labelindex, IntPtr label, int size, out int retrieved )
        {
            s_Studio_EventDescription_GetParameterLabelByIndex_1 ??= LoadStudioFunction<Studio_EventDescription_GetParameterLabelByIndex_Delegate_1>("FMOD_Studio_EventDescription_GetParameterLabelByIndex");
            return s_Studio_EventDescription_GetParameterLabelByIndex_1(eventdescription, index, labelindex, label, size, out retrieved);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_EventDescription_GetParameterLabelByName( IntPtr eventdescription, byte[] name, int labelindex, IntPtr label, int size, out int retrieved )
        {
            s_Studio_EventDescription_GetParameterLabelByName_1 ??= LoadStudioFunction<Studio_EventDescription_GetParameterLabelByName_Delegate_1>("FMOD_Studio_EventDescription_GetParameterLabelByName");
            return s_Studio_EventDescription_GetParameterLabelByName_1(eventdescription, name, labelindex, label, size, out retrieved);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_EventDescription_GetParameterLabelByID( IntPtr eventdescription, Studio.PARAMETER_ID id, int labelindex, IntPtr label, int size, out int retrieved )
        {
            s_Studio_EventDescription_GetParameterLabelByID_1 ??= LoadStudioFunction<Studio_EventDescription_GetParameterLabelByID_Delegate_1>("FMOD_Studio_EventDescription_GetParameterLabelByID");
            return s_Studio_EventDescription_GetParameterLabelByID_1(eventdescription, id, labelindex, label, size, out retrieved);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_EventDescription_GetUserPropertyCount( IntPtr eventdescription, out int count )
        {
            s_Studio_EventDescription_GetUserPropertyCount_1 ??= LoadStudioFunction<Studio_EventDescription_GetUserPropertyCount_Delegate_1>("FMOD_Studio_EventDescription_GetUserPropertyCount");
            return s_Studio_EventDescription_GetUserPropertyCount_1(eventdescription, out count);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_EventDescription_GetUserPropertyByIndex( IntPtr eventdescription, int index, out Studio.USER_PROPERTY property )
        {
            s_Studio_EventDescription_GetUserPropertyByIndex_1 ??= LoadStudioFunction<Studio_EventDescription_GetUserPropertyByIndex_Delegate_1>("FMOD_Studio_EventDescription_GetUserPropertyByIndex");
            return s_Studio_EventDescription_GetUserPropertyByIndex_1(eventdescription, index, out property);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_EventDescription_GetUserProperty( IntPtr eventdescription, byte[] name, out Studio.USER_PROPERTY property )
        {
            s_Studio_EventDescription_GetUserProperty_1 ??= LoadStudioFunction<Studio_EventDescription_GetUserProperty_Delegate_1>("FMOD_Studio_EventDescription_GetUserProperty");
            return s_Studio_EventDescription_GetUserProperty_1(eventdescription, name, out property);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_EventDescription_GetLength( IntPtr eventdescription, out int length )
        {
            s_Studio_EventDescription_GetLength_1 ??= LoadStudioFunction<Studio_EventDescription_GetLength_Delegate_1>("FMOD_Studio_EventDescription_GetLength");
            return s_Studio_EventDescription_GetLength_1(eventdescription, out length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_EventDescription_GetMinMaxDistance( IntPtr eventdescription, out float min, out float max )
        {
            s_Studio_EventDescription_GetMinMaxDistance_1 ??= LoadStudioFunction<Studio_EventDescription_GetMinMaxDistance_Delegate_1>("FMOD_Studio_EventDescription_GetMinMaxDistance");
            return s_Studio_EventDescription_GetMinMaxDistance_1(eventdescription, out min, out max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_EventDescription_GetSoundSize( IntPtr eventdescription, out float size )
        {
            s_Studio_EventDescription_GetSoundSize_1 ??= LoadStudioFunction<Studio_EventDescription_GetSoundSize_Delegate_1>("FMOD_Studio_EventDescription_GetSoundSize");
            return s_Studio_EventDescription_GetSoundSize_1(eventdescription, out size);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_EventDescription_IsSnapshot( IntPtr eventdescription, out bool snapshot )
        {
            s_Studio_EventDescription_IsSnapshot_1 ??= LoadStudioFunction<Studio_EventDescription_IsSnapshot_Delegate_1>("FMOD_Studio_EventDescription_IsSnapshot");
            return s_Studio_EventDescription_IsSnapshot_1(eventdescription, out snapshot);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_EventDescription_IsOneshot( IntPtr eventdescription, out bool oneshot )
        {
            s_Studio_EventDescription_IsOneshot_1 ??= LoadStudioFunction<Studio_EventDescription_IsOneshot_Delegate_1>("FMOD_Studio_EventDescription_IsOneshot");
            return s_Studio_EventDescription_IsOneshot_1(eventdescription, out oneshot);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_EventDescription_IsStream( IntPtr eventdescription, out bool isStream )
        {
            s_Studio_EventDescription_IsStream_1 ??= LoadStudioFunction<Studio_EventDescription_IsStream_Delegate_1>("FMOD_Studio_EventDescription_IsStream");
            return s_Studio_EventDescription_IsStream_1(eventdescription, out isStream);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_EventDescription_Is3D( IntPtr eventdescription, out bool is3D )
        {
            s_Studio_EventDescription_Is3D_1 ??= LoadStudioFunction<Studio_EventDescription_Is3D_Delegate_1>("FMOD_Studio_EventDescription_Is3D");
            return s_Studio_EventDescription_Is3D_1(eventdescription, out is3D);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_EventDescription_IsDopplerEnabled( IntPtr eventdescription, out bool doppler )
        {
            s_Studio_EventDescription_IsDopplerEnabled_1 ??= LoadStudioFunction<Studio_EventDescription_IsDopplerEnabled_Delegate_1>("FMOD_Studio_EventDescription_IsDopplerEnabled");
            return s_Studio_EventDescription_IsDopplerEnabled_1(eventdescription, out doppler);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_EventDescription_HasSustainPoint( IntPtr eventdescription, out bool sustainPoint )
        {
            s_Studio_EventDescription_HasSustainPoint_1 ??= LoadStudioFunction<Studio_EventDescription_HasSustainPoint_Delegate_1>("FMOD_Studio_EventDescription_HasSustainPoint");
            return s_Studio_EventDescription_HasSustainPoint_1(eventdescription, out sustainPoint);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_EventDescription_CreateInstance( IntPtr eventdescription, out IntPtr instance )
        {
            s_Studio_EventDescription_CreateInstance_1 ??= LoadStudioFunction<Studio_EventDescription_CreateInstance_Delegate_1>("FMOD_Studio_EventDescription_CreateInstance");
            return s_Studio_EventDescription_CreateInstance_1(eventdescription, out instance);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_EventDescription_GetInstanceCount( IntPtr eventdescription, out int count )
        {
            s_Studio_EventDescription_GetInstanceCount_1 ??= LoadStudioFunction<Studio_EventDescription_GetInstanceCount_Delegate_1>("FMOD_Studio_EventDescription_GetInstanceCount");
            return s_Studio_EventDescription_GetInstanceCount_1(eventdescription, out count);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_EventDescription_GetInstanceList( IntPtr eventdescription, IntPtr[] array, int capacity, out int count )
        {
            s_Studio_EventDescription_GetInstanceList_1 ??= LoadStudioFunction<Studio_EventDescription_GetInstanceList_Delegate_1>("FMOD_Studio_EventDescription_GetInstanceList");
            return s_Studio_EventDescription_GetInstanceList_1(eventdescription, array, capacity, out count);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_EventDescription_LoadSampleData( IntPtr eventdescription )
        {
            s_Studio_EventDescription_LoadSampleData_1 ??= LoadStudioFunction<Studio_EventDescription_LoadSampleData_Delegate_1>("FMOD_Studio_EventDescription_LoadSampleData");
            return s_Studio_EventDescription_LoadSampleData_1(eventdescription);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_EventDescription_UnloadSampleData( IntPtr eventdescription )
        {
            s_Studio_EventDescription_UnloadSampleData_1 ??= LoadStudioFunction<Studio_EventDescription_UnloadSampleData_Delegate_1>("FMOD_Studio_EventDescription_UnloadSampleData");
            return s_Studio_EventDescription_UnloadSampleData_1(eventdescription);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_EventDescription_GetSampleLoadingState( IntPtr eventdescription, out Studio.LOADING_STATE state )
        {
            s_Studio_EventDescription_GetSampleLoadingState_1 ??= LoadStudioFunction<Studio_EventDescription_GetSampleLoadingState_Delegate_1>("FMOD_Studio_EventDescription_GetSampleLoadingState");
            return s_Studio_EventDescription_GetSampleLoadingState_1(eventdescription, out state);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_EventDescription_ReleaseAllInstances( IntPtr eventdescription )
        {
            s_Studio_EventDescription_ReleaseAllInstances_1 ??= LoadStudioFunction<Studio_EventDescription_ReleaseAllInstances_Delegate_1>("FMOD_Studio_EventDescription_ReleaseAllInstances");
            return s_Studio_EventDescription_ReleaseAllInstances_1(eventdescription);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_EventDescription_SetCallback( IntPtr eventdescription, Studio.EVENT_CALLBACK callback, Studio.EVENT_CALLBACK_TYPE callbackmask )
        {
            s_Studio_EventDescription_SetCallback_1 ??= LoadStudioFunction<Studio_EventDescription_SetCallback_Delegate_1>("FMOD_Studio_EventDescription_SetCallback");
            return s_Studio_EventDescription_SetCallback_1(eventdescription, callback, callbackmask);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_EventDescription_GetUserData( IntPtr eventdescription, out IntPtr userdata )
        {
            s_Studio_EventDescription_GetUserData_1 ??= LoadStudioFunction<Studio_EventDescription_GetUserData_Delegate_1>("FMOD_Studio_EventDescription_GetUserData");
            return s_Studio_EventDescription_GetUserData_1(eventdescription, out userdata);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_EventDescription_SetUserData( IntPtr eventdescription, IntPtr userdata )
        {
            s_Studio_EventDescription_SetUserData_1 ??= LoadStudioFunction<Studio_EventDescription_SetUserData_Delegate_1>("FMOD_Studio_EventDescription_SetUserData");
            return s_Studio_EventDescription_SetUserData_1(eventdescription, userdata);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Studio_EventInstance_IsValid( IntPtr _event )
        {
            s_Studio_EventInstance_IsValid_1 ??= LoadStudioFunction<Studio_EventInstance_IsValid_Delegate_1>("FMOD_Studio_EventInstance_IsValid");
            return s_Studio_EventInstance_IsValid_1(_event);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_EventInstance_GetDescription( IntPtr _event, out IntPtr description )
        {
            s_Studio_EventInstance_GetDescription_1 ??= LoadStudioFunction<Studio_EventInstance_GetDescription_Delegate_1>("FMOD_Studio_EventInstance_GetDescription");
            return s_Studio_EventInstance_GetDescription_1(_event, out description);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_EventInstance_GetSystem( IntPtr _event, out IntPtr system )
        {
            s_Studio_EventInstance_GetSystem_1 ??= LoadStudioFunction<Studio_EventInstance_GetSystem_Delegate_1>("FMOD_Studio_EventInstance_GetSystem");
            return s_Studio_EventInstance_GetSystem_1(_event, out system);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_EventInstance_GetVolume( IntPtr _event, out float volume, IntPtr zero )
        {
            s_Studio_EventInstance_GetVolume_1 ??= LoadStudioFunction<Studio_EventInstance_GetVolume_Delegate_1>("FMOD_Studio_EventInstance_GetVolume");
            return s_Studio_EventInstance_GetVolume_1(_event, out volume, zero);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_EventInstance_GetVolume( IntPtr _event, out float volume, out float finalvolume )
        {
            s_Studio_EventInstance_GetVolume_2 ??= LoadStudioFunction<Studio_EventInstance_GetVolume_Delegate_2>("FMOD_Studio_EventInstance_GetVolume");
            return s_Studio_EventInstance_GetVolume_2(_event, out volume, out finalvolume);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_EventInstance_SetVolume( IntPtr _event, float volume )
        {
            s_Studio_EventInstance_SetVolume_1 ??= LoadStudioFunction<Studio_EventInstance_SetVolume_Delegate_1>("FMOD_Studio_EventInstance_SetVolume");
            return s_Studio_EventInstance_SetVolume_1(_event, volume);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_EventInstance_GetPitch( IntPtr _event, out float pitch, IntPtr zero )
        {
            s_Studio_EventInstance_GetPitch_1 ??= LoadStudioFunction<Studio_EventInstance_GetPitch_Delegate_1>("FMOD_Studio_EventInstance_GetPitch");
            return s_Studio_EventInstance_GetPitch_1(_event, out pitch, zero);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_EventInstance_GetPitch( IntPtr _event, out float pitch, out float finalpitch )
        {
            s_Studio_EventInstance_GetPitch_2 ??= LoadStudioFunction<Studio_EventInstance_GetPitch_Delegate_2>("FMOD_Studio_EventInstance_GetPitch");
            return s_Studio_EventInstance_GetPitch_2(_event, out pitch, out finalpitch);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_EventInstance_SetPitch( IntPtr _event, float pitch )
        {
            s_Studio_EventInstance_SetPitch_1 ??= LoadStudioFunction<Studio_EventInstance_SetPitch_Delegate_1>("FMOD_Studio_EventInstance_SetPitch");
            return s_Studio_EventInstance_SetPitch_1(_event, pitch);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_EventInstance_Get3DAttributes( IntPtr _event, out ATTRIBUTES_3D attributes )
        {
            s_Studio_EventInstance_Get3DAttributes_1 ??= LoadStudioFunction<Studio_EventInstance_Get3DAttributes_Delegate_1>("FMOD_Studio_EventInstance_Get3DAttributes");
            return s_Studio_EventInstance_Get3DAttributes_1(_event, out attributes);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_EventInstance_Set3DAttributes( IntPtr _event, ref ATTRIBUTES_3D attributes )
        {
            s_Studio_EventInstance_Set3DAttributes_1 ??= LoadStudioFunction<Studio_EventInstance_Set3DAttributes_Delegate_1>("FMOD_Studio_EventInstance_Set3DAttributes");
            return s_Studio_EventInstance_Set3DAttributes_1(_event, ref attributes);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_EventInstance_GetListenerMask( IntPtr _event, out uint mask )
        {
            s_Studio_EventInstance_GetListenerMask_1 ??= LoadStudioFunction<Studio_EventInstance_GetListenerMask_Delegate_1>("FMOD_Studio_EventInstance_GetListenerMask");
            return s_Studio_EventInstance_GetListenerMask_1(_event, out mask);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_EventInstance_SetListenerMask( IntPtr _event, uint mask )
        {
            s_Studio_EventInstance_SetListenerMask_1 ??= LoadStudioFunction<Studio_EventInstance_SetListenerMask_Delegate_1>("FMOD_Studio_EventInstance_SetListenerMask");
            return s_Studio_EventInstance_SetListenerMask_1(_event, mask);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_EventInstance_GetProperty( IntPtr _event, Studio.EVENT_PROPERTY index, out float value )
        {
            s_Studio_EventInstance_GetProperty_1 ??= LoadStudioFunction<Studio_EventInstance_GetProperty_Delegate_1>("FMOD_Studio_EventInstance_GetProperty");
            return s_Studio_EventInstance_GetProperty_1(_event, index, out value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_EventInstance_SetProperty( IntPtr _event, Studio.EVENT_PROPERTY index, float value )
        {
            s_Studio_EventInstance_SetProperty_1 ??= LoadStudioFunction<Studio_EventInstance_SetProperty_Delegate_1>("FMOD_Studio_EventInstance_SetProperty");
            return s_Studio_EventInstance_SetProperty_1(_event, index, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_EventInstance_GetReverbLevel( IntPtr _event, int index, out float level )
        {
            s_Studio_EventInstance_GetReverbLevel_1 ??= LoadStudioFunction<Studio_EventInstance_GetReverbLevel_Delegate_1>("FMOD_Studio_EventInstance_GetReverbLevel");
            return s_Studio_EventInstance_GetReverbLevel_1(_event, index, out level);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_EventInstance_SetReverbLevel( IntPtr _event, int index, float level )
        {
            s_Studio_EventInstance_SetReverbLevel_1 ??= LoadStudioFunction<Studio_EventInstance_SetReverbLevel_Delegate_1>("FMOD_Studio_EventInstance_SetReverbLevel");
            return s_Studio_EventInstance_SetReverbLevel_1(_event, index, level);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_EventInstance_GetPaused( IntPtr _event, out bool paused )
        {
            s_Studio_EventInstance_GetPaused_1 ??= LoadStudioFunction<Studio_EventInstance_GetPaused_Delegate_1>("FMOD_Studio_EventInstance_GetPaused");
            return s_Studio_EventInstance_GetPaused_1(_event, out paused);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_EventInstance_SetPaused( IntPtr _event, bool paused )
        {
            s_Studio_EventInstance_SetPaused_1 ??= LoadStudioFunction<Studio_EventInstance_SetPaused_Delegate_1>("FMOD_Studio_EventInstance_SetPaused");
            return s_Studio_EventInstance_SetPaused_1(_event, paused);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_EventInstance_Start( IntPtr _event )
        {
            s_Studio_EventInstance_Start_1 ??= LoadStudioFunction<Studio_EventInstance_Start_Delegate_1>("FMOD_Studio_EventInstance_Start");
            return s_Studio_EventInstance_Start_1(_event);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_EventInstance_Stop( IntPtr _event, Studio.STOP_MODE mode )
        {
            s_Studio_EventInstance_Stop_1 ??= LoadStudioFunction<Studio_EventInstance_Stop_Delegate_1>("FMOD_Studio_EventInstance_Stop");
            return s_Studio_EventInstance_Stop_1(_event, mode);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_EventInstance_GetTimelinePosition( IntPtr _event, out int position )
        {
            s_Studio_EventInstance_GetTimelinePosition_1 ??= LoadStudioFunction<Studio_EventInstance_GetTimelinePosition_Delegate_1>("FMOD_Studio_EventInstance_GetTimelinePosition");
            return s_Studio_EventInstance_GetTimelinePosition_1(_event, out position);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_EventInstance_SetTimelinePosition( IntPtr _event, int position )
        {
            s_Studio_EventInstance_SetTimelinePosition_1 ??= LoadStudioFunction<Studio_EventInstance_SetTimelinePosition_Delegate_1>("FMOD_Studio_EventInstance_SetTimelinePosition");
            return s_Studio_EventInstance_SetTimelinePosition_1(_event, position);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_EventInstance_GetPlaybackState( IntPtr _event, out Studio.PLAYBACK_STATE state )
        {
            s_Studio_EventInstance_GetPlaybackState_1 ??= LoadStudioFunction<Studio_EventInstance_GetPlaybackState_Delegate_1>("FMOD_Studio_EventInstance_GetPlaybackState");
            return s_Studio_EventInstance_GetPlaybackState_1(_event, out state);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_EventInstance_GetChannelGroup( IntPtr _event, out IntPtr group )
        {
            s_Studio_EventInstance_GetChannelGroup_1 ??= LoadStudioFunction<Studio_EventInstance_GetChannelGroup_Delegate_1>("FMOD_Studio_EventInstance_GetChannelGroup");
            return s_Studio_EventInstance_GetChannelGroup_1(_event, out group);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_EventInstance_GetMinMaxDistance( IntPtr _event, out float min, out float max )
        {
            s_Studio_EventInstance_GetMinMaxDistance_1 ??= LoadStudioFunction<Studio_EventInstance_GetMinMaxDistance_Delegate_1>("FMOD_Studio_EventInstance_GetMinMaxDistance");
            return s_Studio_EventInstance_GetMinMaxDistance_1(_event, out min, out max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_EventInstance_Release( IntPtr _event )
        {
            s_Studio_EventInstance_Release_1 ??= LoadStudioFunction<Studio_EventInstance_Release_Delegate_1>("FMOD_Studio_EventInstance_Release");
            return s_Studio_EventInstance_Release_1(_event);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_EventInstance_IsVirtual( IntPtr _event, out bool virtualstate )
        {
            s_Studio_EventInstance_IsVirtual_1 ??= LoadStudioFunction<Studio_EventInstance_IsVirtual_Delegate_1>("FMOD_Studio_EventInstance_IsVirtual");
            return s_Studio_EventInstance_IsVirtual_1(_event, out virtualstate);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_EventInstance_GetParameterByName( IntPtr _event, byte[] name, out float value, out float finalvalue )
        {
            s_Studio_EventInstance_GetParameterByName_1 ??= LoadStudioFunction<Studio_EventInstance_GetParameterByName_Delegate_1>("FMOD_Studio_EventInstance_GetParameterByName");
            return s_Studio_EventInstance_GetParameterByName_1(_event, name, out value, out finalvalue);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_EventInstance_SetParameterByName( IntPtr _event, byte[] name, float value, bool ignoreseekspeed )
        {
            s_Studio_EventInstance_SetParameterByName_1 ??= LoadStudioFunction<Studio_EventInstance_SetParameterByName_Delegate_1>("FMOD_Studio_EventInstance_SetParameterByName");
            return s_Studio_EventInstance_SetParameterByName_1(_event, name, value, ignoreseekspeed);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_EventInstance_SetParameterByNameWithLabel( IntPtr _event, byte[] name, byte[] label, bool ignoreseekspeed )
        {
            s_Studio_EventInstance_SetParameterByNameWithLabel_1 ??= LoadStudioFunction<Studio_EventInstance_SetParameterByNameWithLabel_Delegate_1>("FMOD_Studio_EventInstance_SetParameterByNameWithLabel");
            return s_Studio_EventInstance_SetParameterByNameWithLabel_1(_event, name, label, ignoreseekspeed);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_EventInstance_GetParameterByID( IntPtr _event, Studio.PARAMETER_ID id, out float value, out float finalvalue )
        {
            s_Studio_EventInstance_GetParameterByID_1 ??= LoadStudioFunction<Studio_EventInstance_GetParameterByID_Delegate_1>("FMOD_Studio_EventInstance_GetParameterByID");
            return s_Studio_EventInstance_GetParameterByID_1(_event, id, out value, out finalvalue);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_EventInstance_SetParameterByID( IntPtr _event, Studio.PARAMETER_ID id, float value, bool ignoreseekspeed )
        {
            s_Studio_EventInstance_SetParameterByID_1 ??= LoadStudioFunction<Studio_EventInstance_SetParameterByID_Delegate_1>("FMOD_Studio_EventInstance_SetParameterByID");
            return s_Studio_EventInstance_SetParameterByID_1(_event, id, value, ignoreseekspeed);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_EventInstance_SetParameterByIDWithLabel( IntPtr _event, Studio.PARAMETER_ID id, byte[] label, bool ignoreseekspeed )
        {
            s_Studio_EventInstance_SetParameterByIDWithLabel_1 ??= LoadStudioFunction<Studio_EventInstance_SetParameterByIDWithLabel_Delegate_1>("FMOD_Studio_EventInstance_SetParameterByIDWithLabel");
            return s_Studio_EventInstance_SetParameterByIDWithLabel_1(_event, id, label, ignoreseekspeed);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_EventInstance_SetParametersByIDs( IntPtr _event, Studio.PARAMETER_ID[] ids, float[] values, int count, bool ignoreseekspeed )
        {
            s_Studio_EventInstance_SetParametersByIDs_1 ??= LoadStudioFunction<Studio_EventInstance_SetParametersByIDs_Delegate_1>("FMOD_Studio_EventInstance_SetParametersByIDs");
            return s_Studio_EventInstance_SetParametersByIDs_1(_event, ids, values, count, ignoreseekspeed);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_EventInstance_KeyOff( IntPtr _event )
        {
            s_Studio_EventInstance_KeyOff_1 ??= LoadStudioFunction<Studio_EventInstance_KeyOff_Delegate_1>("FMOD_Studio_EventInstance_KeyOff");
            return s_Studio_EventInstance_KeyOff_1(_event);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_EventInstance_SetCallback( IntPtr _event, Studio.EVENT_CALLBACK callback, Studio.EVENT_CALLBACK_TYPE callbackmask )
        {
            s_Studio_EventInstance_SetCallback_1 ??= LoadStudioFunction<Studio_EventInstance_SetCallback_Delegate_1>("FMOD_Studio_EventInstance_SetCallback");
            return s_Studio_EventInstance_SetCallback_1(_event, callback, callbackmask);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_EventInstance_GetUserData( IntPtr _event, out IntPtr userdata )
        {
            s_Studio_EventInstance_GetUserData_1 ??= LoadStudioFunction<Studio_EventInstance_GetUserData_Delegate_1>("FMOD_Studio_EventInstance_GetUserData");
            return s_Studio_EventInstance_GetUserData_1(_event, out userdata);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_EventInstance_SetUserData( IntPtr _event, IntPtr userdata )
        {
            s_Studio_EventInstance_SetUserData_1 ??= LoadStudioFunction<Studio_EventInstance_SetUserData_Delegate_1>("FMOD_Studio_EventInstance_SetUserData");
            return s_Studio_EventInstance_SetUserData_1(_event, userdata);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_EventInstance_GetCPUUsage( IntPtr _event, out uint exclusive, out uint inclusive )
        {
            s_Studio_EventInstance_GetCPUUsage_1 ??= LoadStudioFunction<Studio_EventInstance_GetCPUUsage_Delegate_1>("FMOD_Studio_EventInstance_GetCPUUsage");
            return s_Studio_EventInstance_GetCPUUsage_1(_event, out exclusive, out inclusive);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_EventInstance_GetMemoryUsage( IntPtr _event, out Studio.MEMORY_USAGE memoryusage )
        {
            s_Studio_EventInstance_GetMemoryUsage_1 ??= LoadStudioFunction<Studio_EventInstance_GetMemoryUsage_Delegate_1>("FMOD_Studio_EventInstance_GetMemoryUsage");
            return s_Studio_EventInstance_GetMemoryUsage_1(_event, out memoryusage);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Studio_Bus_IsValid( IntPtr bus )
        {
            s_Studio_Bus_IsValid_1 ??= LoadStudioFunction<Studio_Bus_IsValid_Delegate_1>("FMOD_Studio_Bus_IsValid");
            return s_Studio_Bus_IsValid_1(bus);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_Bus_GetID( IntPtr bus, out GUID id )
        {
            s_Studio_Bus_GetID_1 ??= LoadStudioFunction<Studio_Bus_GetID_Delegate_1>("FMOD_Studio_Bus_GetID");
            return s_Studio_Bus_GetID_1(bus, out id);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_Bus_GetPath( IntPtr bus, IntPtr path, int size, out int retrieved )
        {
            s_Studio_Bus_GetPath_1 ??= LoadStudioFunction<Studio_Bus_GetPath_Delegate_1>("FMOD_Studio_Bus_GetPath");
            return s_Studio_Bus_GetPath_1(bus, path, size, out retrieved);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_Bus_GetVolume( IntPtr bus, out float volume, out float finalvolume )
        {
            s_Studio_Bus_GetVolume_1 ??= LoadStudioFunction<Studio_Bus_GetVolume_Delegate_1>("FMOD_Studio_Bus_GetVolume");
            return s_Studio_Bus_GetVolume_1(bus, out volume, out finalvolume);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_Bus_SetVolume( IntPtr bus, float volume )
        {
            s_Studio_Bus_SetVolume_1 ??= LoadStudioFunction<Studio_Bus_SetVolume_Delegate_1>("FMOD_Studio_Bus_SetVolume");
            return s_Studio_Bus_SetVolume_1(bus, volume);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_Bus_GetPaused( IntPtr bus, out bool paused )
        {
            s_Studio_Bus_GetPaused_1 ??= LoadStudioFunction<Studio_Bus_GetPaused_Delegate_1>("FMOD_Studio_Bus_GetPaused");
            return s_Studio_Bus_GetPaused_1(bus, out paused);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_Bus_SetPaused( IntPtr bus, bool paused )
        {
            s_Studio_Bus_SetPaused_1 ??= LoadStudioFunction<Studio_Bus_SetPaused_Delegate_1>("FMOD_Studio_Bus_SetPaused");
            return s_Studio_Bus_SetPaused_1(bus, paused);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_Bus_GetMute( IntPtr bus, out bool mute )
        {
            s_Studio_Bus_GetMute_1 ??= LoadStudioFunction<Studio_Bus_GetMute_Delegate_1>("FMOD_Studio_Bus_GetMute");
            return s_Studio_Bus_GetMute_1(bus, out mute);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_Bus_SetMute( IntPtr bus, bool mute )
        {
            s_Studio_Bus_SetMute_1 ??= LoadStudioFunction<Studio_Bus_SetMute_Delegate_1>("FMOD_Studio_Bus_SetMute");
            return s_Studio_Bus_SetMute_1(bus, mute);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_Bus_StopAllEvents( IntPtr bus, Studio.STOP_MODE mode )
        {
            s_Studio_Bus_StopAllEvents_1 ??= LoadStudioFunction<Studio_Bus_StopAllEvents_Delegate_1>("FMOD_Studio_Bus_StopAllEvents");
            return s_Studio_Bus_StopAllEvents_1(bus, mode);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_Bus_LockChannelGroup( IntPtr bus )
        {
            s_Studio_Bus_LockChannelGroup_1 ??= LoadStudioFunction<Studio_Bus_LockChannelGroup_Delegate_1>("FMOD_Studio_Bus_LockChannelGroup");
            return s_Studio_Bus_LockChannelGroup_1(bus);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_Bus_UnlockChannelGroup( IntPtr bus )
        {
            s_Studio_Bus_UnlockChannelGroup_1 ??= LoadStudioFunction<Studio_Bus_UnlockChannelGroup_Delegate_1>("FMOD_Studio_Bus_UnlockChannelGroup");
            return s_Studio_Bus_UnlockChannelGroup_1(bus);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_Bus_GetChannelGroup( IntPtr bus, out IntPtr group )
        {
            s_Studio_Bus_GetChannelGroup_1 ??= LoadStudioFunction<Studio_Bus_GetChannelGroup_Delegate_1>("FMOD_Studio_Bus_GetChannelGroup");
            return s_Studio_Bus_GetChannelGroup_1(bus, out group);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_Bus_GetCPUUsage( IntPtr bus, out uint exclusive, out uint inclusive )
        {
            s_Studio_Bus_GetCPUUsage_1 ??= LoadStudioFunction<Studio_Bus_GetCPUUsage_Delegate_1>("FMOD_Studio_Bus_GetCPUUsage");
            return s_Studio_Bus_GetCPUUsage_1(bus, out exclusive, out inclusive);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_Bus_GetMemoryUsage( IntPtr bus, out Studio.MEMORY_USAGE memoryusage )
        {
            s_Studio_Bus_GetMemoryUsage_1 ??= LoadStudioFunction<Studio_Bus_GetMemoryUsage_Delegate_1>("FMOD_Studio_Bus_GetMemoryUsage");
            return s_Studio_Bus_GetMemoryUsage_1(bus, out memoryusage);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_Bus_GetPortIndex( IntPtr bus, out ulong index )
        {
            s_Studio_Bus_GetPortIndex_1 ??= LoadStudioFunction<Studio_Bus_GetPortIndex_Delegate_1>("FMOD_Studio_Bus_GetPortIndex");
            return s_Studio_Bus_GetPortIndex_1(bus, out index);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_Bus_SetPortIndex( IntPtr bus, ulong index )
        {
            s_Studio_Bus_SetPortIndex_1 ??= LoadStudioFunction<Studio_Bus_SetPortIndex_Delegate_1>("FMOD_Studio_Bus_SetPortIndex");
            return s_Studio_Bus_SetPortIndex_1(bus, index);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Studio_VCA_IsValid( IntPtr vca )
        {
            s_Studio_VCA_IsValid_1 ??= LoadStudioFunction<Studio_VCA_IsValid_Delegate_1>("FMOD_Studio_VCA_IsValid");
            return s_Studio_VCA_IsValid_1(vca);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_VCA_GetID( IntPtr vca, out GUID id )
        {
            s_Studio_VCA_GetID_1 ??= LoadStudioFunction<Studio_VCA_GetID_Delegate_1>("FMOD_Studio_VCA_GetID");
            return s_Studio_VCA_GetID_1(vca, out id);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_VCA_GetPath( IntPtr vca, IntPtr path, int size, out int retrieved )
        {
            s_Studio_VCA_GetPath_1 ??= LoadStudioFunction<Studio_VCA_GetPath_Delegate_1>("FMOD_Studio_VCA_GetPath");
            return s_Studio_VCA_GetPath_1(vca, path, size, out retrieved);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_VCA_GetVolume( IntPtr vca, out float volume, out float finalvolume )
        {
            s_Studio_VCA_GetVolume_1 ??= LoadStudioFunction<Studio_VCA_GetVolume_Delegate_1>("FMOD_Studio_VCA_GetVolume");
            return s_Studio_VCA_GetVolume_1(vca, out volume, out finalvolume);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_VCA_SetVolume( IntPtr vca, float volume )
        {
            s_Studio_VCA_SetVolume_1 ??= LoadStudioFunction<Studio_VCA_SetVolume_Delegate_1>("FMOD_Studio_VCA_SetVolume");
            return s_Studio_VCA_SetVolume_1(vca, volume);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Studio_Bank_IsValid( IntPtr bank )
        {
            s_Studio_Bank_IsValid_1 ??= LoadStudioFunction<Studio_Bank_IsValid_Delegate_1>("FMOD_Studio_Bank_IsValid");
            return s_Studio_Bank_IsValid_1(bank);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_Bank_GetID( IntPtr bank, out GUID id )
        {
            s_Studio_Bank_GetID_1 ??= LoadStudioFunction<Studio_Bank_GetID_Delegate_1>("FMOD_Studio_Bank_GetID");
            return s_Studio_Bank_GetID_1(bank, out id);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_Bank_GetPath( IntPtr bank, IntPtr path, int size, out int retrieved )
        {
            s_Studio_Bank_GetPath_1 ??= LoadStudioFunction<Studio_Bank_GetPath_Delegate_1>("FMOD_Studio_Bank_GetPath");
            return s_Studio_Bank_GetPath_1(bank, path, size, out retrieved);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_Bank_Unload( IntPtr bank )
        {
            s_Studio_Bank_Unload_1 ??= LoadStudioFunction<Studio_Bank_Unload_Delegate_1>("FMOD_Studio_Bank_Unload");
            return s_Studio_Bank_Unload_1(bank);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_Bank_LoadSampleData( IntPtr bank )
        {
            s_Studio_Bank_LoadSampleData_1 ??= LoadStudioFunction<Studio_Bank_LoadSampleData_Delegate_1>("FMOD_Studio_Bank_LoadSampleData");
            return s_Studio_Bank_LoadSampleData_1(bank);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_Bank_UnloadSampleData( IntPtr bank )
        {
            s_Studio_Bank_UnloadSampleData_1 ??= LoadStudioFunction<Studio_Bank_UnloadSampleData_Delegate_1>("FMOD_Studio_Bank_UnloadSampleData");
            return s_Studio_Bank_UnloadSampleData_1(bank);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_Bank_GetLoadingState( IntPtr bank, out Studio.LOADING_STATE state )
        {
            s_Studio_Bank_GetLoadingState_1 ??= LoadStudioFunction<Studio_Bank_GetLoadingState_Delegate_1>("FMOD_Studio_Bank_GetLoadingState");
            return s_Studio_Bank_GetLoadingState_1(bank, out state);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_Bank_GetSampleLoadingState( IntPtr bank, out Studio.LOADING_STATE state )
        {
            s_Studio_Bank_GetSampleLoadingState_1 ??= LoadStudioFunction<Studio_Bank_GetSampleLoadingState_Delegate_1>("FMOD_Studio_Bank_GetSampleLoadingState");
            return s_Studio_Bank_GetSampleLoadingState_1(bank, out state);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_Bank_GetStringCount( IntPtr bank, out int count )
        {
            s_Studio_Bank_GetStringCount_1 ??= LoadStudioFunction<Studio_Bank_GetStringCount_Delegate_1>("FMOD_Studio_Bank_GetStringCount");
            return s_Studio_Bank_GetStringCount_1(bank, out count);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_Bank_GetStringInfo( IntPtr bank, int index, out GUID id, IntPtr path, int size, out int retrieved )
        {
            s_Studio_Bank_GetStringInfo_1 ??= LoadStudioFunction<Studio_Bank_GetStringInfo_Delegate_1>("FMOD_Studio_Bank_GetStringInfo");
            return s_Studio_Bank_GetStringInfo_1(bank, index, out id, path, size, out retrieved);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_Bank_GetEventCount( IntPtr bank, out int count )
        {
            s_Studio_Bank_GetEventCount_1 ??= LoadStudioFunction<Studio_Bank_GetEventCount_Delegate_1>("FMOD_Studio_Bank_GetEventCount");
            return s_Studio_Bank_GetEventCount_1(bank, out count);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_Bank_GetEventList( IntPtr bank, IntPtr[] array, int capacity, out int count )
        {
            s_Studio_Bank_GetEventList_1 ??= LoadStudioFunction<Studio_Bank_GetEventList_Delegate_1>("FMOD_Studio_Bank_GetEventList");
            return s_Studio_Bank_GetEventList_1(bank, array, capacity, out count);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_Bank_GetBusCount( IntPtr bank, out int count )
        {
            s_Studio_Bank_GetBusCount_1 ??= LoadStudioFunction<Studio_Bank_GetBusCount_Delegate_1>("FMOD_Studio_Bank_GetBusCount");
            return s_Studio_Bank_GetBusCount_1(bank, out count);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_Bank_GetBusList( IntPtr bank, IntPtr[] array, int capacity, out int count )
        {
            s_Studio_Bank_GetBusList_1 ??= LoadStudioFunction<Studio_Bank_GetBusList_Delegate_1>("FMOD_Studio_Bank_GetBusList");
            return s_Studio_Bank_GetBusList_1(bank, array, capacity, out count);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_Bank_GetVCACount( IntPtr bank, out int count )
        {
            s_Studio_Bank_GetVCACount_1 ??= LoadStudioFunction<Studio_Bank_GetVCACount_Delegate_1>("FMOD_Studio_Bank_GetVCACount");
            return s_Studio_Bank_GetVCACount_1(bank, out count);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_Bank_GetVCAList( IntPtr bank, IntPtr[] array, int capacity, out int count )
        {
            s_Studio_Bank_GetVCAList_1 ??= LoadStudioFunction<Studio_Bank_GetVCAList_Delegate_1>("FMOD_Studio_Bank_GetVCAList");
            return s_Studio_Bank_GetVCAList_1(bank, array, capacity, out count);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_Bank_GetUserData( IntPtr bank, out IntPtr userdata )
        {
            s_Studio_Bank_GetUserData_1 ??= LoadStudioFunction<Studio_Bank_GetUserData_Delegate_1>("FMOD_Studio_Bank_GetUserData");
            return s_Studio_Bank_GetUserData_1(bank, out userdata);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_Bank_SetUserData( IntPtr bank, IntPtr userdata )
        {
            s_Studio_Bank_SetUserData_1 ??= LoadStudioFunction<Studio_Bank_SetUserData_Delegate_1>("FMOD_Studio_Bank_SetUserData");
            return s_Studio_Bank_SetUserData_1(bank, userdata);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Studio_CommandReplay_IsValid( IntPtr replay )
        {
            s_Studio_CommandReplay_IsValid_1 ??= LoadStudioFunction<Studio_CommandReplay_IsValid_Delegate_1>("FMOD_Studio_CommandReplay_IsValid");
            return s_Studio_CommandReplay_IsValid_1(replay);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_CommandReplay_GetSystem( IntPtr replay, out IntPtr system )
        {
            s_Studio_CommandReplay_GetSystem_1 ??= LoadStudioFunction<Studio_CommandReplay_GetSystem_Delegate_1>("FMOD_Studio_CommandReplay_GetSystem");
            return s_Studio_CommandReplay_GetSystem_1(replay, out system);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_CommandReplay_GetLength( IntPtr replay, out float length )
        {
            s_Studio_CommandReplay_GetLength_1 ??= LoadStudioFunction<Studio_CommandReplay_GetLength_Delegate_1>("FMOD_Studio_CommandReplay_GetLength");
            return s_Studio_CommandReplay_GetLength_1(replay, out length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_CommandReplay_GetCommandCount( IntPtr replay, out int count )
        {
            s_Studio_CommandReplay_GetCommandCount_1 ??= LoadStudioFunction<Studio_CommandReplay_GetCommandCount_Delegate_1>("FMOD_Studio_CommandReplay_GetCommandCount");
            return s_Studio_CommandReplay_GetCommandCount_1(replay, out count);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_CommandReplay_GetCommandInfo( IntPtr replay, int commandindex, out Studio.COMMAND_INFO info )
        {
            s_Studio_CommandReplay_GetCommandInfo_1 ??= LoadStudioFunction<Studio_CommandReplay_GetCommandInfo_Delegate_1>("FMOD_Studio_CommandReplay_GetCommandInfo");
            return s_Studio_CommandReplay_GetCommandInfo_1(replay, commandindex, out info);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_CommandReplay_GetCommandString( IntPtr replay, int commandIndex, IntPtr buffer, int length )
        {
            s_Studio_CommandReplay_GetCommandString_1 ??= LoadStudioFunction<Studio_CommandReplay_GetCommandString_Delegate_1>("FMOD_Studio_CommandReplay_GetCommandString");
            return s_Studio_CommandReplay_GetCommandString_1(replay, commandIndex, buffer, length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_CommandReplay_GetCommandAtTime( IntPtr replay, float time, out int commandIndex )
        {
            s_Studio_CommandReplay_GetCommandAtTime_1 ??= LoadStudioFunction<Studio_CommandReplay_GetCommandAtTime_Delegate_1>("FMOD_Studio_CommandReplay_GetCommandAtTime");
            return s_Studio_CommandReplay_GetCommandAtTime_1(replay, time, out commandIndex);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_CommandReplay_SetBankPath( IntPtr replay, byte[] bankPath )
        {
            s_Studio_CommandReplay_SetBankPath_1 ??= LoadStudioFunction<Studio_CommandReplay_SetBankPath_Delegate_1>("FMOD_Studio_CommandReplay_SetBankPath");
            return s_Studio_CommandReplay_SetBankPath_1(replay, bankPath);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_CommandReplay_Start( IntPtr replay )
        {
            s_Studio_CommandReplay_Start_1 ??= LoadStudioFunction<Studio_CommandReplay_Start_Delegate_1>("FMOD_Studio_CommandReplay_Start");
            return s_Studio_CommandReplay_Start_1(replay);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_CommandReplay_Stop( IntPtr replay )
        {
            s_Studio_CommandReplay_Stop_1 ??= LoadStudioFunction<Studio_CommandReplay_Stop_Delegate_1>("FMOD_Studio_CommandReplay_Stop");
            return s_Studio_CommandReplay_Stop_1(replay);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_CommandReplay_SeekToTime( IntPtr replay, float time )
        {
            s_Studio_CommandReplay_SeekToTime_1 ??= LoadStudioFunction<Studio_CommandReplay_SeekToTime_Delegate_1>("FMOD_Studio_CommandReplay_SeekToTime");
            return s_Studio_CommandReplay_SeekToTime_1(replay, time);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_CommandReplay_SeekToCommand( IntPtr replay, int commandIndex )
        {
            s_Studio_CommandReplay_SeekToCommand_1 ??= LoadStudioFunction<Studio_CommandReplay_SeekToCommand_Delegate_1>("FMOD_Studio_CommandReplay_SeekToCommand");
            return s_Studio_CommandReplay_SeekToCommand_1(replay, commandIndex);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_CommandReplay_GetPaused( IntPtr replay, out bool paused )
        {
            s_Studio_CommandReplay_GetPaused_1 ??= LoadStudioFunction<Studio_CommandReplay_GetPaused_Delegate_1>("FMOD_Studio_CommandReplay_GetPaused");
            return s_Studio_CommandReplay_GetPaused_1(replay, out paused);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_CommandReplay_SetPaused( IntPtr replay, bool paused )
        {
            s_Studio_CommandReplay_SetPaused_1 ??= LoadStudioFunction<Studio_CommandReplay_SetPaused_Delegate_1>("FMOD_Studio_CommandReplay_SetPaused");
            return s_Studio_CommandReplay_SetPaused_1(replay, paused);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_CommandReplay_GetPlaybackState( IntPtr replay, out Studio.PLAYBACK_STATE state )
        {
            s_Studio_CommandReplay_GetPlaybackState_1 ??= LoadStudioFunction<Studio_CommandReplay_GetPlaybackState_Delegate_1>("FMOD_Studio_CommandReplay_GetPlaybackState");
            return s_Studio_CommandReplay_GetPlaybackState_1(replay, out state);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_CommandReplay_GetCurrentCommand( IntPtr replay, out int commandIndex, out float currentTime )
        {
            s_Studio_CommandReplay_GetCurrentCommand_1 ??= LoadStudioFunction<Studio_CommandReplay_GetCurrentCommand_Delegate_1>("FMOD_Studio_CommandReplay_GetCurrentCommand");
            return s_Studio_CommandReplay_GetCurrentCommand_1(replay, out commandIndex, out currentTime);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_CommandReplay_Release( IntPtr replay )
        {
            s_Studio_CommandReplay_Release_1 ??= LoadStudioFunction<Studio_CommandReplay_Release_Delegate_1>("FMOD_Studio_CommandReplay_Release");
            return s_Studio_CommandReplay_Release_1(replay);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_CommandReplay_SetFrameCallback( IntPtr replay, Studio.COMMANDREPLAY_FRAME_CALLBACK callback )
        {
            s_Studio_CommandReplay_SetFrameCallback_1 ??= LoadStudioFunction<Studio_CommandReplay_SetFrameCallback_Delegate_1>("FMOD_Studio_CommandReplay_SetFrameCallback");
            return s_Studio_CommandReplay_SetFrameCallback_1(replay, callback);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_CommandReplay_SetLoadBankCallback( IntPtr replay, Studio.COMMANDREPLAY_LOAD_BANK_CALLBACK callback )
        {
            s_Studio_CommandReplay_SetLoadBankCallback_1 ??= LoadStudioFunction<Studio_CommandReplay_SetLoadBankCallback_Delegate_1>("FMOD_Studio_CommandReplay_SetLoadBankCallback");
            return s_Studio_CommandReplay_SetLoadBankCallback_1(replay, callback);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_CommandReplay_SetCreateInstanceCallback( IntPtr replay, Studio.COMMANDREPLAY_CREATE_INSTANCE_CALLBACK callback )
        {
            s_Studio_CommandReplay_SetCreateInstanceCallback_1 ??= LoadStudioFunction<Studio_CommandReplay_SetCreateInstanceCallback_Delegate_1>("FMOD_Studio_CommandReplay_SetCreateInstanceCallback");
            return s_Studio_CommandReplay_SetCreateInstanceCallback_1(replay, callback);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_CommandReplay_GetUserData( IntPtr replay, out IntPtr userdata )
        {
            s_Studio_CommandReplay_GetUserData_1 ??= LoadStudioFunction<Studio_CommandReplay_GetUserData_Delegate_1>("FMOD_Studio_CommandReplay_GetUserData");
            return s_Studio_CommandReplay_GetUserData_1(replay, out userdata);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Studio_CommandReplay_SetUserData( IntPtr replay, IntPtr userdata )
        {
            s_Studio_CommandReplay_SetUserData_1 ??= LoadStudioFunction<Studio_CommandReplay_SetUserData_Delegate_1>("FMOD_Studio_CommandReplay_SetUserData");
            return s_Studio_CommandReplay_SetUserData_1(replay, userdata);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT System_Create(out IntPtr system, uint headerversion)
        {
            s_System_Create_1 ??= LoadCoreFunction<System_Create_Delegate_1>("FMOD5_System_Create");
            return s_System_Create_1(out system, headerversion);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Memory_Initialize(IntPtr poolmem, int poollen, MEMORY_ALLOC_CALLBACK useralloc, MEMORY_REALLOC_CALLBACK userrealloc, MEMORY_FREE_CALLBACK userfree, MEMORY_TYPE memtypeflags)
        {
            s_Memory_Initialize_1 ??= LoadCoreFunction<Memory_Initialize_Delegate_1>("FMOD5_Memory_Initialize");
            return s_Memory_Initialize_1(poolmem, poollen, useralloc, userrealloc, userfree, memtypeflags);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Memory_GetStats(out int currentalloced, out int maxalloced, bool blocking)
        {
            s_Memory_GetStats_1 ??= LoadCoreFunction<Memory_GetStats_Delegate_1>("FMOD5_Memory_GetStats");
            return s_Memory_GetStats_1(out currentalloced, out maxalloced, blocking);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Debug_Initialize(DEBUG_FLAGS flags, DEBUG_MODE mode, DEBUG_CALLBACK callback, byte[] filename)
        {
            s_Debug_Initialize_1 ??= LoadCoreFunction<Debug_Initialize_Delegate_1>("FMOD5_Debug_Initialize");
            return s_Debug_Initialize_1(flags, mode, callback, filename);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Thread_SetAttributes(THREAD_TYPE type, THREAD_AFFINITY affinity, THREAD_PRIORITY priority, THREAD_STACK_SIZE stacksize)
        {
            s_Thread_SetAttributes_1 ??= LoadCoreFunction<Thread_SetAttributes_Delegate_1>("FMOD5_Thread_SetAttributes");
            return s_Thread_SetAttributes_1(type, affinity, priority, stacksize);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT System_Release(IntPtr system)
        {
            s_System_Release_1 ??= LoadCoreFunction<System_Release_Delegate_1>("FMOD5_System_Release");
            return s_System_Release_1(system);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT System_SetOutput(IntPtr system, OUTPUTTYPE output)
        {
            s_System_SetOutput_1 ??= LoadCoreFunction<System_SetOutput_Delegate_1>("FMOD5_System_SetOutput");
            return s_System_SetOutput_1(system, output);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT System_GetOutput(IntPtr system, out OUTPUTTYPE output)
        {
            s_System_GetOutput_1 ??= LoadCoreFunction<System_GetOutput_Delegate_1>("FMOD5_System_GetOutput");
            return s_System_GetOutput_1(system, out output);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT System_GetNumDrivers(IntPtr system, out int numdrivers)
        {
            s_System_GetNumDrivers_1 ??= LoadCoreFunction<System_GetNumDrivers_Delegate_1>("FMOD5_System_GetNumDrivers");
            return s_System_GetNumDrivers_1(system, out numdrivers);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT System_GetDriverInfo(IntPtr system, int id, IntPtr name, int namelen, out Guid guid, out int systemrate, out SPEAKERMODE speakermode, out int speakermodechannels)
        {
            s_System_GetDriverInfo_1 ??= LoadCoreFunction<System_GetDriverInfo_Delegate_1>("FMOD5_System_GetDriverInfo");
            return s_System_GetDriverInfo_1(system, id, name, namelen, out guid, out systemrate, out speakermode, out speakermodechannels);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT System_SetDriver(IntPtr system, int driver)
        {
            s_System_SetDriver_1 ??= LoadCoreFunction<System_SetDriver_Delegate_1>("FMOD5_System_SetDriver");
            return s_System_SetDriver_1(system, driver);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT System_GetDriver(IntPtr system, out int driver)
        {
            s_System_GetDriver_1 ??= LoadCoreFunction<System_GetDriver_Delegate_1>("FMOD5_System_GetDriver");
            return s_System_GetDriver_1(system, out driver);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT System_SetSoftwareChannels(IntPtr system, int numsoftwarechannels)
        {
            s_System_SetSoftwareChannels_1 ??= LoadCoreFunction<System_SetSoftwareChannels_Delegate_1>("FMOD5_System_SetSoftwareChannels");
            return s_System_SetSoftwareChannels_1(system, numsoftwarechannels);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT System_GetSoftwareChannels(IntPtr system, out int numsoftwarechannels)
        {
            s_System_GetSoftwareChannels_1 ??= LoadCoreFunction<System_GetSoftwareChannels_Delegate_1>("FMOD5_System_GetSoftwareChannels");
            return s_System_GetSoftwareChannels_1(system, out numsoftwarechannels);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT System_SetSoftwareFormat(IntPtr system, int samplerate, SPEAKERMODE speakermode, int numrawspeakers)
        {
            s_System_SetSoftwareFormat_1 ??= LoadCoreFunction<System_SetSoftwareFormat_Delegate_1>("FMOD5_System_SetSoftwareFormat");
            return s_System_SetSoftwareFormat_1(system, samplerate, speakermode, numrawspeakers);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT System_GetSoftwareFormat(IntPtr system, out int samplerate, out SPEAKERMODE speakermode, out int numrawspeakers)
        {
            s_System_GetSoftwareFormat_1 ??= LoadCoreFunction<System_GetSoftwareFormat_Delegate_1>("FMOD5_System_GetSoftwareFormat");
            return s_System_GetSoftwareFormat_1(system, out samplerate, out speakermode, out numrawspeakers);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT System_SetDSPBufferSize(IntPtr system, uint bufferlength, int numbuffers)
        {
            s_System_SetDSPBufferSize_1 ??= LoadCoreFunction<System_SetDSPBufferSize_Delegate_1>("FMOD5_System_SetDSPBufferSize");
            return s_System_SetDSPBufferSize_1(system, bufferlength, numbuffers);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT System_GetDSPBufferSize(IntPtr system, out uint bufferlength, out int numbuffers)
        {
            s_System_GetDSPBufferSize_1 ??= LoadCoreFunction<System_GetDSPBufferSize_Delegate_1>("FMOD5_System_GetDSPBufferSize");
            return s_System_GetDSPBufferSize_1(system, out bufferlength, out numbuffers);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT System_SetFileSystem(IntPtr system, FILE_OPEN_CALLBACK useropen, FILE_CLOSE_CALLBACK userclose, FILE_READ_CALLBACK userread, FILE_SEEK_CALLBACK userseek, FILE_ASYNCREAD_CALLBACK userasyncread, FILE_ASYNCCANCEL_CALLBACK userasynccancel, int blockalign)
        {
            s_System_SetFileSystem_1 ??= LoadCoreFunction<System_SetFileSystem_Delegate_1>("FMOD5_System_SetFileSystem");
            return s_System_SetFileSystem_1(system, useropen, userclose, userread, userseek, userasyncread, userasynccancel, blockalign);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT System_AttachFileSystem(IntPtr system, FILE_OPEN_CALLBACK useropen, FILE_CLOSE_CALLBACK userclose, FILE_READ_CALLBACK userread, FILE_SEEK_CALLBACK userseek)
        {
            s_System_AttachFileSystem_1 ??= LoadCoreFunction<System_AttachFileSystem_Delegate_1>("FMOD5_System_AttachFileSystem");
            return s_System_AttachFileSystem_1(system, useropen, userclose, userread, userseek);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT System_SetAdvancedSettings(IntPtr system, ref ADVANCEDSETTINGS settings)
        {
            s_System_SetAdvancedSettings_1 ??= LoadCoreFunction<System_SetAdvancedSettings_Delegate_1>("FMOD5_System_SetAdvancedSettings");
            return s_System_SetAdvancedSettings_1(system, ref settings);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT System_GetAdvancedSettings(IntPtr system, ref ADVANCEDSETTINGS settings)
        {
            s_System_GetAdvancedSettings_1 ??= LoadCoreFunction<System_GetAdvancedSettings_Delegate_1>("FMOD5_System_GetAdvancedSettings");
            return s_System_GetAdvancedSettings_1(system, ref settings);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT System_SetCallback(IntPtr system, SYSTEM_CALLBACK callback, SYSTEM_CALLBACK_TYPE callbackmask)
        {
            s_System_SetCallback_1 ??= LoadCoreFunction<System_SetCallback_Delegate_1>("FMOD5_System_SetCallback");
            return s_System_SetCallback_1(system, callback, callbackmask);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT System_SetPluginPath(IntPtr system, byte[] path)
        {
            s_System_SetPluginPath_1 ??= LoadCoreFunction<System_SetPluginPath_Delegate_1>("FMOD5_System_SetPluginPath");
            return s_System_SetPluginPath_1(system, path);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT System_LoadPlugin(IntPtr system, byte[] filename, out uint handle, uint priority)
        {
            s_System_LoadPlugin_1 ??= LoadCoreFunction<System_LoadPlugin_Delegate_1>("FMOD5_System_LoadPlugin");
            return s_System_LoadPlugin_1(system, filename, out handle, priority);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT System_UnloadPlugin(IntPtr system, uint handle)
        {
            s_System_UnloadPlugin_1 ??= LoadCoreFunction<System_UnloadPlugin_Delegate_1>("FMOD5_System_UnloadPlugin");
            return s_System_UnloadPlugin_1(system, handle);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT System_GetNumNestedPlugins(IntPtr system, uint handle, out int count)
        {
            s_System_GetNumNestedPlugins_1 ??= LoadCoreFunction<System_GetNumNestedPlugins_Delegate_1>("FMOD5_System_GetNumNestedPlugins");
            return s_System_GetNumNestedPlugins_1(system, handle, out count);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT System_GetNestedPlugin(IntPtr system, uint handle, int index, out uint nestedhandle)
        {
            s_System_GetNestedPlugin_1 ??= LoadCoreFunction<System_GetNestedPlugin_Delegate_1>("FMOD5_System_GetNestedPlugin");
            return s_System_GetNestedPlugin_1(system, handle, index, out nestedhandle);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT System_GetNumPlugins(IntPtr system, PLUGINTYPE plugintype, out int numplugins)
        {
            s_System_GetNumPlugins_1 ??= LoadCoreFunction<System_GetNumPlugins_Delegate_1>("FMOD5_System_GetNumPlugins");
            return s_System_GetNumPlugins_1(system, plugintype, out numplugins);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT System_GetPluginHandle(IntPtr system, PLUGINTYPE plugintype, int index, out uint handle)
        {
            s_System_GetPluginHandle_1 ??= LoadCoreFunction<System_GetPluginHandle_Delegate_1>("FMOD5_System_GetPluginHandle");
            return s_System_GetPluginHandle_1(system, plugintype, index, out handle);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT System_GetPluginInfo(IntPtr system, uint handle, out PLUGINTYPE plugintype, IntPtr name, int namelen, out uint version)
        {
            s_System_GetPluginInfo_1 ??= LoadCoreFunction<System_GetPluginInfo_Delegate_1>("FMOD5_System_GetPluginInfo");
            return s_System_GetPluginInfo_1(system, handle, out plugintype, name, namelen, out version);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT System_SetOutputByPlugin(IntPtr system, uint handle)
        {
            s_System_SetOutputByPlugin_1 ??= LoadCoreFunction<System_SetOutputByPlugin_Delegate_1>("FMOD5_System_SetOutputByPlugin");
            return s_System_SetOutputByPlugin_1(system, handle);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT System_GetOutputByPlugin(IntPtr system, out uint handle)
        {
            s_System_GetOutputByPlugin_1 ??= LoadCoreFunction<System_GetOutputByPlugin_Delegate_1>("FMOD5_System_GetOutputByPlugin");
            return s_System_GetOutputByPlugin_1(system, out handle);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT System_CreateDSPByPlugin(IntPtr system, uint handle, out IntPtr dsp)
        {
            s_System_CreateDSPByPlugin_1 ??= LoadCoreFunction<System_CreateDSPByPlugin_Delegate_1>("FMOD5_System_CreateDSPByPlugin");
            return s_System_CreateDSPByPlugin_1(system, handle, out dsp);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT System_GetDSPInfoByPlugin(IntPtr system, uint handle, out IntPtr description)
        {
            s_System_GetDSPInfoByPlugin_1 ??= LoadCoreFunction<System_GetDSPInfoByPlugin_Delegate_1>("FMOD5_System_GetDSPInfoByPlugin");
            return s_System_GetDSPInfoByPlugin_1(system, handle, out description);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT System_RegisterDSP(IntPtr system, ref DSP_DESCRIPTION description, out uint handle)
        {
            s_System_RegisterDSP_1 ??= LoadCoreFunction<System_RegisterDSP_Delegate_1>("FMOD5_System_RegisterDSP");
            return s_System_RegisterDSP_1(system, ref description, out handle);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT System_Init(IntPtr system, int maxchannels, INITFLAGS flags, IntPtr extradriverdata)
        {
            s_System_Init_1 ??= LoadCoreFunction<System_Init_Delegate_1>("FMOD5_System_Init");
            return s_System_Init_1(system, maxchannels, flags, extradriverdata);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT System_Close(IntPtr system)
        {
            s_System_Close_1 ??= LoadCoreFunction<System_Close_Delegate_1>("FMOD5_System_Close");
            return s_System_Close_1(system);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT System_Update(IntPtr system)
        {
            s_System_Update_1 ??= LoadCoreFunction<System_Update_Delegate_1>("FMOD5_System_Update");
            return s_System_Update_1(system);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT System_SetSpeakerPosition(IntPtr system, SPEAKER speaker, float x, float y, bool active)
        {
            s_System_SetSpeakerPosition_1 ??= LoadCoreFunction<System_SetSpeakerPosition_Delegate_1>("FMOD5_System_SetSpeakerPosition");
            return s_System_SetSpeakerPosition_1(system, speaker, x, y, active);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT System_GetSpeakerPosition(IntPtr system, SPEAKER speaker, out float x, out float y, out bool active)
        {
            s_System_GetSpeakerPosition_1 ??= LoadCoreFunction<System_GetSpeakerPosition_Delegate_1>("FMOD5_System_GetSpeakerPosition");
            return s_System_GetSpeakerPosition_1(system, speaker, out x, out y, out active);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT System_SetStreamBufferSize(IntPtr system, uint filebuffersize, TIMEUNIT filebuffersizetype)
        {
            s_System_SetStreamBufferSize_1 ??= LoadCoreFunction<System_SetStreamBufferSize_Delegate_1>("FMOD5_System_SetStreamBufferSize");
            return s_System_SetStreamBufferSize_1(system, filebuffersize, filebuffersizetype);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT System_GetStreamBufferSize(IntPtr system, out uint filebuffersize, out TIMEUNIT filebuffersizetype)
        {
            s_System_GetStreamBufferSize_1 ??= LoadCoreFunction<System_GetStreamBufferSize_Delegate_1>("FMOD5_System_GetStreamBufferSize");
            return s_System_GetStreamBufferSize_1(system, out filebuffersize, out filebuffersizetype);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT System_Set3DSettings(IntPtr system, float dopplerscale, float distancefactor, float rolloffscale)
        {
            s_System_Set3DSettings_1 ??= LoadCoreFunction<System_Set3DSettings_Delegate_1>("FMOD5_System_Set3DSettings");
            return s_System_Set3DSettings_1(system, dopplerscale, distancefactor, rolloffscale);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT System_Get3DSettings(IntPtr system, out float dopplerscale, out float distancefactor, out float rolloffscale)
        {
            s_System_Get3DSettings_1 ??= LoadCoreFunction<System_Get3DSettings_Delegate_1>("FMOD5_System_Get3DSettings");
            return s_System_Get3DSettings_1(system, out dopplerscale, out distancefactor, out rolloffscale);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT System_Set3DNumListeners(IntPtr system, int numlisteners)
        {
            s_System_Set3DNumListeners_1 ??= LoadCoreFunction<System_Set3DNumListeners_Delegate_1>("FMOD5_System_Set3DNumListeners");
            return s_System_Set3DNumListeners_1(system, numlisteners);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT System_Get3DNumListeners(IntPtr system, out int numlisteners)
        {
            s_System_Get3DNumListeners_1 ??= LoadCoreFunction<System_Get3DNumListeners_Delegate_1>("FMOD5_System_Get3DNumListeners");
            return s_System_Get3DNumListeners_1(system, out numlisteners);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT System_Set3DListenerAttributes(IntPtr system, int listener, ref VECTOR pos, ref VECTOR vel, ref VECTOR forward, ref VECTOR up)
        {
            s_System_Set3DListenerAttributes_1 ??= LoadCoreFunction<System_Set3DListenerAttributes_Delegate_1>("FMOD5_System_Set3DListenerAttributes");
            return s_System_Set3DListenerAttributes_1(system, listener, ref pos, ref vel, ref forward, ref up);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT System_Get3DListenerAttributes(IntPtr system, int listener, out VECTOR pos, out VECTOR vel, out VECTOR forward, out VECTOR up)
        {
            s_System_Get3DListenerAttributes_1 ??= LoadCoreFunction<System_Get3DListenerAttributes_Delegate_1>("FMOD5_System_Get3DListenerAttributes");
            return s_System_Get3DListenerAttributes_1(system, listener, out pos, out vel, out forward, out up);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT System_Set3DRolloffCallback(IntPtr system, CB_3D_ROLLOFF_CALLBACK callback)
        {
            s_System_Set3DRolloffCallback_1 ??= LoadCoreFunction<System_Set3DRolloffCallback_Delegate_1>("FMOD5_System_Set3DRolloffCallback");
            return s_System_Set3DRolloffCallback_1(system, callback);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT System_MixerSuspend(IntPtr system)
        {
            s_System_MixerSuspend_1 ??= LoadCoreFunction<System_MixerSuspend_Delegate_1>("FMOD5_System_MixerSuspend");
            return s_System_MixerSuspend_1(system);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT System_MixerResume(IntPtr system)
        {
            s_System_MixerResume_1 ??= LoadCoreFunction<System_MixerResume_Delegate_1>("FMOD5_System_MixerResume");
            return s_System_MixerResume_1(system);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT System_GetDefaultMixMatrix(IntPtr system, SPEAKERMODE sourcespeakermode, SPEAKERMODE targetspeakermode, float[] matrix, int matrixhop)
        {
            s_System_GetDefaultMixMatrix_1 ??= LoadCoreFunction<System_GetDefaultMixMatrix_Delegate_1>("FMOD5_System_GetDefaultMixMatrix");
            return s_System_GetDefaultMixMatrix_1(system, sourcespeakermode, targetspeakermode, matrix, matrixhop);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT System_GetSpeakerModeChannels(IntPtr system, SPEAKERMODE mode, out int channels)
        {
            s_System_GetSpeakerModeChannels_1 ??= LoadCoreFunction<System_GetSpeakerModeChannels_Delegate_1>("FMOD5_System_GetSpeakerModeChannels");
            return s_System_GetSpeakerModeChannels_1(system, mode, out channels);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT System_GetVersion(IntPtr system, out uint version, out uint buildnumber)
        {
            s_System_GetVersion_1 ??= LoadCoreFunction<System_GetVersion_Delegate_1>("FMOD5_System_GetVersion");
            return s_System_GetVersion_1(system, out version, out buildnumber);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT System_GetOutputHandle(IntPtr system, out IntPtr handle)
        {
            s_System_GetOutputHandle_1 ??= LoadCoreFunction<System_GetOutputHandle_Delegate_1>("FMOD5_System_GetOutputHandle");
            return s_System_GetOutputHandle_1(system, out handle);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT System_GetChannelsPlaying(IntPtr system, out int channels, IntPtr zero)
        {
            s_System_GetChannelsPlaying_1 ??= LoadCoreFunction<System_GetChannelsPlaying_Delegate_1>("FMOD5_System_GetChannelsPlaying");
            return s_System_GetChannelsPlaying_1(system, out channels, zero);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT System_GetChannelsPlaying(IntPtr system, out int channels, out int realchannels)
        {
            s_System_GetChannelsPlaying_2 ??= LoadCoreFunction<System_GetChannelsPlaying_Delegate_2>("FMOD5_System_GetChannelsPlaying");
            return s_System_GetChannelsPlaying_2(system, out channels, out realchannels);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT System_GetCPUUsage(IntPtr system, out CPU_USAGE usage)
        {
            s_System_GetCPUUsage_1 ??= LoadCoreFunction<System_GetCPUUsage_Delegate_1>("FMOD5_System_GetCPUUsage");
            return s_System_GetCPUUsage_1(system, out usage);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT System_GetFileUsage(IntPtr system, out Int64 sampleBytesRead, out Int64 streamBytesRead, out Int64 otherBytesRead)
        {
            s_System_GetFileUsage_1 ??= LoadCoreFunction<System_GetFileUsage_Delegate_1>("FMOD5_System_GetFileUsage");
            return s_System_GetFileUsage_1(system, out sampleBytesRead, out streamBytesRead, out otherBytesRead);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT System_CreateSound(IntPtr system, byte[] name_or_data, MODE mode, ref CREATESOUNDEXINFO exinfo, out IntPtr sound)
        {
            s_System_CreateSound_1 ??= LoadCoreFunction<System_CreateSound_Delegate_1>("FMOD5_System_CreateSound");
            return s_System_CreateSound_1(system, name_or_data, mode, ref exinfo, out sound);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT System_CreateSound(IntPtr system, IntPtr name_or_data, MODE mode, ref CREATESOUNDEXINFO exinfo, out IntPtr sound)
        {
            s_System_CreateSound_2 ??= LoadCoreFunction<System_CreateSound_Delegate_2>("FMOD5_System_CreateSound");
            return s_System_CreateSound_2(system, name_or_data, mode, ref exinfo, out sound);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT System_CreateStream(IntPtr system, byte[] name_or_data, MODE mode, ref CREATESOUNDEXINFO exinfo, out IntPtr sound)
        {
            s_System_CreateStream_1 ??= LoadCoreFunction<System_CreateStream_Delegate_1>("FMOD5_System_CreateStream");
            return s_System_CreateStream_1(system, name_or_data, mode, ref exinfo, out sound);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT System_CreateStream(IntPtr system, IntPtr name_or_data, MODE mode, ref CREATESOUNDEXINFO exinfo, out IntPtr sound)
        {
            s_System_CreateStream_2 ??= LoadCoreFunction<System_CreateStream_Delegate_2>("FMOD5_System_CreateStream");
            return s_System_CreateStream_2(system, name_or_data, mode, ref exinfo, out sound);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT System_CreateDSP(IntPtr system, ref DSP_DESCRIPTION description, out IntPtr dsp)
        {
            s_System_CreateDSP_1 ??= LoadCoreFunction<System_CreateDSP_Delegate_1>("FMOD5_System_CreateDSP");
            return s_System_CreateDSP_1(system, ref description, out dsp);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT System_CreateDSPByType(IntPtr system, DSP_TYPE type, out IntPtr dsp)
        {
            s_System_CreateDSPByType_1 ??= LoadCoreFunction<System_CreateDSPByType_Delegate_1>("FMOD5_System_CreateDSPByType");
            return s_System_CreateDSPByType_1(system, type, out dsp);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT System_CreateDSPConnection(IntPtr system, DSPCONNECTION_TYPE type, out IntPtr connection)
        {
            s_System_CreateDSPConnection_1 ??= LoadCoreFunction<System_CreateDSPConnection_Delegate_1>("FMOD5_System_CreateDSPConnection");
            return s_System_CreateDSPConnection_1(system, type, out connection);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT System_CreateChannelGroup(IntPtr system, byte[] name, out IntPtr channelgroup)
        {
            s_System_CreateChannelGroup_1 ??= LoadCoreFunction<System_CreateChannelGroup_Delegate_1>("FMOD5_System_CreateChannelGroup");
            return s_System_CreateChannelGroup_1(system, name, out channelgroup);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT System_CreateSoundGroup(IntPtr system, byte[] name, out IntPtr soundgroup)
        {
            s_System_CreateSoundGroup_1 ??= LoadCoreFunction<System_CreateSoundGroup_Delegate_1>("FMOD5_System_CreateSoundGroup");
            return s_System_CreateSoundGroup_1(system, name, out soundgroup);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT System_CreateReverb3D(IntPtr system, out IntPtr reverb)
        {
            s_System_CreateReverb3D_1 ??= LoadCoreFunction<System_CreateReverb3D_Delegate_1>("FMOD5_System_CreateReverb3D");
            return s_System_CreateReverb3D_1(system, out reverb);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT System_PlaySound(IntPtr system, IntPtr sound, IntPtr channelgroup, bool paused, out IntPtr channel)
        {
            s_System_PlaySound_1 ??= LoadCoreFunction<System_PlaySound_Delegate_1>("FMOD5_System_PlaySound");
            return s_System_PlaySound_1(system, sound, channelgroup, paused, out channel);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT System_PlayDSP(IntPtr system, IntPtr dsp, IntPtr channelgroup, bool paused, out IntPtr channel)
        {
            s_System_PlayDSP_1 ??= LoadCoreFunction<System_PlayDSP_Delegate_1>("FMOD5_System_PlayDSP");
            return s_System_PlayDSP_1(system, dsp, channelgroup, paused, out channel);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT System_GetChannel(IntPtr system, int channelid, out IntPtr channel)
        {
            s_System_GetChannel_1 ??= LoadCoreFunction<System_GetChannel_Delegate_1>("FMOD5_System_GetChannel");
            return s_System_GetChannel_1(system, channelid, out channel);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT System_GetDSPInfoByType(IntPtr system, DSP_TYPE type, out IntPtr description)
        {
            s_System_GetDSPInfoByType_1 ??= LoadCoreFunction<System_GetDSPInfoByType_Delegate_1>("FMOD5_System_GetDSPInfoByType");
            return s_System_GetDSPInfoByType_1(system, type, out description);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT System_GetMasterChannelGroup(IntPtr system, out IntPtr channelgroup)
        {
            s_System_GetMasterChannelGroup_1 ??= LoadCoreFunction<System_GetMasterChannelGroup_Delegate_1>("FMOD5_System_GetMasterChannelGroup");
            return s_System_GetMasterChannelGroup_1(system, out channelgroup);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT System_GetMasterSoundGroup(IntPtr system, out IntPtr soundgroup)
        {
            s_System_GetMasterSoundGroup_1 ??= LoadCoreFunction<System_GetMasterSoundGroup_Delegate_1>("FMOD5_System_GetMasterSoundGroup");
            return s_System_GetMasterSoundGroup_1(system, out soundgroup);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT System_AttachChannelGroupToPort(IntPtr system, PORT_TYPE portType, ulong portIndex, IntPtr channelgroup, bool passThru)
        {
            s_System_AttachChannelGroupToPort_1 ??= LoadCoreFunction<System_AttachChannelGroupToPort_Delegate_1>("FMOD5_System_AttachChannelGroupToPort");
            return s_System_AttachChannelGroupToPort_1(system, portType, portIndex, channelgroup, passThru);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT System_DetachChannelGroupFromPort(IntPtr system, IntPtr channelgroup)
        {
            s_System_DetachChannelGroupFromPort_1 ??= LoadCoreFunction<System_DetachChannelGroupFromPort_Delegate_1>("FMOD5_System_DetachChannelGroupFromPort");
            return s_System_DetachChannelGroupFromPort_1(system, channelgroup);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT System_SetReverbProperties(IntPtr system, int instance, ref REVERB_PROPERTIES prop)
        {
            s_System_SetReverbProperties_1 ??= LoadCoreFunction<System_SetReverbProperties_Delegate_1>("FMOD5_System_SetReverbProperties");
            return s_System_SetReverbProperties_1(system, instance, ref prop);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT System_GetReverbProperties(IntPtr system, int instance, out REVERB_PROPERTIES prop)
        {
            s_System_GetReverbProperties_1 ??= LoadCoreFunction<System_GetReverbProperties_Delegate_1>("FMOD5_System_GetReverbProperties");
            return s_System_GetReverbProperties_1(system, instance, out prop);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT System_LockDSP(IntPtr system)
        {
            s_System_LockDSP_1 ??= LoadCoreFunction<System_LockDSP_Delegate_1>("FMOD5_System_LockDSP");
            return s_System_LockDSP_1(system);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT System_UnlockDSP(IntPtr system)
        {
            s_System_UnlockDSP_1 ??= LoadCoreFunction<System_UnlockDSP_Delegate_1>("FMOD5_System_UnlockDSP");
            return s_System_UnlockDSP_1(system);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT System_GetRecordNumDrivers(IntPtr system, out int numdrivers, out int numconnected)
        {
            s_System_GetRecordNumDrivers_1 ??= LoadCoreFunction<System_GetRecordNumDrivers_Delegate_1>("FMOD5_System_GetRecordNumDrivers");
            return s_System_GetRecordNumDrivers_1(system, out numdrivers, out numconnected);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT System_GetRecordDriverInfo(IntPtr system, int id, IntPtr name, int namelen, out Guid guid, out int systemrate, out SPEAKERMODE speakermode, out int speakermodechannels, out DRIVER_STATE state)
        {
            s_System_GetRecordDriverInfo_1 ??= LoadCoreFunction<System_GetRecordDriverInfo_Delegate_1>("FMOD5_System_GetRecordDriverInfo");
            return s_System_GetRecordDriverInfo_1(system, id, name, namelen, out guid, out systemrate, out speakermode, out speakermodechannels, out state);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT System_GetRecordPosition(IntPtr system, int id, out uint position)
        {
            s_System_GetRecordPosition_1 ??= LoadCoreFunction<System_GetRecordPosition_Delegate_1>("FMOD5_System_GetRecordPosition");
            return s_System_GetRecordPosition_1(system, id, out position);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT System_RecordStart(IntPtr system, int id, IntPtr sound, bool loop)
        {
            s_System_RecordStart_1 ??= LoadCoreFunction<System_RecordStart_Delegate_1>("FMOD5_System_RecordStart");
            return s_System_RecordStart_1(system, id, sound, loop);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT System_RecordStop(IntPtr system, int id)
        {
            s_System_RecordStop_1 ??= LoadCoreFunction<System_RecordStop_Delegate_1>("FMOD5_System_RecordStop");
            return s_System_RecordStop_1(system, id);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT System_IsRecording(IntPtr system, int id, out bool recording)
        {
            s_System_IsRecording_1 ??= LoadCoreFunction<System_IsRecording_Delegate_1>("FMOD5_System_IsRecording");
            return s_System_IsRecording_1(system, id, out recording);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT System_CreateGeometry(IntPtr system, int maxpolygons, int maxvertices, out IntPtr geometry)
        {
            s_System_CreateGeometry_1 ??= LoadCoreFunction<System_CreateGeometry_Delegate_1>("FMOD5_System_CreateGeometry");
            return s_System_CreateGeometry_1(system, maxpolygons, maxvertices, out geometry);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT System_SetGeometrySettings(IntPtr system, float maxworldsize)
        {
            s_System_SetGeometrySettings_1 ??= LoadCoreFunction<System_SetGeometrySettings_Delegate_1>("FMOD5_System_SetGeometrySettings");
            return s_System_SetGeometrySettings_1(system, maxworldsize);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT System_GetGeometrySettings(IntPtr system, out float maxworldsize)
        {
            s_System_GetGeometrySettings_1 ??= LoadCoreFunction<System_GetGeometrySettings_Delegate_1>("FMOD5_System_GetGeometrySettings");
            return s_System_GetGeometrySettings_1(system, out maxworldsize);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT System_LoadGeometry(IntPtr system, IntPtr data, int datasize, out IntPtr geometry)
        {
            s_System_LoadGeometry_1 ??= LoadCoreFunction<System_LoadGeometry_Delegate_1>("FMOD5_System_LoadGeometry");
            return s_System_LoadGeometry_1(system, data, datasize, out geometry);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT System_GetGeometryOcclusion(IntPtr system, ref VECTOR listener, ref VECTOR source, out float direct, out float reverb)
        {
            s_System_GetGeometryOcclusion_1 ??= LoadCoreFunction<System_GetGeometryOcclusion_Delegate_1>("FMOD5_System_GetGeometryOcclusion");
            return s_System_GetGeometryOcclusion_1(system, ref listener, ref source, out direct, out reverb);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT System_SetNetworkProxy(IntPtr system, byte[] proxy)
        {
            s_System_SetNetworkProxy_1 ??= LoadCoreFunction<System_SetNetworkProxy_Delegate_1>("FMOD5_System_SetNetworkProxy");
            return s_System_SetNetworkProxy_1(system, proxy);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT System_GetNetworkProxy(IntPtr system, IntPtr proxy, int proxylen)
        {
            s_System_GetNetworkProxy_1 ??= LoadCoreFunction<System_GetNetworkProxy_Delegate_1>("FMOD5_System_GetNetworkProxy");
            return s_System_GetNetworkProxy_1(system, proxy, proxylen);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT System_SetNetworkTimeout(IntPtr system, int timeout)
        {
            s_System_SetNetworkTimeout_1 ??= LoadCoreFunction<System_SetNetworkTimeout_Delegate_1>("FMOD5_System_SetNetworkTimeout");
            return s_System_SetNetworkTimeout_1(system, timeout);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT System_GetNetworkTimeout(IntPtr system, out int timeout)
        {
            s_System_GetNetworkTimeout_1 ??= LoadCoreFunction<System_GetNetworkTimeout_Delegate_1>("FMOD5_System_GetNetworkTimeout");
            return s_System_GetNetworkTimeout_1(system, out timeout);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT System_SetUserData(IntPtr system, IntPtr userdata)
        {
            s_System_SetUserData_1 ??= LoadCoreFunction<System_SetUserData_Delegate_1>("FMOD5_System_SetUserData");
            return s_System_SetUserData_1(system, userdata);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT System_GetUserData(IntPtr system, out IntPtr userdata)
        {
            s_System_GetUserData_1 ??= LoadCoreFunction<System_GetUserData_Delegate_1>("FMOD5_System_GetUserData");
            return s_System_GetUserData_1(system, out userdata);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Sound_Release(IntPtr sound)
        {
            s_Sound_Release_1 ??= LoadCoreFunction<Sound_Release_Delegate_1>("FMOD5_Sound_Release");
            return s_Sound_Release_1(sound);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Sound_GetSystemObject(IntPtr sound, out IntPtr system)
        {
            s_Sound_GetSystemObject_1 ??= LoadCoreFunction<Sound_GetSystemObject_Delegate_1>("FMOD5_Sound_GetSystemObject");
            return s_Sound_GetSystemObject_1(sound, out system);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Sound_Lock(IntPtr sound, uint offset, uint length, out IntPtr ptr1, out IntPtr ptr2, out uint len1, out uint len2)
        {
            s_Sound_Lock_1 ??= LoadCoreFunction<Sound_Lock_Delegate_1>("FMOD5_Sound_Lock");
            return s_Sound_Lock_1(sound, offset, length, out ptr1, out ptr2, out len1, out len2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Sound_Unlock(IntPtr sound, IntPtr ptr1, IntPtr ptr2, uint len1, uint len2)
        {
            s_Sound_Unlock_1 ??= LoadCoreFunction<Sound_Unlock_Delegate_1>("FMOD5_Sound_Unlock");
            return s_Sound_Unlock_1(sound, ptr1, ptr2, len1, len2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Sound_SetDefaults(IntPtr sound, float frequency, int priority)
        {
            s_Sound_SetDefaults_1 ??= LoadCoreFunction<Sound_SetDefaults_Delegate_1>("FMOD5_Sound_SetDefaults");
            return s_Sound_SetDefaults_1(sound, frequency, priority);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Sound_GetDefaults(IntPtr sound, out float frequency, out int priority)
        {
            s_Sound_GetDefaults_1 ??= LoadCoreFunction<Sound_GetDefaults_Delegate_1>("FMOD5_Sound_GetDefaults");
            return s_Sound_GetDefaults_1(sound, out frequency, out priority);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Sound_Set3DMinMaxDistance(IntPtr sound, float min, float max)
        {
            s_Sound_Set3DMinMaxDistance_1 ??= LoadCoreFunction<Sound_Set3DMinMaxDistance_Delegate_1>("FMOD5_Sound_Set3DMinMaxDistance");
            return s_Sound_Set3DMinMaxDistance_1(sound, min, max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Sound_Get3DMinMaxDistance(IntPtr sound, out float min, out float max)
        {
            s_Sound_Get3DMinMaxDistance_1 ??= LoadCoreFunction<Sound_Get3DMinMaxDistance_Delegate_1>("FMOD5_Sound_Get3DMinMaxDistance");
            return s_Sound_Get3DMinMaxDistance_1(sound, out min, out max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Sound_Set3DConeSettings(IntPtr sound, float insideconeangle, float outsideconeangle, float outsidevolume)
        {
            s_Sound_Set3DConeSettings_1 ??= LoadCoreFunction<Sound_Set3DConeSettings_Delegate_1>("FMOD5_Sound_Set3DConeSettings");
            return s_Sound_Set3DConeSettings_1(sound, insideconeangle, outsideconeangle, outsidevolume);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Sound_Get3DConeSettings(IntPtr sound, out float insideconeangle, out float outsideconeangle, out float outsidevolume)
        {
            s_Sound_Get3DConeSettings_1 ??= LoadCoreFunction<Sound_Get3DConeSettings_Delegate_1>("FMOD5_Sound_Get3DConeSettings");
            return s_Sound_Get3DConeSettings_1(sound, out insideconeangle, out outsideconeangle, out outsidevolume);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Sound_Set3DCustomRolloff(IntPtr sound, ref VECTOR points, int numpoints)
        {
            s_Sound_Set3DCustomRolloff_1 ??= LoadCoreFunction<Sound_Set3DCustomRolloff_Delegate_1>("FMOD5_Sound_Set3DCustomRolloff");
            return s_Sound_Set3DCustomRolloff_1(sound, ref points, numpoints);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Sound_Get3DCustomRolloff(IntPtr sound, out IntPtr points, out int numpoints)
        {
            s_Sound_Get3DCustomRolloff_1 ??= LoadCoreFunction<Sound_Get3DCustomRolloff_Delegate_1>("FMOD5_Sound_Get3DCustomRolloff");
            return s_Sound_Get3DCustomRolloff_1(sound, out points, out numpoints);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Sound_GetSubSound(IntPtr sound, int index, out IntPtr subsound)
        {
            s_Sound_GetSubSound_1 ??= LoadCoreFunction<Sound_GetSubSound_Delegate_1>("FMOD5_Sound_GetSubSound");
            return s_Sound_GetSubSound_1(sound, index, out subsound);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Sound_GetSubSoundParent(IntPtr sound, out IntPtr parentsound)
        {
            s_Sound_GetSubSoundParent_1 ??= LoadCoreFunction<Sound_GetSubSoundParent_Delegate_1>("FMOD5_Sound_GetSubSoundParent");
            return s_Sound_GetSubSoundParent_1(sound, out parentsound);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Sound_GetName(IntPtr sound, IntPtr name, int namelen)
        {
            s_Sound_GetName_1 ??= LoadCoreFunction<Sound_GetName_Delegate_1>("FMOD5_Sound_GetName");
            return s_Sound_GetName_1(sound, name, namelen);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Sound_GetLength(IntPtr sound, out uint length, TIMEUNIT lengthtype)
        {
            s_Sound_GetLength_1 ??= LoadCoreFunction<Sound_GetLength_Delegate_1>("FMOD5_Sound_GetLength");
            return s_Sound_GetLength_1(sound, out length, lengthtype);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Sound_GetFormat(IntPtr sound, out SOUND_TYPE type, out SOUND_FORMAT format, out int channels, out int bits)
        {
            s_Sound_GetFormat_1 ??= LoadCoreFunction<Sound_GetFormat_Delegate_1>("FMOD5_Sound_GetFormat");
            return s_Sound_GetFormat_1(sound, out type, out format, out channels, out bits);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Sound_GetNumSubSounds(IntPtr sound, out int numsubsounds)
        {
            s_Sound_GetNumSubSounds_1 ??= LoadCoreFunction<Sound_GetNumSubSounds_Delegate_1>("FMOD5_Sound_GetNumSubSounds");
            return s_Sound_GetNumSubSounds_1(sound, out numsubsounds);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Sound_GetNumTags(IntPtr sound, out int numtags, out int numtagsupdated)
        {
            s_Sound_GetNumTags_1 ??= LoadCoreFunction<Sound_GetNumTags_Delegate_1>("FMOD5_Sound_GetNumTags");
            return s_Sound_GetNumTags_1(sound, out numtags, out numtagsupdated);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Sound_GetTag(IntPtr sound, byte[] name, int index, out TAG tag)
        {
            s_Sound_GetTag_1 ??= LoadCoreFunction<Sound_GetTag_Delegate_1>("FMOD5_Sound_GetTag");
            return s_Sound_GetTag_1(sound, name, index, out tag);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Sound_GetOpenState(IntPtr sound, out OPENSTATE openstate, out uint percentbuffered, out bool starving, out bool diskbusy)
        {
            s_Sound_GetOpenState_1 ??= LoadCoreFunction<Sound_GetOpenState_Delegate_1>("FMOD5_Sound_GetOpenState");
            return s_Sound_GetOpenState_1(sound, out openstate, out percentbuffered, out starving, out diskbusy);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Sound_ReadData(IntPtr sound, byte[] buffer, uint length, IntPtr zero)
        {
            s_Sound_ReadData_1 ??= LoadCoreFunction<Sound_ReadData_Delegate_1>("FMOD5_Sound_ReadData");
            return s_Sound_ReadData_1(sound, buffer, length, zero);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Sound_ReadData(IntPtr sound, byte[] buffer, uint length, out uint read)
        {
            s_Sound_ReadData_2 ??= LoadCoreFunction<Sound_ReadData_Delegate_2>("FMOD5_Sound_ReadData");
            return s_Sound_ReadData_2(sound, buffer, length, out read);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Sound_SeekData(IntPtr sound, uint pcm)
        {
            s_Sound_SeekData_1 ??= LoadCoreFunction<Sound_SeekData_Delegate_1>("FMOD5_Sound_SeekData");
            return s_Sound_SeekData_1(sound, pcm);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Sound_SetSoundGroup(IntPtr sound, IntPtr soundgroup)
        {
            s_Sound_SetSoundGroup_1 ??= LoadCoreFunction<Sound_SetSoundGroup_Delegate_1>("FMOD5_Sound_SetSoundGroup");
            return s_Sound_SetSoundGroup_1(sound, soundgroup);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Sound_GetSoundGroup(IntPtr sound, out IntPtr soundgroup)
        {
            s_Sound_GetSoundGroup_1 ??= LoadCoreFunction<Sound_GetSoundGroup_Delegate_1>("FMOD5_Sound_GetSoundGroup");
            return s_Sound_GetSoundGroup_1(sound, out soundgroup);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Sound_GetNumSyncPoints(IntPtr sound, out int numsyncpoints)
        {
            s_Sound_GetNumSyncPoints_1 ??= LoadCoreFunction<Sound_GetNumSyncPoints_Delegate_1>("FMOD5_Sound_GetNumSyncPoints");
            return s_Sound_GetNumSyncPoints_1(sound, out numsyncpoints);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Sound_GetSyncPoint(IntPtr sound, int index, out IntPtr point)
        {
            s_Sound_GetSyncPoint_1 ??= LoadCoreFunction<Sound_GetSyncPoint_Delegate_1>("FMOD5_Sound_GetSyncPoint");
            return s_Sound_GetSyncPoint_1(sound, index, out point);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Sound_GetSyncPointInfo(IntPtr sound, IntPtr point, IntPtr name, int namelen, out uint offset, TIMEUNIT offsettype)
        {
            s_Sound_GetSyncPointInfo_1 ??= LoadCoreFunction<Sound_GetSyncPointInfo_Delegate_1>("FMOD5_Sound_GetSyncPointInfo");
            return s_Sound_GetSyncPointInfo_1(sound, point, name, namelen, out offset, offsettype);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Sound_AddSyncPoint(IntPtr sound, uint offset, TIMEUNIT offsettype, byte[] name, out IntPtr point)
        {
            s_Sound_AddSyncPoint_1 ??= LoadCoreFunction<Sound_AddSyncPoint_Delegate_1>("FMOD5_Sound_AddSyncPoint");
            return s_Sound_AddSyncPoint_1(sound, offset, offsettype, name, out point);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Sound_DeleteSyncPoint(IntPtr sound, IntPtr point)
        {
            s_Sound_DeleteSyncPoint_1 ??= LoadCoreFunction<Sound_DeleteSyncPoint_Delegate_1>("FMOD5_Sound_DeleteSyncPoint");
            return s_Sound_DeleteSyncPoint_1(sound, point);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Sound_SetMode(IntPtr sound, MODE mode)
        {
            s_Sound_SetMode_1 ??= LoadCoreFunction<Sound_SetMode_Delegate_1>("FMOD5_Sound_SetMode");
            return s_Sound_SetMode_1(sound, mode);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Sound_GetMode(IntPtr sound, out MODE mode)
        {
            s_Sound_GetMode_1 ??= LoadCoreFunction<Sound_GetMode_Delegate_1>("FMOD5_Sound_GetMode");
            return s_Sound_GetMode_1(sound, out mode);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Sound_SetLoopCount(IntPtr sound, int loopcount)
        {
            s_Sound_SetLoopCount_1 ??= LoadCoreFunction<Sound_SetLoopCount_Delegate_1>("FMOD5_Sound_SetLoopCount");
            return s_Sound_SetLoopCount_1(sound, loopcount);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Sound_GetLoopCount(IntPtr sound, out int loopcount)
        {
            s_Sound_GetLoopCount_1 ??= LoadCoreFunction<Sound_GetLoopCount_Delegate_1>("FMOD5_Sound_GetLoopCount");
            return s_Sound_GetLoopCount_1(sound, out loopcount);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Sound_SetLoopPoints(IntPtr sound, uint loopstart, TIMEUNIT loopstarttype, uint loopend, TIMEUNIT loopendtype)
        {
            s_Sound_SetLoopPoints_1 ??= LoadCoreFunction<Sound_SetLoopPoints_Delegate_1>("FMOD5_Sound_SetLoopPoints");
            return s_Sound_SetLoopPoints_1(sound, loopstart, loopstarttype, loopend, loopendtype);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Sound_GetLoopPoints(IntPtr sound, out uint loopstart, TIMEUNIT loopstarttype, out uint loopend, TIMEUNIT loopendtype)
        {
            s_Sound_GetLoopPoints_1 ??= LoadCoreFunction<Sound_GetLoopPoints_Delegate_1>("FMOD5_Sound_GetLoopPoints");
            return s_Sound_GetLoopPoints_1(sound, out loopstart, loopstarttype, out loopend, loopendtype);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Sound_GetMusicNumChannels(IntPtr sound, out int numchannels)
        {
            s_Sound_GetMusicNumChannels_1 ??= LoadCoreFunction<Sound_GetMusicNumChannels_Delegate_1>("FMOD5_Sound_GetMusicNumChannels");
            return s_Sound_GetMusicNumChannels_1(sound, out numchannels);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Sound_SetMusicChannelVolume(IntPtr sound, int channel, float volume)
        {
            s_Sound_SetMusicChannelVolume_1 ??= LoadCoreFunction<Sound_SetMusicChannelVolume_Delegate_1>("FMOD5_Sound_SetMusicChannelVolume");
            return s_Sound_SetMusicChannelVolume_1(sound, channel, volume);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Sound_GetMusicChannelVolume(IntPtr sound, int channel, out float volume)
        {
            s_Sound_GetMusicChannelVolume_1 ??= LoadCoreFunction<Sound_GetMusicChannelVolume_Delegate_1>("FMOD5_Sound_GetMusicChannelVolume");
            return s_Sound_GetMusicChannelVolume_1(sound, channel, out volume);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Sound_SetMusicSpeed(IntPtr sound, float speed)
        {
            s_Sound_SetMusicSpeed_1 ??= LoadCoreFunction<Sound_SetMusicSpeed_Delegate_1>("FMOD5_Sound_SetMusicSpeed");
            return s_Sound_SetMusicSpeed_1(sound, speed);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Sound_GetMusicSpeed(IntPtr sound, out float speed)
        {
            s_Sound_GetMusicSpeed_1 ??= LoadCoreFunction<Sound_GetMusicSpeed_Delegate_1>("FMOD5_Sound_GetMusicSpeed");
            return s_Sound_GetMusicSpeed_1(sound, out speed);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Sound_SetUserData(IntPtr sound, IntPtr userdata)
        {
            s_Sound_SetUserData_1 ??= LoadCoreFunction<Sound_SetUserData_Delegate_1>("FMOD5_Sound_SetUserData");
            return s_Sound_SetUserData_1(sound, userdata);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Sound_GetUserData(IntPtr sound, out IntPtr userdata)
        {
            s_Sound_GetUserData_1 ??= LoadCoreFunction<Sound_GetUserData_Delegate_1>("FMOD5_Sound_GetUserData");
            return s_Sound_GetUserData_1(sound, out userdata);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Channel_SetFrequency(IntPtr channel, float frequency)
        {
            s_Channel_SetFrequency_1 ??= LoadCoreFunction<Channel_SetFrequency_Delegate_1>("FMOD5_Channel_SetFrequency");
            return s_Channel_SetFrequency_1(channel, frequency);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Channel_GetFrequency(IntPtr channel, out float frequency)
        {
            s_Channel_GetFrequency_1 ??= LoadCoreFunction<Channel_GetFrequency_Delegate_1>("FMOD5_Channel_GetFrequency");
            return s_Channel_GetFrequency_1(channel, out frequency);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Channel_SetPriority(IntPtr channel, int priority)
        {
            s_Channel_SetPriority_1 ??= LoadCoreFunction<Channel_SetPriority_Delegate_1>("FMOD5_Channel_SetPriority");
            return s_Channel_SetPriority_1(channel, priority);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Channel_GetPriority(IntPtr channel, out int priority)
        {
            s_Channel_GetPriority_1 ??= LoadCoreFunction<Channel_GetPriority_Delegate_1>("FMOD5_Channel_GetPriority");
            return s_Channel_GetPriority_1(channel, out priority);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Channel_SetPosition(IntPtr channel, uint position, TIMEUNIT postype)
        {
            s_Channel_SetPosition_1 ??= LoadCoreFunction<Channel_SetPosition_Delegate_1>("FMOD5_Channel_SetPosition");
            return s_Channel_SetPosition_1(channel, position, postype);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Channel_GetPosition(IntPtr channel, out uint position, TIMEUNIT postype)
        {
            s_Channel_GetPosition_1 ??= LoadCoreFunction<Channel_GetPosition_Delegate_1>("FMOD5_Channel_GetPosition");
            return s_Channel_GetPosition_1(channel, out position, postype);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Channel_SetChannelGroup(IntPtr channel, IntPtr channelgroup)
        {
            s_Channel_SetChannelGroup_1 ??= LoadCoreFunction<Channel_SetChannelGroup_Delegate_1>("FMOD5_Channel_SetChannelGroup");
            return s_Channel_SetChannelGroup_1(channel, channelgroup);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Channel_GetChannelGroup(IntPtr channel, out IntPtr channelgroup)
        {
            s_Channel_GetChannelGroup_1 ??= LoadCoreFunction<Channel_GetChannelGroup_Delegate_1>("FMOD5_Channel_GetChannelGroup");
            return s_Channel_GetChannelGroup_1(channel, out channelgroup);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Channel_SetLoopCount(IntPtr channel, int loopcount)
        {
            s_Channel_SetLoopCount_1 ??= LoadCoreFunction<Channel_SetLoopCount_Delegate_1>("FMOD5_Channel_SetLoopCount");
            return s_Channel_SetLoopCount_1(channel, loopcount);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Channel_GetLoopCount(IntPtr channel, out int loopcount)
        {
            s_Channel_GetLoopCount_1 ??= LoadCoreFunction<Channel_GetLoopCount_Delegate_1>("FMOD5_Channel_GetLoopCount");
            return s_Channel_GetLoopCount_1(channel, out loopcount);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Channel_SetLoopPoints(IntPtr channel, uint loopstart, TIMEUNIT loopstarttype, uint loopend, TIMEUNIT loopendtype)
        {
            s_Channel_SetLoopPoints_1 ??= LoadCoreFunction<Channel_SetLoopPoints_Delegate_1>("FMOD5_Channel_SetLoopPoints");
            return s_Channel_SetLoopPoints_1(channel, loopstart, loopstarttype, loopend, loopendtype);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Channel_GetLoopPoints(IntPtr channel, out uint loopstart, TIMEUNIT loopstarttype, out uint loopend, TIMEUNIT loopendtype)
        {
            s_Channel_GetLoopPoints_1 ??= LoadCoreFunction<Channel_GetLoopPoints_Delegate_1>("FMOD5_Channel_GetLoopPoints");
            return s_Channel_GetLoopPoints_1(channel, out loopstart, loopstarttype, out loopend, loopendtype);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Channel_IsVirtual(IntPtr channel, out bool isvirtual)
        {
            s_Channel_IsVirtual_1 ??= LoadCoreFunction<Channel_IsVirtual_Delegate_1>("FMOD5_Channel_IsVirtual");
            return s_Channel_IsVirtual_1(channel, out isvirtual);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Channel_GetCurrentSound(IntPtr channel, out IntPtr sound)
        {
            s_Channel_GetCurrentSound_1 ??= LoadCoreFunction<Channel_GetCurrentSound_Delegate_1>("FMOD5_Channel_GetCurrentSound");
            return s_Channel_GetCurrentSound_1(channel, out sound);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Channel_GetIndex(IntPtr channel, out int index)
        {
            s_Channel_GetIndex_1 ??= LoadCoreFunction<Channel_GetIndex_Delegate_1>("FMOD5_Channel_GetIndex");
            return s_Channel_GetIndex_1(channel, out index);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Channel_GetSystemObject(IntPtr channel, out IntPtr system)
        {
            s_Channel_GetSystemObject_1 ??= LoadCoreFunction<Channel_GetSystemObject_Delegate_1>("FMOD5_Channel_GetSystemObject");
            return s_Channel_GetSystemObject_1(channel, out system);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Channel_Stop(IntPtr channel)
        {
            s_Channel_Stop_1 ??= LoadCoreFunction<Channel_Stop_Delegate_1>("FMOD5_Channel_Stop");
            return s_Channel_Stop_1(channel);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Channel_SetPaused(IntPtr channel, bool paused)
        {
            s_Channel_SetPaused_1 ??= LoadCoreFunction<Channel_SetPaused_Delegate_1>("FMOD5_Channel_SetPaused");
            return s_Channel_SetPaused_1(channel, paused);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Channel_GetPaused(IntPtr channel, out bool paused)
        {
            s_Channel_GetPaused_1 ??= LoadCoreFunction<Channel_GetPaused_Delegate_1>("FMOD5_Channel_GetPaused");
            return s_Channel_GetPaused_1(channel, out paused);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Channel_SetVolume(IntPtr channel, float volume)
        {
            s_Channel_SetVolume_1 ??= LoadCoreFunction<Channel_SetVolume_Delegate_1>("FMOD5_Channel_SetVolume");
            return s_Channel_SetVolume_1(channel, volume);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Channel_GetVolume(IntPtr channel, out float volume)
        {
            s_Channel_GetVolume_1 ??= LoadCoreFunction<Channel_GetVolume_Delegate_1>("FMOD5_Channel_GetVolume");
            return s_Channel_GetVolume_1(channel, out volume);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Channel_SetVolumeRamp(IntPtr channel, bool ramp)
        {
            s_Channel_SetVolumeRamp_1 ??= LoadCoreFunction<Channel_SetVolumeRamp_Delegate_1>("FMOD5_Channel_SetVolumeRamp");
            return s_Channel_SetVolumeRamp_1(channel, ramp);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Channel_GetVolumeRamp(IntPtr channel, out bool ramp)
        {
            s_Channel_GetVolumeRamp_1 ??= LoadCoreFunction<Channel_GetVolumeRamp_Delegate_1>("FMOD5_Channel_GetVolumeRamp");
            return s_Channel_GetVolumeRamp_1(channel, out ramp);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Channel_GetAudibility(IntPtr channel, out float audibility)
        {
            s_Channel_GetAudibility_1 ??= LoadCoreFunction<Channel_GetAudibility_Delegate_1>("FMOD5_Channel_GetAudibility");
            return s_Channel_GetAudibility_1(channel, out audibility);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Channel_SetPitch(IntPtr channel, float pitch)
        {
            s_Channel_SetPitch_1 ??= LoadCoreFunction<Channel_SetPitch_Delegate_1>("FMOD5_Channel_SetPitch");
            return s_Channel_SetPitch_1(channel, pitch);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Channel_GetPitch(IntPtr channel, out float pitch)
        {
            s_Channel_GetPitch_1 ??= LoadCoreFunction<Channel_GetPitch_Delegate_1>("FMOD5_Channel_GetPitch");
            return s_Channel_GetPitch_1(channel, out pitch);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Channel_SetMute(IntPtr channel, bool mute)
        {
            s_Channel_SetMute_1 ??= LoadCoreFunction<Channel_SetMute_Delegate_1>("FMOD5_Channel_SetMute");
            return s_Channel_SetMute_1(channel, mute);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Channel_GetMute(IntPtr channel, out bool mute)
        {
            s_Channel_GetMute_1 ??= LoadCoreFunction<Channel_GetMute_Delegate_1>("FMOD5_Channel_GetMute");
            return s_Channel_GetMute_1(channel, out mute);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Channel_SetReverbProperties(IntPtr channel, int instance, float wet)
        {
            s_Channel_SetReverbProperties_1 ??= LoadCoreFunction<Channel_SetReverbProperties_Delegate_1>("FMOD5_Channel_SetReverbProperties");
            return s_Channel_SetReverbProperties_1(channel, instance, wet);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Channel_GetReverbProperties(IntPtr channel, int instance, out float wet)
        {
            s_Channel_GetReverbProperties_1 ??= LoadCoreFunction<Channel_GetReverbProperties_Delegate_1>("FMOD5_Channel_GetReverbProperties");
            return s_Channel_GetReverbProperties_1(channel, instance, out wet);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Channel_SetLowPassGain(IntPtr channel, float gain)
        {
            s_Channel_SetLowPassGain_1 ??= LoadCoreFunction<Channel_SetLowPassGain_Delegate_1>("FMOD5_Channel_SetLowPassGain");
            return s_Channel_SetLowPassGain_1(channel, gain);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Channel_GetLowPassGain(IntPtr channel, out float gain)
        {
            s_Channel_GetLowPassGain_1 ??= LoadCoreFunction<Channel_GetLowPassGain_Delegate_1>("FMOD5_Channel_GetLowPassGain");
            return s_Channel_GetLowPassGain_1(channel, out gain);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Channel_SetMode(IntPtr channel, MODE mode)
        {
            s_Channel_SetMode_1 ??= LoadCoreFunction<Channel_SetMode_Delegate_1>("FMOD5_Channel_SetMode");
            return s_Channel_SetMode_1(channel, mode);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Channel_GetMode(IntPtr channel, out MODE mode)
        {
            s_Channel_GetMode_1 ??= LoadCoreFunction<Channel_GetMode_Delegate_1>("FMOD5_Channel_GetMode");
            return s_Channel_GetMode_1(channel, out mode);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Channel_SetCallback(IntPtr channel, CHANNELCONTROL_CALLBACK callback)
        {
            s_Channel_SetCallback_1 ??= LoadCoreFunction<Channel_SetCallback_Delegate_1>("FMOD5_Channel_SetCallback");
            return s_Channel_SetCallback_1(channel, callback);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Channel_IsPlaying(IntPtr channel, out bool isplaying)
        {
            s_Channel_IsPlaying_1 ??= LoadCoreFunction<Channel_IsPlaying_Delegate_1>("FMOD5_Channel_IsPlaying");
            return s_Channel_IsPlaying_1(channel, out isplaying);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Channel_SetPan(IntPtr channel, float pan)
        {
            s_Channel_SetPan_1 ??= LoadCoreFunction<Channel_SetPan_Delegate_1>("FMOD5_Channel_SetPan");
            return s_Channel_SetPan_1(channel, pan);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Channel_SetMixLevelsOutput(IntPtr channel, float frontleft, float frontright, float center, float lfe, float surroundleft, float surroundright, float backleft, float backright)
        {
            s_Channel_SetMixLevelsOutput_1 ??= LoadCoreFunction<Channel_SetMixLevelsOutput_Delegate_1>("FMOD5_Channel_SetMixLevelsOutput");
            return s_Channel_SetMixLevelsOutput_1(channel, frontleft, frontright, center, lfe, surroundleft, surroundright, backleft, backright);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Channel_SetMixLevelsInput(IntPtr channel, float[] levels, int numlevels)
        {
            s_Channel_SetMixLevelsInput_1 ??= LoadCoreFunction<Channel_SetMixLevelsInput_Delegate_1>("FMOD5_Channel_SetMixLevelsInput");
            return s_Channel_SetMixLevelsInput_1(channel, levels, numlevels);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Channel_SetMixMatrix(IntPtr channel, float[] matrix, int outchannels, int inchannels, int inchannel_hop)
        {
            s_Channel_SetMixMatrix_1 ??= LoadCoreFunction<Channel_SetMixMatrix_Delegate_1>("FMOD5_Channel_SetMixMatrix");
            return s_Channel_SetMixMatrix_1(channel, matrix, outchannels, inchannels, inchannel_hop);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Channel_GetMixMatrix(IntPtr channel, float[] matrix, out int outchannels, out int inchannels, int inchannel_hop)
        {
            s_Channel_GetMixMatrix_1 ??= LoadCoreFunction<Channel_GetMixMatrix_Delegate_1>("FMOD5_Channel_GetMixMatrix");
            return s_Channel_GetMixMatrix_1(channel, matrix, out outchannels, out inchannels, inchannel_hop);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Channel_GetDSPClock(IntPtr channel, out ulong dspclock, out ulong parentclock)
        {
            s_Channel_GetDSPClock_1 ??= LoadCoreFunction<Channel_GetDSPClock_Delegate_1>("FMOD5_Channel_GetDSPClock");
            return s_Channel_GetDSPClock_1(channel, out dspclock, out parentclock);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Channel_SetDelay(IntPtr channel, ulong dspclock_start, ulong dspclock_end, bool stopchannels)
        {
            s_Channel_SetDelay_1 ??= LoadCoreFunction<Channel_SetDelay_Delegate_1>("FMOD5_Channel_SetDelay");
            return s_Channel_SetDelay_1(channel, dspclock_start, dspclock_end, stopchannels);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Channel_GetDelay(IntPtr channel, out ulong dspclock_start, out ulong dspclock_end, IntPtr zero)
        {
            s_Channel_GetDelay_1 ??= LoadCoreFunction<Channel_GetDelay_Delegate_1>("FMOD5_Channel_GetDelay");
            return s_Channel_GetDelay_1(channel, out dspclock_start, out dspclock_end, zero);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Channel_GetDelay(IntPtr channel, out ulong dspclock_start, out ulong dspclock_end, out bool stopchannels)
        {
            s_Channel_GetDelay_2 ??= LoadCoreFunction<Channel_GetDelay_Delegate_2>("FMOD5_Channel_GetDelay");
            return s_Channel_GetDelay_2(channel, out dspclock_start, out dspclock_end, out stopchannels);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Channel_AddFadePoint(IntPtr channel, ulong dspclock, float volume)
        {
            s_Channel_AddFadePoint_1 ??= LoadCoreFunction<Channel_AddFadePoint_Delegate_1>("FMOD5_Channel_AddFadePoint");
            return s_Channel_AddFadePoint_1(channel, dspclock, volume);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Channel_SetFadePointRamp(IntPtr channel, ulong dspclock, float volume)
        {
            s_Channel_SetFadePointRamp_1 ??= LoadCoreFunction<Channel_SetFadePointRamp_Delegate_1>("FMOD5_Channel_SetFadePointRamp");
            return s_Channel_SetFadePointRamp_1(channel, dspclock, volume);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Channel_RemoveFadePoints(IntPtr channel, ulong dspclock_start, ulong dspclock_end)
        {
            s_Channel_RemoveFadePoints_1 ??= LoadCoreFunction<Channel_RemoveFadePoints_Delegate_1>("FMOD5_Channel_RemoveFadePoints");
            return s_Channel_RemoveFadePoints_1(channel, dspclock_start, dspclock_end);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Channel_GetFadePoints(IntPtr channel, ref uint numpoints, ulong[] point_dspclock, float[] point_volume)
        {
            s_Channel_GetFadePoints_1 ??= LoadCoreFunction<Channel_GetFadePoints_Delegate_1>("FMOD5_Channel_GetFadePoints");
            return s_Channel_GetFadePoints_1(channel, ref numpoints, point_dspclock, point_volume);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Channel_GetDSP(IntPtr channel, int index, out IntPtr dsp)
        {
            s_Channel_GetDSP_1 ??= LoadCoreFunction<Channel_GetDSP_Delegate_1>("FMOD5_Channel_GetDSP");
            return s_Channel_GetDSP_1(channel, index, out dsp);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Channel_AddDSP(IntPtr channel, int index, IntPtr dsp)
        {
            s_Channel_AddDSP_1 ??= LoadCoreFunction<Channel_AddDSP_Delegate_1>("FMOD5_Channel_AddDSP");
            return s_Channel_AddDSP_1(channel, index, dsp);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Channel_RemoveDSP(IntPtr channel, IntPtr dsp)
        {
            s_Channel_RemoveDSP_1 ??= LoadCoreFunction<Channel_RemoveDSP_Delegate_1>("FMOD5_Channel_RemoveDSP");
            return s_Channel_RemoveDSP_1(channel, dsp);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Channel_GetNumDSPs(IntPtr channel, out int numdsps)
        {
            s_Channel_GetNumDSPs_1 ??= LoadCoreFunction<Channel_GetNumDSPs_Delegate_1>("FMOD5_Channel_GetNumDSPs");
            return s_Channel_GetNumDSPs_1(channel, out numdsps);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Channel_SetDSPIndex(IntPtr channel, IntPtr dsp, int index)
        {
            s_Channel_SetDSPIndex_1 ??= LoadCoreFunction<Channel_SetDSPIndex_Delegate_1>("FMOD5_Channel_SetDSPIndex");
            return s_Channel_SetDSPIndex_1(channel, dsp, index);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Channel_GetDSPIndex(IntPtr channel, IntPtr dsp, out int index)
        {
            s_Channel_GetDSPIndex_1 ??= LoadCoreFunction<Channel_GetDSPIndex_Delegate_1>("FMOD5_Channel_GetDSPIndex");
            return s_Channel_GetDSPIndex_1(channel, dsp, out index);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Channel_Set3DAttributes(IntPtr channel, ref VECTOR pos, ref VECTOR vel)
        {
            s_Channel_Set3DAttributes_1 ??= LoadCoreFunction<Channel_Set3DAttributes_Delegate_1>("FMOD5_Channel_Set3DAttributes");
            return s_Channel_Set3DAttributes_1(channel, ref pos, ref vel);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Channel_Get3DAttributes(IntPtr channel, out VECTOR pos, out VECTOR vel)
        {
            s_Channel_Get3DAttributes_1 ??= LoadCoreFunction<Channel_Get3DAttributes_Delegate_1>("FMOD5_Channel_Get3DAttributes");
            return s_Channel_Get3DAttributes_1(channel, out pos, out vel);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Channel_Set3DMinMaxDistance(IntPtr channel, float mindistance, float maxdistance)
        {
            s_Channel_Set3DMinMaxDistance_1 ??= LoadCoreFunction<Channel_Set3DMinMaxDistance_Delegate_1>("FMOD5_Channel_Set3DMinMaxDistance");
            return s_Channel_Set3DMinMaxDistance_1(channel, mindistance, maxdistance);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Channel_Get3DMinMaxDistance(IntPtr channel, out float mindistance, out float maxdistance)
        {
            s_Channel_Get3DMinMaxDistance_1 ??= LoadCoreFunction<Channel_Get3DMinMaxDistance_Delegate_1>("FMOD5_Channel_Get3DMinMaxDistance");
            return s_Channel_Get3DMinMaxDistance_1(channel, out mindistance, out maxdistance);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Channel_Set3DConeSettings(IntPtr channel, float insideconeangle, float outsideconeangle, float outsidevolume)
        {
            s_Channel_Set3DConeSettings_1 ??= LoadCoreFunction<Channel_Set3DConeSettings_Delegate_1>("FMOD5_Channel_Set3DConeSettings");
            return s_Channel_Set3DConeSettings_1(channel, insideconeangle, outsideconeangle, outsidevolume);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Channel_Get3DConeSettings(IntPtr channel, out float insideconeangle, out float outsideconeangle, out float outsidevolume)
        {
            s_Channel_Get3DConeSettings_1 ??= LoadCoreFunction<Channel_Get3DConeSettings_Delegate_1>("FMOD5_Channel_Get3DConeSettings");
            return s_Channel_Get3DConeSettings_1(channel, out insideconeangle, out outsideconeangle, out outsidevolume);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Channel_Set3DConeOrientation(IntPtr channel, ref VECTOR orientation)
        {
            s_Channel_Set3DConeOrientation_1 ??= LoadCoreFunction<Channel_Set3DConeOrientation_Delegate_1>("FMOD5_Channel_Set3DConeOrientation");
            return s_Channel_Set3DConeOrientation_1(channel, ref orientation);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Channel_Get3DConeOrientation(IntPtr channel, out VECTOR orientation)
        {
            s_Channel_Get3DConeOrientation_1 ??= LoadCoreFunction<Channel_Get3DConeOrientation_Delegate_1>("FMOD5_Channel_Get3DConeOrientation");
            return s_Channel_Get3DConeOrientation_1(channel, out orientation);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Channel_Set3DCustomRolloff(IntPtr channel, ref VECTOR points, int numpoints)
        {
            s_Channel_Set3DCustomRolloff_1 ??= LoadCoreFunction<Channel_Set3DCustomRolloff_Delegate_1>("FMOD5_Channel_Set3DCustomRolloff");
            return s_Channel_Set3DCustomRolloff_1(channel, ref points, numpoints);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Channel_Get3DCustomRolloff(IntPtr channel, out IntPtr points, out int numpoints)
        {
            s_Channel_Get3DCustomRolloff_1 ??= LoadCoreFunction<Channel_Get3DCustomRolloff_Delegate_1>("FMOD5_Channel_Get3DCustomRolloff");
            return s_Channel_Get3DCustomRolloff_1(channel, out points, out numpoints);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Channel_Set3DOcclusion(IntPtr channel, float directocclusion, float reverbocclusion)
        {
            s_Channel_Set3DOcclusion_1 ??= LoadCoreFunction<Channel_Set3DOcclusion_Delegate_1>("FMOD5_Channel_Set3DOcclusion");
            return s_Channel_Set3DOcclusion_1(channel, directocclusion, reverbocclusion);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Channel_Get3DOcclusion(IntPtr channel, out float directocclusion, out float reverbocclusion)
        {
            s_Channel_Get3DOcclusion_1 ??= LoadCoreFunction<Channel_Get3DOcclusion_Delegate_1>("FMOD5_Channel_Get3DOcclusion");
            return s_Channel_Get3DOcclusion_1(channel, out directocclusion, out reverbocclusion);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Channel_Set3DSpread(IntPtr channel, float angle)
        {
            s_Channel_Set3DSpread_1 ??= LoadCoreFunction<Channel_Set3DSpread_Delegate_1>("FMOD5_Channel_Set3DSpread");
            return s_Channel_Set3DSpread_1(channel, angle);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Channel_Get3DSpread(IntPtr channel, out float angle)
        {
            s_Channel_Get3DSpread_1 ??= LoadCoreFunction<Channel_Get3DSpread_Delegate_1>("FMOD5_Channel_Get3DSpread");
            return s_Channel_Get3DSpread_1(channel, out angle);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Channel_Set3DLevel(IntPtr channel, float level)
        {
            s_Channel_Set3DLevel_1 ??= LoadCoreFunction<Channel_Set3DLevel_Delegate_1>("FMOD5_Channel_Set3DLevel");
            return s_Channel_Set3DLevel_1(channel, level);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Channel_Get3DLevel(IntPtr channel, out float level)
        {
            s_Channel_Get3DLevel_1 ??= LoadCoreFunction<Channel_Get3DLevel_Delegate_1>("FMOD5_Channel_Get3DLevel");
            return s_Channel_Get3DLevel_1(channel, out level);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Channel_Set3DDopplerLevel(IntPtr channel, float level)
        {
            s_Channel_Set3DDopplerLevel_1 ??= LoadCoreFunction<Channel_Set3DDopplerLevel_Delegate_1>("FMOD5_Channel_Set3DDopplerLevel");
            return s_Channel_Set3DDopplerLevel_1(channel, level);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Channel_Get3DDopplerLevel(IntPtr channel, out float level)
        {
            s_Channel_Get3DDopplerLevel_1 ??= LoadCoreFunction<Channel_Get3DDopplerLevel_Delegate_1>("FMOD5_Channel_Get3DDopplerLevel");
            return s_Channel_Get3DDopplerLevel_1(channel, out level);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Channel_Set3DDistanceFilter(IntPtr channel, bool custom, float customLevel, float centerFreq)
        {
            s_Channel_Set3DDistanceFilter_1 ??= LoadCoreFunction<Channel_Set3DDistanceFilter_Delegate_1>("FMOD5_Channel_Set3DDistanceFilter");
            return s_Channel_Set3DDistanceFilter_1(channel, custom, customLevel, centerFreq);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Channel_Get3DDistanceFilter(IntPtr channel, out bool custom, out float customLevel, out float centerFreq)
        {
            s_Channel_Get3DDistanceFilter_1 ??= LoadCoreFunction<Channel_Get3DDistanceFilter_Delegate_1>("FMOD5_Channel_Get3DDistanceFilter");
            return s_Channel_Get3DDistanceFilter_1(channel, out custom, out customLevel, out centerFreq);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Channel_SetUserData(IntPtr channel, IntPtr userdata)
        {
            s_Channel_SetUserData_1 ??= LoadCoreFunction<Channel_SetUserData_Delegate_1>("FMOD5_Channel_SetUserData");
            return s_Channel_SetUserData_1(channel, userdata);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Channel_GetUserData(IntPtr channel, out IntPtr userdata)
        {
            s_Channel_GetUserData_1 ??= LoadCoreFunction<Channel_GetUserData_Delegate_1>("FMOD5_Channel_GetUserData");
            return s_Channel_GetUserData_1(channel, out userdata);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT ChannelGroup_Release(IntPtr channelgroup)
        {
            s_ChannelGroup_Release_1 ??= LoadCoreFunction<ChannelGroup_Release_Delegate_1>("FMOD5_ChannelGroup_Release");
            return s_ChannelGroup_Release_1(channelgroup);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT ChannelGroup_AddGroup(IntPtr channelgroup, IntPtr group, bool propagatedspclock, IntPtr zero)
        {
            s_ChannelGroup_AddGroup_1 ??= LoadCoreFunction<ChannelGroup_AddGroup_Delegate_1>("FMOD5_ChannelGroup_AddGroup");
            return s_ChannelGroup_AddGroup_1(channelgroup, group, propagatedspclock, zero);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT ChannelGroup_AddGroup(IntPtr channelgroup, IntPtr group, bool propagatedspclock, out IntPtr connection)
        {
            s_ChannelGroup_AddGroup_2 ??= LoadCoreFunction<ChannelGroup_AddGroup_Delegate_2>("FMOD5_ChannelGroup_AddGroup");
            return s_ChannelGroup_AddGroup_2(channelgroup, group, propagatedspclock, out connection);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT ChannelGroup_GetNumGroups(IntPtr channelgroup, out int numgroups)
        {
            s_ChannelGroup_GetNumGroups_1 ??= LoadCoreFunction<ChannelGroup_GetNumGroups_Delegate_1>("FMOD5_ChannelGroup_GetNumGroups");
            return s_ChannelGroup_GetNumGroups_1(channelgroup, out numgroups);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT ChannelGroup_GetGroup(IntPtr channelgroup, int index, out IntPtr group)
        {
            s_ChannelGroup_GetGroup_1 ??= LoadCoreFunction<ChannelGroup_GetGroup_Delegate_1>("FMOD5_ChannelGroup_GetGroup");
            return s_ChannelGroup_GetGroup_1(channelgroup, index, out group);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT ChannelGroup_GetParentGroup(IntPtr channelgroup, out IntPtr group)
        {
            s_ChannelGroup_GetParentGroup_1 ??= LoadCoreFunction<ChannelGroup_GetParentGroup_Delegate_1>("FMOD5_ChannelGroup_GetParentGroup");
            return s_ChannelGroup_GetParentGroup_1(channelgroup, out group);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT ChannelGroup_GetName(IntPtr channelgroup, IntPtr name, int namelen)
        {
            s_ChannelGroup_GetName_1 ??= LoadCoreFunction<ChannelGroup_GetName_Delegate_1>("FMOD5_ChannelGroup_GetName");
            return s_ChannelGroup_GetName_1(channelgroup, name, namelen);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT ChannelGroup_GetNumChannels(IntPtr channelgroup, out int numchannels)
        {
            s_ChannelGroup_GetNumChannels_1 ??= LoadCoreFunction<ChannelGroup_GetNumChannels_Delegate_1>("FMOD5_ChannelGroup_GetNumChannels");
            return s_ChannelGroup_GetNumChannels_1(channelgroup, out numchannels);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT ChannelGroup_GetChannel(IntPtr channelgroup, int index, out IntPtr channel)
        {
            s_ChannelGroup_GetChannel_1 ??= LoadCoreFunction<ChannelGroup_GetChannel_Delegate_1>("FMOD5_ChannelGroup_GetChannel");
            return s_ChannelGroup_GetChannel_1(channelgroup, index, out channel);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT ChannelGroup_GetSystemObject(IntPtr channelgroup, out IntPtr system)
        {
            s_ChannelGroup_GetSystemObject_1 ??= LoadCoreFunction<ChannelGroup_GetSystemObject_Delegate_1>("FMOD5_ChannelGroup_GetSystemObject");
            return s_ChannelGroup_GetSystemObject_1(channelgroup, out system);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT ChannelGroup_Stop(IntPtr channelgroup)
        {
            s_ChannelGroup_Stop_1 ??= LoadCoreFunction<ChannelGroup_Stop_Delegate_1>("FMOD5_ChannelGroup_Stop");
            return s_ChannelGroup_Stop_1(channelgroup);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT ChannelGroup_SetPaused(IntPtr channelgroup, bool paused)
        {
            s_ChannelGroup_SetPaused_1 ??= LoadCoreFunction<ChannelGroup_SetPaused_Delegate_1>("FMOD5_ChannelGroup_SetPaused");
            return s_ChannelGroup_SetPaused_1(channelgroup, paused);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT ChannelGroup_GetPaused(IntPtr channelgroup, out bool paused)
        {
            s_ChannelGroup_GetPaused_1 ??= LoadCoreFunction<ChannelGroup_GetPaused_Delegate_1>("FMOD5_ChannelGroup_GetPaused");
            return s_ChannelGroup_GetPaused_1(channelgroup, out paused);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT ChannelGroup_SetVolume(IntPtr channelgroup, float volume)
        {
            s_ChannelGroup_SetVolume_1 ??= LoadCoreFunction<ChannelGroup_SetVolume_Delegate_1>("FMOD5_ChannelGroup_SetVolume");
            return s_ChannelGroup_SetVolume_1(channelgroup, volume);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT ChannelGroup_GetVolume(IntPtr channelgroup, out float volume)
        {
            s_ChannelGroup_GetVolume_1 ??= LoadCoreFunction<ChannelGroup_GetVolume_Delegate_1>("FMOD5_ChannelGroup_GetVolume");
            return s_ChannelGroup_GetVolume_1(channelgroup, out volume);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT ChannelGroup_SetVolumeRamp(IntPtr channelgroup, bool ramp)
        {
            s_ChannelGroup_SetVolumeRamp_1 ??= LoadCoreFunction<ChannelGroup_SetVolumeRamp_Delegate_1>("FMOD5_ChannelGroup_SetVolumeRamp");
            return s_ChannelGroup_SetVolumeRamp_1(channelgroup, ramp);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT ChannelGroup_GetVolumeRamp(IntPtr channelgroup, out bool ramp)
        {
            s_ChannelGroup_GetVolumeRamp_1 ??= LoadCoreFunction<ChannelGroup_GetVolumeRamp_Delegate_1>("FMOD5_ChannelGroup_GetVolumeRamp");
            return s_ChannelGroup_GetVolumeRamp_1(channelgroup, out ramp);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT ChannelGroup_GetAudibility(IntPtr channelgroup, out float audibility)
        {
            s_ChannelGroup_GetAudibility_1 ??= LoadCoreFunction<ChannelGroup_GetAudibility_Delegate_1>("FMOD5_ChannelGroup_GetAudibility");
            return s_ChannelGroup_GetAudibility_1(channelgroup, out audibility);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT ChannelGroup_SetPitch(IntPtr channelgroup, float pitch)
        {
            s_ChannelGroup_SetPitch_1 ??= LoadCoreFunction<ChannelGroup_SetPitch_Delegate_1>("FMOD5_ChannelGroup_SetPitch");
            return s_ChannelGroup_SetPitch_1(channelgroup, pitch);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT ChannelGroup_GetPitch(IntPtr channelgroup, out float pitch)
        {
            s_ChannelGroup_GetPitch_1 ??= LoadCoreFunction<ChannelGroup_GetPitch_Delegate_1>("FMOD5_ChannelGroup_GetPitch");
            return s_ChannelGroup_GetPitch_1(channelgroup, out pitch);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT ChannelGroup_SetMute(IntPtr channelgroup, bool mute)
        {
            s_ChannelGroup_SetMute_1 ??= LoadCoreFunction<ChannelGroup_SetMute_Delegate_1>("FMOD5_ChannelGroup_SetMute");
            return s_ChannelGroup_SetMute_1(channelgroup, mute);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT ChannelGroup_GetMute(IntPtr channelgroup, out bool mute)
        {
            s_ChannelGroup_GetMute_1 ??= LoadCoreFunction<ChannelGroup_GetMute_Delegate_1>("FMOD5_ChannelGroup_GetMute");
            return s_ChannelGroup_GetMute_1(channelgroup, out mute);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT ChannelGroup_SetReverbProperties(IntPtr channelgroup, int instance, float wet)
        {
            s_ChannelGroup_SetReverbProperties_1 ??= LoadCoreFunction<ChannelGroup_SetReverbProperties_Delegate_1>("FMOD5_ChannelGroup_SetReverbProperties");
            return s_ChannelGroup_SetReverbProperties_1(channelgroup, instance, wet);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT ChannelGroup_GetReverbProperties(IntPtr channelgroup, int instance, out float wet)
        {
            s_ChannelGroup_GetReverbProperties_1 ??= LoadCoreFunction<ChannelGroup_GetReverbProperties_Delegate_1>("FMOD5_ChannelGroup_GetReverbProperties");
            return s_ChannelGroup_GetReverbProperties_1(channelgroup, instance, out wet);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT ChannelGroup_SetLowPassGain(IntPtr channelgroup, float gain)
        {
            s_ChannelGroup_SetLowPassGain_1 ??= LoadCoreFunction<ChannelGroup_SetLowPassGain_Delegate_1>("FMOD5_ChannelGroup_SetLowPassGain");
            return s_ChannelGroup_SetLowPassGain_1(channelgroup, gain);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT ChannelGroup_GetLowPassGain(IntPtr channelgroup, out float gain)
        {
            s_ChannelGroup_GetLowPassGain_1 ??= LoadCoreFunction<ChannelGroup_GetLowPassGain_Delegate_1>("FMOD5_ChannelGroup_GetLowPassGain");
            return s_ChannelGroup_GetLowPassGain_1(channelgroup, out gain);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT ChannelGroup_SetMode(IntPtr channelgroup, MODE mode)
        {
            s_ChannelGroup_SetMode_1 ??= LoadCoreFunction<ChannelGroup_SetMode_Delegate_1>("FMOD5_ChannelGroup_SetMode");
            return s_ChannelGroup_SetMode_1(channelgroup, mode);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT ChannelGroup_GetMode(IntPtr channelgroup, out MODE mode)
        {
            s_ChannelGroup_GetMode_1 ??= LoadCoreFunction<ChannelGroup_GetMode_Delegate_1>("FMOD5_ChannelGroup_GetMode");
            return s_ChannelGroup_GetMode_1(channelgroup, out mode);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT ChannelGroup_SetCallback(IntPtr channelgroup, CHANNELCONTROL_CALLBACK callback)
        {
            s_ChannelGroup_SetCallback_1 ??= LoadCoreFunction<ChannelGroup_SetCallback_Delegate_1>("FMOD5_ChannelGroup_SetCallback");
            return s_ChannelGroup_SetCallback_1(channelgroup, callback);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT ChannelGroup_IsPlaying(IntPtr channelgroup, out bool isplaying)
        {
            s_ChannelGroup_IsPlaying_1 ??= LoadCoreFunction<ChannelGroup_IsPlaying_Delegate_1>("FMOD5_ChannelGroup_IsPlaying");
            return s_ChannelGroup_IsPlaying_1(channelgroup, out isplaying);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT ChannelGroup_SetPan(IntPtr channelgroup, float pan)
        {
            s_ChannelGroup_SetPan_1 ??= LoadCoreFunction<ChannelGroup_SetPan_Delegate_1>("FMOD5_ChannelGroup_SetPan");
            return s_ChannelGroup_SetPan_1(channelgroup, pan);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT ChannelGroup_SetMixLevelsOutput(IntPtr channelgroup, float frontleft, float frontright, float center, float lfe, float surroundleft, float surroundright, float backleft, float backright)
        {
            s_ChannelGroup_SetMixLevelsOutput_1 ??= LoadCoreFunction<ChannelGroup_SetMixLevelsOutput_Delegate_1>("FMOD5_ChannelGroup_SetMixLevelsOutput");
            return s_ChannelGroup_SetMixLevelsOutput_1(channelgroup, frontleft, frontright, center, lfe, surroundleft, surroundright, backleft, backright);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT ChannelGroup_SetMixLevelsInput(IntPtr channelgroup, float[] levels, int numlevels)
        {
            s_ChannelGroup_SetMixLevelsInput_1 ??= LoadCoreFunction<ChannelGroup_SetMixLevelsInput_Delegate_1>("FMOD5_ChannelGroup_SetMixLevelsInput");
            return s_ChannelGroup_SetMixLevelsInput_1(channelgroup, levels, numlevels);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT ChannelGroup_SetMixMatrix(IntPtr channelgroup, float[] matrix, int outchannels, int inchannels, int inchannel_hop)
        {
            s_ChannelGroup_SetMixMatrix_1 ??= LoadCoreFunction<ChannelGroup_SetMixMatrix_Delegate_1>("FMOD5_ChannelGroup_SetMixMatrix");
            return s_ChannelGroup_SetMixMatrix_1(channelgroup, matrix, outchannels, inchannels, inchannel_hop);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT ChannelGroup_GetMixMatrix(IntPtr channelgroup, float[] matrix, out int outchannels, out int inchannels, int inchannel_hop)
        {
            s_ChannelGroup_GetMixMatrix_1 ??= LoadCoreFunction<ChannelGroup_GetMixMatrix_Delegate_1>("FMOD5_ChannelGroup_GetMixMatrix");
            return s_ChannelGroup_GetMixMatrix_1(channelgroup, matrix, out outchannels, out inchannels, inchannel_hop);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT ChannelGroup_GetDSPClock(IntPtr channelgroup, out ulong dspclock, out ulong parentclock)
        {
            s_ChannelGroup_GetDSPClock_1 ??= LoadCoreFunction<ChannelGroup_GetDSPClock_Delegate_1>("FMOD5_ChannelGroup_GetDSPClock");
            return s_ChannelGroup_GetDSPClock_1(channelgroup, out dspclock, out parentclock);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT ChannelGroup_SetDelay(IntPtr channelgroup, ulong dspclock_start, ulong dspclock_end, bool stopchannels)
        {
            s_ChannelGroup_SetDelay_1 ??= LoadCoreFunction<ChannelGroup_SetDelay_Delegate_1>("FMOD5_ChannelGroup_SetDelay");
            return s_ChannelGroup_SetDelay_1(channelgroup, dspclock_start, dspclock_end, stopchannels);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT ChannelGroup_GetDelay(IntPtr channelgroup, out ulong dspclock_start, out ulong dspclock_end, IntPtr zero)
        {
            s_ChannelGroup_GetDelay_1 ??= LoadCoreFunction<ChannelGroup_GetDelay_Delegate_1>("FMOD5_ChannelGroup_GetDelay");
            return s_ChannelGroup_GetDelay_1(channelgroup, out dspclock_start, out dspclock_end, zero);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT ChannelGroup_GetDelay(IntPtr channelgroup, out ulong dspclock_start, out ulong dspclock_end, out bool stopchannels)
        {
            s_ChannelGroup_GetDelay_2 ??= LoadCoreFunction<ChannelGroup_GetDelay_Delegate_2>("FMOD5_ChannelGroup_GetDelay");
            return s_ChannelGroup_GetDelay_2(channelgroup, out dspclock_start, out dspclock_end, out stopchannels);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT ChannelGroup_AddFadePoint(IntPtr channelgroup, ulong dspclock, float volume)
        {
            s_ChannelGroup_AddFadePoint_1 ??= LoadCoreFunction<ChannelGroup_AddFadePoint_Delegate_1>("FMOD5_ChannelGroup_AddFadePoint");
            return s_ChannelGroup_AddFadePoint_1(channelgroup, dspclock, volume);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT ChannelGroup_SetFadePointRamp(IntPtr channelgroup, ulong dspclock, float volume)
        {
            s_ChannelGroup_SetFadePointRamp_1 ??= LoadCoreFunction<ChannelGroup_SetFadePointRamp_Delegate_1>("FMOD5_ChannelGroup_SetFadePointRamp");
            return s_ChannelGroup_SetFadePointRamp_1(channelgroup, dspclock, volume);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT ChannelGroup_RemoveFadePoints(IntPtr channelgroup, ulong dspclock_start, ulong dspclock_end)
        {
            s_ChannelGroup_RemoveFadePoints_1 ??= LoadCoreFunction<ChannelGroup_RemoveFadePoints_Delegate_1>("FMOD5_ChannelGroup_RemoveFadePoints");
            return s_ChannelGroup_RemoveFadePoints_1(channelgroup, dspclock_start, dspclock_end);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT ChannelGroup_GetFadePoints(IntPtr channelgroup, ref uint numpoints, ulong[] point_dspclock, float[] point_volume)
        {
            s_ChannelGroup_GetFadePoints_1 ??= LoadCoreFunction<ChannelGroup_GetFadePoints_Delegate_1>("FMOD5_ChannelGroup_GetFadePoints");
            return s_ChannelGroup_GetFadePoints_1(channelgroup, ref numpoints, point_dspclock, point_volume);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT ChannelGroup_GetDSP(IntPtr channelgroup, int index, out IntPtr dsp)
        {
            s_ChannelGroup_GetDSP_1 ??= LoadCoreFunction<ChannelGroup_GetDSP_Delegate_1>("FMOD5_ChannelGroup_GetDSP");
            return s_ChannelGroup_GetDSP_1(channelgroup, index, out dsp);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT ChannelGroup_AddDSP(IntPtr channelgroup, int index, IntPtr dsp)
        {
            s_ChannelGroup_AddDSP_1 ??= LoadCoreFunction<ChannelGroup_AddDSP_Delegate_1>("FMOD5_ChannelGroup_AddDSP");
            return s_ChannelGroup_AddDSP_1(channelgroup, index, dsp);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT ChannelGroup_RemoveDSP(IntPtr channelgroup, IntPtr dsp)
        {
            s_ChannelGroup_RemoveDSP_1 ??= LoadCoreFunction<ChannelGroup_RemoveDSP_Delegate_1>("FMOD5_ChannelGroup_RemoveDSP");
            return s_ChannelGroup_RemoveDSP_1(channelgroup, dsp);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT ChannelGroup_GetNumDSPs(IntPtr channelgroup, out int numdsps)
        {
            s_ChannelGroup_GetNumDSPs_1 ??= LoadCoreFunction<ChannelGroup_GetNumDSPs_Delegate_1>("FMOD5_ChannelGroup_GetNumDSPs");
            return s_ChannelGroup_GetNumDSPs_1(channelgroup, out numdsps);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT ChannelGroup_SetDSPIndex(IntPtr channelgroup, IntPtr dsp, int index)
        {
            s_ChannelGroup_SetDSPIndex_1 ??= LoadCoreFunction<ChannelGroup_SetDSPIndex_Delegate_1>("FMOD5_ChannelGroup_SetDSPIndex");
            return s_ChannelGroup_SetDSPIndex_1(channelgroup, dsp, index);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT ChannelGroup_GetDSPIndex(IntPtr channelgroup, IntPtr dsp, out int index)
        {
            s_ChannelGroup_GetDSPIndex_1 ??= LoadCoreFunction<ChannelGroup_GetDSPIndex_Delegate_1>("FMOD5_ChannelGroup_GetDSPIndex");
            return s_ChannelGroup_GetDSPIndex_1(channelgroup, dsp, out index);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT ChannelGroup_Set3DAttributes(IntPtr channelgroup, ref VECTOR pos, ref VECTOR vel)
        {
            s_ChannelGroup_Set3DAttributes_1 ??= LoadCoreFunction<ChannelGroup_Set3DAttributes_Delegate_1>("FMOD5_ChannelGroup_Set3DAttributes");
            return s_ChannelGroup_Set3DAttributes_1(channelgroup, ref pos, ref vel);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT ChannelGroup_Get3DAttributes(IntPtr channelgroup, out VECTOR pos, out VECTOR vel)
        {
            s_ChannelGroup_Get3DAttributes_1 ??= LoadCoreFunction<ChannelGroup_Get3DAttributes_Delegate_1>("FMOD5_ChannelGroup_Get3DAttributes");
            return s_ChannelGroup_Get3DAttributes_1(channelgroup, out pos, out vel);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT ChannelGroup_Set3DMinMaxDistance(IntPtr channelgroup, float mindistance, float maxdistance)
        {
            s_ChannelGroup_Set3DMinMaxDistance_1 ??= LoadCoreFunction<ChannelGroup_Set3DMinMaxDistance_Delegate_1>("FMOD5_ChannelGroup_Set3DMinMaxDistance");
            return s_ChannelGroup_Set3DMinMaxDistance_1(channelgroup, mindistance, maxdistance);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT ChannelGroup_Get3DMinMaxDistance(IntPtr channelgroup, out float mindistance, out float maxdistance)
        {
            s_ChannelGroup_Get3DMinMaxDistance_1 ??= LoadCoreFunction<ChannelGroup_Get3DMinMaxDistance_Delegate_1>("FMOD5_ChannelGroup_Get3DMinMaxDistance");
            return s_ChannelGroup_Get3DMinMaxDistance_1(channelgroup, out mindistance, out maxdistance);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT ChannelGroup_Set3DConeSettings(IntPtr channelgroup, float insideconeangle, float outsideconeangle, float outsidevolume)
        {
            s_ChannelGroup_Set3DConeSettings_1 ??= LoadCoreFunction<ChannelGroup_Set3DConeSettings_Delegate_1>("FMOD5_ChannelGroup_Set3DConeSettings");
            return s_ChannelGroup_Set3DConeSettings_1(channelgroup, insideconeangle, outsideconeangle, outsidevolume);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT ChannelGroup_Get3DConeSettings(IntPtr channelgroup, out float insideconeangle, out float outsideconeangle, out float outsidevolume)
        {
            s_ChannelGroup_Get3DConeSettings_1 ??= LoadCoreFunction<ChannelGroup_Get3DConeSettings_Delegate_1>("FMOD5_ChannelGroup_Get3DConeSettings");
            return s_ChannelGroup_Get3DConeSettings_1(channelgroup, out insideconeangle, out outsideconeangle, out outsidevolume);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT ChannelGroup_Set3DConeOrientation(IntPtr channelgroup, ref VECTOR orientation)
        {
            s_ChannelGroup_Set3DConeOrientation_1 ??= LoadCoreFunction<ChannelGroup_Set3DConeOrientation_Delegate_1>("FMOD5_ChannelGroup_Set3DConeOrientation");
            return s_ChannelGroup_Set3DConeOrientation_1(channelgroup, ref orientation);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT ChannelGroup_Get3DConeOrientation(IntPtr channelgroup, out VECTOR orientation)
        {
            s_ChannelGroup_Get3DConeOrientation_1 ??= LoadCoreFunction<ChannelGroup_Get3DConeOrientation_Delegate_1>("FMOD5_ChannelGroup_Get3DConeOrientation");
            return s_ChannelGroup_Get3DConeOrientation_1(channelgroup, out orientation);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT ChannelGroup_Set3DCustomRolloff(IntPtr channelgroup, ref VECTOR points, int numpoints)
        {
            s_ChannelGroup_Set3DCustomRolloff_1 ??= LoadCoreFunction<ChannelGroup_Set3DCustomRolloff_Delegate_1>("FMOD5_ChannelGroup_Set3DCustomRolloff");
            return s_ChannelGroup_Set3DCustomRolloff_1(channelgroup, ref points, numpoints);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT ChannelGroup_Get3DCustomRolloff(IntPtr channelgroup, out IntPtr points, out int numpoints)
        {
            s_ChannelGroup_Get3DCustomRolloff_1 ??= LoadCoreFunction<ChannelGroup_Get3DCustomRolloff_Delegate_1>("FMOD5_ChannelGroup_Get3DCustomRolloff");
            return s_ChannelGroup_Get3DCustomRolloff_1(channelgroup, out points, out numpoints);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT ChannelGroup_Set3DOcclusion(IntPtr channelgroup, float directocclusion, float reverbocclusion)
        {
            s_ChannelGroup_Set3DOcclusion_1 ??= LoadCoreFunction<ChannelGroup_Set3DOcclusion_Delegate_1>("FMOD5_ChannelGroup_Set3DOcclusion");
            return s_ChannelGroup_Set3DOcclusion_1(channelgroup, directocclusion, reverbocclusion);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT ChannelGroup_Get3DOcclusion(IntPtr channelgroup, out float directocclusion, out float reverbocclusion)
        {
            s_ChannelGroup_Get3DOcclusion_1 ??= LoadCoreFunction<ChannelGroup_Get3DOcclusion_Delegate_1>("FMOD5_ChannelGroup_Get3DOcclusion");
            return s_ChannelGroup_Get3DOcclusion_1(channelgroup, out directocclusion, out reverbocclusion);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT ChannelGroup_Set3DSpread(IntPtr channelgroup, float angle)
        {
            s_ChannelGroup_Set3DSpread_1 ??= LoadCoreFunction<ChannelGroup_Set3DSpread_Delegate_1>("FMOD5_ChannelGroup_Set3DSpread");
            return s_ChannelGroup_Set3DSpread_1(channelgroup, angle);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT ChannelGroup_Get3DSpread(IntPtr channelgroup, out float angle)
        {
            s_ChannelGroup_Get3DSpread_1 ??= LoadCoreFunction<ChannelGroup_Get3DSpread_Delegate_1>("FMOD5_ChannelGroup_Get3DSpread");
            return s_ChannelGroup_Get3DSpread_1(channelgroup, out angle);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT ChannelGroup_Set3DLevel(IntPtr channelgroup, float level)
        {
            s_ChannelGroup_Set3DLevel_1 ??= LoadCoreFunction<ChannelGroup_Set3DLevel_Delegate_1>("FMOD5_ChannelGroup_Set3DLevel");
            return s_ChannelGroup_Set3DLevel_1(channelgroup, level);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT ChannelGroup_Get3DLevel(IntPtr channelgroup, out float level)
        {
            s_ChannelGroup_Get3DLevel_1 ??= LoadCoreFunction<ChannelGroup_Get3DLevel_Delegate_1>("FMOD5_ChannelGroup_Get3DLevel");
            return s_ChannelGroup_Get3DLevel_1(channelgroup, out level);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT ChannelGroup_Set3DDopplerLevel(IntPtr channelgroup, float level)
        {
            s_ChannelGroup_Set3DDopplerLevel_1 ??= LoadCoreFunction<ChannelGroup_Set3DDopplerLevel_Delegate_1>("FMOD5_ChannelGroup_Set3DDopplerLevel");
            return s_ChannelGroup_Set3DDopplerLevel_1(channelgroup, level);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT ChannelGroup_Get3DDopplerLevel(IntPtr channelgroup, out float level)
        {
            s_ChannelGroup_Get3DDopplerLevel_1 ??= LoadCoreFunction<ChannelGroup_Get3DDopplerLevel_Delegate_1>("FMOD5_ChannelGroup_Get3DDopplerLevel");
            return s_ChannelGroup_Get3DDopplerLevel_1(channelgroup, out level);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT ChannelGroup_Set3DDistanceFilter(IntPtr channelgroup, bool custom, float customLevel, float centerFreq)
        {
            s_ChannelGroup_Set3DDistanceFilter_1 ??= LoadCoreFunction<ChannelGroup_Set3DDistanceFilter_Delegate_1>("FMOD5_ChannelGroup_Set3DDistanceFilter");
            return s_ChannelGroup_Set3DDistanceFilter_1(channelgroup, custom, customLevel, centerFreq);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT ChannelGroup_Get3DDistanceFilter(IntPtr channelgroup, out bool custom, out float customLevel, out float centerFreq)
        {
            s_ChannelGroup_Get3DDistanceFilter_1 ??= LoadCoreFunction<ChannelGroup_Get3DDistanceFilter_Delegate_1>("FMOD5_ChannelGroup_Get3DDistanceFilter");
            return s_ChannelGroup_Get3DDistanceFilter_1(channelgroup, out custom, out customLevel, out centerFreq);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT ChannelGroup_SetUserData(IntPtr channelgroup, IntPtr userdata)
        {
            s_ChannelGroup_SetUserData_1 ??= LoadCoreFunction<ChannelGroup_SetUserData_Delegate_1>("FMOD5_ChannelGroup_SetUserData");
            return s_ChannelGroup_SetUserData_1(channelgroup, userdata);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT ChannelGroup_GetUserData(IntPtr channelgroup, out IntPtr userdata)
        {
            s_ChannelGroup_GetUserData_1 ??= LoadCoreFunction<ChannelGroup_GetUserData_Delegate_1>("FMOD5_ChannelGroup_GetUserData");
            return s_ChannelGroup_GetUserData_1(channelgroup, out userdata);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT SoundGroup_Release(IntPtr soundgroup)
        {
            s_SoundGroup_Release_1 ??= LoadCoreFunction<SoundGroup_Release_Delegate_1>("FMOD5_SoundGroup_Release");
            return s_SoundGroup_Release_1(soundgroup);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT SoundGroup_GetSystemObject(IntPtr soundgroup, out IntPtr system)
        {
            s_SoundGroup_GetSystemObject_1 ??= LoadCoreFunction<SoundGroup_GetSystemObject_Delegate_1>("FMOD5_SoundGroup_GetSystemObject");
            return s_SoundGroup_GetSystemObject_1(soundgroup, out system);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT SoundGroup_SetMaxAudible(IntPtr soundgroup, int maxaudible)
        {
            s_SoundGroup_SetMaxAudible_1 ??= LoadCoreFunction<SoundGroup_SetMaxAudible_Delegate_1>("FMOD5_SoundGroup_SetMaxAudible");
            return s_SoundGroup_SetMaxAudible_1(soundgroup, maxaudible);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT SoundGroup_GetMaxAudible(IntPtr soundgroup, out int maxaudible)
        {
            s_SoundGroup_GetMaxAudible_1 ??= LoadCoreFunction<SoundGroup_GetMaxAudible_Delegate_1>("FMOD5_SoundGroup_GetMaxAudible");
            return s_SoundGroup_GetMaxAudible_1(soundgroup, out maxaudible);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT SoundGroup_SetMaxAudibleBehavior(IntPtr soundgroup, SOUNDGROUP_BEHAVIOR behavior)
        {
            s_SoundGroup_SetMaxAudibleBehavior_1 ??= LoadCoreFunction<SoundGroup_SetMaxAudibleBehavior_Delegate_1>("FMOD5_SoundGroup_SetMaxAudibleBehavior");
            return s_SoundGroup_SetMaxAudibleBehavior_1(soundgroup, behavior);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT SoundGroup_GetMaxAudibleBehavior(IntPtr soundgroup, out SOUNDGROUP_BEHAVIOR behavior)
        {
            s_SoundGroup_GetMaxAudibleBehavior_1 ??= LoadCoreFunction<SoundGroup_GetMaxAudibleBehavior_Delegate_1>("FMOD5_SoundGroup_GetMaxAudibleBehavior");
            return s_SoundGroup_GetMaxAudibleBehavior_1(soundgroup, out behavior);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT SoundGroup_SetMuteFadeSpeed(IntPtr soundgroup, float speed)
        {
            s_SoundGroup_SetMuteFadeSpeed_1 ??= LoadCoreFunction<SoundGroup_SetMuteFadeSpeed_Delegate_1>("FMOD5_SoundGroup_SetMuteFadeSpeed");
            return s_SoundGroup_SetMuteFadeSpeed_1(soundgroup, speed);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT SoundGroup_GetMuteFadeSpeed(IntPtr soundgroup, out float speed)
        {
            s_SoundGroup_GetMuteFadeSpeed_1 ??= LoadCoreFunction<SoundGroup_GetMuteFadeSpeed_Delegate_1>("FMOD5_SoundGroup_GetMuteFadeSpeed");
            return s_SoundGroup_GetMuteFadeSpeed_1(soundgroup, out speed);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT SoundGroup_SetVolume(IntPtr soundgroup, float volume)
        {
            s_SoundGroup_SetVolume_1 ??= LoadCoreFunction<SoundGroup_SetVolume_Delegate_1>("FMOD5_SoundGroup_SetVolume");
            return s_SoundGroup_SetVolume_1(soundgroup, volume);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT SoundGroup_GetVolume(IntPtr soundgroup, out float volume)
        {
            s_SoundGroup_GetVolume_1 ??= LoadCoreFunction<SoundGroup_GetVolume_Delegate_1>("FMOD5_SoundGroup_GetVolume");
            return s_SoundGroup_GetVolume_1(soundgroup, out volume);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT SoundGroup_Stop(IntPtr soundgroup)
        {
            s_SoundGroup_Stop_1 ??= LoadCoreFunction<SoundGroup_Stop_Delegate_1>("FMOD5_SoundGroup_Stop");
            return s_SoundGroup_Stop_1(soundgroup);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT SoundGroup_GetName(IntPtr soundgroup, IntPtr name, int namelen)
        {
            s_SoundGroup_GetName_1 ??= LoadCoreFunction<SoundGroup_GetName_Delegate_1>("FMOD5_SoundGroup_GetName");
            return s_SoundGroup_GetName_1(soundgroup, name, namelen);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT SoundGroup_GetNumSounds(IntPtr soundgroup, out int numsounds)
        {
            s_SoundGroup_GetNumSounds_1 ??= LoadCoreFunction<SoundGroup_GetNumSounds_Delegate_1>("FMOD5_SoundGroup_GetNumSounds");
            return s_SoundGroup_GetNumSounds_1(soundgroup, out numsounds);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT SoundGroup_GetSound(IntPtr soundgroup, int index, out IntPtr sound)
        {
            s_SoundGroup_GetSound_1 ??= LoadCoreFunction<SoundGroup_GetSound_Delegate_1>("FMOD5_SoundGroup_GetSound");
            return s_SoundGroup_GetSound_1(soundgroup, index, out sound);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT SoundGroup_GetNumPlaying(IntPtr soundgroup, out int numplaying)
        {
            s_SoundGroup_GetNumPlaying_1 ??= LoadCoreFunction<SoundGroup_GetNumPlaying_Delegate_1>("FMOD5_SoundGroup_GetNumPlaying");
            return s_SoundGroup_GetNumPlaying_1(soundgroup, out numplaying);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT SoundGroup_SetUserData(IntPtr soundgroup, IntPtr userdata)
        {
            s_SoundGroup_SetUserData_1 ??= LoadCoreFunction<SoundGroup_SetUserData_Delegate_1>("FMOD5_SoundGroup_SetUserData");
            return s_SoundGroup_SetUserData_1(soundgroup, userdata);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT SoundGroup_GetUserData(IntPtr soundgroup, out IntPtr userdata)
        {
            s_SoundGroup_GetUserData_1 ??= LoadCoreFunction<SoundGroup_GetUserData_Delegate_1>("FMOD5_SoundGroup_GetUserData");
            return s_SoundGroup_GetUserData_1(soundgroup, out userdata);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT DSP_Release(IntPtr dsp)
        {
            s_DSP_Release_1 ??= LoadCoreFunction<DSP_Release_Delegate_1>("FMOD5_DSP_Release");
            return s_DSP_Release_1(dsp);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT DSP_GetSystemObject(IntPtr dsp, out IntPtr system)
        {
            s_DSP_GetSystemObject_1 ??= LoadCoreFunction<DSP_GetSystemObject_Delegate_1>("FMOD5_DSP_GetSystemObject");
            return s_DSP_GetSystemObject_1(dsp, out system);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT DSP_AddInput(IntPtr dsp, IntPtr input, IntPtr zero, DSPCONNECTION_TYPE type)
        {
            s_DSP_AddInput_1 ??= LoadCoreFunction<DSP_AddInput_Delegate_1>("FMOD5_DSP_AddInput");
            return s_DSP_AddInput_1(dsp, input, zero, type);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT DSP_AddInput(IntPtr dsp, IntPtr input, out IntPtr connection, DSPCONNECTION_TYPE type)
        {
            s_DSP_AddInput_2 ??= LoadCoreFunction<DSP_AddInput_Delegate_2>("FMOD5_DSP_AddInput");
            return s_DSP_AddInput_2(dsp, input, out connection, type);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT DSP_AddInputPreallocated(IntPtr dsp, IntPtr input, out IntPtr connection)
        {
            s_DSP_AddInputPreallocated_1 ??= LoadCoreFunction<DSP_AddInputPreallocated_Delegate_1>("FMOD5_DSP_AddInputPreallocated");
            return s_DSP_AddInputPreallocated_1(dsp, input, out connection);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT DSP_DisconnectFrom(IntPtr dsp, IntPtr target, IntPtr connection)
        {
            s_DSP_DisconnectFrom_1 ??= LoadCoreFunction<DSP_DisconnectFrom_Delegate_1>("FMOD5_DSP_DisconnectFrom");
            return s_DSP_DisconnectFrom_1(dsp, target, connection);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT DSP_DisconnectAll(IntPtr dsp, bool inputs, bool outputs)
        {
            s_DSP_DisconnectAll_1 ??= LoadCoreFunction<DSP_DisconnectAll_Delegate_1>("FMOD5_DSP_DisconnectAll");
            return s_DSP_DisconnectAll_1(dsp, inputs, outputs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT DSP_GetNumInputs(IntPtr dsp, out int numinputs)
        {
            s_DSP_GetNumInputs_1 ??= LoadCoreFunction<DSP_GetNumInputs_Delegate_1>("FMOD5_DSP_GetNumInputs");
            return s_DSP_GetNumInputs_1(dsp, out numinputs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT DSP_GetNumOutputs(IntPtr dsp, out int numoutputs)
        {
            s_DSP_GetNumOutputs_1 ??= LoadCoreFunction<DSP_GetNumOutputs_Delegate_1>("FMOD5_DSP_GetNumOutputs");
            return s_DSP_GetNumOutputs_1(dsp, out numoutputs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT DSP_GetInput(IntPtr dsp, int index, out IntPtr input, out IntPtr inputconnection)
        {
            s_DSP_GetInput_1 ??= LoadCoreFunction<DSP_GetInput_Delegate_1>("FMOD5_DSP_GetInput");
            return s_DSP_GetInput_1(dsp, index, out input, out inputconnection);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT DSP_GetOutput(IntPtr dsp, int index, out IntPtr output, out IntPtr outputconnection)
        {
            s_DSP_GetOutput_1 ??= LoadCoreFunction<DSP_GetOutput_Delegate_1>("FMOD5_DSP_GetOutput");
            return s_DSP_GetOutput_1(dsp, index, out output, out outputconnection);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT DSP_SetActive(IntPtr dsp, bool active)
        {
            s_DSP_SetActive_1 ??= LoadCoreFunction<DSP_SetActive_Delegate_1>("FMOD5_DSP_SetActive");
            return s_DSP_SetActive_1(dsp, active);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT DSP_GetActive(IntPtr dsp, out bool active)
        {
            s_DSP_GetActive_1 ??= LoadCoreFunction<DSP_GetActive_Delegate_1>("FMOD5_DSP_GetActive");
            return s_DSP_GetActive_1(dsp, out active);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT DSP_SetBypass(IntPtr dsp, bool bypass)
        {
            s_DSP_SetBypass_1 ??= LoadCoreFunction<DSP_SetBypass_Delegate_1>("FMOD5_DSP_SetBypass");
            return s_DSP_SetBypass_1(dsp, bypass);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT DSP_GetBypass(IntPtr dsp, out bool bypass)
        {
            s_DSP_GetBypass_1 ??= LoadCoreFunction<DSP_GetBypass_Delegate_1>("FMOD5_DSP_GetBypass");
            return s_DSP_GetBypass_1(dsp, out bypass);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT DSP_SetWetDryMix(IntPtr dsp, float prewet, float postwet, float dry)
        {
            s_DSP_SetWetDryMix_1 ??= LoadCoreFunction<DSP_SetWetDryMix_Delegate_1>("FMOD5_DSP_SetWetDryMix");
            return s_DSP_SetWetDryMix_1(dsp, prewet, postwet, dry);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT DSP_GetWetDryMix(IntPtr dsp, out float prewet, out float postwet, out float dry)
        {
            s_DSP_GetWetDryMix_1 ??= LoadCoreFunction<DSP_GetWetDryMix_Delegate_1>("FMOD5_DSP_GetWetDryMix");
            return s_DSP_GetWetDryMix_1(dsp, out prewet, out postwet, out dry);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT DSP_SetChannelFormat(IntPtr dsp, CHANNELMASK channelmask, int numchannels, SPEAKERMODE source_speakermode)
        {
            s_DSP_SetChannelFormat_1 ??= LoadCoreFunction<DSP_SetChannelFormat_Delegate_1>("FMOD5_DSP_SetChannelFormat");
            return s_DSP_SetChannelFormat_1(dsp, channelmask, numchannels, source_speakermode);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT DSP_GetChannelFormat(IntPtr dsp, out CHANNELMASK channelmask, out int numchannels, out SPEAKERMODE source_speakermode)
        {
            s_DSP_GetChannelFormat_1 ??= LoadCoreFunction<DSP_GetChannelFormat_Delegate_1>("FMOD5_DSP_GetChannelFormat");
            return s_DSP_GetChannelFormat_1(dsp, out channelmask, out numchannels, out source_speakermode);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT DSP_GetOutputChannelFormat(IntPtr dsp, CHANNELMASK inmask, int inchannels, SPEAKERMODE inspeakermode, out CHANNELMASK outmask, out int outchannels, out SPEAKERMODE outspeakermode)
        {
            s_DSP_GetOutputChannelFormat_1 ??= LoadCoreFunction<DSP_GetOutputChannelFormat_Delegate_1>("FMOD5_DSP_GetOutputChannelFormat");
            return s_DSP_GetOutputChannelFormat_1(dsp, inmask, inchannels, inspeakermode, out outmask, out outchannels, out outspeakermode);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT DSP_Reset(IntPtr dsp)
        {
            s_DSP_Reset_1 ??= LoadCoreFunction<DSP_Reset_Delegate_1>("FMOD5_DSP_Reset");
            return s_DSP_Reset_1(dsp);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT DSP_SetCallback(IntPtr dsp, DSP_CALLBACK callback)
        {
            s_DSP_SetCallback_1 ??= LoadCoreFunction<DSP_SetCallback_Delegate_1>("FMOD5_DSP_SetCallback");
            return s_DSP_SetCallback_1(dsp, callback);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT DSP_SetParameterFloat(IntPtr dsp, int index, float value)
        {
            s_DSP_SetParameterFloat_1 ??= LoadCoreFunction<DSP_SetParameterFloat_Delegate_1>("FMOD5_DSP_SetParameterFloat");
            return s_DSP_SetParameterFloat_1(dsp, index, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT DSP_SetParameterInt(IntPtr dsp, int index, int value)
        {
            s_DSP_SetParameterInt_1 ??= LoadCoreFunction<DSP_SetParameterInt_Delegate_1>("FMOD5_DSP_SetParameterInt");
            return s_DSP_SetParameterInt_1(dsp, index, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT DSP_SetParameterBool(IntPtr dsp, int index, bool value)
        {
            s_DSP_SetParameterBool_1 ??= LoadCoreFunction<DSP_SetParameterBool_Delegate_1>("FMOD5_DSP_SetParameterBool");
            return s_DSP_SetParameterBool_1(dsp, index, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT DSP_SetParameterData(IntPtr dsp, int index, byte[] data, uint length)
        {
            s_DSP_SetParameterData_1 ??= LoadCoreFunction<DSP_SetParameterData_Delegate_1>("FMOD5_DSP_SetParameterData");
            return s_DSP_SetParameterData_1(dsp, index, data, length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT DSP_GetParameterFloat(IntPtr dsp, int index, out float value, IntPtr valuestr, int valuestrlen)
        {
            s_DSP_GetParameterFloat_1 ??= LoadCoreFunction<DSP_GetParameterFloat_Delegate_1>("FMOD5_DSP_GetParameterFloat");
            return s_DSP_GetParameterFloat_1(dsp, index, out value, valuestr, valuestrlen);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT DSP_GetParameterInt(IntPtr dsp, int index, out int value, IntPtr valuestr, int valuestrlen)
        {
            s_DSP_GetParameterInt_1 ??= LoadCoreFunction<DSP_GetParameterInt_Delegate_1>("FMOD5_DSP_GetParameterInt");
            return s_DSP_GetParameterInt_1(dsp, index, out value, valuestr, valuestrlen);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT DSP_GetParameterBool(IntPtr dsp, int index, out bool value, IntPtr valuestr, int valuestrlen)
        {
            s_DSP_GetParameterBool_1 ??= LoadCoreFunction<DSP_GetParameterBool_Delegate_1>("FMOD5_DSP_GetParameterBool");
            return s_DSP_GetParameterBool_1(dsp, index, out value, valuestr, valuestrlen);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT DSP_GetParameterData(IntPtr dsp, int index, out IntPtr data, out uint length, IntPtr valuestr, int valuestrlen)
        {
            s_DSP_GetParameterData_1 ??= LoadCoreFunction<DSP_GetParameterData_Delegate_1>("FMOD5_DSP_GetParameterData");
            return s_DSP_GetParameterData_1(dsp, index, out data, out length, valuestr, valuestrlen);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT DSP_GetNumParameters(IntPtr dsp, out int numparams)
        {
            s_DSP_GetNumParameters_1 ??= LoadCoreFunction<DSP_GetNumParameters_Delegate_1>("FMOD5_DSP_GetNumParameters");
            return s_DSP_GetNumParameters_1(dsp, out numparams);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT DSP_GetParameterInfo(IntPtr dsp, int index, out IntPtr desc)
        {
            s_DSP_GetParameterInfo_1 ??= LoadCoreFunction<DSP_GetParameterInfo_Delegate_1>("FMOD5_DSP_GetParameterInfo");
            return s_DSP_GetParameterInfo_1(dsp, index, out desc);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT DSP_GetDataParameterIndex(IntPtr dsp, int datatype, out int index)
        {
            s_DSP_GetDataParameterIndex_1 ??= LoadCoreFunction<DSP_GetDataParameterIndex_Delegate_1>("FMOD5_DSP_GetDataParameterIndex");
            return s_DSP_GetDataParameterIndex_1(dsp, datatype, out index);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT DSP_ShowConfigDialog(IntPtr dsp, IntPtr hwnd, bool show)
        {
            s_DSP_ShowConfigDialog_1 ??= LoadCoreFunction<DSP_ShowConfigDialog_Delegate_1>("FMOD5_DSP_ShowConfigDialog");
            return s_DSP_ShowConfigDialog_1(dsp, hwnd, show);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT DSP_GetInfo(IntPtr dsp, IntPtr name, out uint version, out int channels, out int configwidth, out int configheight)
        {
            s_DSP_GetInfo_1 ??= LoadCoreFunction<DSP_GetInfo_Delegate_1>("FMOD5_DSP_GetInfo");
            return s_DSP_GetInfo_1(dsp, name, out version, out channels, out configwidth, out configheight);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT DSP_GetType(IntPtr dsp, out DSP_TYPE type)
        {
            s_DSP_GetType_1 ??= LoadCoreFunction<DSP_GetType_Delegate_1>("FMOD5_DSP_GetType");
            return s_DSP_GetType_1(dsp, out type);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT DSP_GetIdle(IntPtr dsp, out bool idle)
        {
            s_DSP_GetIdle_1 ??= LoadCoreFunction<DSP_GetIdle_Delegate_1>("FMOD5_DSP_GetIdle");
            return s_DSP_GetIdle_1(dsp, out idle);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT DSP_SetUserData(IntPtr dsp, IntPtr userdata)
        {
            s_DSP_SetUserData_1 ??= LoadCoreFunction<DSP_SetUserData_Delegate_1>("FMOD5_DSP_SetUserData");
            return s_DSP_SetUserData_1(dsp, userdata);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT DSP_GetUserData(IntPtr dsp, out IntPtr userdata)
        {
            s_DSP_GetUserData_1 ??= LoadCoreFunction<DSP_GetUserData_Delegate_1>("FMOD5_DSP_GetUserData");
            return s_DSP_GetUserData_1(dsp, out userdata);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT DSPConnection_GetInput(IntPtr dspconnection, out IntPtr input)
        {
            s_DSPConnection_GetInput_1 ??= LoadCoreFunction<DSPConnection_GetInput_Delegate_1>("FMOD5_DSPConnection_GetInput");
            return s_DSPConnection_GetInput_1(dspconnection, out input);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT DSPConnection_GetOutput(IntPtr dspconnection, out IntPtr output)
        {
            s_DSPConnection_GetOutput_1 ??= LoadCoreFunction<DSPConnection_GetOutput_Delegate_1>("FMOD5_DSPConnection_GetOutput");
            return s_DSPConnection_GetOutput_1(dspconnection, out output);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT DSPConnection_SetMix(IntPtr dspconnection, float volume)
        {
            s_DSPConnection_SetMix_1 ??= LoadCoreFunction<DSPConnection_SetMix_Delegate_1>("FMOD5_DSPConnection_SetMix");
            return s_DSPConnection_SetMix_1(dspconnection, volume);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT DSPConnection_GetMix(IntPtr dspconnection, out float volume)
        {
            s_DSPConnection_GetMix_1 ??= LoadCoreFunction<DSPConnection_GetMix_Delegate_1>("FMOD5_DSPConnection_GetMix");
            return s_DSPConnection_GetMix_1(dspconnection, out volume);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT DSPConnection_SetMixMatrix(IntPtr dspconnection, float[] matrix, int outchannels, int inchannels, int inchannel_hop)
        {
            s_DSPConnection_SetMixMatrix_1 ??= LoadCoreFunction<DSPConnection_SetMixMatrix_Delegate_1>("FMOD5_DSPConnection_SetMixMatrix");
            return s_DSPConnection_SetMixMatrix_1(dspconnection, matrix, outchannels, inchannels, inchannel_hop);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT DSPConnection_GetMixMatrix(IntPtr dspconnection, float[] matrix, out int outchannels, out int inchannels, int inchannel_hop)
        {
            s_DSPConnection_GetMixMatrix_1 ??= LoadCoreFunction<DSPConnection_GetMixMatrix_Delegate_1>("FMOD5_DSPConnection_GetMixMatrix");
            return s_DSPConnection_GetMixMatrix_1(dspconnection, matrix, out outchannels, out inchannels, inchannel_hop);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT DSPConnection_GetType(IntPtr dspconnection, out DSPCONNECTION_TYPE type)
        {
            s_DSPConnection_GetType_1 ??= LoadCoreFunction<DSPConnection_GetType_Delegate_1>("FMOD5_DSPConnection_GetType");
            return s_DSPConnection_GetType_1(dspconnection, out type);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT DSPConnection_SetUserData(IntPtr dspconnection, IntPtr userdata)
        {
            s_DSPConnection_SetUserData_1 ??= LoadCoreFunction<DSPConnection_SetUserData_Delegate_1>("FMOD5_DSPConnection_SetUserData");
            return s_DSPConnection_SetUserData_1(dspconnection, userdata);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT DSPConnection_GetUserData(IntPtr dspconnection, out IntPtr userdata)
        {
            s_DSPConnection_GetUserData_1 ??= LoadCoreFunction<DSPConnection_GetUserData_Delegate_1>("FMOD5_DSPConnection_GetUserData");
            return s_DSPConnection_GetUserData_1(dspconnection, out userdata);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Geometry_Release(IntPtr geometry)
        {
            s_Geometry_Release_1 ??= LoadCoreFunction<Geometry_Release_Delegate_1>("FMOD5_Geometry_Release");
            return s_Geometry_Release_1(geometry);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Geometry_AddPolygon(IntPtr geometry, float directocclusion, float reverbocclusion, bool doublesided, int numvertices, VECTOR[] vertices, out int polygonindex)
        {
            s_Geometry_AddPolygon_1 ??= LoadCoreFunction<Geometry_AddPolygon_Delegate_1>("FMOD5_Geometry_AddPolygon");
            return s_Geometry_AddPolygon_1(geometry, directocclusion, reverbocclusion, doublesided, numvertices, vertices, out polygonindex);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Geometry_GetNumPolygons(IntPtr geometry, out int numpolygons)
        {
            s_Geometry_GetNumPolygons_1 ??= LoadCoreFunction<Geometry_GetNumPolygons_Delegate_1>("FMOD5_Geometry_GetNumPolygons");
            return s_Geometry_GetNumPolygons_1(geometry, out numpolygons);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Geometry_GetMaxPolygons(IntPtr geometry, out int maxpolygons, out int maxvertices)
        {
            s_Geometry_GetMaxPolygons_1 ??= LoadCoreFunction<Geometry_GetMaxPolygons_Delegate_1>("FMOD5_Geometry_GetMaxPolygons");
            return s_Geometry_GetMaxPolygons_1(geometry, out maxpolygons, out maxvertices);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Geometry_GetPolygonNumVertices(IntPtr geometry, int index, out int numvertices)
        {
            s_Geometry_GetPolygonNumVertices_1 ??= LoadCoreFunction<Geometry_GetPolygonNumVertices_Delegate_1>("FMOD5_Geometry_GetPolygonNumVertices");
            return s_Geometry_GetPolygonNumVertices_1(geometry, index, out numvertices);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Geometry_SetPolygonVertex(IntPtr geometry, int index, int vertexindex, ref VECTOR vertex)
        {
            s_Geometry_SetPolygonVertex_1 ??= LoadCoreFunction<Geometry_SetPolygonVertex_Delegate_1>("FMOD5_Geometry_SetPolygonVertex");
            return s_Geometry_SetPolygonVertex_1(geometry, index, vertexindex, ref vertex);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Geometry_GetPolygonVertex(IntPtr geometry, int index, int vertexindex, out VECTOR vertex)
        {
            s_Geometry_GetPolygonVertex_1 ??= LoadCoreFunction<Geometry_GetPolygonVertex_Delegate_1>("FMOD5_Geometry_GetPolygonVertex");
            return s_Geometry_GetPolygonVertex_1(geometry, index, vertexindex, out vertex);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Geometry_SetPolygonAttributes(IntPtr geometry, int index, float directocclusion, float reverbocclusion, bool doublesided)
        {
            s_Geometry_SetPolygonAttributes_1 ??= LoadCoreFunction<Geometry_SetPolygonAttributes_Delegate_1>("FMOD5_Geometry_SetPolygonAttributes");
            return s_Geometry_SetPolygonAttributes_1(geometry, index, directocclusion, reverbocclusion, doublesided);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Geometry_GetPolygonAttributes(IntPtr geometry, int index, out float directocclusion, out float reverbocclusion, out bool doublesided)
        {
            s_Geometry_GetPolygonAttributes_1 ??= LoadCoreFunction<Geometry_GetPolygonAttributes_Delegate_1>("FMOD5_Geometry_GetPolygonAttributes");
            return s_Geometry_GetPolygonAttributes_1(geometry, index, out directocclusion, out reverbocclusion, out doublesided);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Geometry_SetActive(IntPtr geometry, bool active)
        {
            s_Geometry_SetActive_1 ??= LoadCoreFunction<Geometry_SetActive_Delegate_1>("FMOD5_Geometry_SetActive");
            return s_Geometry_SetActive_1(geometry, active);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Geometry_GetActive(IntPtr geometry, out bool active)
        {
            s_Geometry_GetActive_1 ??= LoadCoreFunction<Geometry_GetActive_Delegate_1>("FMOD5_Geometry_GetActive");
            return s_Geometry_GetActive_1(geometry, out active);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Geometry_SetRotation(IntPtr geometry, ref VECTOR forward, ref VECTOR up)
        {
            s_Geometry_SetRotation_1 ??= LoadCoreFunction<Geometry_SetRotation_Delegate_1>("FMOD5_Geometry_SetRotation");
            return s_Geometry_SetRotation_1(geometry, ref forward, ref up);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Geometry_GetRotation(IntPtr geometry, out VECTOR forward, out VECTOR up)
        {
            s_Geometry_GetRotation_1 ??= LoadCoreFunction<Geometry_GetRotation_Delegate_1>("FMOD5_Geometry_GetRotation");
            return s_Geometry_GetRotation_1(geometry, out forward, out up);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Geometry_SetPosition(IntPtr geometry, ref VECTOR position)
        {
            s_Geometry_SetPosition_1 ??= LoadCoreFunction<Geometry_SetPosition_Delegate_1>("FMOD5_Geometry_SetPosition");
            return s_Geometry_SetPosition_1(geometry, ref position);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Geometry_GetPosition(IntPtr geometry, out VECTOR position)
        {
            s_Geometry_GetPosition_1 ??= LoadCoreFunction<Geometry_GetPosition_Delegate_1>("FMOD5_Geometry_GetPosition");
            return s_Geometry_GetPosition_1(geometry, out position);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Geometry_SetScale(IntPtr geometry, ref VECTOR scale)
        {
            s_Geometry_SetScale_1 ??= LoadCoreFunction<Geometry_SetScale_Delegate_1>("FMOD5_Geometry_SetScale");
            return s_Geometry_SetScale_1(geometry, ref scale);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Geometry_GetScale(IntPtr geometry, out VECTOR scale)
        {
            s_Geometry_GetScale_1 ??= LoadCoreFunction<Geometry_GetScale_Delegate_1>("FMOD5_Geometry_GetScale");
            return s_Geometry_GetScale_1(geometry, out scale);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Geometry_Save(IntPtr geometry, IntPtr data, out int datasize)
        {
            s_Geometry_Save_1 ??= LoadCoreFunction<Geometry_Save_Delegate_1>("FMOD5_Geometry_Save");
            return s_Geometry_Save_1(geometry, data, out datasize);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Geometry_SetUserData(IntPtr geometry, IntPtr userdata)
        {
            s_Geometry_SetUserData_1 ??= LoadCoreFunction<Geometry_SetUserData_Delegate_1>("FMOD5_Geometry_SetUserData");
            return s_Geometry_SetUserData_1(geometry, userdata);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Geometry_GetUserData(IntPtr geometry, out IntPtr userdata)
        {
            s_Geometry_GetUserData_1 ??= LoadCoreFunction<Geometry_GetUserData_Delegate_1>("FMOD5_Geometry_GetUserData");
            return s_Geometry_GetUserData_1(geometry, out userdata);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Reverb3D_Release(IntPtr reverb3d)
        {
            s_Reverb3D_Release_1 ??= LoadCoreFunction<Reverb3D_Release_Delegate_1>("FMOD5_Reverb3D_Release");
            return s_Reverb3D_Release_1(reverb3d);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Reverb3D_Set3DAttributes(IntPtr reverb3d, ref VECTOR position, float mindistance, float maxdistance)
        {
            s_Reverb3D_Set3DAttributes_1 ??= LoadCoreFunction<Reverb3D_Set3DAttributes_Delegate_1>("FMOD5_Reverb3D_Set3DAttributes");
            return s_Reverb3D_Set3DAttributes_1(reverb3d, ref position, mindistance, maxdistance);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Reverb3D_Get3DAttributes(IntPtr reverb3d, ref VECTOR position, ref float mindistance, ref float maxdistance)
        {
            s_Reverb3D_Get3DAttributes_1 ??= LoadCoreFunction<Reverb3D_Get3DAttributes_Delegate_1>("FMOD5_Reverb3D_Get3DAttributes");
            return s_Reverb3D_Get3DAttributes_1(reverb3d, ref position, ref mindistance, ref maxdistance);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Reverb3D_SetProperties(IntPtr reverb3d, ref REVERB_PROPERTIES properties)
        {
            s_Reverb3D_SetProperties_1 ??= LoadCoreFunction<Reverb3D_SetProperties_Delegate_1>("FMOD5_Reverb3D_SetProperties");
            return s_Reverb3D_SetProperties_1(reverb3d, ref properties);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Reverb3D_GetProperties(IntPtr reverb3d, ref REVERB_PROPERTIES properties)
        {
            s_Reverb3D_GetProperties_1 ??= LoadCoreFunction<Reverb3D_GetProperties_Delegate_1>("FMOD5_Reverb3D_GetProperties");
            return s_Reverb3D_GetProperties_1(reverb3d, ref properties);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Reverb3D_SetActive(IntPtr reverb3d, bool active)
        {
            s_Reverb3D_SetActive_1 ??= LoadCoreFunction<Reverb3D_SetActive_Delegate_1>("FMOD5_Reverb3D_SetActive");
            return s_Reverb3D_SetActive_1(reverb3d, active);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Reverb3D_GetActive(IntPtr reverb3d, out bool active)
        {
            s_Reverb3D_GetActive_1 ??= LoadCoreFunction<Reverb3D_GetActive_Delegate_1>("FMOD5_Reverb3D_GetActive");
            return s_Reverb3D_GetActive_1(reverb3d, out active);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Reverb3D_SetUserData(IntPtr reverb3d, IntPtr userdata)
        {
            s_Reverb3D_SetUserData_1 ??= LoadCoreFunction<Reverb3D_SetUserData_Delegate_1>("FMOD5_Reverb3D_SetUserData");
            return s_Reverb3D_SetUserData_1(reverb3d, userdata);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RESULT Reverb3D_GetUserData(IntPtr reverb3d, out IntPtr userdata)
        {
            s_Reverb3D_GetUserData_1 ??= LoadCoreFunction<Reverb3D_GetUserData_Delegate_1>("FMOD5_Reverb3D_GetUserData");
            return s_Reverb3D_GetUserData_1(reverb3d, out userdata);
        }

    }
}
#endif
