#ifndef __NOMAD_CORE_H__
#define __NOMAD_CORE_H__

#include "nomad_platform.h"
#include "nomad_compiler.h"

#if defined(NOMAD_PRAGMA_ONCE_SUPPORTED)
	#pragma once
#endif

typedef void (*EventCallback)( const void *pData );

NOMAD_DLL_EXPORT int nomad_core_initialize( void );
NOMAD_DLL_EXPORT void nomad_core_shutdown( void );

NOMAD_DLL_EXPORT void nomad_logger_flush( void );
NOMAD_DLL_EXPORT void nomad_logger_print( int eLevel, const char *pszMessage );
NOMAD_DLL_EXPORT void nomad_logger_printline( int eLevel, const  char *pszMessage );

NOMAD_DLL_EXPORT void *nomad_event_get( int typekey, const char *pszNameSpace, const char *pszName );
NOMAD_DLL_EXPORT void nomad_event_publish( void *event, void *data );

#endif