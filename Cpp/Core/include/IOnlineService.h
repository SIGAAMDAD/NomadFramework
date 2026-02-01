#ifndef __IONLINE_SERVICE_H__
#define __IONLINE_SERVICE_H__

#include "nomad_platform.h"

#if defined(NOMAD_PRAGMA_ONCE_SUPPORTED)
	#pragma once
#endif

namespace Nomad::Core
{
	class IOnlineService
	{
	public:
		IOnlineService( void );
		virtual ~IOnlineService() = 0;
	private:
	};
};

#endif