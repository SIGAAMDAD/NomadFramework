#ifndef __ILOGGER_SERVICE_H__
#define __ILOGGER_SERVICE_H__

#include "nomad_platform.h"

#if defined(NOMAD_PRAGMA_ONCE_SUPPORTED)
	#pragma once
#endif

#include "nomad_core.h"

namespace Nomad::Core
{
	class ILoggerService
	{
	public:
		ILoggerService( void );
		virtual ~ILoggerService() = 0;

		virtual void Print( NomadLoggerLevel eLevel, const char *pszMessage ) = 0;
		virtual void Flush( void ) = 0;
		virtual void AddSink();
	protected:
	};
};

#endif