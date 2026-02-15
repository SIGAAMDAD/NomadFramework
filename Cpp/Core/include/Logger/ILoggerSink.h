#ifndef __ILOGGER_SINK_H__
#define __ILOGGER_SINK_H__

#include "nomad_platform.h"

#if defined(NOMAD_PRAGMA_ONCE_SUPPORTED)
	#pragma once
#endif

namespace Nomad::Core
{
	class ILoggerSink
	{
	public:
		ILoggerSink( void );
		virtual ~ILoggerSink() = 0;

		virtual void AddMessage( const char *pszMessage ) = 0;
		virtual void Flush( void ) = 0;
	};
};

#endif