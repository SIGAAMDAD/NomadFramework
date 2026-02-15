#ifndef __LOGGER_SINK_H__
#define __LOGGER_SINK_H__

#include "nomad_platform.h"

#if defined(NOMAD_PRAGMA_ONCE_SUPPORTED)
	#pragma once
#endif

#include "ILoggerSink.h"

namespace Nomad::Core
{
	class LoggerSink : public ILoggerSink
	{
	public:
		LoggerSink( void );
		virtual ~LoggerSink() override;

		virtual void AddMessage( const char *pszMessage ) override;
		virtual void Flush( void ) override;
	};
};

#endif