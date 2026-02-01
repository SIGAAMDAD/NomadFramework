#ifndef __LOGGER_SERVICE_H__
#define __LOGGER_SERVICE_H__

#include "nomad_platform.h"

#if defined(NOMAD_PRAGMA_ONCE_SUPPORTED)
	#pragma once
#endif

#include "ILoggerService.h"

namespace Nomad::Core
{
	class LoggerService : public ILoggerService
	{
	public:
		LoggerService();
		virtual ~LoggerService() override;

		virtual void Flush( void ) override;
	};
};

#endif